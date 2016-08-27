using UnityEngine;
using System.Collections;
using Valve.VR;

public class Paddle : BreakoutPhysicObject {

    override protected void Awake() {
        SteamVR_Utils.Event.Listen("render_model_loaded", OnModelLoaded);
        base.Awake();
    }

    private void OnModelLoaded(params object[] args) {
        gameObject.SetActive(true);
        SetKinematic(false);
    }

    void Update() {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
