using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 3D ring buffer
/// </summary>
public class TerrainBuffer {

  public TerrainBuffer(int bufferSize, GameObject meshPrototype) {
    this.bufferSize = bufferSize;
    this.entries = new TerrainBufferEntry[bufferSize, bufferSize, bufferSize];
    this.meshPrototype = meshPrototype;
  }

  public bool HasEntry(int x, int y, int z) {
    var entry = GetEntry(x, y, z);
    if (entry == null) {
      return false;
    }
    return entry.IsAt(x, y, z);
  }

  public void Generate(int x, int y, int z, MarchingCubes evaluator,
        IDensity density, float isoLevel, float size, int resolution) {

    // already generated?
    var entry = GetOrCreateEntry(x, y, z);
    if (entry.IsAt(x, y, z)) {
      return;
    }

    // evaluate density
    entry.EvaluatePatch(x, y, z, size, resolution, evaluator, density, isoLevel);
  }

  private TerrainBufferEntry GetEntry(int x, int y, int z) {

    // map into ring buffer
    int mx = MapIndex(x);
    int my = MapIndex(y);
    int mz = MapIndex(z);
    
    lock (mutex) {
      return entries[mx, my, mz];
    }
  }

  private TerrainBufferEntry GetOrCreateEntry(int x, int y, int z) {

    // map into ring buffer
    int mx = MapIndex(x);
    int my = MapIndex(y);
    int mz = MapIndex(z);

    lock (mutex) {
      // get entry?
      var entry = entries[mx, my, mz];
      if (entry != null) {
        return entry;
      }

      // create entry
      // HACK: displace coordinate to make it immediately evaluate its patch
      entry = new TerrainBufferEntry(Int32.MinValue, 0, 0, meshPrototype);
      entries[mx, my, mz] = entry;
      return entry;
    }
  }

  private int MapIndex(int idx) {
    int m = idx % bufferSize;
    if (m < 0) {
      return m + bufferSize;
    }
    return m;
  }

  private readonly int bufferSize;
  private readonly TerrainBufferEntry[, ,] entries;
  private readonly object mutex = new object();
  private readonly GameObject meshPrototype;
}
