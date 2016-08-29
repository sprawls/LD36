using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using System;

public struct LevelLoadCallbackInfo
{
	public LevelName Previous { get; private set; }
	public LevelName Next { get; private set; }

	public LevelLoadCallbackInfo(LevelName previous, LevelName next) : this()
	{
		Previous = previous;
		Next = next;
	}
}

namespace GamejamToolset.LevelLoading
{
	public class LevelManager : Singleton<LevelManager>
	{
		[LargeHeader("Parameter"), SerializeField]
		private float m_defaultMinimumLoadingTime = 1f;

		[LargeHeader("Levels Prefabs"), SerializeField]
		private Levels_Data m_scenesData = new Levels_Data();

		//==========================================================================================

		public List<Level_Data> Levels { get { return m_scenesData.LevelsList; } }
		public Levels_Data RawLevelsData { get { return m_scenesData; } }
        public LevelName CurrentLevel { get {return m_currentLevel; } }
        public bool IsInTransition { get { return m_isInTransition; } }

		private LevelName m_currentLevel = LevelName.Null;
	    private bool m_isInTransition = false;

		//==========================================================================================

		public static event Action<LevelLoadCallbackInfo> OnLevelPreEnd;
		public static event Action<LevelLoadCallbackInfo> OnLevelEnd;
		public static event Action<LevelLoadCallbackInfo> OnLevelPreStart;
		public static event Action<LevelLoadCallbackInfo> OnLevelStart;

		//==========================================================================================
		public void RequestLoadLevel(LevelName name)
		{
			if (m_currentLevel == name)
				return;

			if (name == LevelName.Null)
			{
				Debug.LogError("Cannot use Null has a level name");
				return;
			}

			LoadLevel(name);
		}

		//------------------------------------------------------------------------------------
		/// <summary>
		/// WARNING: The LevelName enum should be used instead of the string name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public void RequestLoadLevel(string name)
		{
			LevelName levelToLoad = LevelName.Null;

			for (int i = 0; i < Enum.GetNames(typeof(LevelName)).Length; ++i)
			{
				LevelName level = (LevelName)i;
				if (level.ToString() == name)
				{
					levelToLoad = level;
				}
			}

			if (levelToLoad == LevelName.Null)
			{
				CustomLogger.LevelLogWarning(string.Format("Trying to load level with name: {0}. It does not exist, falling back to default.", name));
				levelToLoad = LevelName.Menu;
			}

			RequestLoadLevel(levelToLoad);
		}

		//------------------------------------------------------------------------------------
		private void LoadLevel(LevelName name)
		{
		    m_isInTransition = true;
			LevelLoadCallbackInfo info = new LevelLoadCallbackInfo(m_currentLevel, name);
			StopAllCoroutines();
			UnloadLevel(m_currentLevel, info);
			m_currentLevel = name;
			StartCoroutine(Coroutine_LoadingLevel(name, info));
		}

		//------------------------------------------------------------------------------------
		private IEnumerator Coroutine_LoadingLevel(LevelName name, LevelLoadCallbackInfo info)
		{
			CustomLogger.LevelLog(string.Format("Switching to level |{0}| - State: started", name.ToString()));
			yield return new WaitForSeconds(m_defaultMinimumLoadingTime);
			AsyncOperation loading = SceneManager.LoadSceneAsync(name.ToString(), LoadSceneMode.Single);

			bool preload = true;
			while (!loading.isDone)
			{
				if (preload && loading.progress > 0.9f)
				{
					preload = false;
					if (OnLevelPreStart != null)
						OnLevelPreStart(info);
				}

				yield return null;
			}

            GC.Collect();
            Shader.WarmupAllShaders();

            HMDController.Instance.RequestVrRoomExit();

            CustomLogger.LevelLog(string.Format("Switching to level |{0}| - State: ended", name.ToString()));
			if (OnLevelStart != null)
				OnLevelStart(info);

		    m_isInTransition = false;
		}

		//------------------------------------------------------------------------------------s
		private void UnloadLevel(LevelName name, LevelLoadCallbackInfo info)
		{
			if (OnLevelPreEnd != null)
				OnLevelPreEnd(info);

            HMDController.Instance.RequestVrRoomEnter();
			CustomLogger.LevelLog(string.Format("Unloading scene |{0}|", name.ToString()));
			SceneManager.UnloadScene(name.ToString());

			if (OnLevelEnd != null)
				OnLevelEnd(info);
		}
	}
}