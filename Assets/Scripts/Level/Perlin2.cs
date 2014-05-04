using UnityEngine;

/// <summary>
/// Tileable 2D Perlin noise.
/// </summary>
public class Perlin2 : IDensity {

    /// <summary>
    /// Create a tileable 2D Perlin noise with given frequency and amplitude.
    /// </summary>
    public Perlin2(float frequency, float amplitude) {
        this.frequency = frequency;
        this.amplitude = amplitude;
    }

    /// <summary>
    /// Evaluates the tileable Perlin noise at the given position.
    /// Ignores the position's y component.
    /// </summary>
    /// <returns>Perlin noise in the [-amplitude, amplitude] range.</returns>
    public float Evaluate(Vector3 position) {

        // sample Perlin noise
        float a = position.x * frequency;
        float b = position.z * frequency;
        float perlin = PerlinNoise(a, b);

        // map to [-amplitude, amplitude]
        return (2 * perlin - 1) * amplitude;
    }

    /// <summary>
    /// Tileable 2D Perlin noise with reasonable variation
    /// within [0,1] x [0,1] input range.
    /// </summary>
    /// <returns>Perlin noise in the [0,1] range.</returns>
    private float PerlinNoise(float x, float y) {
        const float frequency = 10.0f;
        float a = MathUtils.Wrap(x) * frequency;
        float b = MathUtils.Wrap(y) * frequency;
        return Mathf.PerlinNoise(a, b);
    }

    private float frequency, amplitude;
}
