using UnityEngine;
using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class UIFadeable : MonoBehaviour 
{
	private enum State
	{
		FadeIn,
		FadeInProcess,
		FadeOut,
		FadeOutProcess
	}

	//===========================================================================

	[Header("Parameter"), SerializeField]
	private float m_timeToFade = 0.5f;

	//===========================================================================

	private State m_currentState;
	private CanvasGroup m_canvasGroup;

	public float TimeToFade 
	{ 
		get { return m_timeToFade; } 
		set { m_timeToFade = value; } 
	}

	//===========================================================================
	//------------------------------------------------------------
	private void Awake()
	{
		m_canvasGroup = GetComponent<CanvasGroup>();
		m_canvasGroup.alpha = 0;
		m_currentState = State.FadeOut;
	}

	//------------------------------------------------------------
	public void Show(bool skipFade = false)
	{
		if (skipFade)
		{
			m_canvasGroup.alpha = 1f;
			m_currentState = State.FadeIn;
		}
		else
		{
			m_currentState = State.FadeInProcess;
		}
	}

	//------------------------------------------------------------
	public void Hide(bool skipFade = false)
	{
		if (skipFade)
		{
			m_canvasGroup.alpha = 0f;
			m_currentState = State.FadeOut;
		}
		else
		{
			m_currentState = State.FadeOutProcess;
		}
	}

	//------------------------------------------------------------
	private void Update()
	{
		switch (m_currentState)
		{
			case State.FadeIn:
			case State.FadeOut:
				return;

			case State.FadeInProcess:
				m_canvasGroup.alpha = Mathf.Min(1f, m_canvasGroup.alpha + (1f / m_timeToFade) * Time.deltaTime);
				if (m_canvasGroup.alpha == 1f)
				{
					m_currentState = State.FadeIn;
				}
				break;
				
			case State.FadeOutProcess:
				m_canvasGroup.alpha = Mathf.Max(0f, m_canvasGroup.alpha - (1f / m_timeToFade) * Time.deltaTime);
				if (m_canvasGroup.alpha == 0f)
				{
					m_currentState = State.FadeOut;
				}
				break;
		}
	}
}
