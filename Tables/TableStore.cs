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
    /// Represents a cache that holds table headers and pages
    /// </summary>
    public sealed class TableStore
    {

        /*
         * Terminology:
         *      Push: add or set/update an object
         *      Flush: save the object to disk
         *      Release: removes the object from memory
         *      Close: Flushes and releases a table
         * 
         */

        /// <summary>
        /// Minimum memory is 8MB
        /// </summary>
        public const long MIN_MEMORY = 1024 * 1024 * 8;

        /// <summary>
        /// Default maximum memory is 32MB
        /// </summary>
        public const long DEFAULT_MAX_MEMORY = 1024 * 1024 * 64;

        /// <summary>
        /// When freeing pages from memory, the free page factor increases the amount of memory freed
        /// </summary>
        private int _FreePageFactor = 2;

        /// <summary>
        /// Max memory the page cache can hold
        /// </summary>
        private long _MaxMemory = DEFAULT_MAX_MEMORY;

        /// <summary>
        /// The Spike memory being used
        /// </summary>
        private long _CurrentMemory = 0;

        /// <summary>
        /// Represents the cache of tables currently in memory
        /// </summary>
        private Dictionary<string, Table> _TableStore;

        /// <summary>
        /// Keeps track of the count of pages in memory for each table; if it ever reaches zero, the table will be removed form memory
        /// </summary>
        private Dictionary<string, int> _PageInMemoryCounts;

        /// <summary>
        /// Represents a cache of pages 
        /// </summary>
        private Dictionary<PageUID, Page> _PageStore;

        /// <summary>
        /// Represents a list of page IDs that can be dumped in order to save memory
        /// </summary>
        private ArcticWind.Elements.FloatingQueue<PageUID> _PageBurnStack;

        /// <summary>
        /// Represents a list of table objects that are temporary and can be destroyed
        /// </summary>
        private SortedSet<string> _RecycleBin;

        /// <summary>
        /// The host that owns this page cache
        /// </summary>
        private Host _Host;

        // ------------------------------------ CTOR ------------------------------------ //
        /// <summary>
        /// Creates a page cache with a given capacity
        /// </summary>
        /// <param name="MemoryCapacity"></param>
        public TableStore(Host Host, long MemoryCapacity)
        {

            this._Host = Host;
            this._MaxMemory = Math.Max(MemoryCapacity, MIN_MEMORY);
            
            this._TableStore = new Dictionary<string, Table>(StringComparer.OrdinalIgnoreCase);
            this._PageInMemoryCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            this._PageStore = new Dictionary<PageUID, Page>(PageUID.DefaultComparer);
            this._PageBurnStack = new FloatingQueue<PageUID>(1024, PageUID.DefaultComparer, FloatingQueue<PageUID>.State.LeastRecentlyUsed);
            this._RecycleBin = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);

        }

        /// <summary>
        /// Creates a table store with 64mb memory
        /// </summary>
        /// <param name="Host"></param>
        public TableStore(Host Host)
            : this(Host, DEFAULT_MAX_MEMORY)
        {
        }

        // Table Methods //
        /// <summary>
        /// Checks if a table is in memory
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool TableIsInMemory(string Key)
        {
            return this._TableStore.ContainsKey(Key);
        }

        /// <summary>
        /// Gets a table, either from memory or disk
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public Table RequestTable(string Key)
        {

            // #DEBUG# //
            this._Host.DebugPrint("TableStore.RequestTable({0})", Key);
            this._Host.DebugDepth++;

            if (this.TableIsInMemory(Key))
            {

                // #DEBUG# //
                this._Host.DebugPrint("TableStore.RequestTable->Table in memory({0})", Key);

                return this._TableStore[Key];

            }
            else
            {

                // Get the header //
                TableHeader h = Buffer(Key);
                Table t;

                // Create the actual table //
                if (h.RootPageID == -1)
                {

                    t = new HeapTable(this._Host, h);

                    // #DEBUG# //
                    this._Host.DebugPrint("TableStore.RequestTable->Table not in memory; buffering table as HEAP ({0})", Key);

                }
                else
                {

                    t = new ClusteredTable(this._Host, h);

                    // #DEBUG# //
                    this._Host.DebugPrint("TableStore.RequestTable->Table not in memory; buffering table as CLUSTER ({0})", Key);

                }

                // Add to the table store //
                this.PushTable(t);

                // Need to buffer a block of pages and make sure these pages are not in memory

                // Check to see how many pages we can buffer //
                int MaxPages = (int)(this.FreeMemory / h.PageSize);
                int Pages = Math.Min(h.PageCount, MaxPages);

                // Buffer a block of pages //

                // #DEBUG# //
                this._Host.DebugPrint("TableStore.RequestTable->Buffering a block of pages ({0}); from {1} - {2} of {3}", Key, 0, Pages, h.PageCount);

                foreach (Page p in BufferBlock(h, 0, Pages))
                {
                    
                    PageUID k = new PageUID(Key, p.PageID);
                    if (this._PageStore.ContainsKey(k))
                    {
                        this._PageStore[k] = p;
                    }
                    else
                    {
                        this._PageStore.Add(k, p);
                    }

                }
                this._Host.DebugDepth--;

                return t;

            }

        }

        /// <summary>
        /// Pushes a table to the cache
        /// </summary>
        /// <param name="Key"></param>
        public void PushTable(Table Element)
        {

            this._Host.DebugPrint("TableStore.PushTable({0})", Element.Key); // #DEBUG#
            if (this.TableIsInMemory(Element.Key))
            {
                this._Host.DebugPrint("TableStore.PushTable->table currently in memory({0})", Element.Key); // #DEBUG#
                this._TableStore[Element.Key] = Element;
            }
            else
            {
                this._Host.DebugPrint("TableStore.PushTable->table not in memory; table added to store({0})", Element.Key); // #DEBUG#
                this._TableStore.Add(Element.Key, Element);
                this._PageInMemoryCounts.Add(Element.Key, 0);
                this._CurrentMemory += TableHeader.SIZE;
                Element.IsOpen = true;
            }

        }

        /// <summary>
        /// Renames a table
        /// </summary>
        /// <param name="Element"></param>
        /// <param name="NewDir"></param>
        /// <param name="NewName"></param>
        public void RenameTable(Table Element, string NewDir, string NewName)
        {

            // Need to remove the table from memory completely //
            this.CloseTable(Element.Key);

            // Save the key //
            string OldPath = Element.Header.Path;

            // Change the dir and name //
            Element.Header.Directory = NewDir;
            Element.Header.Name = NewName;

            // Save the new file name //
            string NewPath = Element.Header.Path;

            // Change the file name //
            File.Move(OldPath, NewPath);

            // Save the header //
            Flush(NewPath, Element.Header);

            // Buffer the table back //
            this.RequestTable(Element.Key);

        }

        /// <summary>
        /// Copies a table to a new directory
        /// </summary>
        /// <param name="Element"></param>
        /// <param name="NewDir"></param>
        /// <param name="NewName"></param>
        public void CopyTable(Table Element, string NewDir, string NewName)
        {

            // Need to save the current copy to disk //
            this.FlushTable(Element.Key);

            // Save the key //
            string OldPath = Element.Header.Path;
            string NewPath = TableHeader.DeriveV1Path(NewDir, NewName);

            // Copy the file //
            File.Copy(OldPath, NewPath);

            // Buffer the header //
            TableHeader h = Buffer(NewPath);

            // Change the dir and name //
            h.Directory = NewDir;
            h.Name = NewName;

            // Save the header //
            Flush(NewPath, h);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        public void DropTable(string Key)
        {

            // #DEBUG# //
            this._Host.DebugPrint("TableStore.DropTable({0})", Key);

            // Remove the table from memory //
            if (this.TableIsInMemory(Key))
            {
            
                // Release all pages //
                //this.ReleaseAllPages(Key);

                // Release just the table //
                this.ReleaseTable(Key);
            
            }

            // Delete the file //
            if (File.Exists(Key))
                File.Delete(Key);

        }

        /// <summary>
        /// Saves the table to disk
        /// </summary>
        /// <param name="Key"></param>
        public void FlushTable(string Key)
        {

            // #DEBUG# //
            this._Host.DebugPrint("TableStore.FlushTable({0})", Key);

            if (this.TableIsInMemory(Key))
            {

                Table t = this._TableStore[Key];
                TableStore.Flush(Key, t.Header);
                this._Host.DebugPrint("TableStore.FlushTable->Flushing table header({0})", Key);
                
                foreach (Page p in this.SelectPages(Key))
                {
                    TableStore.Flush(Key, p);
                    this._Host.DebugPrint("TableStore.FlushTable->Flushing page({0}:{1})", Key, p.PageID);
                }

            }

        }

        /// <summary>
        /// Removes the table from memory
        /// </summary>
        /// <param name="Key"></param>
        public void ReleaseTable(string Key)
        {

            // #DEBUG# //
            this._Host.DebugPrint("TableStore.FlushTable({0})", Key);

            if (!this.TableIsInMemory(Key))
                return;

            this.ReleaseAllPages(Key); // Releases all pages 
            this._TableStore[Key].IsOpen = false; // Communicates to the in memory object that it's now out of memory 
            this._TableStore.Remove(Key); // Removes the table from the store
            this._PageInMemoryCounts.Remove(Key);
            this._CurrentMemory -= TableHeader.SIZE; // Decrements the memory size

        }

        /// <summary>
        /// Flushes and releases the table
        /// </summary>
        /// <param name="Key"></param>
        public void CloseTable(string Key)
        {

            // #DEBUG# //
            this._Host.DebugPrint("TableStore.CloseTable({0})", Key);

            this.FlushTable(Key);
            this.FlushAllPages(Key);

        }

        /// <summary>
        /// Checks if a table doesn't have any more pages in memory, if so, then 
        /// </summary>
        /// <param name="Key"></param>
        public void CheckTableClose(string Key)
        {
        }

        /// <summary>
        /// Checks if a table exists either in memory or on disk
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool TableExists(string Key)
        {
            return File.Exists(Key) || this.TableIsInMemory(Key);
        }

        // Page Methods //
        /// <summary>
        /// Checks if the table is in memory
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool PageIsInMemory(PageUID ID)
        {
            return this._PageStore.ContainsKey(ID);
        }

        /// <summary>
        /// Requests a page, either buffering it or pulling it from the in memory page store
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Page RequestPage(PageUID ID)
        {

            // Check the Page ID //
            if (ID.PageID < 0)
                throw new Exception(string.Format("Page ID {0} is invalid", ID.PageID));

            this._Host.DebugPrint("TableStore.RequestPage({0})", ID.ToString()); // #DEBUG#

            // Check if the page is in memory //
            if (this.PageIsInMemory(ID))
            {
                this._Host.DebugPrint("TableStore.RequestPage->page found in memory({0})", ID.ToString()); // #DEBUG#
                return this._PageStore[ID];
            }

            // Check if the table is in memory or not //
            if (!this.TableIsInMemory(ID.Key))
            {
                throw new ObjectNotInMemoryException(string.Format("Table '{0}' is not memory; critical code error", ID.Key)); 
            }

            // Otherwise, check if we have space //
            int Size = this._TableStore[ID.Key].PageSize;
            if (!this.HasSpaceFor(Size))
            {
                this.FreeSpace(Size);
            }

            // Tag in the recycling bin //
            this._PageBurnStack.EnqueueOrTag(ID);

            this._Host.DebugPrint("TableStore.RequestPage->page added to burn Intermediary({0})", ID.ToString()); // #DEBUG#

            // Page //
            Page p = TableStore.Buffer(ID.Key, ID.PageID, Size);
            this._PageInMemoryCounts[ID.Key]++;
            
            this._Host.DebugPrint("TableStore.RequestPage->page buffered({0})", ID.ToString()); // #DEBUG#
            
            this.PushPage(ID.Key, p);

            return p;

        }

        /// <summary>
        /// Adds a page to the store
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Key"></param>
        public void PushPage(string Key, Page Element)
        {

            // Check if the table is in memory or not //
            if (!this.TableIsInMemory(Key))
            {
                throw new ObjectNotInMemoryException(string.Format("Table '{0}' is not memory; critical code error", Key));
            }
            
            PageUID id = new PageUID(Key, Element.PageID);

            // Check if the page is in memory //
            if (this.PageIsInMemory(id))
            {

                // #DEBUG# //
                this._Host.DebugPrint("TableStore.PushPage-> page found in memory({0})", id.ToString());
                this._Host.DebugPrint("TableStore.PushPage-> page added to burn Intermediary({0})", id.ToString());

                this._PageStore[id] = Element;
                this._PageBurnStack.EnqueueOrTag(id);
                return;

            }

            // Otherwise, check if we have space //
            int Size = Element.PageSize;
            if (!this.HasSpaceFor(Size))
            {
                this._Host.DebugPrint("TableStore.PushPage-> Freeing space");
                this._Host.DebugDepth++;
                this.FreeSpace(Size * this._FreePageFactor);
                this._Host.DebugDepth--;
            }

            // Tag in the recycling bin //
            this._PageBurnStack.EnqueueOrTag(id);

            // Add to the store //
            this._PageStore.Add(id, Element);

            // Increment the used memory //
            this._CurrentMemory += Element.PageSize;

            // #DEBUG# //
            this._Host.DebugPrint("TableStore.PushPage-> page added({0}) : used/free memory {1}/{2}", id.ToString(), this.UsedMemory, this.FreeMemory);

        }

        /// <summary>
        /// Flushes a page to disk
        /// </summary>
        /// <param name="ID"></param>
        public void FlushPage(PageUID ID)
        {

            // Check if the page is in memory //
            if (!this.PageIsInMemory(ID))
                throw new ObjectNotInMemoryException(string.Format("Page ID '{0}' is not memory; critical code error", ID));

            Page p = this._PageStore[ID];

            // Check the version, only flush if the page has been altered //
            if (p.Version > 0)
            {

                TableStore.Flush(ID.Key, p);

                // #DEBUG# //
                this._Host.DebugPrint("TableStore.FlushPage-> page flushed to disk({0})", ID.ToString());

            }
            else
            {

                // #DEBUG# //
                this._Host.DebugPrint("TableStore.FlushPage-> page unaltered; not flushed({0})", ID.ToString());

            }


        }

        /// <summary>
        /// Removes a page from memory //
        /// </summary>
        /// <param name="ID"></param>
        public void ReleasePage(PageUID ID)
        {

            // Removes a page from memory //
            if (this.PageIsInMemory(ID))
            {
                this._CurrentMemory -= this._PageStore[ID].PageSize;
                this._PageInMemoryCounts[ID.Key]--;
                this._PageStore.Remove(ID);
                if (this._PageBurnStack.Contains(ID))
                    this._PageBurnStack.Remove(ID);
                this._Host.DebugPrint("TableStore.ReleasePage-> page released from memory({0}) : used/free memory {1}/{2}", ID.ToString(), this.UsedMemory, this.FreeMemory); // #DEBUG#
            }
            else
            {
                this._Host.DebugPrint("TableStore.ReleasePage-> page not found in memory({0}) : used/free memory {1}/{2}", ID.ToString(), this.UsedMemory, this.FreeMemory); // #DEBUG#
            }

        }

        /// <summary>
        /// Flushes a page and releases it from memory
        /// </summary>
        /// <param name="ID"></param>
        public void ClosePage(PageUID ID)
        {

            // #DEBUG# //
            this._Host.DebugPrint("TableStore.ClosePage-> ({0})", ID.ToString());

            this.FlushPage(ID);
            this.ReleasePage(ID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        public void FlushAllPages(string Key)
        {

            // #DEBUG# //
            this._Host.DebugPrint("TableStore.FlushAllPages({0})", Key);

            foreach (Page p in this.SelectPages(Key))
            {
                this.FlushPage(new PageUID(Key, p.PageID));
            }

        }

        /// <summary>
        /// Releases all pages from memory
        /// </summary>
        /// <param name="Key"></param>
        public void ReleaseAllPages(string Key)
        {

            // #DEBUG# //
            this._Host.DebugPrint("TableStore.ReleaseAllPages({0})", Key);

            foreach (Page p in this.SelectPages(Key))
            {
                this.ReleasePage(new PageUID(Key, p.PageID));
            }

        }

        /// <summary>
        /// Flushes all pages and releases them from memory
        /// </summary>
        /// <param name="Key"></param>
        public void CloseAllPages(string Key)
        {

            // #DEBUG# //
            this._Host.DebugPrint("TableStore.CloseAllPages({0})", Key);

            foreach (Page p in this.SelectPages(Key))
            {
                this.FlushPage(new PageUID(Key, p.PageID));
                this.ReleasePage(new PageUID(Key, p.PageID));
            }

        }

        /// <summary>
        /// Returns all pages linked to a given key
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public List<Page> SelectPages(string Key)
        {
            return this._PageStore.Where((x) => { return x.Key.Key == Key; }).Select((y) => { return y.Value; }).ToList();
        }

        // Memory Managment //
        /// <summary>
        /// Returns the maximum system memory
        /// </summary>
        public long MaxMemory
        {
            get { return this._MaxMemory; }
        }

        /// <summary>
        /// Returns the memory currently in use
        /// </summary>
        public long UsedMemory
        {
            get { return this._CurrentMemory; }
        }

        /// <summary>
        /// Returns the memory currently free
        /// </summary>
        public long FreeMemory
        {
            get { return this._MaxMemory - this._CurrentMemory; }
        }

        /// <summary>
        /// Frees disk space
        /// </summary>
        /// <param name="MemoryRequirement">The memory required to be freed</param>
        /// <returns>The actual memory freed</returns>
        public long FreeSpace(long MemoryRequirement)
        {

            // #DEBUG# //
            this._Host.DebugPrint("TableStore.FreeSpace({0})", MemoryRequirement);

            long ReleasedSpace = 0;
            this._Host.DebugDepth++;
            while (!this.HasSpaceFor(MemoryRequirement))
            {

                PageUID x = this._PageBurnStack.Dequeue();
                Page p = this._PageStore[x];
                this.ClosePage(x);
                ReleasedSpace += p.PageSize;

                // #DEBUG# //
                this._Host.DebugPrint("TableStore.FreeSpace-> page removed from memory({0})", x.ToString());

            }
            this._Host.DebugDepth--;

            // #DEBUG# //
            this._Host.DebugPrint("TableStore.FreeSpace-> total memory reclaimed({0})", ReleasedSpace);

            return ReleasedSpace;

        }

        /// <summary>
        /// Checks if we have space for an object
        /// </summary>
        /// <param name="MemorySize"></param>
        /// <returns></returns>
        public bool HasSpaceFor(long MemorySize)
        {
            return this.FreeMemory >= MemorySize;
        }

        // Shutdown //
        /// <summary>
        /// Adds a value to the recycling bin
        /// </summary>
        /// <param name="Key"></param>
        public void PlaceInRecycleBin(string Key)
        {

            // #DEBUG# //
            this._Host.DebugPrint("TableStore.PlaceInRecycleBin({0})", Key);

            if (this._RecycleBin.Contains(Key))
                return;
            this._RecycleBin.Add(Key);
        }

        /// <summary>
        /// Drops all tables in the recycle bin
        /// </summary>
        public void EmptyRecycleBin()
        {

            // #DEBUG# //
            this._Host.DebugPrint("TableStore.EmptyRecycleBin()");

            foreach (string s in this._RecycleBin)
                this.DropTable(s);

        }

        /// <summary>
        /// Empties the recycle bin and removes all objects from memory
        /// </summary>
        public void ShutDown()
        {

            // #DEBUG# //
            this._Host.DebugPrint("TableStore.ShutDown");

            // Memory Dump //
            this.MemoryDump(Host.LogDir + "Memory_Dump.txt");

            // Empty the recycle bin //
            this.EmptyRecycleBin();

            // Close all tables //
            foreach (string t in this._TableStore.Keys)
            {
                this.CloseTable(t);
            }

        }

        // Debugging //
        /// <summary>
        /// Dumps the Spike object meta data in memory to disk
        /// </summary>
        /// <param name="Path"></param>
        public void MemoryDump(string Path)
        {

            using (StreamWriter sw = new StreamWriter(Path))
            {

                // Meta Elements //
                sw.WriteLine("----- Meta Elements -----");
                sw.WriteLine("Used Memory={0}", this.UsedMemory);
                sw.WriteLine("Free Memory={0}", this.FreeMemory);
                sw.WriteLine("Max Memory={0}", this.MaxMemory);
                sw.WriteLine("Table Count={0}", this._TableStore.Count);
                sw.WriteLine("Page Count={0}", this._PageStore.Count);
                sw.WriteLine("Burn Pile Count={0}", this._PageBurnStack.Count);
                sw.WriteLine("Recycle Bin Count={0}", this._RecycleBin.Count);

                // Dump the table store //
                sw.WriteLine("----- Table Add -----");
                foreach (KeyValuePair<string, Table> kv in this._TableStore)
                {
                    sw.WriteLine("Name={0}; PageCount={1}; RecordCount={2}; Path={3}", kv.Value.Name, kv.Value.PageCount, kv.Value.RecordCount, kv.Value.Header.Path);
                }

                // Dump the page store //
                sw.WriteLine("----- Page Add -----");
                foreach (KeyValuePair<PageUID, Page> kv in this._PageStore)
                {
                    sw.WriteLine("ID={0}; RecorcCount={1}; Path={2}", kv.Value.PageID, kv.Value.Count, kv.Key.Key);
                }

                // Dump the recycle bind //
                sw.WriteLine("----- Recycle Bin -----");
                foreach (string x in this._RecycleBin)
                {
                    sw.WriteLine("Path={0}", x);
                }

                // Dump the page counts //
                sw.WriteLine("----- Page Counts -----");
                foreach (KeyValuePair<string, int> kv in this._PageInMemoryCounts)
                {
                    sw.WriteLine("Path={0}; Count={1}", kv.Key, kv.Value);
                }

                // Dump the burn pile //
                sw.WriteLine("----- Burn Stack -----");
                foreach (PageUID x in this._PageBurnStack)
                {
                    sw.WriteLine("ID={0}; Path={1}", x.PageID, x.Key);
                }

            }

        }

        /*
         * Disk methods:
         * All these methods hit the disk, but none of them use any class objects
         * 
         */

        /// <summary>
        /// Gets the on disk address of a given page
        /// </summary>
        /// <param name="PageID"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        internal static long PageAddress(int PageID, int PageSize)
        {

            long HeaderOffset = TableHeader.SIZE;
            long pid = (long)PageID;
            long ps = (long)PageSize;
            return HeaderOffset + pid * ps;

        }

        /// <summary>
        /// Buffers a page
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="PageID"></param>
        /// <returns></returns>
        internal static Page Buffer(string Path, int PageID, int PageSize)
        {

            
            // Get the location on disk of the page //
            long Location = PageAddress(PageID, PageSize);

            // Check the page id was valid //
            if (Location + PageSize > (new FileInfo(Path)).Length)
                throw new Exception("Page address is out of bounds");

            // Buffer the page //
            byte[] b = new byte[PageSize];
            using (FileStream x = File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.None))
            {

                // Go to the file offset //
                x.Position = Location;

                // Buffer the bytes //
                x.Read(b, 0, PageSize);

            }

            Page p = Page.Read2(b, 0);

            return p;

        }

        /// <summary>
        /// Buffers a block of pages
        /// </summary>
        /// <param name="Header"></param>
        /// <param name="PageOffset"></param>
        /// <param name="PageCount"></param>
        /// <returns></returns>
        internal static List<Page> BufferBlock(TableHeader Header, int PageOffset, int PageCount)
        {

            long Offset = TableHeader.SIZE + (long)(PageOffset) * (long)Header.PageSize;
            long ByteCount = (long)PageCount * (long)Header.PageSize;
            if (ByteCount > (long)int.MaxValue)
                throw new IndexOutOfRangeException("Cannot read more than 2gb into memory at once");

            byte[] b = new byte[ByteCount];
            using (FileStream fs = File.Open(Header.Path, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                fs.Position = Offset;
                fs.Read(b, 0, (int)ByteCount);
            }
            
            RecordMatcher matcher = new RecordMatcher(Header.ClusterKey);
            long Location = 0;
            List<Page> pages = new List<Page>();
            for (int i = 0; i < PageCount; i++)
            {

                Page p = Page.Read2(b, Location);
                if (p.PageType == Page.SORTED_PAGE_TYPE)
                {
                    p = new SortedPage(p, matcher);
                }
                Location += Header.PageSize;
                pages.Add(p);

            }

            return pages;

        }

        /// <summary>
        /// Reads the table header from disk, but does NOT allocate in the Spike heap
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        internal static TableHeader Buffer(string Path)
        {

            byte[] buffer = new byte[TableHeader.SIZE];
            using (FileStream x = File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                x.Read(buffer, 0, TableHeader.SIZE);
            }

            TableHeader h = TableHeader.FromHash(buffer, 0);

            return h;

        }

        /// <summary>
        /// Flushes a page to disk
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Key"></param>
        internal static void Flush(string Path, Page Element)
        {

            // Get the disk location //
            long Position = PageAddress(Element.PageID, Element.PageSize);

            // Convert to a hash //
            byte[] b = new byte[Element.PageSize];
            Page.Write2(b, 0, Element);

            // Hit the disk //
            using (FileStream x = File.Open(Path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                x.Position = Position;
                x.Write(b, 0, Element.PageSize);
            }


        }

        /// <summary>
        /// Flushes a table header to disk
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Key"></param>
        internal static void Flush(string Path, TableHeader Element)
        {

            // Convert to a hash //
            byte[] b = new byte[TableHeader.SIZE];
            TableHeader.ToHash(b, 0, Element);

            // Hit the disk //
            using (FileStream x = File.Open(Path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                x.Write(b, 0, b.Length);
            }

        }

        /// <summary>
        /// Deletes a file
        /// </summary>
        /// <param name="Path"></param>
        internal static void Burn(string Path)
        {
            if (File.Exists(Path))
                File.Delete(Path);
        }

        /// <summary>
        /// Checks if a table exists on disk
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        internal static bool TableExistsOnDisk(string Path)
        {
            return File.Exists(Path);
        }

        // Exception Classes //
        public sealed class ObjectDoesNotExistException : Exception
        {

            public ObjectDoesNotExistException(string Message)
                : base(Message)
            {
            }

        }

        public sealed class ObjectNotInMemoryException : Exception
        {

            public ObjectNotInMemoryException(string Message)
                : base(Message)
            {
            }

        }

    }

}
