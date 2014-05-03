using UnityEngine;
using System.Collections;

public class CyclicRotation : MonoBehaviour {

    const float speed = 36.0f;

    void Update() {
        transform.Rotate(Vector3.up, speed * Time.deltaTime, Space.World);
    }
}
