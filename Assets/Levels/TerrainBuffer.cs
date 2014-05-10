using UnityEngine;
using System.Collections;

/// <summary>
/// 3D ring buffer
/// </summary>
public class TerrainBuffer {

    public TerrainBuffer(int size) {
        this.patches = new BufferedTerrainPatch[size, size, size];
    }

    public IEnumerable Evaluate(int x, int y, int z) {
        var patch = patches[MapIndex(x), MapIndex(y), MapIndex(z)];
        if (patch == null) {
            // this patch needs to be initialized!
            var meshObject = GameObject.Instantiate(meshPrototype);
            var p = new TerrainPatch(meshObject);
            patch = new BufferedTerrainPatch(x, y, z, p);
            patches[MapIndex(x), MapIndex(y), MapIndex(z)] = patch;
            yield return p.Evaluate(density, isoLevel, offset, size, resolution, evaluator);
        }

        if (!patch.IsAt(x, y, z)) {
            yield return patch.GetPatch().Evaluate(density, isoLevel, offset, size, resolution, evaluator);
        }

        yield return 0;
    }

    private int MapIndex(int idx) {
        int m = idx % size;
        if (m < 0) return m + size;
        return m;
    }

    private int size;
    private BufferedTerrainPatch[, ,] patches;
    private GameObject meshPrototype;
}
