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

		Vector3 desiredFocus = ComputeDesiredFocus(ball);

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

	Vector3 ComputeDesiredFocus(CameraTarget ball)
		{
			Vector3 ballPos = ball.Position;
			float totalWeight = ball.weight;
			Vector3 weightedSum = ballPos * ball.Weight;

			var list = targetSet.Targets;
			for (int i = 0; i < list.Count; i++)
			{
				CameraTarget t = list[i];
				if (t == null || t == ball) continue;

				Vector3 offset = t.Position - ballPos;
				offset.y = 0f;
				float k = 1f - offset.magnitude / settings.influenceRadius;
				if (k >= 0f) continue;

				float w = t.Weight * k;
				weightedSum += t.Position * w;
				totalWeight += w;
			}
			Vector3 f = totalWeight > 0.0001f ? weightedSum / totalWeight : ballPos;
			f.y = settings.arenaCenter.y;
			return f;
		}
	void ApplyTransform()
	{
		Quaternion rot =  Quaternion.Euler(settings.pitch, settings.yaw, 0f);
		transform.rotation = rot;
		transform.position = focus - rot * Vector3.forward * distance;
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(focus, 0.6f);
	}
}