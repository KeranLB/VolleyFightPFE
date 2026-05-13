using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HitZone : MonoBehaviour
{
    protected Rigidbody _rigidbody;
    
    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = true;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    public virtual void OnTrajectory(Ball ball, RaycastHit hitInfo)
    {
        
    }

    public virtual void OnOverlap(Ball ball)
    {
        
    }
}
