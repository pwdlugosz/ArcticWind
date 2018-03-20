using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.RecordExpressions;

namespace ArcticWind.Expressions.ActionExpressions
{
    
    public sealed class ActionExpressionIf : ActionExpression
    {

        private ScalarExpressionSet _Scalars;
        private bool _HasElse = false;

        public ActionExpressionIf(Host Host, ActionExpression Parent, ScalarExpressionSet Scalars)
            : base(Host, Parent)
        {
            this._Scalars = Scalars;
        }

        public override void BeginInvoke(SpoolSpace Variant)
        {

            // Need to check out numbers a bit //
            // Check that the child count either = scalar count, which mean's there's no else, or it's childcount = scalars + 1, meaning there is an else
            if (this._Children.Count == this._Scalars.Count)
                this._HasElse = false;
            else if (this._Children.Count == this._Scalars.Count + 1)
                this._HasElse = true;
            else
                throw new Exception("Cannot evaluate the if statement; conditions and actions are out of sync");

            this._Children.ForEach((ae) => { ae.BeginInvoke(Variant); });

        }

        public override void EndInvoke(SpoolSpace Variant)
        {
            this._Children.ForEach((ae) => { ae.EndInvoke(Variant); });
        }

        public override void Invoke(SpoolSpace Variant)
        {

            Record Conditions = this._Scalars.Evaluate(Variant);
            for (int i = 0; i < Conditions.Count; i++)
            {
                
                if (Conditions[i].valueBOOL == true)
                {
                    this._Children[i].Invoke(Variant);
                    return;
                }
            }

            if (this._HasElse)
                this._Children.Last().Invoke(Variant);

        }

    }



}
