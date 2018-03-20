using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;


namespace ArcticWind.Tables
{

    /// <summary>
    /// The base class for all record readers
    /// </summary>
    public abstract class RecordReader : IDisposable, IColumns
    {

        /// <summary>
        /// True if on the first record of the stream
        /// </summary>
        public abstract bool IsFirst { get; }

        /// <summary>
        /// True if on the last record of the stream
        /// </summary>
        public abstract bool IsLast { get; }

        /// <summary>
        /// True if the stream can advance
        /// </summary>
        public abstract bool CanAdvance
        {
            get;
        }

        /// <summary>
        /// True if the stream can revert
        /// </summary>
        public abstract bool CanRevert
        {
            get;
        }

        /// <summary>
        /// Gets the columns of the underlying data structure
        /// </summary>
        public abstract Schema Columns
        {
            get;
        }

        /// <summary>
        /// Reads a record without advancing the stream
        /// </summary>
        /// <returns></returns>
        public abstract Record Read();

        /// <summary>
        /// Reads a record and advances the stream
        /// </summary>
        /// <returns></returns>
        public abstract Record ReadNext();

        /// <summary>
        /// Advances the stream one unit forward
        /// </summary>
        public abstract void Advance();

        /// <summary>
        /// Advances the stream up to N units forward; this will stop advancing if the end of the stream is reached
        /// </summary>
        /// <param name="Itterations"></param>
        public abstract void Advance(int Itterations);

        /// <summary>
        /// Reverts the stream back one unit
        /// </summary>
        public abstract void Revert();

        /// <summary>
        /// Reverts the stream back up to N units; this will stop reverting if the begining of the stream is reached
        /// </summary>
        /// <param name="Itterations"></param>
        public abstract void Revert(int Itterations);

        /// <summary>
        /// The position on the page of the Spike record
        /// </summary>
        /// <returns></returns>
        public abstract int RecordID();

        /// <summary>
        /// The ID of the Spike page being read
        /// </summary>
        /// <returns></returns>
        public abstract int PageID();

        /// <summary>
        /// The position, in terms of records read, of the Spike stream
        /// </summary>
        /// <returns></returns>
        public abstract long Position();

        /// <summary>
        /// Resets the stream to the origin
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// The Spike position of the stream expressed as a record key
        /// </summary>
        public RecordKey PositionKey
        {
            get { return new RecordKey(this.PageID(), this.RecordID()); }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Dispose()
        {
        }

    }



}
