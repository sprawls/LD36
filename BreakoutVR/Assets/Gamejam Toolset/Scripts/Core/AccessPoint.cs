using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GamejamToolset.Saving;
using GamejamToolset.LevelLoading;
//using GamejamToolset.Audio;

//===========================================================================================
public static class Access_SaveGameController
{
	/// <summary> This shouldn't be called outside the boot </summary>
	public static void Init()
	{
		CustomLogger.BootLog("Save Game Controller Access - INIT");
	}

	/// <summary> Request the game to be saved in it's current status </summary>
	public static void RequestSave()
	{
		SaveGameController.Instance.RequestSave();
	}

	/// <summary> Request the game to be loaded from the save file </summary>
	public static void RequestLoad()
	{
		SaveGameController.Instance.RequestSave();
	}
}

//===========================================================================================
public static class Access_LevelManager
{
	/// <summary> Event when the level is about to be changed </summary>
	public static event Action<LevelLoadCallbackInfo> OnLevelPreEnd;

	/// <summary> Event when the level is ending and switching to the next one </summary>
	public static event Action<LevelLoadCallbackInfo> OnLevelEnd;

	/// <summary> Event when the level is about to start but not loaded yet </summary>
	public static event Action<LevelLoadCallbackInfo> OnLevelPreStart;

	/// <summary> Event when the level has been loaded and is starting </summary>
	public static event Action<LevelLoadCallbackInfo> OnLevelStart;

	private static LevelManager levelManager;


	/// <summary> This shouldn't be called outside the boot </summary>
	public static void Init()
	{
		CustomLogger.BootLog("Level Manager Access - INIT");
		LevelManager.OnLevelPreEnd += OnLevelPreEnd;
		LevelManager.OnLevelEnd += OnLevelEnd;
		LevelManager.OnLevelPreStart += OnLevelPreStart;
		LevelManager.OnLevelStart += OnLevelStart;
	}

	/// <summary> Request the menu to be loaded </summary>
	public static void RequestLoadMenu()
	{
		LevelManager.Instance.RequestLoadLevel(LevelName.Menu);
	}

	/// <summary> Request a level to be loaded from is LevelName value </summary>
	public static void RequestLoadLevel(LevelName name)
	{
		LevelManager.Instance.RequestLoadLevel(name);
	}

	/// <summary> Request a level to be loaded from is string name. WARNING: using LevelName is way better in term of performance and code safety. </summary>
	public static void RequestLoadLevel(string name)
	{
		LevelManager.Instance.RequestLoadLevel(name);
	}
}

#if false
//===========================================================================================
public static class Access_AudioController
{
	/// <summary> Event the sound is paused </summary>
	public static event Action OnSoundPause;

	/// <summary> Event the sound is resumed </summary>
	public static event Action OnSoundResume;

	/// <summary> Event the music is stopped </summary>
	public static event Action<int> OnMusicStop;

	/// <summary> Event the music id started </summary>
	public static event Action<int> OnMusicStart;

	/// <summary> This shouldn't be called outside the boot </summary>
	public static void Init()
	{
		CustomLogger.BootLog("Sound Controller Access - INIT");
		SoundController.OnSoundResume += OnSoundResume;
		SoundController.OnSoundStop += OnSoundPause;
		MusicController.OnMusicStart += OnMusicStop;
		MusicController.OnMusicStop += OnMusicStart;
	}

	/// <summary> Post a sound event to the SoundManager with the audio source to play the sound at. </summary>
	public static void PostSoundEvent(SoundEvent soundEvent, AudioSource source)
	{
		SoundController.Instance.PostEvent(soundEvent, source);
	}

	/// <summary> Request the Sound to be resume after being paused </summary>
	public static void RequestSoundResume()
	{
		SoundController.Instance.RequestAudioResume();
	}

	/// <summary> Request the to be paused </summary>
	public static void RequestSoundPause()
	{
		SoundController.Instance.RequestAudioPause();
	}

	/// <summary>
	/// Request a change of music with a given ID
	/// </summary>
	/// <param name="ID"> The ID of the music track in the list </param>
	/// <param name="transitionTime"> The transition time from the previous track to the next (fading between the two) </param>
	/// <param name="loop">Should the track loop. True by default. </param>
	public static void RequestMusicChange(int ID, float transitionTime = -1.0f, bool loop = true)
	{
		MusicController.Instance.RequestMusicChange(ID, transitionTime, loop);
	}

	/// <summary> Request the music to resume after being paused </summary>
	public static void RequestMusicResume()
	{
		MusicController.Instance.RequestMusicResume();
	}

	/// <summary> Request the music to pause </summary>
	public static void RequestMusicPause()
	{
		MusicController.Instance.RequestMusicPause();
	}
}
#endif