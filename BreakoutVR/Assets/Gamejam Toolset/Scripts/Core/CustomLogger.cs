using UnityEngine;
using System.Collections;

public enum LogCategory 
{
	Debug,
	Boot,
	Level_Loading,
	Build,
}

public enum LogType 
{
	Normal,
	Warning,
	Error
}

public abstract class CustomLogger
{
	public static void LogWarning(string message, LogCategory category = LogCategory.Debug)
	{
		Log(message, category, LogType.Warning);
	}

	public static void LogError(string message, LogCategory category = LogCategory.Debug)
	{
		Log(message, category, LogType.Error);
	}

	public static void Log(string message, LogCategory category = LogCategory.Debug, LogType type = LogType.Normal)
	{
		Log(message, type, category);
	}

	private static void Log(string message, LogType type, LogCategory category, string colorCode = "#ff6600")
	{
		string final = string.Format("<b><color={3}>{0}{1} -></color> {2}</b>", category.ToString().ToUpper(), type == LogType.Normal ? "" : string.Format(" {0}", type.ToString().ToUpper()), message, colorCode);

		switch (type)
		{
			case LogType.Normal:
				Debug.Log(final);
				break;

			case LogType.Warning:
				Debug.LogWarning(final);
				break;

			case LogType.Error:
				Debug.LogError(final);
				break;
		}
	}

	public static void BootLog(string message, LogType type = LogType.Normal)
	{
		Log(message, type, LogCategory.Boot, "#006622");
	}

	public static void BootLogError(string message)
	{
		BootLog(message, LogType.Error);
	}

	public static void BootLogWarning(string message)
	{
		BootLog(message, LogType.Warning);
	}

	public static void LevelLog(string message, LogType type = LogType.Normal)
	{
		Log(message, type, LogCategory.Level_Loading, "#006bb3");
	}

	public static void LevelLogError(string message)
	{
		LevelLog(message, LogType.Error);
	}

	public static void LevelLogWarning(string message)
	{
		LevelLog(message, LogType.Warning);
	}

	public static void BuildLog(string message, LogType type = LogType.Normal)
	{
		Log(message, type, LogCategory.Build, "#006622");
	}

	public static void BuildLogError(string message)
	{
		BuildLog(message, LogType.Error);
	}

	public static void BuildLogWarning(string message)
	{
		BuildLog(message, LogType.Warning);
	}
}
