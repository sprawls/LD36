using UnityEngine;
using System.Collections;

public class Ball : BreakoutPhysicObject {

    [Header("Ball Properties :")]
    public Vector3 startDirection;
    public float startSpeed;

    private Vector3 _currentVelocity;

    //Hit Conditions
    private float hitCooldown = 0.05f;
    private bool canHit = true;


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
        StartCoroutine(OnHitCooldown());

        float bounceFactor = GetBouncinessFactor(collision.collider);
        _currentVelocity = Vector3.Reflect(_currentVelocity, collision.contacts[0].normal) * bounceFactor;
        MoveBall(); //To prevent ball being stuck because of collision

        //Send Hit Message
        collision.collider.gameObject.SendMessageUpwards("OnHit", 1, SendMessageOptions.DontRequireReceiver);

        //Check death
        if (collision.collider.tag == "KillZone") {
            OnKill();
        }
    }

    public void OnKill() {
        DestroyBall();
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

    private void DestroyBall() {
        //FX
        Destroy(gameObject);
    }

    IEnumerator OnHitCooldown() {
        canHit = false;
        yield return new WaitForSeconds(hitCooldown);
        canHit = true;
    }

}
