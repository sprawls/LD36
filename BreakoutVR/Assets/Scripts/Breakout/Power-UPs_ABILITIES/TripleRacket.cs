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
    [SerializeField]
    private float m_floatBlinkTimeSlow = 0.75f;
    [SerializeField]
    private float m_floatBlinkTimeFast = 0.25f;
    [SerializeField]
    private float m_floatBlinkTimeVeryFast = 0.1f;


    [SerializeField]
    private Vector3 m_offsetLeftPaddle;
    [SerializeField]
    private Vector3 m_offsetRightPaddle;
    [SerializeField]
    private GameObject m_padObjectPrefab = null;


    //Timers and 
    private bool m_isActivated = false;
    private float m_currentAnimTime = 0.0f;
    private float m_currentPowerUPTime = 0.0f;
    private GameObject m_padRef = null;
    private GameObject m_leftPaddle = null;
    private GameObject m_rightPaddle = null;
    private bool flash = false;




    protected override void Start()
    {
	    
	}

    protected override void Update()
    {
        if (m_isActivated)
        {
            m_currentPowerUPTime += Time.deltaTime;
            if (m_currentAnimTime < m_animTime)
            {
                m_currentAnimTime += Time.deltaTime;
                if (m_currentAnimTime > m_animTime) m_currentAnimTime = m_animTime;
                float angle = Mathf.Lerp(0.0f, m_angleDiff, m_currentAnimTime / m_animTime);
                
                //Translations
                m_leftPaddle.transform.localPosition = Vector3.Lerp(Vector3.zero, m_offsetLeftPaddle, m_currentAnimTime / m_animTime);
                m_rightPaddle.transform.localPosition = Vector3.Lerp(Vector3.zero, m_offsetRightPaddle, m_currentAnimTime / m_animTime);

                //Rotations
                m_leftPaddle.transform.eulerAngles = new Vector3(0.0f, -angle, 0.0f);
                m_rightPaddle.transform.eulerAngles = new Vector3(0.0f, angle, 0.0f);
            }
            if(m_currentPowerUPTime > m_powerupTime) //it's over
            {
                Destroy(m_leftPaddle);
                Destroy(m_rightPaddle);
                m_isActivated = false;
                m_currentAnimTime = 0.0f;
                m_currentPowerUPTime = 0.0f;
            }
            else if (m_currentPowerUPTime > m_powerupTime - m_powerupTime / 8.0f) //it's 3/4 over
            {
                Flash(m_floatBlinkTimeVeryFast);
            }
            else if (m_currentPowerUPTime > m_powerupTime - m_powerupTime / 4.0f) //it's 3/4 over
            {
                Flash(m_floatBlinkTimeFast);
            }
            else if (m_currentPowerUPTime > m_powerupTime / 2.0f) //it's 1/2 over
            {
                Flash(m_floatBlinkTimeSlow);
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
            m_padRef = this.gameObject.GetComponentInChildren<Paddle>(true).gameObject;
            if (m_padRef)
            {
                m_leftPaddle = Instantiate(m_padObjectPrefab, m_padRef.transform) as GameObject;
                m_rightPaddle = Instantiate(m_padObjectPrefab, m_padRef.transform) as GameObject;
            }
        }
        else
        {
            m_currentPowerUPTime -= m_powerupTime;
        }
        
    }

    private void Flash(float frequency)
    {
        float delta = Mathf.Repeat(m_currentPowerUPTime, frequency) / frequency;
        if (flash)
        {
            m_leftPaddle.GetComponentInChildren<Renderer>().material.color = Color.Lerp(Color.white, Color.red, delta);
            m_rightPaddle.GetComponentInChildren<Renderer>().material.color = Color.Lerp(Color.white, Color.red, delta);
            if (delta == 1.0)
            {
                flash = false;
            }
        }
        else
        {
            m_leftPaddle.GetComponentInChildren<Renderer>().material.color = Color.Lerp(Color.red, Color.white, delta);
            m_rightPaddle.GetComponentInChildren<Renderer>().material.color = Color.Lerp(Color.red, Color.white, delta);

            if (delta == 1.0)
            {
                flash = true;
            }
        }
    }
}
