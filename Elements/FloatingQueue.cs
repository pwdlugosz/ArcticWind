using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticWind.Elements
{

    /// <summary>
    /// Represents a Queue where elements can move up or down
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FloatingQueue<T> : IEnumerable<T>, IEnumerable
    {

        public enum State
        {
            LeastRecentlyUsed,
            MostRecentlyUsed
        }

        private Dictionary<T, LinkedListNode<T>> _Index;
        LinkedList<T> _Trail;
        private int _Capacity;
        private State _State = State.LeastRecentlyUsed;

        public FloatingQueue(int Capacity, IEqualityComparer<T> Comparer, FloatingQueue<T>.State State)
        {
            this._Index = new Dictionary<T, LinkedListNode<T>>(Comparer);
            this._Trail = new LinkedList<T>();
            this._Capacity = Capacity;
            this._State = FloatingQueue<T>.State.LeastRecentlyUsed;
        }

        public FloatingQueue(int Capacity, IEqualityComparer<T> Comparer)
            : this(Capacity, Comparer, State.LeastRecentlyUsed)
        {
        }

        public FloatingQueue(int Capacity)
            : this(Capacity, EqualityComparer<T>.Default, FloatingQueue<T>.State.LeastRecentlyUsed)
        {
        }

        public FloatingQueue()
            : this(128, EqualityComparer<T>.Default, FloatingQueue<T>.State.LeastRecentlyUsed)
        {
        }

        public bool IsEmpty
        {
            get { return this._Index.Count == 0; }
        }

        public bool IsFull
        {
            get { return this._Index.Count >= this._Capacity; }
        }

        public int Count
        {
            get { return this._Index.Count; }
        }

        // Public //
        public T Peek()
        {
            if (this._State == State.LeastRecentlyUsed)
                return this._Trail.Last.Value;
            else
                return this._Trail.First.Value;
        }

        public T Dequeue()
        {

            if (this.IsEmpty)
                throw new IndexOutOfRangeException();

            T element;
            if (this._State == State.LeastRecentlyUsed)
            {
                element = this._Trail.Last.Value;
                this._Trail.RemoveLast();
            }
            else
            {
                element = this._Trail.First.Value;
                this._Trail.RemoveFirst();
            }
            this._Index.Remove(element);

            return element;

        }

        public void Enqueue(T Value)
        {

            LinkedListNode<T> node = new LinkedListNode<T>(Value);
            this._Index.Add(Value, node);
            this._Trail.AddFirst(node);

        }

        public void Tag(T Value)
        {

            LinkedListNode<T> node = this._Index[Value];
            this._Trail.Remove(node);
            this._Index.Remove(Value);
            this.Enqueue(Value);

        }

        public void EnqueueOrTag(T Value)
        {

            if (this._Index.ContainsKey(Value))
            {
                this.Tag(Value);
            }
            else
            {
                this.Enqueue(Value);
            }

        }

        public void Remove(T Value)
        {

            if (!this._Index.ContainsKey(Value))
                return;

            LinkedListNode<T> x = this._Index[Value];
            this._Trail.Remove(x);
            this._Index.Remove(Value);

        }

        public bool Contains(T Value)
        {
            return this._Index.ContainsKey(Value);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this._Trail.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._Trail.GetEnumerator();
        }

    }

}
