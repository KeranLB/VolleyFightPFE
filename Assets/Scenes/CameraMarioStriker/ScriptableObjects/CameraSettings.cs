using UnityEngine;

/// <summary>Réglages de la caméra de match. Tout le reste est en constantes dans StrikersCameraRig.</summary>
[CreateAssetMenu(fileName = "CameraSettings_Match", menuName = "Strikers/Camera/Camera Settings")]
public class CameraSettings : ScriptableObject
{
    [Header("Vue (fixe)")]
    [Range(20f, 80f)] public float pitch = 45f;
    public float yaw = 0f;
    [Min(1f)] public float distance = 70f;

    [Header("Arène (axe long = X, axe court = Z)")]
    public Vector3 arenaCenter = Vector3.zero;
    public Vector2 arenaSize = new Vector2(60f, 36f);

    [Header("Follow")]
    [Tooltip("0 = suit le milieu des joueurs. 1 = suit la balle.")]
    [Range(0f, 1f)] public float ballInfluence = 0.65f;
    [Tooltip("1 = suit en profondeur. 0 = reste centré sur l'arène sur cet axe (moins de mouvement vertical).")]
    [Range(0f, 1f)] public float depthFollow = 0.5f;
    [Tooltip("Temps de suivi total quand la balle est lente (plus bas = plus réactif).")]
    [Min(0.05f)] public float followSmoothTime = 0.5f;
    [Tooltip("Temps de suivi total quand la balle est à ballMaxSpeed (plus haut = plus posé).")]
    [Min(0.05f)] public float followSmoothTimeFastBall = 1.0f;
    [Tooltip("Vitesse de la balle (unités/s) considérée comme maximale. Sert aussi à plafonner la vitesse de la caméra.")]
    [Min(0.1f)] public float ballMaxSpeed = 100f;

    [Header("Événements (hit, KO...)")]
    [Tooltip("Multiplicateur global. 0 = aucun effet (option d'accessibilité).")]
    [Range(0f, 1f)] public float eventIntensity = 1f;
    [Tooltip("Amplitude du shake (unités monde).")]
    [Min(0f)] public float shakeAmount = 0.35f;
    [Tooltip("Amplitude du coup de caméra dans la direction du hit (unités monde).")]
    [Min(0f)] public float kickAmount = 0.5f;

    void OnValidate()
    {
        if (followSmoothTimeFastBall < followSmoothTime) followSmoothTimeFastBall = followSmoothTime;
    }
}