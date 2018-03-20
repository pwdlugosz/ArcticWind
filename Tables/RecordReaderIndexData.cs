using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;


namespace ArcticWind.Tables
{

    /// <summary>
    /// Reads data from an index
    /// </summary>
    public class RecordReaderIndexData : RecordReaderBase
    {

        private IndexHeader _Header;
        private Table _Parent;

        /// <summary>
        /// Opens an indexed reader
        /// </summary>
        /// <param name="Header">The index header</param>
        /// <param name="Storage">The table that stores the index pages</param>
        /// <param name="Parent">The table that stores the data pages; may be the same object as 'Storage'</param>
        /// <param name="LKey">The lower bound key</param>
        /// <param name="RKey">The upper bound key</param>
        public RecordReaderIndexData(IndexHeader Header, Table Storage, Table Parent, RecordKey LKey, RecordKey RKey)
            : base(Storage, LKey, RKey)
        {
            this._Header = Header;
            this._Parent = Parent;
        }

        /// <summary>
        /// Opens an indexed reader
        /// </summary>
        /// <param name="Header">The index header</param>
        /// <param name="Storage">The table that stores the index pages</param>
        /// <param name="Parent">The table that stores the data pages; may be the same object as 'Storage'</param>
        public RecordReaderIndexData(IndexHeader Header, Table Storage, Table Parent)
            : this(Header, Storage, Parent, RecordReaderBase.OriginKey(Storage, Header), RecordReaderBase.TerminalKey(Storage, Header))
        {
        }

        public override Schema Columns
        {
            get
            {
                return this._Parent.Columns;
            }
        }

        public RecordKey ReadKey()
        {
            return new RecordKey(base.Read()[this._Header.PointerIndex]);
        }

        public RecordKey ReadNextKey()
        {
            return new RecordKey(base.ReadNext()[this._Header.PointerIndex]);
        }

        public override Record Read()
        {
            Record v = base.Read();
            RecordKey x = new RecordKey(v[this._Header.PointerIndex]);
            RecordKey y = base.PositionKey;
            return this._Parent.GetPage(x.PAGE_ID).Select(x.ROW_ID);
        }

    }

}
