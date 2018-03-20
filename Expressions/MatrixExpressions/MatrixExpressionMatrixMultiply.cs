using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;

namespace ArcticWind.Expressions.MatrixExpressions
{

    //public sealed class MatrixExpressionMatrixMultiply : MatrixExpression
    //{

    //    public MatrixExpressionMatrixMultiply(MatrixExpression Parent)
    //        : base(Parent)
    //    {
    //    }

    //    public override CellMatrix Evaluate(FieldResolver Variant)
    //    {
    //        return this[0].Evaluate(Variant) ^ this[1].Evaluate(Variant);
    //    }

    //    public override CellAffinity ReturnAffinity()
    //    {
    //        return this[0].ReturnAffinity();
    //    }

    //    public override MatrixExpression CloneOfMe()
    //    {
    //        MatrixExpression node = new MatrixExpressionMatrixMultiply(this.ParentNode);
    //        foreach (MatrixExpression m in this._Children)
    //            node.AddChildNode(m.CloneOfMe());
    //        return node;
    //    }

    //    public override int ReturnSize()
    //    {
    //        return Math.Max(this._Children[0].ReturnSize(), this._Children[1].ReturnSize());
    //    }

    //}
    
}
