using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.MatrixExpressions;

namespace ArcticWind.Expressions.ScalarExpressions
{


    /// <summary>
    /// Represents a scalar in a matrix in a heap
    /// </summary>
    public sealed class ScalarExpressionMatrixRef : ScalarExpression
    {

        private MatrixExpression _Value;
        private Host _Host;
        private ScalarExpression _Row;
        private ScalarExpression _Col;

        public ScalarExpressionMatrixRef(Host Host, ScalarExpression Parent, MatrixExpression Value, ScalarExpression Row, ScalarExpression Col)
            : base(Parent, ScalarExpressionAffinity.Matrix)
        {
            this._Value = Value;
            this._Host = Host;
            this._Row = Row;
            this._Col = Col ?? new ScalarExpressionConstant(null, CellValues.One(CellAffinity.INT));
        }

        // Overrides //
        public override string Unparse(SpoolSpace Variants)
        {
            return "TODO: ScalarExpressionMatrixRef.Unparse";
        }

        public override ScalarExpression CloneOfMe()
        {
            return new ScalarExpressionMatrixRef(this._Host, this._ParentNode, this._Value, this._Row, this._Col);
        }

        public override int ReturnSize()
        {
            return this._Value.ReturnSize();
        }

        public override CellAffinity ReturnAffinity()
        {
            return this._Value.ReturnAffinity();
        }

        public override Cell Evaluate(SpoolSpace Variants)
        {
            int row = this._Row.Evaluate(Variants).valueINT;
            int col = this._Col.Evaluate(Variants).valueINT;
            return this._Value.Evaluate(Variants)[row, col];
        }

    }


}
