using UnityEngine;
using System.Collections;

public static class ColorManager {

    public static Color GetCurrentColor() {
        float r = Mathf.Sin(Time.time / 5f);
        float g = Mathf.Sin(Time.time / 10f);
        float b = Mathf.Sin(Time.time / 3f);

        return new Color(r, g, b);
    }

}
