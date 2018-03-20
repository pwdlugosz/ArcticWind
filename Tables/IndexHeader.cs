using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;


namespace ArcticWind.Tables
{

    public class IndexHeader : IElementHeader
    {

        /*
            Index table record:
            Alias: 36 (32 chars + 4 length)
            PageSize: 4 bytes
            OriginPageID: 4 bytes
            TerminalPageID: 4 bytes
            RootPageID: 4 bytes
            RecordCount: 4 bytes
            PageCount: 4 bytes
            Key: 4 byte count, n OriginalPage 8 bytes length (up to 8 columns)
         
            128 bytes 
         
         */

        public const int MAX_INDEX_COLUMNS = 8;
        public const int MAX_NAME_LEN = 32;
        public const int OFFSET_NAME = 0;
        public const int OFFSET_ORIGIN_PAGE_ID = 36;
        public const int OFFSET_TERMINAL_PAGE_ID = 40;
        public const int OFFSET_ROOT_PAGE_ID = 44;
        public const int OFFSET_RECORD_COUNT = 48;
        public const int OFFSET_PAGE_COUNT = 56;
        public const int OFFSET_INDEX_COLUMNS = 64;

        /// <summary>
        /// 128 bytes
        /// </summary>
        public const int SIZE_HASH = 128;

        private string _Name;
        private int _OriginPageID;
        private int _TerminalPageID;
        private int _RootPageID;
        private long _RecordCount;
        private int _PageCount;
        private Key _IndexColumns;

        private IndexHeader()
        {
            this._Name = "";
            this._IndexColumns = new Key();
        }

        public IndexHeader(string Name, int OriginPageID, int TerminalPageID, int RootPageID, long RecordCount, int PageCount, Key IndexColumns)
        {
            this.Name = Name;
            this.OriginPageID = OriginPageID;
            this.TerminalPageID = TerminalPageID;
            this.RootPageID = RootPageID;
            this.RecordCount = RecordCount;
            this.PageCount = PageCount;
            this.IndexColumns = IndexColumns;
        }

        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                if (value.Length > MAX_NAME_LEN)
                {
                    value = value.Substring(0, MAX_NAME_LEN);
                }
                this._Name = value;
            }

        }

        public int OriginPageID
        {
            get { return this._OriginPageID; }
            set { this._OriginPageID = value; }
        }

        public int TerminalPageID
        {
            get { return this._TerminalPageID; }
            set { this._TerminalPageID = value; }
        }

        public int RootPageID
        {
            get { return this._RootPageID; }
            set { this._RootPageID = value; }
        }

        public long RecordCount
        {
            get { return this._RecordCount; }
            set { this._RecordCount = value; }
        }

        public int PageCount
        {
            get { return this._PageCount; }
            set { this._PageCount = value; }
        }

        public Key IndexColumns
        {
            get
            {
                return this._IndexColumns;
            }
            set
            {
                if (value.Count > MAX_INDEX_COLUMNS)
                    throw new IndexOutOfRangeException("Can't index more than eight columns");
                this._IndexColumns = value;
            }
        }

        public int PointerIndex
        {
            get { return this._IndexColumns.Count; }
        }

        public static void Write(byte[] Buffer, int Location, IndexHeader Header)
        {

            Array.Copy(BitConverter.GetBytes(Header.Name.Length), 0, Buffer, Location + OFFSET_NAME, 4);
            Array.Copy(ASCIIEncoding.ASCII.GetBytes(Header.Name), 0, Buffer, Location + OFFSET_NAME + 4, Header.Name.Length);
            Array.Copy(BitConverter.GetBytes(Header.OriginPageID), 0, Buffer, Location + OFFSET_ORIGIN_PAGE_ID, 4);
            Array.Copy(BitConverter.GetBytes(Header.TerminalPageID), 0, Buffer, Location + OFFSET_TERMINAL_PAGE_ID, 4);
            Array.Copy(BitConverter.GetBytes(Header.RootPageID), 0, Buffer, Location + OFFSET_ROOT_PAGE_ID, 4);
            Array.Copy(BitConverter.GetBytes(Header.RecordCount), 0, Buffer, Location + OFFSET_RECORD_COUNT, 8);
            Array.Copy(BitConverter.GetBytes(Header.PageCount), 0, Buffer, Location + OFFSET_PAGE_COUNT, 4);
            Array.Copy(Header.IndexColumns.Bash(), 0, Buffer, Location + OFFSET_INDEX_COLUMNS, 4);

        }

        public static IndexHeader Read(byte[] Buffer, int Location)
        {

            IndexHeader h = new IndexHeader();
            int len = BitConverter.ToInt32(Buffer, Location + OFFSET_NAME);
            h.Name = ASCIIEncoding.ASCII.GetString(Buffer, Location + OFFSET_NAME + 4, len);
            h.OriginPageID = BitConverter.ToInt32(Buffer, Location + OFFSET_ORIGIN_PAGE_ID);
            h.TerminalPageID = BitConverter.ToInt32(Buffer, Location + OFFSET_TERMINAL_PAGE_ID);
            h.RootPageID = BitConverter.ToInt32(Buffer, Location + OFFSET_ROOT_PAGE_ID);
            h.RecordCount = BitConverter.ToInt64(Buffer, Location + OFFSET_RECORD_COUNT);
            h.PageCount = BitConverter.ToInt32(Buffer, Location + OFFSET_PAGE_COUNT);
            h.IndexColumns = Key.Read(Buffer, Location + OFFSET_INDEX_COLUMNS);

            return h;

        }

    }

}
