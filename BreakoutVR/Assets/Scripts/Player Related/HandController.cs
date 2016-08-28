using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

[RequireComponent(typeof(SphereCollider))]
public class HandController : MonoBehaviour
{
    [SerializeField]
    private HMDController.ControllerIndex m_controller;

    [SerializeField]
    private Transform m_anchor;

    //=============================================================================================

    private List<Grabbable> m_grabbableInRange = new List<Grabbable>();
    private Grabbable m_currentClosest = null;
    private Grabbable m_grabbed = null;

    //=============================================================================================
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
        if (m_currentClosest != null && m_grabbed == null)
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

    //------------------------------------------------------------------
    private void RequestRelease()
    {
        if (m_grabbed != null)
        {
            m_grabbed.transform.SetParent(m_grabbed.OriginalParent);
            m_grabbed.OnRelease();
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
}
