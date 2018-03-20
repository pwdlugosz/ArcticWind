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


    public class AggregateAvg : Aggregate
    {

        private ScalarExpression _Value;
        private ScalarExpression _Weight;

        public AggregateAvg(ScalarExpression Value, ScalarExpression Weight, Filter Filter)
            : base(Filter)
        {
            this._Value = Value;
            this._Weight = Weight;
        }

        public AggregateAvg(ScalarExpression Value, Filter Filter)
            : this(Value, ScalarExpression.OneNUM, Filter)
        {
        }

        public AggregateAvg(ScalarExpression Value, ScalarExpression Weight)
            : this(Value, Weight, Filter.TrueForAll)
        {
        }

        public AggregateAvg(ScalarExpression Value)
            : this(Value, ScalarExpression.OneNUM, Filter.TrueForAll)
        {
        }

        public override int SignitureLength()
        {
            return 2;
        }
        
        public override Schema WorkSchema()
        {
            return new Schema("A.DOUBLE, B.DOUBLE");
        }

        public override CellAffinity AggregateAffinity()
        {
            return CellAffinity.DOUBLE;
        }

        public override int AggregateSize()
        {
            return CellSerializer.FixLength(CellAffinity.DOUBLE, 0);
        }

        public override Aggregate CloneOfMe()
        {
            return new AggregateAvg(this._Value.CloneOfMe(), this._Weight.CloneOfMe(), this._Filter.CloneOfMe());
        }

        public override void Initialize(Record Work, int Offset)
        {
            Work[Offset] = new Cell(0D);        // COUNT OriginalPage
            Work[Offset + 1] = new Cell(0D);    // SUM OriginalPage
        }

        public override void Accumulate(SpoolSpace Variants, Record Work, int Offset)
        {

            if (!this._Filter.Evaluate(Variants))
                return;

            Cell x = this._Value.Evaluate(Variants);
            Cell w = this._Weight.Evaluate(Variants);

            if (x.IsNull || w.IsNull || !(CellAffinityHelper.IsNumeric(x.AFFINITY) && CellAffinityHelper.IsNumeric(w.AFFINITY)))
                return;

            Work[Offset] += w;
            Work[Offset + 1] += x;

        }

        public override void Merge(Record MergeIntoWork, Record Work, int Offset)
        {

            MergeIntoWork[Offset] += Work[Offset];
            MergeIntoWork[Offset + 1] += Work[Offset + 1];

        }

        public override Cell Evaluate(Record Work, int Offset)
        {

            if (Work[Offset].valueDOUBLE == 0)
                return CellValues.NullDOUBLE;
            return Work[Offset + 1] / Work[Offset];

        }

    }


}
