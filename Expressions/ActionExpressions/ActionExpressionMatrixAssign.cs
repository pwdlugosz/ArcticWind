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


    public sealed class ActionExpressionMatrixAssign : ActionExpression
    {

        private Assignment _Logic;
        private string _StoreName;
        private string _MatrixName;
        private MatrixExpression _Value;

        public ActionExpressionMatrixAssign(Host Host, ActionExpression Parent, string StoreName, string MatrixName, MatrixExpression Value, Assignment Logic)
            : base(Host, Parent)
        {
            this._StoreName = StoreName;
            this._MatrixName = MatrixName;
            this._Logic = Logic;
            this._Value = Value;
        }

        public override void Invoke(SpoolSpace Variant)
        {

            CellMatrix m = this._Value.Evaluate(Variant);
            CellMatrix n = Variant[this._StoreName].GetMatrix(this._MatrixName);
            switch (this._Logic)
            {

                case Assignment.Equals:
                    // Do Nothing
                    break;
                case Assignment.PlusEquals:
                    m = n + m;
                    break;
                case Assignment.MinusEquals:
                    m = n - m;
                    break;
                case Assignment.ProductEquals:
                    m = n * m;
                    break;
                case Assignment.DivideEquals:
                    m = n / m;
                    break;
                case Assignment.CheckDivideEquals:
                    m = CellMatrix.CheckDivide(n,m);
                    break;
                case Assignment.ModEquals:
                    m = n % m;
                    break;

            }
            Variant[this._StoreName].SetMatrix(this._MatrixName, m);

        }

    }

}
