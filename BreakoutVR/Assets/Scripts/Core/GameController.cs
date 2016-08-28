using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GamejamToolset.Configs;
using UnityEngine.SceneManagement;
using GamejamToolset.Saving;
using GamejamToolset.LevelLoading;
using JetBrains.Annotations;
using UnityEditor;

public class GameController : Singleton<GameController>
{

	//=============================================================================================

	public static event Action OnGamePause;
	public static event Action OnGameResume;
	public static event Action OnGameQuit;

    public static event Action<LevelName> OnLevelPreStarted;
    public static event Action<LevelName> OnLevelStarted;
    public static event Action<LevelName> OnLevelPreEnded;
    public static event Action<LevelName> OnLevelEnded;

    public static event Action OnPlayStarted;
    public static event Action OnPlayEnded;

    //=============================================================================================

    public bool IsGamePaused { get { return m_isPause; } }
	public bool IsGameQuitting { get; private set; }

    public bool IsInPlayLevel
    {
        get { return ConfigHelper.OverrideAlwaysPlayLevel || IsLevelPlayLevel(LevelManager.Instance.CurrentLevel); }
    }

    public bool IsInLevelTransition { get { return LevelManager.Instance.IsInTransition; } }

	private bool m_isPause = false;
    private bool m_isPlayingLevel = false;

    //=============================================================================================
    //---------------------------------------------------------------------------------------------
    [UsedImplicitly]
    private void Awake()
    {
        if (!BootController.WasBooted)
        {
            Config.Init();
        }
    }

    //---------------------------------------------------------------------------------------------
    protected override void RegisterCallbacks()
    {
        base.RegisterCallbacks();

        LevelManager.OnLevelPreStart += Callback_OnLevelPreStarted;
        LevelManager.OnLevelStart += Callback_OnLevelStarted;
        LevelManager.OnLevelPreEnd += Callback_OnLevelPreEnded;
        LevelManager.OnLevelEnd += Callback_OnLevelEnded;
    }

    //---------------------------------------------------------------------------------------------
    protected override void UnregisterCallbacks()
    {
        base.UnregisterCallbacks();

        LevelManager.OnLevelPreStart -= Callback_OnLevelPreStarted;
        LevelManager.OnLevelStart -= Callback_OnLevelStarted;
        LevelManager.OnLevelPreEnd -= Callback_OnLevelPreEnded;
        LevelManager.OnLevelEnd -= Callback_OnLevelEnded;
    }
    
    #region Play Level
    //---------------------------------------------------------------------------------------------
    public void RequestPlayLevelStart()
    {
        if (!IsInPlayLevel || m_isPlayingLevel)
            return;

        if (OnPlayStarted != null)
        {
            OnPlayStarted();
        }
    }

    //---------------------------------------------------------------------------------------------
    public void RequestPlayLevelEnd()
    {
        if (!IsInPlayLevel || !m_isPlayingLevel)
            return;

        if (OnPlayEnded != null)
        {
            OnPlayEnded();
        }
    }

    #endregion

    #region Level
    //---------------------------------------------------------------------------------------------
    public void RequestMenuLoad()
    {
        LevelManager.Instance.RequestLoadLevel(LevelName.Menu);
    }

    //---------------------------------------------------------------------------------------------
    public void RequestPlayLevelLoad(uint index)
    {
        switch (index)
        {
            default:
                Debug.LogErrorFormat("Trying to load a level with index {0}. This play level doesn't exist");
                break;
        }
    }

    //---------------------------------------------------------------------------------------------
    public bool IsLevelPlayLevel(LevelName level)
    {
        return level != LevelName.Menu
            && level != LevelName.Null
            && !IsInLevelTransition;
    }

    //---------------------------------------------------------------------------------------------
    private void Callback_OnLevelPreStarted(LevelLoadCallbackInfo info)
    {
        if (OnLevelPreStarted != null)
            OnLevelPreStarted(info.Next);
    }

    //---------------------------------------------------------------------------------------------
    private void Callback_OnLevelStarted(LevelLoadCallbackInfo info)
    {
        if (OnLevelStarted != null)
            OnLevelStarted(info.Next);
    }

    //---------------------------------------------------------------------------------------------
    private void Callback_OnLevelPreEnded(LevelLoadCallbackInfo info)
    {
        //In case the player quits without finishing his level
        if (OnPlayEnded != null && IsInPlayLevel)
            OnPlayEnded();

        if (OnLevelPreEnded != null)
            OnLevelPreEnded(info.Next);
    }

    //---------------------------------------------------------------------------------------------
    private void Callback_OnLevelEnded(LevelLoadCallbackInfo info)
    {
        if (OnLevelEnded != null)
            OnLevelEnded(info.Next);
    }
    #endregion

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