using UnityEngine;
using System.Collections;

public class BeatDetectScale : BeatDetect {

    public float scaleRatioMin;
    public float scaleRatioMax;
    public float dropTime = 0.2f;

    private Vector3 beatScale;
    private Vector3 initialScale;

    protected override void Awake()
    {
        initialScale = transform.localScale;
        float scaleRatio = Random.Range(scaleRatioMin, scaleRatioMax);
        beatScale = new Vector3(initialScale.x * scaleRatio, initialScale.y * scaleRatio, initialScale.z * scaleRatio);
        base.Awake();
    }

    public override void onOnbeatDetected()
    {
        transform.localScale = initialScale + beatScale;
        StopCoroutine("beatScaleDrop");
        StartCoroutine("beatScaleDrop");
    }

    IEnumerator beatScaleDrop()
    {
        for (float i = 0; i < 1.0f; i += Time.deltaTime / dropTime)
        {
            transform.localScale -= beatScale * Time.deltaTime / dropTime;
            yield return null;
        }
    }

    public override void onSpectrum(float[] spectrum){ }

    
}
