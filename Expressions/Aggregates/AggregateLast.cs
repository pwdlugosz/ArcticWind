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


    public class AggregateLast : Aggregate
    {

        private ScalarExpression _Value;

        public AggregateLast(ScalarExpression Value, Filter Filter)
            : base(Filter)
        {
            this._Value = Value;
        }

        public AggregateLast(ScalarExpression Value)
            : this(Value, Filter.TrueForAll)
        {
        }

        public override int SignitureLength()
        {
            return 1;
        }

        public override Schema WorkSchema()
        {
            Schema s = new Schema();
            s.Add("A", this.AggregateAffinity(), this.AggregateSize());
            return s;
        }

        public override CellAffinity AggregateAffinity()
        {
            return this._Value.ReturnAffinity();
        }

        public override int AggregateSize()
        {
            return this._Value.ReturnSize();
        }

        public override Aggregate CloneOfMe()
        {
            return new AggregateLast(this._Value.CloneOfMe(), this._Filter.CloneOfMe());
        }

        public override void Initialize(Record Work, int Offset)
        {
            Work[Offset] = new Cell(this.AggregateAffinity());
        }

        public override void Accumulate(SpoolSpace Variants, Record Work, int Offset)
        {

            if (!this._Filter.Evaluate(Variants))
                return;

            Cell x = this._Value.Evaluate(Variants);
            Work[Offset] = x;

        }

        public override void Merge(Record MergeIntoWork, Record Work, int Offset)
        {

            MergeIntoWork[Offset] = Work[Offset];

        }

        public override Cell Evaluate(Record Work, int Offset)
        {
            return Work[Offset];
        }

    }


}
