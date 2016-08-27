using UnityEngine;
using System.Collections;

public class Ball : BreakoutPhysicObject {

    [Header("Ball Properties :")]
    public Vector3 startDirection;
    public float startSpeed;

    private Vector3 _currentVelocity;

    override protected void Awake() {
        base.Awake();
    }

    void Start() {
        _currentVelocity = startDirection.normalized * startSpeed;
    }

    void Update() {
        MoveBall();
    }

    void OnCollisionEnter(Collision collision) {
        float bounceFactor = GetBouncinessFactor(collision.collider);
        _currentVelocity = Vector3.Reflect(_currentVelocity, collision.contacts[0].normal) * bounceFactor;
        MoveBall(); //To prevent ball being stuck because of collision

        //Send Hit Message
        collision.collider.gameObject.SendMessageUpwards("OnHit", 1, SendMessageOptions.DontRequireReceiver);
    }

    float GetBouncinessFactor(Collider coll) {
        BreakoutPhysicObject bho = coll.GetComponent<BreakoutPhysicObject>();
        if(bho != null){
            return bho.bouncinessFactor;
        }
        else return 1;
    }

    private void MoveBall() {
        _transform.position += _currentVelocity * Time.deltaTime;
    }

}
