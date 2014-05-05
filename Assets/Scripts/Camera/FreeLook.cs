using UnityEngine;
using System.Collections;

public class FreeLook : MonoBehaviour {

    public float sensitivityX = 8F;
    public float sensitivityY = 8F;
    public float velocity = 1.0f;

    void Start() {
        pitch = transform.eulerAngles.x;
        heading = transform.eulerAngles.y;
    }

    void Update() {

        // strafe
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
            transform.position -= velocity * transform.right;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
            transform.position += velocity * transform.right;
        }

        // forward/backward
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
            transform.position += velocity * transform.forward;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
            transform.position -= velocity * transform.forward;
        }

        // up/down
        if (Input.GetKey(KeyCode.PageUp) || Input.GetKey(KeyCode.E)) {
            transform.position += velocity * Vector3.up;
        }
        if (Input.GetKey(KeyCode.PageDown) || Input.GetKey(KeyCode.Q)) {
            transform.position -= velocity * Vector3.up;
        }

        // look around
        float deltaX = Input.GetAxis("Mouse X") * sensitivityX;
        float deltaY = Input.GetAxis("Mouse Y") * sensitivityY;
        ChangePitch(-deltaY);
        ChangeHeading(deltaX);
    }

    void ChangePitch(float aVal) {
        pitch += aVal;
        WrapAngle(ref pitch);
        transform.localEulerAngles = new Vector3(pitch, heading, 0);
    }

    void ChangeHeading(float aVal) {
        heading += aVal;
        WrapAngle(ref heading);
        transform.localEulerAngles = new Vector3(pitch, heading, 0);
    }

    public static void WrapAngle(ref float angle) {
        if (angle < -360f)
            angle += 360f;
        if (angle > 360f)
            angle -= 360f;
    }

    private float pitch;
    private float heading;
}
