#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace GamejamToolset.LevelLoading
{
	public static class LevelManagerMenu	
	{
		public static void CloseAllMaps()
		{
			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

			for (int i = EditorSceneManager.sceneCount - 1; i >= 0; --i)
			{
				Scene scene = EditorSceneManager.GetSceneAt(i);
				if (scene.name != BootController.PERSISTENT_SCENE_NAME)
				{
					EditorSceneManager.CloseScene(scene, true);
				}
			}
		}
	}

	public class LevelManagerWindow : EditorWindow
	{
		const float PAGE_WIDTH = 400;

		private Color m_lightBlue = new Color(160f / 255f, 180f / 255f, 255f / 255f);
		private Color m_lightGreen = new Color(100f / 255f, 200f / 255f, 100f / 255f);
		//private Color m_superLightBlue = new Color(160f / 255f, 220f / 255f, 255f / 255f);
		private Color m_lightRed = new Color(255f / 255f, 180f / 255f, 180f / 255f);

		private Vector2 scrollPos;
		private Levels_Data levelsData;

		//--------------------------------------------------------------------------------------------
		public static void OpenWindow()
		{
			EditorWindow window = EditorWindow.GetWindow(typeof(LevelManagerWindow));

			Vector2 size = new Vector2(665, 550);
			window.minSize = size;
			window.maxSize = size;
		}

		//--------------------------------------------------------------------------------------------
		private void OnEnable()
		{
			levelsData = EditorDataObject.Instance.LevelsData;

			if (levelsData == null)
				return;

			if (levelsData.GetLevel("Menu") == null)
			{
				levelsData.LevelsList.Add(new Level_Data("Menu"));
			}
		}

		//--------------------------------------------------------------------------------------------
		private void OnGUI()
		{
			if (levelsData == null)
				return;

			if (Application.isPlaying)
				return;

			Levels_Data data = levelsData;

			Header1("Level Builder");
			GUILayout.Space(15);

			LevelGenerationButtonsGUI(data.LevelsList);
			GUILayout.Space(15);
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(320));
			EditorGUILayout.BeginVertical(GUILayout.Height(300));
			for (int i = 0; i < data.LevelsList.Count; ++i)
			{
				LevelsOptionGUI(data.LevelsList[i]);
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndScrollView();

			GUILayout.Space(15);

			EditorGUILayout.BeginHorizontal(GUILayout.Width(640));
			if (GUILayout.Button("Include All", GUILayout.Height(50)))
			{

				foreach (Level_Data d in data.LevelsList)
				{
					ChangeLevelsLoadState(d, Scene_Data.EditorLoadState.Include);
				}
			}

			if (GUILayout.Button("Exclude All", GUILayout.Height(50)))
			{
				foreach (Level_Data d in data.LevelsList)
				{
					ChangeLevelsLoadState(d, Scene_Data.EditorLoadState.Exclude);
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		//--------------------------------------------------------------------------------------------
		private void LevelGenerationButtonsGUI(List<Level_Data> levels) 
		{
			EditorGUILayout.BeginHorizontal(GUILayout.Width(640));
			GUI.color = m_lightBlue;
			if (GUILayout.Button("Debug Scene Builder", GUILayout.Height(50)))
			{
				LevelBuilder.LoadRawLevelScenes(levels);
			}

			GUI.color = m_lightGreen;
			if (GUILayout.Button("Final Scene Builder", GUILayout.Height(50)))
			{
				LevelBuilder.CreateBuildLevels(levels);
				return;
			}

			GUI.color = m_lightRed;
			if (GUILayout.Button("Generate Level Enum \n(Leave to Programmers)", GUILayout.Height(50)))
			{
				int choice = EditorUtility.DisplayDialogComplex
				(
					"Regenerating Level Enum",
					"Regenerating the LevelName enum could break part of the code if done carelessly. Only programmer should do this. Are you sure you want to continue?",
					"Continue",
					"Cancel",
					"Get the fuck out"
				);
				if (choice == 0)
				{
					LevelBuilder.RecreateLevelNameEnum(levels);
				}
				return;
			}
			GUI.color = Color.white;

			EditorGUILayout.EndHorizontal();
		}

		//--------------------------------------------------------------------------------------------
		private void LevelsOptionGUI(Level_Data data)
		{
			Header2(data.Name);

			if (data.Scenes.Count > 0) 
			{
				int count = 0;
				EditorGUILayout.BeginHorizontal();
				foreach (Scene_Data scene in data.Scenes)
				{
					if (count % 3 == 0)
					{
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
					}

					GUI.color = (scene.m_loadState == Scene_Data.EditorLoadState.Exclude) ? Color.red : Color.green;
					scene.m_loadState = (Scene_Data.EditorLoadState)EditorGUILayout.EnumPopup(scene.m_loadState, GUILayout.Width(70));
					EditorGUILayout.LabelField(scene.Scene.name, EditorStyles.boldLabel, GUILayout.Width(80));
					GUI.color = Color.white;
					GUILayout.Space(50);
					++count;
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal(GUILayout.Width(640));
				if (GUILayout.Button("Include All"))
				{
					ChangeLevelsLoadState(data, Scene_Data.EditorLoadState.Include);
				}

				if (GUILayout.Button("Exclude All"))
				{
					ChangeLevelsLoadState(data, Scene_Data.EditorLoadState.Exclude);
				}
				EditorGUILayout.EndHorizontal();
			}
			else 
			{
				EditorGUILayout.LabelField("No scene related to this level");
			}

			GUILayout.Space(15);
		}

		#region Utils
		private void ChangeLevelsLoadState(Level_Data data, Scene_Data.EditorLoadState state)
		{
			foreach (var scene in data.Scenes)
			{
				scene.m_loadState = state;
			}
		}

		private void Header1(string text)
		{
			GUILayout.Space(15);
			GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
			style.fontSize = 35;
			style.fixedHeight = 40;
			EditorGUILayout.LabelField(text, style);
			GUILayout.Space(25);
		}

		private void Header2(string text)
		{
			GUILayout.Space(10);
			GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
			style.fontSize = 18;
			style.fixedHeight = 25;
			EditorGUILayout.LabelField(text, style);
			GUILayout.Space(10);
		}

		private void Header3(string text)
		{
			GUILayout.Space(5);
			GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
			style.fontSize = 14;
			style.fixedHeight = 25;
			EditorGUILayout.LabelField(text, style);
			GUILayout.Space(5);
		}
		#endregion
	}
}
#endif