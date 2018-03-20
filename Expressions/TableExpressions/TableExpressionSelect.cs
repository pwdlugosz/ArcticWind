using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.Aggregates;
using ArcticWind.Tables;
using ArcticWind.Expressions;
using ArcticWind.Expressions.RecordExpressions;

namespace ArcticWind.Expressions.TableExpressions
{

    /// <summary>
    /// Represents a table  expression for a basic select statement
    /// </summary>
    public sealed class TableExpressionSelect : TableExpression
    {

        private ScalarExpressionSet _Fields;
        private Filter _Where;
        private string _SpoolSpace;
        private string _RecordName;
        private long _Limit;
        private bool _Trigger = false;

        public TableExpressionSelect(Host Host, TableExpression Parent, ScalarExpressionSet Fields, Filter Where, string NameSpace, string Alias, long Limit)
            : base(Host, Parent)
        {
            this._Fields = Fields;
            this._Where = Where;
            this.Alias = "SELECT";
            this._Limit = Limit;
            this._SpoolSpace = NameSpace;
            this._RecordName = Alias;
        }

        public TableExpressionSelect(Host Host, TableExpression Parent, ScalarExpressionSet Fields, Filter Where, string NameSpace, string Alias)
            : this(Host, Parent, Fields, Where, NameSpace, Alias, -1)
        {
        }

        /// <summary>
        /// The limit of records to return
        /// </summary>
        public long Limit
        {
            get { return this._Limit; }
        }

        /// <summary>
        /// Gets the underlying columns
        /// </summary>
        public override Schema Columns
        {
            get { return this._Fields.Columns; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Variants"></param>
        /// <returns></returns>
        public override SpoolSpace CreateResolver(SpoolSpace Variants)
        {
            SpoolSpace x = Variants;
            return x;
        }

        /// <summary>
        /// Gets the seleced fields
        /// </summary>
        public ScalarExpressionSet Fields
        {
            get { return this._Fields; }
        }

        /// <summary>
        /// Gets the filter
        /// </summary>
        public Filter Where
        {
            get { return this._Where; }
        }

        /// <summary>
        /// Renders the expression
        /// </summary>
        /// <param name="Writer"></param>
        public override void Evaluate(SpoolSpace Variants, RecordWriter Writer)
        {

            // Render the base table //
            Table t = this.Children[0].Select(Variants);

            // Open the reader //
            RecordReader rs = t.OpenReader();

            // Ticks //
            long ticks = 0;

            // Initialize //
            this.InitializeResolver(Variants);

            // Main read loop //
            while (rs.CanAdvance)
            {

                if (this._Limit >= ticks)
                    break;

                Variants[this._SpoolSpace].SetRecord(this._RecordName, new AssociativeRecord(t.Columns, rs.ReadNext()));
                
                if (Where.Evaluate(Variants))
                {
                    Writer.Insert(Fields.Evaluate(Variants));
                }

                ticks++;

            }

            // Clean up //
            if (this._Host.IsSystemTemp(t))
                this._Host.TableStore.DropTable(t.Key);

            // Fix Resolver //
            this.CleanUpResolver(Variants);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Variants"></param>
        public override void InitializeResolver(SpoolSpace Variants)
        {

            if (!Variants.Exists(this._SpoolSpace))
                Variants.Add(this._SpoolSpace, new Spool(this._Host, this._SpoolSpace));
            
            // Fix the resolver //
            if (!Variants[this._SpoolSpace].RecordExists(this._RecordName))
            {
                Variants[this._SpoolSpace].SetRecord(this._RecordName, new AssociativeRecord(this._Children[0].Columns));
                this._Trigger = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Variants"></param>
        public override void CleanUpResolver(SpoolSpace Variants)
        {
            // Fix the resolver //
            if (Variants[this._SpoolSpace].RecordExists(this._RecordName) && this._Trigger)
            {
                this._Trigger = false;
                Variants[this._SpoolSpace].DeleteRecord(this._RecordName);
            }
        }

        /// <summary>
        /// Gets the estimated record count
        /// </summary>
        public override long EstimatedCount
        {
            get
            {
                return this._Children.Max((x) => { return x.EstimatedCount; });
            }
        }

    }



}
