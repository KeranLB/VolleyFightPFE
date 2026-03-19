using System;
using UnityEngine;

public class HitZoneCharacter : HitZone
{
    [SerializeField] private PlayerCharacter _playerCharacter;
    protected override void HitBall(Ball ball)
    {
        base.HitBall(ball);
        Debug.Log("I m a child of hitzone");
    }

    protected void ChangeTeamPossess(Ball ball)
    {
        
    }
}
