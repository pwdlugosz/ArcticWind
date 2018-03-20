using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.RecordExpressions;

namespace ArcticWind.Expressions.ActionExpressions
{

    public sealed class ActionExpressionScalarAssign : ActionExpression
    {

        private string _Name;
        private string _Store;
        private Assignment _Logic;
        private ScalarExpression _Value;

        public ActionExpressionScalarAssign(Host Host, ActionExpression Parent, string Store, string Name, ScalarExpression Value, Assignment Logic)
            : base(Host, Parent)
        {
            this._Store = Store;
            this._Name = Name;
            this._Logic = Logic;
            this._Value = Value;
        }

        public override void Invoke(SpoolSpace Variant)
        {

            Cell a = Variant[this._Store].GetScalar(this._Name);
            Cell b = this._Value.Evaluate(Variant);

            switch (this._Logic)
            {

                case Assignment.Equals:
                    Variant[this._Store].SetScalar(this._Name, b);
                    break;
                case Assignment.PlusEquals:
                    Variant[this._Store].SetScalar(this._Name, a + b);
                    break;
                case Assignment.MinusEquals:
                    Variant[this._Store].SetScalar(this._Name, a - b);
                    break;
                case Assignment.ProductEquals:
                    Variant[this._Store].SetScalar(this._Name, a * b);
                    break;
                case Assignment.DivideEquals:
                    Variant[this._Store].SetScalar(this._Name, a / b);
                    break;
                case Assignment.CheckDivideEquals:
                    Variant[this._Store].SetScalar(this._Name, Cell.CheckDivide(a, b));
                    break;
                case Assignment.ModEquals:
                    Variant[this._Store].SetScalar(this._Name, a % b);
                    break;

            }

        }

    }

    public sealed class ActionExpressionRecordAssign : ActionExpression
    {

        private string _Name;
        private string _Store;
        private RecordExpression _Value;

        public ActionExpressionRecordAssign(Host Host, ActionExpression Parent, string Store, string Name, RecordExpression Value)
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

    public sealed class ActionExpressionRecordMemberAssign : ActionExpression
    {

        private string _sName;
        private string _rName;
        private string _vName;
        private Assignment _Logic;
        private ScalarExpression _Value;

        public ActionExpressionRecordMemberAssign(Host Host, ActionExpression Parent, string StoreName, string RecordName, string ValueName, ScalarExpression Value, Assignment Logic)
            :base(Host, Parent)
        {
            this._sName = StoreName;
            this._rName = RecordName;
            this._vName = ValueName;
            this._Value = Value;
            this._Logic = Logic;
        }

        public override void Invoke(SpoolSpace Variant)
        {
            
            Cell a = Variant[this._sName].GetRecord(this._rName)[this._vName];
            Cell b = this._Value.Evaluate(Variant);
            switch(this._Logic)
            {
                case Assignment.Equals:
                    // Do Nothing
                    break;
                case Assignment.PlusEquals:
                    b = a + b;
                    break;
                case Assignment.MinusEquals:
                    b = a - b;
                    break;
                case Assignment.ProductEquals:
                    b = a * b;
                    break;
                case Assignment.DivideEquals:
                    b = a / b;
                    break;
                case Assignment.CheckDivideEquals:
                    b = Cell.CheckDivide(a, b);
                    break;
                case Assignment.ModEquals:
                    b = a % b;
                    break;
            }

            Variant[this._sName].SetRecordElement(this._rName, this._vName, b);

        }
    
    }



}
