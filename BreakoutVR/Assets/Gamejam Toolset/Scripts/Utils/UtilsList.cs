using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class GameObjectUtils 
{
	public static T[] GetAllObjectOfTypeWithInactive<T>()
	{
		List<T> objects = new List<T>();

		for (int i = 0; i < SceneManager.sceneCount; ++i)
		{
			Scene scene = SceneManager.GetSceneAt(i);
			GameObject[] roots = scene.GetRootGameObjects();
			for (int j = 0; j < roots.Length; ++j)
			{
				objects.AddRange(roots[i].GetComponentsInChildren<T>(true));
			}
		}

		return objects.ToArray();
	}
}