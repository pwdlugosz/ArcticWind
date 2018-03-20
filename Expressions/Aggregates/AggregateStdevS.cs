using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;

namespace ArcticWind.Expressions.Aggregates
{


    public class AggregateStdevS : AggregateVarS
    {

        public AggregateStdevS(ScalarExpression Value, ScalarExpression Weight, Filter Filter)
            : base(Value, Weight, Filter)
        {
        }

        public AggregateStdevS(ScalarExpression Value, Filter Filter)
            : this(Value, ScalarExpression.OneNUM, Filter)
        {
        }

        public AggregateStdevS(ScalarExpression Value, ScalarExpression Weight)
            : this(Value, Weight, Filter.TrueForAll)
        {
        }

        public AggregateStdevS(ScalarExpression Value)
            : this(Value, ScalarExpression.OneNUM, Filter.TrueForAll)
        {
        }

        public override Aggregate CloneOfMe()
        {
            return new AggregateStdevS(this._Value.CloneOfMe(), this._Weight.CloneOfMe(), this._Filter.CloneOfMe());
        }

        public override Cell Evaluate(Record Work, int Offset)
        {

            if (Work[Offset].valueDOUBLE == 0)
                return CellValues.NullDOUBLE;
            Cell x = Work[Offset + 1] / Work[Offset];
            Cell y = Work[Offset + 2] / Work[Offset];
            Cell z = Work[Offset + 3] / (Work[Offset + 3] - new Cell(1D));
            return CellFunctions.Sqrt((y - x * x) * z);

        }

    }


}
