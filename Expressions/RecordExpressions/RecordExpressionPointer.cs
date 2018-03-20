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


    public sealed class RecordExpressionPointer : RecordExpression
    {

        private string _Name;
        private Schema _Columns;

        public RecordExpressionPointer(Host Host, RecordExpression Parent, string Name, Schema Columns)
            : base(Host, Parent)
        {
            this._Name = Name;
            this._Columns = Columns;
        }

        public override Schema Columns
        {
            get { return this._Columns; }
        }

        public override Record Evaluate(SpoolSpace Variants)
        {
            throw new Exception("Cannot evaluate pointer expressions");
        }

        public override AssociativeRecord EvaluateAssociative(SpoolSpace Variants)
        {
            throw new Exception("Cannot evaluate pointer expressions");
        }

        public override RecordExpression CloneOfMe()
        {
            return new RecordExpressionPointer(this._Host, this._Parent, this._Name, this._Columns);
        }

    }


}
