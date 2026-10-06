using UnityEngine;

[RequireComponent(typeof(Camera))]
public class StrikersCameraRig : MonoBehaviour
{
	[SerializeField] CameraSettings settings;
	[SerializeField] CameraTargetSet targetSet;

	Vector3 focus;
	Vector3 focusVelocity;
	float distance;
	bool initialized;

	void LateUpdate()
	{
		if (settings == null || targetSet == null) return;

		CameraTarget ball = targetSet.Primary;
		if (ball == null) return;

		float dt = Time.deltaTime;
		if (dt <= 0f) return;

		Vector3 desiredFocus = ball.Position;
		desiredFocus.y = settings.arenaCenter.y;

		if(!initialized)
		{
			focus = desiredFocus;
			distance = (settings.minDistance + settings.maxDistance) *0.5f;
			initialized = true;
		}
		else
		{
			focus = Vector3.SmoothDamp(focus, desiredFocus, ref focusVelocity, settings.followSmoothTime, settings.maxFollowSpeed, dt);
		}

		ApplyTransform();
	}
	void ApplyTransform()
	{
		Quaternion rot =  Quaternion.Euler(settings.pitch, settings.yaw, 0f);
		transform.rotation = rot;
		transform.position = focus - rot * Vector3.forward * distance;
	}

}