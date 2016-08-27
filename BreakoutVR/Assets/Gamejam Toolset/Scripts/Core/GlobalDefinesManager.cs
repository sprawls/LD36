#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.IO;

namespace GamejamToolset.Defines
{
	public static class GlobalDefinesManager
	{
		public static readonly string GlobalDefinesFilePath = Application.dataPath + "/CustomGlobalDefines.ini";

		//-------------------------------------------------------------------------------
		public static void UpdateDefines(string path)
		{
			string defines = "";
			bool hasDefine = false;

			CustomLogger.BuildLog("Updating Global Defines");
			string[] lines = File.ReadAllLines(path);
			for (int i = 0; i < lines.Length; ++i)
			{
				bool isDisabled = false;
				string define = ParseForDefine(lines[i], out isDisabled);

				if (define.Length > 0)
				{
					hasDefine = true;

					CustomLogger.BuildLog(string.Format("{0} - {1}", define, isDisabled ? "Disabled" : "Enabled"));

					if (!isDisabled)
						defines += string.Format("{0};", define);
				}
			}

			if (defines.Length > 0)
			{
				//Set defines on all platforms
				for (int i = 0; i < BuildOptions.SupportedPlatformsGroup.Count; ++i)
				{
					PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildOptions.SupportedPlatformsGroup[i], defines);
				}
			}
			else if (!hasDefine)
			{
				CustomLogger.BuildLog("No global defines in the file");
			}
		}

		//-------------------------------------------------------------------------------
		private static string ParseForDefine(string line, out bool isDisabled)
		{
			string define = "";
			bool wasLetter = false;
			bool wasSlash = false;
			isDisabled = false;

			for (int i = 0; i < line.Length; ++i)
			{
				if (line[i] == '/')
				{
					if (wasSlash)
						return define;
					else
						wasSlash = true;
				}

				if (char.IsLetter(line[i]) || line[i] == '_')
				{
					wasSlash = false;
					wasLetter = true;
					define += line[i];
				}
				else if (wasLetter)
				{
					return define;
				}
				else if (line[i] == '-')
				{
					isDisabled = true;
				}
			}
			return define;
		}
	}
}
#endif
