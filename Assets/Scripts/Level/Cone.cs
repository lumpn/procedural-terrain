using System;
using UnityEngine;

public class Cone : IDensity {

    public Cone(Vector3 origin, Func<float, float> shape) {
        this.origin = origin;
        this.shape = shape;
    }

    public float Evaluate(Vector3 position) {

        // relative position
        var diff = position - origin;

        // distance in xz plane
        var xz = new Vector3(diff.x, 0, diff.z);
        var dist = xz.magnitude;

        // build cone
        return -(diff.y + shape(dist));
    }

    private Vector3 origin;
    private Func<float, float> shape;
}
