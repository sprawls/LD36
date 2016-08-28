using UnityEngine;
using System.Collections;

public class TripleRacket : PowerUpAbstract
{

    [Header("TripleRacket Properties :")]
    [SerializeField]
    private float m_angleDiff = 45.0f;
    [SerializeField]
    private float m_animTime = 0.5f;
    [SerializeField]
    private float m_powerupTime = 10.0f;
   

    //Timers and 
    private bool m_isActivated = false;
    private float m_currentAnimTime = 0.0f;
    private float m_currentPowerUPTime = 0.0f;
    private Paddle m_padRef = null;
    private GameObject m_leftPaddle = null;
    private GameObject m_rightPaddle = null;



    protected override void Start()
    {
	    
	}

    protected override void Update()
    {
        if (m_isActivated)
        {
            m_currentAnimTime += Time.deltaTime;
            m_currentPowerUPTime += Time.deltaTime;
            if (m_currentAnimTime <= m_animTime)
            {
                float angle = Mathf.Lerp(0.0f, m_angleDiff, m_currentAnimTime / m_animTime);
                Quaternion rot = Quaternion.Euler(angle,0.0f,0.0f);
                m_leftPaddle.transform.localRotation = m_leftPaddle.transform.localRotation * rot;
            }
            if(m_currentPowerUPTime > m_powerupTime)
            {
                Destroy(m_leftPaddle);
                Destroy(m_rightPaddle);
                m_isActivated = false;
                m_currentAnimTime = 0.0f;
                m_currentPowerUPTime = 0.0f;
            }
        }
	}

    protected override void Awake()
    {

    }

    public override void Activate(HMDController.ControllerIndex cIndex)
    {
        if (!m_isActivated)
        {
            m_isActivated = true;
            m_padRef = PowerupController.Instance.GetHandController(cIndex).paddle;
            if (m_padRef)
            {
                m_leftPaddle = Instantiate(Resources.Load("Prefabs/PaddleObject"), m_padRef.transform) as GameObject;
                m_rightPaddle = Instantiate(Resources.Load("Prefabs/PaddleObject"), m_padRef.transform) as GameObject;
            }
        }
        else
        {
            m_currentPowerUPTime -= m_powerupTime;
        }
        
    }
}
