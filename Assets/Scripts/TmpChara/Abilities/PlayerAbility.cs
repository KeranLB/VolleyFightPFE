using System;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    public float startupTime;
    public float activeTime;
    public float recoveryTime;

    public AttackZone attackZone;

    public bool overridesMovement;
    public AnimationCurve velocityCurveX;
    public AnimationCurve velocityCurveY;
    public AnimationCurve velocityCurveZ;

    public float speedFactor;

    public Vector3 EvaluateVelocity(float relativeTime)
    {
        float lerpValue = relativeTime / (startupTime + activeTime + recoveryTime);
        Vector3 velocity = new Vector3(
            velocityCurveX.Evaluate(lerpValue) * speedFactor,
            velocityCurveY.Evaluate(lerpValue) * speedFactor,
            velocityCurveZ.Evaluate(lerpValue) * speedFactor
        );
        return velocity;
    }

    public void EnableEffect()
    {
        attackZone.Activate();
    }

    public void DisableEffect()
    {
        attackZone.Deactivate();
    }
}
