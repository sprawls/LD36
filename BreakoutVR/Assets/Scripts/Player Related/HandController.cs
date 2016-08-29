using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;


public class HandController : ExtendedMonoBehaviour
{
    private enum PaddleState
    {
        Idle,
        Respawning,
        Held,
        Thrown,
    }

    //================================================================================================

    [SerializeField] 
    private HMDController.ControllerIndex m_controller;
    public HMDController.ControllerIndex controllerID { get { return m_controller; } private set { m_controller = value; } }

    [SerializeField]
    private Transform m_anchor;

    [SerializeField]
    private float m_paddleCooldown = 5.0f;

    [SerializeField]
    private GameObject m_paddlePrefab;

    AudioSource audioSource;

    //================================================================================================

    private bool m_paddlePresent = false;
    private Paddle m_paddle = null;
    private PaddleState m_paddleState = PaddleState.Idle;
    private bool m_paddleInCooldown = false;

    //================================================================================================
    //-------------------------------------------------------------------------
    protected override void RegisterCallbacks()
    {
        base.RegisterCallbacks();

        GameController.OnLevelStarted += Callback_OnLevelStart;
        GameController.OnLevelPreEnded += Callback_OnLevelPreEnded;
    }

    //-------------------------------------------------------------------------
    protected override void UnregisterCallbacks()
    {
        base.UnregisterCallbacks();

        GameController.OnLevelStarted -= Callback_OnLevelStart;
        GameController.OnLevelPreEnded -= Callback_OnLevelPreEnded;
    }

    //-------------------------------------------------------------------------
    private void Callback_OnLevelStart(LevelName level)
    {
        //SpawnPaddle();
    }

    //-------------------------------------------------------------------------
    private void Callback_OnLevelPreEnded(LevelName level)
    {
        //DespawnPaddle();
    }

    //-------------------------------------------------------------------------
    [UsedImplicitly]
    private void Update()
    {
        if (HMDController.Instance.GetButtonDown(m_controller, HMDController.ControllerButton.Trigger))
        {
            SpawnPaddle();
        }
    }

    //-------------------------------------------------------------------------
    private void SpawnPaddle()
    {
        if (m_paddle != null)
            return;

        if (m_paddleInCooldown)
            return;

        //paddle spawn sound
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(AudioManager.Instance.getPaddleGrowSound());

        m_paddle = (Instantiate(m_paddlePrefab, m_anchor, false) as GameObject).GetComponentInChildren<Paddle>();
        m_paddle.transform.localPosition = Vector3.zero;
        m_paddle.transform.localRotation = Quaternion.identity;
        m_paddle.transform.localScale = Vector3.zero;
        m_paddle.transform.DOScale(Vector3.one, 1f);
        m_paddlePresent = true;
        m_paddleState = PaddleState.Held;
        m_paddle.SetHandController(m_controller);
    }

    //-------------------------------------------------------------------------
    private void DespawnPaddle()
    {
        if (m_paddle == null)
            return;

        m_paddle.transform.DOScale(Vector3.zero, 1f);
        Destroy(m_paddle.gameObject, 1.1f);
        m_paddle = null;
        m_paddleState = PaddleState.Idle;
        m_paddle.RemoveHandController();
    }

    //-------------------------------------------------------------------------
    public void ThrowPaddle()
    {
        //TODO
        //m_paddle.RemoveHandController(); // to add somewhere here
    }

    //-------------------------------------------------------------------------
    private IEnumerator Coroutine_RespawnHandler()
    {
        yield return new WaitForSeconds(m_paddleCooldown);
        m_paddleInCooldown = false;
    }

    public void ActivatePowerUP(PowerupType pType)
    {
        switch(pType)
        {
            case PowerupType.TripleRacket:
                TripleRacket script = GetComponent<TripleRacket>();
                script.Activate(m_controller);
                break;
        }
    }

#if false
    //=============================================================================================

    private List<Grabbable> m_grabbableInRange = new List<Grabbable>();
    private Grabbable m_currentClosest = null;
    private Grabbable m_grabbed = null;

    //=============================================================================================

    //------------------------------------------------------------------
    private void RequestThrow()
    {
        
    }

    //------------------------------------------------------------------
    private void RequestRelease()
    {
        if (m_grabbed != null)
        {
            m_grabbed.transform.SetParent(m_grabbed.OriginalParent);
            m_grabbed.OnRelease();
            m_grabbableInRange.Remove(m_grabbed);
            m_grabbed = null;
        }
    }


    //------------------------------------------------------------------
    [UsedImplicitly]
    void OnTriggerEnter(Collider other)
    {
        Grabbable grabbable = other.GetComponent<Grabbable>();
        if (grabbable != null && !grabbable.IsGrabbed && grabbable != m_grabbed)
        {
            m_grabbableInRange.Add(grabbable);
        }
    }

    //------------------------------------------------------------------
    [UsedImplicitly]
    void OnTriggerExit(Collider other)
    {
        Grabbable grabbable = other.GetComponent<Grabbable>();
        if (grabbable != null)
        {
            m_grabbableInRange.Remove(grabbable);
        }
    }

    //------------------------------------------------------------------
    [UsedImplicitly]
    private void LateUpdate()
    {
        CheckClosestGrabbable();

        if (HMDController.Instance.GetButtonDown(m_controller, HMDController.ControllerButton.Trigger))
        {
            if (m_grabbed == null)
            {
                RequestGrab();
            }
            else
            {
                RequestRelease();
            }
        }
    }

    //------------------------------------------------------------------
    private void CheckClosestGrabbable()
    {
        if (m_grabbableInRange.Count == 0)
            return;

        if (m_grabbed != null)
            return;

        Grabbable closest = GameObjectUtils.GetClosest(m_grabbableInRange, this);
        if (closest != m_currentClosest)
        {
            if (m_currentClosest != null)
                m_currentClosest.OnHoverEnd(m_controller);
            m_currentClosest = closest;
            m_currentClosest.OnHoverStart(m_controller);
        }
    }

    //------------------------------------------------------------------
    private void RequestGrab()
    {
        if (m_currentClosest != null && m_grabbed == null && !m_currentClosest.IsGrabbed)
        {
            m_grabbed = m_currentClosest;
            m_grabbed.OnHoverEnd(m_controller);
            m_grabbableInRange.Remove(m_currentClosest);
            m_currentClosest = null;
            m_grabbed.transform.SetParent(m_anchor);
            m_grabbed.transform.localPosition = Vector3.zero;
            m_grabbed.transform.localRotation = Quaternion.identity;
            m_grabbed.OnGrab(m_controller);
        }
    }
#endif
}
