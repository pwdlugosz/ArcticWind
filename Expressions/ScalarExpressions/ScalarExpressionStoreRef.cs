using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.RecordExpressions;

namespace ArcticWind.Expressions.ScalarExpressions
{

    /// <summary>
    /// Represents a scalar in a heap
    /// </summary>
    public sealed class ScalarExpressionStoreRef : ScalarExpression
    {

        private string _StoreName;
        private string _FieldName;
        private CellAffinity _ValueAffinity;
        private int _Size;
        private Host _Host;

        public ScalarExpressionStoreRef(Host Host, ScalarExpression Parent, string StoreName, string FieldName, CellAffinity Affinity, int Size)
            : base(Parent, ScalarExpressionAffinity.Heap)
        {
            this._StoreName = StoreName;
            this._FieldName = FieldName;
            this._ValueAffinity = Affinity;
            this._Size = Size;
            this._Host = Host;
        }

        // Overrides //
        public override string Unparse(SpoolSpace Variants)
        {
            return this._FieldName;
        }

        public override ScalarExpression CloneOfMe()
        {
            return new ScalarExpressionStoreRef(this._Host, this.ParentNode, this._StoreName, this._FieldName, this._ValueAffinity, this._Size);
        }

        public override int ReturnSize()
        {
            return this._Size;
        }

        public override CellAffinity ReturnAffinity()
        {
            return this._ValueAffinity;
        }

        public override Cell Evaluate(SpoolSpace Variants)
        {
            return Variants[this._StoreName].GetScalar(this._FieldName);
        }

        public override string BuildAlias()
        {
            return this._FieldName;
        }

    }


}
