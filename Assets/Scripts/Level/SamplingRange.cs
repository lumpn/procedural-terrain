using UnityEngine;
using System.Collections;

public struct SamplingRange {

  public SamplingRange(float min, float max, float stepSize) {
    this.min = min;
    this.max = max;
    this.stepSize = stepSize;
  }

  public SamplingRange(float min, float max)
  :this(min,max,1.0f) {
  }

  public float min, max, stepSize;
}
