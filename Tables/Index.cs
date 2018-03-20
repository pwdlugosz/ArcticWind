using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;


namespace ArcticWind.Tables
{

    /// <summary>
    /// Represents a non-clustered index using a b+tree
    /// </summary>
    public class Index
    {

        /// <summary>
        /// Represents the base b+tree object
        /// </summary>
        protected Cluster _Tree;

        /// <summary>
        /// Represents the table where the index will be stored
        /// </summary>
        protected Table _Storage;

        /// <summary>
        /// Represents the table that the index is
        /// </summary>
        protected Table _Parent;

        /// <summary>
        /// 
        /// </summary>
        protected Key _IndexColumns;

        /// <summary>
        /// 
        /// </summary>
        protected IndexHeader _Header;

        /// <summary>
        /// Used only by inherited classes
        /// </summary>
        protected Index()
        {
        }

        /// <summary>
        /// Opens an existing index
        /// </summary>
        /// <param name="Storage"></param>
        /// <param name="Parent"></param>
        /// <param name="Header"></param>
        public Index(Table Storage, Table Parent, IndexHeader Header)
        {

            this._Storage = Storage;
            this._Parent = Parent;
            this._Header = Header;
            this._IndexColumns = Header.IndexColumns;
            ClusterPage root = ClusterPage.Mutate(this._Storage.GetPage(Header.RootPageID), Header.IndexColumns);
            Schema s = Cluster.NonClusteredIndexColumns(this._Parent.Columns, Header.IndexColumns);
            this._Tree = new Cluster(Storage, s, Key.Build(this._IndexColumns.Count), root, Header, ClusterState.Universal);

        }

        /// <summary>
        /// Creates a new index
        /// </summary>
        /// <param name="Storage"></param>
        /// <param name="Parent"></param>
        /// <param name="IndexColumns"></param>
        public Index(Table Storage, Table Parent, string Name, Key IndexColumns)
        {

            this._Header = new IndexHeader(Name, -1, -1, -1, 0, 0, IndexColumns);
            this._Storage = Storage;
            this._Parent = Parent;
            this._IndexColumns = IndexColumns;
            Schema s = Cluster.NonClusteredIndexColumns(this._Parent.Columns, this._IndexColumns);
            this._Tree = new Cluster(this._Storage, s, Key.Build(this._IndexColumns.Count), null, this._Header, ClusterState.Universal);
        }

        // Properties //
        /// <summary>
        /// The table the data is stored in (may be the same as Parent)
        /// </summary>
        public Table Storage
        {
            get { return this._Storage; }
        }

        /// <summary>
        /// The table the data is indexing (may be the same as Storage)
        /// </summary>
        public Table Parent
        {
            get { return this._Parent; }
        }

        /// <summary>
        /// The columns that are indexed
        /// </summary>
        public Key IndexColumns
        {
            get { return this._IndexColumns; }
        }

        /// <summary>
        /// The index header
        /// </summary>
        public IndexHeader Header
        {
            get { return this._Header; }
        }

        /// <summary>
        /// The inner cluster
        /// </summary>
        public Cluster Tree
        {
            get { return this._Tree; }
        }

        // Methods //
        /// <summary>
        /// Inserts a record into the index
        /// </summary>
        /// <param name="Key">The record</param>
        /// <param name="Key">The position it's located in the table</param>
        public virtual void Insert(Record Element, RecordKey Key)
        {
            Record x = Index.GetIndexElement(Element, Key, this._IndexColumns);
            this._Tree.Insert(x);
        }

        /// <summary>
        /// Opens a reader
        /// </summary>
        /// <returns></returns>
        public virtual RecordReader OpenReader()
        {
            return new RecordReaderIndexData(this._Header, this._Storage, this._Parent);
        }

        /// <summary>
        /// Opens a reader at the location of the key and containing only the key
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public virtual RecordReader OpenReader(Record Key)
        {
            RecordKey l = this._Tree.SeekFirst(Key, false);
            RecordKey u = this._Tree.SeekLast(Key, false);
            return new RecordReaderIndexData(this._Header, this._Storage, this._Parent, l, u);
        }

        /// <summary>
        /// Opens a reader between each key
        /// </summary>
        /// <param name="LKey"></param>
        /// <param name="UKey"></param>
        /// <returns></returns>
        public virtual RecordReader OpenReader(Record LKey, Record UKey)
        {
            RecordKey l = this._Tree.SeekFirst(LKey, false);
            RecordKey u = this._Tree.SeekLast(UKey, false);
            return new RecordReaderIndexData(this._Header, this._Storage, this._Parent, l, u);
        }

        /// <summary>
        /// Opens a reader, but if the key is not found, it will return null
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public virtual RecordReader OpenStrictReader(Record Key)
        {
            RecordKey l = this._Tree.SeekFirst(Key, true);
            RecordKey u = this._Tree.SeekLast(Key, true);
            if (l.IsNotFound || u.IsNotFound)
                return null;
            return new RecordReaderIndexData(this._Header, this._Storage, this._Parent, l, u);
        }

        /// <summary>
        /// Calibrates the index
        /// </summary>
        public virtual void Calibrate()
        {

            if (this._Header.RecordCount != 0 || this._Parent.RecordCount == 0)
                return;

            RecordReader stream = this._Parent.OpenReader();
            while (stream.CanAdvance)
            {

                RecordKey rk = stream.PositionKey;
                Record rec = stream.ReadNext();

                this.Insert(rec, rk);

            }

        }

        // Statics //
        /// <summary>
        /// Creates the (Key, Pointer) record
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Pointer"></param>
        /// <param name="IndexColumns"></param>
        /// <returns></returns>
        public static Record GetIndexElement(Record Element, RecordKey Pointer, Key IndexColumns)
        {

            //Cell[] b = new Cell[IndexColumns.Count + 1];
            //for (int i = 0; i < IndexColumns.Count; i++)
            //{
            //    b[i] = Element[IndexColumns[i]];
            //}
            //b[b.Length - 1] = Pointer.Element;
            //return new Record(b);

            Record t = (Element * IndexColumns) + Pointer.Element;
            return t;

        }

        /// <summary>
        /// Creates an external index, but does not load it
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="IndexColumns"></param>
        /// <returns></returns>
        public static Index CreateExternalIndex(Table Parent, Key IndexColumns)
        {

            Schema columns = Cluster.NonClusteredIndexColumns(Parent.Columns, IndexColumns);
            ShellTable storage = new ShellTable(Parent.Host, Host.RandomName, Parent.Host.TempDB, columns, Page.DEFAULT_SIZE);
            return new Index(storage, Parent, Host.RandomName, IndexColumns);

        }

        /// <summary>
        /// Creates and builds a temporary index
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="IndexColumns"></param>
        /// <returns></returns>
        public static Index BuildTemporaryIndex(Table Parent, Key IndexColumns)
        {

            Schema columns = Cluster.NonClusteredIndexColumns(Parent.Columns, IndexColumns);
            ShellTable storage = new ShellTable(Parent.Host, Host.RandomName, Parent.Host.TempDB, columns, Page.DEFAULT_SIZE);
            Index idx = new Index(storage, Parent, Host.RandomName, IndexColumns);
            idx.Calibrate();
            Parent.Host.TableStore.PlaceInRecycleBin(storage.Key);
            return idx;

        }

    }

}
