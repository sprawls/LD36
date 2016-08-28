﻿using UnityEngine;
using System.Collections;

public class Ball : BreakoutPhysicObject {

    [Header("Ball Properties :")]
    public Vector3 startDirection;
    public float startSpeed;
    public float zGravity = 0.2f;

    private Vector3 _currentDirection;
    private float _currentSpeed;
    private Transform _ModelSpeedScaler;

    //Hit Conditions
    [Header("Ball Collision :")]
    public float paddleSpeedBounceFactor = 0.4f;
    public float paddleDirectionInfluenceFromPaddleSpeed = 0.5f;
    public float collisionRescaleTime = 1f;
    public AnimationCurve collisionBounceAnimation;

    private float hitCooldown = 0.05f;
    private bool canHit = true;
    private float _collisionScaleFactor = 1f;

    [Header("Ball Speed :")]
    public float minSpeed = 1f;
    public float maxSpeedWithoutLinearReduction = 3f;
    public float maxSpeedWithoutRatioReduction = 5f;

    public float linearReductionSpeed = 1f;
    public float clampSpeedRatio = 0.2f;

    public float speedForMaxStretch = 6f;
    public float maxXYStretchRatio = 0.6f;
    public float maxZStretchRatio = 1.3f;

    private float startXYScale;
    private float startZScale;
    private float endXYScale;
    private float endZScale;
    private float currentXYScale;
    private float currentZScale;

    override protected void Awake() {
        base.Awake();
    }

    void Start() {
        _currentDirection = startDirection.normalized;
        _currentSpeed = startSpeed;
        _ModelSpeedScaler = _transform.Find("ModelSpeedScaler");

        startXYScale = _ModelSpeedScaler.localScale.x;
        startZScale = _ModelSpeedScaler.localScale.z;
        endXYScale = startXYScale * maxXYStretchRatio;
        endZScale = startZScale * maxZStretchRatio;
    }

    void Update() {
        ApplyVelocityModification();
        CheckBallSpeed();
        OrientModel();
        StretchBall();
        RezizeModel();
        MoveBall();
    }

    private void ApplyVelocityModification() {
        _currentDirection = (_currentDirection.normalized + new Vector3(0,0,-zGravity*Time.deltaTime)).normalized;
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

    private void RezizeModel() {
        _ModelSpeedScaler.localScale = new Vector3(currentXYScale, currentXYScale, currentZScale) * _collisionScaleFactor;
    }

    private void StretchBall() {
        float maxSpeedStep = (_currentSpeed - maxSpeedWithoutLinearReduction) / (speedForMaxStretch - maxSpeedWithoutLinearReduction);

        currentXYScale = Mathf.Lerp(startXYScale, endXYScale, maxSpeedStep);
        currentZScale = Mathf.Lerp(startZScale, endZScale, maxSpeedStep);

    }

    private void MoveBall() {
        _transform.position += _currentDirection * _currentSpeed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision) {
        if (canHit == false) return;
        StartCoroutine(OnHitCooldown());
        StopCoroutine("OnHitModelScale");
        StartCoroutine("OnHitModelScale");

        float bounceFactor = GetBouncinessFactor(collision.collider);
        _currentDirection = GetDirection(collision);
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

    private float GetBouncinessFactor(Collider coll) {
        if (coll.tag == "Paddle") {
            Paddle paddleScript = coll.GetComponentInParent<Paddle>();
            //Debug.Log("Paddle Speed "  + paddleScript.GetCurrentVelocity());
            return (Mathf.Max(paddleScript.GetCurrentVelocityMagnitude() * paddleSpeedBounceFactor, 1f));
        } else {
            BreakoutPhysicObject bho = coll.GetComponent<BreakoutPhysicObject>();
            if (bho != null) {
                return bho.bouncinessFactor;
            } else return 1;
        }
    }

    private Vector3 GetDirection(Collision collision) {
        Vector3 reflectedDirection;
        Collider collider = collision.collider;
        if (collider.tag == "Paddle") {
            Paddle paddleScript = collider.GetComponentInParent<Paddle>();
            reflectedDirection = collision.contacts[0].normal;
            float DotProduct = Vector3.Dot(reflectedDirection, paddleScript.transform.up);
            if (!(DotProduct < 0.1f && DotProduct > -0.1f)) {
                reflectedDirection = paddleScript.transform.up * Mathf.Sign(DotProduct);
            }

            reflectedDirection += paddleScript.GetCurrentVelocity().normalized * paddleDirectionInfluenceFromPaddleSpeed;
        } else {
            reflectedDirection = Vector3.Reflect(_currentDirection, collision.contacts[0].normal);
        }
        return reflectedDirection;
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

    IEnumerator OnHitModelScale() {
        for (float i = 0; i < 1f; i += Time.deltaTime / collisionRescaleTime) {
            _collisionScaleFactor = collisionBounceAnimation.Evaluate(i);
            yield return null;
        }
        _collisionScaleFactor = 1f;
    }

}
