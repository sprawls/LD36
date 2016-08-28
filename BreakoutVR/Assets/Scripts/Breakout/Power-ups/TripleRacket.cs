using UnityEngine;
using System.Collections;

public class TripleRacket : PowerUpAbstract
{

    [Header("TripleRacket Properties :")]
    [SerializeField]
    private float m_leftPaddleAngle = 45.0f;
    [SerializeField]
    private float m_rigtPaddleAngle = 45.0f;
    [SerializeField]
    private float m_animTime = 0.5f;
    [SerializeField]
    private float m_powerupTime = 10.0f;
   

    //Timers and 
    public bool m_isActivated { get; private set; }
    private float m_currentAnimTime = 0.0f;
    private float m_currentPowerUPTime = 0.0f;



    protected override void Start()
    {
	    
	}

    protected override void Update()
    {
        if (m_isActivated)
        {
            m_currentAnimTime += Time.deltaTime;
            m_currentPowerUPTime += Time.deltaTime;
            if (m_currentAnimTime < m_animTime)
            {
                //Anim over
            }
            if(m_currentPowerUPTime > m_powerupTime)
            {
                //It's over
            }
        }
	}

    protected override void Awake()
    {

    }

    public override void Activate(HMDController.ControllerIndex cIndex)
    {
        m_isActivated = true;
    }
}
