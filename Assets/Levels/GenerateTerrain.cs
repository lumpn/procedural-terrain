using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateTerrain : MonoBehaviour {

    void Start() {

        Debug.Log("Starting terrain generator...");

        // create density function
        density = new Plane(0);

        // create terrain buffer
        var meshPrototype = GameObject.Find("Terrain Patch");
        buffer = new TerrainBuffer(bufferSize, meshPrototype, StartGeneration);
    }

    void Update() {

        // calculate camera patch
        var camPos = Camera.main.transform.position;
        var patchIdx = camPos / patchSize;
        int px = Mathf.FloorToInt(patchIdx.x);
        int py = Mathf.FloorToInt(patchIdx.y);
        int pz = Mathf.FloorToInt(patchIdx.z);
        py = 0; // HACK

        // generate surrounding terrain
        for (int z = pz - numPatches; z < pz + numPatches; z++) {
            for (int y = py - numPatches; y < py + numPatches; y++) {
                for (int x = px - numPatches; x < px + numPatches; x++) {
                    buffer.Generate(x, y, z, evaluator, density, isoLevel, patchSize, resolution);
                }
            }
        }
    }

    private void StartGeneration(IEnumerator routine) {
        StartCoroutine(routine);
    }

    private MarchingCubes evaluator = new MarchingCubes();
    private IDensity density;
    private TerrainBuffer buffer;

    private const float isoLevel = 0.0f;
    private const int resolution = 10;

    private const float drawDistance = 15.0f;
    private const float patchSize = 10.0f;

    private const int numPatches = (int)(drawDistance / patchSize);
    private const int numBorderPatches = 2;
    private const int bufferSize = (numPatches * 2) + numBorderPatches;
}
