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


    public class AggregateVarS : Aggregate
    {

        protected ScalarExpression _Value;
        protected ScalarExpression _Weight;

        public AggregateVarS(ScalarExpression Value, ScalarExpression Weight, Filter Filter)
            : base(Filter)
        {
            this._Value = Value;
            this._Weight = Weight;
        }

        public AggregateVarS(ScalarExpression Value, Filter Filter)
            : this(Value, ScalarExpression.OneNUM, Filter)
        {
        }

        public AggregateVarS(ScalarExpression Value, ScalarExpression Weight)
            : this(Value, Weight, Filter.TrueForAll)
        {
        }

        public AggregateVarS(ScalarExpression Value)
            : this(Value, ScalarExpression.OneNUM, Filter.TrueForAll)
        {
        }

        public override int SignitureLength()
        {
            return 4;
        }

        public override Schema WorkSchema()
        {
            return new Schema("A.DOUBLE, B.DOUBLE, C.DOUBLE, D.DOUBLE");
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
            return new AggregateVarS(this._Value.CloneOfMe(), this._Weight.CloneOfMe(), this._Filter.CloneOfMe());
        }

        public override void Initialize(Record Work, int Offset)
        {
            Work[Offset] = new Cell(0D);        // COUNT OriginalPage
            Work[Offset + 1] = new Cell(0D);    // SUM OriginalPage
            Work[Offset + 2] = new Cell(0D);    // SUM OriginalPage * OriginalPage
            Work[Offset + 3] = new Cell(0D);    // SUM 1
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
            Work[Offset + 2] += x * x;
            if (!w.IsZero) Work[Offset + 3]++;

        }

        public override void Merge(Record MergeIntoWork, Record Work, int Offset)
        {

            MergeIntoWork[Offset] += Work[Offset];
            MergeIntoWork[Offset + 1] += Work[Offset + 1];
            MergeIntoWork[Offset + 2] += Work[Offset + 2];
            MergeIntoWork[Offset + 3] += Work[Offset + 3];

        }

        public override Cell Evaluate(Record Work, int Offset)
        {

            if (Work[Offset].valueDOUBLE == 0)
                return CellValues.NullDOUBLE;
            Cell x = Work[Offset + 1] / Work[Offset];
            Cell y = Work[Offset + 2] / Work[Offset];
            Cell z = Work[Offset + 3] / (Work[Offset + 3] - new Cell(1D));
            return (y - x * x) * z;

        }

    }


}
