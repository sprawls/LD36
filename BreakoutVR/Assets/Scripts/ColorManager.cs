using UnityEngine;
using System.Collections;

public class ColorManager : Singleton<ColorManager> {

    private Material _wallMaterial;
    private float _emission = 2f;

    public static Color GetCurrentColor() {
        float r = Mathf.Clamp(Mathf.Sin(Time.time / 4f), 0.3f, 0.8f);
        float g = Mathf.Clamp(Mathf.Sin(Time.time / 8f), 0.3f, 0.8f);
        float b = Mathf.Clamp(Mathf.Sin(Time.time / 2.5f), 0.3f, 0.8f);

        return new Color(r, g, b);
    }

    void Awake() {
        _wallMaterial = (Material)Resources.Load("Textures/Wall");

    }

    void Update() {
        Color emissiveColor = GetCurrentColor() * _emission;
        _wallMaterial.SetColor("_EmissionColor", emissiveColor);
    }

}
