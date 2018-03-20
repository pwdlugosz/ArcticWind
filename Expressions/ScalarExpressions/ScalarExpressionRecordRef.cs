using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Elements;

namespace ArcticWind.Expressions.ScalarExpressions
{

    public sealed class ScalarExpressionRecordRef2 : ScalarExpression
    {

        private RecordExpression _Record;
        private string _Name;
        private Host _Host;

        public ScalarExpressionRecordRef2(Host Host, ScalarExpression Parent, RecordExpression Value, string Name)
            : base(Parent, ScalarExpressionAffinity.Heap)
        {
            this._Record = Value;
            this._Name = Name;
            this._Host = Host;
        }

        // Overrides //
        public override string Unparse(SpoolSpace Variants)
        {
            return null;
        }

        public override ScalarExpression CloneOfMe()
        {
            return new ScalarExpressionRecordRef2(this._Host, this._ParentNode, this._Record, this._Name);
        }

        public override int ReturnSize()
        {
            return this._Record.Columns.ColumnSize(this._Name);
        }

        public override CellAffinity ReturnAffinity()
        {
            return this._Record.Columns.ColumnAffinity(this._Name);
        }

        public override Cell Evaluate(SpoolSpace Variants)
        {
            return this._Record.EvaluateAssociative(Variants)[this._Name];
        }

    }

    public sealed class ScalarExpressionRecordRef : ScalarExpression
    {

        private string _StoreName;
        private string _RecordName;
        private string _FieldName;
        private CellAffinity _CellAffinity;
        private int _Size;
        private Host _Host;

        public ScalarExpressionRecordRef(Host Host, ScalarExpression Parent, string StoreName, string RecordName, string FieldName, CellAffinity Affinity, int Size)
            : base(Parent, ScalarExpressionAffinity.Heap)
        {
            this._StoreName = StoreName;
            this._RecordName = RecordName;
            this._FieldName = FieldName;
            this._CellAffinity = Affinity;
            this._Size = Size;
            this._Host = Host;
        }

        public string StoreName
        {
            get { return this._StoreName; }
        }

        public string RecordName
        {
            get { return this._RecordName; }
        }

        public string FieldName
        {
            get { return this._FieldName; }
        }

        // Overrides //
        public override string Unparse(SpoolSpace Variants)
        {
            return null;
        }

        public override ScalarExpression CloneOfMe()
        {
            return new ScalarExpressionRecordRef(this._Host, this._ParentNode, this._StoreName, this._RecordName, this._FieldName, this._CellAffinity, this._Size);
        }

        public override int ReturnSize()
        {
            return this._Size;
        }

        public override CellAffinity ReturnAffinity()
        {
            return this._CellAffinity;
        }

        public override Cell Evaluate(SpoolSpace Variants)
        {
            return Variants[this._StoreName].GetRecord(this._RecordName)[this._FieldName];
        }

        public override string BuildAlias()
        {
            return this.FieldName;
        }

    }

}
