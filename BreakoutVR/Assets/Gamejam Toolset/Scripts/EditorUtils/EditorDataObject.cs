using UnityEngine;
using GamejamToolset.LevelLoading;

namespace GamejamToolset
{
	public class EditorDataObject : MonoBehaviour
	{
		[SerializeField]
		private LevelManager levelManagerPrefabRef;

		[SerializeField]
		private ConfigHelper configHelperPrefabRef;

		//=============================================================================================

		public Levels_Data LevelsData
		{
			get 
			{
				if (levelManagerPrefabRef == null)
				{
					Debug.LogError("The level manager ref was not set in the EditorDataObject");
				}

				return levelManagerPrefabRef.RawLevelsData; 
			}
		}

		public ConfigHelper ConfigHelperData
		{
			get 
			{
				if (configHelperPrefabRef == null)
				{
					Debug.LogError("The config helper ref was not set in the EditorDataObject");
				}

				return configHelperPrefabRef; 
			}
		}

		public static EditorDataObject Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = Resources.Load<EditorDataObject>("Toolset/EditorDataObject");

					if (m_instance == null)
					{
						Debug.LogError("Could not find Editor Data Object in path Toolset/EditorDataObject");
					}
				}

				return m_instance;
			}
		}

		//=============================================================================================

		private static EditorDataObject m_instance = null;
	}
}