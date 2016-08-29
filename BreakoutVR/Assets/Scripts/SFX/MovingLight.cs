using UnityEngine;
using System.Collections;

public class MovingLight : MonoBehaviour
{

    private float speed = 7.0f;
    private float duration = 1.35f;
    private float intensityMin = 0f;
    private float intensityMax = 1.5f;
    public Gradient gradient;
    public AnimationCurve intensityCurve;
    private Light light;

    void Awake()
    {
        light = gameObject.GetComponent<Light>();
        StartCoroutine(MoveLightAndDestroy());
    }

    IEnumerator MoveLightAndDestroy()
    {
        for (float i = 0; i < 1.0f; i += Time.deltaTime / duration)
        {
            transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
            light.color = gradient.Evaluate(i);
            light.intensity = intensityCurve.Evaluate(i);
            yield return null;
        }
        Destroy(gameObject);
    }





    /*void Update()
    {
        moveLight();
    }

    public void moveLight()
    {
        transform.Translate(new Vector3(0, 0, speed));

        float t = Mathf.Repeat(Time.time, duration) / duration;
        gameObject.GetComponent<Light>().color = gradient.Evaluate(t);

        StartCoroutine(destroyIfTimeUp());
    }

    IEnumerator destroyIfTimeUp()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }*/

}