using System.Collections.Concurrent;

namespace ABBDataManagerSystem.Tools
{
    public delegate void EventHandler(object sender, TestEventArgs e);

    public class EventManager
    {
        private ConcurrentDictionary<string, List<EventHandler>> _eventHandlers = new ConcurrentDictionary<string, List<EventHandler>>();
        private ConcurrentQueue<QueuedEvent> _eventQueue = new ConcurrentQueue<QueuedEvent>();
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Task? _eventDispatchTask;

        private static EventManager? _Instance = null;
        private static object _lock = new object();

        public static EventManager Instance 
        {
            get {
                lock (_lock)
                {
                    if (_Instance == null)
                    {
                        _Instance = new EventManager();
                    }
                    return _Instance;
                }
            }
        }

        public void Subscribe(string eventName, EventHandler handler)
        {
            if (!_eventHandlers.ContainsKey(eventName))
            {
                _eventHandlers[eventName] = new List<EventHandler>();
            }
            bool Exist = _eventHandlers[eventName].Exists((item) => { return item == handler; });
            if (!Exist)
                _eventHandlers[eventName].Add(handler);
        }

        public void Unsubscribe(string eventName, EventHandler handler)
        {
            if (_eventHandlers.TryGetValue(eventName, out List<EventHandler>? handlers))
            {
                handlers.RemoveAll((item) => { return item == handler; });

                // 如果没有订阅者了，可以考虑从字典中移除事件名  
                if (handlers.Count == 0)
                {
                    _eventHandlers.TryRemove(eventName, out _);
                }
            }
        }

        public void TriggerEvent(string eventName, object sender, TestEventArgs e)
        {
            // 直接分发事件（同步）或者将事件加入队列（异步）  
            EnqueueEvent(eventName, sender, e);
        }

        private void EnqueueEvent(string eventName, object sender, TestEventArgs e)
        {
            _eventQueue.Enqueue(new QueuedEvent { EventName = eventName, Sender = sender, EventArgs = e });

            // 如果分发线程还没有启动，则启动它  
            if (_eventDispatchTask == null || _eventDispatchTask.IsCompleted)
            {
                _eventDispatchTask = Task.Run(() => DispatchEvents(_cancellationTokenSource.Token));
            }
        }

        private void DispatchEvents(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_eventQueue.TryDequeue(out QueuedEvent? queuedEvent))
                {
                    if (_eventHandlers.TryGetValue(queuedEvent.EventName, out List<EventHandler>? handlers))
                    {
                        foreach (var handler in handlers)
                        {
                            handler(queuedEvent.Sender, queuedEvent.EventArgs);
                        }
                    }
                }
                else
                {
                    // 队列为空时，可以等待一段时间再检查或者直接休眠  
                    Thread.Sleep(50); // 例如，休眠100毫秒  
                }
            }
        }

        // 停止分发线程（通常在应用程序关闭时调用）  
        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _eventDispatchTask?.Wait(); // 等待分发线程完成  
            _cancellationTokenSource.Dispose();
        }

        private class QueuedEvent
        {
            public string EventName { get; set; }
            public object Sender { get; set; }
            public TestEventArgs EventArgs { get; set; }
        }
    }
}
