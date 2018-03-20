using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;


namespace ArcticWind.Tables
{

    /// <summary>
    /// Represents a collection of Alias, Type, Type PageSize, and Nullness; this forms the basis of horse structured datasets
    /// </summary>
    public sealed class Schema
    {

        public const char SCHEMA_DELIM = ',';
        public const char ALIAS_DELIM = '.';
        public const int OFFSET_NAME = 0;
        public const int OFFSET_AFFINITY = 1;
        public const int OFFSET_NULL = 2;
        public const int OFFSET_SIZE = 3;
        public const int RECORD_LEN = 4;
        public const int MAX_COLUMNS = 1024;
        public const int MAX_COLUMN_NAME_LEN = 32;
        
        /* Sizing variables:
         * -- Represent the maxium size of the data on disk, not in memory
         * -- Each element takes up two meta data bytes
         *      -- Affinity
         *      -- Null bit
         * -- Bools (BOOL) take up 1 byte
         * -- Longs (LONG), Dates (DATE_TIME), floating points (DOUBLE) all take up 8 bytes
         * -- Strings take up 4 + 2 OriginalPage n bytes of data (4 == length, each n takes up 2 bytes)
         * -- Blobs take up 4 + n bytes of data
         * 
         */
        internal const int MAX_VARIABLE_SIZE = 8192; // 8k
        internal const int DEFAULT_STRING_SIZE = 64;
        internal const int DEFAULT_BLOB_SIZE = 16; // 256 bit

        internal Heap<Record> _Cache;
        private int _HashCode = int.MaxValue ^ (int)short.MaxValue;

        // Schema //
        /// <summary>
        /// Initializes an empty schema
        /// </summary>
        public Schema()
        {
            this._Cache = new Heap<Record>();
        }

        /// <summary>
        /// Initializes a schema given a set of recors; note: this is designed only to be called by the serializer.
        /// </summary>
        /// <param name="Cache">A list of records, each with four elements</param>
        public Schema(List<Record> Cache)
            : this()
        {
            for (int i = 0; i < Cache.Count; i++)
            {
                if (Cache[i].Count == RECORD_LEN)
                {
                    this._Cache.Allocate(Cache[i][OFFSET_NAME].valueCSTRING, Cache[i]);
                    this._HashCode += Cache[i].GetHashCode(new Key(1, 2)) * this.Count;
                }
            }
        }

        /// <summary>
        /// Initializes a schema based on text passed
        /// </summary>
        /// <param name="BString">A string representing the schema</param>
        public Schema(string Text)
            : this()
        {
            string[] s = Text.Split(SCHEMA_DELIM);
            foreach (string t in s)
            {
                this.Add(t);
            }
        }

        // Properties //
        /// <summary>
        /// Returns the count of columns in the schema
        /// </summary>
        public int Count
        {
            get
            {
                return this._Cache.Count;
            }
        }

        /// <summary>
        /// Returns a record filled with null cell elements based on the Spike schema
        /// </summary>
        public Record NullRecord
        {
            get
            {
                List<Cell> c = new List<Cell>();
                for (int i = 0; i < this.Count; i++)
                {
                    c.Add(new Cell(this.ColumnAffinity(i)));
                }
                return new Record(c.ToArray());
            }
        }

        /// <summary>
        /// Gets the highest possible record
        /// </summary>
        public Record MaxRecord
        {
            get
            {
                RecordBuilder rb = new RecordBuilder();
                for (int i = 0; i < this.Count; i++)
                {
                    rb.Add(CellValues.Max(this.ColumnAffinity(i)));
                }
                return rb.ToRecord();
            }
        }

        /// <summary>
        /// Gets the data cost
        /// </summary>
        public int RecordDataCost
        {
            get
            {
                int Size = 0;
                for (int i = 0; i < this.Count; i++)
                {
                    Size += CellSerializer.Length(this.ColumnAffinity(i), this.ColumnSize(i));
                }
                return Size;
            }
        }

        /// <summary>
        /// Gets the memory cost
        /// </summary>
        public int RecordMemCost
        {
            get
            {
                int Size = 0; 
                for (int i = 0; i < this.Count; i++)
                {
                    Size += CellSerializer.MemorySize(this.ColumnAffinity(i), this.ColumnSize(i));
                }
                return Size;
            }
        }

        /// <summary>
        /// Gets the cost on disk
        /// </summary>
        public int RecordDiskCost
        {
            get
            {
                int Size = 0;
                for (int i = 0; i < this.Count; i++)
                {
                    Size += CellSerializer.DiskSize(this.ColumnAffinity(i), this.ColumnSize(i));
                }
                return Size;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int DataCost
        {

            get 
            { 
                return Schema.SchemaSchema().RecordDataCost * this.Count; 
            }

        }
        
        /// <summary>
        /// 
        /// </summary>
        public int MemCost
        {

            get 
            { 
                return Schema.SchemaSchema().RecordMemCost * this.Count; 
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public int DiskCost
        {

            get 
            { 
                return Schema.SchemaSchema().RecordDiskCost * this.Count; 
            }

        }

        // Property Functions //
        /// <summary>
        /// Finds the index of the column in the schema; the seek is not case sensative
        /// </summary>
        /// <param name="Alias">A column name</param>
        /// <returns>An integer index of the column; -1 if the column does not exist</returns>
        public int ColumnIndex(string Name)
        {
            int i = 0;
            string t = Name.Trim();
            if (this._Cache.Exists(t))
                return this._Cache.GetPointer(Name);
            return -1;
        }

        /// <summary>
        /// Gets a column name given an index
        /// </summary>
        /// <param name="Index">A column's index in the schema</param>
        /// <returns>A column</returns>
        public string ColumnName(int Index)
        {
            if (Index < 0 || Index >= this.Count)
                throw new Exception("Index supplied is invalid: " + Index.ToString() + " : " + this.Count.ToString());
            return this._Cache[Index][OFFSET_NAME].valueCSTRING;
        }

        /// <summary>
        /// Checks to see if the column exists
        /// </summary>
        /// <param name="Alias">A column name</param>
        /// <returns>True if the column exists; false if the column does not</returns>
        public bool Contains(string Name)
        {
            return this._Cache.Exists(Name);
        }

        /// <summary>
        /// Gets a column affinity given given an index
        /// </summary>
        /// <param name="Index">A column's index in the schema</param>
        /// <returns>A cell affinity</returns>
        public CellAffinity ColumnAffinity(int Index)
        {
            if (Index < 0 || Index >= this.Count)
                throw new Exception("Index supplied is invalid: " + Index.ToString());
            return (CellAffinity)this._Cache[Index][OFFSET_AFFINITY].LONG;
        }

        /// <summary>
        /// Gets a column's affinity
        /// </summary>
        /// <param name="Alias">A column name</param>
        /// <returns>A cell affinity</returns>
        public CellAffinity ColumnAffinity(string Name)
        {
            return this.ColumnAffinity(this.ColumnIndex(Name));
        }

        /// <summary>
        /// Gets a column nullness given given an index
        /// </summary>
        /// <param name="Index">A column's index in the schema</param>
        /// <returns>True if the column can be nulled, false if it cant</returns>
        public bool ColumnNull(int Index)
        {
            if (Index < 0 || Index >= this.Count)
                throw new Exception("Index supplied is invalid: " + Index.ToString());
            return this._Cache[Index][OFFSET_NULL].valueBOOL;
        }

        /// <summary>
        /// Gets a column's nullness
        /// </summary>
        /// <param name="Alias">A column name</param>
        /// <returns>True if the column can be nulled, false otherwise</returns>
        public bool ColumnNull(string Name)
        {
            return this.ColumnNull(this.ColumnIndex(Name));
        }

        /// <summary>
        /// Gets a column size given given an index
        /// </summary>
        /// <param name="Index">A column's index in the schema</param>
        /// <returns>The column's type's size</returns>
        public int ColumnSize(int Index)
        {
            if (Index < 0 || Index >= this.Count)
                throw new Exception("Index supplied is invalid: " + Index.ToString());
            return (int)this._Cache[Index][OFFSET_SIZE].LONG;
        }

        /// <summary>
        /// Gets a column's size
        /// </summary>
        /// <param name="Alias">A column name</param>
        /// <returns>The column's type's size</returns>
        public int ColumnSize(string Name)
        {
            return this.ColumnSize(this.ColumnIndex(Name));
        }

        // Adds //
        /// <summary>
        /// Adds a column to the schema; will throw an exception if a column name passed already exists in the schema
        /// </summary>
        /// <param name="Alias">The column name</param>
        /// <param name="Affinity">The column affinity</param>
        /// <param name="Nullable">A boolean, true means the column can be nulls, false means the column cannot be null</param>
        /// <param name="PageSize">The size in bytes; this will be ignored if the affinity is not variable (not string or blob)</param>
        public void Add(string Name, CellAffinity Affinity, bool Nullable, int Size)
        {

            // Check the name size //
            if (Name.Length > MAX_COLUMN_NAME_LEN)
                Name = Name.Substring(0, MAX_COLUMN_NAME_LEN);

            // Check the size //
            if (CellAffinityHelper.IsVariableLength(Affinity) && Size == 0)
                throw new Exception("Variable length types must have a size greater than zero");

            // Check if exists //
            if (this.Contains(Name))
                throw new Exception("Column already exists: " + Name);

            // Check for capacity //
            if (this.Count >= MAX_COLUMNS)
                throw new Exception("Schema cannot accept any more columns");

            // Get the size //
            Size = CellSerializer.FixLength(Affinity, Size);

            // Build record //
            Record r = Record.Stitch(new Cell(Name), new Cell((byte)Affinity), new Cell(Nullable), new Cell(Size));
            
            // Accumulate record //
            this._Cache.Allocate(Name, r);

            // Hash code //
            this._HashCode += r.GetHashCode(new Key(1, 2)) * this.Count;

        }

        /// <summary>
        /// Adds a column to the schema; will throw an exception if a column name passed already exists in the schema; assumes the column is nullable
        /// </summary>
        /// <param name="Alias">The column name</param>
        /// <param name="Affinity">The column affinity</param>
        /// <param name="PageSize">The size in bytes; this will be ignored if the affinity is not variable (not string or blob)</param>
        public void Add(string Name, CellAffinity Affinity, int Size)
        {
            this.Add(Name, Affinity, false, Size);
        }

        /// <summary>
        /// Adds a column to the schema; will throw an exception if a column name passed already exists in the schema; assumes the column is nullable; assumes a default type size
        /// </summary>
        /// <param name="Alias">The column name</param>
        /// <param name="Affinity">The column affinity</param>
        public void Add(string Name, CellAffinity Affinity)
        {
            this.Add(Name, Affinity, true, -1);
        }

        /// <summary>
        /// Adds a columns based on a text expression
        /// </summary>
        /// <param name="ScalarExpression">A text expression [Alias] [Type].[PageSize] [Nullable]</param>
        public void Add(string Expression)
        {

            /*
             * <NAME>.<TYPE>.<SIZE>.<NULL>
             * 
             * Len 2 = name, type
             * Len 3 = name, type, size
             * Len 4 = name, type, size, bool
             * 
             */

            // Split //
            string[] s = Expression.Trim().Split('.');

            // Check length //
            if (s.Length < 2 || s.Length > 5)
                throw new Exception("ScalarExpression is invalid: " + Expression);

            // Set defaults //
            string name = s[0].Trim();
            CellAffinity ca = CellAffinityHelper.Parse(s[1].ToUpper().Trim());
            bool n = true;
            int Size = -1;

            // nullability //
            if (s.Length == 3)
            {
                Size = int.Parse(s[2]);
            }
            else if (s.Length == 4)
            {
                Size = int.Parse(s[2]);
                if (s[3].Trim().ToUpper() == "FALSE")
                    n = false;
            }

            // Accumulate the Value //
            this.Add(name, ca, n, Size);

        }

        // Special methods //
        /// <summary>
        /// Renames a column to another name
        /// </summary>
        /// <param name="OldName">The Spike name</param>
        /// <param name="NewName">The proposed name</param>
        public void Rename(string OldName, string NewName)
        {
            if (this.Contains(NewName))
                throw new Exception("Rename-Column already exists: " + NewName);
            if (!this.Contains(OldName))
                throw new Exception("Rename-Column does not exist: " + OldName);

            int i = this.ColumnIndex(OldName);
            Record r = this._Cache[i];
            r[OFFSET_NAME] = new Cell(NewName);
            this._Cache.Deallocate(OldName);
            this._Cache.Allocate(NewName, r);

        }

        /// <summary>
        /// Checks a record for nullness, type affinity, size overflow, and field count
        /// </summary>
        /// <param name="R">A record to check</param>
        /// <param name="FixAffinity">If true, the method will cast an invalid affinity cell</param>
        /// <returns>A boolean describing whether or not the check fails</returns>
        public bool Check(Record R, bool FixAffinity)
        {

            // Check length //
            if (R.Count != this.Count)
                throw new Exception(String.Format("Record size {0} does not match schema size {1}", R.Count, this.Count));

            // Check nullness and affinity //
            for (int i = 0; i < R.Count; i++)
            {

                // Check affinity //
                if (R[i].Affinity != this.ColumnAffinity(i) && this.ColumnAffinity(i) != CellAffinity.VARIANT && !FixAffinity)
                    throw new Exception(String.Format("Column Affinities do not match {0} : {1} != {2}", i, R[i].Affinity, this.ColumnAffinity(i)));
                else if (R[i].Affinity != this.ColumnAffinity(i) && this.ColumnAffinity(i) != CellAffinity.VARIANT)
                    R[i] = CellConverter.Cast(R[i], this.ColumnAffinity(i));
                
                // Check nullness //
                if (!this.ColumnNull(i) && R[i].IsNull)
                    throw new Exception(String.Format("Column {0} is null", i));

            }

            return true;
        }

        /// <summary>
        /// Checks a record for nullness, type affinity, size overflow, and field count; turns off auto-casting
        /// </summary>
        /// <param name="R">A record to check</param>
        /// <returns>A boolean describing whether or not the check fails</returns>
        public bool Check(Record R)
        {
            return this.Check(R, false);
        }

        /// <summary>
        /// Checks a record for nullness, type affinity, size overflow, and field count; wont throw an exception
        /// </summary>
        /// <param name="R">A record to check</param>
        /// <returns>A boolean describing whether or not the check fails</returns>
        public bool TryCheck(Record R)
        {

            try
            {
                return this.Check(R);
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Fixing any affinity issues by auto casting the cell type to the correct type based on the column
        /// </summary>
        /// <param name="R">A record to check</param>
        /// <returns>A record</returns>
        public Record Fix(Record R)
        {

            // Check nullness and affinity //
            for (int i = 0; i < R.Count; i++)
            {

                // Cast to a new affinity if need by //
                if (R[i].Affinity != this.ColumnAffinity(i) && this.ColumnAffinity(i) != CellAffinity.VARIANT)
                    R[i] = CellConverter.Cast(R[i], this.ColumnAffinity(i));

            }

            return R;

        }

        /// <summary>
        /// Parses a string into a key
        /// </summary>
        /// <param name="BString">The text list of columns</param>
        /// <returns>A key</returns>
        public Key KeyParse(string Text)
        {

            Key k = new Key();

            if (Text == "*")
                return Key.Build(this.Count);

            string[] t = Text.Split(',');

            foreach (string s in t)
            {

                // Parse out the 'NAME KEY_AFFINITY' logic //
                string[] u = s.Trim().Split(' ');
                string v = u[0]; // column name
                string w = "A"; // affinity (Optional)
                if (u.Length > 1)
                    w = u[1];

                // get index and affinity
                int j = this.ColumnIndex(v);
                KeyAffinity a = Key.ParseAffinity(w);

                // Accumulate values //
                if (j != -1)
                    k.Add(j, a);
                else if (v.ToList().TrueForAll((c) => { return "1234567890".Contains(c); }))
                    k.Add(int.Parse(v), a);

            }
            return k;

        }

        /// <summary>
        /// Parses a string into a key
        /// </summary>
        /// <param name="Columns">A variable list of columns</param>
        /// <returns>A key</returns>
        public Key KeyParse(string[] Columns)
        {
            Key k = new Key();
            foreach (string s in Columns)
            {
                k.Add(this.ColumnIndex(s));
            }
            return k;
        }

        /// <summary>
        /// Gets a key that links to all columns in the table
        /// </summary>
        /// <returns></returns>
        public Key GetKey()
        {
            return Key.Build(this.Count);
        }

        /// <summary>
        /// Given a key, will fill it in with the remaining column indexes that are not present in the key
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public Key GetKey(Key Key)
        {
            Key k = Key.CloneOfMe();
            for (int i = 0; i < this.Count; i++)
            {
                if (k.IndexOf(i) == -1)
                    k.Add(i);
            }
            return k;
        }

        // Prints //
        /// <summary>
        /// Prints the contents of a schema
        /// </summary>
        public void Print()
        {
            for (int i = 0; i < this.Count; i++)
            {
                Console.WriteLine("{0} : {1} : {2} : {3}", this.ColumnName(i), this.ColumnAffinity(i), this.ColumnSize(i), this.ColumnNull(i));
            }
        }

        // Header data //
        /// <summary>
        /// Returns a string representation of the schema: NAME TYPE(.SIZE)? NULLABLE
        /// </summary>
        /// <param name="Delim">A character to deliminate the fields</param>
        /// <returns>A string</returns>
        public string ToString(char Delim)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this._Cache.Count; i++)
            {
                sb.Append(this.ColumnName(i) + " " + this.ColumnAffinity(i).ToString());
                if (this.ColumnAffinity(i) == CellAffinity.BINARY || this.ColumnAffinity(i) == CellAffinity.CSTRING)
                    sb.Append("." + this.ColumnSize(i).ToString());
                if (this.ColumnNull(i) == true)
                {
                    sb.Append(" TRUE");
                }
                else
                {
                    sb.Append(" FALSE");
                }
                if (i != this._Cache.Count - 1)
                    sb.Append(Delim);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns a string with a ',' deliminator
        /// </summary>
        /// <returns>A string representation of the scheam</returns>
        public override string ToString()
        {
            return this.ToString(SCHEMA_DELIM);
        }

        /// <summary>
        /// Returns a string representation of the schema: NAME TYPE(.SIZE)? NULLABLE
        /// </summary>
        /// <param name="K">A key to filter on</param>
        /// <param name="Delim">A character to deliminate the fields</param>
        /// <returns>A string</returns>
        public string ToString(Key K, char Delim)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < K.Count; i++)
            {
                sb.Append(this.ColumnName(K[i]) + " " + this.ColumnAffinity(K[i]).ToString());
                if (this.ColumnNull(K[i]) == true)
                {
                    sb.Append(" NULL");
                }
                else
                {
                    sb.Append(" NOT NULL");
                }
                if (i != K.Count - 1) sb.Append(Delim);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns a string representation of the schema: NAME TYPE(.SIZE)? NULLABLE
        /// </summary>
        /// <param name="K">A key to filter on</param>
        /// <returns>A string</returns>
        public string ToString(Key K)
        {
            return this.ToString(K, SCHEMA_DELIM);
        }

        /// <summary>
        /// Returns a string of column names
        /// </summary>
        /// <param name="Delim">A character to deliminate the fields</param>
        /// <returns>A string</returns>
        public string ToNameString(char Delim)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this._Cache.Count; i++)
            {
                sb.Append(this.ColumnName(i));
                if (i != this._Cache.Count - 1) sb.Append(Delim);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns a string of column names
        /// </summary>
        /// <returns>A string</returns>
        public string ToNameString()
        {
            return this.ToNameString(SCHEMA_DELIM);
        }

        /// <summary>
        /// Returns a string of column names
        /// </summary>
        /// <param name="K">A key to filter on</param>
        /// <param name="Delim">A character to deliminate the fields</param>
        /// <returns>A string</returns>
        public string ToNameString(Key K, char Delim)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < K.Count; i++)
            {
                sb.Append(this.ColumnName(K[i]));
                if (i != K.Count - 1) sb.Append(Delim);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns a string of column names
        /// </summary>
        /// <param name="K">A key to filter on</param>
        /// <returns>A string</returns>
        public string ToNameString(Key K)
        {
            return this.ToNameString(K, SCHEMA_DELIM);
        }

        /// <summary>
        /// Returns a string of column affinities
        /// </summary>
        /// <param name="Delim">A character to deliminate the fields</param>
        /// <returns>A string</returns>
        public string ToAffinityString(char Delim)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this._Cache.Count; i++)
            {
                sb.Append(this.ColumnAffinity(i).ToString());
                if (i != this._Cache.Count - 1) sb.Append(Delim);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns a string of column affinities
        /// </summary>
        /// <returns>A string</returns>
        public string ToAffinityString()
        {
            return this.ToAffinityString(SCHEMA_DELIM);
        }

        /// <summary>
        /// Returns a string of column affinities
        /// </summary>
        /// <param name="K">A key to filter on</param>
        /// <param name="Delim">A character to deliminate the fields</param>
        /// <returns>A string</returns>
        public string ToAffinityString(Key K, char Delim)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < K.Count; i++)
            {
                sb.Append(this.ColumnAffinity(K[i]).ToString());
                if (i != K.Count - 1) sb.Append(Delim);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns a string of column affinities
        /// </summary>
        /// <returns>A string</returns>
        public string ToAffinityString(Key K)
        {
            return this.ToAffinityString(K, SCHEMA_DELIM);
        }

        /// <summary>
        /// Returns a unique hash code Value based on the schema's type and order; two schema with identical field types and 
        /// </summary>
        /// <returns>An integer hash code</returns>
        public override int GetHashCode()
        {
            return this._HashCode;
        }

        /// <summary>
        /// Checks if a given object's hash code matches another's; does not attempt to cast obj as a schema
        /// </summary>
        /// <param name="obj">An object passed</param>
        /// <returns>An bool indicating if the objects have the same hash code</returns>
        public override bool Equals(object obj)
        {
            return this._HashCode == obj.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string DebugString()
        {

            StringBuilder sb = new StringBuilder();
            foreach (Record r in this._Cache.Values)
            {
                sb.AppendLine(string.Format("{0}.{1}.{2}.{3}", r[0], r[1], r[2], r[3]));
            }
            return sb.ToString();

        }

        // Statics //
        /// <summary>
        /// Combines two schemas; throws an exception if two columns have the same name.
        /// </summary>
        /// <param name="S1">The left schema</param>
        /// <param name="S2">The right schema</param>
        /// <returns>A combined schema</returns>
        public static Schema Join(Schema S1, Schema S2)
        {

            Schema s = new Schema();
            for (int i = 0; i < S1.Count; i++)
            {
                s.Add(S1.ColumnName(i), S1.ColumnAffinity(i), S1.ColumnNull(i), S1.ColumnSize(i));
            }
            for (int i = 0; i < S2.Count; i++)
            {
                s.Add(S2.ColumnName(i), S2.ColumnAffinity(i), S2.ColumnNull(i), S2.ColumnSize(i));
            }
            return s;

        }

        /// <summary>
        /// Creates a schema from another schema
        /// </summary>
        /// <param name="S">The starting point schema</param>
        /// <param name="K">A key representing the columns to keep</param>
        /// <returns>A schema</returns>
        public static Schema Split(Schema S, Key K)
        {

            Schema s = new Schema();
            for (int i = 0; i < K.Count; i++)
            {
                s.Add(S.ColumnName(K[i]), S.ColumnAffinity(K[i]), S.ColumnNull(K[i]), S.ColumnSize(K[i]));
            }
            return s;

        }

        /// <summary>
        /// Checks if two columns have compatible schemas
        /// </summary>
        /// <param name="S1">The left schema</param>
        /// <param name="S2">The right schema</param>
        /// <returns>A boolean indicating whether or not each schema's type arrays match</returns>
        public static bool operator ==(Schema S1, Schema S2)
        {
            int hc1 = (S1 ?? new Schema())._HashCode;
            int hc2 = (S2 ?? new Schema())._HashCode;
            return hc1 == hc2;
        }

        /// <summary>
        /// Checks if two columns have incompatible schemas
        /// </summary>
        /// <param name="S1">The left schema</param>
        /// <param name="S2">The right schema</param>
        /// <returns>A boolean indicating whether or not each schema's type arrays match</returns>
        public static bool operator !=(Schema S1, Schema S2)
        {
            int hc1 = (S1 ?? new Schema())._HashCode;
            int hc2 = (S2 ?? new Schema())._HashCode;
            return hc1 != hc2;
        }

        /// <summary>
        /// Returns the schema's inner schema type NAME, TYPE, ISNULL, SIZE
        /// </summary>
        /// <returns>A schema</returns>
        public static Schema SchemaSchema()
        {
            string text = "name.text.64, type.byte, isnull.bool, size.short";
            return new Schema(text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Names"></param>
        /// <param name="Types"></param>
        /// <returns></returns>
        public static Schema FromString(string Names, string Types)
        {

            string[] NameArray = Names.Split(',');
            string[] TypeArray = Types.Split(',');

            if (NameArray.Length != TypeArray.Length)
                throw new ArgumentException("The name and type tags have different counts");

            Schema columns = new Schema();

            for (int i = 0; i < NameArray.Length; i++)
            {

                string[] x = TypeArray[i].Split('.');
                CellAffinity t = CellAffinityHelper.Parse(x[0]);
                int size = 8;
                if (t == CellAffinity.CSTRING && x.Length == 2)
                    size = int.Parse(x[1]);
                else if (t == CellAffinity.CSTRING)
                    size = Schema.DEFAULT_STRING_SIZE;
                else if (t == CellAffinity.BINARY && x.Length == 2)
                    size = int.Parse(x[1]);
                else if (t == CellAffinity.BINARY)
                    size = Schema.DEFAULT_BLOB_SIZE;

                columns.Add(NameArray[i], t, size);

            }

            return columns;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Element"></param>
        /// <returns></returns>
        public static List<Record> ToRecords(Schema Element)
        {
            return Element._Cache.Values;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Element"></param>
        /// <returns></returns>
        public static Schema FromRecords(List<Record> Element)
        {
            return new Schema(Element);
        }


    }

}
