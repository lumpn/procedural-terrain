using UnityEngine;
using System.Collections;

public class AddDensity : IDensity {

    public AddDensity(IDensity a, IDensity b) {
        this.a = a;
        this.b = b;
    }

    public float Evaluate(Vector3 position) {
        return a.Evaluate(position) + b.Evaluate(position);
    }

    private IDensity a, b;
}
