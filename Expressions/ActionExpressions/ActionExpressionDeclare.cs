using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.TableExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Tables;

namespace ArcticWind.Expressions.ActionExpressions
{
    
    public class ActionExpressionDeclareScalar : ActionExpression
    {

        private string _Store;
        private string _Name;
        private ScalarExpression _Value;
        private bool _Dynamic;

        public ActionExpressionDeclareScalar(Host Host, ActionExpression Parent, string Store, string Name, ScalarExpression Value)
            : base(Host, Parent)
        {
            this._Store = Store;
            this._Name = Name;
            this._Value = Value;
        }

        public override void Invoke(SpoolSpace Variant)
        {
            Variant[this._Store].SetScalar(this._Name, this._Value.Evaluate(Variant));
        }

    }

    public class ActionExpressionDeclareMatrix : ActionExpression
    {

        private string _Store;
        private string _Name;
        private MatrixExpression _Value;

        public ActionExpressionDeclareMatrix(Host Host, ActionExpression Parent, string Store, string Name, MatrixExpression Value)
            : base(Host, Parent)
        {
            this._Store = Store;
            this._Name = Name;
            this._Value = Value;
            if (Value == null) throw new Exception();
        }

        public override void Invoke(SpoolSpace Variant)
        {
            Variant[this._Store].SetMatrix(this._Name, this._Value.Evaluate(Variant));
        }

    }

    public class ActionExpressionDeclareRecord : ActionExpression
    {

        private string _Store;
        private string _Name;
        private RecordExpression _Value;

        public ActionExpressionDeclareRecord(Host Host, ActionExpression Parent, string Store, string Name, RecordExpression Value)
            : base(Host, Parent)
        {
            this._Store = Store;
            this._Name = Name;
            this._Value = Value;
        }

        public override void Invoke(SpoolSpace Variant)
        {
            Variant[this._Store].SetRecord(this._Name, this._Value.EvaluateAssociative(Variant));
        }

    }

    public class ActionExpressionDeclareTable1 : ActionExpression
    {

        private string _SpoolName;
        private string _MemName;
        private ScalarExpression _DiskDir;
        private ScalarExpression _DiskName;
        private TableExpression _Value;

        public ActionExpressionDeclareTable1(Host Host, ActionExpression Parent, string SpoolName, string MemName, 
            ScalarExpression DiskDir, ScalarExpression DiskName, TableExpression Value)
            : base(Host, Parent)
        {
            this._SpoolName = SpoolName;
            this._MemName = MemName;
            this._DiskDir = DiskDir;
            this._DiskName = DiskName;
            this._Value = Value;
            if (Value == null) 
                throw new Exception();
        }

        public override void Invoke(SpoolSpace Variant)
        {

            // Evaluate the table
            string dir = this._DiskDir.Evaluate(Variant).valueCSTRING;
            string name = this._DiskName.Evaluate(Variant).valueCSTRING;
            Table t = new HeapTable(this._Host, name, dir, this._Value.Columns, Page.DEFAULT_SIZE);
            using (RecordWriter w = t.OpenWriter())
            {
                this._Value.Evaluate(Variant, w);
            }

            // Sets the table's alias in memory
            Variant[this._SpoolName].SetTable(this._MemName, t);

        }


    }

    public class ActionExpressionDeclareTable2 : ActionExpression
    {

        private string _SpoolName;
        private string _MemName;
        private ScalarExpression _DiskDir;
        private ScalarExpression _DiskName;
        private ScalarExpression _SourceDir;
        private ScalarExpression _SourceName;
        
        public ActionExpressionDeclareTable2(Host Host, ActionExpression Parent, string SpoolName, string MemName,
            ScalarExpression DiskDir, ScalarExpression DiskName, ScalarExpression SourceDir, ScalarExpression SourceName)
            : base(Host, Parent)
        {
            this._SpoolName = SpoolName;
            this._MemName = MemName;
            this._DiskDir = DiskDir;
            this._DiskName = DiskName;
            this._SourceDir = SourceDir;
            this._SourceName = SourceName;
        }

        public override void Invoke(SpoolSpace Variant)
        {

            // Evaluate the table
            string dir = this._DiskDir.Evaluate(Variant).valueCSTRING;
            string name = this._DiskName.Evaluate(Variant).valueCSTRING;
            string sdir = this._SourceDir.Evaluate(Variant).valueCSTRING;
            string sname = this._SourceName.Evaluate(Variant).valueCSTRING;

            Table t = this._Host.OpenTable(sdir, sname);

            // Sets the table's alias in memory
            Variant[this._SpoolName].SetTable(this._MemName, t);

        }


    }

    public class ActionExpressionDeclareVoidMethod : ActionExpression
    {

        private string _SpoolName;
        private AbstractMethod _Method;

        public ActionExpressionDeclareVoidMethod(Host Host, ActionExpression Parent, string SpoolName, string MethodName)
            : base(Host, Parent)
        {
            this._SpoolName = SpoolName;
            this._Method = new AbstractMethod(this._Host, MethodName);
        }

        public void AddParameter(string Name, ParameterAffinity Affinity)
        {
            this._Method.AddSigniture(Name, Affinity);
        }

        public void AddStatement(ActionExpression Statement)
        {
            this._Method.AddStatement(Statement);
        }

        public override void Invoke(SpoolSpace Variant)
        {
            this._Host.UserLibrary.AddVoid(this._Method);
        }

    }

    public class ActionExpressionDeclareScalarMethod : ActionExpression
    {

        private string _SpoolName;
        private TypedAbstractMethod _Method;
        
        public ActionExpressionDeclareScalarMethod(Host Host, ActionExpression Parent, string SpoolName, string MethodName, CellAffinity Affinity, int Size)
            : base(Host, Parent)
        {
            this._SpoolName = SpoolName;
            this._Method = new TypedAbstractMethod(this._Host, MethodName, Affinity, Size);
        }

        public void AddParameter(string Name, ParameterAffinity Affinity)
        {
            this._Method.AddSigniture(Name, Affinity);
        }

        public void AddStatement(ActionExpression Statement)
        {
            this._Method.AddStatement(Statement);
        }

        public override void Invoke(SpoolSpace Variant)
        {
            this._Host.UserLibrary.AddScalar(this._Method);
        }

    }

}
