using UnityEngine;
using System.Collections;

public class UIController : UISingleton<UIController> 
{
	[LargeHeader("UI Objects List"), SerializeField]
	private GameObject[] m_uiList;

	private static bool m_alreadySpawned = false;

	private void Awake()
	{
		SpawnUIs();
	}

	private void SpawnUIs()
	{
		if (m_alreadySpawned)
			return;

		foreach (var ui in m_uiList)
		{
			Transform t = Instantiate(ui).transform;
			t.SetParent(transform);
			t.localPosition = Vector3.zero;
		}

		m_alreadySpawned = true;
	}
}
