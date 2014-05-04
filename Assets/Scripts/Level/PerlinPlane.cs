using UnityEngine;
using System.Collections;

public class PerlinPlane : IDensity {

    public PerlinPlane(float frequency, float amplitude) {
        this.frequency = frequency;
        this.amplitude = amplitude;
    }

    public float Evaluate(Vector3 position) {
        float a = position.x * frequency;
        float b = position.z * frequency;
        //float perlin = PerlinNoise(Wrap(a), Wrap(b));
        float perlin = TiledPerlinNoise(a, b);
        float offset = (2 * perlin - 1) * amplitude;
        return Mathf.Clamp(offset - position.y, -1, 1);
    }

    private float TiledPerlinNoise(float x, float y) {
        float wx = Wrap(x);
        float wy = Wrap(y);
        float mx = Wrap(x + 0.5f);
        float my = Wrap(y + 0.5f);
        float a = PerlinNoise(wx, wy);
        float b = PerlinNoise(mx, wy);
        float c = PerlinNoise(wx, my);
        float d = PerlinNoise(mx, my);
        // 0 1 2 3 4 5 6 7 8 9
        // 5 6 7 8 9 0 1 2 3 4
        float p = Mathf.Lerp(a, b, FadeMid(wx));
        float q = Mathf.Lerp(c, d, FadeMid(wx));
        return Mathf.Lerp(p, q, FadeMid(wy));
    }

    /** Tileable Perlin noise with reasonable frequency in [0, 1] input range */
    private float PerlinNoise(float x, float y) {
        const float frequency = 10.0f;
        return Mathf.PerlinNoise(Wrap(x * frequency), Wrap(y * frequency));
    }

    /** Wraps around [0, 1] */
    private float Wrap(float v) {
        return (v - Mathf.Floor(v));
    }

    private float FadeMid(float v) {
        return Mathf.Abs(2 * v - 1);
    }

    private float frequency, amplitude;
}
