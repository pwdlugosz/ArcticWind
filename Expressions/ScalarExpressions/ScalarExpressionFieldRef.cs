using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;

namespace ArcticWind.Expressions.ScalarExpressions
{

    /// <summary>
    /// 
    /// </summary>
    public sealed class ScalarExpressionRecordElementRef : ScalarExpression
    {

        private string _SpoolSpace = null;
        private string _RecordName = null;
        private string _FieldName = null;
        private int _FieldSize;
        private CellAffinity _FieldAffinity;
        private Host _Host;

        public ScalarExpressionRecordElementRef(Host Host, ScalarExpression Parent, string SpoolSpace, string RecordName, string FieldName, CellAffinity FieldAffinity, int FieldSize)
            : base(Parent, ScalarExpressionAffinity.Field)
        {
            this._SpoolSpace = SpoolSpace;
            this._RecordName = RecordName;
            this._FieldName = FieldName;
            this._FieldAffinity = FieldAffinity;
            this._FieldSize = FieldSize;
            this._Host = Host;
        }

        // Field Name //
        public string StoreName
        {
            get { return this._SpoolSpace; }
        }

        public string RecordName
        {
            get { return this._RecordName; }
        }

        // Overrides //
        public override string Unparse(SpoolSpace Variants)
        {
            return this._RecordName + "." + this._FieldName;
        }

        public override ScalarExpression CloneOfMe()
        {
            return new ScalarExpressionRecordElementRef(this._Host, this._ParentNode, this._SpoolSpace, this._RecordName, this._FieldName, this._FieldAffinity, this._FieldSize);
        }

        public override int ReturnSize()
        {
            return this._FieldSize;
        }

        public override CellAffinity ReturnAffinity()
        {
            return this._FieldAffinity;
        }

        public override Cell Evaluate(SpoolSpace Variants)
        {
            return Variants[this._SpoolSpace].GetRecord(this._RecordName)[this._FieldName];
        }

        public override int GetHashCode()
        {
            return this._SpoolSpace.GetHashCode() ^ this._RecordName.GetHashCode() ^ this._FieldName.GetHashCode() ^ (int)this._FieldAffinity ^ this._FieldSize;
        }

        public override string BuildAlias()
        {
            return this._FieldName;
        }

    }

    public sealed class ScalarExpressionRecordElementRef2 : ScalarExpression
    {

        private string _SpoolSpace = null;
        private string _RecordName = null;
        private string _FieldName = null;
        private int _FieldSize;
        private CellAffinity _FieldAffinity;
        private Host _Host;
        private bool _IsInitialized = false;

        public ScalarExpressionRecordElementRef2(Host Host, ScalarExpression Parent, string SpoolSpace, string RecordName, string FieldName)
            : base(Parent, ScalarExpressionAffinity.Field)
        {
            this._SpoolSpace = SpoolSpace;
            this._RecordName = RecordName;
            this._FieldName = FieldName;
            this._Host = Host;
        }

        // Field Name //
        public string StoreName
        {
            get { return this._SpoolSpace; }
        }

        public string RecordName
        {
            get { return this._RecordName; }
        }

        // Overrides //
        public override string Unparse(SpoolSpace Variants)
        {
            return this._RecordName + "." + this._FieldName;
        }

        public override ScalarExpression CloneOfMe()
        {
            return new ScalarExpressionRecordElementRef(this._Host, this._ParentNode, this._SpoolSpace, this._RecordName, this._FieldName, this._FieldAffinity, this._FieldSize);
        }

        public override int ReturnSize()
        {
            if (!this._IsInitialized) 
                throw new Exception("Field not initialized");
            return this._FieldSize;
        }

        public override CellAffinity ReturnAffinity()
        {
            if (!this._IsInitialized)
                throw new Exception("Field not initialized");
            return this._FieldAffinity;
        }

        public void Initialize(SpoolSpace Variants)
        {
            if (this._IsInitialized)
                return;
            this._FieldAffinity = Variants[this._SpoolSpace].GetColumns(this._RecordName).ColumnAffinity(this._FieldName);
            this._FieldSize = Variants[this._SpoolSpace].GetColumns(this._RecordName).ColumnSize(this._FieldName);
        }

        public override Cell Evaluate(SpoolSpace Variants)
        {
            this.Initialize(Variants);
            return Variants[this._SpoolSpace].GetRecord(this._RecordName)[this._FieldName];
        }

        public override int GetHashCode()
        {
            return this._SpoolSpace.GetHashCode() ^ this._RecordName.GetHashCode() ^ this._FieldName.GetHashCode() ^ (int)this._FieldAffinity ^ this._FieldSize;
        }

        public override string BuildAlias()
        {
            return this._FieldName;
        }

    }


}
