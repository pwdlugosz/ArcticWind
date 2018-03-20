using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticWind.Elements
{

    /// <summary>
    /// Represents a 'String' keyed collection that allows either keyed index lookups or direct index lookups
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Heap<T>
    {

        protected const Single V_RATE = 0.5f;

        protected Dictionary<string, int> _RefSet;
        protected List<T> _Heap;
        protected List<bool> _IsReadOnly;
        internal Guid _UID;

        public Heap()
        {
            _RefSet = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            _Heap = new List<T>();
            _IsReadOnly = new List<bool>();
            this.Identifier = "UNKNOWN";
            this._UID = Guid.NewGuid();
        }

        // Properties //
        /// <summary>
        /// Returns the possible compression rate if vacuumed
        /// </summary>
        public Single CompressionRate
        {
            get
            {
                Single num = (Single)this._RefSet.Count;
                Single den = (Single)this._Heap.Count;
                return den == 0f ? 0f : num / den;
            }
        }

        /// <summary>
        /// Gets the county of variables in the heap
        /// </summary>
        public int Count
        {
            get { return this._RefSet.Count; }
        }

        /// <summary>
        /// Gets the variable given it's name
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public T this[string Name]
        {

            get
            {
                return this._Heap[this._RefSet[Name]];
            }

            set
            {
                if (this.IsReadOnly(Name))
                    throw new Exception(string.Format("'{0}' is read only", Name));
                this._Heap[this._RefSet[Name]] = value;
            }

        }

        /// <summary>
        /// Gets the variable at a given point in memory
        /// </summary>
        /// <param name="Pointer"></param>
        /// <returns></returns>
        public T this[int Pointer]
        {

            get
            {
                return this._Heap[Pointer];
            }

            set
            {
                this._Heap[Pointer] = value;
            }

        }

        /// <summary>
        /// Gets or sets the heap's name
        /// </summary>
        public string Identifier
        {
            get;
            set;
        }

        // Methods //
        /// <summary>
        /// Checks if a variable is readonly
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool IsReadOnly(string Name)
        {
            return this._IsReadOnly[this._RefSet[Name]];
        }

        /// <summary>
        /// Tags a variable as readonly
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="IsReadOnly"></param>
        public void SetReadOnly(string Name, bool IsReadOnly)
        {
            this._IsReadOnly[this._RefSet[Name]] = IsReadOnly;
        }

        /// <summary>
        /// Gets the name given a pointer
        /// </summary>
        /// <param name="Pointer"></param>
        /// <returns></returns>
        public string Name(int Pointer)
        {
            return this._RefSet.Keys.ToArray()[Pointer];
        }

        /// <summary>
        /// Checks if a name exists
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool Exists(string Name)
        {
            return this._RefSet.ContainsKey(Name);
        }

        /// <summary>
        /// Gets the direct heap location of the variable; this may change after vacuuming
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public int GetPointer(string Name)
        {
            return this._RefSet[Name];
        }

        /// <summary>
        /// Adds a name/variable to memory
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        public void Allocate(string Name, T Value)
        {
            if (this.Exists(Name))
                throw new Exception(string.Format("Cannot allocate '{0}', an allocation with that name already exists", Name));
            this._RefSet.Add(Name, this._Heap.Count);
            this._Heap.Add(Value);
            this._IsReadOnly.Add(false);
        }

        /// <summary>
        /// Removes a name/value form memory
        /// </summary>
        /// <param name="Name"></param>
        public void Deallocate(string Name)
        {

            if (this.Exists(Name))
            {
                int ptr = this.GetPointer(Name);
                this._RefSet.Remove(Name);
            }

        }

        /// <summary>
        /// If a variable exists, it overrites it, otherwise it adds it
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        public void Reallocate(string Name, T Value)
        {

            if (this.Exists(Name))
            {
                this[Name] = Value;
                return;
            }

            this.Allocate(Name, Value);

        }

        //internal void Collide(string Name, T Value, int Pointer)
        //{
        //    if (this.Exists(Name))
        //        throw new Exception(string.Format("Cannot allocate '{0}', an allocation with that name already exists", Name));
        //    this._RefSet.Add(Name, Pointer);
        //    this._Heap.Add(Value);
        //}

        /// <summary>
        /// Compresses the heapset
        /// </summary>
        public void Vacum()
        {

            Dictionary<string, int> x = new Dictionary<string, int>(this._RefSet, StringComparer.OrdinalIgnoreCase);
            List<T> y = new List<T>();
            List<bool> z = new List<bool>();
            
            int NewPointer = 0;

            foreach (KeyValuePair<string, int> kv in x)
            {

                // Accumulate a Value to the new heap //
                y.Add(this._Heap[kv.Value]);
                z.Add(this._IsReadOnly[kv.Value]);

                // Reset the pointer //
                x[kv.Key] = NewPointer;

                // Increment the pointer //
                NewPointer++;

            }

            // Point the new heap //
            this._RefSet = x;
            this._Heap = y;
            this._IsReadOnly = z;

        }

        public Dictionary<string, T> Entries
        {
            get
            {
                Dictionary<string, T> values = new Dictionary<string, T>();
                foreach (KeyValuePair<string, int> kv in this._RefSet)
                    values.Add(kv.Key, this[kv.Value]);
                return values;
            }
        }

        public List<T> Values
        {
            get { return this._Heap; }
        }

        public void Import(Heap<T> Value)
        {

            foreach (KeyValuePair<string, T> x in Value.Entries)
            {

                if (!this.Exists(x.Key))
                    this.Allocate(x.Key, x.Value);

            }

        }

    }

}
