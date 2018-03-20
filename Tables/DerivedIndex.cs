using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;

namespace ArcticWind.Tables
{

    /// <summary>
    /// Represents an index derived from a clustered table
    /// </summary>
    public class DerivedIndex : Index
    {

        public DerivedIndex(ClusteredTable Table)
            : base()
        {

            this._Storage = Table;
            this._Parent = Table;
            this._Header = new IndexHeader(Table.Name, -1, -1, -1, 0, 0, Table.BaseTree.IndexColumns);
            this._IndexColumns = Table.BaseTree.IndexColumns;
            this._Tree = Table.BaseTree;

        }

        // Methods //
        public override void Insert(Record Element, RecordKey Key)
        {
            // Note: key is ignored here
            this._Parent.Insert(Element);
        }

        public override RecordReader OpenReader()
        {
            return this._Parent.OpenReader();
        }

        public override RecordReader OpenReader(Record Key)
        {
            return this._Parent.OpenReader(Key);
        }

        public override RecordReader OpenReader(Record LKey, Record UKey)
        {
            return this._Parent.OpenReader(LKey, UKey);
        }

        public override void Calibrate()
        {

            throw new Exception("Cannot calibrate a derived index");

        }

    }

    


}
