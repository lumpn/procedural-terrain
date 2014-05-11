using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateTerrain : MonoBehaviour {


    private static IDensity BuildDensity() {

        // base layer
        var plane = new Plane(0);

        // perlin layers
        const float baseFreq = 0.025f;
        var perlin0 = new Perlin2(baseFreq * 3.99f, 0.25f);
        var perlin1 = new Perlin2(baseFreq * 2.03f, 0.50f);
        var perlin2 = new Perlin2(baseFreq * 1.01f, 1.00f);
        var perlin3 = new Perlin2(baseFreq * 0.49f, 2.00f);

        // floating islands
        var island1 = new FloatingIsland(new Vector3(10, 4, 10), 6);
        var island2 = new FloatingIsland(new Vector3(8, 2, -1), 4);
        var island3 = new FloatingIsland(new Vector3(0, 3, 11), 3);

        // rings
        var ring1 = new Ring(new Vector3(5, -4, 5), 8);
        var ring2 = new Ring(new Vector3(0, -2, 2), 4);

        // stitch together
        return new Density(p => {

            // start with plane
            float density = plane.Evaluate(p);

            // add perlin noise
            density += perlin0.Evaluate(p);
            density += perlin1.Evaluate(p);
            density += perlin2.Evaluate(p);
            density += perlin3.Evaluate(p);

            // combine with islands and add a bit of noise
            // add structural variation to big island
            float islands = -1000.0f;
            islands = Mathf.Max(islands, island1.Evaluate(p) + perlin1.Evaluate(p));
            islands = Mathf.Max(islands, island2.Evaluate(p));
            islands = Mathf.Max(islands, island3.Evaluate(p));
            islands += perlin0.Evaluate(p);
            density = Mathf.Max(density, islands);

            // combine with rings and a bit of noise
            float rings = -1000.0f;
            rings = Mathf.Max(rings, ring1.Evaluate(p));
            rings = Mathf.Max(rings, ring2.Evaluate(Quaternion.AngleAxis(30, Vector3.up) * p));
            rings += perlin0.Evaluate(p);
            density = Mathf.Max(density, rings);

            return density;
        });
    }

    void Start() {

        Debug.Log("Starting terrain generator...");

        // create density function
        density = BuildDensity();

        // create terrain buffer
        var meshPrototype = GameObject.Find("Terrain Patch");
        buffer = new TerrainBuffer(bufferSize, meshPrototype, StartGeneration);
    }

    void Update() {

        // calculate camera patch
        var camPos = Camera.main.transform.position;
        var patchIdx = camPos / patchSize;
        int px = Mathf.CeilToInt(patchIdx.x);
        int py = Mathf.CeilToInt(patchIdx.y);
        int pz = Mathf.CeilToInt(patchIdx.z);
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

    private const float drawDistance = 50.0f;
    private const float patchSize = 10.0f;

    private const int numPatches = (int)(drawDistance / patchSize);
    private const int bufferSize = numPatches * 2;
}
