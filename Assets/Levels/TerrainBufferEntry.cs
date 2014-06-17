using UnityEngine;
using System.Collections;
using System;

public class TerrainBufferEntry {

    public TerrainBufferEntry(int x, int y, int z, GameObject meshPrototype) {

        // set position
        this.x = x;
        this.y = y;
        this.z = z;

        // initialize patch
        var meshObject = GameObject.Instantiate(meshPrototype) as GameObject;
        this.patch = new TerrainPatch(meshObject);
    }

    public bool IsAt(int px, int py, int pz) {
        return (x == px) && (y == py) && (z == pz);
    }

    public void EvaluatePatch(int x, int y, int z, float size, int resolution,
        MarchingCubes evaluator, IDensity density, float isoLevel,
        Action<IEnumerator> startCoroutine, Action<Action> executeOnMainThread) {

        // new position
        //Debug.Log("Evaluating patch (" + x + "," + y + "," + z + ") replacing (" + this.x + "," + this.y + "," + this.z + ")");
        this.x = x;
        this.y = y;
        this.z = z;

        // re-evaluate patch
        var offset = new Vector3(x * size, y * size, z * size);
    var coroutine = patch.Evaluate(evaluator, density, isoLevel, offset, size, resolution, executeOnMainThread);
        startCoroutine(coroutine);
    }

    private int x, y, z;
    private TerrainPatch patch;
}
