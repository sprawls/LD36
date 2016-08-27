#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using GamejamToolset.LevelLoading;
using GamejamToolset.Defines;

namespace GamejamToolset.Builder 
{
	public enum BuildType 
	{
		Debug,
		Profile,
		Final
	}

	public static class CustomBuilder
	{
		public static bool IsBuilding { get { return s_isBuilding; } }
		private static bool s_isBuilding = false;

		public static void BuildPlayer(BuildType buildType, BuildTarget platform)
		{
			if (!IsSupportedPlatform(platform))
			{
				CustomLogger.BuildLogError("Trying to build on a platform that is not supported (BuildOptions.SupportedPlatforms). Aborting Build");
				return;
			}

			s_isBuilding = true;
			CustomLogger.BuildLog("Starting Build Setup");

			string path = EditorUtility.SaveFolderPanel("Choose Folder for Build", "", "");
			CustomLogger.BuildLog("Building at path: " + path);

			CustomLogger.BuildLog(string.Format("Target platform: {0}", platform.ToString()));
			if (EditorUserBuildSettings.activeBuildTarget != platform)
			{
				CustomLogger.BuildLog(string.Format("Switching to desired platform", platform.ToString()));
				EditorUserBuildSettings.SwitchActiveBuildTarget(platform);
			}

			bool includedOnly = !EditorUtility.DisplayDialog("Choose Level to Build", "Choose the level to load from the list", "Build All", "Build Included Only");
			CustomLogger.BuildLog("Building Map");

			//Build Levels
			LevelBuilder.CreateBuildLevels(EditorDataObject.Instance.LevelsData.LevelsList, includedOnly);

			string[] levels = new string[EditorBuildSettings.scenes.Length];
			for (int i = 0; i < levels.Length; ++i)
			{
				levels[i] = EditorBuildSettings.scenes[i].path;
			}

			if (!EditorUtility.DisplayDialog(
				"Global Defines", 
				string.Format("The build will use the default Global Defines in {0}. Are you sure you want to continue?", GlobalDefinesManager.GlobalDefinesFilePath),
				"Start Building", 
				"Abort"
			))
			{
				CustomLogger.BuildLog("Aborting build");
				s_isBuilding = false;
				return;
			}

			//Update Defines
			GlobalDefinesManager.UpdateDefines(GlobalDefinesManager.GlobalDefinesFilePath);

			//Build Player
			CustomLogger.BuildLog("Player building started");
			BuildPipeline.BuildPlayer(levels, path, platform, GetBuildOptions(buildType));
			CustomLogger.BuildLog("Player building ended");

			s_isBuilding = false;
		}

		private static bool IsSupportedPlatform(BuildTarget platform)
		{
			return BuildOptions.SupportedPlatforms.Contains(platform);
		}

		private static UnityEditor.BuildOptions GetBuildOptions(BuildType buildType)
		{
			switch (buildType)
			{
				case BuildType.Debug:
					return UnityEditor.BuildOptions.Development | UnityEditor.BuildOptions.AllowDebugging;

				case BuildType.Profile:
					return UnityEditor.BuildOptions.Development | UnityEditor.BuildOptions.AllowDebugging | UnityEditor.BuildOptions.ConnectWithProfiler;

				case BuildType.Final:
				default:
					return UnityEditor.BuildOptions.None;
			}
		}
	}
}
#endif
