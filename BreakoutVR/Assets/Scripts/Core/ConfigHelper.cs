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

    [Config("OverrideAlwaysPlayLevel", ConfigCategory.Level, SupportedConfigType.Bool)]
    [Description("Override the game to always be in play level")]
#if UNITY_EDITOR
    [DefaultValue(true)]
    public static bool OverrideAlwaysPlayLevel = true;
#else
    [DefaultValue(false)]
    public static bool OverrideAlwaysPlayLevel = false;
#endif

    [Config("DebugStartingLevel", ConfigCategory.Level, SupportedConfigType.String)]
	[Description("Go directly to a level after boot")]
	[DefaultValue("Menu")]
	public static string StartingLevel = "Menu";
}