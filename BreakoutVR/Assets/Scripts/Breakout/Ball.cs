﻿using UnityEngine;
using System.Collections;

public class Ball : BreakoutPhysicObject {

    [Header("Ball Properties :")]
    public Vector3 startDirection;
    public float startSpeed;

    public float angularSpeed;

    private Vector3 _currentDirection;
    private float _currentSpeed;
    private Transform _ModelSpeedScaler;

    //Hit Conditions
    [Header("Ball Collision :")]
    public float paddleSpeedBounceFactor = 0.4f;
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
    public float clampSpeedRatio = 0.05f;

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
        CheckBallSpeed();
        OrientModel();
        StretchBall();
        RezizeModel();
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
        StartCoroutine(OnHitCooldown());
        StopCoroutine("OnHitModelScale");
        StartCoroutine("OnHitModelScale");

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

    private float GetBouncinessFactor(Collider coll) {
        if (coll.tag == "Paddle") {
            Paddle paddleScript = coll.GetComponentInParent<Paddle>();
            //Debug.Log("Paddle Speed "  + paddleScript.GetCurrentVelocity());
            return (Mathf.Max(paddleScript.GetCurrentVelocity() * paddleSpeedBounceFactor, 1f));
        } else {
            BreakoutPhysicObject bho = coll.GetComponent<BreakoutPhysicObject>();
            if (bho != null) {
                return bho.bouncinessFactor;
            } else return 1;
        }
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
