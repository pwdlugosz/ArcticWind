using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;

namespace ArcticWind.Tables
{

    /// <summary>
    /// Represents a scribe table that keeps records in an unordered linked list
    /// </summary>
    public class HeapTable : Table, IReadable
    {

        protected Page _Terminis;
        private RecordKey _LastKey;

        /// <summary>
        /// This method should be used for creating a table object from an existing table on disk
        /// </summary>
        /// <param name="Host"></param>
        /// <param name="Header"></param>
        /// <param name="ClusterKey"></param>
        public HeapTable(Host Host, TableHeader Header)
            : base(Host, Header)
        {
            this._Terminis = this.GetPage(this._Header.TerminalPageID);
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
        public HeapTable(Host Host, string Name, string Dir, Schema Columns, int PageSize)
            : base(Host, Name, Dir, Columns, PageSize)
        {
            this._Header.OriginPageID = 0;
            this._Header.TerminalPageID = 0;
            this._Terminis = new Page(PageSize, 0, -1, -1, Columns.Count, 0);
            this._Terminis.Version++;
            this.SetPage(this._Terminis);
            this._Header.PageCount++;
            this._TableType = "HEAP_SCRIBE";
        }

        public override Page TerminalPage
        {
            get
            {
                return this._Terminis;
            }
        }

        public override int TerminalPageID
        {
            get
            {
                return this._Terminis.PageID;
            }
        }

        public RecordKey LastInsertKey
        {
            get { return this._LastKey; }
        }

        public override void Insert(Record Value)
        {

            if (this.State == TableState.ReadOnly || this.State == TableState.FullLock)
                throw new Exception("Table is locked for writing");

            // Handle the terminal page being full //
            if (!this._Terminis.CanAccept(Value))
            {

                Page p = new Page(this.PageSize, this.GenerateNewPageID, this._Terminis.PageID, -1, this.Columns.Count, 0);
                p.Version++;
                this._Header.PageCount++;
                this._Terminis.NextPageID = p.PageID;
                this.SetPage(p);
                this._Terminis = p;
                this._Header.TerminalPageID = p.PageID;

            }

            // Add the actual record //
            Value = this._Header.Columns.Fix(Value);
            this._Terminis.Insert(Value);
            this.RecordCount++;

            // Get the pointer //
            this._LastKey = new RecordKey(this._Terminis.PageID, this._Terminis.Count - 1);
            this._Indexes.Insert(Value, this._LastKey);

            // Step the version //
            this._Version++;

        }

        public override Table Partition(int PartitionIndex, int PartitionCount)
        {

            throw new NotImplementedException();

        }

        public override void PreSerialize()
        {
            if (this._Terminis != null)
                this.SetPage(this._Terminis);
        }

    }

}
