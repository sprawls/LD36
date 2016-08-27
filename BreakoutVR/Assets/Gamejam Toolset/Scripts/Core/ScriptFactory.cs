using UnityEngine;
using System.Collections;
using GamejamToolset.Saving;

public class ScriptFactory : MonoBehaviour 
{
	private void Awake()
	{
		GameObject spawnedManagers = new GameObject();
		spawnedManagers.transform.SetParent(transform);
		spawnedManagers.name = "Dynamically spawned managers";

		//Scripts to add
		spawnedManagers.AddComponent<SaveGameController>();
	}
}
