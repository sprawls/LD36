using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

public class PlayLoopController : Singleton<PlayLoopController>
{
    private enum State
    {
        Menu,
        Starting,
        SpawningBall,
        WaitingForBallHit,
        Started,
        Ending,
        Ended,
        GameResult
    }

    //=======================================================================================================

    [LargeHeader("Rounds Data References"), SerializeField]
    private RoundController[] m_roundControllers;

    [LargeHeader("Round Parameter"), SerializeField]
    private float m_endOfRoundCountdown = 5f;

    //=======================================================================================================

    public float EndOfRoundCountdown { get { return m_endOfRoundCountdown; } }

    private int m_currentRoundIndex = -1;
    private State m_currentState = State.Menu;

    //=======================================================================================================
    //------------------------------------------------------------------------------
    public void RequestStartRounds()
    {
        
    }

    //------------------------------------------------------------------------------
    [UsedImplicitly]
    private void Update()
    {
        switch (m_currentState)
        {
            
        }
    }

    #region Callbacks
    //------------------------------------------------------------------------------
    protected override void RegisterCallbacks()
    {
        base.RegisterCallbacks();

        RoundController.OnRoundEnd += Callback_OnRoundEnd;
    }

    //------------------------------------------------------------------------------
    private void Callback_OnRoundEnd(RoundController.RoundEndReason reason)
    {
        
    }

    //------------------------------------------------------------------------------
    protected override void UnregisterCallbacks()
    {
        base.UnregisterCallbacks();

        RoundController.OnRoundEnd -= Callback_OnRoundEnd;
    }
    #endregion
}
