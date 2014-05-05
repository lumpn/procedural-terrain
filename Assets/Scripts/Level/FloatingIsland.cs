using UnityEngine;
using System.Collections;

public class FloatingIsland : IDensity {

    public FloatingIsland(Vector3 origin, float radius) {
        this.sphere = new Sphere(origin, radius);
        this.cone = new Cone(Vector3.zero, v => Mathf.Sqrt(2 * v));
        this.origin = origin;
    }

    public float Evaluate(Vector3 position) {

        // calculate flipped y cone point
        Vector3 relative = position - origin;
        relative.y = -relative.y;

        return Mathf.Min(sphere.Evaluate(position), cone.Evaluate(relative));
    }

    private Sphere sphere;
    private Cone cone;
    private Vector3 origin;
}
