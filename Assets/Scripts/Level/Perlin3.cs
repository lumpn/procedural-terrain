using UnityEngine;
using System.Collections;
using LibNoise;

/// <summary>
/// 3D Perlin noise.
/// </summary>
public class Perlin3 : IDensity {

    public Perlin3(int seed, float frequency, float amplitude) {

        // configure perlin module
        var module = new Perlin();
        module.Seed = seed;
        module.Frequency = frequency * freqScale;
        module.NoiseQuality = NoiseQuality.Standard;

        // initialize
        this.perlin = module;
        this.amplitude = amplitude;
    }

    /// <summary>
    /// Evaluates the Perlin noise at the given position.
    /// </summary>
    /// <returns>Perlin noise in the [-amplitude, amplitude] range.</returns>
    public float Evaluate(Vector3 position) {
        float sample = (float)perlin.GetValue(position.x, position.y, position.z);
        return sample * amplitude;
    }

    private IModule perlin;
    private float amplitude;

    /// <summary>
    /// Provides reasonable variation within [0,1] input range.
    /// </summary>
    private const float freqScale = 10.0f;
}
