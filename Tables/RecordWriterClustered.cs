using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;


namespace ArcticWind.Tables
{

    /// <summary>
    /// Writes the data to a temp table that uses a clustered index, then selects the data into the destination table
    /// </summary>
    public class RecordWriterClustered : RecordWriterBase
    {

        private Table _t;

        public RecordWriterClustered(Table Data, Key OrderedColumns)
            : base(Data)
        {
            this._t = Data.Host.CreateTable(Host.TEMP, Host.RandomName, Data.Columns, OrderedColumns, ClusterState.Universal);
        }

        public override void Insert(Record Value)
        {
            this._t.Insert(Value);
        }

        public override void Close()
        {

            // Open a writer over the parent
            RecordWriter ws = this._Parent.OpenWriter();

            // Open a reader over the cluster table 
            RecordReader rs = this._t.OpenReader();

            // Consume the reader / close the writer
            ws.Consume(rs);
            ws.Close();

            // Drop the table //
            Host h = this._t.Host;
            h.TableStore.DropTable(this._t.Key);

        }

    }

}
