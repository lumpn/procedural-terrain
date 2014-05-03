using UnityEngine;
using System.Collections;

public interface IDensity {

    /**
     * Evaluates the given position to a density.
     * Density ranges between [-1, 1] where 1 is solid and -1 is vacuum.
     * The iso-surface where the density equals 0 is rendered.
     */
    float Evaluate(Vector3 pos);
}
