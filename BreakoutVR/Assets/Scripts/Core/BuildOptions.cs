#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;

public static class BuildOptions
{
	public static readonly List<BuildTargetGroup> SupportedPlatformsGroup = new List<BuildTargetGroup>()
	{
		BuildTargetGroup.Standalone,
	};

	public static readonly List<BuildTarget> SupportedPlatforms = new List<BuildTarget>()
	{
		BuildTarget.StandaloneWindows64,
		BuildTarget.StandaloneOSXIntel64,
		BuildTarget.StandaloneLinux64,
	};
}
#endif
