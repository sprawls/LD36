using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

public class PlayLoopController : Singleton<PlayLoopController>
{
    private enum PlayState
    {
        Idle,
        LevelReset,
        Starting,
        LifeLost,
        Started,
        Completed
    }

    //=============================================================================================

    public static Action OnLifeLost;

    //=============================================================================================

    private PlayState m_playState = PlayState.Idle;
    private int m_ballCount = 0;
    private int m_totalBall = 0;
    private int m_ballSpawned = 0;
    private float m_delayBetweenBall = 0;
    private int m_lives = 0;
    private Spawner m_spawner = null;

    private float m_timeToNextBall = 0;

    //=============================================================================================
    //--------------------------------------------------------------------
    protected override void RegisterCallbacks()
    {
        base.RegisterCallbacks();

        GameController.OnPlayStarted += Callback_OnPlayStarted;
        GameController.OnLevelStarted += Callback_OnLevelStarted;
    }

    //--------------------------------------------------------------------
    protected override void UnregisterCallbacks()
    {
        base.UnregisterCallbacks();

        GameController.OnPlayStarted -= Callback_OnPlayStarted;
        GameController.OnLevelStarted -= Callback_OnLevelStarted;
    }

    //--------------------------------------------------------------------
    private void Callback_OnLevelStarted(LevelName levelName)
    {
        m_spawner = null;
        GameObject[] spawner = GameObject.FindGameObjectsWithTag("Spawner");
        foreach (var s in spawner)
        {
            Spawner spawn = s.GetComponent<Spawner>();
            if (spawn != null && spawn.Type == Spawner.SpawnType.Ball)
            {
                m_spawner = spawn;
                break;
            }
        }

        if (m_spawner == null)
        {
            Debug.LogError("Found no spawner of type ball in the level");
        }

        m_playState = PlayState.LevelReset;
    }

    //--------------------------------------------------------------------
    private void Callback_OnPlayStarted()
    {
        m_playState = PlayState.Starting;
    }

    //--------------------------------------------------------------------
    private void Callback_OnBallDestroyed()
    {
        --m_ballCount;
        if (m_ballCount <= 0)
        {
            m_playState = PlayState.LifeLost;            
        }
    }

    //=============================================================================================
    //--------------------------------------------------------------------
    [UsedImplicitly]
    private void Update()
    {
        switch (m_playState)
        {
            case PlayState.Idle:
                return;

            case PlayState.LevelReset:
                ResetData();
                m_playState = PlayState.Idle;
                break;

            case PlayState.Starting:
                m_playState = PlayState.Started;
                break;

            case PlayState.Started:
                m_timeToNextBall -= Time.deltaTime;

                if (m_timeToNextBall <= 0 && m_ballSpawned < m_totalBall)
                {
                    SpawnBall();
                }
                break;

            case PlayState.LifeLost:
                --m_lives;

                if (m_lives < 0)
                {
                    m_playState = PlayState.Completed;
                    GameController.Instance.RequestPlayLevelEnd();
                }
                else
                {
                    if (OnLifeLost != null)
                    {
                        OnLifeLost();
                    }    
                }

                break;

            case PlayState.Completed:

                break;
        }
    }

    //--------------------------------------------------------------------
    private void ResetData()
    {
        GameController.PlayLevelData data = GameController.Instance.CurrentPlayLevelData;

        if (data == null)
            return;
        
        m_totalBall = data.ballCount;
        m_delayBetweenBall = data.delayBetweenSpawn;
        m_lives = GameController.LIVES_COUNT;
        m_timeToNextBall = m_delayBetweenBall;
        ResetBalls();
    }

    //--------------------------------------------------------------------
    private void ResetBalls()
    {
        m_ballSpawned = 0;
        m_ballCount = m_totalBall;
    }

    //--------------------------------------------------------------------
    private void SpawnBall()
    {
        if (m_spawner == null)
            return;

        Ball ball = m_spawner.Spawn().GetComponent<Ball>();
        if (ball == null)
        {
            Debug.LogError("No component ball on the spawned ball");
        }
        else
        {
            ball.OnDestroy += Callback_OnBallDestroyed;
        }

        m_timeToNextBall = m_delayBetweenBall;
        ++m_ballSpawned;
    }
}
