using UnityEngine;

/// <summary>
/// Caméra Strikers, volontairement calme (motion sickness) :
///  - angle et distance fixes, translation seule
///  - suit le milieu des joueurs, tiré vers la balle (ballInfluence)
///  - suivi plus posé quand la balle est rapide
///  - ne montre jamais le vide de l'arène, ralentit en douceur près des murs
///  - joueurs et balle gardés dans l'écran (safe zone douce)
///  - dynamisme par événements : Punch (hit) et PlayShot (KO, spécial, but)
/// Caméra perspective uniquement.
/// </summary>
[RequireComponent(typeof(Camera))]
public class StrikersCameraRig : MonoBehaviour
{
    [SerializeField] CameraSettings settings;
    [SerializeField] bool debugOverlay;

    public static StrikersCameraRig Current { get; private set; }

    // ---- Constantes internes (réglées une fois, volontairement pas dans l'inspector) ----
    const float AnchorShare = 0.4f;             // part du temps de suivi pour le filtre de cible (étage 1)
    const float SpeedAttack = 15f;              // passage rapide en mode "balle rapide"
    const float SpeedRelease = 2.5f;            // retour lent en mode "balle lente"
    const float SafeMargin = 0.1f;              // marge du viewport pour la safe zone
    const float SafeSharpness = 4f;             // douceur de la safe zone
    const float SafeSpeedRatio = 0.5f;          // vitesse max de la safe zone, relative à la vitesse max de la caméra
    const float MaxCameraSpeedRatio = 0.6f;     // vitesse max de la caméra = ratio x ballMaxSpeed
    const float ClampViewTop = 0.6f;            // hauteur du viewport testée contre le vide d'arène
    const float ArenaSoftEdgeRatio = 0.15f;     // zone de ralentissement près des murs (x petit côté de l'arène)
    static readonly Vector2 DeadZone = new Vector2(0.04f, 0.03f); // demi-taille (viewport) : évite les micro-mouvements
    const float ShakeFrequency = 22f;
    const float TraumaDecay = 3.5f;
    const float ImpulseFrequency = 4f;
    const float ImpulseDamping = 0.6f;
    const float ShotSmoothTime = 0.3f;

    Camera cam;
    bool initialized;

    // Simulation
    Vector3 focus, focusVelocity;
    Vector3 goal;
    Vector3 anchorSmoothed, anchorVelocity;
    float distanceScale = 1f, distanceScaleVelocity;
    float speedRaw, speedSmoothed;
    Vector3 lastBallPos;
    bool hasLastBallPos;
    float limLoX, limHiX, limLoZ, limHiZ;

    // Plan spécial
    bool shotActive;
    Vector3 shotPoint;
    float shotTimer;
    float shotDistanceScale = 1f;

    // Impulsions (temps non scalé : le hitstop ne fige pas la caméra)
    float trauma, noiseSeed;
    Vector3 kickOffset, kickVelocity;

    // Debug
    Vector3 dbgPrevFocus;
    float dbgFocusSpeed, dbgPeak, dbgPeakTimer, dbgSpeedT;

    void Awake()
    {
        cam = GetComponent<Camera>();
        noiseSeed = Random.value * 100f;
        if (cam.orthographic)
            Debug.LogWarning("StrikersCameraRig : caméra orthographique non supportée.", this);
    }

    void OnEnable() { Current = this; }
    void OnDisable() { if (Current == this) Current = null; }

    // ------------------------------------------------------------------
    // API publique (événements)
    // ------------------------------------------------------------------

    /// <summary>Hit : strength 0-1 (ex : dégâts normalisés), direction = sens du coup (monde). Translation seulement.</summary>
    public void Punch(float strength, Vector3 worldDirection)
    {
        if (settings == null) return;
        strength = Mathf.Clamp01(strength) * settings.eventIntensity;
        if (strength <= 0f) return;

        float omega = 2f * Mathf.PI * ImpulseFrequency;
        trauma = Mathf.Clamp01(trauma + strength);

        worldDirection.y = 0f;
        if (worldDirection.sqrMagnitude > 1e-4f)
            kickVelocity += worldDirection.normalized * (settings.kickAmount * strength * omega);
    }

    /// <summary>
    /// Plan spécial (KO, spécial, but) : la caméra se cale sur worldPoint pendant duration secondes puis revient.
    /// distanceScale = 1 : aucun zoom. Ignoré si eventIntensity = 0.
    /// </summary>
    public void PlayShot(Vector3 worldPoint, float duration, float distanceScale = 1f)
    {
        if (settings == null || settings.eventIntensity <= 0f) return;
        shotActive = true;
        shotPoint = worldPoint;
        shotTimer = duration;
        shotDistanceScale = Mathf.Lerp(1f, Mathf.Clamp(distanceScale, 0.5f, 1.5f), settings.eventIntensity);
    }

    public void EndShot() { shotActive = false; }

    [ContextMenu("Test Punch")]
    void TestPunch() { Punch(0.8f, transform.right); }

    // ------------------------------------------------------------------
    // Boucle principale
    // ------------------------------------------------------------------

    void LateUpdate()
    {
        if (settings == null) return;
        CameraTarget ball = CameraTarget.Primary;
        if (ball == null) return;

        UpdateImpulses(Mathf.Min(Time.unscaledDeltaTime, 0.05f));

        float dt = Time.deltaTime;
        if (!initialized) Initialize(ball);
        else if (dt > 0f) Simulate(ball, dt);

        ApplyTransform();
    }

    void Initialize(CameraTarget ball)
    {
        focusVelocity = Vector3.zero;
        distanceScale = 1f; distanceScaleVelocity = 0f;
        speedRaw = speedSmoothed = 0f;
        hasLastBallPos = false;

        goal = ComputeAnchor(ball);
        anchorSmoothed = goal;
        anchorVelocity = Vector3.zero;
        focus = goal;
        dbgPrevFocus = focus;
        ConstrainToArena();
        initialized = true;
    }

    void Simulate(CameraTarget ball, float dt)
    {
        // Vitesse mesurée sur le déplacement réel de la balle : 0 = lente, 1 = ballMaxSpeed
        Vector3 bp = ball.Position;
        bp.y = 0f;
        float instant = hasLastBallPos ? (bp - lastBallPos).magnitude / dt : 0f;
        lastBallPos = bp;
        hasLastBallPos = true;
        if (instant > settings.ballMaxSpeed * 3f) instant = speedRaw; // reset / téléportation : ignoré
        speedRaw = Mathf.Lerp(speedRaw, instant, 1f - Mathf.Exp(-25f * dt)); // retire le bruit des pas de physique

        // Attaque rapide, relâchement lent
        float rate = speedRaw > speedSmoothed ? SpeedAttack : SpeedRelease;
        speedSmoothed = Mathf.Lerp(speedSmoothed, speedRaw, 1f - Mathf.Exp(-rate * dt));
        float speedT = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(speedSmoothed / settings.ballMaxSpeed));
        dbgSpeedT = speedT;

        // Balle lente = suivi réactif. Balle rapide = suivi plus posé. Deux étages : filtre de cible puis suivi.
        float total = Mathf.Lerp(settings.followSmoothTime, settings.followSmoothTimeFastBall, speedT);
        float anchorTime = total * AnchorShare;
        float stageTime = total * (1f - AnchorShare);

        if (shotActive)
        {
            shotTimer -= dt;
            if (shotTimer <= 0f) shotActive = false;
        }

        // Étage 1 : cible filtrée, ralentie en douceur près des murs
        ComputeLimits();
        Vector3 rawAnchor = SoftLimitAnchor(ComputeAnchor(ball));
        anchorSmoothed = Vector3.SmoothDamp(anchorSmoothed, rawAnchor, ref anchorVelocity, anchorTime, Mathf.Infinity, dt);

        if (shotActive)
        {
            goal = shotPoint;
            goal.y = settings.arenaCenter.y;
            stageTime = ShotSmoothTime;
        }
        else
        {
            UpdateDeadZoneGoal(anchorSmoothed);
        }

        float targetScale = shotActive ? shotDistanceScale : 1f;
        distanceScale = Mathf.SmoothDamp(distanceScale, targetScale, ref distanceScaleVelocity, ShotSmoothTime, Mathf.Infinity, dt);

        ConstrainToArena(); // l'objectif doit être valide

        // Étage 2 : la caméra suit l'objectif (sans dépassement, vitesse plafonnée)
        float maxSpeed = settings.ballMaxSpeed * MaxCameraSpeedRatio;
        focus = Vector3.SmoothDamp(focus, goal, ref focusVelocity, stageTime, maxSpeed, dt);
        focus.y = settings.arenaCenter.y;

        // Safe zone douce, vitesse limitée : joueurs d'abord, balle en dernier (prioritaire)
        float softK = 1f - Mathf.Exp(-SafeSharpness * dt);
        float budget = maxSpeed * SafeSpeedRatio * dt;
        var list = CameraTarget.All;
        for (int i = 0; i < list.Count; i++)
        {
            CameraTarget t = list[i];
            if (t == null || t == ball) continue;
            PushIntoSafeZone(t.Position, softK, ref budget);
        }
        PushIntoSafeZone(ball.Position, softK, ref budget);

        ConstrainToArena();

        // Debug
        dbgFocusSpeed = (focus - dbgPrevFocus).magnitude / dt;
        dbgPrevFocus = focus;
        dbgPeakTimer -= dt;
        if (dbgFocusSpeed >= dbgPeak || dbgPeakTimer <= 0f) { dbgPeak = dbgFocusSpeed; dbgPeakTimer = 2f; }
    }

    void OnGUI()
    {
        if (!debugOverlay || !initialized || settings == null) return;
        GUI.Label(new Rect(10, 10, 600, 70),
            "camera speed " + dbgFocusSpeed.ToString("F1") + "  (peak 2s: " + dbgPeak.ToString("F1") + ")\n" +
            "ball speed " + speedSmoothed.ToString("F1") + "  -> speedT " + dbgSpeedT.ToString("F2"));
    }

    // ------------------------------------------------------------------
    // Cadrage
    // ------------------------------------------------------------------

    /// <summary>Point de la zone de jeu : milieu (pondéré) des joueurs, tiré vers la balle.</summary>
    Vector3 ComputeAnchor(CameraTarget ball)
    {
        Vector3 sum = Vector3.zero;
        float w = 0f;
        var list = CameraTarget.All;
        for (int i = 0; i < list.Count; i++)
        {
            CameraTarget t = list[i];
            if (t == null || t == ball) continue;
            sum += t.Position * t.Weight;
            w += t.Weight;
        }

        Vector3 players = w > 1e-4f ? sum / w : ball.Position;
        Vector3 a = Vector3.Lerp(players, ball.Position, settings.ballInfluence);
        a.y = settings.arenaCenter.y;

        // depthFollow : atténue le suivi sur l'axe de profondeur
        Vector3 depthAxis = FlatForward();
        float depthOffset = Vector3.Dot(a - settings.arenaCenter, depthAxis);
        a -= depthAxis * (depthOffset * (1f - settings.depthFollow));
        return a;
    }

    /// <summary>L'objectif ne bouge que du dépassement : tant que l'ancre reste dans la dead zone, rien ne change.</summary>
    void UpdateDeadZoneGoal(Vector3 anchor)
    {
        GetDeadZoneHalfExtents(out float hr, out float hd);
        Vector3 right = FlatRight();
        Vector3 fwd = FlatForward();

        Vector3 d = anchor - goal;
        float r = Vector3.Dot(d, right);
        float f = Vector3.Dot(d, fwd);

        goal += right * (r - Mathf.Clamp(r, -hr, hr)) + fwd * (f - Mathf.Clamp(f, -hd, hd));
        goal.y = settings.arenaCenter.y;
    }

    void GetDeadZoneHalfExtents(out float halfRight, out float halfDepth)
    {
        float dist = CurrentDistance();
        Vector3 right = FlatRight();
        Vector3 fwd = FlatForward();

        Vector2 c = GroundOffset(new Vector2(0.5f, 0.5f), dist);
        Vector2 r = GroundOffset(new Vector2(0.5f + DeadZone.x, 0.5f), dist) - c;
        Vector2 u = GroundOffset(new Vector2(0.5f, 0.5f + DeadZone.y), dist) - c;

        halfRight = Mathf.Abs(r.x * right.x + r.y * right.z);
        halfDepth = Mathf.Abs(u.x * fwd.x + u.y * fwd.z);
    }

    /// <summary>Cible dans la marge du viewport : poussée douce (totale au bord de l'écran), vitesse limitée. Décale focus ET objectif.</summary>
    void PushIntoSafeZone(Vector3 worldPos, float softK, ref float budget)
    {
        if (budget <= 0f) return;
        float m = SafeMargin;

        for (int i = 0; i < 2; i++)
        {
            Vector2 vp = ProjectToViewport(worldPos);
            float ox = vp.x < m ? vp.x - m : (vp.x > 1f - m ? vp.x - (1f - m) : 0f);
            float oy = vp.y < m ? vp.y - m : (vp.y > 1f - m ? vp.y - (1f - m) : 0f);
            if (ox == 0f && oy == 0f) break;

            float tx = Mathf.Clamp01(Mathf.Abs(ox) / m);
            float ty = Mathf.Clamp01(Mathf.Abs(oy) / m);
            ox *= Mathf.Lerp(softK, 1f, tx * tx);
            oy *= Mathf.Lerp(softK, 1f, ty * ty);

            float dist = CurrentDistance();
            Vector2 a = GroundOffset(new Vector2(0.5f, 0.5f), dist);
            Vector2 b = GroundOffset(new Vector2(0.5f + ox, 0.5f + oy), dist);
            Vector2 d = b - a;

            // Pas de poussée là où l'arène l'interdit
            d.x = Mathf.Clamp(focus.x + d.x, limLoX, limHiX) - focus.x;
            d.y = Mathf.Clamp(focus.z + d.y, limLoZ, limHiZ) - focus.z;

            // Vitesse de poussée limitée : pas de téléportation
            float mag = d.magnitude;
            if (mag > budget) d *= budget / mag;
            budget -= d.magnitude;

            focus.x += d.x; focus.z += d.y;
            goal.x += d.x; goal.z += d.y;
            if (budget <= 0f) break;
        }
    }

    // ------------------------------------------------------------------
    // Arène : jamais de vide, approche des murs en douceur
    // ------------------------------------------------------------------

    float CurrentDistance() { return Mathf.Max(1f, settings.distance * distanceScale); }

    /// <summary>Plage atteignable par le focus pour que la vue reste dans l'arène (lo <= hi garanti).</summary>
    void ComputeLimits()
    {
        GetFootprint(CurrentDistance(), out Vector2 min, out Vector2 max);
        Vector2 half = settings.arenaSize * 0.5f;
        Vector3 c = settings.arenaCenter;

        limLoX = c.x - half.x - min.x; limHiX = c.x + half.x - max.x;
        limLoZ = c.z - half.y - min.y; limHiZ = c.z + half.y - max.y;

        if (limLoX > limHiX) limLoX = limHiX = (limLoX + limHiX) * 0.5f;
        if (limLoZ > limHiZ) limLoZ = limHiZ = (limLoZ + limHiZ) * 0.5f;
    }

    /// <summary>Ralentit la cible en approchant des limites (continu en vitesse) : pas de blocage net contre les murs.</summary>
    Vector3 SoftLimitAnchor(Vector3 a)
    {
        float soft = Mathf.Min(settings.arenaSize.x, settings.arenaSize.y) * ArenaSoftEdgeRatio;
        a.x = SoftLimit(a.x, limLoX, limHiX, soft);
        a.z = SoftLimit(a.z, limLoZ, limHiZ, soft);
        return a;
    }

    static float SoftLimit(float v, float lo, float hi, float soft)
    {
        if (hi - lo < 1e-4f) return lo;
        soft = Mathf.Min(soft, (hi - lo) * 0.5f);
        if (soft < 1e-4f) return Mathf.Clamp(v, lo, hi);

        float a = lo + soft, b = hi - soft;
        if (v > b) return b + soft * (1f - Mathf.Exp(-(v - b) / soft));
        if (v < a) return a - soft * (1f - Mathf.Exp(-(a - v) / soft));
        return v;
    }

    void ConstrainToArena()
    {
        ComputeLimits();

        ClampAxis(ref focus.x, ref focusVelocity.x, limLoX, limHiX);
        ClampAxis(ref focus.z, ref focusVelocity.z, limLoZ, limHiZ);

        float unused = 0f;
        ClampAxis(ref goal.x, ref unused, limLoX, limHiX);
        ClampAxis(ref goal.z, ref unused, limLoZ, limHiZ);
    }

    static void ClampAxis(ref float value, ref float velocity, float lo, float hi)
    {
        if (value < lo) { value = lo; if (velocity < 0f) velocity = 0f; }
        else if (value > hi) { value = hi; if (velocity > 0f) velocity = 0f; }
    }

    // ------------------------------------------------------------------
    // Géométrie de la vue (analytique)
    // ------------------------------------------------------------------

    Quaternion BaseRotation() { return Quaternion.Euler(settings.pitch, settings.yaw, 0f); }
    Vector3 FlatForward() { return Quaternion.Euler(0f, settings.yaw, 0f) * Vector3.forward; }
    Vector3 FlatRight() { return Quaternion.Euler(0f, settings.yaw, 0f) * Vector3.right; }

    /// <summary>Décalage XZ (par rapport au focus) du point du sol vu en viewport uv.</summary>
    Vector2 GroundOffset(Vector2 uv, float dist)
    {
        Quaternion rot = BaseRotation();
        float tanV = Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float tanH = tanV * cam.aspect;

        Vector3 local = new Vector3((uv.x * 2f - 1f) * tanH, (uv.y * 2f - 1f) * tanV, 1f);
        Vector3 dir = (rot * local).normalized;
        Vector3 camOffset = -(rot * Vector3.forward) * dist;

        float dy = Mathf.Min(dir.y, -0.02f); // garde-fou : rayon au-dessus de l'horizon
        float t = -camOffset.y / dy;
        Vector3 hit = camOffset + dir * t;
        return new Vector2(hit.x, hit.z);
    }

    Vector2 ProjectToViewport(Vector3 worldPos)
    {
        Quaternion rot = BaseRotation();
        Vector3 camPos = focus - (rot * Vector3.forward) * CurrentDistance();
        Vector3 local = Quaternion.Inverse(rot) * (worldPos - camPos);

        float tanV = Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float tanH = tanV * cam.aspect;
        float z = Mathf.Max(local.z, 0.01f);
        return new Vector2(local.x / (z * tanH) * 0.5f + 0.5f, local.y / (z * tanV) * 0.5f + 0.5f);
    }

    void GetFootprint(float dist, out Vector2 min, out Vector2 max)
    {
        Vector2 c0 = GroundOffset(new Vector2(0f, 0f), dist);
        Vector2 c1 = GroundOffset(new Vector2(1f, 0f), dist);
        Vector2 c2 = GroundOffset(new Vector2(0f, ClampViewTop), dist);
        Vector2 c3 = GroundOffset(new Vector2(1f, ClampViewTop), dist);

        min = Vector2.Min(Vector2.Min(c0, c1), Vector2.Min(c2, c3));
        max = Vector2.Max(Vector2.Max(c0, c1), Vector2.Max(c2, c3));
    }

    // ------------------------------------------------------------------
    // Impulsions & application
    // ------------------------------------------------------------------

    void UpdateImpulses(float rawDt)
    {
        if (rawDt <= 0f) return;
        float omega = 2f * Mathf.PI * ImpulseFrequency;
        Spring(ref kickOffset, ref kickVelocity, Vector3.zero, omega, ImpulseDamping, rawDt);
        trauma = Mathf.MoveTowards(trauma, 0f, TraumaDecay * rawDt);
    }

    Vector3 ShakeOffset(Quaternion rot)
    {
        float s = trauma * trauma;
        if (s <= 0f) return Vector3.zero;

        float t = Time.unscaledTime * ShakeFrequency;
        float x = Mathf.PerlinNoise(noiseSeed, t) * 2f - 1f;
        float y = Mathf.PerlinNoise(noiseSeed + 17.3f, t) * 2f - 1f;
        return (rot * Vector3.right * x + rot * Vector3.up * y) * (s * settings.shakeAmount);
    }

    void ApplyTransform()
    {
        Quaternion rot = BaseRotation();
        Vector3 pos = focus - rot * Vector3.forward * CurrentDistance();
        pos += kickOffset + ShakeOffset(rot);

        transform.rotation = rot; // pas de roll, pas de yaw dynamique
        transform.position = pos;
    }

    static void Spring(ref Vector3 x, ref Vector3 v, Vector3 target, float omega, float zeta, float dt)
    {
        float f = 1f + 2f * dt * zeta * omega;
        float hoo = dt * omega * omega;
        float hhoo = dt * hoo;
        float detInv = 1f / (f + hhoo);
        Vector3 detX = f * x + dt * v + hhoo * target;
        Vector3 detV = v + hoo * (target - x);
        x = detX * detInv;
        v = detV * detInv;
    }

    // ------------------------------------------------------------------
    // Gizmos
    // ------------------------------------------------------------------

    void OnDrawGizmosSelected()
    {
        if (settings == null) return;

        Vector3 c = settings.arenaCenter;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(c, new Vector3(settings.arenaSize.x, 0.1f, settings.arenaSize.y));

        if (!Application.isPlaying || !initialized) return;
        if (cam == null) cam = GetComponent<Camera>();

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(focus, 0.6f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(goal, 0.4f);

        GetDeadZoneHalfExtents(out float hr, out float hd);
        Vector3 r = FlatRight() * hr, f = FlatForward() * hd;
        Gizmos.color = new Color(1f, 0.55f, 0f);
        Gizmos.DrawLine(goal - r - f, goal + r - f);
        Gizmos.DrawLine(goal + r - f, goal + r + f);
        Gizmos.DrawLine(goal + r + f, goal - r + f);
        Gizmos.DrawLine(goal - r + f, goal - r - f);

        Vector2[] uvs = { new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(1f, ClampViewTop), new Vector2(0f, ClampViewTop) };
        Gizmos.color = Color.green;
        float dist = CurrentDistance();
        for (int i = 0; i < 4; i++)
        {
            Vector2 a = GroundOffset(uvs[i], dist);
            Vector2 b = GroundOffset(uvs[(i + 1) % 4], dist);
            Gizmos.DrawLine(new Vector3(focus.x + a.x, c.y, focus.z + a.y), new Vector3(focus.x + b.x, c.y, focus.z + b.y));
        }
    }
}