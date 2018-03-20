using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;

namespace ArcticWind.Expressions.ActionExpressions
{

    public sealed class ActionExpressionMatrixUnitAssign : ActionExpression
    {

        private string _SpoolSpace;
        private string _VarName;
        private Assignment _Logic;
        private ScalarExpression _Value;
        private ScalarExpression _Row;
        private ScalarExpression _Col;

        public ActionExpressionMatrixUnitAssign(Host Host, ActionExpression Parent, string NameSpace, string VarName, ScalarExpression Row, 
            ScalarExpression Col, ScalarExpression Value, Assignment Logic)
            : base(Host, Parent)
        {
            this._SpoolSpace = NameSpace;
            this._VarName = VarName;
            this._Logic = Logic;
            this._Value = Value;
            this._Row = Row ?? ScalarExpression.ZeroINT;
            this._Col = Col ?? ScalarExpression.ZeroINT;
        }

        public override void Invoke(SpoolSpace Variant)
        {

            int Row = (int)this._Row.Evaluate(Variant).valueLONG;
            int Col = (int)this._Col.Evaluate(Variant).valueLONG;

            Cell a = Variant[this._SpoolSpace].GetMatrix(this._VarName)[Row, Col];
            Cell b = this._Value.Evaluate(Variant);

            switch (this._Logic)
            {

                case Assignment.Equals:
                    // Do Nothing
                    break;
                case Assignment.PlusEquals:
                    b = a + b;
                    break;
                case Assignment.MinusEquals:
                    b = b - a;
                    break;
                case Assignment.ProductEquals:
                    b = b * a;
                    break;
                case Assignment.DivideEquals:
                    b = b / a;
                    break;
                case Assignment.CheckDivideEquals:
                    b = Cell.CheckDivide(b, a);
                    break;
                case Assignment.ModEquals:
                    b = b % a;
                    break;

            }

            Variant[this._SpoolSpace].SetMatrixElement(this._VarName, Row, Col, b);

        }

    }


}
