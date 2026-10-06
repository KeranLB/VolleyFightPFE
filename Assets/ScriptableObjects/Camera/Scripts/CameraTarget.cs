using UnityEngine;

[DisallowMultipleComponent]
public class CameraTarget : MonoBehaviour
{
	[SerializeField] CameraTargetSet targetSet;
	[SerializeField] bool isPrimary;
	[SerializeField, Min(0f)] float weight = 1f;

	Rigidbody body;

	public bool IsPrimary => isPrimary;
	public float Weigh => weight;
	public Vector3 Position => transform.position;
	public Vector3 Velocity => body != null? body.linearVelocity : Vector3.zero;

	void Awake()
	{
		body = GetComponentInParent<Rigidbody>();
	}

	void OnEnable()
	{
		if (targetSet != null) targetSet.Register(this);
	}

	void OnDisable()
	{
		if (targetSet != null) targetSet.Unregister(this);
	}
}