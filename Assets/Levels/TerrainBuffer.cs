using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 3D ring buffer
/// </summary>
public class TerrainBuffer {

    public TerrainBuffer(int bufferSize, GameObject meshPrototype, Action<IEnumerable> startCoroutine) {
        this.bufferSize = bufferSize;
        this.patches = new BufferedTerrainPatch[bufferSize, bufferSize, bufferSize];
        this.meshPrototype = meshPrototype;
    }

    public void Generate(int x, int y, int z, MarchingCubes evaluator, IDensity density, float isoLevel) {
        var patch = patches[MapIndex(x), MapIndex(y), MapIndex(z)];
        if (patch == null) {
            // this patch needs to be initialized!
            var meshObject = GameObject.Instantiate(meshPrototype);
            var p = new TerrainPatch(meshObject);
            patch = new BufferedTerrainPatch(x, y, z, p);
            patches[MapIndex(x), MapIndex(y), MapIndex(z)] = patch;
            yield return p.Evaluate(density, isoLevel, offset, bufferSize, resolution, evaluator);
        }

        if (!patch.IsAt(x, y, z)) {
            yield return patch.GetPatch().Evaluate(density, isoLevel, offset, bufferSize, resolution, evaluator);
        }

        yield return 0;
    }

    private int MapIndex(int idx) {
        int m = idx % bufferSize;
        if (m < 0) return m + bufferSize;
        return m;
    }

    private int bufferSize;
    private BufferedTerrainPatch[, ,] patches;

    private GameObject meshPrototype;
}
