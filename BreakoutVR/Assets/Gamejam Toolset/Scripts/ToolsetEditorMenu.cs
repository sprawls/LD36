#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using GamejamToolset.LevelLoading;
using GamejamToolset.Documentation;
using GamejamToolset.Configs;
using GamejamToolset.Defines;
using GamejamToolset.Builder;

namespace GamejamToolset
{
	public class ToolsetEditorMenu
	{

		private const string TITLE = "Gamejam Toolset/";
		private const string UTILS_MENU = "Utils/";
		private const string TOOLSET_INFO = "Toolset Information/";
		private const string CONFIGS = "Configs/";
		private const string DEFINES = "Custom Global Defines/";

		#region Toolset Menu
		//[MenuItem(TITLE + "Open Toolset Documentation", false, 10)]
		//private static void OpenDocumentation()
		//{
		//	DocumentationStarter.OpenDocumentation();
		//}

		[MenuItem(TITLE + TOOLSET_INFO + "About", false, 15)]
		private static void About()
		{
			EditorWindow window = EditorWindow.GetWindow(typeof(ToolsetAboutWindow));
			window.Show();
		}

		[MenuItem(TITLE + TOOLSET_INFO + "List of Plugins", false, 15)]
		private static void PluginList()
		{
			EditorWindow window = EditorWindow.GetWindow(typeof(ToolsetPluginListWindow));
			window.Show();
		}
		#endregion

		#region Level Manager
		[MenuItem(TITLE + "Level Builder", false, 30)]
		private static void ToggleLevelEditorWindow()
		{
			LevelManagerWindow.OpenWindow();
		}
		#endregion

		#region Config
		[MenuItem(TITLE + CONFIGS + "Open", false, 500)]
		private static void OpenConfigFile()
		{
			Application.OpenURL(string.Format("file:///{0}", Config.ConfigFilePath));	
		}

		[MenuItem(TITLE + CONFIGS + "Generate Default", false, 500)]
		private static void GenerateDefaultConfig()
		{
			if (Application.isPlaying)
				return;

			int choice = EditorUtility.DisplayDialogComplex(
				"Generating Default Config",
				"This action will overwrite the current config. Are you sure you want to continue?",
				"Yes",
				"No",
				"Hell Naw!"
			);

			if (choice == 0)
			{
				Config.Generate();
				Application.OpenURL(string.Format("file:///{0}", Config.ConfigFilePath));
			}
		}
		#endregion

		#region Global Defines
		[MenuItem(TITLE + DEFINES + "Edit", false, 500)]
		private static void EditGlobalDefines()
		{
			Application.OpenURL(string.Format("file:///{0}", GlobalDefinesManager.GlobalDefinesFilePath));
		}

		[MenuItem(TITLE + DEFINES + "Update", false, 500)]
		private static void UpdateGlobalDefines()
		{
			GlobalDefinesManager.UpdateDefines(GlobalDefinesManager.GlobalDefinesFilePath);
		}
		#endregion

		#region Utils
		[MenuItem(TITLE + UTILS_MENU + "Take ScreenShot", false, 1000)]
		public static void TakeScreenshot()
		{
			CaptureScreenShot.TakeScreenShot();
		}
		#endregion

		#region CustomBuilder
		const int buildFolderPriority = 10;

		[MenuItem("File/Custom Builder/Windows/Debug", false, buildFolderPriority)]
		private static void Build_Debug_WindowsX64()
		{
			CustomBuilder.BuildPlayer(BuildType.Debug, BuildTarget.StandaloneWindows64);
		}

		[MenuItem("File/Custom Builder/Windows/Profile", false, buildFolderPriority)]
		private static void Build_Profile_WindowsX64()
		{
			CustomBuilder.BuildPlayer(BuildType.Profile, BuildTarget.StandaloneWindows64);
		}

		[MenuItem("File/Custom Builder/Windows/Final", false, buildFolderPriority)]
		private static void Build_Final_WindowsX64()
		{
			CustomBuilder.BuildPlayer(BuildType.Final, BuildTarget.StandaloneWindows64);
		}

		[MenuItem("File/Custom Builder/OSX/Debug", false, buildFolderPriority)]
		private static void Build_Debug_OSX()
		{
			CustomBuilder.BuildPlayer(BuildType.Debug, BuildTarget.StandaloneOSXIntel64);
		}

		[MenuItem("File/Custom Builder/OSX/Profile", false, buildFolderPriority)]
		private static void Build_Profile_OSX()
		{
			CustomBuilder.BuildPlayer(BuildType.Profile, BuildTarget.StandaloneOSXIntel64);
		}

		[MenuItem("File/Custom Builder/OSX/Final", false, buildFolderPriority)]
		private static void Build_Final_OSX()
		{
			CustomBuilder.BuildPlayer(BuildType.Final, BuildTarget.StandaloneOSXIntel64);
		}

		[MenuItem("File/Custom Builder/Linux/Debug", false, buildFolderPriority)]
		private static void Build_Debug_Linux()
		{
			CustomBuilder.BuildPlayer(BuildType.Debug, BuildTarget.StandaloneLinux64);
		}

		[MenuItem("File/Custom Builder/Linux/Profile", false, buildFolderPriority)]
		private static void Build_Profile_Linux()
		{
			CustomBuilder.BuildPlayer(BuildType.Profile, BuildTarget.StandaloneLinux64);
		}

		[MenuItem("File/Custom Builder/Linux/Final", false, buildFolderPriority)]
		private static void Build_Final_Linux()
		{
			CustomBuilder.BuildPlayer(BuildType.Final, BuildTarget.StandaloneLinux64);
		}
		#endregion
	}
}
#endif
