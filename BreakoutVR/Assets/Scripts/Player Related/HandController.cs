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

    [SerializeField]
    private Transform m_anchor;

    [SerializeField]
    private GameObject m_paddlePrefab;

    //================================================================================================

    private bool m_paddlePresent = false;
    private Paddle m_paddle = null;
    public Paddle paddle { get { return m_paddle} set{ m_paddle = value;} }
    private PaddleState m_paddleState = PaddleState.Idle;

    //================================================================================================
    //-------------------------------------------------------------------------
    private void Awake()
    {
#if UNITY_EDITOR && true
        Callback_OnPlayStart();
#endif
    }

    //-------------------------------------------------------------------------
    protected override void RegisterCallbacks()
    {
        base.RegisterCallbacks();

        GameController.OnPlayStarted += Callback_OnPlayStart;
        GameController.OnPlayEnded += Callback_OnPlayEnded;
    }

    //-------------------------------------------------------------------------
    protected override void UnregisterCallbacks()
    {
        base.UnregisterCallbacks();

        GameController.OnPlayStarted -= Callback_OnPlayStart;
        GameController.OnPlayEnded -= Callback_OnPlayEnded;
    }

    //-------------------------------------------------------------------------
    private void Callback_OnPlayStart()
    {
        SpawnPaddle();
    }

    //-------------------------------------------------------------------------
    private void Callback_OnPlayEnded()
    {
        DespawnPaddle();
    }

    //-------------------------------------------------------------------------
    private void SpawnPaddle()
    {
        m_paddle = (Instantiate(m_paddlePrefab, m_anchor, false) as GameObject).GetComponentInChildren<Paddle>();
        m_paddle.transform.localPosition = Vector3.zero;
        m_paddle.transform.localRotation = Quaternion.identity;
        m_paddle.transform.localScale = Vector3.zero;
        m_paddle.transform.DOScale(Vector3.one, 1f);
        m_paddlePresent = true;
        m_paddleState = PaddleState.Held;
    }

    //-------------------------------------------------------------------------
    private void DespawnPaddle()
    {
        m_paddle.transform.DOScale(Vector3.zero, 1f);
        Destroy(m_paddle.gameObject, 1.1f);
        m_paddle = null;
        m_paddleState = PaddleState.Idle;
    }

    //-------------------------------------------------------------------------
    public void ThrowPaddle()
    {
        //TODO
    }

    //-------------------------------------------------------------------------
    private IEnumerator Coroutine_RespawnHandler()
    {
        //TODO
        yield return null;
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
