using UnityEngine;

/// <summary>
/// Réglages de la caméra "par zones" (itération 3 des caméras du match).
/// La caméra a 3 positions : gauche, centre, droite (le long de l'axe X, l'axe long de l'arène).
/// Elle glisse en douceur d'une position à l'autre selon l'endroit où se trouve la balle.
/// Quand la balle devient très rapide, la caméra se recentre peu à peu et ne change plus de position,
/// pour éviter qu'elle bouge sans arrêt.
/// Pour créer l'asset : clic droit dans le Project > Create > Camera > Ball Zones Camera Settings.
/// </summary>
[CreateAssetMenu(fileName = "BallZonesCameraSettings", menuName = "Camera/Ball Zones Camera Settings")]
public class BallZonesCameraSettings : ScriptableObject
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

    #region Les 3 zones

    [Header("Les 3 zones")]

    [Tooltip("Distance (en unités) entre la position du centre et les positions gauche et droite de la caméra, " +
             "le long de l'axe X.")]
    public float sideZoneOffset = 50f;

    [Tooltip("Distance de la balle au centre (sur l'axe X) à partir de laquelle on considère qu'elle est dans la zone gauche ou droite.")]
    public float zoneLimitDistance = 40f;

    [Tooltip("Marge de sécurité autour de la limite de zone. La balle doit dépasser la limite de cette marge pour changer de zone. " +
             "Évite que la caméra fasse des allers-retours quand la balle reste près de la limite.")]
    public float zoneChangeMargin = 10f;

    #endregion

    #region Mouvement de la caméra

    [Header("Mouvement de la caméra")]

    [Tooltip("Temps (en secondes) que met la caméra à glisser d'une position à l'autre. " +
             "Petit = nerveux, grand = lent et doux.")]
    public float transitionSmoothTime = 0.8f;

    [Tooltip("Vitesse maximale de la caméra, en unités par seconde. " +
             "Réglage anti motion sickness : la caméra ne dépasse jamais cette vitesse.")]
    public float maxCameraSpeed = 30f;

    #endregion

    #region Balle rapide (recentrage)

    [Header("Balle rapide (recentrage)")]

    [Tooltip("Vitesse de la balle considérée comme maximale. Sert de référence pour calculer si la balle est rapide.")]
    public float ballMaxSpeed = 300f;

    [Tooltip("Entre 0 et 1 : à partir de quelle part de la vitesse maximale la caméra commence à se recentrer. " +
             "Exemple : 0.6 = la caméra commence à se recentrer quand la balle atteint 60 % de sa vitesse max. " +
             "À 100 % de la vitesse max, la caméra est complètement au centre.")]
    [Range(0f, 1f)]
    public float recenterStartSpeedRatio = 0.6f;

    [Tooltip("Temps (en secondes) utilisé pour lisser la vitesse mesurée de la balle. " +
             "Petit = la caméra réagit vite aux changements de vitesse, grand = elle réagit plus doucement.")]
    public float ballSpeedSmoothTime = 0.3f;

    #endregion
}