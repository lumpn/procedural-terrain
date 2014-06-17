using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class UnityDispatcher : MonoBehaviour, IUnityDispatcher {

  void Start() {
    if (SynchronizationContext.Current == null) {
      Debug.Log("Setting synchronization context");
      var syncContext = new UnitySynchronizationContext(this);
      SynchronizationContext.SetSynchronizationContext(syncContext);
    }
  }
  
  // update is called once per frame on the main thread
  void Update() {

    // execute all pending actions
    while (true) {

      // get action if any
      Action action = null;
      lock (queueMutex) {
        if (!queuedItems.Any()) {
          break;
        }
        action = queuedItems.Dequeue();
      }
      
      // execute action
      try {
        action.Invoke();
      } catch (Exception e) {
        Debug.LogException(e);
      }
    }
  }

  public void QueueWorkItem(Action action) {
    lock (queueMutex) {
      queuedItems.Enqueue(action);
    }
  }

  private Queue<Action> queuedItems = new Queue<Action>();
  private object queueMutex = new object();
}
