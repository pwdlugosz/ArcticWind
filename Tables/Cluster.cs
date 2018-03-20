using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ArcticWind.Elements;

namespace ArcticWind.Tables
{


    /// <summary>
    /// Represents a B+ Tree
    /// </summary>
    public class Cluster
    {

        /*
         * Note:
         *      Key = data record
         *      Key = just the key piece of element
         *      Value = just the value piece of element
         * 
         */


        protected Table _Storage;
        protected ClusterPage _Root;
        protected Key _IndexColumns;
        protected Record _MaxRecord;
        protected IElementHeader _Header;
        
        /// <summary>
        /// This creates a B+Tree given a root page from an already existing B+Tree
        /// </summary>
        /// <param name="Storage"></param>
        /// <param name="IndexColumns"></param>
        public Cluster(Table Storage, Schema ParentSchema, Key IndexColumns, ClusterPage Root, IElementHeader Header, ClusterState State)
        {

            this._Storage = Storage;
            this._Header = Header;
            this._IndexColumns = IndexColumns;
            this.State = State;
            if (Root == null)
            {
                this._Root = this.NewRootAsLeaf();
                this._Storage.SetPage(this._Root);
                this._Storage.Header.PageCount++;
            }
            else
            {
                this._Root = Root;
            }
            this._MaxRecord = Schema.Split(ParentSchema, this._IndexColumns).MaxRecord;
            this.OriginBTreePageID = this.SeekOriginPageID();
            this.TerminalBTreePageID = this.SeekTerminalPageID();
            
        }

        /// <summary>
        /// Gets the table that stores the b+tree
        /// </summary>
        public Table Storage
        {
            get { return this._Storage; }
        }

        /// <summary>
        /// Gets the root page
        /// </summary>
        public ClusterPage Root
        {
            get { return this._Root; }
        }

        /// <summary>
        /// Gets a key describing the index columns
        /// </summary>
        public Key IndexColumns
        {
            get { return this._IndexColumns; }
        }

        /// <summary>
        /// True if unigue, false otherwise
        /// </summary>
        public virtual ClusterState State
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the first data page ID
        /// </summary>
        public virtual int OriginBTreePageID
        {
            get { return this._Header.OriginPageID; }
            set { this._Header.OriginPageID = value; }
        }

        /// <summary>
        /// Gets the last data page ID
        /// </summary>
        public virtual int TerminalBTreePageID
        {
            get { return this._Header.TerminalPageID; }
            set { this._Header.TerminalPageID = value; }
        }

        /// <summary>
        /// Gets the root page id
        /// </summary>
        public virtual int RootPageID
        {
            get { return this._Root.PageID; }
        }

        // Seek Methods //
        /// <summary>
        /// Finds the leaf page this record belongs on
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public virtual ClusterPage SeekPage(Record Element)
        {

            // If the root page is a leaf node //
            if (this._Root.IsLeaf)
                return this._Root;

            // Otherwise, starting at root, find the page //
            Record Key = Record.Split(Element, this._IndexColumns);
            ClusterPage x = this._Root;
            while (true)
            {
                
                int PageID = x.GetPageID(Key);
                x = this.GetPage(PageID);
                if (x.IsLeaf)
                    return x;

            }

            // ## For debuggin ##
            //ClusterPage OriginalPage = this._Root;
            //while (true)
            //{
            //    int PageID = OriginalPage.GetPageID(Key);
            //    ClusterPage NewPage = this.GetBranchPage(PageID);
            //    if (NewPage.IsLeaf)
            //    {
            //        return NewPage;
            //    }
            //    else
            //    {
            //        OriginalPage = NewPage;
            //    }
            //}

        }

        /// <summary>
        /// Finds the page with the FIRST instance of the key
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public virtual ClusterPage SeekFirstPage(Record Key)
        {

            // If the root page is a leaf node //
            if (this._Root.IsLeaf)
                return this._Root;

            // Otherwise, starting at root, find the page //
            ClusterPage x = this._Root;
            while (true)
            {

                int PageID = x.SearchBranch(Key, ClusterPage.BPlusTreeSearchType.FirstElement, false);
                x = this.GetPage(PageID);
                if (x.IsLeaf)
                    return x;

            }

        }

        /// <summary>
        /// Finds the page with the LAST instance of the key on it
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public virtual ClusterPage SeekLastPage(Record Key)
        {

            // If the root page is a leaf node //
            if (this._Root.IsLeaf)
                return this._Root;

            // Otherwise, starting at root, find the page //
            ClusterPage x = this._Root;
            while (true)
            {
                int PageID = x.SearchBranch(Key, ClusterPage.BPlusTreeSearchType.LastElement, false);
                x = this.GetPage(PageID);
                if (x.IsLeaf)
                    return x;

            }

        }

        /// <summary>
        /// Finds the first location of a record
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public virtual RecordKey SeekFirst(Record Key, bool Exact)
        {

            ClusterPage x = this.SeekFirstPage(Key);
            int location = x.SearchLeaf(Key, ClusterPage.BPlusTreeSearchType.FirstElement, Exact);
            if (location < 0)
                return RecordKey.RecordNotFound;
            while (location == 0 && x.LastPageID != -1)
            {
                x = this.GetPage(x.LastPageID);
                location = x.SearchLeaf(Key, ClusterPage.BPlusTreeSearchType.FirstElement, false);
            }
            return new RecordKey(x.PageID, location);

        }

        /// <summary>
        /// Finds the last location of a record
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public virtual RecordKey SeekLast(Record Key, bool Exact)
        {

            ClusterPage x = this.SeekLastPage(Key);
            int location = x.SearchLeaf(Key, ClusterPage.BPlusTreeSearchType.LastElement, Exact);
            if (location < 0)
                return RecordKey.RecordNotFound;
            while (location == (x.Count - 1) && x.NextPageID != -1)
            {
                x = this.GetPage(x.NextPageID);
                location = x.SearchLeaf(Key, ClusterPage.BPlusTreeSearchType.LastElement, false);
            }
            return new RecordKey(x.PageID, location);

        }

        ///// <summary>
        ///// Gets the location of the first occurance of the key, returning the RecordKey.RecordNotFound if it doesnt exist
        ///// </summary>
        ///// <param name="Key"></param>
        ///// <returns></returns>
        //public virtual RecordKey SeekFirstStrict(Record Key)
        //{
            
        //    RecordKey k = this.SeekFirst(Key);
        //    ClusterPage p = this.GetBranchPage(k.PAGE_ID);

        //    if (k.ROW_ID >= p.Count)
        //        return RecordKey.RecordNotFound;
        //    else if (Record.Compare(Record.Split(p.Select(k.ROW_ID), p.WeakKeyColumns), Key) != 0)
        //        return RecordKey.RecordNotFound;

        //    return k;

        //}

        ///// <summary>
        ///// Gets the location of the last occurance of the key, returning the RecordKey.RecordNotFound if it doesnt exist
        ///// </summary>
        ///// <param name="Key"></param>
        ///// <returns></returns>
        //public virtual RecordKey SeekLastStrict(Record Key)
        //{

        //    RecordKey k = this.SeekLast(Key);
        //    ClusterPage p = this.GetBranchPage(k.PAGE_ID);

        //    if (k.ROW_ID >= p.Count)
        //        return RecordKey.RecordNotFound;
        //    else if (Record.Compare(Record.Split(p.Select(k.ROW_ID), p.WeakKeyColumns), Key) != 0)
        //        return RecordKey.RecordNotFound;

        //    return k;

        //}

        /// <summary>
        /// Checks if a key exists
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public virtual bool Exists(Record Key)
        {

            // If the root page is a leaf node //
            if (this._Root.IsLeaf)
                return this._Root.KeyExists(Key);

            // Otherwise, starting at root, find the page //
            ClusterPage x = this._Root;
            while (true)
            {
                int PageID = x.GetPageID(Key);
                x = this.GetPage(PageID);
                if (x.IsLeaf)
                    return x.KeyExists(Key);

            }

        }

        /// <summary>
        /// Gets the very first leaf page id in the data stream
        /// </summary>
        /// <returns></returns>
        public virtual int SeekOriginPageID()
        {

            if (this.Root.IsLeaf)
                return this.Root.PageID;

            ClusterPage p = this.Root;
            while (true)
            {
                p = this.GetPage(p.GetPageID(0));
                if (p.IsLeaf)
                    return p.PageID;
            }

        }

        /// <summary>
        /// Gets the very last page id in the data stream
        /// </summary>
        /// <returns></returns>
        public virtual int SeekTerminalPageID()
        {

            if (this.Root.IsLeaf)
                return this.Root.PageID;

            ClusterPage p = this.Root;
            while (true)
            {
                int id = p.GetPageID(p.Count - 1); // Note that the last record is always MAX_RECORD, so we want the second to last
                p = this.GetPage(id);
                if (p.IsLeaf)
                    return p.PageID;
            }

        }

        // Inserts //
        /// <summary>
        /// Core insertion control; inserts a value into the b+ tree and splits any nodes if needed
        /// </summary>
        /// <param name="Key"></param>
        public virtual void Insert(Record Element)
        {

            // Check if it exists only if this is unqiue //
            //if (this.IsUnique)
            //{
            //    if (this.Exists(Record.Split(Key, this._IndexColumns)))
            //        throw new DuplicateKeyException(string.Format("Key exists {0}", Record.Split(Key, this._IndexColumns)));
            //}

            // Finde the leaf node to insert into //
            ClusterPage node = this.SeekPage(Element);

            // Actually insert the value //
            this.InsertValue(node, Element);

            // Tick up the records //
            this._Header.RecordCount++;

        }

        /// <summary>
        /// Adds a key/page id to the tree; if the node is currently full, this method will split it, and update it's parent; if the parent is full, it'll also split; if the parent is the root and it's
        /// full, it'll split that too;
        /// </summary>
        /// <param name="OriginalNode">A branch node to append</param>
        /// <param name="Key">The key value of the index</param>
        /// <param name="PageID">The page id linking to the key</param>
        private void InsertKey(ClusterPage Node, Record Key, int PageID)
        {

            if (Node.IsLeaf)
                throw new Exception("OriginalNode passed must be a branch node");

            // Get the child page //
            ClusterPage Child = this.GetPage(PageID);

            // The node is not full; note that the node will handle flipping the overflow ID if needed //
            if (Node.CanAccept(ClusterPage.Composite(Key, PageID)))
            {
                Child.ParentPageID = Node.PageID;
                Node.InsertKey(Key, PageID);
                return;
            }

            // Otherwise, the node is full and we need to split //
            ClusterPage y = this.SplitBranch(Node);

            // But... we don't know for sure if we should insert into 'OriginalNode' or 'NewPage', so we need to check //
            if (Node.LessThanTerminal(Key))
            {
                Child.ParentPageID = Node.PageID;
                Node.InsertKey(Key, PageID);
            }
            else
            {
                Child.ParentPageID = y.PageID;
                y.InsertKey(Key, PageID);
            }

        }

        /// <summary>
        /// Inserts a value into a node;
        /// If the node is full, this method will split it and update it's parent; using 'InsertKey'
        /// </summary>
        /// <param name="OriginalNode">The node to insert</param>
        /// <param name="Key">The data record (having the same schema as the parent table)</param>
        private void InsertValue(ClusterPage Node, Record Element)
        {

            if (!Node.IsLeaf)
                throw new Exception("OriginalNode passed must be a branch node");

            // InsertKey if the node isnt full //
            if (Node.CanAccept(Element))
            {
                Node.Insert(Element);
                return;
            }

            // Otherwise, the node is full and we need to split //
            ClusterPage y = this.SplitLeaf(Node);

            // But... we don't know for sure if we should insert into 'OriginalNode' or 'NewPage', so we need to check //
            if (Node.LessThanTerminal(Element))
            {
                Node.Insert(Element);
            }
            else
            {
                y.Insert(Element);
            }

        }

        // Splits //
        /// <summary>
        /// Splits a branch node; this will re-balance all nodes above it;
        /// </summary>
        /// <param name="OriginalPage">The node to be split; after this method is called, this node will have the LOWER half of all records before the split</param>
        /// <returns>A new node with the UPPER half of all record in the OriginalPage; this method will append the parent nodes</returns>
        private ClusterPage SplitBranch(ClusterPage OriginalNode)
        {

            if (OriginalNode.IsLeaf)
                throw new Exception("OriginalNode passed must be a branch node");

            // Split up the page; splitting will set the ParentID and the Leafness, but it won't set the overflow page id //
            ClusterPage NewNode = OriginalNode.SplitXPage(this._Storage.GenerateNewPageID, OriginalNode.PageID, OriginalNode.NextPageID, OriginalNode.Count / 2);
            this._Storage.SetPage(NewNode);
            this._Storage.Header.PageCount++;
            NewNode.Version++;

            // Marry the new and og nodes //
            NewNode.LastPageID = OriginalNode.PageID;
            OriginalNode.NextPageID = NewNode.PageID;

            // Set the last page id for the next page id
            if (NewNode.NextPageID != -1)
            {
                ClusterPage UpPage = this.GetPage(NewNode.NextPageID);
                UpPage.LastPageID = NewNode.PageID;
            }

            // Now we have to go through all NewNode's children and update their parent page id //
            List<int> PageIDs = NewNode.AllPageIDs();
            foreach (int PageID in PageIDs)
            {
                ClusterPage q = this.GetPage(PageID);
                q.ParentPageID = NewNode.PageID;
            }

            // Finally, we need to handle introducing NewNode to OriginalPage's parent //
            if (OriginalNode.ParentPageID != -1) // OriginalPage isnt the root node
            {

                // Get the parent //
                ClusterPage parent = this.GetPage(OriginalNode.ParentPageID);

                // Need to update the key for OriginalPage to be it's last record //
                parent.Delete(ClusterPage.Composite(Record.Split(NewNode.TerminalRecord, NewNode.WeakKeyColumns), OriginalNode.PageID));

                // Note: we know because we just removed a record that the parent node is not full, so we do a direct insert
                //      we would run into trouble if we didnt do this and the record we were deleting was the last in the tree's node
                //      becuase down stream the nodes will error out if we try to insert something higher than they are
                parent.InsertKeyUnsafe(Record.Split(NewNode.TerminalRecord, NewNode.WeakKeyColumns), NewNode.PageID);

                // If it is the highest //
                if (OriginalNode.IsHighest)
                {
                    OriginalNode.IsHighest = false;
                    NewNode.IsHighest = true;
                }

                // Need to add in NewNode //
                this.InsertKey(parent, Record.Split(OriginalNode.TerminalRecord, OriginalNode.WeakKeyColumns), OriginalNode.PageID);

            }
            // if this is the root page //
            else
            {

                // Create a new root page //
                this._Root = this.NewRootAsBranch();
                OriginalNode.ParentPageID = this._Root.PageID;
                NewNode.ParentPageID = this._Root.PageID;

                // Need to insert the last row of the current record //
                this._Root.Insert(ClusterPage.Composite(Record.Split(OriginalNode.TerminalRecord, OriginalNode.WeakKeyColumns), OriginalNode.PageID));

                // Need to add the max record to this layer //
                this._Root.Insert(ClusterPage.Composite(this._MaxRecord, NewNode.PageID));

                // Set the new node to be the highest //
                NewNode.IsHighest = true;
                OriginalNode.IsHighest = false;

                // Set the root //
                this._Storage.SetPage(this._Root);
                this._Storage.Header.PageCount++;

            }

            return NewNode;

        }

        /// <summary>
        /// Splits a leaf node
        /// </summary>
        /// <param name="OriginalPage">The node to split; this node will have the LOWER half of the original nodes record after the split</param>
        /// <returns>A new node with the UPPER half of all records on the original node</returns>
        private ClusterPage SplitLeaf(ClusterPage OriginalNode)
        {

            if (!OriginalNode.IsLeaf)
                throw new Exception("OriginalNode passed must be a leaf node");

            Record OriginalTerminal = OriginalNode.TerminalRecord;

            // Split up the page; splitting will set the ParentID and the Leafness, but it won't set the overflow page id //
            ClusterPage NewNode = OriginalNode.SplitXPage(this._Storage.GenerateNewPageID, OriginalNode.PageID, OriginalNode.NextPageID, OriginalNode.Count / 2);
            this._Storage.SetPage(NewNode);
            this._Storage.Header.PageCount++;
            NewNode.Version++;

            // Check the last ID //
            if (this.TerminalBTreePageID == OriginalNode.PageID)
            {
                this.TerminalBTreePageID = NewNode.PageID;
            }

            // Marry the new and og nodes //
            NewNode.LastPageID = OriginalNode.PageID;
            OriginalNode.NextPageID = NewNode.PageID;

            // Set the last page id for the next page id
            if (NewNode.NextPageID != -1)
            {
                ClusterPage UpPage = this.GetPage(NewNode.NextPageID);
                UpPage.LastPageID = NewNode.PageID;
            }

            // We need to introduce NewNode to it's parent //
            if (OriginalNode.ParentPageID != -1) // OriginalPage isnt the root node
            {

                // Get the parent //
                ClusterPage parent = this.GetPage(OriginalNode.ParentPageID);

                // If it is the highest //
                if (OriginalNode.IsHighest)
                {

                    // Turn off the highest flag //
                    OriginalNode.IsHighest = false;
                    NewNode.IsHighest = true;

                    // The record in the parent is NOT the terminal record, it's the highest record //
                    parent.Delete(ClusterPage.Composite(this._MaxRecord, OriginalNode.PageID));

                    // Note: we know because we just removed a record that the parent node is not full, so we do a direct insert
                    //      we would run into trouble if we didnt do this and the record we were deleting was the last in the tree's node
                    //      becuase down stream the nodes will error out if we try to insert something higher than they are
                    parent.InsertKeyUnsafe(this._MaxRecord, NewNode.PageID);


                }
                else  // Note that becuase the child nodes are leafs, we do want to use the 'OriginalKeyColumns' not the 'Weak Key Columns'
                {

                    // Need to update the key for OriginalPage to be it's last record //
                    parent.Delete(ClusterPage.Composite(Record.Split(NewNode.TerminalRecord, NewNode.OriginalKeyColumns), OriginalNode.PageID));

                    // Note: we know because we just removed a record that the parent node is not full, so we do a direct insert
                    //      we would run into trouble if we didnt do this and the record we were deleting was the last in the tree's node
                    //      becuase down stream the nodes will error out if we try to insert something higher than they are
                    parent.InsertKeyUnsafe(Record.Split(NewNode.TerminalRecord, NewNode.OriginalKeyColumns), NewNode.PageID);

                }

                // add in OriginalPage because it didnt exist before
                this.InsertKey(parent, Record.Split(OriginalNode.TerminalRecord, OriginalNode.OriginalKeyColumns), OriginalNode.PageID);

            }
            // if this is the root page - note that this piece of code only get's triggered once in the tree development //
            else
            {

                // Create a new root page //
                this._Root = this.NewRootAsBranch();
                OriginalNode.ParentPageID = this._Root.PageID;
                NewNode.ParentPageID = this._Root.PageID;

                // Need to insert
                this._Root.Insert(ClusterPage.Composite(Record.Split(OriginalNode.TerminalRecord, OriginalNode.OriginalKeyColumns), OriginalNode.PageID));

                // Need to add the max record to this layer //
                this._Root.Insert(ClusterPage.Composite(this._MaxRecord, NewNode.PageID));

                // Set the new node to be the highest //
                NewNode.IsHighest = true;
                OriginalNode.IsHighest = false;

                // Set the root //
                this._Storage.SetPage(this._Root);
                this._Storage.Header.PageCount++;

            }

            return NewNode;

        }

        /// <summary>
        /// Gets a b+tree page; this method calls the page from the parent table and will either cast or do a hard convert to the b+tree method
        /// </summary>
        /// <param name="PageID">The page ID requested</param>
        /// <returns></returns>
        private ClusterPage GetPage(int PageID)
        {
            Page p = this._Storage.GetPage(PageID);
            return ClusterPage.Mutate(p, this._IndexColumns);
        }

        // New Root Methods //
        /// <summary>
        /// Generates a new root page as if it were a branch node
        /// </summary>
        /// <returns></returns>
        private ClusterPage NewRootAsBranch()
        {

            Schema s = Schema.Split(this._Storage.Columns, this._IndexColumns);
            s.Add("@IDX", CellAffinity.LONG);

            ClusterPage NewRoot = new ClusterPage(this._Storage.PageSize, this._Storage.GenerateNewPageID, -1, -1, s.Count, 0, this._IndexColumns, false, this.State);
            NewRoot.ParentPageID = -1;
            NewRoot.IsHighest = true;
            this._Header.RootPageID = NewRoot.PageID;
            NewRoot.Version++;

            return NewRoot;

        }

        /// <summary>
        /// Generates a new root node as a leaf node; note that this only called in the ctor method
        /// </summary>
        /// <returns></returns>
        private ClusterPage NewRootAsLeaf()
        {

            Schema s = this._Storage.Columns;

            ClusterPage NewRoot = new ClusterPage(this._Storage.PageSize, this._Storage.GenerateNewPageID, -1, -1, s.Count, 0, this._IndexColumns, true, this.State);
            NewRoot.ParentPageID = -1;
            NewRoot.IsHighest = true;
            this._Header.RootPageID = NewRoot.PageID;
            NewRoot.Version++;

            return NewRoot;

        }

        // Print //
        public void Dump(string Path, bool NonClustered)
        {

            using (StreamWriter writer = new StreamWriter(Path))
            {

                writer.WriteLine(string.Format("Alias: {0}", this._Header.Name));
                writer.WriteLine(string.Format("OriginPageID: {0}", this._Header.OriginPageID));
                writer.WriteLine(string.Format("TerminalPageID: {0}", this._Header.TerminalPageID));
                writer.WriteLine(string.Format("RootPageID: {0}", this._Header.RootPageID));
                writer.WriteLine(string.Format("PageCount: {0}", this._Header.PageCount));
                writer.WriteLine(string.Format("RecordCount: {0}", this._Header.RecordCount));

                this.DumpPage(writer, this._Root, 0, NonClustered);

            }

        }

        private void DumpPage(StreamWriter Writer, ClusterPage Branch, int Level, bool NonClustered)
        {

            // Dump this page //
            for (int i = 0; i < Branch.Count; i++)
            {
                string x = string.Format("Lvl {0} <{1},{2}>: {3}", Level, Branch.PageID, i, Branch.DebugString(i, NonClustered));
                Writer.WriteLine(x);
            }

            // Check if a leaf //
            if (Branch.IsLeaf)
                return;

            // Dump the child pages //
            foreach(int PageID in Branch.AllPageIDs())
            {
                this.DumpPage(Writer, this.GetPage(PageID), Level + 1, NonClustered);
            }

        }

        // Debugging //
        internal void Print(StreamWriter writer, ClusterPage Page)
        {

            // Print this data //
            if (Page.IsLeaf)
            {

                writer.WriteLine("--- Leaf {0} <{1},{2}> ---", Page.PageID, Page.LastPageID, Page.NextPageID);
                foreach (Record r in Page.Elements)
                {
                    writer.WriteLine(r);
                }

            }
            else
            {

                writer.WriteLine("--- Branch {0} <{1},{2}> ---", Page.PageID, Page.LastPageID, Page.NextPageID);
                foreach (Record r in Page.Elements)
                {
                    writer.WriteLine("Key {0} : Page ID {1}", Record.Split(r, Page.StrongKeyColumns), r._data.Last().INT_A);
                }

                // Write all the child nodes //
                List<int> pages = Page.AllPageIDs();
                foreach (int pageid in pages)
                {
                    this.Print(writer, this.GetPage(pageid));
                }

            }



        }

        internal void Print(string Path)
        {

            using (StreamWriter sw = new StreamWriter(Path))
            {
                this.Print(sw, this._Root);
                sw.Flush();
            }

        }

        internal void AppendString(StringBuilder sb, ClusterPage Node)
        {

            if (Node.IsLeaf)
            {
                sb.AppendLine(string.Format("\tLeaf: {0} | {1}", Node.PageID, Node.ParentPageID));
                return;
            }

            List<int> nodes = Node.AllPageIDs();
            int j = 0;
            sb.AppendLine(string.Format("Storage PageID {0}", Node.PageID));
            foreach (int i in nodes)
            {
                sb.AppendLine(string.Format("Child PageID {0} ", i));
            }
            sb.AppendLine("---------------");

            foreach (int i in nodes)
            {
                ClusterPage x = this.GetPage(i);
                this.AppendString(sb, x);
            }

        }

        internal string Tree()
        {
            StringBuilder sb = new StringBuilder();
            this.AppendString(sb, this._Root);
            return sb.ToString();
        }

        internal string PageMap()
        {

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this._Storage.PageCount; i++)
            {
                Page p = this._Storage.GetPage(i);
                sb.AppendLine(string.Format("PageID: {0} | ParentPageID {1} | IsLeaf {2}", p.PageID, p.X0, p.X1 == 1));
            }

            return sb.ToString();

        }

        // Exceptions //
        public class DuplicateKeyException : Exception
        {

            public DuplicateKeyException(string Message)
                : base(Message)
            {
            }

        }

        // Statics //
        public static Cluster CreateClusteredIndex(Table Parent, Key IndexColumns, ClusterState State)
        {
            return new Cluster(Parent, Parent.Columns, IndexColumns, null, Parent.Header, State);
        }

        public static Cluster OpenClusteredIndex(Table Parent)
        {

            if (Parent.Header.RootPageID == -1)
                throw new ArgumentException("Cannot open a clustered index; no such index exists");
            Key k = Parent.Header.ClusterKey;
            ClusterPage root = ClusterPage.Mutate(Parent.GetPage(Parent.Header.RootPageID), k);
            return new Cluster(Parent, Parent.Columns, k, root, Parent.Header, Parent.Header.ClusterKeyState);

        }

        public static Cluster CreateNonClusteredIndex(Table StorageAndParent, Key IndexColumns)
        {

            Key k = Key.Build(IndexColumns.Count);
            Schema s = Schema.Split(StorageAndParent.Columns, IndexColumns);
            s.Add("@PTR", CellAffinity.LONG, true, 8);
            return new Cluster(StorageAndParent, s, k, null, StorageAndParent.Header, ClusterState.Universal);

        }

        public static Cluster OpenNonClusteredIndex(Table StorageAndParent, IndexHeader Header)
        {

            Key k = Key.Build(Header.IndexColumns.Count);
            Schema s = Schema.Split(StorageAndParent.Columns, Header.IndexColumns);
            s.Add("@PTR", CellAffinity.LONG, true, 8);
            ClusterPage root = ClusterPage.Mutate(StorageAndParent.GetPage(Header.RootPageID), Header.IndexColumns);
            return new Cluster(StorageAndParent, s, k, root, Header, ClusterState.Universal);

        }

        public static Schema NonClusteredIndexColumns(Schema Columns, Key IndexColumns)
        {
            Key k = Key.Build(IndexColumns.Count);
            Schema s = Schema.Split(Columns, IndexColumns);
            s.Add("@PTR", CellAffinity.LONG, true, 8);
            return s;
        }

    }

}
