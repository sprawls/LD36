using UnityEngine;
using System.Collections;

public class IntroScripted : MonoBehaviour
{
    public GameObject m_leftPaddleScripted;
    public GameObject m_rightPaddleScripted;
    public GameObject m_start;

    private bool m_leftPickedUp = false;
    private bool m_rightPickedUp = false;

    private void Awake()
    {
        m_leftPaddleScripted.SetActive(true);
        m_rightPaddleScripted.SetActive(true);
        m_start.SetActive(false);
    }

    private void Update()
    {
        if (HMDController.Instance.GetButtonDown(HMDController.ControllerIndex.Left,
            HMDController.ControllerButton.Trigger))
        {
            m_leftPaddleScripted.SetActive(false);
            m_leftPickedUp = true;
        }

        if (HMDController.Instance.GetButtonDown(HMDController.ControllerIndex.Right,
            HMDController.ControllerButton.Trigger))
        {
            m_rightPaddleScripted.SetActive(false);
            m_rightPickedUp = true;
        }

        if (m_leftPickedUp && m_rightPickedUp)
        {
            m_start.SetActive(true);
        }
    }

    public void GotoFirstLevel()
    {
        GameController.Instance.RequestPlayLevelLoad(0);
    }
}
