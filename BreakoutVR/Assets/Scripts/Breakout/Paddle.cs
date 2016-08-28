﻿using UnityEngine;
using System.Collections;
using Valve.VR;

public class Paddle : BreakoutPhysicObject {

    public Vector3 currentVelocity;
    private Vector3 prevPosition;
    private HMDController.ControllerIndex _handController;
    private bool isGrabbed = false;

    override protected void Awake() {
        SteamVR_Utils.Event.Listen("render_model_loaded", OnModelLoaded);
        base.Awake();
    }

    private void OnModelLoaded(params object[] args) {
        gameObject.SetActive(true);
        SetKinematic(false);
        gameObject.layer = 9; //Paddles
    }

    void Update() {
        //_transform.localPosition = Vector3.zero;
        //_transform.localRotation = Quaternion.identity;

        currentVelocity = (_transform.position - prevPosition)/Time.deltaTime;
        prevPosition = _transform.position;
    }

    void FixedUpdate() {
        _rigidBody.MovePosition(_transform.parent.position);
        _rigidBody.MoveRotation(_transform.parent.rotation);
    }

    public float GetCurrentVelocityMagnitude() {
        return currentVelocity.magnitude;
    }

    public Vector3 GetCurrentVelocity() {
        return currentVelocity;
    }

    public void SetHandController(HMDController.ControllerIndex handController) {
        _handController = handController;
        isGrabbed = true;
    }

    public void RemoveHandController() {
        isGrabbed = false;
    }

    public void Rumble(float speed) {
        if (isGrabbed) {
            ushort duration = (ushort)(((int)speed + (int)GetCurrentVelocityMagnitude()) * 2500);
            //Debug.Log("duration: " + duration);
            //Debug.Log("_handController" + _handController);
            HMDController.Instance.TriggerHapticPulse(_handController, duration);
        }
    }

}
