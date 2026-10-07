using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class GlobalOutlineStretch : MonoBehaviour
{
    public KeyCode key = KeyCode.R;
    public float duration = 0.6f;
    [Range(0f, 1f)] public float peak = 1f;
    [Tooltip("0 = simple gonflement. Plus haut = l'épaisseur pulse pendant l'effet")]
    public float pulsesPerSecond = 12f;
    public AnimationCurve falloff = AnimationCurve.EaseInOut(0, 1, 1, 0);

    const string SHADER_NAME = "Custom/InvertedHullOutline";
    static readonly int ID = Shader.PropertyToID("_StretchIntensity");

    readonly List<Material> mats = new List<Material>();
    readonly List<float> bases = new List<float>();
    float timer = -1f;

    void Start()
    {
        var seen = new HashSet<Material>();
        foreach (var r in FindObjectsByType<Renderer>(FindObjectsSortMode.None))
            foreach (var m in r.sharedMaterials)
                if (m != null && m.shader.name == SHADER_NAME && seen.Add(m))
                {
                    mats.Add(m);
                    bases.Add(m.GetFloat(ID));
                }
    }

    bool Pressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(key);
#endif
    }

    void Update()
    {
        if (Pressed()) timer = 0f;
        if (timer < 0f) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);
        float k = falloff.Evaluate(t);

        if (pulsesPerSecond > 0f)
            k *= 0.5f + 0.5f * Mathf.Sin(timer * pulsesPerSecond * Mathf.PI * 2f);

        for (int i = 0; i < mats.Count; i++)
            mats[i].SetFloat(ID, Mathf.Lerp(bases[i], peak, k));

        if (t >= 1f) { Restore(); timer = -1f; }
    }

    void Restore()
    {
        for (int i = 0; i < mats.Count; i++) mats[i].SetFloat(ID, bases[i]);
    }

    void OnDisable() => Restore();
}