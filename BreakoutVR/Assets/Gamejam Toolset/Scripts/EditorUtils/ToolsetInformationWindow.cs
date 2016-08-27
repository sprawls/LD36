#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

public class ToolsetAboutWindow : EditorWindow 
{
	private void OnGUI()
	{
		EditorGUIUtils.Title_H1("About");
		EditorGUILayout.Space();

		EditorGUIUtils.Title_H2("Version 1.0");
		EditorGUIUtils.Title_H2("Made by Antoine Brouillette");
	}
}

public class ToolsetPluginListWindow : EditorWindow
{
	Vector2 m_position = Vector2.zero;

	private void OnGUI()
	{
		EditorGUIUtils.Title_H1("List of Plugins");
		EditorGUILayout.Space();

		EditorGUILayout.BeginScrollView(m_position);
		EditorGUILayout.BeginVertical();

		PluginInfoGUI
		(
			"DOTween",
			"",
			"http://dotween.demigiant.com/"
		);

		PluginInfoGUI
		(
			"InControl",
			"",
			"http://www.gallantgames.com/pages/incontrol-introduction"
		);

		PluginInfoGUI
		(
			"Primitive Plus",
			"",
			"https://www.assetstore.unity3d.com/en/#!/content/25542"
		);

		EditorGUILayout.EndVertical();
		EditorGUILayout.EndScrollView();
	}

	private void PluginInfoGUI(string name, string description, string link)
	{
		EditorGUIUtils.Title_H2(name);
		GUILayout.Space(5);
		EditorGUILayout.LabelField(description);

		if (GUILayout.Button("Documentation"))
		{
			Application.OpenURL(link);
		}

		GUILayout.Space(10);
	}
}
#endif
