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


    public sealed class RecordExpressionLiteral : RecordExpression
    {

        private Heap<ScalarExpression> _Scalars;

        public RecordExpressionLiteral(Host Host, RecordExpression Parent)
            : base(Host, Parent)
        {
            this._Scalars = new Heap<ScalarExpression>();
        }

        public void Add(ScalarExpression Value, string Alias)
        {
            this._Scalars.Allocate(Alias, Value);
        }

        public void Add(ScalarExpression Value)
        {
            string x = "F" + this._Scalars.Count.ToString();
            this.Add(Value, x);
        }

        public override RecordExpression CloneOfMe()
        {
            RecordExpressionLiteral x = new RecordExpressionLiteral(this._Host, this._Parent);
            foreach (KeyValuePair<string, ScalarExpression> kv in this._Scalars.Entries)
            {
                x.Add(kv.Value.CloneOfMe(), kv.Key);
            }
            return x;
        }

        public override AssociativeRecord EvaluateAssociative(SpoolSpace Variants)
        {

            Schema columns = new Schema();
            List<Cell> cells = new List<Cell>();

            foreach (KeyValuePair<string, ScalarExpression> kv in this._Scalars.Entries)
            {
                columns.Add(kv.Key, kv.Value.ReturnAffinity(), kv.Value.ReturnSize());
                cells.Add(kv.Value.Evaluate(Variants));
            }

            return new AssociativeRecord(columns, cells.ToArray());

        }

        public override Schema Columns
        {
            get
            {

                Schema columns = new Schema();
                foreach (KeyValuePair<string, ScalarExpression> kv in this._Scalars.Entries)
                {
                    columns.Add(kv.Key, kv.Value.ReturnAffinity(), kv.Value.ReturnSize());
                }
                return columns;

            }
        }

    }


}
