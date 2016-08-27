#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using System.IO;

namespace GamejamToolset.LevelLoading
{
	public static class LevelBuilder
	{
		private const string BUILD_SCENE_FOLDER_PATH = "/Gamejam Toolset/BuildScenes";
		private const string BOOT_SCENE_PATH = "/Gamejam Toolset/Scenes/Boot Scene.unity";
		private const string LEVEL_NAME_ENUM_FILE_PATH = "/Gamejam Toolset/Scripts/Core/LevelManagement/LevelNameEnum.cs";

		public static Scene OpenBoot()
		{
			return EditorSceneManager.OpenScene(Application.dataPath + BOOT_SCENE_PATH);
		}

		public static void CreateBuildLevels(List<Level_Data> levelsData, bool includedOnly = true)
		{
			RemoveAllMapFromBuild();
			CreateBuildFolder();
			AddMapToBuild(OpenBoot());

			List<Level_Data> included = new List<Level_Data>();
			foreach (Level_Data data in levelsData)
			{
				if (!includedOnly || data.Included)
					included.Add(data);
			}

			CustomLogger.BuildLog("Creating Build Levels");
			int count = 1;
			foreach (Level_Data data in included)
			{
				EditorUtility.DisplayProgressBar("Building Maps", string.Format("Map being build {0} / {1}", count, included.Count), (float)count / (float)included.Count);
				Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
				EditorSceneManager.SetActiveScene(scene);

				foreach (Scene_Data sceneData in data.Scenes)
				{
					if (!includedOnly || sceneData.m_loadState == Scene_Data.EditorLoadState.Include)
					{
						Scene s = EditorSceneManager.OpenScene(AssetDatabase.GetAssetOrScenePath(sceneData.Scene), OpenSceneMode.Additive);
						foreach (GameObject root in s.GetRootGameObjects())
						{
							MonoBehaviour.Instantiate(root);
						}
						EditorSceneManager.CloseScene(s, true);
					}
				}
				EditorSceneManager.SaveScene(scene, string.Format("{0}{1}/{2}.unity", "Assets", BUILD_SCENE_FOLDER_PATH, data.Name));
				AddMapToBuild(scene);

				EditorUtility.ClearProgressBar();
				++count;
			}
		}

		private static void CreateBuildFolder()
		{
			CustomLogger.BuildLog("Creating Build Folder at " + Application.dataPath + BUILD_SCENE_FOLDER_PATH);
			if (Directory.Exists(Application.dataPath + BUILD_SCENE_FOLDER_PATH))
			{
				DirectoryInfo directory = new DirectoryInfo(Application.dataPath + BUILD_SCENE_FOLDER_PATH);
				directory.Delete(true);
			}
				
			Directory.CreateDirectory(Application.dataPath + BUILD_SCENE_FOLDER_PATH);
		}

		public static void LoadRawLevelScenes(List<Level_Data> levelsData)
		{
			bool first = true;
			bool hasIncluded = false;

			foreach (Level_Data data in levelsData)
			{
				foreach (Scene_Data scene in data.Scenes)
				{
					if (scene.m_loadState == Scene_Data.EditorLoadState.Include)
					{
						EditorSceneManager.OpenScene(AssetDatabase.GetAssetOrScenePath(scene.Scene), first ? OpenSceneMode.Single : OpenSceneMode.Additive);
						first = false;
						hasIncluded = true;
					}
				}
			}

			if (!hasIncluded)
			{
				Debug.LogError("No Scene was included");
			}
		}

		#region Utils
		public static void AddMapToBuild(Scene scene)
		{
			List<EditorBuildSettingsScene> build = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
			build.Add(new EditorBuildSettingsScene(scene.path, true));
			EditorBuildSettings.scenes = build.ToArray();
		}

		public static void AddMapToBuild(string path)
		{
			List<EditorBuildSettingsScene> build = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
			build.Add(new EditorBuildSettingsScene(path, true));
			EditorBuildSettings.scenes = build.ToArray();
		}

		public static bool IsMapInBuild(string path)
		{
			foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
			{
				if (scene.path == path)
				{
					return true;
				}
			}

			return false;
		}

		public static void RemoveMapFromBuild(string path)
		{
			List<EditorBuildSettingsScene> build = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
			for (int i = 0; i < build.Count; ++i)
			{
				if (build[i].path == path)
				{
					build.RemoveAt(i);
					break;
				}
			}
			EditorBuildSettings.scenes = build.ToArray();
		}

		public static void RemoveAllMapFromBuild()
		{
			List<EditorBuildSettingsScene> build = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
			foreach (EditorBuildSettingsScene scene in build)
			{
				RemoveMapFromBuild(scene.path);
			}
		}

		public static void RecreateLevelNameEnum(List<Level_Data> levelData)
		{
			bool hasMenu = false;

			string[] levelNameEnumRaw = new string[levelData.Count + 4];
			levelNameEnumRaw[0] = "public enum LevelName";
			levelNameEnumRaw[1] = "{";
			levelNameEnumRaw[2] = "\tNull,";
			for(int i = 0; i < levelData.Count; ++i)
			{
				levelNameEnumRaw[i + 3] = string.Format("\t{0},", levelData[i].Name);
				if (levelData[i].Name == "Menu")
				{
					hasMenu = true;
				}
			}
			levelNameEnumRaw[levelNameEnumRaw.Length - 1] = "}";

			if (!hasMenu)
			{
				Debug.LogError("A level must be called Menu in the LevelManager");
				return;
			}
			File.WriteAllLines(Application.dataPath + LEVEL_NAME_ENUM_FILE_PATH, levelNameEnumRaw);
		}
		#endregion
	}
}
#endif
