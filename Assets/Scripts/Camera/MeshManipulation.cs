using UnityEngine;
using System.Collections;

public class MeshManipulation : MonoBehaviour {

    void Update() {

        if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) {

            // cast ray into scene
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {

                UnityEngine.Debug.Log("hit at " + hit.point + " of " + hit.transform.position, this);

                // manipulate terrain
                var terrain = hit.transform.gameObject.GetComponent<TerrainMesh>();
                if (terrain != null) {
                    if (Input.GetMouseButton(0)) {
                        terrain.IncreaseDensity(hit.point);
                    } else {
                        terrain.DecreaseDensity(hit.point);
                    }
                }
            }
        }
    }
}
