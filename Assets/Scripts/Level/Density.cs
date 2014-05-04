using UnityEngine;
using System.Collections;
using System;

public class Density : IDensity {

    public Density(Func<Vector3, float> density) {
        this.density = density;
    }

    public float Evaluate(Vector3 position) {
        return density(position);
    }

    private Func<Vector3, float> density;
}
