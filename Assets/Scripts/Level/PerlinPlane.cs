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
        float perlin = PerlinNoise(Wrap(a), Wrap(b));
        float offset = (2 * perlin - 1) * amplitude;
        return Mathf.Clamp(offset - position.y, -1, 1);
    }

    private float TiledPerlinNoise(float x, float y) {
        float wx = Wrap(x);
        float wy = Wrap(y);
        float mx = Wrap(x + 0.5f);
        float a = PerlinNoise(wx, wy);
        float b = PerlinNoise(mx, wy);
        // 0 1 2 3 4 5 6 7 8 9
        // 5 6 7 8 9 0 1 2 3 4
        return Mathf.Lerp(a, b, FadeMid(wx));
    }

    /** Perlin noise with reasonable frequency in [0, 1] input range */
    private float PerlinNoise(float x, float y) {
        const float frequency = 10.0f;
        return Mathf.PerlinNoise(x * frequency, y * frequency);
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
