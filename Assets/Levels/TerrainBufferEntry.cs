using UnityEngine;
using System;
using System.Collections;

public class TerrainBufferEntry {

  public TerrainBufferEntry(int x, int y, int z, GameObject meshPrototype) {

    // set position
    this.x = x;
    this.y = y;
    this.z = z;

    // initialize patch
    GameObject meshObject = null;
    UnitySynchronizationContext.Main.Send(() =>
        meshObject = GameObject.Instantiate(meshPrototype) as GameObject);
    this.meshObject = meshObject;
    this.patch = new TerrainPatch(meshObject);
  }

  public bool IsAt(int px, int py, int pz) {
    lock (mutex) {
      return (x == px) && (y == py) && (z == pz);
    }
  }

  public void EvaluatePatch(int x, int y, int z, float size, int resolution,
        MarchingCubes evaluator, IDensity density, float isoLevel) {

    // new position
    //Debug.Log("Evaluating patch (" + x + "," + y + "," + z + ") replacing (" + this.x + "," + this.y + "," + this.z + ")");
    lock (mutex) {
      this.x = x;
      this.y = y;
      this.z = z;
    }

    UnitySynchronizationContext.Main.Post(() =>
        meshObject.name = "Patch (" + x + "," + y + "," + z + ")");

    // re-evaluate patch
    var offset = new Vector3(x * size, y * size, z * size);
    patch.Evaluate(evaluator, density, isoLevel, offset, size, resolution);
  }

  private int x, y, z;
  private readonly object mutex = new object();
  private readonly GameObject meshObject;
  private readonly TerrainPatch patch;
}
