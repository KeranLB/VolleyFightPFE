using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Caméra "joueurs et zoom" (itération 2).
/// Elle reste plutôt au centre de l'arène, se décale un peu vers les joueurs (et un peu vers la balle),
/// et recule quand un joueur s'éloigne du centre.
/// Ce script ne modifie aucun autre script : les joueurs et la balle sont donnés dans l'Inspector.
/// À placer sur l'objet Camera de la scène de l'itération 2.
/// </summary>
public class PlayersZoomCamera : MonoBehaviour
{
    #region Réglages

    [Header("Réglages")]

    [Tooltip("L'asset PlayersZoomCameraSettings qui contient tous les réglages de cette caméra.")]
    [SerializeField] private PlayersZoomCameraSettings _settings;

    #endregion

    #region Éléments à cadrer

    [Header("Éléments à cadrer")]

    [Tooltip("Objet vide placé au centre de l'arène. S'il est vide, le centre est (0, 0, 0).")]
    [SerializeField] private Transform _arenaCenterPoint;

    [Tooltip("Les joueurs à cadrer. Glisser ici les joueurs présents dans la scène (un seul pour tester).")]
    [SerializeField] private List<Transform> _playersToFrame = new List<Transform>();

    [Tooltip("La balle. Peut rester vide : la caméra ignore alors la balle.")]
    [SerializeField] private Transform _ball;

    #endregion

    #region Valeurs internes

    // Point que la caméra regarde, après lissage.
    private Vector3 _smoothedLookPoint;
    // Vitesse actuelle du point regardé (utilisée par SmoothDamp, ne pas modifier à la main).
    private Vector3 _lookPointVelocity;
    // Distance actuelle entre la caméra et le point regardé.
    private float _currentDistance;
    // Vitesse actuelle du zoom (utilisée par SmoothDamp, ne pas modifier à la main).
    private float _zoomVelocity;

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
            Debug.LogWarning("PlayersZoomCamera : ce script doit être posé sur l'objet Camera, pas sur un autre objet. Le script est désactivé.", this);
            enabled = false;
            return;
        }

        if (_settings == null)
        {
            Debug.LogWarning("PlayersZoomCamera : aucun PlayersZoomCameraSettings n'est assigné. La caméra est désactivée.", this);
            enabled = false;
            return;
        }

        _smoothedLookPoint = ComputeTargetLookPoint();
        _currentDistance = ComputeTargetDistance();
        ApplyCameraTransform();
    }

    /// <summary>
    /// Met à jour la caméra après que tous les objets ont bougé dans la frame.
    /// </summary>
    private void LateUpdate()
    {
        MoveLookPoint();
        UpdateZoom();
        ApplyCameraTransform();
    }

    #region Déplacement et zoom

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
            _settings.followSmoothTime,
            _settings.maxCameraSpeed);
    }

    /// <summary>
    /// Change en douceur la distance de la caméra vers la distance voulue (zoom / dézoom).
    /// </summary>
    private void UpdateZoom()
    {
        float targetDistance = ComputeTargetDistance();

        _currentDistance = Mathf.SmoothDamp(
            _currentDistance,
            targetDistance,
            ref _zoomVelocity,
            _settings.zoomSmoothTime);
    }

    /// <summary>
    /// Place et oriente la caméra selon le point regardé, la distance et l'angle des réglages.
    /// </summary>
    private void ApplyCameraTransform()
    {
        Quaternion cameraRotation = Quaternion.Euler(_settings.cameraPitch, _settings.cameraYaw, 0f);
        Vector3 backwardDirection = cameraRotation * Vector3.back;

        transform.rotation = cameraRotation;
        transform.position = _smoothedLookPoint + backwardDirection * _currentDistance;
    }

    #endregion

    #region Calcul du point regardé

    /// <summary>
    /// Calcule le point que la caméra voudrait regarder (avant lissage).
    /// On part du centre de l'arène, on se décale vers les joueurs, puis un peu vers la balle.
    /// </summary>
    /// <returns>Le point voulu, en position du monde.</returns>
    private Vector3 ComputeTargetLookPoint()
    {
        Vector3 arenaCenter = GetArenaCenter();
        Vector3 playersCenter = ComputePlayersCenter(arenaCenter);

        Vector3 lookPoint = Vector3.Lerp(arenaCenter, playersCenter, _settings.playersFollowStrength);

        if (_ball != null)
        {
            lookPoint = Vector3.Lerp(lookPoint, _ball.position, _settings.ballInfluence);
        }

        return lookPoint;
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

    /// <summary>
    /// Calcule le milieu des joueurs (la moyenne de leurs positions).
    /// </summary>
    /// <param name="fallbackPoint">Point renvoyé si aucun joueur n'est disponible.</param>
    /// <returns>Le milieu des joueurs, ou fallbackPoint s'il n'y a aucun joueur.</returns>
    private Vector3 ComputePlayersCenter(Vector3 fallbackPoint)
    {
        Vector3 positionsSum = Vector3.zero;
        int playersCount = 0;

        foreach (Transform player in _playersToFrame)
        {
            if (player == null)
            {
                continue;
            }

            positionsSum += player.position;
            playersCount++;
        }

        if (playersCount == 0)
        {
            return fallbackPoint;
        }

        return positionsSum / playersCount;
    }

    #endregion

    #region Calcul du zoom

    /// <summary>
    /// Calcule la distance voulue entre la caméra et le point regardé.
    /// Joueurs proches du centre = zoom proche. Joueurs loin du centre = dézoom.
    /// </summary>
    /// <returns>La distance voulue (avant lissage).</returns>
    private float ComputeTargetDistance()
    {
        float farthestPlayerDistance = ComputeFarthestPlayerDistanceFromCenter();

        // 0 = joueurs près du centre, 1 = joueurs très loin du centre.
        float zoomOutAmount = Mathf.InverseLerp(
            _settings.playerDistanceForClosestZoom,
            _settings.playerDistanceForFarthestZoom,
            farthestPlayerDistance);

        return Mathf.Lerp(_settings.closestCameraDistance, _settings.farthestCameraDistance, zoomOutAmount);
    }

    /// <summary>
    /// Cherche le joueur le plus éloigné du centre de l'arène et renvoie sa distance (sans tenir compte de la hauteur).
    /// </summary>
    /// <returns>La distance du joueur le plus éloigné, ou 0 s'il n'y a aucun joueur.</returns>
    private float ComputeFarthestPlayerDistanceFromCenter()
    {
        Vector3 arenaCenter = GetArenaCenter();
        float farthestDistance = 0f;

        foreach (Transform player in _playersToFrame)
        {
            if (player == null)
            {
                continue;
            }

            Vector3 offsetFromCenter = player.position - arenaCenter;
            offsetFromCenter.y = 0f;

            if (offsetFromCenter.magnitude > farthestDistance)
            {
                farthestDistance = offsetFromCenter.magnitude;
            }
        }

        return farthestDistance;
    }

    #endregion
}