using UnityEngine;

[CreateAssetMenu(fileName = "CameraSettings_Match", menuName = "Strikers/Camera/Camera Settings")]
public class CameraSettings : ScriptableObject
{
	[Header("Angle (fixe)")]
	[Range(20f, 80f)] public float pitch = 45f;
	public float yaw = 0f;
	
	[Header("Arène (axe long = X, axe court = Z)")]
	public Vector3 arenaCenter = Vector3.zero;
	public Vector2 arenaSize = new Vector2(60f, 36f);
	[Min(0f)] public float focusInset = 6f;
	
	[Header("Point d'intérêt")]
	[Min(0.1f)] public float influenceRadius = 15f;
	[Range(0f, 1f)] public float depthFollow = 0.4f;

	[Header("Anticipation")]
	[Min(0f)] public float lookAheadTime = 0.25f;
	[Min(0.1f)] public float maxLookAhead = 5f;
	[Min(0.1f)] public float velocitySmoothing = 5f;

	[Header("Suivi")]
	[Min(0.01f)] public float followSmoothTime = 0.35f;
	[Min(1f)] public float maxFollowSpeed = 35f;

	[Header ("Zoom")]
	[Min(1f)] public float minDistance = 14f;
	[Min(1f)] public float maxDistance = 40f;
	[Min(0f)] public float framingPadding = 5f;
	[Min(0.01f)] public float zoomSmoothTime = 0.8f;

	[Header("Dezoom")]
	[Min(0.1f)] public float ballMaxSpeed = 40f;
	[Min(0f)] public float speedZoomBonus = 6f;

	void OnValidate()
	{
		if (maxDistance < minDistance) maxDistance = minDistance;
	}
}