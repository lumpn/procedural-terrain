using UnityEngine;

public interface IDensity {

    /// <summary>
    /// Evaluates the given position to a density.
    /// Density ranges between [-1, 1] where 1 is solid and -1 is vacuum.
    /// The iso-surface where the density equals 0 is rendered.
    /// </summary>
    float Evaluate(Vector3 pos);
}
