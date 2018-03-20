using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Tables;
using ArcticWind.Expressions;

namespace ArcticWind.Expressions.Aggregates
{
    
    public abstract class Aggregate
    {

        protected Filter _Filter;

        public Aggregate(Filter Filter)
        {
            this._Filter = Filter;
        }

        /// <summary>
        /// The base filter
        /// </summary>
        public Filter InnerFilter
        {
            get { return this._Filter; }
        }

        /// <summary>
        /// When in a collection, represents the work record offset
        /// </summary>
        public int AggregateCollectionOffset
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the number of cells needed in a work-data record
        /// </summary>
        /// <returns></returns>
        public abstract int SignitureLength();

        /// <summary>
        /// Gets the size in bytes of the data returned
        /// </summary>
        /// <param name="Variants"></param>
        /// <returns></returns>
        public abstract int AggregateSize();

        /// <summary>
        /// Gets the affinity of the value returned
        /// </summary>
        /// <param name="Variants"></param>
        /// <returns></returns>
        public abstract CellAffinity AggregateAffinity();

        /// <summary>
        /// Gets a schema describing the work data elements
        /// </summary>
        /// <returns></returns>
        public abstract Schema WorkSchema();

        /// <summary>
        /// Returns a clone of this aggregate
        /// </summary>
        /// <returns></returns>
        public abstract Aggregate CloneOfMe();

        /// <summary>
        /// Sets up the values in a work-data record
        /// </summary>
        /// <param name="Variants"></param>
        /// <param name="Work"></param>
        /// <param name="Offset"></param>
        public abstract void Initialize(Record Work, int Offset);

        /// <summary>
        /// Performs a single accumulation over a single record
        /// </summary>
        /// <param name="Variants"></param>
        /// <param name="Work"></param>
        /// <param name="Offset"></param>
        public abstract void Accumulate(SpoolSpace Variants, Record Work, int Offset);

        /// <summary>
        /// Merges two work data records; this is only used during parallel processing
        /// </summary>
        /// <param name="MergeIntoWork"></param>
        /// <param name="Work"></param>
        /// <param name="Offset"></param>
        public abstract void Merge(Record MergeIntoWork, Record Work, int Offset);

        /// <summary>
        /// Finalizes the work data return the final cell result
        /// </summary>
        /// <param name="Work"></param>
        /// <param name="Offset"></param>
        /// <returns></returns>
        public abstract Cell Evaluate(Record Work, int Offset);

    }

}
