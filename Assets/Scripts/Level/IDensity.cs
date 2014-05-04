using UnityEngine;

public interface IDensity {

    /// <summary>
    /// Evaluates the given position to a density.
    /// Density ranges between [-inf, inf] where positive is
    /// solid ground and negative is empty space.
    /// The iso-surface where the density equals 0 is rendered.
    /// </summary>
    float Evaluate(Vector3 pos);
}
