using System;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    public AbilityType abilityType;
    public float startupTime;
    public float activeTime;
    public float recoveryTime;

    public AttackZone attackZone;
    public GuardZone guardZone;

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
        attackZone?.Activate();
        guardZone?.Activate();
    }

    public void DisableEffect()
    {
        attackZone?.Deactivate();
        guardZone?.Deactivate();
    }
}

public enum AbilityType
{
    Attack1,
    Attack2,
    Block
}