using UnityEngine;
using System.Collections;

public interface IIndexDensity {

    float Evaluate(int x, int y, int z, Vector3 pos);
}
