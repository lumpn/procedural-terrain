using UnityEngine;
using System.Collections;

public class Ring : IDensity {

    public Ring(Vector3 origin, float radius) {
        this.origin = origin;
        this.radius = radius;
    }

    public float Evaluate(Vector3 pos) {

        // calculate cylinder density (ignoring x)
        var yz = new Vector3(0, pos.y, pos.z);
        float distance = Vector3.Distance(origin, yz);
        float cylinder = 1 - Mathf.Abs(radius - distance) * ringShape;

        // calculate yz plane density
        float dx = (pos.x - origin.x);
        float plane = 1 - dx * dx;

        // calculate bottom skew
        float height = 0.7f - (pos.y - origin.y) / radius;
        float bottom = height * bottomWeight;

        // combine
        return (cylinder + plane + bottom);
    }

    private Vector3 origin;
    private float radius;

    private const float ringShape = 4.0f;
    private const float bottomWeight = 4.0f;
}
