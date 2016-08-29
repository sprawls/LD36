using UnityEngine;
using System.Collections;

public class ImpactTorus : MonoBehaviour {

    public AnimationCurve animCurve;

    private float _time = 1f;
    private float _scale = 5f;
    private Material _material;
    private Transform _transform;

    void Awake() {
        _material = GetComponentInChildren<MeshRenderer>().material;
        _transform = transform;
    }

    public void ActivateTorus(float time, float scale) {
        _time = time;
        _scale = scale;
        StartCoroutine(FadeAnimation());
    }

    IEnumerator FadeAnimation() {
        for (float i = 0f; i < 1f; i += Time.deltaTime / _time) {
            _material.color = new Color(1f, 1f, 1f, 1f-i);
            _transform.localScale = animCurve.Evaluate(i) * Vector3.one * _scale;
            yield return null;
        }
        Destroy(gameObject);
    }
}
