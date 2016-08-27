#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using GamejamToolset.LevelLoading;

#region Header
public class LargeHeaderAttribute : TitleAttribute
{
	protected override GUIStyle GetStyle()
	{
		GUIStyle style = new GUIStyle();

		style.fontStyle = FontStyle.Bold;
		style.fontSize = 20;
		style.alignment = TextAnchor.MiddleCenter;
		style.fixedHeight = 35;
		style.normal.textColor = new Color(0.9f, 0.9f, 0.9f);

		return style;
	}

	public override Color BackgroundColor
	{
		get { return new Color(0.5f, 0.5f, 0.5f); }
	}

	public LargeHeaderAttribute(string title) : base(title.ToUpper(), 35) {}
}

public abstract class TitleAttribute : PropertyAttribute
{
	public string Title { get; private set; }
	public float Height { get; private set; }
	public GUIStyle Style 
	{
		get
		{
			if (m_style == null)
			{
				m_style = GetStyle();
			}

			return m_style;
		}
	}

	public abstract Color BackgroundColor { get; }

	private GUIStyle m_style = null;

	protected abstract GUIStyle GetStyle();

	public TitleAttribute(string title, float height)
	{
		Title = title;
		Height = height;
	}
}


[CustomPropertyDrawer(typeof(TitleAttribute))]
[CustomPropertyDrawer(typeof(LargeHeaderAttribute))]
public class TitleDrawer : DecoratorDrawer
{
	TitleAttribute titleAttribute = null;

	public override float GetHeight()
	{
		SetupAttributeRef();

		return titleAttribute.Height + 15;
	}

	public override void OnGUI(Rect position)
	{
		base.OnGUI(position);

		EditorGUI.indentLevel = 0;

		SetupAttributeRef();

		Texture2D texture = new Texture2D(1, 1);
		texture.SetPixel(0, 0, titleAttribute.BackgroundColor);
		texture.Apply();

		GUIStyle style = new GUIStyle();
		style.normal.background = texture;

		position.y += 10;
		position.height -= 15;

		EditorGUI.LabelField(position, GUIContent.none, style);
		EditorGUI.LabelField(position, titleAttribute.Title, titleAttribute.Style);
	}

	private void SetupAttributeRef()
	{
		if (titleAttribute == null)
		{
			titleAttribute = attribute as TitleAttribute;
		}
	}

	private void Separator(Rect position)
	{
		GUIStyle style = new GUIStyle(GUI.skin.horizontalSlider);
		style.fixedHeight = 35f;

		EditorGUI.LabelField(position, "", GUI.skin.horizontalSlider);
	}
}
#endregion

#region Readonly
public class InspectorReadOnlyAttribute : PropertyAttribute {}

[CustomPropertyDrawer(typeof(InspectorReadOnlyAttribute))]
public class InspectorReadOnlyDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(property, label, true);
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		GUI.enabled = false;
		EditorGUI.PropertyField(position, property, label, true);
		GUI.enabled = true;
	}
}
#endregion
#else
//For build
using System;

public class LargeHeaderAttribute : Attribute
{
	public LargeHeaderAttribute(string title) {}
}

public class InspectorReadOnlyAttribute : Attribute { }
#endif