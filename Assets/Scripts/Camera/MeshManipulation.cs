using UnityEngine;
using System.Collections;

public class MeshManipulation : MonoBehaviour {

    void Update() {
        if (Input.GetMouseButton(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {

                UnityEngine.Debug.Log("hit at " + hit.point + " of " + hit.transform.position, this);

                var terrain = hit.transform.gameObject.GetComponent<TerrainMesh>();
                if (terrain != null) {
                    terrain.Manipulate();
                }
            }
        }
    }
}
