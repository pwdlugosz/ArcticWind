using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;

namespace ArcticWind.Expressions.ActionExpressions
{
    
    public class ActionExpressionWhile : ActionExpression
    {

        private ScalarExpression _Predicate;

        public ActionExpressionWhile(Host Host, ActionExpression Parent, ScalarExpression Predicate)
            : base(Host, Parent)
        {
            this._Predicate = Predicate;
        }

        public override void BeginInvoke(SpoolSpace Variant)
        {
            this._Children.ForEach((x) => { x.BeginInvoke(Variant); });
        }

        public override void EndInvoke(SpoolSpace Variant)
        {
            this._Children.ForEach((x) => { x.EndInvoke(Variant); });
        }

        public override bool IsBreakAble
        {
            get
            {
                return true;
            }
        }

        public override void Invoke(SpoolSpace Variant)
        {

            while (this._Predicate.Evaluate(Variant).valueBOOL == true)
            {

                foreach (ActionExpression ae in this._Children)
                {

                    ae.Invoke(Variant);
                    if (this.Condition == BreakLevel.Break || this.Condition == BreakLevel.Return)
                        break;

                }

            }

        }
    
    }

}
