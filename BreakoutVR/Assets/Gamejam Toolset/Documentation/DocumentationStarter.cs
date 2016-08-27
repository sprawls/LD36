#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

namespace GamejamToolset.Documentation
{
	public static class DocumentationStarter
	{
		public static DocumentationWindow window = null;

		public static void OpenDocumentation()
		{
			if (window == null)
			{
				window = EditorWindow.GetWindow<DocumentationWindow>();
			}

			window.Show();
		}
	}

	public class DocumentationWindow : EditorWindow
	{
		private DocumentationWindow Instance = null;

		//----------------------------------------------------------------------------

		private List<DocumentationPage> m_pages = null;
		private string m_pathToDoc { get { return Application.dataPath + "/Gamejam Toolset/Documentation/Pages/"; } }
		private int m_currentPageID = 0;
		private GUIStyle m_H1Style;
		private GUIStyle m_H2Style;
		private GUIStyle m_H3Style;
		private GUIStyle m_basicStyle;

		//----------------------------------------------------------------------------
		private void CreatePages()
		{
			m_pages = new List<DocumentationPage>();

			m_pages.Add(new DocumentationPage("Home", m_pathToDoc + "Index.txt"));
			m_pages.Add(new DocumentationPage("Persistence", m_pathToDoc + "Documentation_Persistence.txt"));
			m_pages.Add(new DocumentationPage("Game Controller", m_pathToDoc + "Documentation_GameController.txt"));
			m_pages.Add(new DocumentationPage("Base UI Stuff", m_pathToDoc + "Documentation_BaseUIStuff.txt"));
			m_pages.Add(new DocumentationPage("Input Controller", m_pathToDoc + "Documentation_InputController.txt"));
			m_pages.Add(new DocumentationPage("Level Manager", m_pathToDoc + "Documentation_LevelManager.txt"));
			m_pages.Add(new DocumentationPage("Save Manager", m_pathToDoc + "Documentation_SaveManager.txt"));
			m_pages.Add(new DocumentationPage("Loading Controller", m_pathToDoc + "Documentation_LoadingController.txt"));
			m_pages.Add(new DocumentationPage("Audio Controller", m_pathToDoc + "Documentation_AudioController.txt"));
			m_pages.Add(new DocumentationPage("Configs", m_pathToDoc + "Documentation_Configs.txt"));
			m_pages.Add(new DocumentationPage("Custom Builder", m_pathToDoc + "Documentation_CustomBuilder.txt"));
			m_pages.Add(new DocumentationPage("Custom Logger", m_pathToDoc + "Documentation_CustomLogger.txt"));
		}

		//----------------------------------------------------------------------------
		private void SetStyles()
		{
			m_H1Style = new GUIStyle(EditorStyles.largeLabel)
			{
				fontStyle = FontStyle.Bold,
				fontSize = 45,
				fixedHeight = 55,
			};

			m_H2Style = new GUIStyle(EditorStyles.largeLabel)
			{
				fontStyle = FontStyle.Bold,
				fontSize = 30,
				fixedHeight = 40
			};

			m_H3Style = new GUIStyle(EditorStyles.largeLabel)
			{
				fontStyle = FontStyle.Bold,
				fontSize = 23,
				fixedHeight = 33
			};

			m_basicStyle = new GUIStyle(EditorStyles.label)
			{
				richText = true,
				fontSize = 14,
				fixedHeight = 24
			};
		}

		//----------------------------------------------------------------------------
		private void OnGUI()
		{
			if (Instance == null)
			{
				Instance = EditorWindow.GetWindow<DocumentationWindow>();
				CreatePages();
				SetStyles();
			}

			if (m_pages == null)
				return;

			BlockWindowSize();
			WindowTitleGUI();
			NavGUI();
			GUILayout.Space(20);
			CurrentPageGUI(m_currentPageID);
		}

		//----------------------------------------------------------------------------
		private void BlockWindowSize()
		{
			Instance.maxSize = new Vector2(700, 800);
			Instance.minSize = new Vector2(700, 800);
		}

		//----------------------------------------------------------------------------
		private void WindowTitleGUI()
		{
			EditorGUILayout.LabelField("GAMEJAM TOOLSET", m_H1Style);
			GUILayout.Space(40);
		}

		//----------------------------------------------------------------------------
		private void NavGUI()
		{
			//Change color by category
			GUILayout.BeginHorizontal();
			for (int i = 0; i < m_pages.Count; ++i)
			{
				if (i != 0 && i % 5 == 0)
				{
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
				}

				if (i == m_currentPageID)
					GUI.enabled = false;

				if (GUILayout.Button(m_pages[i].Title, GUILayout.Width(135), GUILayout.Height(30)))
				{
					m_currentPageID = i;
				}

				GUI.enabled = true;
			}
			GUILayout.EndHorizontal();
		}

		//----------------------------------------------------------------------------
		private void CurrentPageGUI(int id)
		{
			if (id >= m_pages.Count)
				return;

			EditorGUILayout.LabelField(m_pages[id].Title, m_H2Style);
			GUILayout.Space(10);

			foreach (ParsedText line in m_pages[id].Lines)
			{
				switch (line.TextType)
				{
					case ParsedText.Type.Basic:
						EditorGUILayout.LabelField(line.RawText, m_basicStyle);
						break;

					case ParsedText.Type.ListElement:
						EditorGUILayout.LabelField(string.Format("    - {0}", line.RawText), m_basicStyle);
						break;

					case ParsedText.Type.Title:
						GUILayout.Space(20);
						EditorGUILayout.LabelField(line.RawText, m_H3Style);
						GUILayout.Space(15);
						break;

					case ParsedText.Type.Image:

						break;
				}
			}
		}

		//----------------------------------------------------------------------------
		private void OnDisable()
		{
			Instance = null;
		}

		//----------------------------------------------------------------------------
		private void OnDestroy()
		{
			Instance = null;
		}
	}

	public class DocumentationPage
	{
		public string Title { get; private set; }
		public ParsedText[] Lines { get; private set; }

		public DocumentationPage(string title, string path)
		{
			Title = title;

			if (!File.Exists(path))
			{
				Debug.LogError("Could not find documentation page at path " + path);
				Lines = new ParsedText[0];
				return;
			}

			Lines = Parse(File.ReadAllLines(path));
		}

		private ParsedText[] Parse(string[] lines)
		{
			ParsedText[] parsedLines = new ParsedText[lines.Length];
			for (int i = 0; i < lines.Length; ++i)
			{
				string current = lines[i];
				ParsedText.Type type = ParsedText.Type.Basic;
				string rawText = "";

				if (current.Contains("<title>"))
				{
					type = ParsedText.Type.Title;
					rawText = current.Replace("<title>", "");
				}
				else if (current.Contains("<l>"))
				{
					type = ParsedText.Type.ListElement;
					rawText = current.Replace("<l>", "");
				}
				else if (current.Contains("<image="))
				{
					type = ParsedText.Type.Image;
					rawText = GetImagePath(current);
				}
				else
				{
					type = ParsedText.Type.Basic;
					rawText = current;
				}

				parsedLines[i] = new ParsedText()
				{
					TextType = type,
					RawText = rawText
				};
			}

			return parsedLines;
		}

		private string GetImagePath(string line)
		{
			string result = "";
			//bool insideTag = false;
			line = line.Replace("image=", "");

			for (int i = 0; i < line.Length; ++i)
			{
				char current = line[i];
				switch (current)
				{
					case '<': 
						//insideTag = true; 
						break;

					case '>': 
						return result; 

					default: 
						result += current; 
						break;
				}
			}

			return result;
		}
	}

	public class ParsedText
	{
		public enum Type
		{
			Title,
			Image,
			ListElement,
			Basic
		}

		public string RawText;
		public Type TextType;
	}
}
#endif
