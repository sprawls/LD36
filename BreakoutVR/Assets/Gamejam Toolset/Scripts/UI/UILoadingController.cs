using UnityEngine;
using System.Collections;
using System;

public class UILoadingController : UISingleton<UILoadingController> 
{
	//=============================================================================

	[Header("Parameter"), SerializeField]
	private float m_imagesFadeTime = 0.5f;

	[SerializeField]
	private bool shownOnLevelEnd = false;

	[SerializeField]
	private bool removedOnLevelStart = false;

	[Header("Component"), SerializeField]
	private Transform m_loadingImageGroup = null;	

	//=============================================================================

	private UIFadeable[] m_loadingImages;
	private bool m_isShown = false;
	private float m_timer = 0f;
	private int m_currentIndex = 0;

	//=============================================================================
	//----------------------------------------------------------
	private void Awake()
	{
		m_loadingImages = m_loadingImageGroup.GetComponentsInChildren<UIFadeable>();
		for (int i = 0; i < m_loadingImages.Length; ++i)
		{
			m_loadingImages[i].TimeToFade = m_imagesFadeTime;
		}
	}

	//----------------------------------------------------------
	protected override void RegisterCallbacks()
	{
		Access_LevelManager.OnLevelPreEnd += Callback_SceneEnd;
		Access_LevelManager.OnLevelStart += Callback_SceneStart;
	}

	//----------------------------------------------------------
	protected override void UnregisterCallbacks()
	{
		Access_LevelManager.OnLevelPreEnd -= Callback_SceneEnd;
		Access_LevelManager.OnLevelStart -= Callback_SceneStart;
	}

	//----------------------------------------------------------
	private void Callback_SceneEnd(LevelLoadCallbackInfo info)
	{
		RequestShow();
	}

	//----------------------------------------------------------
	private void Callback_SceneStart(LevelLoadCallbackInfo info)
	{
		RequestHide();
	}

	//----------------------------------------------------------
	public void RequestShow()
	{
		if (m_isShown || !shownOnLevelEnd)
			return;

		ShowLoading();
	}

	//----------------------------------------------------------
	public void RequestHide()
	{
		if (!m_isShown || !removedOnLevelStart)
			return;

		HideLoading();
	}

	//----------------------------------------------------------
	private void ShowLoading()
	{
		Show();
		m_isShown = true;
	}

	//----------------------------------------------------------
	private void HideLoading()
	{
		Hide();
		m_isShown = false;
	}

	//----------------------------------------------------------
	private void Update()
	{
		if (!m_isShown)
			return;

		m_timer += Time.deltaTime;
		if (m_timer >= m_imagesFadeTime)
		{
			m_loadingImages[m_currentIndex].Hide();

			++m_currentIndex;
			if (m_currentIndex >= m_loadingImages.Length)
				m_currentIndex = 0;

			m_loadingImages[m_currentIndex].Show();

			m_timer = 0f;
		}
	}
}
