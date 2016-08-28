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

    public static T GetClosest<T>(List<T> list, Component closestTo) where T : Component
    {
        if (list.Count == 0)
            return null;

        if (list.Count == 1)
            return list[0];

        Vector3 closestToPos = closestTo.transform.position;
        T closest = list[0];
        float closestDistance = Vector3.Distance(closestToPos, list[0].transform.position);

        for (int i = 1; i < list.Count; ++i)
        {
            float distance = Vector3.Distance(closestToPos, list[i].transform.position);
            if (distance < closestDistance)
            {
                closest = list[i];
                closestDistance = distance;
            }
        }

        return closest;
    }
}