using UnityEngine;
using System.Collections;

public class Ball : BreakoutPhysicObject {

    [Header("Ball Properties :")]
    public Vector3 startDirection;
    public float startSpeed;

    private Vector3 _currentDirection;
    private float _currentSpeed;

    //Hit Conditions
    private float hitCooldown = 0.05f;
    private bool canHit = true;

    [Header("Ball Speed :")]
    public float minSpeed = 1f;
    public float maxSpeedWithoutLinearReduction = 3f;
    public float maxSpeedWithoutRatioReduction = 5f;

    public float linearReductionSpeed = 1f;
    public float clampSpeedRatio = 0.05f;

    public float speedForMaxStretch = 6f;
    public float maxXYStretchRatio = 0.6f;
    public float maxZStretchRatio = 1.3f;

    private float startXYScale;
    private float startZScale;
    private float endXYScale;
    private float endZScale;

    override protected void Awake() {
        base.Awake();
    }

    void Start() {
        _currentDirection = startDirection.normalized;
        _currentSpeed = startSpeed;

        startXYScale = _transform.localScale.x;
        startZScale = _transform.localScale.z;
        endXYScale = startXYScale * maxXYStretchRatio;
        endZScale = startZScale * maxZStretchRatio;
    }

    void Update() {
        CheckBallSpeed();
        OrientModel();
        StretchBall();
        MoveBall();
    }

    private void CheckBallSpeed() {
        if (_currentSpeed < minSpeed) _currentSpeed = minSpeed;
        else if (_currentSpeed > maxSpeedWithoutRatioReduction) {
            _currentSpeed = Mathf.Lerp(_currentSpeed, maxSpeedWithoutLinearReduction, clampSpeedRatio);
        }   
        else if (_currentSpeed > maxSpeedWithoutLinearReduction) {
            _currentSpeed -= linearReductionSpeed * Time.deltaTime;
        }
    }

    private void OrientModel() {
        _transform.rotation = Quaternion.LookRotation(_currentDirection);
    }

    private void StretchBall() {
        float maxSpeedStep = (_currentSpeed - maxSpeedWithoutLinearReduction) / (speedForMaxStretch - maxSpeedWithoutLinearReduction);

        float newXYScale = Mathf.Lerp(startXYScale, endXYScale, maxSpeedStep);
        float newZScale = Mathf.Lerp(startZScale, endZScale, maxSpeedStep);
        _transform.localScale = new Vector3(newXYScale, newXYScale, newZScale);
    }

    private void MoveBall() {
        _transform.position += _currentDirection * _currentSpeed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision) {
        StartCoroutine(OnHitCooldown());

        float bounceFactor = GetBouncinessFactor(collision.collider);
        _currentDirection = Vector3.Reflect(_currentDirection, collision.contacts[0].normal);
        _currentSpeed *= bounceFactor;
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
        if (coll.tag == "Paddle") {

        }

        BreakoutPhysicObject bho = coll.GetComponent<BreakoutPhysicObject>();
        if(bho != null){
            return bho.bouncinessFactor;
        }
        else return 1;

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
