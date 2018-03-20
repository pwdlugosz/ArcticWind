using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;


namespace ArcticWind.Tables
{

    /// <summary>
    /// A vanilla write stream
    /// </summary>
    public class RecordWriterBase : RecordWriter
    {

        protected Table _Parent;
        protected long _Ticks = 0;

        public RecordWriterBase(Table Data)
            : base()
        {
            this._Parent = Data;
        }

        public override Table Source
        {
            get { return this._Parent; }
        }

        public override void Close()
        {
            // do nothing
        }

        public override void Insert(Record Value)
        {
            this._Parent.Insert(Value);
        }

        public override void BulkInsert(IEnumerable<Record> Value)
        {
            this._Parent.Insert(Value);
        }

        public override long WriteCount()
        {
            return this._Ticks;
        }

    }

}
