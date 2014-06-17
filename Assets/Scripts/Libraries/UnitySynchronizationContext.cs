using System;
using System.Threading;

public class UnitySynchronizationContext : SynchronizationContext {

  public static UnitySynchronizationContext Main {
    get;
    private set;
  }
  
  public UnitySynchronizationContext(IUnityDispatcher dispatcher) {
    this.dispatcher = dispatcher;
    Main = this;
  }

  /// <summary>
  /// Creates a copy.
  /// </summary>
  public override SynchronizationContext CreateCopy() {
    return new UnitySynchronizationContext(dispatcher);
  }

  /// <summary>
  /// Asynchronously dispatches a message.
  /// </summary>
  public override void Post(SendOrPostCallback d, object state) {
    Post(() => d.Invoke(state));
  }

  /// <summary>
  /// Synchronously dispatches a message.
  /// </summary>
  public override void Send(SendOrPostCallback d, object state) {
    Send(() => d.Invoke(state));
  }

  /// <summary>
  /// Asynchronously executes an action in the synchronization context.
  /// </summary>
  public void Post(Action action) {
    dispatcher.QueueWorkItem(action);
  }
  
  /// <summary>
  /// Synchronously executes an action in the synchronization context.
  /// </summary>
  public void Send(Action action) {
    var handle = new EventWaitHandle(false, EventResetMode.AutoReset);
    dispatcher.QueueWorkItem(() => {
      action.Invoke();
      handle.Set();
    });
    handle.WaitOne();
  }

  private IUnityDispatcher dispatcher;
}
