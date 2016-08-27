using UnityEngine;
using System.Collections;

public class Singleton<T> : ExtendedMonoBehaviour where T : ExtendedMonoBehaviour {

	private static T m_instance;

	public static T Instance 
	{
		get
		{
			if (m_instance == null) 
			{
				T[] instances = FindObjectsOfType<T>();

				if (instances.Length > 0)
				{
					if (instances.Length > 1)
						Debug.LogErrorFormat("Multiple instances of singleton {0} in scene. This is big source of error. Fix it", typeof(T).Name);

					m_instance = instances[0];
				}
				else 
				{
					Debug.LogError("No Instance of object in scene.");
				}
			}

			return m_instance;
		}
	}

	public static bool IsNull()
	{
		return m_instance == null;
	}

	protected virtual void OnDestroy()
	{
		if (m_instance == this)
		{
			m_instance = null;
		}
	}
}
