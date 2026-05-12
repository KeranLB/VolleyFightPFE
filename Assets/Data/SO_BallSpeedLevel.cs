using UnityEngine;

[CreateAssetMenu(fileName = "SO_BallSpeedLevel", menuName = "Data/SO_BallSpeedLevel")]
public class SO_BallSpeedLevel : ScriptableObject
{
    public int index;
    public float speedMultiplier;
    public float damageMultiplier;
    public int freezeFrames;
}
