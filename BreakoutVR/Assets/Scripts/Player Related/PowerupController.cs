using UnityEngine;
using System.Collections;

public class PowerupController : Singleton<PowerupController>
{
    [LargeHeader("Hand Controllers"), SerializeField]
    private HandController m_rightHandController;

    [SerializeField]
    private HandController m_leftHandController;

    [LargeHeader("Debug"), SerializeField, InspectorReadOnly]
    private PowerupType m_rightHandPowerup;

    [SerializeField, InspectorReadOnly]
    private PowerupType m_leftHandPowerup;
}
