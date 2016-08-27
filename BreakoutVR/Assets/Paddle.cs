using UnityEngine;
using System.Collections;
using Valve.VR;

public class Paddle : MonoBehaviour {

    void Awake() {
        SteamVR_Utils.Event.Listen("render_model_loaded", OnModelLoaded);
    }

    private void OnModelLoaded(params object[] args) {
        gameObject.SetActive(true);
    }
}
