using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind;
using ArcticWind.Expressions;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Expressions.TableExpressions;
using ArcticWind.Expressions.ActionExpressions;
using ArcticWind.Tables;
using ArcticWind.Elements;

namespace ArcticWind.Expressions.ActionExpressions
{

    public sealed class ActionExpressionInvokeVoid : ActionExpressionParameterized
    {

        private AbstractMethod _Method;

        public ActionExpressionInvokeVoid(Host Host, ActionExpression Parent, AbstractMethod Method)
            : base(Host, Parent, Method.Name, Method.ParameterCount)
        {
            this._Method = Method;
        }

        public override void AddParameter(Parameter Value)
        {
            this._Method.AddParameter(Value);
            base.AddParameter(Value);
        }

        public override void BeginInvoke(SpoolSpace Variant)
        {
            // Do nothing
        }

        public override void Invoke(SpoolSpace Variant)
        {
            SpoolSpace s = this._Method.MemorySpace(Variant);
            this._Method.BindParameters(s);
            this._Method.Invoke(s);
        }

        public override void EndInvoke(SpoolSpace Variant)
        {
            // Do nothing
        }

        public override bool IsBreakAble
        {
            get
            {
                return false;
            }
        }

    }

}
