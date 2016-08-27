#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

public class EditorGUIUtils 
{
	public static void Title_H1(string title)
	{
		GUILayout.Space(10);
		GUIStyle style = new GUIStyle();

		style.fontStyle = FontStyle.Bold;
		style.fontSize = 20;
		style.alignment = TextAnchor.MiddleCenter;
		style.fixedHeight = 35;
		style.normal.textColor = new Color(0.9f, 0.9f, 0.9f);

		Texture2D texture = new Texture2D(1, 1);
		texture.SetPixel(0, 0, new Color(0.5f, 0.5f, 0.5f));
		texture.Apply();
		style.normal.background = texture;

		EditorGUILayout.LabelField(title.ToUpper(), style);
		GUILayout.Space(20);
	}

	public static void Title_H2(string title)
	{
		GUIStyle style = new GUIStyle();

		style.fontStyle = FontStyle.Bold;
		style.fontSize = 15;
		style.alignment = TextAnchor.MiddleCenter;
		style.fixedHeight = 20;
		style.normal.textColor = new Color(0.9f, 0.9f, 0.9f);

		Texture2D texture = new Texture2D(1, 1);
		texture.SetPixel(0, 0, new Color(0.6f, 0.6f, 0.6f));
		texture.Apply();
		style.normal.background = texture;

		EditorGUILayout.LabelField(title, style);
		GUILayout.Space(10);
	}
}
#endif