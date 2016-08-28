using UnityEngine;
using System.Collections;

public class BreakoutPhysicObject : MonoBehaviour {

    [Header("Physics Object :")]
    public float bouncinessFactor = 1;

    protected Rigidbody _rigidBody;
    protected Transform _transform;

    public bool isDestroyed { get; protected set; }

	// Use this for initialization
    virtual protected void Awake() {
        _rigidBody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
	}

    public void SetGravity(bool activated) {
        _rigidBody.useGravity = activated;
    }

    public void SetKinematic(bool activated) {
        _rigidBody.isKinematic = activated;
    }

    protected void RemovePhysicsComponents() {
        Destroy(_rigidBody);
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders) Destroy(c);
    }

    protected void RemoveBeatDetectsChildren() {
        BeatDetect[] beatDetects = GetComponentsInChildren<BeatDetect>();
        foreach (BeatDetect b in beatDetects) Destroy(b);
    }
}
