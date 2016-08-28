using UnityEngine;
using System.Collections;

public class PowerUPObject : MonoBehaviour {

    [SerializeField]
    private PowerupType m_type;
    [SerializeField]
    private float m_speed=10.0f;
    [SerializeField]
    private Vector3 m_direction= -Vector3.forward;

    void OnCollisionEnter(Collision col)
    {
        HandController hController = col.gameObject.GetComponentInChildren<HandController>();
         if(hController)
         {
             PowerupController.Instance.ActivatePowerUP(m_type, hController.controllerID);
         }
    }

    void Update()
    {
        this.transform.position += Time.deltaTime * m_speed * m_direction;
    }
}
