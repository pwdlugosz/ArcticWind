using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ArcticWind.Elements;
using ArcticWind.Tables;

namespace ArcticWind.Alpha.XTree
{

    ///// <summary>
    ///// Universal, Unique, Distinct
    ///// </summary>
    //public enum XTreeState : byte
    //{

    //    /// <summary>
    //    /// Allows duplicates
    //    /// </summary>
    //    Universal,

    //    /// <summary>
    //    /// Does not allow duplicates and errors out if duplicates are passed
    //    /// </summary>
    //    Unique,

    //    /// <summary>
    //    /// Does not allow duplicates, but doesnt error out; instead it ignores any duplicate records and does not insert them into the table
    //    /// </summary>
    //    Distinct

    //}

    ///// <summary>
    ///// FirstElement, LastElement, AnyElement
    ///// </summary>
    //public enum XTreeSearchType : int
    //{
    //    FirstElement = -1,
    //    LastElement = 1,
    //    AnyElement = 0
    //}

    ///// <summary>
    ///// Represents a branch page in an XTree holding key-pointer records
    ///// </summary>
    //public class XTreeBranchPage : Page
    //{

    //    /*
    //     * _B0: if 0 then the page is not the edge page; 1 then the page is the edge page; the edge page is a page with the max record on it
    //     * _B1: XTreeState
    //     * _B2: if 0 then the page has not fruit (points to branch pages) otherwise the page has fruit (points to leaf pages)
    //     * 
    //     */

    //    private const int XTREE_BRANCH_TYPE = 10;

    //    /// <summary>
    //    /// Represents a key that maps the value record to a key record; use this only when adding a record from the table into the tree
    //    /// </summary>
    //    private Key _ValueToKey;

    //    /// <summary>
    //    /// Represents a key that holds the just the key columns; use this when searching for a key within the tree
    //    /// </summary>
    //    private Key _Key;

    //    /// <summary>
    //    /// Represents a key that holds the key and the page ID; use this when searching for a key and page ID combination
    //    /// </summary>
    //    private Key _KeyPageID;

    //    /// <summary>
    //    /// Represents a key matcher that maps the value record to a key record; use this only when adding a record from the table into the tree
    //    /// </summary>
    //    private RecordMatcher _ValueToKeyMatcher;

    //    /// <summary>
    //    /// Represents a key matcher that holds the just the key columns; use this when searching for a key within the tree
    //    /// </summary>
    //    private RecordMatcher _KeyMatcher;

    //    /// <summary>
    //    /// Represents a key matcher that holds the key and the page ID; use this when searching for a key and page ID combination
    //    /// </summary>
    //    private RecordMatcher _KeyPageIDMatcher;

    //    /// <summary>
    //    /// Represents the index of the record that holds the page ID
    //    /// </summary>
    //    private int _PageIDRef = -1;

    //    // --------------------------- CTOR --------------------------- //
    //    public XTreeBranchPage(int PageSize, int PageID, int LastPageID, int NextPageID, Key ClusterKey, XTreeState State)
    //        : base(PageSize, PageID, LastPageID, NextPageID)
    //    {

    //        this._ValueToKey = ClusterKey;
    //        this._Key = Key.Build(ClusterKey.Count);
    //        this._KeyPageID = Key.Build(ClusterKey.Count + 1);
    //        this._ValueToKeyMatcher = new RecordMatcher(this._ValueToKey);
    //        this._KeyMatcher = new RecordMatcher(this._Key);
    //        this._KeyPageIDMatcher = new RecordMatcher(this._KeyPageID);
    //        this._PageIDRef = this._KeyPageID.Count - 1;
    //        this.State = State;
    //        this._Type = XTREE_BRANCH_TYPE;
    //        this.PointsToLeafs = true;

    //    }

    //    // --------------------------- Properties --------------------------- //
    //    /// <summary>
    //    /// True if the page is an edge page
    //    /// </summary>
    //    public bool IsEdge
    //    {
    //        get { return this._B0 == 1; }
    //        set { this._B0 = (byte)(value ? 1 : 0); }
    //    }

    //    /// <summary>
    //    /// Gets the tree's state
    //    /// </summary>
    //    public XTreeState State
    //    {
    //        get { return (XTreeState)this._B1; }
    //        set { this._B1 = (byte)value; }
    //    }

    //    /// <summary>
    //    /// If true then the page points to leaf pages, otherwise it points to data pages
    //    /// </summary>
    //    public bool PointsToLeafs
    //    {
    //        get { return this._B2 == 1; }
    //        set { this._B2 = (byte)(value ? 1 : 0); }
    //    }

    //    /// <summary>
    //    /// True if the page is the root page (has no parent)
    //    /// </summary>
    //    public bool IsRoot
    //    {
    //        get { return this.ParentPageID == -1; }
    //    }

    //    /// <summary>
    //    /// Overriden to limit page size for debugging
    //    /// </summary>
    //    public override bool CanAccept(Record Value)
    //    {
    //        return base.CanAccept(Value) || this.Count >= XTree.MAX_RECORD_COUNT;
    //    }

    //    // --------------------------- Methods --------------------------- //
    //    /// <summary>
    //    /// Inserts a key into the page
    //    /// </summary>
    //    /// <param name="Key"></param>
    //    /// <param name="PageID"></param>
    //    public void InsertKey(Record Key, int PageID)
    //    {

    //        if (this._KeyMatcher.Compare(Key, this._Elements.Last()) > 0 && !this.IsEdge)
    //            throw new Exception("Can't insert a record greater the max record unless this is the highest page");

    //        // InsertKey as usual //
    //        this.InsertKeyUnsafe(Key, PageID);

    //    }

    //    /// <summary>
    //    /// Inserts a key into the table without checking if it is within the bounds of the tree
    //    /// </summary>
    //    /// <param name="Key"></param>
    //    /// <param name="PageID"></param>
    //    public void InsertKeyUnsafe(Record Key, int PageID)
    //    {

    //        // Find the insertion point //
    //        Record k = Key + new Cell(PageID);
    //        int idx = this._Elements.BinarySearch(k, this._KeyPageIDMatcher);
    //        if (idx >= 0 && this.State == XTreeState.Unique)
    //        {
    //            throw new Cluster.DuplicateKeyException(string.Format("Key '{0}' exists on page '{1}' at position '{2}'", Key.ToString(), this.PageID, idx));
    //        }
    //        else if (idx >= 0 && this.State == XTreeState.Distinct)
    //        {
    //            return;
    //        }
    //        else if (idx < 0)
    //        {
    //            idx = ~idx;
    //        }

    //        // InsertKey as usual //
    //        this._Elements.Insert(idx, k);
    //        this._Version++;

    //    }

    //    /// <summary>
    //    /// Finds the page ID given a key and page ID
    //    /// </summary>
    //    /// <param name="Key"></param>
    //    /// <param name="SearchType"></param>
    //    /// <returns></returns>
    //    public int GetPageID(Record Key, int PageID)
    //    {

    //        Record z = Key + new Cell(PageID);

    //        int idx = this._Elements.BinarySearch(z, this._KeyPageIDMatcher);

    //        // If we didnt find the element, we dont have to search for multiple keys
    //        if (idx < 0)
    //        {
    //            idx = ~idx;
    //            return (int)this._Elements[idx][this._PageIDRef].LONG;
    //        }

    //        return (int)this._Elements[idx][this._PageIDRef].LONG;

    //    }

    //    /// <summary>
    //    /// Finds the page ID given a key
    //    /// </summary>
    //    /// <param name="Key"></param>
    //    /// <param name="SearchType"></param>
    //    /// <returns></returns>
    //    public int GetPageID(Record Key, XTreeSearchType SearchType)
    //    {

    //        int idx = this._Elements.BinarySearch(Key, this._KeyMatcher);

    //        // If we didnt find the element, we dont have to search for multiple keys
    //        if (idx < 0)
    //        {
    //            idx = ~idx;
    //            return (int)this._Elements[idx][this._PageIDRef].LONG;
    //        }

    //        // If we really don't care about anything, then return the index //
    //        if (SearchType == XTreeSearchType.AnyElement)
    //            return idx;

    //        // Search Upper //
    //        int pos = 0;
    //        if (SearchType == XTreeSearchType.FirstElement)
    //        {

    //            while (true)
    //            {
    //                pos = this._Elements.BinarySearch(0, idx, Key, this._KeyMatcher);
    //                if (pos < 0) break;
    //                idx = pos;
    //            }

    //        }
    //        else
    //        {

    //            pos = idx;
    //            while (true)
    //            {
    //                pos = this._Elements.BinarySearch(pos + 1, this.Count - idx - 1, Key, this._KeyMatcher);
    //                if (pos < 0) break;
    //                idx = pos;
    //            }

    //        }

    //        return (int)this._Elements[idx][this._PageIDRef].LONG;

    //    }
        
    //    /// <summary>
    //    /// Given an element, this finds the page ID of the page it belongs on
    //    /// </summary>
    //    /// <param name="Key"></param>
    //    /// <returns></returns>
    //    public int GetPageID(Record Key)
    //    {
    //        return this.GetPageID(Key, XTreeSearchType.AnyElement);
    //    }

    //    /// <summary>
    //    /// Gets all the page IDs
    //    /// </summary>
    //    /// <returns></returns>
    //    public List<int> AllPageIDs()
    //    {

    //        List<int> ids = new List<int>();
    //        foreach (Record r in this._Elements)
    //        {
    //            int i = (int)r[this._PageIDRef].LONG;
    //            ids.Add(i);
    //        }

    //        return ids;

    //    }

    //    /// <summary>
    //    /// Checks if a Key/PageID exists
    //    /// </summary>
    //    /// <param name="Key"></param>
    //    /// <param name="PageID"></param>
    //    /// <returns></returns>
    //    public bool KeyExists(Record Key, int PageID)
    //    {
    //        return this._Elements.BinarySearch(Key + new Cell(PageID), this._KeyPageIDMatcher) >= 0;
    //    }

    //    /// <summary>
    //    /// Checks if a key exists
    //    /// </summary>
    //    /// <param name="Key"></param>
    //    /// <returns></returns>
    //    public bool KeyExists(Record Key)
    //    {
    //        return this._Elements.BinarySearch(Key, this._KeyMatcher) >= 0;
    //    }

    //    /// <summary>
    //    /// Deletes a record
    //    /// </summary>
    //    /// <param name="Key"></param>
    //    public void Delete(Record Key)
    //    {

    //        // Key must be the entire data record //
    //        int idx = this._Elements.BinarySearch(Key, this._KeyMatcher);
    //        if (idx < 0)
    //        {
    //            throw new IndexOutOfRangeException("Key is not in this page");
    //        }

    //        this._Elements.RemoveAt(idx);
    //        this._Version++;

    //    }

    //    /// <summary>
    //    /// Deletes a record
    //    /// </summary>
    //    /// <param name="Key"></param>
    //    public void Delete(Record Key, int PageID)
    //    {

    //        // Key must be the entire data record //
    //        int idx = this._Elements.BinarySearch(Key + new Cell(PageID), this._KeyPageIDMatcher);
    //        if (idx < 0)
    //        {
    //            throw new IndexOutOfRangeException("Key is not in this page");
    //        }

    //        this._Elements.RemoveAt(idx);
    //        this._Version++;

    //    }

    //    /// <summary>
    //    /// Checks if the key is less than the terminal record
    //    /// </summary>
    //    /// <param name="Key"></param>
    //    /// <returns></returns>
    //    public bool LessThanTerminal(Record Key)
    //    {

    //        // We want this to be strictly less than the last element, not the case where it may be equal to
    //        return this._KeyMatcher.Compare(Key, this._Elements.Last()) < 0;

    //    }

    //    /// <summary>
    //    /// Gets the key portion of the terminal record
    //    /// </summary>
    //    public Record TerminalKeyOnly
    //    {
    //        get { return Record.Split(this._Elements.Last(), this._Key); }
    //    }

    //    // --------------------------- Splits --------------------------- //
    //    /// <summary>
    //    /// Splits a page into two pages
    //    /// </summary>
    //    /// <param name="Position">The position to cut the records at; for example if the page has 10 record, and the split position is 5, records 5,6,7,8 and 9 will be moved to a new page</param>
    //    /// <param name="PageID">The id of the new page</param>
    //    /// <returns></returns>
    //    public XTreeBranchPage Split(int PageID, int Position)
    //    {

    //        // Check the position is valid //
    //        if (Position < 0 || Position >= this.Count)
    //            throw new Exception(string.Format("Position {0} is invalid", Position));

    //        // Create the new page //
    //        XTreeBranchPage x = new XTreeBranchPage(this.PageSize, PageID, this.PageID, this.NextPageID, this._ValueToKey, this.State);
            
    //        // Split up the records //
    //        x._Elements = this._Elements.GetRange(Position, this.Count - Position);
    //        x._Elements.RemoveRange(Position, this.Count - Position);

    //        // Fix up the edge piece //
    //        x.IsEdge = this.IsEdge;
    //        this.IsEdge = false;

    //        // Fix up the base instance linked list nodes //
    //        this.NextPageID = x.PageID;

    //        // Fix up the parent page ID //
    //        x.ParentPageID = this.ParentPageID;

    //        return x;

    //    }

    //    /// <summary>
    //    /// Splits a page in half
    //    /// </summary>
    //    /// <param name="PageID"></param>
    //    /// <returns></returns>
    //    public XTreeBranchPage Split(int PageID)
    //    {
    //        return this.Split(PageID, this.Count / 2);
    //    }
        
    //    // --------------------------- Debugging --------------------------- //
    //    public Record GetKeyRecord(int Index)
    //    {
    //        return this._Elements[Index] * this._ValueToKey;
    //    }

    //    public int GetPageID(int Index)
    //    {
    //        return (int)this._Elements[Index].BaseArray.Last().LONG;
    //    }

    //    // --------------------------- Mutates --------------------------- //
    //    /// <summary>
    //    /// Mutates an abstract page into an XTreeBranchPage
    //    /// </summary>
    //    /// <param name="Primitive"></param>
    //    /// <param name="ValueToKey"></param>
    //    /// <returns></returns>
    //    public static XTreeBranchPage Mutate(Page Primitive, Key ValueToKey)
    //    {

    //        if (Primitive.PageType != XTREE_BRANCH_TYPE)
    //            throw new Exception("Page is not an XTreeBranch");

    //        if (Primitive is XTreeBranchPage)
    //            return Primitive as XTreeBranchPage;

    //        XTreeBranchPage x = new XTreeBranchPage(Primitive.PageSize, Primitive.PageID, Primitive.LastPageID, Primitive.NextPageID, ValueToKey, (XTreeState)Primitive.B1);
    //        x._X0 = Primitive.X0;
    //        x._X1 = Primitive.X1;
    //        x._X2 = Primitive.X2;
    //        x._X3 = Primitive.X3;
    //        x._B0 = Primitive.B0;
    //        x._B1 = Primitive.B1;
    //        x._B2 = Primitive.B2;
    //        x._B3 = Primitive.B3;
    //        x._Elements = Primitive.Cache;

    //        return x;

    //    }

    //}

    ///// <summary>
    ///// Represents a leaf page in an XTree holding data records
    ///// </summary>
    //public class XTreeLeafPage : Page
    //{

    //    /*
    //     * 
    //     * _B1: XTreeState
    //     * 
    //     */

    //    private const int XTREE_LEAF_TYPE = 11;

    //    /// <summary>
    //    /// Represents a key that maps the value record to a key record; use this only when adding a record from the table into the tree
    //    /// </summary>
    //    private Key _ValueToKey;

    //    /// <summary>
    //    /// Represents a key matcher that maps the value record to a key record; use this only when adding a record from the table into the tree
    //    /// </summary>
    //    private RecordMatcher _ValueToKeyMatcher;

    //    // --------------------------- CTOR --------------------------- //
    //    public XTreeLeafPage(int PageSize, int PageID, int LastPageID, int NextPageID, Key ClusterKey, XTreeState State)
    //        : base(PageSize, PageID, LastPageID, NextPageID)
    //    {

    //        this._ValueToKey = ClusterKey;
    //        this._ValueToKeyMatcher = new RecordMatcher(this._ValueToKey);
    //        this.State = State;
    //        this._Type = XTREE_LEAF_TYPE;
            
    //    }

    //    // --------------------------- Properties --------------------------- //
    //    /// <summary>
    //    /// Gets the tree's state
    //    /// </summary>
    //    public XTreeState State
    //    {
    //        get { return (XTreeState)this._B1; }
    //        set { this._B1 = (byte)value; }
    //    }

    //    /// <summary>
    //    /// Overriden to limit page size for debugging
    //    /// </summary>
    //    public override bool CanAccept(Record Value)
    //    {
    //        return base.CanAccept(Value) || this.Count >= XTree.MAX_RECORD_COUNT;
    //    }

    //    // --------------------------- Methods --------------------------- //
    //    /// <summary>
    //    /// Inserts an element into the page in order
    //    /// </summary>
    //    /// <param name="Key"></param>
    //    public override void Insert(Record Element)
    //    {

    //        if (!this.CanAccept(Element))
    //            throw new Exception("Page is full");

    //        int idx = this._Elements.BinarySearch(Element, this._ValueToKeyMatcher);

    //        if (idx < 0) idx = ~idx;

    //        this._Elements.Insert(idx, Element);

    //    }

    //    /// <summary>
    //    /// Splits the page in two
    //    /// </summary>
    //    /// <param name="PageID">The ID of the new page</param>
    //    /// <param name="Position">The position to pivot on</param>
    //    /// <returns>A new page filled with records above the position</returns>
    //    public XTreeLeafPage Split(int PageID, int Position)
    //    {
    //        // Check the position is valid //
    //        if (Position < 0 || Position >= this.Count)
    //            throw new Exception(string.Format("Position {0} is invalid", Position));

    //        // Build the page //
    //        XTreeLeafPage x = new XTreeLeafPage(this.PageSize, PageID, this.PageID, this.NextPageID, this._ValueToKey, this.State);
    //        x.ParentPageID = this.ParentPageID;
    //        this.NextPageID = PageID;

    //        // Split up the records //
    //        x._Elements = this._Elements.GetRange(Position, this.Count - Position);
    //        this._Elements.RemoveRange(Position, this.Count - Position);

    //        return x;

    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="PageID"></param>
    //    /// <returns></returns>
    //    public XTreeLeafPage Split(int PageID)
    //    {
    //        return this.Split(PageID, this.Count / 2);
    //    }

    //    /// <summary>
    //    /// Finds a record on the page
    //    /// </summary>
    //    /// <param name="Key"></param>
    //    /// <param name="SearchType"></param>
    //    /// <returns></returns>
    //    public int Seek(Record Key, XTreeSearchType SearchType)
    //    {

    //        RecordMatcher m = new RecordMatcher(ArcticWind.Elements.Key.Build(this._ValueToKey.Count), this._ValueToKey);

    //        int idx = this._Elements.BinarySearch(Key, m);

    //        // If we didnt find the element, we dont have to search for multiple keys
    //        if (idx < 0)
    //        {
    //            idx = ~idx;
    //            return idx;
    //        }

    //        // If we really don't care about anything, then return the index //
    //        if (SearchType == XTreeSearchType.AnyElement)
    //            return idx;

    //        // Search Upper //
    //        int pos = 0;
    //        if (SearchType == XTreeSearchType.FirstElement)
    //        {

    //            while (true)
    //            {
    //                pos = this._Elements.BinarySearch(0, idx, Key, this._ValueToKeyMatcher);
    //                if (pos < 0) break;
    //                idx = pos;
    //            }

    //        }
    //        else
    //        {

    //            pos = idx;
    //            while (true)
    //            {
    //                pos = this._Elements.BinarySearch(pos + 1, this.Count - idx - 1, Key, this._ValueToKeyMatcher);
    //                if (pos < 0) break;
    //                idx = pos;
    //            }

    //        }

    //        return idx;

    //    }

    //    // --------------------------- Mutates --------------------------- //
    //    /// <summary>
    //    /// Mutates an abstract page into an XTreeBranchPage
    //    /// </summary>
    //    /// <param name="Primitive"></param>
    //    /// <param name="ValueToKey"></param>
    //    /// <returns></returns>
    //    public static XTreeLeafPage Mutate(Page Primitive, Key ValueToKey)
    //    {

    //        if (Primitive.PageType != XTREE_LEAF_TYPE)
    //            throw new Exception("Page is not an XTreeBranch");

    //        if (Primitive is XTreeLeafPage)
    //            return Primitive as XTreeLeafPage;

    //        XTreeLeafPage x = new XTreeLeafPage(Primitive.PageSize, Primitive.PageID, Primitive.LastPageID, Primitive.NextPageID, ValueToKey, (XTreeState)Primitive.B1);
    //        x._X0 = Primitive.X0;
    //        x._X1 = Primitive.X1;
    //        x._X2 = Primitive.X2;
    //        x._X3 = Primitive.X3;
    //        x._B0 = Primitive.B0;
    //        x._B1 = Primitive.B1;
    //        x._B2 = Primitive.B2;
    //        x._B3 = Primitive.B3;
    //        x._Elements = Primitive.Cache;

    //        return x;

    //    }


    //}

    ///// <summary>
    ///// Holds a map of the branch pages
    ///// </summary>
    //public class XTree
    //{

    //    public const int MAX_RECORD_COUNT = 8;

    //    protected Table _Storage;
    //    protected XTreeBranchPage _Root;
    //    protected Key _IndexColumns;
    //    protected Record _MaxRecord;
    //    protected IElementHeader _Header;
    //    protected XTreeState _State;

    //    public XTree(Table Storage, Schema ParentSchema, Key IndexColumns, XTreeBranchPage Root, IElementHeader Header, XTreeState State)
    //    {

    //        this._Storage = Storage;
    //        this._Header = Header;
    //        this._IndexColumns = IndexColumns;
    //        this._State = State;
    //        this._MaxRecord = Schema.Split(ParentSchema, this._IndexColumns).MaxRecord;
    //        this._Root = Root;
    //    }

    //    public XTree(Table Storage, Schema ParentSchema, Key IndexColumns, IElementHeader Header, XTreeState State)
    //        :this(Storage, ParentSchema, IndexColumns, null, Header, State)
    //    {

    //        Schema s = Schema.Split(this._Storage.Columns, this._IndexColumns);
    //        s.Add("@IDX", CellAffinity.LONG);

    //        this._Root = new XTreeBranchPage(this._Storage.PageSize, this._Storage.GenerateNewPageID, -1, -1, this._IndexColumns, State);
    //        this._Root.ParentPageID = -1;
    //        this._Root.IsEdge = true;
    //        this._Root.PointsToLeafs = true; // False because it starts out pointing to leaf pages
    //        this._Header.RootPageID = this._Root.PageID;
    //        this._Storage.SetPage(this._Root);
    //        this._Storage.Header.PageCount++;
    //        this._Root.Version++;

    //        // Build a leaf page //
    //        XTreeLeafPage xl = new XTreeLeafPage(this._Storage.PageSize, this._Storage.GenerateNewPageID, -1, -1, this._IndexColumns, this._State);
    //        xl.ParentPageID = this._Root.PageID;
            
    //        // Add the page to the table //
    //        this._Storage.SetPage(xl);
    //        this._Header.PageCount++;

    //        // Add the max record with this page id to the root //
    //        this._Root.InsertKeyUnsafe(this._MaxRecord, xl.PageID);

    //    }

    //    // -------------------------------------------- Properties -------------------------------------------- //
    //    /// <summary>
    //    /// The table where all the pages are stored
    //    /// </summary>
    //    public Table Storage
    //    {
    //        get { return this._Storage; }
    //    }

    //    /// <summary>
    //    /// The tree's root page
    //    /// </summary>
    //    public XTreeBranchPage Root
    //    {
    //        get { return this._Root; }
    //    }

    //    /// <summary>
    //    /// The columns of the original table that are indexed
    //    /// </summary>
    //    public Key IndexColumns
    //    {
    //        get { return this._IndexColumns; }
    //    }

    //    /// <summary>
    //    /// The xtree header
    //    /// </summary>
    //    public IElementHeader Header
    //    {
    //        get { return this._Header; }
    //    }

    //    /// <summary>
    //    /// The state of the xtree
    //    /// </summary>
    //    public XTreeState State
    //    {
    //        get { return this._State; }
    //    }

    //    // -------------------------------------------- Searching -------------------------------------------- //
    //    /// <summary>
    //    /// Returns the page that the key belongs on
    //    /// </summary>
    //    /// <param name="Key"></param>
    //    /// <param name="SearchType"></param>
    //    /// <returns></returns>
    //    public XTreeBranchPage Search(Record Key, int PageID)
    //    {

    //        XTreeBranchPage x = this._Root;

    //        while (!x.PointsToLeafs)
    //        {
    //            int id = x.GetPageID(Key, PageID);
    //            x = this.GetBranchPage(id);
    //        }

    //        return x;

    //    }

    //    /// <summary>
    //    /// Gets the leaf page a given key belongs on
    //    /// </summary>
    //    /// <param name="Key"></param>
    //    /// <returns></returns>
    //    public XTreeLeafPage Search(Record Key)
    //    {

    //        XTreeBranchPage x = this._Root;

    //        while (!x.PointsToLeafs)
    //        {
    //            int id = x.GetPageID(Key);
    //            x = this.GetBranchPage(id);
    //        }

    //        int PageID = x.GetPageID(Key, XTreeSearchType.AnyElement);

    //        return GetLeafPage(PageID);

    //    }

    //    /// <summary>
    //    /// Gets the first page in the tree
    //    /// </summary>
    //    /// <returns></returns>
    //    public XTreeLeafPage SeekOrigin()
    //    {

    //        XTreeBranchPage x = this._Root;

    //        while (!x.PointsToLeafs)
    //        {
    //            int id = x.GetPageID(0);
    //            x = this.GetBranchPage(id);
    //        }

    //        int PageID = x.GetPageID(0);

    //        return GetLeafPage(PageID);

    //    }

    //    /// <summary>
    //    /// Finds a record key given a key
    //    /// </summary>
    //    /// <param name="Key"></param>
    //    /// <param name="SearchType"></param>
    //    /// <returns></returns>
    //    public RecordKey Seek(Record Key, XTreeSearchType SearchType)
    //    {
    //        XTreeBranchPage x = this._Root;

    //        while (!x.PointsToLeafs)
    //        {
    //            int id = x.GetPageID(Key);
    //            x = this.GetBranchPage(id);
    //        }

    //        int PageID = x.GetPageID(Key, SearchType);
    //        XTreeLeafPage y = this.GetLeafPage(PageID);

    //        int idx = y.Seek(Key, SearchType);

    //        return new RecordKey(y.PageID, idx);
    //    }

    //    // -------------------------------------------- Add / Update -------------------------------------------- //
    //    /// <summary>
    //    /// Inserts a record into the tree
    //    /// </summary>
    //    /// <param name="Key"></param>
    //    public void Insert(Record Element)
    //    {

    //        Record k = Element * this._IndexColumns;

    //        // Otherwise, find the page this belongs on
    //        XTreeLeafPage x = this.Search(k);

    //        // If not full, then insert
    //        if (x.CanAccept(Element))
    //        {
    //            x.Insert(Element);
    //            this._Header.RecordCount++;
    //        }
    //        // Otherwise, split
    //        else
    //        {

    //            XTreeLeafPage y = this.SplitLeaf(x);

    //            if (Record.Compare(k, x.TerminalRecord * this._IndexColumns) <= 0)
    //            {
    //                x.Insert(Element);
    //            }
    //            else
    //            {
    //                y.Insert(Element);
    //            }
    //            this._Header.RecordCount++;

    //        }

    //    }
        
    //    /// <summary>
    //    /// Adds a key and page ID to the map
    //    /// </summary>
    //    /// <param name="Key"></param>
    //    /// <param name="PageID"></param>
    //    public void AddKey(Record Key, int PageID)
    //    {

    //        // Find the page this key belongs on //
    //        XTreeBranchPage OriginalPage = this.Search(Key, PageID);

    //        // Check if the page is full //
    //        if (!OriginalPage.IsFull)
    //        {
    //            OriginalPage.InsertKey(Key, PageID);
    //        }
    //        else if (OriginalPage.IsRoot)
    //        {

    //            // Create a new root page //
    //            this._Root = this.NewRoot();
                
    //            // Split the page //
    //            XTreeBranchPage NewPage = OriginalPage.Split(this._Storage.GenerateNewPageID);
    //            this._Header.RecordCount++;
    //            this._Header.PageCount++;
    //            this._Storage.SetPage(NewPage);
                
    //            // Update NewPage meta data //
    //            NewPage.ParentPageID = this._Root.PageID;
    //            NewPage.IsEdge = true;

    //            // Update original page meta data //
    //            OriginalPage.ParentPageID = this._Root.PageID;
    //            OriginalPage.IsEdge = false;

    //            // Insert the page keys //
    //            this._Root.InsertKey(OriginalPage.TerminalKeyOnly, OriginalPage.PageID);
    //            this._Root.InsertKey(NewPage.TerminalKeyOnly, NewPage.PageID);

    //            // Update the parent page IDs //
    //            this.UpdateAllParentPageIDs(NewPage);

    //        }
    //        else
    //        {

    //            // Get the parent page //
    //            XTreeBranchPage ParentPage = this.GetBranchPage(OriginalPage.ParentPageID);
                
    //            // Split the page //
    //            XTreeBranchPage NewPage = OriginalPage.Split(this._Storage.GenerateNewPageID);
    //            this._Header.RecordCount++;
    //            this._Header.PageCount++;
    //            this._Storage.SetPage(NewPage);

    //            // Remove the pointer to OriginalPage from the map //
    //            OriginalPage.Delete(NewPage.TerminalKeyOnly, OriginalPage.PageID);

    //            // Add the new node's terminal record //
    //            // A couple of notes on this:
    //            //      -- We know because we just deleted the old terminal record that the parent page has space
    //            //      -- We also know that we deleted the edge record
    //            //      -- If we try to insert like normal, it will crash because we'd be attempting to add a record above the edge record
    //            //      -- Therefore we have to add in the record unsafely
    //            NewPage.InsertKeyUnsafe(NewPage.TerminalKeyOnly, NewPage.PageID);

    //            // Add the old node's terminal record and page ID //
    //            this.AddKey(OriginalPage.TerminalKeyOnly, OriginalPage.PageID);
                
    //            // Update the meta data //
    //            NewPage.ParentPageID = OriginalPage.ParentPageID;
    //            NewPage.IsEdge = OriginalPage.IsEdge;
    //            OriginalPage.IsEdge = false;
    //            NewPage.PointsToLeafs = OriginalPage.PointsToLeafs;

    //            // Update the parent page IDs //
    //            this.UpdateAllParentPageIDs(NewPage);

    //        }

    //    }

    //    /// <summary>
    //    /// Splits a leaf page
    //    /// </summary>
    //    /// <param name="OriginalNode"></param>
    //    /// <returns></returns>
    //    public XTreeLeafPage SplitLeaf(XTreeLeafPage OriginalNode)
    //    {

    //        // Create the new leaf //
    //        XTreeLeafPage NewLeaf = OriginalNode.Split(this._Storage.GenerateNewPageID);
    //        this._Storage.SetPage(NewLeaf);
    //        this._Header.PageCount++;
    //        Console.WriteLine("OriginalNode: {0} \n{1}", OriginalNode.PageID, OriginalNode.ToString());
    //        Console.WriteLine("NewLeaf: {0} \n{1}", NewLeaf.PageID, NewLeaf.ToString());
            
    //        // Need to update the parent page record //
    //        XTreeBranchPage Parent = this.GetBranchPage(OriginalNode.ParentPageID);

    //        // Need to remove the old record //
    //        // Don't delete if (1) the parent is the root and (2) if the root only has one element //
    //        if (!(Parent.PageID == this._Root.PageID && Parent.Count == 1))
    //        {
    //            Parent.Delete(NewLeaf.TerminalRecord * this._IndexColumns, OriginalNode.PageID);
    //        }

    //        // Add in the new record //
    //        Parent.InsertKeyUnsafe(NewLeaf.TerminalRecord * this._IndexColumns, NewLeaf.PageID);
            
    //        // Add back the old record //
    //        this.AddKey(OriginalNode.TerminalRecord * this._IndexColumns, OriginalNode.PageID);
            
    //        return NewLeaf;

    //    }

    //    /// <summary>
    //    /// Updates all parent page IDs
    //    /// </summary>
    //    /// <param name="PageIDs"></param>
    //    /// <param name="NewParentPageID"></param>
    //    public void UpdateAllParentPageIDs(XTreeBranchPage XPage)
    //    {

    //        foreach (int ID in XPage.AllPageIDs())
    //        {
    //            Page p = this._Storage.GetPage(ID);
    //            p.ParentPageID = XPage.ParentPageID;
    //            this._Storage.SetPage(p);
    //        }

    //    }

    //    // -------------------------------------------- Get Pages -------------------------------------------- //
    //    /// <summary>
    //    /// Pulls in a page as a branch
    //    /// </summary>
    //    /// <param name="ID"></param>
    //    /// <returns></returns>
    //    public XTreeBranchPage GetBranchPage(int ID)
    //    {

    //        Page p = this._Storage.GetPage(ID);
    //        XTreeBranchPage x = XTreeBranchPage.Mutate(p, this._IndexColumns);
    //        return x;

    //    }

    //    /// <summary>
    //    /// Gets the given page as a leaf
    //    /// </summary>
    //    /// <param name="ID"></param>
    //    /// <returns></returns>
    //    public XTreeLeafPage GetLeafPage(int ID)
    //    {
    //        Page p = this._Storage.GetPage(ID);
    //        XTreeLeafPage x = XTreeLeafPage.Mutate(p, this._IndexColumns);
    //        return x;
    //    }

    //    /// <summary>
    //    /// Generates a new root page as if it were a branch node
    //    /// </summary>
    //    /// <returns></returns>
    //    private XTreeBranchPage NewRoot()
    //    {

    //        Schema s = Schema.Split(this._Storage.Columns, this._IndexColumns);
    //        s.Add("@IDX", CellAffinity.LONG);

    //        XTreeBranchPage NewRoot = new XTreeBranchPage(this._Storage.PageSize, this._Storage.GenerateNewPageID, -1, -1, this._IndexColumns, this._State);
    //        NewRoot.ParentPageID = -1;
    //        NewRoot.IsEdge = true;
    //        NewRoot.PointsToLeafs = false;
    //        this._Header.RootPageID = NewRoot.PageID;
    //        NewRoot.Version++;
    //        this._Storage.SetPage(NewRoot);
    //        this._Header.PageCount++;

    //        return NewRoot;

    //    }

    //    // -------------------------------------------- Debugging -------------------------------------------- //
    //    public void DumpTree(string Path)
    //    {

    //        using (StreamWriter sw = new StreamWriter(Path))
    //        {
    //            this.DumpTree(this._Root, sw, 0);
    //        }

    //    }

    //    private void DumpTree(XTreeBranchPage Branch, StreamWriter Writer, int Level)
    //    {

    //        List<int> PageIDs = new List<int>();
    //        string Padding = new string(' ', Level * 3);
    //        Writer.WriteLine("{0}-- Branch Page ID: {1}", Padding, Branch.ParentPageID);
            
    //        // Write the page meta data //
    //        for (int i = 0; i < Branch.Count; i++)
    //        {
    //            Writer.WriteLine("{0}  Key<{1}> Page ID<{2}>", Padding, Branch.GetKeyRecord(i), Branch.GetPageID(i));
    //            PageIDs.Add(i);
    //        }

    //        if (Branch.PointsToLeafs)
    //            return;

    //        // Call all child pages //
    //        foreach (int i in PageIDs)
    //        {
    //            XTreeBranchPage x = this.GetBranchPage(i);
    //            this.DumpTree(x, Writer, Level + 1);
    //        }


    //    }

    //    public void DumpData(string Path)
    //    {

    //        if (this._Header.RecordCount == 0)
    //            return;

    //        XTreeLeafPage x = this.SeekOrigin();
    //        int PageID = x.PageID;

    //        using (StreamWriter sw = new StreamWriter(Path))
    //        {

    //            while (PageID != -1)
    //            {
    //                this.DumpData(x, sw);
    //                PageID = x.NextPageID;
    //                x = (PageID == -1 ? null : this.GetLeafPage(PageID));
    //            }

    //        }

    //    }

    //    private void DumpData(XTreeLeafPage Leaf, StreamWriter Writer)
    //    {
    //        foreach (Record r in Leaf.Elements)
    //        {
    //            Writer.WriteLine(r);
    //        }
    //    }

    //}

    ///// <summary>
    ///// Creates a table sorted usina a b+tree that can be saved to disk
    ///// </summary>
    //public class XTable : Table
    //{

    //    protected XTree _Cluster;

    //    /// <summary>
    //    /// This method should be used for creating a table object from an existing table on disk
    //    /// </summary>
    //    /// <param name="Host"></param>
    //    /// <param name="Header"></param>
    //    /// <param name="ClusterKey"></param>
    //    public XTable(Host Host, TableHeader Header)
    //        : base(Host, Header)
    //    {

    //        if (Header.RootPageID == -1)
    //            throw new ArgumentException("The root page ID cannot be null");

    //        // Get the sort key //
    //        Key k = Header.ClusterKey;

    //        // Get the root page ID //
    //        XTreeBranchPage root = XTreeBranchPage.Mutate(this.GetPage(Header.RootPageID), k);

    //        // Cluster //
    //        this._Cluster = new XTree(this, this.Columns, k, root, this.Header, XTreeState.Universal);

    //        this._TableType = "CLUSTER_SCRIBE";

    //    }

    //    /// <summary>
    //    /// This method should be used for creating a brand new table object
    //    /// </summary>
    //    /// <param name="Host"></param>
    //    /// <param name="Alias"></param>
    //    /// <param name="Dir"></param>
    //    /// <param name="Columns"></param>
    //    /// <param name="PageSize"></param>
    //    /// <param name="ClusterKey"></param>
    //    public XTable(Host Host, string Name, string Dir, Schema Columns, Key ClusterColumns, XTreeState State, int PageSize)
    //        : base(Host, Name, Dir, Columns, PageSize)
    //    {

    //        this._Cluster = new XTree(this, Columns, ClusterColumns, this.Header, State);
    //        this._TableType = "CLUSTER_SCRIBE";
    //        this._Header.ClusterKey = ClusterColumns;

    //    }

    //    /// <summary>
    //    /// This method should be used for creating a brand new table object
    //    /// </summary>
    //    /// <param name="Host"></param>
    //    /// <param name="Alias"></param>
    //    /// <param name="Dir"></param>
    //    /// <param name="Columns"></param>
    //    /// <param name="ClusterColumns"></param>
    //    public XTable(Host Host, string Name, string Dir, Schema Columns, Key ClusterColumns, XTreeState State)
    //        : this(Host, Name, Dir, Columns, ClusterColumns, State, Page.DEFAULT_SIZE)
    //    {
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="Host"></param>
    //    /// <param name="Name"></param>
    //    /// <param name="Dir"></param>
    //    /// <param name="Columns"></param>
    //    /// <param name="ClusterColumns"></param>
    //    public XTable(Host Host, string Name, string Dir, Schema Columns, Key ClusterColumns)
    //        : this(Host, Name, Dir, Columns, ClusterColumns, XTreeState.Universal, Page.DEFAULT_SIZE)
    //    {
    //    }

    //    /// <summary>
    //    /// Inner B+Tree
    //    /// </summary>
    //    public XTree BaseTree
    //    {
    //        get { return this._Cluster; }
    //    }

    //    /// <summary>
    //    /// Appends a record to a table
    //    /// </summary>
    //    /// <param name="Value"></param>
    //    public override void Insert(Record Value)
    //    {

    //        // Step the version //
    //        this._Version++;

    //        this._Cluster.Insert(Value);

    //    }

    //    /// <summary>
    //    /// Opens a record reader that focuses on a single key
    //    /// </summary>
    //    /// <param name="Key"></param>
    //    /// <returns></returns>
    //    public override RecordReader OpenReader(Record Key)
    //    {

    //        if (Key.Count != this._Cluster.IndexColumns.Count)
    //            return this.OpenReader();
    //        RecordKey l = this._Cluster.Seek(Key, XTreeSearchType.FirstElement);
    //        RecordKey u = this._Cluster.Seek(Key, XTreeSearchType.LastElement);
    //        return new RecordReaderBase(this, l, u);

    //    }

    //    /// <summary>
    //    /// Opens a record reader to focus on records between A and B (inclusive)
    //    /// </summary>
    //    /// <param name="LKey"></param>
    //    /// <param name="UKey"></param>
    //    /// <returns></returns>
    //    public override RecordReader OpenReader(Record LKey, Record UKey)
    //    {

    //        if (LKey.Count != UKey.Count || LKey.Count != this._Cluster.IndexColumns.Count)
    //            return this.OpenReader();
    //        RecordKey lk = this._Cluster.Seek(LKey, XTreeSearchType.FirstElement);
    //        RecordKey uk = this._Cluster.Seek(UKey, XTreeSearchType.LastElement);
    //        return new RecordReaderBase(this, lk, uk);

    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="Alias"></param>
    //    /// <param name="IndexColumns"></param>
    //    public override void CreateIndex(string Name, Key IndexColumns)
    //    {
    //        throw new Exception("Cannot create indexes on clustered tables");
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="Name"></param>
    //    /// <returns></returns>
    //    public override Index GetIndex(string Name)
    //    {
    //        throw new Exception("Cannot request indexes on clustered tables");
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="IndexColumns"></param>
    //    /// <returns></returns>
    //    public override Index GetIndex(Key IndexColumns)
    //    {
    //        throw new Exception("Cannot request indexes on clustered tables");
    //        //if (Elements.Key.EqualsStrong(IndexColumns, this._Header.ClusterKey))
    //        //    return new DerivedIndex(this);
    //        //return null;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public override void PreSerialize()
    //    {
    //        this.SetPage(this._Cluster.Root);
    //    }

    //    // Methods not implemented //
    //    /// <summary>
    //    /// Splits a table into N sub tables
    //    /// </summary>
    //    /// <param name="PartitionIndex"></param>
    //    /// <param name="PartitionCount"></param>
    //    /// <returns></returns>
    //    public override Table Partition(int PartitionIndex, int PartitionCount)
    //    {
    //        throw new NotImplementedException();
    //    }

    //}

}
