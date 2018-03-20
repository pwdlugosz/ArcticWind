using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;


namespace ArcticWind.Tables
{

    /// <summary>
    /// The base class for all record writers
    /// </summary>
    public abstract class RecordWriter : IColumns, IDisposable
    {

        /// <summary>
        /// Gets the base table
        /// </summary>
        public abstract Table Source
        {
            get;
        }

        /// <summary>
        /// Gets the columns of the underlying data structure
        /// </summary>
        public virtual Schema Columns
        {
            get { return this.Source.Columns; }
        }

        /// <summary>
        /// Inserts a record into the stream
        /// </summary>
        /// <param name="Value"></param>
        public abstract void Insert(Record Value);

        /// <summary>
        /// Inserts many records into the stream
        /// </summary>
        /// <param name="Value"></param>
        public abstract void BulkInsert(IEnumerable<Record> Value);

        /// <summary>
        /// Gets the total number of writes this stream has made
        /// </summary>
        /// <returns></returns>
        public abstract long WriteCount();

        /// <summary>
        /// Closes the stream, releasing all resources; this calls the 'PreSerialize' method form the page table
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Reads all records form the readstream and writes to the Spike instance
        /// </summary>
        /// <param name="Reader"></param>
        public virtual void Consume(RecordReader Reader)
        {

            while (Reader.CanAdvance)
            {
                this.Insert(Reader.ReadNext());
            }

        }

        // Interface //
        /// <summary>
        /// Disposes of the object
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }

    }

}
