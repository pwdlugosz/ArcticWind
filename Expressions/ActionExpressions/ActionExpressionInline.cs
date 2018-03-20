using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Expressions;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Elements;


namespace ArcticWind.Expressions.ActionExpressions
{
    
    public class ActionExpressionInline : ActionExpression
    {

        private ScalarExpression _Script;
        private ScalarExpressionSet _Parameters;
        private ScalarExpressionSet _Values;
        private Scripting.ScriptProcessor _Engine;

        public ActionExpressionInline(Host Host, ActionExpression Parent, ScalarExpression Script, ScalarExpressionSet Parameters, ScalarExpressionSet Values, Scripting.ScriptProcessor Engine)
            : base(Host, Parent)
        {
            this._Script = Script;
            this._Parameters = Parameters;
            this._Values = Values;
            this._Engine = Engine;
        }

        public override void Invoke(SpoolSpace Variant)
        {

            StringBuilder sb = new StringBuilder(this._Script.Evaluate(Variant).valueCSTRING);

            for (int i = 0; i < this._Values.Count; i++)
            {
                string parameter = this._Parameters[i].Evaluate(Variant).valueCSTRING;
                string value = this._Values
                    [i].Evaluate(Variant).valueCSTRING;
                sb.Replace(parameter, value);
            }

            // Render all actions //
            this._Engine.RenderAction(sb.ToString());

        }

    }

}
