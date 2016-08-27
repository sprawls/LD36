using UnityEngine;
using System.Collections;
using Valve.VR;

public class Paddle : BreakoutPhysicObject {

    public float currentVelocity;
    private Vector3 prevPosition;

    override protected void Awake() {
        Debug.Log("gf");
        SteamVR_Utils.Event.Listen("render_model_loaded", OnModelLoaded);
        base.Awake();
    }

    private void OnModelLoaded(params object[] args) {
        gameObject.SetActive(true);
        SetKinematic(false);
        gameObject.layer = 9; //Paddles
    }

    void Update() {
        _transform.localPosition = Vector3.zero;
        _transform.localRotation = Quaternion.identity;

        currentVelocity = (_transform.position - prevPosition).magnitude/Time.deltaTime;
        prevPosition = _transform.position;
    }

    public float GetCurrentVelocity() {
        return currentVelocity;
    }
}
