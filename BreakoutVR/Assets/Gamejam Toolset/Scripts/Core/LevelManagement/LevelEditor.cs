#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class LevelEditor : EditorWindow
{
	/*private const int MAP_COUNT_PER_ROW = 3;

	public enum LevelLoadState
	{
		Included,
		Excluded
	}

	private class LevelStateEnum
	{
		public const string Included = "Included";
		public const string Excluded = "Excluded";
	}

	//=================================================================================

	private static bool m_isWindowShowing = false;

	//=================================================================================

	[MenuItem("Window/Level Editor")]
	public static void ToggleWindow()
	{
		EditorWindow windowRef = EditorWindow.GetWindow(typeof(LevelEditor));
		
		/*if (m_isWindowShowing)
		{
			windowRef.Close();
		}

		m_isWindowShowing = !m_isWindowShowing;*//*
	}

	//-------------------------------------------------------------------
	private void OnEnable()
	{
		m_isWindowShowing = true;
		LevelManager.Instance.LoadData();
	}

	//-------------------------------------------------------------------
	private void OnDisable()
	{
		m_isWindowShowing = false;
		LevelManager.Instance.SaveData();
	}

	//-------------------------------------------------------------------
	void OnGUI()
	{
		//Change style
		EditorGUILayout.LabelField("Level Managment Editor", EditorStyles.largeLabel);
		GUILayout.Space(100);

		GlobalLevels_Data data = LevelManager.Instance.LevelData;

		//Static Level
		EditorGUILayout.BeginHorizontal(GUILayout.Width(200));
		EditorGUILayout.LabelField(new GUIContent("Static Scene", "Object that stays between scene"), EditorStyles.boldLabel);
		if (data.StaticLevelData != null)
		{
			Scene scene = EditorSceneManager.GetSceneByName("Static Objects (Never Destroyed)");
			if (scene != null && !scene.isLoaded)
			{
				if (GUILayout.Button("Load"))
				{

				}
			}
		}
		else
		{
			if (GUILayout.Button("Create Map"))
			{
				TryCreatingScene("Static Objects (Never Destroyed)");
				data.StaticLevelData = new Level_Data("Static Objects (Never Destroyed"));
			}
		}
		EditorGUILayout.EndHorizontal();

		GUILayout.Space(50);

		EditorGUILayout.BeginHorizontal(GUILayout.Width(MAP_COUNT_PER_ROW * 200));
		for (int i = 0; i < data.LevelsData.Count; ++i)
		{
			Level_Data level = data.LevelsData[i];
			EditorGUILayout.LabelField(level.Name, EditorStyles.boldLabel);
			if (i % MAP_COUNT_PER_ROW == 0)
			{
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal(GUILayout.Width(MAP_COUNT_PER_ROW * 200));
			}
		}
		EditorGUILayout.EndHorizontal();

		GUILayout.Space(50);

		EditorGUILayout.BeginHorizontal(GUILayout.Width(400));
	}

	//-------------------------------------------------------------------
	private void TryCreatingScene(string name)
	{
		Scene scene = EditorSceneManager.GetSceneByName(name);
		if (scene == null)
		{
			CreateNewScene(name);
		}
		else
		{
			if (LevelData_Utils.GetLevelByName(name) == null)
			{
				Debug.Log("A level by that name already exists, adding it to list of level");
				LevelManager.Instance.LevelData.LevelsData.Add(new Level_Data(name));
			} 
			else 
			{
				Debug.LogError("Trying to add a level that already exist and is part of the level list");
			}
		}
	}

	//-------------------------------------------------------------------
	private void CreateNewScene(string name)
	{
		Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Additive);
		//Scene scene = EditorSceneManager.CreateScene("Static Objects (Never Destroyed)");
		EditorSceneManager.SaveScene(scene, string.Format("Maps/{0}", name));
		LevelManager.Instance.LevelData.LevelsData.Add(new Level_Data(scene.name));
	}

	//-------------------------------------------------------------------
	private void LoadScene(string name)
	{
		Scene scene = EditorSceneManager.GetSceneByName(name);

		if (scene == null)
		{
			Debug.LogError("The scene you are trying to load does not exist. It was deleted from outside this window, removing it from level list.");
		}
	}

	//-------------------------------------------------------------------
	private void LoadAllScenes()
	{

	}

	//-------------------------------------------------------------------
	private void DeleteScene(string name)
	{
		Level_Data data = LevelData_Utils.GetLevelByName(name);

		Scene scene = EditorSceneManager.GetSceneByName(name);
		if (scene != null)
		{
			
		}
	}*/
}
#endif