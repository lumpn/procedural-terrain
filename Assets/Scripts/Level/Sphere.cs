using UnityEngine;
using System.Collections;

public class Sphere : IDensity {

    public Sphere(Vector3 origin, float radius) {
        this.origin = origin;
        this.radius = radius;
    }

    public float Evaluate(Vector3 pos) {
        float distance = Vector3.Distance(origin, pos);
        return (radius - distance);
    }

    private Vector3 origin;
    private float radius;
}
