using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;


namespace ArcticWind.Tables
{

    /// <summary>
    /// Reads the record keys from an index
    /// </summary>
    public class RecordReaderIndexKey : RecordReaderBase
    {

        private IndexHeader _Header;

        public RecordReaderIndexKey(IndexHeader Header, Table Storage, RecordKey LKey, RecordKey RKey)
            : base(Storage, LKey, RKey)
        {
            this._Header = Header;
        }

        public RecordReaderIndexKey(IndexHeader Header, Table Storage)
            : this(Header, Storage, RecordReaderBase.OriginKey(Storage, Header), RecordReaderBase.TerminalKey(Storage, Header))
        {
        }

        public RecordKey ReadKey()
        {
            return new RecordKey(this.Read()[this._Header.PointerIndex]);
        }

        public RecordKey ReadNextKey()
        {
            return new RecordKey(this.ReadNext()[this._Header.PointerIndex]);
        }

    }


}
