using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;

namespace ArcticWind.Expressions.ActionExpressions
{
    
    public class ActionExpressionDo : ActionExpression
    {

        public ActionExpressionDo(Host Host, ActionExpression Parent)
            : base(Host, Parent)
        {
        }

        public override void BeginInvoke(SpoolSpace Variant)
        {
            this._Children.ForEach((x) => { x.BeginInvoke(Variant); });
        }

        public override void EndInvoke(SpoolSpace Variant)
        {
            this._Children.ForEach((x) => { x.EndInvoke(Variant); });
        }

        public override void Invoke(SpoolSpace Variant)
        {
            this._Children.ForEach((x) => { x.Invoke(Variant); });
        }

    }

}
