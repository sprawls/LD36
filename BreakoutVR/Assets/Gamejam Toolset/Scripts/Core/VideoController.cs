using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class VideoController : Singleton<VideoController>
{
	[LargeHeader("UI Data"), SerializeField]
	private RawImage m_videoRawImage;

	[SerializeField]
	private AudioSource m_audioSource;

	[SerializeField]
	private UIFadeable m_fadeable;

	[LargeHeader("List of Videos"), SerializeField]
	private MovieTexture[] m_videos;

	//============================================================================

	public static event Action<int> OnVideoStartPlaying;
	public static event Action OnVideoPause;
	public static event Action OnVideoResume;
	public static event Action<int> OnVideoStopPlaying;

	//============================================================================

	public bool IsPaused { get { return m_isPaused; } }

	private Texture2D m_blackTexture = null;
	private MovieTexture m_currentVideo = null;
	private int m_currentVideoID = -1;
	private bool m_isPaused = false;

	//============================================================================
	//---------------------------------------------------------------
	private void Awake()
	{
		if (m_videoRawImage == null)
		{
			Debug.LogError("VideoController raw image ref is null");
		}

		if (m_audioSource == null)
		{
			Debug.LogError("VideoController audio source ref is null");
		}

		if (m_fadeable == null)
		{
			Debug.LogError("VideoController fadeable ref is null");
		}

		m_blackTexture = new Texture2D(1, 1);
		m_blackTexture.SetPixel(0, 0, Color.black);
		m_blackTexture.SetPixel(1, 0, Color.black);
		m_blackTexture.SetPixel(0, 1, Color.black);
		m_blackTexture.SetPixel(1, 1, Color.black);
		m_blackTexture.Apply();

		m_videoRawImage.texture = m_blackTexture;
	}

	//---------------------------------------------------------------
	private void Update()
	{
		if (m_isPaused)
			return;

		if (m_currentVideo == null)
			return;

		if (!m_currentVideo.isPlaying)
			RequestStopVideo();
	}

	//---------------------------------------------------------------
	protected override void RegisterCallbacks()
	{
		GameController.OnGamePause += Callback_OnGamePause;
		GameController.OnGameResume += Callback_OnGameResume; 
	}

	//---------------------------------------------------------------
	protected override void UnregisterCallbacks()
	{
		GameController.OnGamePause -= Callback_OnGamePause;
		GameController.OnGameResume -= Callback_OnGameResume; 
	}

	//---------------------------------------------------------------
	private void Callback_OnGamePause()
	{
		RequestVideoPause();
	}

	//---------------------------------------------------------------
	private void Callback_OnGameResume()
	{
		RequestVideoResume();
	}

	//============================================================================
	//---------------------------------------------------------------
	public void RequestStartVideo(int ID)
	{
		if (ID >= m_videos.Length)
		{
			Debug.LogError("Trying to access an video ID that doesn't exists");
			return;
		}

		if (m_currentVideoID == ID)
			return;

		m_currentVideoID = ID;
		m_currentVideo = m_videos[ID];
		m_videoRawImage.texture = m_currentVideo;

		m_audioSource.clip = m_currentVideo.audioClip;
		m_currentVideo.loop = false;
		m_currentVideo.Stop(); //Reset video
		m_fadeable.Show(true);

		if (!m_isPaused)
		{
			m_currentVideo.Play();
			m_audioSource.Play();
		}
		else
		{
			Debug.LogWarning("Starting video with id {0} but the video is paused.");
		}

		if (OnVideoStartPlaying != null)
		{
			OnVideoStartPlaying(m_currentVideoID);
		}
	}

	//---------------------------------------------------------------
	public void RequestVideoPause()
	{
		if (m_isPaused)
			return;

		m_currentVideo.Pause();
		m_audioSource.Pause();
		m_isPaused = true;

		if (OnVideoPause != null)
		{
			OnVideoPause();
		}
	}

	//---------------------------------------------------------------
	public void RequestVideoResume()
	{
		if (!m_isPaused)
			return;

		m_currentVideo.Play();
		m_audioSource.UnPause();
		m_isPaused = false;

		if (OnVideoResume != null)
		{
			OnVideoResume();
		}
	}

	//---------------------------------------------------------------
	public void RequestStopVideo()
	{
		if (m_currentVideo == null)
			return;

		m_currentVideo.Stop();
		m_currentVideo = null;
		m_currentVideoID = -1;

		m_audioSource.Stop();

		m_videoRawImage.texture = m_blackTexture;
		m_fadeable.Hide();

		if (OnVideoStopPlaying != null)
		{
			OnVideoStopPlaying(m_currentVideoID);
		}
	}
}
