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


    public class AggregateMinOf : Aggregate
    {

        private ScalarExpression _Key;
        private ScalarExpression _Value;

        public AggregateMinOf(ScalarExpression Key, ScalarExpression Value, Filter Filter)
            : base(Filter)
        {
            this._Key = Key;
            this._Value = Value;
        }

        public AggregateMinOf(ScalarExpression Key, ScalarExpression Value)
            : this(Key, Value, Filter.TrueForAll)
        {
        }

        public override int SignitureLength()
        {
            return 2;
        }
        
        public override Schema WorkSchema()
        {
            Schema s = new Schema();
            s.Add("A", this._Key.ReturnAffinity(), this._Key.ReturnSize());
            s.Add("B", this._Value.ReturnAffinity(), this._Value.ReturnSize());
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
            return new AggregateMinOf(this._Key.CloneOfMe(), this._Value.CloneOfMe(), this._Filter.CloneOfMe());
        }

        public override void Initialize(Record Work, int Offset)
        {
            Work[Offset] = CellValues.Max(this._Key.ReturnAffinity());
            Work[Offset + 1] = new Cell(this.AggregateAffinity());
        }

        public override void Accumulate(SpoolSpace Variants, Record Work, int Offset)
        {

            if (!this._Filter.Evaluate(Variants))
                return;

            Cell k = this._Key.Evaluate(Variants);
            Cell v = this._Value.Evaluate(Variants);

            if (k.IsNull)
                return;

            if (Work[Offset].IsNull)
            {
                Work[Offset] = v;
            }
            else if (Work[Offset] > k)
            {
                Work[Offset] = v;
            }

        }

        public override void Merge(Record MergeIntoWork, Record Work, int Offset)
        {

            if (MergeIntoWork[Offset].IsNull)
            {
                MergeIntoWork[Offset] = Work[Offset];
            }
            else if (Work[Offset].IsNull)
            {
                return;
            }
            else if (MergeIntoWork[Offset] > Work[Offset])
            {
                MergeIntoWork[Offset] = Work[Offset];
                MergeIntoWork[Offset + 1] = Work[Offset + 1];
            }

        }

        public override Cell Evaluate(Record Work, int Offset)
        {
            return Work[Offset + 1];
        }

    }


}
