using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.TableExpressions;
using ArcticWind.Expressions.RecordExpressions;

namespace ArcticWind.Expressions
{

    public enum ParameterAffinity : byte
    {
        Missing,
        Scalar,
        Matrix,
        Record,
        Table
    }

    public sealed class Parameter
    {

        private ParameterAffinity _Affinity;
        private ScalarExpression _s;
        private MatrixExpression _m;
        private RecordExpression _r;
        private TableExpression _t;

        public Parameter()
        {
            this._Affinity = ParameterAffinity.Missing;
        }

        public Parameter(ScalarExpression Value)
        {
            this._s = Value;
            this._Affinity = ParameterAffinity.Scalar;
        }

        public Parameter(MatrixExpression Value)
        {
            this._m = Value;
            this._Affinity = ParameterAffinity.Matrix;
        }

        public Parameter(RecordExpression Value)
        {
            this._r = Value;
            this._Affinity = ParameterAffinity.Record;
        }

        public Parameter(TableExpression Value)
        {
            this._t = Value;
            this._Affinity = ParameterAffinity.Table;
        }

        public ParameterAffinity Affinity
        {
            get { return this._Affinity; }
        }

        public ScalarExpression Scalar
        {
            get { return this._s; }
        }

        public MatrixExpression Matrix
        {
            get { return this._m; }
        }

        public RecordExpression Record
        {
            get { return this._r; }
        }

        public TableExpression Table
        {
            get { return this._t; }
        }


    }

}
