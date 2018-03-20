using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Expressions.TableExpressions;
using ArcticWind.Tables;

namespace ArcticWind.Expressions.RecordExpressions
{

    public sealed class RecordExpressionStoreRef : RecordExpression
    {

        private string _StoreName;
        private string _ValueName;
        private Schema _Columns;

        public RecordExpressionStoreRef(Host Host, RecordExpression Parent, string StoreName, string ValueName, Schema Columns)
            : base(Host, Parent)
        {
            this._StoreName = StoreName;
            this._ValueName = ValueName;
            this._Columns = Columns;
        }

        public string RecordName
        {
            get { return this._ValueName; }
        }

        public override Schema Columns
        {
            get { return this._Columns; }
        }

        public override AssociativeRecord EvaluateAssociative(SpoolSpace Variants)
        {
            AssociativeRecord r = Variants[this._StoreName].GetRecord(this._ValueName);
            return new AssociativeRecord(r);
        }

        public override RecordExpression CloneOfMe()
        {
            return new RecordExpressionStoreRef(this._Host, this._Parent, this._StoreName, this._ValueName, this._Columns);
        }

    }

}
