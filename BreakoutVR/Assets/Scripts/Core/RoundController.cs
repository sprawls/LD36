using UnityEngine;
using System;
using System.Collections;
using JetBrains.Annotations;

public class RoundController : MonoBehaviour
{
    public enum RoundPauseReason
    {
        LifeLost,
    }

    public enum RoundEndReason
    {
        Success,
        GameOver,
    }

    //============================================================================================

    private enum RoundStep
    {
        NotStarted,
        Starting,
        Started,
        Paused,
        StartingCountdownToEnd,
        CountdownToEnd,
        Ending,
    }

    //============================================================================================

    [SerializeField]
    private RoundData m_roundData = new RoundData();

    //============================================================================================

    public static Action OnRoundPreStart;
    public static Action OnRoundStart;
    public static Action<RoundPauseReason> OnRoundPause;
    public static Action OnRoundResume;
    public static Action OnCountdownToEndStarted;
    public static Action<RoundEndReason> OnRoundPreEnd;
    public static Action<RoundEndReason> OnRoundEnd;

    //============================================================================================

    private Brick[] m_bricks = null;
    private RoundStep m_currentStep = RoundStep.NotStarted;
    private int m_completionNeeded = 0;
    private int m_brickTotal = 0;
    private int m_brickDestroyed = 0;
    private float m_countdownTimer = 0;

    //============================================================================================
    #region Init
    //-------------------------------------------------------------------------
    [UsedImplicitly]
    private void Awake()
    {
        Init();
    }

    //-------------------------------------------------------------------------
    private void Init()
    {
        m_currentStep = RoundStep.NotStarted;

        m_bricks = gameObject.GetComponentsInChildren<Brick>(true);
        for (int i = 0; i < m_bricks.Length; ++i)
        {
            m_bricks[i].OnDestroy += Callback_OnBrickDestroyed;
        }

        m_brickTotal = m_bricks.Length;
        m_brickDestroyed = 0;
        CalculateCompletionCount(m_brickTotal);
        gameObject.SetActive(false);
    }

    //-------------------------------------------------------------------------
    private void CalculateCompletionCount(int brickCount)
    {
        m_completionNeeded = Mathf.CeilToInt((float)brickCount * ((float)m_roundData.CompletionNeeded) / 100.0f);
    }
    #endregion

    //-------------------------------------------------------------------------
    public void RequestStart()
    {
        if (m_currentStep != RoundStep.NotStarted)
            return;

        m_currentStep = RoundStep.Starting;
    }

    //-------------------------------------------------------------------------
    public void RequestStop()
    {
        TriggerEndingEvents(RoundEndReason.GameOver);
        m_currentStep = RoundStep.NotStarted;
    }

    //-------------------------------------------------------------------------
    public void SetPause(bool pause, RoundPauseReason reason)
    {
        if (!pause && m_currentStep != RoundStep.Paused)
            return;

        if (pause && m_currentStep == RoundStep.Paused)
            return;

        if (pause)
        {
            if (reason == RoundPauseReason.LifeLost
                && (m_currentStep == RoundStep.CountdownToEnd || m_currentStep == RoundStep.StartingCountdownToEnd))
            {
                m_currentStep = RoundStep.Ending;
            }
            else
            {
                m_currentStep = RoundStep.Paused;
            }

            if (OnRoundPause != null)
            {
                OnRoundPause(reason);
            }
        }
        else
        {
            m_currentStep = RoundStep.Started;

            if (OnRoundResume != null)
            {
                OnRoundResume();
            }
        }
    }

    //-------------------------------------------------------------------------
    [UsedImplicitly]
    private void Update()
    {
        switch (m_currentStep)
        {
            case RoundStep.NotStarted:
            {
                break;
            }

            case RoundStep.Starting:
            {
                TriggerStartingEvents();
                m_currentStep = RoundStep.Started;
                break;
            }

            case RoundStep.Started:
            {
                if (m_brickDestroyed >= m_completionNeeded)
                {
                    m_currentStep = RoundStep.StartingCountdownToEnd;
                }
                break;
            }

            case RoundStep.Paused:
            {
                break;
            }

            case RoundStep.StartingCountdownToEnd:
            {
                m_countdownTimer = PlayLoopController.Instance.EndOfRoundCountdown;

                if (OnCountdownToEndStarted != null)
                {
                    OnCountdownToEndStarted();
                }
                break;
            }

            case RoundStep.CountdownToEnd:
            {
                m_countdownTimer -= Time.deltaTime;

                if (m_countdownTimer <= 0)
                {
                    m_currentStep = RoundStep.Ending;
                }
                break;
            }

            case RoundStep.Ending:
            {
                TriggerEndingEvents(RoundEndReason.Success);
                m_currentStep = RoundStep.NotStarted;
                break;
            }
        }
    }

    #region Events Trigger
    //-------------------------------------------------------------------------
    private void TriggerStartingEvents()
    {
        StartCoroutine(Coroutine_StartingEvents());
    }

    //-------------------------------------------------------------------------
    private IEnumerator Coroutine_StartingEvents()
    {
        if (OnRoundPreStart != null)
        {
            OnRoundPreStart();
        }

        yield return null;

        if (OnRoundStart != null)
        {
            OnRoundStart();
        }
    }

    //-------------------------------------------------------------------------
    private void TriggerEndingEvents(RoundEndReason reason)
    {
        StartCoroutine(Coroutine_EndingEvents(reason));
    }

    //-------------------------------------------------------------------------
    private IEnumerator Coroutine_EndingEvents(RoundEndReason reason)
    {
        if (OnRoundPreEnd != null)
        {
            OnRoundPreEnd(reason);
        }

        yield return null;

        if (OnRoundEnd != null)
        {
            OnRoundEnd(reason);
        }
    }
    #endregion

    #region Callbacks
    //-------------------------------------------------------------------------
    private void Callback_OnBrickDestroyed()
    {
        ++m_brickDestroyed;
    }

    #endregion
}

[Serializable]
public class RoundData
{
    [Tooltip("Completion needed in percent"), Range(0, 100)]
    public int CompletionNeeded = 50;
}