using UnityEngine;
using System.Collections;

public class PowerUPObject : MonoBehaviour {

    [SerializeField]
    private PowerupType m_type;
    [SerializeField]
    private float m_speed=10.0f;
    [SerializeField]
    private Vector3 m_direction= -Vector3.forward;

    void OnTriggerEnter(Collider col)
    {
        HandController hController = col.gameObject.GetComponentInParent<HandController>();
         if(hController)
         {
             PowerupController.Instance.ActivatePowerUP(m_type, hController.controllerID);
             Destroy(this.gameObject);
         }
    }

    void Update()
    {
        this.transform.position += Time.deltaTime * m_speed * m_direction;
    }
}
