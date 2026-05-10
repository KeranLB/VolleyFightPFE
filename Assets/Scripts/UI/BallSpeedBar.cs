using UnityEngine;
using UnityEngine.UI;

public class BallSpeedBar : MonoBehaviour
{
    public Image shadowImage;

    private void OnEnable()
    {
        Ball.OnSpeedLevelChanged += OnSpeedLevelChanged;
    }

    private void OnSpeedLevelChanged(int index)
    {
        shadowImage.fillAmount = 1 - index / 10f;
    }
}
