using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;

namespace ArcticWind.Expressions.MatrixExpressions
{

    public sealed class MatrixExpressionIdentity : MatrixExpression
    {

        private int _Size;
        private CellAffinity _Type;

        public MatrixExpressionIdentity(MatrixExpression Parent, int Size, CellAffinity Type)
            : base(Parent)
        {
            this._Size = Size;
            this._Type = Type;
        }

        public override int ReturnSize()
        {
            return this._Size;
        }

        public override CellMatrix Evaluate(SpoolSpace Variant)
        {
            return CellMatrix.Identity(this._Size, this._Type);
        }

        public override CellAffinity ReturnAffinity()
        {
            return this._Type;
        }

        public override MatrixExpression CloneOfMe()
        {
            return new MatrixExpressionIdentity(this.ParentNode, this._Size, this._Type);
        }

    }

}
