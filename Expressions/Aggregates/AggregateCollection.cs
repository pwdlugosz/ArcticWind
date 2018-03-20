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

    /// <summary>
    /// Represents a collection of aggregates
    /// </summary>
    public sealed class AggregateCollection : IColumns
    {

        private Heap<Aggregate> _Aggregates;
        
        public AggregateCollection()
        {
            this._Aggregates = new Heap<Aggregate>();
        }

        /// <summary>
        /// Represents the count of columns in the aggregate
        /// </summary>
        public int Count
        {
            get { return this._Aggregates.Count; }
        }

        /// <summary>
        /// Represents the total signiture size; this is the size of the work data record
        /// </summary>
        /// <returns></returns>
        public int SignitureLength()
        {
            return this._Aggregates.Values.Sum((x) => { return x.SignitureLength(); });
        }

        /// <summary>
        /// Gets the aggregate at a given index
        /// </summary>
        /// <param name="IndexOf"></param>
        /// <returns></returns>
        public Aggregate this[int IndexOf]
        {
            get { return this._Aggregates[IndexOf]; }
        }

        /// <summary>
        /// Gets an aggregate given an alias
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public Aggregate this[string Name]
        {
            get { return this._Aggregates[Name]; }
        }

        /// <summary>
        /// Gets all aggregates as a collection
        /// </summary>
        public IEnumerable<Aggregate> Aggregates
        {
            get { return this._Aggregates.Values; }
        }

        /// <summary>
        /// Gets the columns associated
        /// </summary>
        public Schema Columns
        {
            get
            {

                Schema s = new Schema();
                for (int i = 0; i < this._Aggregates.Count; i++)
                {
                    s.Add(this._Aggregates.Name(i), this._Aggregates[i].AggregateAffinity(), this._Aggregates[i].AggregateSize());
                }
                return s;

            }
        }

        /// <summary>
        /// Gets the columns for all the work data
        /// </summary>
        public Schema WorkColumns
        {
            get
            {

                Schema s = new Schema();

                // For each aggregatge //
                for (int i = 0; i < this.Count; i++)
                {

                    // For each work-data element in aggregate //
                    Schema w = this._Aggregates[i].WorkSchema();

                    for (int j = 0; j < w.Count; j++)
                    {

                        string name = "A" + i.ToString() + "_W" + j.ToString();
                        s.Add(name, w.ColumnAffinity(j), w.ColumnSize(j));

                    }


                }

                return s;

            }
        }

        /// <summary>
        /// Adds a given aggregate to the collection
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Value"></param>
        public void Add(string Alias, Aggregate Value)
        {
            Value.AggregateCollectionOffset = this._Aggregates.Values.Sum((x) => { return x.SignitureLength(); });
            this._Aggregates.Allocate(Alias, Value);
        }

        /// <summary>
        /// Sets up the values in a work-data record
        /// </summary>
        /// <param name="Variants"></param>
        /// <param name="Work"></param>
        /// <param name="Offset"></param>
        public void Initialize(Record Work, int Offset)
        {

            foreach (Aggregate a in this._Aggregates.Values)
            {
                a.Initialize(Work, Offset + a.AggregateCollectionOffset);
            }

        }

        /// <summary>
        /// Performs a single accumulation over a single record
        /// </summary>
        /// <param name="Variants"></param>
        /// <param name="Work"></param>
        /// <param name="Offset"></param>
        public void Accumulate(SpoolSpace Variants, Record Work, int Offset)
        {

            foreach (Aggregate a in this._Aggregates.Values)
            {
                a.Accumulate(Variants, Work, Offset + a.AggregateCollectionOffset);
            }

        }

        /// <summary>
        /// Merges two work data records; this is only used during parallel processing
        /// </summary>
        /// <param name="MergeIntoWork"></param>
        /// <param name="Work"></param>
        /// <param name="Offset"></param>
        public void Merge(Record MergeIntoWork, Record Work, int Offset)
        {

            foreach (Aggregate a in this._Aggregates.Values)
            {
                a.Merge(MergeIntoWork, Work, Offset + a.AggregateCollectionOffset);
            }

        }

        /// <summary>
        /// Finalizes the work data return the final cell result
        /// </summary>
        /// <param name="Work"></param>
        /// <param name="Offset"></param>
        /// <returns></returns>
        public Record Evaluate(Record Work, int Offset)
        {

            Cell[] c = new Cell[this.Count];
            int i = 0;
            foreach (Aggregate a in this._Aggregates.Values)
            {
                c[i] = a.Evaluate(Work, a.AggregateCollectionOffset + Offset);
                i++;
            }
            return new Record(c);

        }

    }


}
