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

    public class AggregateCovariance : Aggregate
    {

        protected ScalarExpression _ValueX;
        protected ScalarExpression _ValueY;
        protected ScalarExpression _Weight;

        public AggregateCovariance(ScalarExpression ValueX, ScalarExpression ValueY, ScalarExpression Weight, Filter Filter)
            : base(Filter)
        {
            this._ValueX = ValueX;
            this._ValueY = ValueY;
            this._Weight = Weight;
        }

        public override int SignitureLength()
        {
            return 6;
        }

        public override Schema WorkSchema()
        {
            return new Schema("A.DOUBLE, B.DOUBLE, C.DOUBLE, D.DOUBLE, E.DOUBLE, F.DOUBLE");
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
            return new AggregateCovariance(this._ValueX.CloneOfMe(), this._ValueY.CloneOfMe(), this._Weight.CloneOfMe(), this._Filter.CloneOfMe());
        }

        public override void Initialize(Record Work, int Offset)
        {
            Work[Offset] = new Cell(0D);        // SUM w
            Work[Offset + 1] = new Cell(0D);    // SUM OriginalPage
            Work[Offset + 2] = new Cell(0D);    // SUM OriginalPage * OriginalPage
            Work[Offset + 3] = new Cell(0D);    // SUM NewPage
            Work[Offset + 4] = new Cell(0D);    // SUM NewPage * NewPage
            Work[Offset + 5] = new Cell(0D);    // SUM OriginalPage * NewPage
        }

        public override void Accumulate(SpoolSpace Variants, Record Work, int Offset)
        {

            if (!this._Filter.Evaluate(Variants))
                return;

            Cell x = this._ValueX.Evaluate(Variants);
            Cell y = this._ValueY.Evaluate(Variants);
            Cell w = this._Weight.Evaluate(Variants);

            if (x.IsNull || y.IsNull || w.IsNull || !(CellAffinityHelper.IsNumeric(x.AFFINITY) && CellAffinityHelper.IsNumeric(y.AFFINITY) && CellAffinityHelper.IsNumeric(w.AFFINITY)))
                return;

            Work[Offset] += w;
            Work[Offset + 1] += x;
            Work[Offset + 2] += x * x;
            Work[Offset + 3] += y;
            Work[Offset + 4] += y * y;
            Work[Offset + 5] += x * y;

        }

        public override void Merge(Record MergeIntoWork, Record Work, int Offset)
        {

            MergeIntoWork[Offset] += Work[Offset];
            MergeIntoWork[Offset + 1] += Work[Offset + 1];
            MergeIntoWork[Offset + 2] += Work[Offset + 2];

        }

        public override Cell Evaluate(Record Work, int Offset)
        {

            if (Work[Offset].valueDOUBLE == 0)
                return CellValues.NullDOUBLE;
            Cell x = Work[Offset + 1] / Work[Offset];
            Cell x2 = Work[Offset + 2] / Work[Offset] - x * x;
            Cell y = Work[Offset + 3] / Work[Offset];
            Cell y2 = Work[Offset + 4] / Work[Offset] - y * y;
            Cell xy = Work[Offset + 5] / Work[Offset] - x * y;
            return xy;

        }

    }

}
