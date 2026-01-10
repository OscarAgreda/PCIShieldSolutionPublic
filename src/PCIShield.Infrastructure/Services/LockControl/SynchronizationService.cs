using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace PCIShield.Infrastructure.Services.LockControl
{
    public interface ISynchronizationService
    {
        void Produce(int item);
        int Consume();
        void PerformThreadSafeOperation(Action action);
    }
    public class SynchronizationService : ISynchronizationService
    {
        private readonly Queue<int> _queue = new Queue<int>();
        private readonly object _lockObject = new object();
        private readonly object _lockObj = new object();
        private const int Limit = 10;
        private const int _capacity = 5;
        public void Produce(int item)
        {
            lock (_lockObject)
            {
                while (_queue.Count >= Limit)
                {
                    Monitor.Wait(_lockObject);
                }
                _queue.Enqueue(item);
                Monitor.PulseAll(_lockObject);
            }
        }
        public int Consume()
        {
            lock (_lockObject)
            {
                while (_queue.Count == 0)
                {
                    Monitor.Wait(_lockObject);
                }
                int item = _queue.Dequeue();
                Monitor.PulseAll(_lockObject);
                return item;
            }
        }
        public void PerformThreadSafeOperation(Action action)
        {
            bool lockTaken = false;
            try
            {
                Monitor.Enter(_lockObject, ref lockTaken);
                action();
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(_lockObject);
                }
            }
        }
    }
}
