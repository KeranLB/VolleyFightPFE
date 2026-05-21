using UnityEngine;

[ExecuteInEditMode]
public class LightSource : MonoBehaviour
{
    [Header("Apparence")]
    public Color volumeColor = Color.white;
    [Range(0f, 20f)] public float emissionIntensity = 1f;

    [Header("Forme")]
    public float radius = 2f;
    [Range(0.01f, 50f)] public float softness = 5f;

    void OnEnable()
    {
        if (!LightMaskManager.allLights.Contains(this))
            LightMaskManager.allLights.Add(this);
    }

    void OnDisable()
    {
        LightMaskManager.allLights.Remove(this);
    }

    void OnDestroy()
    {
        LightMaskManager.allLights.Remove(this);
    }
}