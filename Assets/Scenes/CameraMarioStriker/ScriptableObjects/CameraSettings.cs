using UnityEngine;

[CreateAssetMenu(fileName = "CameraSettings_Match", menuName = "Strikers/Camera/Camera Settings")]
public class CameraSettings : ScriptableObject
{
    [Header("Angle (fixe)")]
    [Range(20f, 80f)] public float pitch = 45f;
    public float yaw = 0f;

    [Header("Distance (fixe) : plan large, aucun zoom dynamique")]
    [Min(1f)] public float distance = 70f;
    [Tooltip("Si activé, la distance est plafonnée pour que la vue tienne dans l'arène (plan plus serré).")]
    public bool capDistanceToArena = false;

    [Header("Arène (axe long = X, axe court = Z)")]
    public Vector3 arenaCenter = Vector3.zero;
    public Vector2 arenaSize = new Vector2(60f, 36f);
    [Tooltip("Vide (unités monde) toléré au-delà de l'arène. 0 = aucun vide.")]
    [Min(0f)] public float arenaViewOverscan = 0f;
    [Tooltip("Hauteur du viewport (0-1) jusqu'où on interdit de voir du vide. Baisse-le pour tolérer du vide / mur du fond en haut de l'écran.")]
    [Range(0.3f, 1f)] public float clampViewportTop = 0.6f;
    [Tooltip("Distance (unités monde) avant les limites sur laquelle la caméra ralentit progressivement. Plus haut = approche des murs plus douce. 0 = blocage net (brusque).")]
    [Min(0f)] public float arenaSoftEdge = 15f;
    [Range(0f, 0.4f)] public float clampViewportSide = 0f;

    [Header("Cadrage : on suit la zone de jeu (joueurs), pas la balle")]
    [Tooltip("0 = la caméra suit le milieu des joueurs. 1 = elle suit la balle. ~0.6-0.7 pour un vrai follow.")]
    [Range(0f, 1f)] public float ballInfluence = 0.65f;
    [Tooltip("1 = suit pleinement sur l'axe de profondeur. 0 = reste centré sur l'arène sur cet axe. Le mouvement vertical à l'écran est le plus inconfortable : garde-le modéré.")]
    [Range(0f, 1f)] public float depthFollow = 0.5f;

    [Header("Lissage en 2 étages (confort)")]
    [Tooltip("Étage 1 : filtre la cible (rebonds, zigzags). Plus haut = plus doux, mais plus de retard.")]
    [Min(0.01f)] public float anchorSmoothTime = 0.2f;

    [Header("Dead zone (la caméra ne bouge pas tant que l'action reste dedans)")]
    [Tooltip("Demi-taille de la dead zone en fraction du viewport (x = largeur, y = hauteur).")]
    public Vector2 deadZoneViewport = new Vector2(0.04f, 0.03f);
    [Tooltip("Étage 2 : suivi de l'objectif. 0.2-0.4 pour un follow lissé.")]
    [Min(0.01f)] public float followSmoothTime = 0.3f;
    [Min(1f)] public float maxFollowSpeed = 60f;

    [Header("Follow adaptatif à la vitesse de la balle")]
    [Tooltip("Vitesse de la balle (unités/s) considérée comme 'maximale'. Mets la vraie valeur de ton jeu.")]
    [Min(0.1f)] public float ballMaxSpeed = 100f;
    [Tooltip("Lissage de la vitesse mesurée (plus bas = transitions plus douces).")]
    [Min(0.1f)] public float speedSmoothing = 6f;
    [Tooltip("Réponse à la vitesse : 0 = balle lente, 1 = balle à ballMaxSpeed.")]
    public AnimationCurve speedResponse = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [Tooltip("Temps de lissage de la cible (étage 1) quand la balle est à vitesse max. Plus haut que anchorSmoothTime.")]
    [Min(0.01f)] public float anchorSmoothTimeAtMaxSpeed = 0.35f;
    [Tooltip("Temps de suivi (étage 2) quand la balle est à vitesse max. Plus haut que followSmoothTime.")]
    [Min(0.01f)] public float followSmoothTimeAtMaxSpeed = 0.7f;
    [Tooltip("Influence de la balle sur le cadrage à vitesse max. Plus bas = on s'appuie plus sur les joueurs.")]
    [Range(0f, 1f)] public float ballInfluenceAtMaxSpeed = 0.35f;

    [Header("Safe zone (joueurs et balle toujours visibles)")]
    [Range(0f, 0.3f)] public float safeZoneMargin = 0.1f;
    [Tooltip("Douceur de la poussée (plus bas = plus doux). Elle devient totale au bord de l'écran.")]
    [Min(0.1f)] public float safeZoneSharpness = 4f;
    [Tooltip("Vitesse max (unités/s) de la poussée de la safe zone. Bas = jamais brusque, mais la balle peut frôler le bord sur un tir très rapide. 0 = illimité (peut téléporter).")]
    [Min(0f)] public float safeZoneMaxSpeed = 30f;

    [Header("Événements (seul dynamisme de la caméra)")]
    [Tooltip("Multiplicateur global des effets d'événements. 0 = aucun (option d'accessibilité / motion sickness).")]
    [Range(0f, 1f)] public float eventIntensity = 1f;
    [Tooltip("Décalage max du shake (unités monde). Translation seulement, très court.")]
    [Min(0f)] public float shakeMaxOffset = 0.35f;
    [Min(0.1f)] public float shakeFrequency = 22f;
    [Tooltip("Plus haut = shake plus court.")]
    [Min(0.1f)] public float traumaDecay = 3.5f;
    [Tooltip("Coup de caméra dans la direction du hit (unités monde).")]
    [Min(0f)] public float kickDistance = 0.5f;
    [Min(0.5f)] public float impulseFrequency = 4f;
    [Range(0.2f, 1f)] public float impulseDamping = 0.6f;
    [Tooltip("Vitesse de transition vers/depuis un plan spécial (KO, spécial).")]
    [Min(0.05f)] public float shotSmoothTime = 0.3f;

#if UNITY_EDITOR
    [ContextMenu("Apply semi-dynamic preset")]
    void ApplyCalmPreset()
    {
        UnityEditor.Undo.RecordObject(this, "Apply camera preset");
        // Ne touche pas à : pitch, yaw, distance, arène.
        ballInfluence = 0.65f;
        depthFollow = 0.5f;
        anchorSmoothTime = 0.2f;
        deadZoneViewport = new Vector2(0.04f, 0.03f);
        followSmoothTime = 0.3f;
        maxFollowSpeed = 60f;
        safeZoneMargin = 0.1f;
        safeZoneSharpness = 4f;
        safeZoneMaxSpeed = 30f;
        speedSmoothing = 6f;
        speedResponse = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        anchorSmoothTimeAtMaxSpeed = 0.35f;
        followSmoothTimeAtMaxSpeed = 0.7f;
        ballInfluenceAtMaxSpeed = 0.35f;
        clampViewportTop = 0.6f;
        arenaSoftEdge = 15f;
        capDistanceToArena = false;
        eventIntensity = 1f;
        shakeMaxOffset = 0.35f;
        shakeFrequency = 22f;
        traumaDecay = 3.5f;
        kickDistance = 0.5f;
        impulseFrequency = 4f;
        impulseDamping = 0.6f;
        shotSmoothTime = 0.3f;
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif

    void OnValidate()
    {
        deadZoneViewport.x = Mathf.Clamp(deadZoneViewport.x, 0f, 0.45f);
        deadZoneViewport.y = Mathf.Clamp(deadZoneViewport.y, 0f, 0.45f);
    }
}