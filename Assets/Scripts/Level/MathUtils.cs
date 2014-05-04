using UnityEngine;

public class MathUtils {

    /// <summary>
    /// Wraps around [0, 1]
    /// </summary>
    public static float Wrap(float v) {
        return (v - Mathf.Floor(v));
    }
}
