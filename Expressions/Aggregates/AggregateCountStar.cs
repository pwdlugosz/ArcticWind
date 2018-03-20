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


    public class AggregateCountStar : Aggregate
    {

        public AggregateCountStar(Filter Filter)
            : base(Filter)
        {
        }

        public override int SignitureLength()
        {
            return 1;
        }

        public override Schema WorkSchema()
        {
            return new Schema("A.LONG");
        }

        public override CellAffinity AggregateAffinity()
        {
            return CellAffinity.LONG;
        }

        public override int AggregateSize()
        {
            return CellSerializer.FixLength(CellAffinity.LONG, 0);
        }

        public override Aggregate CloneOfMe()
        {
            return new AggregateCountStar(this._Filter.CloneOfMe());
        }

        public override void Initialize(Record Work, int Offset)
        {
            Work[Offset] = CellValues.NullLONG;
        }

        public override void Accumulate(SpoolSpace Variants, Record Work, int Offset)
        {

            if (!this._Filter.Evaluate(Variants))
                return;

            if (Work[Offset].IsNull)
                Work[Offset] = new Cell(1);
            else
                Work[Offset]++;

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
            else
            {
                MergeIntoWork[Offset] += Work[Offset];
            }

        }

        public override Cell Evaluate(Record Work, int Offset)
        {
            return Work[Offset];
        }

    }


}
