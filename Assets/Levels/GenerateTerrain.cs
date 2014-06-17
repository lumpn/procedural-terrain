using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class GenerateTerrain : MonoBehaviour
{


	private static IDensity BuildDensity ()
	{

		// base layer
		var plane = new Plane (0);

		// perlin layers
		const float baseFreq = 0.05f;
		var perlin0 = new Perlin2 (baseFreq * 3.99f, 0.25f);
		var perlin1 = new Perlin2 (baseFreq * 2.03f, 0.50f);
		var perlin2 = new Perlin2 (baseFreq * 1.01f, 1.00f);
		var perlin3 = new Perlin2 (baseFreq * 0.49f, 2.00f);

		// floating islands
		var island1 = new FloatingIsland (new Vector3 (10, 4, 10), 6);
		var island2 = new FloatingIsland (new Vector3 (8, 2, -1), 4);
		var island3 = new FloatingIsland (new Vector3 (0, 3, 11), 3);

		// rings
		var ring1 = new Ring (new Vector3 (5, -4, 5), 8);
		var ring2 = new Ring (new Vector3 (0, -2, 2), 4);

		// stitch together
		return new Density (p => {

			// start with plane
			float density = plane.Evaluate (p);

			// add perlin noise
			density += perlin0.Evaluate (p);
			density += perlin1.Evaluate (p);
			density += perlin2.Evaluate (p);
			density += perlin3.Evaluate (p);

			// combine with islands and add a bit of noise
			// add structural variation to big island
			float islands = -1000.0f;
			islands = Mathf.Max (islands, island1.Evaluate (p) + perlin1.Evaluate (p));
			islands = Mathf.Max (islands, island2.Evaluate (p));
			islands = Mathf.Max (islands, island3.Evaluate (p));
			islands += perlin0.Evaluate (p);
			density = Mathf.Max (density, islands);

			// combine with rings and a bit of noise
			float rings = -1000.0f;
			rings = Mathf.Max (rings, ring1.Evaluate (p));
			rings = Mathf.Max (rings, ring2.Evaluate (Quaternion.AngleAxis (30, Vector3.up) * p));
			rings += perlin0.Evaluate (p);
			density = Mathf.Max (density, rings);

			// combine with bridge
			float bridge = -1000.0f;
			Vector3 q = p;
			q.y -= 5; // move up
			q.y *= 2; // squish
			q.z *= (20.0f - Mathf.Sqrt (q.x * q.x)) * 0.1f; // widen
			// weight
			q.y += (q.x * q.x) * 0.05f; // bend

			// add turbulence
			Vector3 t1 = new Vector3 (p.x + 20.0f, p.y + 20.0f, p.z + 20.0f);
			Vector3 t2 = new Vector3 (t1.z, t1.y, -t1.x);
			q.y += perlin2.Evaluate (t1);
			q.z += perlin3.Evaluate (t2);

			float radius = 3;
			float cylinder = (radius * radius) - (q.y * q.y + q.z * q.z);
			cylinder += perlin0.Evaluate (p);
			if (Mathf.Abs (p.x) < 15.0f) {
				bridge = Mathf.Max (bridge, cylinder);
			}
			density = Mathf.Max (density, bridge);

			return density;
		});
	}

	void Start ()
	{

		Debug.Log ("Starting terrain generator...");
		var context = SynchronizationContext.Current;
		Debug.Log ("Main thread has " + ((context == null) ? "no" : "a") + " synchronization context");

		// create density function
		density = BuildDensity ();

		// create terrain buffer
		var meshPrototype = GameObject.Find ("Terrain Patch");
		buffer = new TerrainBuffer (bufferSize, meshPrototype, StartWorker, QueueOnMainThread);
	}

	void Update ()
	{

		var camPos = Camera.main.transform.position;
		ThreadPool.QueueUserWorkItem (s => {
			UpdateTerrain (camPos);
		});

		EmulateDispatcher ();
	}

	private void UpdateTerrain (Vector3 camPos)
	{

		// calculate camera patch
		var patchIdx = camPos / patchSize;
		int px = Mathf.CeilToInt (patchIdx.x);
		int py = Mathf.CeilToInt (patchIdx.y);
		int pz = Mathf.CeilToInt (patchIdx.z);
		py = 0; // HACK
    
		// generate surrounding terrain
		for (int z = pz - numPatches; z < pz + numPatches; z++) {
			for (int y = py - numPatches; y < py + numPatches; y++) {
				for (int x = px - numPatches; x < px + numPatches; x++) {
					QueueOnMainThread (() => {
						buffer.Generate (x, y, z, evaluator, density, isoLevel, patchSize, resolution);
					});
				}
			}
		}
	}

	private void EmulateDispatcher ()
	{
		while (true) {
			// get action if any
			Action action = null;
			lock (queueMutex) {
				if (queuedActions.Any ()) {
					action = queuedActions.Dequeue ();
				} else {
					break;
				}
			}

			// execute action
			try {
				if (action != null) {
					action.Invoke ();
				}
			} catch (Exception e) {
				Debug.LogException (e);
			}
		}
	}

	private void QueueOnMainThread (Action action)
	{
		lock (queueMutex) {
			queuedActions.Enqueue (action);
		}
	}

	private void StartGeneration (IEnumerator routine)
	{
		StartCoroutine (routine);
	}

	private void StartWorker (IEnumerator routine)
	{
		ThreadPool.QueueUserWorkItem (s => {
			while (routine.MoveNext()) {
			}
		});
	}

	private object queueMutex = new object ();
	private Queue<Action> queuedActions = new Queue<Action> ();
	private MarchingCubes evaluator = new MarchingCubes ();
	private IDensity density;
	private TerrainBuffer buffer;
	private const float isoLevel = 0.0f;
	private const int resolution = 20;
	private const float drawDistance = 20.0f;
	private const float patchSize = 10.0f;
	private const int numPatches = (int)(drawDistance / patchSize);
	private const int bufferSize = numPatches * 2 + 1;
}
