using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Expressions.TableExpressions;
using ArcticWind.Tables;

namespace ArcticWind.Expressions.RecordExpressions
{


    public sealed class RecordExpressionCTOR : RecordExpression
    {

        private Schema _cols;

        public RecordExpressionCTOR(Host Host, RecordExpression Parent, Schema Columns)
            : base(Host, Parent)
        {
            this._cols = Columns;
        }

        public override RecordExpression CloneOfMe()
        {
            return new RecordExpressionCTOR(this._Host, this._Parent, this._cols);
        }

        public override AssociativeRecord EvaluateAssociative(SpoolSpace Variants)
        {

            return new AssociativeRecord(this._cols);

        }

        public override Schema Columns
        {
            get
            {
                return this._cols;
            }
        }


    }


}
