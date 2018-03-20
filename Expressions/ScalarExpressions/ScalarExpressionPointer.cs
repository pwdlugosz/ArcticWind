using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;

namespace ArcticWind.Expressions.ScalarExpressions
{

    public class ScalarExpressionPointer : ScalarExpression
    {

        private string _NameID;
        private CellAffinity _Type;
        private int _Size;

        public ScalarExpressionPointer(ScalarExpression Parent, string RefName, CellAffinity Type, int Size)
            : base(Parent, ScalarExpressionAffinity.Pointer)
        {
            this._Type = Type;
            this._NameID = RefName;
            this._name = RefName;
            this._Size = CellSerializer.FixLength(Type, Size);
        }

        public override Cell Evaluate(SpoolSpace Variants)
        {
            throw new Exception(string.Format("Cannot evaluate pointer nodes; Name '{0}'", _NameID));
        }

        public override CellAffinity ReturnAffinity()
        {
            return this._Type;
        }

        public override string ToString()
        {
            return this.Affinity.ToString() + " : " + _NameID;
        }

        public override string Unparse(SpoolSpace Variants)
        {
            return this._NameID;
        }

        public override ScalarExpression CloneOfMe()
        {
            return new ScalarExpressionPointer(this.ParentNode, this._NameID, this._Type, this._Size);
        }

        public override int ReturnSize()
        {
            return this._Size;
        }

        public string PointerName
        {
            get { return _NameID; }
        }


    }

}
