using UnityEngine;
using System.Collections;

public abstract class ExtendedMonoBehaviour : MonoBehaviour {

	protected virtual void RegisterCallbacks() { }
	protected virtual void UnregisterCallbacks() { }

	protected virtual void OnEnable()
	{
		RegisterCallbacks();
	}

	protected virtual void OnDisable()
	{
		UnregisterCallbacks();
	}
}
