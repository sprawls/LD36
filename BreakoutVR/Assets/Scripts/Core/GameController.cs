using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using GamejamToolset.Saving;
using UnityEditor;

public class GameController : Singleton<GameController>
{

	//=============================================================================================

	public static event Action OnGamePause;
	public static event Action OnGameResume;
	public static event Action OnGameQuit;

	//=============================================================================================

	public bool IsGamePaused { get { return m_isPause; } }
	public bool IsGameQuitting { get; private set; }

	private bool m_isPause = false;

	//=============================================================================================

	void Awake()
	{
	}

	#region Quitting
	//---------------------------------------------------------------------------------------------
	public void RequestQuitGame()
	{
		if (!Application.isPlaying)
			return;

		if (IsGameQuitting)
			return;

		IsGameQuitting = true;

		if (OnGameQuit != null)
		{
			OnGameQuit();
		}

		Debug.Log("<Quitting Game>");
#if UNITY_EDITOR
	    EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}
    #endregion

    #region Pausing
    //---------------------------------------------------------------------------------------------
    public void RequestGamePause()
	{
		m_isPause = true;

		/*Access_AudioController.RequestMusicPause();
		Access_AudioController.RequestSoundPause();*/

		if (OnGamePause != null)
		{
			OnGamePause.Invoke();
		}
	}

	//---------------------------------------------------------------------------------------------
	public void RequestGameResume()
	{
		m_isPause = false;

		/*Access_AudioController.RequestMusicResume();
		Access_AudioController.RequestSoundResume();*/

		if (OnGameResume != null)
		{
			OnGameResume.Invoke();
		}
	}

#endregion
}