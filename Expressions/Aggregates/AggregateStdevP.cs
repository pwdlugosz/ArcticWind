using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;

namespace ArcticWind.Expressions.Aggregates
{


    public class AggregateStdevP : AggregateVarP
    {

        public AggregateStdevP(ScalarExpression Value, ScalarExpression Weight, Filter Filter)
            : base(Value, Weight, Filter)
        {
        }

        public AggregateStdevP(ScalarExpression Value, Filter Filter)
            : this(Value, ScalarExpression.OneNUM, Filter)
        {
        }

        public AggregateStdevP(ScalarExpression Value, ScalarExpression Weight)
            : this(Value, Weight, Filter.TrueForAll)
        {
        }

        public AggregateStdevP(ScalarExpression Value)
            : this(Value, ScalarExpression.OneNUM, Filter.TrueForAll)
        {
        }

        public override Aggregate CloneOfMe()
        {
            return new AggregateStdevP(this._Value.CloneOfMe(), this._Weight.CloneOfMe(), this._Filter.CloneOfMe());
        }

        public override Cell Evaluate(Record Work, int Offset)
        {

            if (Work[Offset].valueDOUBLE == 0)
                return CellValues.NullDOUBLE;
            Cell x = Work[Offset + 1] / Work[Offset];
            Cell y = Work[Offset + 2] / Work[Offset];
            return CellFunctions.Sqrt(y - x * x);

        }

    }


}
