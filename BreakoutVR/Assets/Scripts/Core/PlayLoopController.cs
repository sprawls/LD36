using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

public class PlayLoopController : Singleton<PlayLoopController>
{
    private enum State
    {
        Idle,
        Starting,
        SpawningBall,
        WaitingForBallHit,
        Started,
        Ending,
        Ended,
    }

    public enum PlayLoopState
    {
        Menu,
        Rounds,
        GameResult
    }

    //=======================================================================================================

    [LargeHeader("Global Parameter"), SerializeField, Range(1, 10)]
    private int m_totalLives = 5;

    [SerializeField, InspectorReadOnly]
    private int m_currentLives = 0;

    [LargeHeader("Rounds Data References"), SerializeField]
    private RoundController[] m_roundControllers;

    [LargeHeader("Round Parameter"), SerializeField]
    private float m_endOfRoundCountdown = 5f;

    //=======================================================================================================

    public static Action<PlayLoopState> OnPlayStateStart;
    public static Action<PlayLoopState> OnPlayStateEnd;

    //=======================================================================================================

    public float EndOfRoundCountdown { get { return m_endOfRoundCountdown; } }

    private int m_currentRoundIndex = -1;
    private State m_currentState = State.Idle;
    private PlayLoopState m_loopState = PlayLoopState.Menu;

    //=======================================================================================================
    //------------------------------------------------------------------------------
    public void RequestRoundsStart()
    {
        if (m_loopState != PlayLoopState.Menu)
            return;

        SwitchPlayLoopState(PlayLoopState.Rounds);
    }

    //------------------------------------------------------------------------------
    /// <summary> Request go back to menu. Can only be done from Game Result </summary>
    public void RequestMenu()
    {
        if (m_loopState != PlayLoopState.GameResult)
            return;

        GameController.Instance.RequestLoadMain();
    }

    //=======================================================================================================
    //------------------------------------------------------------------------------
    private void LoseLife()
    {
        if (m_currentState != State.Started)
            return;

        m_currentState = State.SpawningBall;
        --m_currentLives;

        if (m_currentLives <= 0)
        {
            m_roundControllers[m_currentRoundIndex].RequestStop();
        }
        else
        {
            m_roundControllers[m_currentRoundIndex].SetPause(true, RoundController.RoundPauseReason.LifeLost);
        }
    }

    //------------------------------------------------------------------------------
    private void RequestNextRound()
    {
        ++m_currentRoundIndex;

        if (m_currentRoundIndex < m_roundControllers.Length)
        {
            m_roundControllers[m_currentRoundIndex].RequestStart();
        }
        else
        {
            RequestGameResult();
        }
    }

    //------------------------------------------------------------------------------
    private void RequestGameResult()
    {
        m_currentState = State.Ending;
        SwitchPlayLoopState(PlayLoopState.GameResult);
    }

    //------------------------------------------------------------------------------
    private void SwitchPlayLoopState(PlayLoopState state)
    {
        StartCoroutine(Coroutine_SwitchPlayLoopState(state));
    }

    //------------------------------------------------------------------------------
    private IEnumerator Coroutine_SwitchPlayLoopState(PlayLoopState state)
    {
        if (OnPlayStateEnd != null)
        {
            OnPlayStateEnd(m_loopState);
        }

        yield return null;

        m_loopState = state;

        if (OnPlayStateStart != null)
        {
            OnPlayStateStart(m_loopState);
        }
    }

    //------------------------------------------------------------------------------
    [UsedImplicitly]
    private void Start()
    {
        if (OnPlayStateStart != null)
        {
            OnPlayStateStart(PlayLoopState.Menu);
        }
    }

    //------------------------------------------------------------------------------
    [UsedImplicitly]
    private void Update()
    {
        if (m_loopState != PlayLoopState.Rounds)
            return;

        switch (m_currentState)
        {
            /*--------------------------------------------------------------------------*/
            case State.Idle:
                break;

            /*--------------------------------------------------------------------------*/
            case State.Starting:
                m_currentLives = m_totalLives;
                m_currentRoundIndex = 0;
                m_currentState = State.SpawningBall;
                break;

            /*--------------------------------------------------------------------------*/
            case State.SpawningBall:
                BallsController.Instance.SpawnBall();

                m_currentState = State.WaitingForBallHit;
                break;

            /*--------------------------------------------------------------------------*/
            case State.WaitingForBallHit:
                break;

            /*--------------------------------------------------------------------------*/
            case State.Started:
                break;

            /*--------------------------------------------------------------------------*/
            case State.Ending:
                m_currentState = State.Ended;
                break;

            /*--------------------------------------------------------------------------*/
            case State.Ended:
                m_currentState = State.Idle;
                break;
        }
    }

    #region Callbacks
    //------------------------------------------------------------------------------
    protected override void RegisterCallbacks()
    {
        base.RegisterCallbacks();

        RoundController.OnRoundEnd += Callback_OnRoundEnd;
        Ball.OnGlobalBallHit += Callback_OnGlobalBallHit;
        BallsController.OnNoBallsLeft += Callback_OnNoBallsLeft;
    }

    //------------------------------------------------------------------------------
    private void Callback_OnGlobalBallHit(Ball ballRef)
    {
        if (m_currentState == State.WaitingForBallHit)
        {
            m_roundControllers[m_currentRoundIndex].SetPause(false, RoundController.RoundPauseReason.LifeLost);
            m_currentState = State.Started;
        }
    }

    //------------------------------------------------------------------------------
    private void Callback_OnNoBallsLeft()
    {
        LoseLife();
    }

    //------------------------------------------------------------------------------
    private void Callback_OnRoundEnd(RoundController.RoundEndReason reason)
    {
        switch (reason)
        {
            case RoundController.RoundEndReason.Success:
                RequestNextRound();
                break;

            case RoundController.RoundEndReason.Failure:
                RequestGameResult();
                break;
        }
    }

    //------------------------------------------------------------------------------
    protected override void UnregisterCallbacks()
    {
        base.UnregisterCallbacks();

        RoundController.OnRoundEnd -= Callback_OnRoundEnd;
        Ball.OnGlobalBallHit -= Callback_OnGlobalBallHit;
        BallsController.OnNoBallsLeft -= Callback_OnNoBallsLeft;
    }
    #endregion
}
