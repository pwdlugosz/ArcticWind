using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;

namespace ArcticWind.Tables
{

    /// <summary>
    /// Represents a single page in a b+ tree
    /// </summary>
    public class ClusterPage : Page
    {

        private int DEBUG_MAX_RECORDS = -1; // used only for debugging; set to -1 to revert to the classic logic

        public enum BPlusTreeSearchType : int
        {
            FirstElement = -1,
            LastElement = 1,
            AnyElement = 0
        }

        // This overrides:
        // _X0 = parent page ID
        // _B0 = (1 == is leaf, 0 == is branch)
        // _B1 = is highest
        // _B2 = is unique; 0 = not unique, 1 = unique, 2 = quasi unique, which just means it doesnt throw an exception when it finds a duplicate

        private RecordMatcher _StrongMatcher; // Matches all key columns + page id for the branch nodes
        private RecordMatcher _WeakMatcher; // Only matches key columns;
        private RecordMatcher _PageSearchMatcher;
        private Key _StrongKeyColumns;
        private Key _WeakKeyColumns;
        private Key _OriginalKeyColumns; // Used only for generating
        private int _RefColumn = 0;

        public ClusterPage(int PageSize, int PageID, int LastPageID, int NextPageID, int FieldCount, int UsedSpace, Key KeyColumns, bool IsLeaf, ClusterState State)
            : base(PageSize, PageID, LastPageID, NextPageID, FieldCount, UsedSpace)
        {

            //if (PageID == 1 && IsLeaf == false) throw new Exception();

            this._B0 = (byte)(IsLeaf ? 1 : 0); //this.IsLeaf = IsLeaf;
            this.State = State;
            this._OriginalKeyColumns = KeyColumns;
            this._StrongKeyColumns = IsLeaf ? KeyColumns : BranchObjectiveClone(KeyColumns, false);
            if (this.IsLeaf)
            {
                this._StrongMatcher = new RecordMatcher(KeyColumns); // Designed to match keys to keys or elements to elements
                this._WeakMatcher = new RecordMatcher(KeyColumns); // Designed to match keys to keys or elements to elements
                this._PageSearchMatcher = null; // not used
                this._StrongKeyColumns = KeyColumns;
                this._WeakKeyColumns = KeyColumns;
            }
            else
            {
                this._StrongMatcher = new RecordMatcher(BranchObjectiveClone(KeyColumns, false)); // Designed to match keys to keys
                this._WeakMatcher = new RecordMatcher(BranchObjectiveClone(KeyColumns, true)); // Designed to match keys and keys
                this._PageSearchMatcher = new RecordMatcher(BranchObjectiveClone(KeyColumns, true), KeyColumns);
                this._StrongKeyColumns = BranchObjectiveClone(KeyColumns, false);
                this._WeakKeyColumns = BranchObjectiveClone(KeyColumns, true);
            }
            this._RefColumn = KeyColumns.Count;
            this._Type = Page.BTREE_PAGE_TYPE;

        }

        // Overrides //
        public override bool CanAccept(Record Value)
        {
            if (DEBUG_MAX_RECORDS == -1)
                return base.CanAccept(Value);
            else
                return this.Count >= DEBUG_MAX_RECORDS;
        }

        public override void Insert(Record Element)
        {

            int idx = this._Elements.BinarySearch(Element, this._StrongMatcher);
            if (idx >= 0 && this.State == ClusterState.Unique)
            {
                throw new Cluster.DuplicateKeyException(string.Format("Key '{0}' exists on page '{1}' at position '{2}'", Record.Split(Element, this._WeakKeyColumns).ToString(), this.PageID, idx));
            }
            else if (idx >= 0 && this.State == ClusterState.Distinct)
            {
                return;
            }
            else if (idx < 0)
            {
                idx = ~idx;
            }

            if (idx == this.Count && !this.IsHighest)
                throw new Exception("Cannot add a higher record to this page");

            this._Elements.Insert(idx, Element);
            this._UsedSpace += Element.DiskCost;
            this._Version++;

        }

        public override int Search(Record Element)
        {
            return this._Elements.BinarySearch(Element, this._StrongMatcher);
        }

        public override int PageType
        {
            get
            {
                return this._Type;
            }
        }

        // Join Leaf / Branch Methods //
        public int ParentPageID
        {
            get { return this._X0; }
            set { this._X0 = value; }
        }

        public bool IsLeaf
        {
            get 
            { 
                return this._B0 == 1; 
            }
            set 
            {
                if (value)
                    this._B0 = 1;
                else
                    this._B0 = 0;
            }
        }

        public bool IsHighest
        {
            get { return this._B1 == 1; }
            set { this._B1 = (byte)(value ? 1 : 0); }
        }

        public ClusterState State
        {
            get { return (ClusterState)this._B2; }
            set { this._B2 = (byte)(value); }
        }

        public Key StrongKeyColumns
        {
            get { return this._StrongKeyColumns; }
        }

        public Key WeakKeyColumns
        {
            get { return this._WeakKeyColumns; }
        }

        public Key OriginalKeyColumns
        {
            get { return this._OriginalKeyColumns; }
        }

        public List<Record> SelectAll(Record Element)
        {

            int Lower = this.SearchLeaf(Record.Split(Element, this._WeakKeyColumns), BPlusTreeSearchType.FirstElement, true);
            int Upper = this.SearchLeaf(Record.Split(Element, this._WeakKeyColumns), BPlusTreeSearchType.LastElement, true);

            List<Record> elements = new List<Record>();
            if (Lower < 0 || Upper < 0)
                return elements;

            elements.AddRange(this._Elements.GetRange(Lower, Upper - Lower));
            return elements;

        }

        public ClusterPage GenerateXPage(int PageID, int LastPageID, int NextPageID)
        {
            ClusterPage x = new ClusterPage(this.PageSize, PageID, LastPageID, NextPageID, this._FieldCount, 0, this._OriginalKeyColumns, this.IsLeaf, this.State);
            x.IsLeaf = this.IsLeaf;
            return x;
        }

        public ClusterPage SplitXPage(int PageID, int LastPageID, int NextPageID, int Pivot)
        {

            if (this.Count < 2)
                throw new IndexOutOfRangeException("Cannot split a page with fewer than 2 records");
            if (Pivot == 0 || Pivot == this.Count - 1)
                throw new IndexOutOfRangeException("Cannot split on the first or last record");
            if (Pivot < 0)
                throw new IndexOutOfRangeException(string.Format("Pivot ({0}) must be greater than 0", Pivot));
            if (Pivot >= this.Count)
                throw new IndexOutOfRangeException(string.Format("The pivot ({0}) cannot be greater than the element count ({1})", Pivot, this.Count));

            ClusterPage p = this.GenerateXPage(PageID, LastPageID, NextPageID);
            for (int i = Pivot; i < this.Count; i++)
            {
                p._Elements.Add(this._Elements[i]);
            }
            this._Elements.RemoveRange(Pivot, this.Count - Pivot);
            this._Version++;

            // Set the leafness and the parent page id //
            p.IsLeaf = this.IsLeaf;
            p.ParentPageID = this.ParentPageID;

            // Set the sizes //
            p._UsedSpace = p._Elements.Sum((x) => { return x.DiskCost; });
            this._UsedSpace = this._Elements.Sum((x) => { return x.DiskCost; });

            return p;

        }

        // Branch only methods //
        /// <summary>
        /// Inserts a key into the page
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="PageID"></param>
        public void InsertKey(Record Key, int PageID)
        {

            if (this._WeakMatcher.Compare(Key, this._Elements.Last()) > 0 && !this.IsHighest)
                throw new Exception("Can't insert a record greater the max record unless this is the highest page");

            // InsertKey as usual //
            this.InsertKeyUnsafe(Key, PageID);

        }

        /// <summary>
        /// Inserts a key into the table without checking if it is within the bounds of the tree
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="PageID"></param>
        public void InsertKeyUnsafe(Record Key, int PageID)
        {

            // Find the insertion point //
            Record k = Composite(Key, PageID);
            int idx = this._Elements.BinarySearch(k, this._StrongMatcher);
            if (idx >= 0 && this.State == ClusterState.Unique)
            {
                throw new Cluster.DuplicateKeyException(string.Format("Key '{0}' exists on page '{1}' at position '{2}'", Key.ToString(), this.PageID, idx));
            }
            else if (idx >= 0 && this.State == ClusterState.Distinct)
            {
                return;
            }
            else if (idx < 0)
            {
                idx = ~idx;
            }

            // InsertKey as usual //
            this._Elements.Insert(idx, k);
            this._UsedSpace += k.DiskCost;
            this._Version++;

        }

        /// <summary>
        /// Given an element, this finds the page ID of the page it belongs on
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public int GetPageID(Record Key)
        {

            if (this.IsLeaf)
                throw new Exception("Cannot page search a leaf");

            int idx = this._Elements.BinarySearch(Key, this._PageSearchMatcher);
            if (idx < 0)
                idx = ~idx;

            if (idx != this._Elements.Count)
            {
                return this._Elements[idx][this._RefColumn].INT_A;
            }
            else
            {
                throw new Exception();
            }


        }

        /// <summary>
        /// Gets the page ID given an index
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public int GetPageID(int Index)
        {
            return this._Elements[Index][this._RefColumn].INT_A;
        }

        /// <summary>
        /// Gets all the page IDs
        /// </summary>
        /// <returns></returns>
        public List<int> AllPageIDs()
        {

            List<int> ids = new List<int>();
            foreach (Record r in this._Elements)
            {
                int i = r[this._RefColumn].INT_A;
                ids.Add(i);
            }

            return ids;

        }

        /// <summary>
        /// Checks if a Key/PageID exists
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="PageID"></param>
        /// <returns></returns>
        public bool KeyExists(Record Key, int PageID)
        {
            return this._Elements.BinarySearch(Composite(Key, PageID), this._StrongMatcher) >= 0;
        }

        /// <summary>
        /// Checks if a key exists
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool KeyExists(Record Key)
        {
            return this._Elements.BinarySearch(Key, this._WeakMatcher) >= 0;
        }

        /// <summary>
        /// Deletes a record
        /// </summary>
        /// <param name="Key"></param>
        public void Delete(Record Element)
        {

            // Key must be the entire data record //
            int idx = this._Elements.BinarySearch(Element, this._StrongMatcher);
            if (idx < 0)
            {
                throw new IndexOutOfRangeException("Key is not in this page");
            }

            this._UsedSpace -= this._Elements[idx].DiskCost;
            this._Elements.RemoveAt(idx);
            this._Version++;

        }

        /// <summary>
        /// Checks if the key is less than the terminal record
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool LessThanTerminal(Record Key)
        {

            // We want this to be strictly less than the last element, not the case where it may be equal to
            return this._WeakMatcher.Compare(Key, this._Elements.Last()) < 0;

        }

        /// <summary>
        /// Gets the key portion of the terminal record
        /// </summary>
        public Record TerminalKeyOnly
        {
            get { return Record.Split(this._Elements.Last(), this._WeakKeyColumns); }
        }

        // Searches //
        /// <summary>
        /// Finds a page ID given a key
        /// </summary>
        /// <param name="Key">The value to find</param>
        /// <param name="SearchType">The method of search</param>
        /// <returns>A page ID; if the value doesnt exist</returns>
        public int SearchBranch(Record Key, BPlusTreeSearchType SearchType, bool Exact)
        {

            int idx = this._Elements.BinarySearch(Key, this._WeakMatcher);

            // If we didnt find the element, we dont have to search for multiple keys
            if (idx < 0)
            {
                if (!Exact) idx = ~idx;
                return this.GetPageID(idx);
            }

            // If we really don't care about anything, then return the index //
            if (SearchType == BPlusTreeSearchType.AnyElement)
                return idx;

            // Search Upper //
            int pos = 0;
            if (SearchType == BPlusTreeSearchType.FirstElement)
            {

                while (true)
                {
                    pos = this._Elements.BinarySearch(0, idx, Key, this._WeakMatcher);
                    if (pos < 0) break;
                    idx = pos;
                }

            }
            else
            {

                pos = idx;
                while (true)
                {
                    pos = this._Elements.BinarySearch(pos + 1, this.Count - idx - 1, Key, this._WeakMatcher);
                    if (pos < 0) break;
                    idx = pos;
                }

            }

            return this.GetPageID(idx);

        }

        /// <summary>
        /// Finds a record position in a page
        /// </summary>
        /// <param name="Key">The key to search for</param>
        /// <param name="SearchType">The method of search</param>
        /// <returns>The index of the record on this page</returns>
        public int SearchLeaf(Record Key, BPlusTreeSearchType SearchType, bool Exact)
        {

            int idx = this._Elements.BinarySearch(Key, this._WeakMatcher);

            // If we didnt find the element, we dont have to search for multiple keys
            if (idx < 0)
            {
                if (!Exact) idx = ~idx;
                return idx;
            }

            // If we really don't care about anything, then return the index //
            if (SearchType == BPlusTreeSearchType.AnyElement)
                return idx;

            // Search Upper //
            int pos = 0;
            if (SearchType == BPlusTreeSearchType.FirstElement)
            {

                while (true)
                {
                    pos = this._Elements.BinarySearch(0, idx, Key, this._WeakMatcher);
                    if (pos < 0) break;
                    idx = pos;
                }

            }
            else
            {

                pos = idx;
                while (true)
                {
                    pos = this._Elements.BinarySearch(pos + 1, this.Count - idx - 1, Key, this._WeakMatcher);
                    if (pos < 0) break;
                    idx = pos;
                }

            }

            return idx;

        }

        // Debugs //
        internal string DebugString(int Index, bool NonCluster)
        {

            if (this.IsLeaf)
            {
                if (!NonCluster)
                    this._Elements[Index].ToString(',');
                Cell c = this._Elements[Index]._data.Last();
                return string.Format("{0} <{1},{2}>", this._Elements[Index].ToString(Key.Build(this.FieldCount - 2), ','), c.INT_A, c.INT_B);
            }

            Cell y = this._Elements[Index][this._RefColumn];
            return string.Format("Key [{0}] <{1},{2}>", this._Elements[Index].ToString(this._WeakKeyColumns), y.INT_A, y.INT_B);

        }

        // Statics //
        public static ClusterPage Mutate(Page Primitive, Key KeyColumns)
        {

            if (Primitive.PageType != Page.BTREE_PAGE_TYPE)
                throw new Exception("Cannot convert to B+Tree");

            if (Primitive is ClusterPage)
                return Primitive as ClusterPage;

            ClusterPage x = new ClusterPage(Primitive.PageSize, Primitive.PageID, Primitive.LastPageID, Primitive.NextPageID, Primitive.FieldCount, Primitive.UsedSpace, KeyColumns, Primitive.B0 == 1, (ClusterState)Primitive.B2);
            x._X0 = Primitive.X0;
            x._X1 = Primitive.X1;
            x._X2 = Primitive.X2;
            x._X3 = Primitive.X3;
            x._B0 = Primitive.B0;
            x._B1 = Primitive.B1;
            x._B2 = Primitive.B2;
            x._B3 = Primitive.B3;
            x._Elements = Primitive.Cache;

            return x;

        }

        public static Key BranchObjectiveClone(Key KeyColumns, bool Weak)
        {

            Key k = new Key();
            for (int i = 0; i < KeyColumns.Count; i++)
            {
                k.Add(i, KeyColumns.Affinity(i));
            }
            if (!Weak)
                k.Add(k.Count, KeyAffinity.Ascending);
            return k;

        }

        public static Record Composite(Record Key, int PageID)
        {
            Cell[] c = new Cell[Key.Count + 1];
            Array.Copy(Key.BaseArray, 0, c, 0, Key.Count);
            c[c.Length - 1] = new Cell(PageID, 0);
            return new Record(c);
        }

    }


}
