using UnityEngine;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine.Events;

public class EventBall : Ball
{
    [Header("Event Ball Parameter"), SerializeField] private float m_eventAfterTime = 0f;
    [SerializeField] private float m_hoverMultiplier = 0.05f;
    [SerializeField, Range(10, 50)] private float m_hoverDifference = 20f;
    [SerializeField] private AnimationCurve y_curve;
    [SerializeField] private AnimationCurve xz_curve;
    [SerializeField] private UnityEvent m_events;

    private float m_time = 0;
    private Vector3 m_initPos;
    private Quaternion m_initRot;
    private bool wasHit = false;

    [UsedImplicitly]
    private void LateUpdate()
    {
        if (wasHit)
            return;

        m_time += Time.deltaTime;

        transform.rotation = m_initRot;

        transform.position = m_initPos + m_hoverMultiplier * new Vector3
        (
            xz_curve.Evaluate(Mathf.Sin(m_time / 20f + m_hoverDifference)),
            y_curve.Evaluate(Mathf.Sin(m_time / 10f + m_hoverDifference)),
            xz_curve.Evaluate(Mathf.Sin(m_time / 10f + m_hoverDifference))
        ); 
    }

    private void ThrowEvent()
    {
        m_events.Invoke();
    }

    protected override void Awake()
    {
        m_initPos = transform.position;
        m_initRot = transform.rotation;
        base.Awake();
    }

    protected override void Internal_OnHit()
    {
        wasHit = true;
        StartCoroutine(WaitForEvent());

        base.Internal_OnHit();
    }

    private IEnumerator WaitForEvent()
    {
        yield return new WaitForSeconds(m_eventAfterTime);

        ThrowEvent();
        OnKill();
    }
}
