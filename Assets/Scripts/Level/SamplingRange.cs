using UnityEngine;
using System.Collections;

public struct SamplingRange {

    public SamplingRange(float min, float max, float stepSize = 1.0f) {
        this.min = min;
        this.max = max;
        this.stepSize = stepSize;
    }

    public float min, max, stepSize;
}
