using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;

namespace ArcticWind.Expressions.MatrixExpressions
{

    public sealed class MatrixExpressionStoreRef : MatrixExpression
    {


        private string _StoreName;
        private string _MatrixName;
        private CellAffinity _Affinity;
        private int _Size;

        public MatrixExpressionStoreRef(MatrixExpression Parent, string StoreName, string MatrixName, CellAffinity Affinity, int Size)
            : base(Parent)
        {
            this._StoreName = StoreName;
            this._MatrixName = MatrixName;
            this._Affinity = Affinity;
            this._Size = Size;
        }

        public override CellMatrix Evaluate(SpoolSpace Variant)
        {
            return Variant[this._StoreName].GetMatrix(this._MatrixName);
        }

        public override CellAffinity ReturnAffinity()
        {
            return this._Affinity;
        }

        public override int ReturnSize()
        {
            return this._Size;
        }
        
        public override MatrixExpression CloneOfMe()
        {
            return new MatrixExpressionStoreRef(this.ParentNode, this._StoreName, this._MatrixName, this._Affinity, this._Size);
        }

        public string MatrixName
        {
            get { return this._MatrixName; }
        }

    }

}
