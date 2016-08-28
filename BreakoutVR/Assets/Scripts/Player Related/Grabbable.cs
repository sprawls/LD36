using UnityEngine;
using System.Collections;

public class Grabbable : MonoBehaviour
{
    public Rigidbody Body { get { return m_rigidbody; } }

    private Rigidbody m_rigidbody;
    public bool IsGrabbed { get; private set; }
    public Transform OriginalParent { get; private set; }

    void Awake()
    {
        m_rigidbody = GetComponentInChildren<Rigidbody>();
        OriginalParent = transform.parent;
        ResetRigidbodyState();
    }

    protected virtual void ResetRigidbodyState()
    {
        OnRelease();
    }

    public virtual void OnHoverStart(HMDController.ControllerIndex controller)
    {
        
    }

    public virtual void OnHoverEnd(HMDController.ControllerIndex controller)
    {
        
    }

    public virtual void OnGrab(HMDController.ControllerIndex controller)
    {
        m_rigidbody.useGravity = true;
        IsGrabbed = true;
    }

    public virtual void OnRelease()
    {
        m_rigidbody.useGravity = true;
        IsGrabbed = false;
    }
}

