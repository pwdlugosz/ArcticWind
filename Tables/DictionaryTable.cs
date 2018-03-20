using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;

namespace ArcticWind.Tables
{

    /// <summary>
    /// Represents a disk based key-value table
    /// </summary>
    public class DictionaryTable : ClusteredTable, IReadable
    {

        protected int _KeyCount = 0;
        protected int _ValueCount = 0;
        protected Key _KeyFields;
        protected Key _ValueFields;

        /*
         * The _LastRef and _LastKey store the last referenced record key and row pointer. This is used to speed up opperations when the dictionary is used as the 
         * temporary storage for aggregate data. This only optimizes 
         * -- Often time the program will first request the value, and if it does exist, it will try to update the value; this causes two index seeks that could get
         *      reduced to one if we check the last key.
         * -- This also offers a large speed up if we're dealing with ordered or partially ordered data
         */
        protected RecordKey _LastRef = RecordKey.RecordNotFound;
        protected Record _LastKey = null;
        
        public DictionaryTable(Host Host, TableHeader Header)
            : base(Host, Header)
        {
            this._KeyCount = Header.ClusterKey.Count;
            this._ValueCount = this.Columns.Count - this._KeyCount;
            this._KeyFields = Elements.Key.Build(this._KeyCount);
            this._ValueFields = Elements.Key.Build(this._KeyCount, this._ValueCount);
            this._TableType = "DICTIONARY_SCRIBE";
        }

        public DictionaryTable(Host Host, string Name, string Dir, Schema KeyColumns, Schema ValueColumns, int PageSize)
            : base(Host, Name, Dir, Schema.Join(KeyColumns, ValueColumns), Elements.Key.Build(KeyColumns.Count), ClusterState.Unique, PageSize)
        {
            this._KeyCount = KeyColumns.Count;
            this._ValueCount = ValueColumns.Count;
            this._KeyFields = Elements.Key.Build(this._KeyCount);
            this._ValueFields = Elements.Key.Build(this._KeyCount, this._ValueCount);
            this._TableType = "DICTIONARY_SCRIBE";
        }

        // Methods not implemented //   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="IndexColumns"></param>
        public override void CreateIndex(string Name, Key IndexColumns)
        {
            throw new Exception("Cannot create indexes on clustered tables");
        }

        /// <summary>
        /// Splits a table into N sub tables
        /// </summary>
        /// <param name="PartitionIndex"></param>
        /// <param name="PartitionCount"></param>
        /// <returns></returns>
        public override Table Partition(int PartitionIndex, int PartitionCount)
        {
            throw new NotImplementedException();
        }

        // Dictionary Methods //
        /// <summary>
        /// Returns a key containing all the key fields
        /// </summary>
        public Key KeyFields
        {
            get { return this._KeyFields; }
        }

        /// <summary>
        /// Returns a key containing all the value fields
        /// </summary>
        public Key ValueFields
        {
            get { return this._ValueFields; }
        }

        /// <summary>
        /// Adds a key-value pair
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public void Add(Record Key, Record Value)
        {

            // Step the version //
            this._Version++;

            // Check that everything's ok //
            if (Key.Count != this._KeyCount || Value.Count != this._ValueCount)
                throw new ArgumentException("Key or value passed is/are invalid");

            Record r = Record.Join(Key, Value);

            this.Insert(r);

        }

        /// <summary>
        /// Sets a value
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public void SetValue(Record Key, Record Value)
        {

            // Step the version //
            this._Version++;

            // Get the final record value //
            Record r = Record.Join(Key, Value);

            // Check the last key first //
            if (this._LastKey != null)
            {
                if (Record.Compare(Key, this._LastKey) == 0)
                {
                    this._Cluster.Storage.GetPage(this._LastRef.PAGE_ID).Update(r, this._LastRef.ROW_ID);
                    return;
                }
            }

            // Find the page it belongs on //
            ClusterPage p = this._Cluster.SeekPage(r);

            // Find the location of the record //
            int idx = p.Search(r);

            // Error out if the value doesnt actually exist //
            if (idx < 0)
                throw new Cluster.DuplicateKeyException("Key not found");

            // Update the record //
            p.Update(r, idx);

            // Save the last reference and the last key //
            this._LastRef = new RecordKey(p.PageID, idx);
            this._LastKey = Key;

        }

        /// <summary>
        /// Gets a value from the tree
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public Record GetValue(Record Key)
        {

            Record x = this.GetKeyValue(Key);

            if (x == null)
                return null;

            return Record.Split(x, this._ValueFields);

        }

        /// <summary>
        /// Gets a record, invluding the key and value
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public Record GetKeyValue(Record Key)
        {

            // Check the last key first //
            if (this._LastKey != null)
            {
                if (Record.Compare(Key, this._LastKey) == 0)
                {
                    return this._Cluster.Storage.GetPage(this._LastRef.PAGE_ID).Select(this._LastRef.ROW_ID);
                }
            }

            // Get the record key //
            RecordKey x = this._Cluster.SeekFirst(Key, true);

            // This should really only trigger if the table is empty; actually, this should never trigger //
            if (x.IsNotFound)
                return null;

            // Select the record, and if it's null, return //
            Record y = this._Cluster.Storage.TrySelect(x);
            if (y == null)
                return null;

            // Need to check if we didn't find the actual record //
            if (Record.Compare(y, Key, this._KeyFields) == 0)
            {

                this._LastRef = x;
                this._LastKey = Key;
                return y;
            }

            // Not found //
            return null;

        }

        /// <summary>
        /// Checks if the table contains a key
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool ContainsKey(Record Key)
        {
            return this.GetKeyValue(Key) == null;
        }

    }

}
