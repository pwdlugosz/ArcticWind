using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;

namespace ArcticWind.Expressions.MatrixExpressions
{

    public sealed class MatrixExpressionNullLiteral : MatrixExpression
    {

        private ScalarExpression _row;
        private ScalarExpression _col;
        private CellAffinity _type;

        public MatrixExpressionNullLiteral(MatrixExpression Parent, ScalarExpression Row, ScalarExpression Col, CellAffinity Type)
            : base(Parent)
        {
            this._row = Row;
            this._col = Col;
            this._type = Type;
        }

        public override CellAffinity ReturnAffinity()
        {
            return this._type;
        }

        public override int ReturnSize()
        {
            return CellSerializer.DefaultLength(this._type);
        }

        public override CellMatrix Evaluate(SpoolSpace Variant)
        {
            return new CellMatrix((int)this._row.Evaluate(Variant).LONG, (int)this._col.Evaluate(Variant).LONG, new Cell(this._type));
        }

        public override MatrixExpression CloneOfMe()
        {
            return new MatrixExpressionNullLiteral(this.ParentNode, this._row.CloneOfMe(), this._col.CloneOfMe(), this._type);
        }

    }

}
