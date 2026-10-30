using UnityEngine;

/// <summary>
/// Caméra "par zones" (itération 3).
/// La caméra a 3 positions (gauche, centre, droite) et glisse en douceur de l'une à l'autre selon la position de la balle.
/// Quand la balle est très rapide, la caméra se recentre peu à peu pour ne pas bouger sans arrêt.
/// Ce script ne modifie aucun autre script : la balle est donnée dans l'Inspector, et sa vitesse est mesurée ici.
/// À placer sur l'objet Camera de la scène.
/// </summary>
public class BallZonesCamera : MonoBehaviour
{
    /// <summary>
    /// Les 3 zones possibles de la caméra.
    /// </summary>
    private enum CameraZone
    {
        Left,
        Center,
        Right
    }

    #region Réglages

    [Header("Réglages")]

    [Tooltip("L'asset BallZonesCameraSettings qui contient tous les réglages de cette caméra.")]
    [SerializeField] private BallZonesCameraSettings _settings;

    #endregion

    #region Éléments à cadrer

    [Header("Éléments à cadrer")]

    [Tooltip("Objet vide placé au centre de l'arène. S'il est vide, le centre est (0, 0, 0).")]
    [SerializeField] private Transform _arenaCenterPoint;

    [Tooltip("La balle. Obligatoire : cette caméra se décide selon la position et la vitesse de la balle.")]
    [SerializeField] private Transform _ball;

    #endregion

    #region Valeurs internes

    // Zone actuelle de la caméra.
    private CameraZone _currentZone = CameraZone.Center;
    // Point que la caméra regarde, après lissage.
    private Vector3 _smoothedLookPoint;
    // Vitesse actuelle du point regardé (utilisée par SmoothDamp, ne pas modifier à la main).
    private Vector3 _lookPointVelocity;
    // Position de la balle à la frame précédente, pour mesurer sa vitesse.
    private Vector3 _lastBallPosition;
    // Vitesse de la balle, mesurée puis lissée.
    private float _smoothedBallSpeed;
    // Vitesse actuelle du lissage de la vitesse de la balle (utilisée par SmoothDamp, ne pas modifier à la main).
    private float _ballSpeedVelocity;

    #endregion

    /// <summary>
    /// Appelé à chaque fois que le script est activé (au lancement, ou quand tu recoches le script dans l'Inspector).
    /// Place la caméra directement au bon endroit pour qu'elle ne glisse pas depuis la position laissée par une autre caméra.
    /// Ce script déplace l'objet sur lequel il est posé : il refuse donc de fonctionner s'il n'est pas sur un objet Camera.
    /// </summary>
    private void OnEnable()
    {
        if (GetComponent<Camera>() == null)
        {
            Debug.LogWarning("BallZonesCamera : ce script doit être posé sur l'objet Camera, pas sur un autre objet. Le script est désactivé.", this);
            enabled = false;
            return;
        }

        if (_settings == null)
        {
            Debug.LogWarning("BallZonesCamera : aucun BallZonesCameraSettings n'est assigné. La caméra est désactivée.", this);
            enabled = false;
            return;
        }

        if (_ball == null)
        {
            Debug.LogWarning("BallZonesCamera : la balle n'est pas assignée. La caméra est désactivée.", this);
            enabled = false;
            return;
        }

        _lastBallPosition = _ball.position;
        _smoothedBallSpeed = 0f;
        _ballSpeedVelocity = 0f;
        _lookPointVelocity = Vector3.zero;
        _currentZone = ChooseZoneFromBallPosition(CameraZone.Center);
        _smoothedLookPoint = ComputeTargetLookPoint();
        ApplyCameraTransform();
    }

    /// <summary>
    /// Met à jour la caméra après que tous les objets ont bougé dans la frame.
    /// </summary>
    private void LateUpdate()
    {
        UpdateBallSpeed();
        _currentZone = ChooseZoneFromBallPosition(_currentZone);
        MoveLookPoint();
        ApplyCameraTransform();
    }

    #region Vitesse de la balle

    /// <summary>
    /// Mesure la vitesse de la balle en comparant sa position avec celle de la frame précédente, puis la lisse.
    /// La vitesse mesurée ne dépasse jamais la vitesse maximale des réglages (utile si la balle est téléportée).
    /// </summary>
    private void UpdateBallSpeed()
    {
        float measuredSpeed = 0f;

        if (Time.deltaTime > 0f)
        {
            measuredSpeed = (_ball.position - _lastBallPosition).magnitude / Time.deltaTime;
        }

        measuredSpeed = Mathf.Min(measuredSpeed, _settings.ballMaxSpeed);
        _lastBallPosition = _ball.position;

        _smoothedBallSpeed = Mathf.SmoothDamp(
            _smoothedBallSpeed,
            measuredSpeed,
            ref _ballSpeedVelocity,
            _settings.ballSpeedSmoothTime);
    }

    /// <summary>
    /// Calcule à quel point la caméra doit se recentrer à cause de la vitesse de la balle.
    /// </summary>
    /// <returns>0 = balle lente, la caméra reste dans sa zone. 1 = balle à vitesse max, la caméra est complètement au centre.</returns>
    private float ComputeRecenterAmount()
    {
        float speedRatio = Mathf.Clamp01(_smoothedBallSpeed / _settings.ballMaxSpeed);

        return Mathf.InverseLerp(_settings.recenterStartSpeedRatio, 1f, speedRatio);
    }

    #endregion

    #region Choix de la zone

    /// <summary>
    /// Choisit la zone de la caméra selon la position de la balle sur l'axe X.
    /// La balle doit dépasser la limite de zone d'une marge pour changer de zone (cela évite les allers-retours).
    /// </summary>
    /// <param name="currentZone">La zone actuelle de la caméra.</param>
    /// <returns>La zone à utiliser (la même que currentZone si la balle n'a pas assez dépassé la limite).</returns>
    private CameraZone ChooseZoneFromBallPosition(CameraZone currentZone)
    {
        float ballOffsetX = _ball.position.x - GetArenaCenter().x;
        float enterSideLimit = _settings.zoneLimitDistance + _settings.zoneChangeMargin;
        float returnToCenterLimit = _settings.zoneLimitDistance - _settings.zoneChangeMargin;

        if (ballOffsetX > enterSideLimit)
        {
            return CameraZone.Right;
        }

        if (ballOffsetX < -enterSideLimit)
        {
            return CameraZone.Left;
        }

        if (currentZone == CameraZone.Right && ballOffsetX < returnToCenterLimit)
        {
            return CameraZone.Center;
        }

        if (currentZone == CameraZone.Left && ballOffsetX > -returnToCenterLimit)
        {
            return CameraZone.Center;
        }

        return currentZone;
    }

    /// <summary>
    /// Donne le décalage de la caméra sur l'axe X pour une zone donnée.
    /// </summary>
    /// <param name="zone">La zone dont on veut le décalage.</param>
    /// <returns>Négatif pour la gauche, 0 pour le centre, positif pour la droite.</returns>
    private float GetZoneOffsetX(CameraZone zone)
    {
        if (zone == CameraZone.Left)
        {
            return -_settings.sideZoneOffset;
        }

        if (zone == CameraZone.Right)
        {
            return _settings.sideZoneOffset;
        }

        return 0f;
    }

    #endregion

    #region Déplacement de la caméra

    /// <summary>
    /// Calcule le point que la caméra voudrait regarder (avant lissage).
    /// On part de la position de la zone actuelle, puis on la ramène vers le centre si la balle est rapide.
    /// </summary>
    /// <returns>Le point voulu, en position du monde.</returns>
    private Vector3 ComputeTargetLookPoint()
    {
        float zoneOffsetX = GetZoneOffsetX(_currentZone);
        float offsetXWithRecenter = Mathf.Lerp(zoneOffsetX, 0f, ComputeRecenterAmount());

        return GetArenaCenter() + Vector3.right * offsetXWithRecenter;
    }

    /// <summary>
    /// Déplace en douceur le point regardé vers le point voulu, sans jamais dépasser la vitesse maximale.
    /// </summary>
    private void MoveLookPoint()
    {
        Vector3 targetLookPoint = ComputeTargetLookPoint();

        _smoothedLookPoint = Vector3.SmoothDamp(
            _smoothedLookPoint,
            targetLookPoint,
            ref _lookPointVelocity,
            _settings.transitionSmoothTime,
            _settings.maxCameraSpeed);
    }

    /// <summary>
    /// Place et oriente la caméra selon le point regardé, la distance et l'angle des réglages.
    /// </summary>
    private void ApplyCameraTransform()
    {
        Quaternion cameraRotation = Quaternion.Euler(_settings.cameraPitch, _settings.cameraYaw, 0f);
        Vector3 backwardDirection = cameraRotation * Vector3.back;

        transform.rotation = cameraRotation;
        transform.position = _smoothedLookPoint + backwardDirection * _settings.cameraDistance;
    }

    /// <summary>
    /// Donne le centre de l'arène.
    /// </summary>
    /// <returns>La position de l'objet centre de l'arène, ou (0, 0, 0) s'il n'est pas assigné.</returns>
    private Vector3 GetArenaCenter()
    {
        if (_arenaCenterPoint != null)
        {
            return _arenaCenterPoint.position;
        }

        return Vector3.zero;
    }

    #endregion
}