using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Tables;

namespace ArcticWind.Elements
{

    /// <summary>
    /// Represents an array of cells; unlike a CellVector, each cell is allowed to have different types
    /// </summary>
    public class Record : ITextWritable
    {

        public const char DELIM = '\t';

        // Private //
        internal Cell[] _data;

        // Constructor //
        /// <summary>
        /// Creates a record with a pre-defined size
        /// </summary>
        /// <param name="PageSize">The size of the record's element cache</param>
        public Record(int Size)
        {
            this._data = new Cell[Size];
        }

        /// <summary>
        /// Creates a record based on an array of cells
        /// </summary>
        /// <param name="Elements">The data to load the record with</param>
        public Record(Cell[] Data)
        {
            this._data = Data;
        }

        /// <summary>
        /// Creates a new record by value
        /// </summary>
        /// <param name="Value"></param>
        public Record(Record Value)
        {
            this._data = new Cell[Value.Count];
            Array.Copy(Value._data, 0, this._data, 0, this._data.Length);
        }
        
        // Properties //
        /// <summary>
        /// The number of elements in the record
        /// </summary>
        public int Count
        {
            get
            {
                return this._data.Length;
            }
        }

        /// <summary>
        /// Gets or sets an element in a record
        /// </summary>
        /// <param name="Index">The offset of the record</param>
        /// <returns>A cell</returns>
        public Cell this[int Index]
        {
            get
            {
                return this._data[Index];
            }
            set
            {
                this._data[Index] = value;
            }
        }

        /// <summary>
        /// Returns the inner array supporting the record
        /// </summary>
        internal Cell[] BaseArray
        {
            get
            {
                return this._data;
            }
        }

        // Overrides //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Delim"></param>
        /// <param name="Escape"></param>
        /// <returns></returns>
        public string ToString(char Delim, char Escape)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.Count; i++)
            {
                sb.Append(Escape);
                sb.Append(this[i].ToString());
                sb.Append(Escape);
                if (i != this.Count - 1) 
                    sb.Append(Delim);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts the record to a string with a defined deliminator
        /// </summary>
        /// <param name="Delim">The delim to space each cell Value</param>
        /// <returns>A string representation of the record</returns>
        public string ToString(char Delim)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.Count; i++)
            {
                sb.Append(this[i].ToString());
                if (i != this.Count - 1) sb.Append(Delim);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns a string representation of a record using '\t' as the deliminator
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return this.ToString(DELIM);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="K"></param>
        /// <param name="Delim"></param>
        /// <param name="Escape"></param>
        /// <returns></returns>
        public string ToString(Key K, char Delim, char Escape)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < K.Count; i++)
            {
                sb.Append(Escape);
                sb.Append(this[K[i]].ToString());
                sb.Append(Escape);
                if (i != K.Count - 1) sb.Append(Delim);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Turns a record into a string
        /// </summary>
        /// <param name="K">A key to filter the record on</param>
        /// <param name="Delim">A delim to composite the string based on</param>
        /// <returns>A string</returns>
        public string ToString(Key K, char Delim)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < K.Count; i++)
            {
                sb.Append(this[K[i]].ToString());
                if (i != K.Count - 1) sb.Append(Delim);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Turns a record into a string
        /// </summary>
        /// <param name="K">A key to filter the record on</param>
        /// <returns>A string</returns>
        public string ToString(Key K)
        {
            return this.ToString(K, DELIM);
        }

        /// <summary>
        /// Returns the hash code for the entire record
        /// </summary>
        /// <returns>An interger hash code</returns>
        public override int GetHashCode()
        {
            int hxc = 0;
            for (int i = 0; i < this.Count; i++)
            {
                hxc += (i + 1) * this[i].GetHashCode();
            }
            return hxc;
        }

        /// <summary>
        /// Returns the hash code for the entire record
        /// </summary>
        /// <param name="K">A key to filter the record on</param>
        /// <returns>An interger hash code</returns>
        public int GetHashCode(Key K)
        {
            int hxc = 0;
            for (int i = 0; i < K.Count; i++)
            {
                hxc += (i + 1) * this[K[i]].GetHashCode();
            }
            return hxc;
        }

        /// <summary>
        /// Writes a record to a stream
        /// </summary>
        /// <param name="Writer"></param>
        public void Write(System.IO.TextWriter Writer)
        {
            Writer.WriteLine(this.ToString());
        }

        // Comparers //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="R1"></param>
        /// <param name="K1"></param>
        /// <param name="R2"></param>
        /// <param name="K2"></param>
        /// <returns></returns>
        public static int Compare(Record R1, Key K1, Record R2, Key K2)
        {

            if (K1.Count != K2.Count)
                throw new Exception("Keys size do not match " + K1.Count.ToString() + " : " + K2.Count.ToString());

            int c = 0;
            for (int i = 0; i < K1.Count; i++)
            {
                c = CellComparer.Compare(R1[K1[i]], R2[K2[i]]);
                if (K1.Affinity(i) == KeyAffinity.Descending || K2.Affinity(i) == KeyAffinity.Descending) c = -c;
                if (c != 0) return c;
            }
            return 0;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="R1"></param>
        /// <param name="R2"></param>
        /// <param name="K"></param>
        /// <returns></returns>
        public static int Compare(Record R1, Record R2, Key K)
        {

            int c = 0;
            for (int i = 0; i < K.Count; i++)
            {
                c = CellComparer.Compare(R1[K[i]], R2[K[i]]);
                if (K.Affinity(i) == KeyAffinity.Descending) c = -c;
                if (c != 0) return c;
            }
            return 0;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="R1"></param>
        /// <param name="R2"></param>
        /// <returns></returns>
        public static int Compare(Record R1, Record R2)
        {

            if (R1.Count != R2.Count)
                throw new Exception("Record sizes do not match " + R1.Count.ToString() + " : " + R2.Count.ToString());

            int c = 0;
            for (int i = 0; i < R1.Count; i++)
            {
                c = CellComparer.Compare(R1[i], R2[i]);
                if (c != 0) return c;
            }
            return 0;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="R1"></param>
        /// <param name="K1"></param>
        /// <param name="R2"></param>
        /// <param name="K2"></param>
        /// <returns></returns>
        public static bool Equals(Record R1, Key K1, Record R2, Key K2)
        {

            if (K1.Count != K2.Count)
                throw new Exception("Keys size do not match " + K1.Count.ToString() + " : " + K2.Count.ToString());

            for (int i = 0; i < K1.Count; i++)
            {

                if (R1[K1[i]].INT_A != R2[K2[i]].INT_A)
                    return false;
                else if (R1[K1[i]] != R2[K2[i]])
                    return false;

            }
            return true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="R1"></param>
        /// <param name="R2"></param>
        /// <returns></returns>
        public static bool Equals(Record R1, Record R2)
        {

            if (R1.Count != R2.Count)
                throw new Exception("Keys size do not match " + R1.Count.ToString() + " : " + R2.Count.ToString());

            for (int i = 0; i < R1.Count; i++)
            {

                if (R1[i].INT_A != R2[i].INT_A)
                    return false;
                else if (R1[i] != R2[i])
                    return false;

            }
            return true;

        }

        // Builders //
        ///// <summary>
        ///// Unboxes an object into a cell; throws an exception if the unbox fails
        ///// </summary>
        ///// <param name="Elements">An array of objects</param>
        ///// <returns>A record representation of each object</returns>
        //public static Record Unbox(params object[] Elements)
        //{

        //    int len = Elements.Length;
        //    Cell[] b = new Cell[len];

        //    for (int i = 0; i < len; i++)
        //    {
        //        b[i] = Cell.UnBox(Elements[i]);
        //    }
        //    return new Record(b);

        //}

        ///// <summary>
        ///// Unboxes an object into a cell; puts a null cell into the record if the unbox fails
        ///// </summary>
        ///// <param name="Elements">An array of objects</param>
        ///// <returns>A record representation of each object</returns>
        //public static Record TryUnbox(params object[] Elements)
        //{

        //    int len = Elements.Length;
        //    Cell[] b = new Cell[len];

        //    for (int i = 0; i < len; i++)
        //    {
        //        b[i] = Cell.TryUnBox(Elements[i]);
        //    }
        //    return new Record(b);

        //}

        ///// <summary>
        ///// Unboxes an object into a specific type; throws an exception if the unbox fails
        ///// </summary>
        ///// <param name="Columns">A schema representing the types to unbox</param>
        ///// <param name="Elements">The collection of objects</param>
        ///// <returns>A record</returns>
        //public static Record UnboxInto(Schema Columns, params object[] Elements)
        //{

        //    int len = Elements.Length;
        //    if (len != Columns.Count)
        //        throw new Exception("Column count does not match data length");
        //    Cell[] b = new Cell[len];

        //    for (int i = 0; i < len; i++)
        //    {
        //        b[i] = Cell.UnBoxInto(Elements[i], Columns.ColumnAffinity(i));
        //    }
        //    return new Record(b);

        //}

        ///// <summary>
        ///// Unboxes an object into a specific type; returns null if the unbox fails
        ///// </summary>
        ///// <param name="Columns">A schema representing the types to unbox</param>
        ///// <param name="Elements">The collection of objects</param>
        ///// <returns>A record</returns>
        //public static Record TryUnboxInto(Schema Columns, params object[] Elements)
        //{

        //    int len = Elements.Length;
        //    if (len != Columns.Count) throw new Exception("Column count does not match data length");
        //    Cell[] b = new Cell[len];

        //    for (int i = 0; i < len; i++)
        //    {
        //        b[i] = Cell.TryUnBoxInto(Elements[i], Columns.ColumnAffinity(i));
        //    }
        //    return new Record(b);

        //}

        /// <summary>
        /// Combines a variables length array of cells into a record
        /// </summary>
        /// <param name="Elements">A variable array of cells</param>
        /// <returns>A record</returns>
        public static Record Stitch(params Cell[] Data)
        {
            return new Record(Data);
        }

        // Others //
        /// <summary>
        /// Combines two records
        /// </summary>
        /// <param name="R1">The left record</param>
        /// <param name="R2">The right record</param>
        /// <returns>A record</returns>
        public static Record Join(Record R1, Record R2)
        {

            List<Cell> c = new List<Cell>();
            for (int i = 0; i < R1.Count; i++)
            {
                c.Add(R1[i]);
            }
            for (int i = 0; i < R2.Count; i++)
            {
                c.Add(R2[i]);
            }
            return new Record(c.ToArray());

        }

        /// <summary>
        /// Combines two records
        /// </summary>
        /// <param name="R1">The left record</param>
        /// <param name="K1">A key filter to apply to the left record</param>
        /// <param name="R2">The right record</param>
        /// <param name="K2">A key filter to apply to the right record</param>
        /// <returns>A record</returns>
        public static Record Join(Record R1, Key K1, Record R2, Key K2)
        {

            List<Cell> c = new List<Cell>();
            for (int i = 0; i < K1.Count; i++)
            {
                c.Add(R1[K1[i]]);
            }
            for (int i = 0; i < K2.Count; i++)
            {
                c.Add(R2[K2[i]]);
            }
            return new Record(c.ToArray());

        }

        /// <summary>
        /// Cuts a reocrd into a smaller record
        /// </summary>
        /// <param name="R">A record to split</param>
        /// <param name="K">A key to filter the record on</param>
        /// <returns>A record</returns>
        public static Record Split(Record R, Key K)
        {
            List<Cell> c = new List<Cell>();
            for (int i = 0; i < K.Count; i++)
            {
                c.Add(R[K[i]]);
            }
            return new Record(c.ToArray());
        }

        /// <summary>
        /// Chops a record into a smaller record
        /// </summary>
        /// <param name="R">The record to chop</param>
        /// <param name="Start">The starting index of the record</param>
        /// <param name="Length">The length of the new record</param>
        /// <returns>A record</returns>
        public static Record Subrecord(Record R, int Start, int Length)
        {
            Cell[] c = new Cell[Length];
            Array.Copy(R.BaseArray, Start, c, 0, Length);
            return new Record(c);
        }

        // Opperators //
        /// <summary>
        /// Concatenates the cell and the record
        /// </summary>
        /// <param name="C"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static Record operator +(Cell C, Record R)
        {
            Cell[] x = new Cell[R.Count + 1];
            Array.Copy(R._data, 0, x, 1, R.Count);
            x[0] = C;
            return new Record(x);
        }

        /// <summary>
        /// Concatenates the cell and the record
        /// </summary>
        /// <param name="R"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Record operator +(Record R, Cell C)
        {
            Cell[] x = new Cell[R.Count + 1];
            Array.Copy(R._data, 0, x, 0, R.Count);
            x[R.Count] = C;
            return new Record(x);
        }

        /// <summary>
        /// Concatenates two records
        /// </summary>
        /// <param name="R1"></param>
        /// <param name="R2"></param>
        /// <returns></returns>
        public static Record operator +(Record R1, Record R2)
        {

            List<Cell> c = new List<Cell>();
            for (int i = 0; i < R1.Count; i++)
            {
                c.Add(R1[i]);
            }
            for (int i = 0; i < R2.Count; i++)
            {
                c.Add(R2[i]);
            }
            return new Record(c.ToArray());

        }

        /// <summary>
        /// Splits a record according to a key
        /// </summary>
        /// <param name="R"></param>
        /// <param name="K"></param>
        /// <returns></returns>
        public static Record operator *(Record R, Key K)
        {
            return Record.Split(R, K);
        }

        // Costs //
        public int MemCost
        {
            get
            {
                int cost = 0;
                foreach (Cell c in this._data)
                    cost += CellSerializer.MemorySize(c);
                return cost;
            }
        }

        public int DiskCost
        {
            get
            {
                int cost = 0;
                foreach (Cell c in this._data)
                    cost += CellSerializer.DiskSize(c);
                return cost;
            }
        }

        public int DataCost
        {
            get
            {
                int cost = 0;
                foreach (Cell c in this._data)
                    cost += CellSerializer.Length(c);
                return cost;
            }
        }

    }

    public class AssociativeRecord : Record
    {

        private Schema _Columns;

        public AssociativeRecord(Schema Columns, Cell[] Values)
            : base(Columns.Count)
        {
            this._Columns = Columns;
            this._data = Values;
        }

        public AssociativeRecord(Schema Columns, Record Value)
            : this(Columns, Value._data)
        {
        }

        public AssociativeRecord(Schema Columns)
            : this(Columns, Columns.NullRecord)
        {
        }

        public AssociativeRecord(AssociativeRecord Value)
            : base(Value)
        {
            this._Columns = Value.Columns;
        }

        public Schema Columns
        {
            get { return this._Columns; }
        }

        public Cell this[string Name]
        {
            get
            {
                int idx = this._Columns.ColumnIndex(Name);
                return this._data[idx];
            }
            set
            {
                int idx = this._Columns.ColumnIndex(Name);
                if (this._Columns.ColumnAffinity(idx) != value.Affinity)
                    value = CellConverter.Cast(value, this._Columns.ColumnAffinity(idx));
                this._data[idx] = value;
            }
        }

    }

}
