using UnityEngine;

public class HUDScoreWindowSpawner : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_leftOffset;

    [SerializeField]
    private Vector3 m_rightOffset;

    [SerializeField]
    private GameObject m_scoreWindowElement = null;

    private void Start()
    {
        UIScoreWindow scoreWindowLeft = Instantiate(m_scoreWindowElement).GetComponent<UIScoreWindow>();
        scoreWindowLeft.m_controllerBinded = HMDController.ControllerIndex.Left;
        HMDController.Instance.AddObjectToController(scoreWindowLeft.m_controllerBinded, scoreWindowLeft.transform, m_leftOffset);

        UIScoreWindow scoreWindowRight = Instantiate(m_scoreWindowElement).GetComponent<UIScoreWindow>();
        scoreWindowRight.m_controllerBinded = HMDController.ControllerIndex.Right;
        HMDController.Instance.AddObjectToController(scoreWindowRight.m_controllerBinded, scoreWindowRight.transform, m_rightOffset);
    }
}
