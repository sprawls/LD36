using UnityEngine;
using System.Collections;

public class PersistentObjectsManager : Singleton<PersistentObjectsManager>
{
	[SerializeField]
	private GameObject[] persistentList;

	private static bool m_alreadySpawned = false;

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		SpawnPersistent();
	}

	private void SpawnPersistent()
	{
		if (m_alreadySpawned)
			return;

		foreach (var persistent in persistentList)
		{
			Transform t = Instantiate(persistent).transform;
			t.SetParent(transform);
			t.localPosition = Vector3.zero;
		}

		m_alreadySpawned = true;
	}
}
