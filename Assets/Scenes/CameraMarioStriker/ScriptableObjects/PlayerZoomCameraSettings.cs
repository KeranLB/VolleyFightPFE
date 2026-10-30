using UnityEngine;

/// <summary>
/// Réglages de la caméra "joueurs et zoom" (itération 2 des caméras du match).
/// Cette caméra reste plutôt au centre de l'arène, comme dans FIFA.
/// Ce sont surtout les joueurs qui la font bouger :
/// - elle se décale un peu vers les joueurs ;
/// - elle recule (dézoom) quand des joueurs s'éloignent du centre, et se rapproche (zoom) quand ils sont proches du centre.
/// Pour créer l'asset : clic droit dans le Project > Create > Camera > Players Zoom Camera Settings.
/// </summary>
[CreateAssetMenu(fileName = "PlayersZoomCameraSettings", menuName = "Camera/Players Zoom Camera Settings")]
public class PlayersZoomCameraSettings : ScriptableObject
{
    #region Angle (fixe)

    [Header("Angle (fixe)")]

    [Tooltip("Inclinaison de la caméra vers le bas, en degrés. 90 = vue de dessus, 0 = vue de côté. " +
             "Reprendre la même valeur que les autres caméras : les déplacements des joueurs dépendent de l'angle.")]
    [Range(0f, 90f)]
    public float cameraPitch = 45f;

    [Tooltip("Rotation de la caméra autour de l'axe vertical, en degrés. " +
             "Reprendre la même valeur que les autres caméras : les déplacements des joueurs dépendent de l'angle.")]
    [Range(-180f, 180f)]
    public float cameraYaw = 0f;

    #endregion

    #region Position (reste au centre)

    [Header("Position (reste au centre)")]

    [Tooltip("0 = la caméra reste toujours au centre de l'arène. 1 = la caméra se place exactement au milieu des joueurs. " +
             "Une valeur basse donne une caméra calme, une valeur haute une caméra qui suit le jeu.")]
    [Range(0f, 1f)]
    public float playersFollowStrength = 0.4f;

    [Tooltip("Entre 0 et 1 : à quel point la balle tire la caméra vers elle. " +
             "0 = la caméra ignore la balle (risque : la balle peut sortir de l'écran). 0.2 = léger coup d'œil vers la balle.")]
    [Range(0f, 1f)]
    public float ballInfluence = 0.2f;

    [Tooltip("Temps (en secondes) que met la caméra à se décaler. Petit = nerveux, grand = lent et doux.")]
    public float followSmoothTime = 0.4f;

    [Tooltip("Vitesse maximale de déplacement de la caméra, en unités par seconde. " +
             "Réglage anti motion sickness : la caméra ne dépasse jamais cette vitesse.")]
    public float maxCameraSpeed = 40f;

    #endregion

    #region Zoom selon l'éloignement des joueurs

    [Header("Zoom selon l'éloignement des joueurs")]

    [Tooltip("Distance de la caméra quand les joueurs sont proches du centre (zoom le plus proche).")]
    public float closestCameraDistance = 50f;

    [Tooltip("Distance de la caméra quand les joueurs sont loin du centre (dézoom le plus large).")]
    public float farthestCameraDistance = 90f;

    [Tooltip("Si le joueur le plus éloigné du centre est à cette distance ou moins, la caméra est au zoom le plus proche.")]
    public float playerDistanceForClosestZoom = 30f;

    [Tooltip("Si le joueur le plus éloigné du centre est à cette distance ou plus, la caméra est au dézoom le plus large.")]
    public float playerDistanceForFarthestZoom = 120f;

    [Tooltip("Temps (en secondes) que met le zoom à changer. Grand = zoom lent et doux.")]
    public float zoomSmoothTime = 0.6f;

    #endregion
}