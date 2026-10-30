using UnityEngine;

/// <summary>
/// Réglages de la caméra "suivi de balle" (itération 1 des caméras du match).
/// Cette caméra suit toujours la balle. Pour éviter la motion sickness :
/// - elle regarde un peu devant la balle (anticipation), mais de façon limitée ;
/// - sa vitesse de déplacement est plafonnée, même si la balle va très vite.
/// Pour créer l'asset : clic droit dans le Project > Create > Camera > Ball Follow Camera Settings.
/// </summary>
[CreateAssetMenu(fileName = "BallFollowCameraSettings", menuName = "Camera/Ball Follow Camera Settings")]
public class BallFollowCameraSettings : ScriptableObject
{
    #region Angle et distance (fixes)

    [Header("Angle et distance (fixes)")]

    [Tooltip("Inclinaison de la caméra vers le bas, en degrés. 90 = vue de dessus, 0 = vue de côté. " +
             "Reprendre la même valeur que les autres caméras : les déplacements des joueurs dépendent de l'angle.")]
    [Range(0f, 90f)]
    public float cameraPitch = 45f;

    [Tooltip("Rotation de la caméra autour de l'axe vertical, en degrés. " +
             "Reprendre la même valeur que les autres caméras : les déplacements des joueurs dépendent de l'angle.")]
    [Range(-180f, 180f)]
    public float cameraYaw = 0f;

    [Tooltip("Distance entre la caméra et le point qu'elle regarde. Cette distance ne change jamais (pas de zoom).")]
    public float cameraDistance = 70f;

    #endregion

    #region Suivi de la balle

    [Header("Suivi de la balle")]

    [Tooltip("Temps (en secondes) que met la caméra à rattraper la balle. " +
             "Petit = caméra nerveuse, grand = caméra lente et douce.")]
    public float followSmoothTime = 0.3f;

    [Tooltip("Vitesse maximale de la caméra, en unités par seconde. " +
             "C'est le réglage anti motion sickness : même si la balle va très vite, la caméra ne dépasse jamais cette vitesse.")]
    public float maxCameraSpeed = 60f;

    #endregion

    #region Anticipation

    [Header("Anticipation")]

    [Tooltip("La caméra regarde la position que la balle aura dans ce nombre de secondes. " +
             "0 = pas d'anticipation, la caméra regarde la balle là où elle est.")]
    public float anticipationSeconds = 0.25f;

    [Tooltip("Distance maximale de l'anticipation, en unités. " +
             "Évite que la caméra parte trop loin devant la balle quand elle est très rapide.")]
    public float maxAnticipationDistance = 20f;

    #endregion
}