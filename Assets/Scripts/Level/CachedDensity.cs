using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CachedDensity : IIndexDensity {

    private struct CacheKey : IEquatable<CacheKey> {
        public CacheKey(int x, int y, int z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public bool Equals(CacheKey other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return (x == other.x) && (y == other.y) && (z == other.z);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CacheKey)obj);
        }

        public override int GetHashCode() {
            unchecked {
                return x + y * 997 + z * 988027;
            }
        }

        private int x, y, z;
    }

    public CachedDensity(IDensity density) {
        this.density = density;
    }

    public float Evaluate(int x, int y, int z, Vector3 pos) {

        // query cache
        float value;
        var key = new CacheKey(x, y, z);
        if (cache.TryGetValue(key, out value)) {
            return value;
        }

        // evaluate and cache density
        value = density.Evaluate(pos);
        cache.Add(key, value);
        return value;
    }

    public void Add(int x, int y, int z, float amount) {
        float value = 0; // default density
        var key = new CacheKey(x, y, z);
        cache.TryGetValue(key, out value);
        cache[key] = value + amount;
    }

    public void Set(int x, int y, int z, float value) {
        var key = new CacheKey(x, y, z);
        cache[key] = value;
    }

    private IDensity density;
    private Dictionary<CacheKey, float> cache = new Dictionary<CacheKey, float>();
}
