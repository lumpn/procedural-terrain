using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Provides a tileable 2D function by multisampling a non-tileable 2D function.
/// </summary>
public class Tiling {

    public Tiling(Func<float, float, float> nonTileable) {
        this.nonTileable = nonTileable;
    }

    /// <summary>
    /// Evaluates the tileable 2D function.
    /// </summary>
    public float Evaluate(float x, float y) {

        // 4x sample
        float wx = MathUtils.Wrap(x);
        float wy = MathUtils.Wrap(y);
        float mx = MathUtils.Wrap(x + 0.5f);
        float my = MathUtils.Wrap(y + 0.5f);
        float a = nonTileable(wx, wy);
        float b = nonTileable(mx, wy);
        float c = nonTileable(wx, my);
        float d = nonTileable(mx, my);

        // interpolate samples
        float p = Mathf.Lerp(a, b, FadeMid(wx));
        float q = Mathf.Lerp(c, d, FadeMid(wx));
        return Mathf.Lerp(p, q, FadeMid(wy));
    }

    /// <summary>
    /// Linearly maps [0, .., 0.5, .., 1] to [1, .., 0, .., 1]
    /// </summary>
    private float FadeMid(float v) {
        return Mathf.Abs(2 * v - 1);
    }

    /// <summary>
    /// Non-tileable 2D function
    /// </summary>
    private Func<float, float, float> nonTileable;
}
