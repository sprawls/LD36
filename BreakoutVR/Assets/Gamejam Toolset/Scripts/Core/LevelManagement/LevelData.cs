using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamejamToolset.LevelLoading
{
	[Serializable]
	public class Levels_Data
	{
		[SerializeField]
		private List<Level_Data> m_levels;

		public List<Level_Data> LevelsList { get { return m_levels; } }

		//==========================================================================================

		//----------------------------------------------------------------
		public bool DoesLevelExists(string name)
		{
			return GetLevel(name) != null;
		}

		//----------------------------------------------------------------
		public bool IsLevelAlreadyLoaded(string name)
		{
			Scene level = SceneManager.GetSceneByName(name);
			return level.name != null;
		}

		//----------------------------------------------------------------
		public Level_Data GetLevel(string name)
		{
			foreach (Level_Data data in m_levels)
			{
				if (data.Name == name)
					return data;
			}

			return null;
		}
	}

	[Serializable]
	public class Level_Data
	{
		[SerializeField]
		private string levelName;

		[SerializeField]
		private List<Scene_Data> scenes = new List<Scene_Data>();

		public string Name { get { return levelName; } }
		public List<Scene_Data> Scenes { get { return scenes; } }

		public Level_Data()
		{
			levelName = "";
		}

		public Level_Data(string name)
		{
			levelName = name;
		}

#if UNITY_EDITOR
		public bool Included
		{
			get
			{
				foreach (Scene_Data data in scenes)
				{
					if (data.m_loadState == Scene_Data.EditorLoadState.Include)
						return true;
				}

				return false;
			}
		}
#endif
	}

	[Serializable]
	public class Scene_Data
	{
#if UNITY_EDITOR
		public enum EditorLoadState
		{
			Include,
			Exclude
		}

		[SerializeField]

		private SceneAsset m_scene;

		[HideInInspector]
		public EditorLoadState m_loadState;

		public SceneAsset Scene { get { return m_scene; } set { m_scene = value; } }
#endif
	}

	#region Custom Property Drawer
#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(Levels_Data))]
	public class LevelsDataDrawer : PropertyDrawer
	{
		private float height = 0;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return height;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty levels = property.FindPropertyRelative("m_levels");
			float startingPos = position.y;

			++EditorGUI.indentLevel;

			for (int i = 0; i < levels.arraySize; ++i)
			{
				EditorGUI.PropertyField(position, levels.GetArrayElementAtIndex(i), true);

				position.y += EditorGUI.GetPropertyHeight(levels.GetArrayElementAtIndex(i), label, true);

				if (GUI.Button(new Rect(position.x + 15, position.y, 100, 15), "Remove Level"))
				{
					levels.DeleteArrayElementAtIndex(i);
				}

				position.y += 35;
			}

			if (GUI.Button(new Rect(position.x + 15, position.y, position.width - 15, 25), "Add Level"))
			{
				levels.InsertArrayElementAtIndex(levels.arraySize);
			}

			position.y += 25;

			height = position.y - startingPos;
		}
	}

	[CustomPropertyDrawer(typeof(Level_Data))]
	public class LevelDataDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			SerializedProperty scenes = property.FindPropertyRelative("scenes");

			return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("levelName"), label, true) +
				EditorGUI.GetPropertyHeight(scenes, label, true) + 27;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty levelName = property.FindPropertyRelative("levelName");

			Texture2D texture = new Texture2D(1, 1);
			texture.SetPixel(0, 0, new Color(0.6f, 0.6f, 0.6f));
			texture.Apply();

			GUIStyle styleBackground = new GUIStyle();
			styleBackground.normal.background = texture;
			EditorGUI.LabelField(new Rect(position.x - 10, position.y, position.width + 10, 20), GUIContent.none, styleBackground);

			GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
			style.fixedHeight = 25;
			style.fontSize = 15;
			style.normal.textColor = new Color(0.9f, 0.9f, 0.9f);

			EditorGUI.LabelField(position, levelName.stringValue, style);
			position.y += 25;
			EditorGUI.PropertyField(position, levelName, true);
			position.y += EditorGUI.GetPropertyHeight(levelName, label, true);
			SerializedProperty scenes = property.FindPropertyRelative("scenes");
			EditorGUI.PropertyField(position, scenes, true);
		}
	}
#endif
	#endregion
}
