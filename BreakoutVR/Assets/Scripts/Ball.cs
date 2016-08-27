using UnityEngine;
using System.Collections;

public class Ball : BreakoutPhysicObject {

    [Header("Ball Properties :")]
    public Vector3 startVelocity;

    private Vector3 _currentVelocity;

    override protected void Awake() {
        base.Awake();
    }

    void Start() {
        _currentVelocity = startVelocity;
    }

    void FixedUpdate() {
        _transform.position += _currentVelocity * Time.fixedDeltaTime;
    }

    void OnCollisionEnter(Collision collision) {
        float bounceFactor = GetBouncinessFactor(collision.collider);
        _currentVelocity = Vector3.Reflect(_currentVelocity, collision.contacts[0].normal) * bounceFactor;

        //Send Hit Message
        Debug.Log(collision.collider.gameObject.name);
        collision.collider.gameObject.SendMessageUpwards("OnHit", 1, SendMessageOptions.DontRequireReceiver);
    }

    float GetBouncinessFactor(Collider coll) {
        BreakoutPhysicObject bho = coll.GetComponent<BreakoutPhysicObject>();
        if(bho != null){
            return bho.bouncinessFactor;
        }
        else return 1;
    }

}
