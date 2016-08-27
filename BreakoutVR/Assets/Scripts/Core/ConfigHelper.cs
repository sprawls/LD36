using UnityEngine;
using GamejamToolset.Configs;

namespace GamejamToolset.Configs
{
	public enum ConfigCategory
	{
		Boot,
		Level,
		Gameplay,
		Debug
	}
}

/// <summary>
/// Config file access, use to set value even in build to test things
/// </summary>
public class ConfigHelper
{
	[Config("SkipIntro", ConfigCategory.Boot, SupportedConfigType.Bool)]
	[Description("Skip the intro screen during boot")]
	[DefaultValue(false)]
	public static bool BootSkipIntro = false;

	[Config("DebugStartingLevel", ConfigCategory.Level, SupportedConfigType.String)]
	[Description("Go directly to a level after boot")]
	[DefaultValue("Menu")]
	public static string StartingLevel = "Menu";
}