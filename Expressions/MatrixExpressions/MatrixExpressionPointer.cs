using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;

namespace ArcticWind.Expressions.MatrixExpressions
{

    public sealed class MatrixExpressionPointer : MatrixExpression
    {

        private CellAffinity _Affinity;
        private int _Size;

        public MatrixExpressionPointer(MatrixExpression Parent, string Name, CellAffinity Affinity, int Size)
            : base(Parent)
        {
            this._Name = Name;
            this._Affinity = Affinity;
            this._Size = Size;
        }

        public override MatrixExpression CloneOfMe()
        {
            return new MatrixExpressionPointer(this.ParentNode, this._Name, this._Affinity, this._Size);
        }

        public override CellAffinity ReturnAffinity()
        {
            return this._Affinity;
        }

        public override int ReturnSize()
        {
            return this._Size;
        }

        public override CellMatrix Evaluate(SpoolSpace Variant)
        {
            throw new Exception("Cannot evaluate pointers");
        }


    }

}
