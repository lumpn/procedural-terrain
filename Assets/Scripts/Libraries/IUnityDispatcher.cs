using System;

public interface IUnityDispatcher {
  
  void QueueWorkItem(Action action) ;
}
