using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreWindow : UISingleton<UIScoreWindow>
{
    [Header("Paramter"), SerializeField]
    private int m_totalNumberOfDigit = 8;

    [Header("Component"), SerializeField]
    private Text m_scoreField;

    [SerializeField]
    private Text m_multiplierField;

    //======================================================================================================

    public HMDController.ControllerIndex m_controllerBinded { get; set; }

    //======================================================================================================
    //---------------------------------------------------------------------------------------
    protected override void RegisterCallbacks()
    {
        base.RegisterCallbacks();

        ScoreController.OnScoreChanged += Callback_OnScoreChanged;
        ScoreController.OnMultiplierChange += Callback_OnMultiplierChanged;
        GameController.OnLevelPreEnded += Callback_OnLevelPreEnded;
    }

    //---------------------------------------------------------------------------------------
    protected override void UnregisterCallbacks()
    {
        base.UnregisterCallbacks();

        ScoreController.OnScoreChanged -= Callback_OnScoreChanged;
        ScoreController.OnMultiplierChange -= Callback_OnMultiplierChanged;
        GameController.OnLevelPreEnded -= Callback_OnLevelPreEnded;
    }

    //---------------------------------------------------------------------------------------
    private void Callback_OnLevelPreEnded(LevelName levelName)
    {
        HideWindow();
    }

    //---------------------------------------------------------------------------------------
    private void Callback_OnScoreChanged(int value)
    {
        m_scoreField.text = value.ToString().FillInsideCounter(m_totalNumberOfDigit);
    }

    //---------------------------------------------------------------------------------------
    private void Callback_OnMultiplierChanged(int value)
    {
        m_multiplierField.text = string.Format("x{0}", value);
    }

    //---------------------------------------------------------------------------------------
    [UsedImplicitly]
    private void Update()
    {
        if (HMDController.Instance.GetButtonDown(m_controllerBinded, HMDController.ControllerButton.ApplicationMenu))
        {
            ShowWindow();
        }
        else if (HMDController.Instance.GetButtonUp(m_controllerBinded, HMDController.ControllerButton.ApplicationMenu))
        {
            HideWindow();
        }
    }

    //---------------------------------------------------------------------------------------
    private void ShowWindow()
    {
        if (!GameController.Instance.IsInPlayLevel)
            return;

        m_fadeable.Show();
    }

    //---------------------------------------------------------------------------------------
    private void HideWindow()
    {
        m_fadeable.Hide();
    }
}
