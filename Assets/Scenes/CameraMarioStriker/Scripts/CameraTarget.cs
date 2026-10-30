using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// À poser sur la balle (isPrimary) et sur chaque joueur. S'enregistre tout seul : plus besoin de ScriptableObject de liste.
/// </summary>
[DisallowMultipleComponent]
public class CameraTarget : MonoBehaviour
{
    public static readonly List<CameraTarget> All = new List<CameraTarget>();
    public static CameraTarget Primary { get; private set; }

    [SerializeField] bool isPrimary;
    [SerializeField, Min(0f)] float weight = 1f;

    public bool IsPrimary => isPrimary;
    public float Weight => weight;
    public Vector3 Position => transform.position;

    // Sécurité si "Enter Play Mode Options" (domain reload désactivé) est utilisé
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() { All.Clear(); Primary = null; }

    void OnEnable()
    {
        if (!All.Contains(this)) All.Add(this);
        if (isPrimary) Primary = this;
    }

    void OnDisable()
    {
        All.Remove(this);
        if (Primary == this) Primary = null;
    }
}