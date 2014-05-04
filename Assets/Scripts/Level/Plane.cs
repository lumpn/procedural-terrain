using UnityEngine;
using System.Collections;

public class Plane : IDensity {

    public Plane(float y) {
        this.y = y;
    }

    public float Evaluate(Vector3 position) {
        return y - position.y;
    }

    private float y;
}
