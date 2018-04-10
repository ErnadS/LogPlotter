using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GraphLog.graph
{
    public class CircularBuffer<T> : IEnumerable<T>
    {
        private readonly int _capacity;
        private readonly Queue<T> _queue;

        public int Count { get { return _queue.Count; } }
        public int Capacity { get { return _capacity; } }

        public CircularBuffer(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException("Capacity must be a positive value.");

            _capacity = capacity;
            _queue = new Queue<T>(capacity);
        }

        public void Add(T value)
        {
            if (_queue.Count == _capacity)
            {
                _queue.Dequeue();
            }

            _queue.Enqueue(value);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_queue).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        List<T> ToList()
        {
            return _queue.ToList();
        }

        T[] ToArray()
        {
            return _queue.ToArray();
        }
    }
}
