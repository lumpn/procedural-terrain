using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 3D ring buffer
/// </summary>
public class TerrainBuffer {

  public TerrainBuffer(int bufferSize, GameObject meshPrototype,
                       Action<IEnumerator> startCoroutine, Action<Action> executeOnMainThread) {
    this.bufferSize = bufferSize;
    this.entries = new TerrainBufferEntry[bufferSize, bufferSize, bufferSize];
    this.meshPrototype = meshPrototype;
    this.startCoroutine = startCoroutine;
    this.executeOnMainThread = executeOnMainThread;
  }

  public void Generate(int x, int y, int z, MarchingCubes evaluator,
        IDensity density, float isoLevel, float size, int resolution) {

    // already generated?
    var entry = GetOrCreateEntry(x, y, z);
    if (entry.IsAt(x, y, z)) {
      return;
    }

    // evaluate density
    entry.EvaluatePatch(x, y, z, size, resolution, evaluator, density, isoLevel,
                        startCoroutine, executeOnMainThread);
  }

  private TerrainBufferEntry GetOrCreateEntry(int x, int y, int z) {

    // map into ring buffer
    int mx = MapIndex(x);
    int my = MapIndex(y);
    int mz = MapIndex(z);

    // get entry?
    var entry = entries[mx, my, mz];
    if (entry != null) {
      return entry;
    }

    // create entry
    // HACK: displace coordinate to make it evaluate its patch
    entry = new TerrainBufferEntry(Int32.MinValue, 0, 0, meshPrototype);
    entries[mx, my, mz] = entry;
    return entry;
  }

  private int MapIndex(int idx) {
    int m = idx % bufferSize;
    if (m < 0) {
      return m + bufferSize;
    }
    return m;
  }

  private int bufferSize;
  private TerrainBufferEntry[, ,] entries;
  private GameObject meshPrototype;
  private Action<IEnumerator> startCoroutine;
  private Action<Action> executeOnMainThread;
}
