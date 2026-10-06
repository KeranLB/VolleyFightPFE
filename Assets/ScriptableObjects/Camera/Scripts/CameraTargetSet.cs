using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraTargetSet_Match", menuName = "Strikers/Camera/Camera Target Set")]
public class CameraTargetSet : ScriptableObject
{
	[NonSerialized] readonly List<CameraTarget> targets = new List<CameraTarget>();
	[NonSerialized] CameraTarget primary;

	public IReadOnlyList<CameraTarget> Targets => targets;
	public CameraTarget Primary => primary;

	void OnEnable()
	{
		targets.Clear();
		primary = null;
	}
	
	public void Register(CameraTarget target)
	{
		if (target == null || targets.Contains(target)) return;
		targets.Add(target);
		if (target.IsPrimary) primary = target;
	}

	public void Unregister(CameraTarget target)
	{
		targets.Remove(target);
		if (primary == target) primary = null;
	}

}