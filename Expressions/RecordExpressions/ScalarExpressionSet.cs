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

    public sealed class ScalarExpressionSet : IColumns
    {

        private Heap<ScalarExpression> _Expressions;
        private Schema _Columns;
        private Host _Host;

        public ScalarExpressionSet(Host Host)
        {
            this._Host = Host;
            this._Expressions = new Heap<ScalarExpression>();
            this._Columns = new Schema();
        }

        public ScalarExpressionSet(Host Host, Schema Columns, string SpoolName, string Alias)
            : this(Host)
        {

            for (int i = 0; i < Columns.Count; i++)
            {

                ScalarExpressionRecordElementRef e = new ScalarExpressionRecordElementRef(this._Host, null, SpoolName, Alias, Columns.ColumnName(i), Columns.ColumnAffinity(i), Columns.ColumnSize(i));
                this.Add(Columns.ColumnName(i), e);
            }

        }

        public ScalarExpressionSet(Host Host, AssociativeRecord Record)
            : this(Host)
        {

            for (int i = 0; i < Record.Count; i++)
            {
                this.Add(Record.Columns.ColumnName(i), new ScalarExpressionConstant(null, Record[i]));
            }

        }

        public int Count
        {
            get { return this._Expressions.Count; }
        }

        public ScalarExpression this[int IndexOf]
        {
            get { return this._Expressions[IndexOf]; }
        }

        public ScalarExpression this[string Name]
        {
            get { return this._Expressions[Name]; }
        }

        public IEnumerable<ScalarExpression> Expressions
        {
            get { return this._Expressions.Values; }
        }

        public CellAffinity MaxAffinity
        {
            get
            {
                CellAffinity c = CellAffinityHelper.LOWEST_AFFINITY;
                foreach (ScalarExpression se in this._Expressions.Values)
                {
                    c = CellAffinityHelper.Highest(c, se.ReturnAffinity());
                }
                return c;
            }
        }

        public int MaxSize
        {
            get
            {
                int c = -1;
                foreach (ScalarExpression se in this._Expressions.Values)
                {
                    c = Math.Max(c, se.ReturnSize());
                }
                return c;
            }
        }

        public Schema Columns
        {
            get { return this._Columns; }
        }

        public void Bind(string Name, Parameter Value)
        {

            foreach (ScalarExpression s in this._Expressions.Values)
                s.Bind(Name, Value);

        }

        public bool Exists(string Name)
        {
            return this._Expressions.Exists(Name);
        }

        public string Alias(int IndexOf)
        {
            return this._Expressions.Name(IndexOf);
        }

        public void Add(string Alias, ScalarExpression Element)
        {
            Element.Name = Alias;
            this._Expressions.Allocate(Alias, Element);
            this._Columns.Add(Alias, Element.ReturnAffinity(), Element.ReturnSize());
        }

        public void Add(ScalarExpressionSet Elements)
        {

            for (int i = 0; i < Elements.Count; i++)
            {
                this.Add(Elements.Alias(i), Elements[i]);
            }

        }

        public Record Evaluate(SpoolSpace Variants)
        {
            Cell[] c = new Cell[this.Count];
            for (int i = 0; i < c.Length; i++)
            {
                c[i] = this._Expressions[i].Evaluate(Variants);
            }
            return new Record(c);
        }

        public AssociativeRecord EvaluateAssociative(SpoolSpace Variants)
        {
            return new AssociativeRecord(this.Columns, this.Evaluate(Variants));
        }

        public static ScalarExpressionSet operator +(ScalarExpressionSet A, ScalarExpressionSet B)
        {

            ScalarExpressionSet rex = new ScalarExpressionSet(A._Host);

            for (int i = 0; i < A.Count; i++)
                rex.Add(A.Alias(i), A[i]);
            for (int i = 0; i < B.Count; i++)
                rex.Add(B.Alias(i), B[i]);

            return rex;

        }

        public static List<AssociativeRecord> ToAssociativeRecordSet(List<ScalarExpressionSet> Records, SpoolSpace Variants)
        {

            List<AssociativeRecord> rexs = new List<AssociativeRecord>();
            foreach (ScalarExpressionSet rex in Records)
                rexs.Add(rex.EvaluateAssociative(Variants));
            return rexs;

        }

        public static List<Record> ToRecordSet(List<ScalarExpressionSet> Records, SpoolSpace Variants)
        {

            List<Record> rexs = new List<Record>();
            foreach (ScalarExpressionSet rex in Records)
                rexs.Add(rex.Evaluate(Variants));
            return rexs;

        }

    }


}
