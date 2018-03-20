
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.RecordExpressions;


namespace ArcticWind.Expressions.MatrixExpressions
{

    public sealed class MatrixExpressionLiteral : MatrixExpression
    {

        private List<ScalarExpressionSet> _exp;
        private int _MaxCol = -1;
        private CellAffinity _ReturnAffinity = CellAffinity.BOOL;
        private int _ReturnSize = -1;

        public MatrixExpressionLiteral(MatrixExpression Parent, List<ScalarExpressionSet> Values)
            : base(Parent)
        {
            this._exp = Values;
            foreach (ScalarExpressionSet ses in this._exp)
            {
                this._ReturnAffinity = CellAffinityHelper.Highest(ses.MaxAffinity, this._ReturnAffinity);
                this._ReturnSize = Math.Max(ses.MaxSize, this._ReturnSize);
                this._MaxCol = Math.Max(ses.Columns.Count, this._MaxCol);
            }
        }

        public override CellMatrix Evaluate(SpoolSpace Variant)
        {

            CellMatrix m = new CellMatrix(this._exp.Count, this._MaxCol, this._ReturnAffinity, this._ReturnSize);
            Cell n = CellValues.Null(this._ReturnAffinity);
            for (int i = 0; i < this._exp.Count; i++)
            {

                for (int j = 0; j < this._MaxCol; j++)
                {

                    if (this._exp[i].Count > j)
                        m[i, j] = this._exp[i][j].Evaluate(Variant);

                }

            }

            return m;

        }

        public override CellAffinity ReturnAffinity()
        {
            return this._ReturnAffinity;
        }

        public override int ReturnSize()
        {
            return this._ReturnSize;
        }

        public override MatrixExpression CloneOfMe()
        {
            return new MatrixExpressionLiteral(this.ParentNode, this._exp);
        }

    }

}


