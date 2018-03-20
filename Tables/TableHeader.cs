using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;


namespace ArcticWind.Tables
{

    /// <summary>
    /// Represents meta data around a table
    /// </summary>
    public sealed class TableHeader : IElementHeader
    {

        public const int HASH_KEY = 0;

        /// <summary>
        /// The disk size of the header; 64k
        /// </summary>
        public const int SIZE = 64 * 1024;
        public const int LEN_SIZE = 4;

        public const int OFFSET_HASH_KEY = 0;
        public const int OFFSET_NAME_LEN = 4;
        public const int OFFSET_NAME = 8;
        public const int OFFSET_DIR_LEN = 72;
        public const int OFFSET_DIR = 76;
        public const int OFFSET_EXT_LEN = 332;
        public const int OFFSET_EXT = 336;
        public const int OFFSET_PAGE_COUNT = 352;
        public const int OFFSET_RECORD_COUNT = 356;
        public const int OFFSET_COLUMN_COUNT = 364;
        public const int OFFSET_FIRST_PAGE_ID = 368;
        public const int OFFSET_LAST_PAGE_ID = 372;
        public const int OFFSET_PAGE_SIZE = 376;
        public const int OFFSET_INDEX_COUNT = 380;
        public const int OFFSET_SORT_KEY = 384; // 136 bytes max; 4 for key len, 4 for primary flag, 8 * 16 (16 is the max) for column / affinty
        public const int OFFSET_ROOT_PAGE_ID = 520;
        public const int OFFSET_INDEX_TABLE = 1024; // 1024 total bytes

        public const int OFFSET_COLUMNS = 2048;

        public const int COL_NAME_LEN_PTR = 0;
        public const int COL_NAME_PTR = 1;
        public const int COL_AFFINITY = 33;
        public const int COL_SIZE = 34;
        //public const int COL_NULL = 35;
        public const int COL_REC_LEN = 36;

        public const string V1_EXTENSION = ".tdatv1";

        private string _Name;
        private string _Directory;
        private string _Extension;
        private int _PageSize;

        private TableHeader()
        {
            this.ClusterKey = new Key();
            this.RootPageID = -1;
            this.IndexHeaders = new List<IndexHeader>(8);
        }

        public TableHeader(string Name, string Directory, string Extension, int PageCount, long RecordCount, int FirstPageID, int LastPageID, int PageSize, Schema Columns)
            :this()
        {

            this.Name = Name;
            this.Directory = Directory;
            this.Extension = Extension;
            this.PageCount = PageCount;
            this.RecordCount = RecordCount;
            this.OriginPageID = FirstPageID;
            this.TerminalPageID = LastPageID;
            this.Columns = Columns;
            this.PageSize = PageSize;
            
        }

        public TableHeader(string Name, int PageCount, long RecordCount, int PageSize, Schema Columns)
            : this(Name, null, null, PageCount, RecordCount, 0, PageCount - 1, PageSize, Columns)
        {
        }

        public TableHeader(string Name, string Directory, string Extension, Schema Columns)
            : this(Name, Directory, Extension, 0, 0, 0, 0, Page.DEFAULT_SIZE, Columns)
        {
        }

        public TableHeader(string Name, Schema Columns)
            : this(Name, null, null, 0, 0, 0, 0, Page.DEFAULT_SIZE, Columns)
        {
        }

        /// <summary>
        /// The name of the table
        /// </summary>
        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
            }
        }

        /// <summary>
        /// If the table is Scribed, then this is the directory the file exists in; null if it's a dream table
        /// </summary>
        public string Directory
        {
            get
            {
                return this._Directory;
            }
            set
            {
                if (value == null)
                {
                    this._Directory = null;
                    return;
                }
                if (value.Last() != '\\')
                    value += '\\';
                this._Directory = value;
            }
        }

        /// <summary>
        /// If the table is Scribed, then this is the file extension; null if it's a dream table
        /// </summary>
        public string Extension
        {
            get
            {
                return this._Extension;
            }
            set
            {
                if (value == null)
                {
                    this._Extension = null;
                    return;
                }
                if (value.First() != '.')
                    value = '.' + value;
                this._Extension = value;
            }
        }

        /// <summary>
        /// The file path
        /// </summary>
        public string Path
        {
            get { return this.Directory + this.Name + this.Extension; }
        }

        /// <summary>
        /// The count of all pages (index and data)
        /// </summary>
        public int PageCount
        {
            get;
            set;
        }

        /// <summary>
        /// The count of all data records
        /// </summary>
        public long RecordCount
        {
            get;
            set;
        }

        /// <summary>
        /// The first data page id
        /// </summary>
        public int OriginPageID
        {
            get;
            set;
        }

        /// <summary>
        /// The last data page id
        /// </summary>
        public int TerminalPageID
        {
            get;
            set;
        }

        /// <summary>
        /// True if the table is a dream table, false if the table is scribed
        /// </summary>
        public bool IsMemoryOnly
        {
            get { return this.Directory == null; }
        }

        /// <summary>
        /// The table columns 
        /// </summary>
        public Schema Columns
        {
            get;
            set;
        }

        /// <summary>
        /// The table page size
        /// </summary>
        public int PageSize
        {
            get { return this._PageSize; }
            set
            {
                if (value % 4096 != 0 || value < 4096)
                    throw new ArgumentException(string.Format("The page size must be a multiple of 4KB (4096 bytes)"));
                this._PageSize = value;
            }
        }

        /// <summary>
        /// The page cache key; the name for dream tables, the path for scribed tables
        /// </summary>
        public string Key
        {
            get { return this.IsMemoryOnly ? this.Name : this.Path; }
        }

        /// <summary>
        /// The sorted key; if the table is not sorted, the key will have a length of zero; this will never be null
        /// </summary>
        public Key ClusterKey
        {
            get;
            set;
        }

        /// <summary>
        /// True if the ClusterKey is the primary key
        /// </summary>
        public ClusterState ClusterKeyState
        {
            get;
            set;
        }

        /// <summary>
        /// The root page only exists
        /// </summary>
        public int RootPageID
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum records per a page
        /// </summary>
        public int MaxRecordsPerPage
        {
            get { return (this.PageSize - Page.HEADER_SIZE) / this.Columns.RecordDiskCost; }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<IndexHeader> IndexHeaders
        {
            get;
            private set;
        }

        // Debug Print //
        public string DebugPrint()
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Alias: {0}", this.Name));
            sb.AppendLine(string.Format("Directory: {0}", this.Directory ?? "<Memory Only>"));
            sb.AppendLine(string.Format("Extension: {0}", this.Extension ?? "<Memory Only>"));
            sb.AppendLine(string.Format("Path: {0}", this.Path));
            sb.AppendLine(string.Format("Lookup Key: {0}", this.Key));
            sb.AppendLine(string.Format("Page Size: {0}", this.PageSize));
            sb.AppendLine(string.Format("Page Count: {0}", this.PageCount));
            sb.AppendLine(string.Format("Record Count: {0}", this.RecordCount));
            sb.AppendLine(string.Format("Origin Page: {0}", this.OriginPageID));
            sb.AppendLine(string.Format("Terminal Page: {0}", this.TerminalPageID));
            sb.AppendLine(string.Format("Radix Page: {0}", this.RootPageID));
            sb.AppendLine(string.Format("Max Records Per Page: {0}", this.MaxRecordsPerPage));
            sb.AppendLine(string.Format("Record Byte Length: {0}", this.Columns.RecordDiskCost));
            sb.AppendLine(string.Format("Disk Size: {0}", TableHeader.SIZE + this.PageCount * this.PageSize));
            sb.AppendLine(string.Format("Avg Page Fullness: {0}%", Math.Round((double)this.RecordCount / ((double)this.PageCount * (double)this.MaxRecordsPerPage), 3) * 100D));
            if (this.ClusterKey.Count != 0)
            {
                sb.AppendLine(this.ClusterKeyState.ToString() + " Index:");
                for (int i = 0; i < this.ClusterKey.Count; i++)
                {
                    sb.AppendLine(string.Format("\t{0} : {1}", this.Columns.ColumnName(this.ClusterKey[i]), this.Columns.ColumnAffinity(i)));
                }
            }
            sb.AppendLine("Columns:");
            for (int i = 0; i < this.Columns.Count; i++)
            {
                sb.AppendLine(string.Format("\t{0} : {1}.{2}", this.Columns.ColumnName(i), this.Columns.ColumnAffinity(i), this.Columns.ColumnSize(i)));
            }
            if (this.IndexHeaders.Count != 0)
            {
                sb.AppendLine("Indexes:");
                foreach (IndexHeader ih in this.IndexHeaders)
                {
                    sb.AppendLine(string.Format("\tAlias: {0}", ih.Name));
                    sb.AppendLine(string.Format("\tKeys: {0}", Schema.Split(this.Columns, ih.IndexColumns).ToNameString(',')));
                    sb.AppendLine(string.Format("\tOrigin Page: {0}", ih.OriginPageID));
                    sb.AppendLine(string.Format("\tTerminal Page: {0}", ih.TerminalPageID));
                    sb.AppendLine(string.Format("\tRoot Page: {0}", ih.RootPageID));
                }
            }

            return sb.ToString();

        }

        // Static Methods //
        /// <summary>
        /// Converts a byte array to a TableHeader
        /// </summary>
        /// <param name="Buffer"></param>
        /// <param name="Location"></param>
        /// <returns></returns>
        public static TableHeader FromHash(byte[] Buffer, int Location)
        {

            // Check the size //
            if (Buffer.Length - Location < TableHeader.SIZE)
                throw new Exception("Buffer is incorrect size");

            // Check the hash key //
            if (BitConverter.ToInt32(Buffer, Location + OFFSET_HASH_KEY) != HASH_KEY)
                throw new Exception("Invalid hash key");

            // Create //
            TableHeader h = new TableHeader();
            int Len = 0;

            // Alias //
            Len = BitConverter.ToInt32(Buffer, Location + OFFSET_NAME_LEN);
            h.Name = ASCIIEncoding.ASCII.GetString(Buffer, Location + OFFSET_NAME, Len);

            // Directory //
            Len = BitConverter.ToInt32(Buffer, Location + OFFSET_DIR_LEN);
            h.Directory = ASCIIEncoding.ASCII.GetString(Buffer, Location + OFFSET_DIR, Len);

            // Extension //
            Len = BitConverter.ToInt32(Buffer, Location + OFFSET_EXT_LEN);
            h.Extension = ASCIIEncoding.ASCII.GetString(Buffer, Location + OFFSET_EXT, Len);

            // Page count //
            h.PageCount = BitConverter.ToInt32(Buffer, Location + OFFSET_PAGE_COUNT);

            // Row count //
            h.RecordCount = BitConverter.ToInt64(Buffer, Location + OFFSET_RECORD_COUNT);

            // Column Count //
            int ColCount = BitConverter.ToInt32(Buffer, Location + OFFSET_COLUMN_COUNT);

            // First Page //
            h.OriginPageID = BitConverter.ToInt32(Buffer, Location + OFFSET_FIRST_PAGE_ID);

            // Last Page //
            h.TerminalPageID = BitConverter.ToInt32(Buffer, Location + OFFSET_LAST_PAGE_ID);

            // Page PageSize //
            h.PageSize = BitConverter.ToInt32(Buffer, Location + OFFSET_PAGE_SIZE);

            // Radix Page //
            h.RootPageID = BitConverter.ToInt32(Buffer, Location + OFFSET_ROOT_PAGE_ID);

            // Key //
            h.ClusterKey = new Key();
            h.ClusterKeyState = (ClusterState)Buffer[Location + OFFSET_SORT_KEY]; // gets the unique
            int KeyCount = BitConverter.ToInt32(Buffer, Location + OFFSET_SORT_KEY + 4); // gets the key size
            for (int i = 0; i < KeyCount; i++)
            {
                int loc = Location + OFFSET_SORT_KEY + 8 + 8 * i;
                int idx = BitConverter.ToInt32(Buffer, loc);
                KeyAffinity affinity = (KeyAffinity)BitConverter.ToInt32(Buffer, loc + 4);
                h.ClusterKey.Add(idx, affinity);
            }

            // Read the index table //
            int idx_cnt = BitConverter.ToInt32(Buffer, Location + OFFSET_INDEX_COUNT);
            for (int i = 0; i < idx_cnt; i++)
            {
                int pos = Location + OFFSET_INDEX_TABLE + i * IndexHeader.SIZE_HASH;
                IndexHeader idx_h = IndexHeader.Read(Buffer, pos);
                h.IndexHeaders.Add(idx_h);
            }

            // Load the columns //
            h.Columns = new Schema();
            for (int i = 0; i < ColCount; i++)
            {

                int RecordOffset = Location + OFFSET_COLUMNS + i * COL_REC_LEN;
                int NameLen = (int)Buffer[RecordOffset];
                string ColName = ASCIIEncoding.ASCII.GetString(Buffer, RecordOffset + COL_NAME_PTR, NameLen);
                CellAffinity ColType = (CellAffinity)Buffer[RecordOffset + COL_AFFINITY];

                int ColSize = (int)(BitConverter.ToInt16(Buffer, RecordOffset + COL_SIZE));
                bool ColNull = true;
                if (ColSize < 0)
                {
                    ColSize = -ColSize;
                    ColNull = false;
                }

                h.Columns.Add(ColName, ColType, ColNull, ColSize);

            }

            return h;

        }

        /// <summary>
        /// Converts a TableHeader to a byte array
        /// </summary>
        /// <param name="Buffer"></param>
        /// <param name="Location"></param>
        /// <param name="Key"></param>
        public static void ToHash(byte[] Buffer, int Location, TableHeader Element)
        {

            // Check the size //
            if (Buffer.Length - Location < TableHeader.SIZE)
                throw new Exception("Buffer is incorrect size");

            // Write the hash key //
            Array.Copy(BitConverter.GetBytes(HASH_KEY), 0, Buffer, Location + OFFSET_HASH_KEY, LEN_SIZE);

            // Write the name //
            Array.Copy(BitConverter.GetBytes(Element.Name.Length), 0, Buffer, Location + OFFSET_NAME_LEN, LEN_SIZE);
            Array.Copy(ASCIIEncoding.ASCII.GetBytes(Element.Name), 0, Buffer, Location + OFFSET_NAME, Element.Name.Length);

            // Write the directory //
            Array.Copy(BitConverter.GetBytes(Element.Directory.Length), 0, Buffer, Location + OFFSET_DIR_LEN, LEN_SIZE);
            Array.Copy(ASCIIEncoding.ASCII.GetBytes(Element.Directory), 0, Buffer, Location + OFFSET_DIR, Element.Directory.Length);

            // Write the extension //
            Array.Copy(BitConverter.GetBytes(Element.Extension.Length), 0, Buffer, Location + OFFSET_EXT_LEN, LEN_SIZE);
            Array.Copy(ASCIIEncoding.ASCII.GetBytes(Element.Extension), 0, Buffer, Location + OFFSET_EXT, Element.Extension.Length);

            // Write page count //
            Array.Copy(BitConverter.GetBytes(Element.PageCount), 0, Buffer, Location + OFFSET_PAGE_COUNT, LEN_SIZE);

            // Write record count //
            Array.Copy(BitConverter.GetBytes(Element.RecordCount), 0, Buffer, Location + OFFSET_RECORD_COUNT, 8); // Long integer

            // Write column count //
            Array.Copy(BitConverter.GetBytes(Element.Columns.Count), 0, Buffer, Location + OFFSET_COLUMN_COUNT, LEN_SIZE);

            // Write first page ID //
            Array.Copy(BitConverter.GetBytes(Element.OriginPageID), 0, Buffer, Location + OFFSET_FIRST_PAGE_ID, LEN_SIZE);

            // Write last page ID //
            Array.Copy(BitConverter.GetBytes(Element.TerminalPageID), 0, Buffer, Location + OFFSET_LAST_PAGE_ID, LEN_SIZE);

            // Write page size //
            Array.Copy(BitConverter.GetBytes(Element.PageSize), 0, Buffer, Location + OFFSET_PAGE_SIZE, LEN_SIZE);

            // Write radix page //
            Array.Copy(BitConverter.GetBytes(Element.RootPageID), 0, Buffer, Location + OFFSET_ROOT_PAGE_ID, LEN_SIZE);

            // Write key //
            Buffer[Location + OFFSET_SORT_KEY] = (byte)Element.ClusterKeyState;
            byte[] b = Element.ClusterKey.Bash();
            Array.Copy(b, 0, Buffer, Location + OFFSET_SORT_KEY + 4, b.Length);

            // Write the index table //
            Array.Copy(BitConverter.GetBytes(Element.IndexHeaders.Count), 0, Buffer, Location + OFFSET_INDEX_COUNT, LEN_SIZE);
            for (int i = 0; i < Element.IndexHeaders.Count; i++)
            {
                int pos = Location + OFFSET_INDEX_TABLE + i * IndexHeader.SIZE_HASH;
                IndexHeader.Write(Buffer, pos, Element.IndexHeaders[i]);
            }

            // Write schema //
            for (int i = 0; i < Element.Columns.Count; i++)
            {

                Cell c = new Cell((short)(Element.Columns.ColumnSize(i) * (Element.Columns.ColumnNull(i) ? 1 : -1)));

                byte NameLen = (byte)Element.Columns.ColumnName(i).Length;
                byte Affinity = (byte)Element.Columns.ColumnAffinity(i);
                byte Size1 = c.B0;
                byte Size2 = c.B1;
                byte[] Name = ASCIIEncoding.ASCII.GetBytes(Element.Columns.ColumnName(i));

                int ptr = Location + OFFSET_COLUMNS + i * COL_REC_LEN;
                Buffer[ptr + COL_NAME_LEN_PTR] = NameLen;
                Buffer[ptr + COL_AFFINITY] = Affinity;
                Buffer[ptr + COL_SIZE] = Size1;
                Buffer[ptr + COL_SIZE + 1] = Size2;
                //Buffer[ptr + COL_NULL] = Nullness;
                Array.Copy(Name, 0, Buffer, ptr + COL_NAME_PTR, Name.Length);

            }

        }

        /// <summary>
        /// Creates a header for a dream table
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Columns"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static TableHeader DreamHeader(string Name, Schema Columns, int PageSize)
        {
            return new TableHeader(Name, null, null, 0, 0, -1, -1, PageSize, Columns);
        }

        /// <summary>
        /// Gets the file name of a v1 rye dataset
        /// </summary>
        /// <param name="Dir"></param>
        /// <param name="Alias"></param>
        /// <returns></returns>
        public static string DeriveV1Path(string Dir, string Name)
        {
            if (Dir.Last() != '\\') Dir += '\\';
            return Dir + Name + V1_EXTENSION;
        }

    }


}
