using UnityEngine;
using System;
using JetBrains.Annotations;

public class ScoreController : Singleton<ScoreController>
{
    [LargeHeader("Score"), SerializeField]
    private int m_baseScore = 0;

    [SerializeField]
    private int m_baseMultiplier = 0;

    //==============================================================================================================

    /// <summary> int: new multiplier </summary>
    public static event Action<int> OnMultiplierChange;

    /// <summary> int: score gotten | transform: source </summary>
    public static event Action<int, Transform> OnScoreGotten;

    /// <summary> int: new score </summary>
    public static event Action<int> OnScoreChanged;

    //==============================================================================================================

    private int m_currentScore;
    private int m_currentMultiplier;

    public int CurrentScore { get { return m_currentScore; } }
    public int CurrentMultiplier { get { return m_currentMultiplier; } }

    //==============================================================================================================
    //----------------------------------------------------------------------------------------
    [UsedImplicitly]
    void Awake()
    {
        ResetData();
    }

    //----------------------------------------------------------------------------------------
    protected override void RegisterCallbacks()
    {
        base.RegisterCallbacks();

        GameController.OnLevelPreStarted += Callback_OnLevelPreStart;
    }

    //----------------------------------------------------------------------------------------
    protected override void UnregisterCallbacks()
    {
        base.UnregisterCallbacks();

        GameController.OnLevelPreStarted -= Callback_OnLevelPreStart;
    }

    //----------------------------------------------------------------------------------------
    private void Callback_OnLevelPreStart(LevelName levelName)
    {
        if (levelName != LevelName.Menu)
        {
            ResetData();
        }
    }

    //==============================================================================================================
    //----------------------------------------------------------------------------------------
    public void AddRawScore(int scoreAdded, Transform source)
    {
        if (scoreAdded == 0)
            return;

        if (source == null)
        {
            Debug.LogError("Cannot have null as source of score added");
            return;
        }

        if (OnScoreGotten != null)
        {
            OnScoreGotten(scoreAdded, source);
        }

        SetScore(m_currentScore + m_currentMultiplier * scoreAdded);
    }

    //----------------------------------------------------------------------------------------
    public void AddMultiplier(int modifier)
    {
        if (modifier == 0)
            return;

        if (m_currentMultiplier + modifier < m_baseMultiplier)
        {
            SetMultiplier(m_baseMultiplier);
        }
        else
        {
            SetMultiplier(m_currentMultiplier + modifier);
        }
    }

    //----------------------------------------------------------------------------------------
    public void ResetMultiplier()
    {
        SetMultiplier(m_baseMultiplier);
    }

    //==============================================================================================================
    //----------------------------------------------------------------------------------------
    private void SetMultiplier(int value)
    {
        if (value == m_currentMultiplier)
            return;

        m_currentMultiplier = value;
        if (OnMultiplierChange != null)
        {
            OnMultiplierChange(m_currentMultiplier);
        }
    }

    //----------------------------------------------------------------------------------------
    private void SetScore(int value)
    {
        if (value == m_currentScore)
            return;

        m_currentScore = value;
        if (OnScoreChanged != null)
        {
            OnScoreChanged(m_currentScore);
        }
    }

    //----------------------------------------------------------------------------------------
    private void ResetData()
    {
        ResetScore();
        ResetMultiplier();
    }

    //----------------------------------------------------------------------------------------
    private void ResetScore()
    {
       SetScore(m_baseScore);
    }
}
