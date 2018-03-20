using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;


namespace ArcticWind.Tables
{

    /// <summary>
    /// Represents a table whose only purpose is to temporarily store data
    /// </summary>
    public sealed class ShellTable : Table, IReadable
    {

        /// <summary>
        /// This method should be used for creating a table object from an existing table on disk
        /// </summary>
        /// <param name="Host"></param>
        /// <param name="Header"></param>
        /// <param name="ClusterKey"></param>
        public ShellTable(Host Host, TableHeader Header)
            : base(Host, Header)
        {
            this._TableType = "HEAP_SCRIBE";
        }

        /// <summary>
        /// This method should be used for creating a brand new table object
        /// </summary>
        /// <param name="Host"></param>
        /// <param name="Alias"></param>
        /// <param name="Dir"></param>
        /// <param name="Columns"></param>
        /// <param name="PageSize"></param>
        /// <param name="ClusterKey"></param>
        public ShellTable(Host Host, string Name, string Dir, Schema Columns, int PageSize)
            : base(Host, Name, Dir, Columns, PageSize)
        {
            this._TableType = "HEAP_SCRIBE";
        }

        public override void Insert(Record Value)
        {
            throw new NotImplementedException();
        }

        public override Table Partition(int PartitionIndex, int PartitionCount)
        {
            throw new NotImplementedException();
        }

        internal override void Dump(string Path)
        {

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Path))
            {
                
                int pageid = this.OriginPageID;
                while (pageid != -1)
                {
                    Page p = this.GetPage(pageid);
                    foreach (Record r in p.Elements)
                    {
                        sw.WriteLine(r);
                    }
                    pageid = p.NextPageID;
                }
                sw.Flush();

            }

        }

    }

}
