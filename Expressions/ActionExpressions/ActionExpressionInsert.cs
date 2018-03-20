using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.TableExpressions;
using ArcticWind.Tables;
using ArcticWind.Expressions;
using ArcticWind.Expressions.RecordExpressions;

namespace ArcticWind.Expressions.ActionExpressions
{

    public sealed class ActionExpressionInsert : ActionExpression
    {

        private RecordWriter _Writer;
        private RecordExpression _Fields;

        public ActionExpressionInsert(Host Host, ActionExpression Parent, RecordWriter Writer, RecordExpression Fields)
            : base(Host, Parent)
        {
            this._Writer = Writer;
            this._Fields = Fields;
        }

        public override void Invoke(SpoolSpace Variant)
        {

            Record r = this._Fields.Evaluate(Variant);
            this._Writer.Insert(r);

        }

        public override void EndInvoke(SpoolSpace Variant)
        {
            this._Writer.Close();
        }

    }

}
