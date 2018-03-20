using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Tables;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Expressions.TableExpressions;

namespace ArcticWind.Expressions.ActionExpressions
{

    public sealed class ActionExpressionFor : ActionExpression
    {

        private string _SpoolSpace;
        private string _VarName;

        private ScalarExpression _Start;
        private ScalarExpression _Control;
        private ActionExpression _Increment;
        private bool _Burn = false;

        public ActionExpressionFor(Host Host, ActionExpression Parent, string SpoolSpace, string VarName, 
            ScalarExpression Start, ScalarExpression Control, ActionExpression Increment)
            : base(Host, Parent)
        {
            this._SpoolSpace = SpoolSpace;
            this._VarName = VarName;
            this._Start = Start;
            this._Control = Control;
            this._Increment = Increment;
        }

        public override void BeginInvoke(SpoolSpace Variant)
        {

            this._Children.ForEach((x) => { x.BeginInvoke(Variant); });
            this._Increment.BeginInvoke(Variant);

            if (!Variant[this._SpoolSpace].ScalarExists(this._VarName))
            {
                Variant[this._SpoolSpace].SetScalar(this._VarName, new Cell(this._Start.ReturnAffinity()));
                this._Burn = true;
            }

        }

        public override void EndInvoke(SpoolSpace Variant)
        {

            this._Children.ForEach((x) => { x.EndInvoke(Variant); });
            this._Increment.EndInvoke(Variant);

            if (this._Burn)
            {
                Variant[this._SpoolSpace].DeleteScalar(this._VarName);
                this._Burn = false;
            }

        }

        public override void Invoke(SpoolSpace Variant)
        {

            this.Condition = BreakLevel.NoBreak;

            Variant[this._SpoolSpace].SetScalar(this._VarName, this._Start.Evaluate(Variant));

            while (this._Control.Evaluate(Variant).valueBOOL)
            {

                foreach (ActionExpression ae in this._Children)
                {
                    ae.BeginInvoke(Variant);
                    ae.Invoke(Variant);
                    ae.EndInvoke(Variant);
                }

                if (this.Condition == BreakLevel.Return || this.Condition == BreakLevel.Break)
                    break;

                this._Increment.Invoke(Variant);

            }


        }

    }

    public class ActionExpressionEscape : ActionExpression
    {

        public ActionExpressionEscape(Host Host, ActionExpression Parent)
            : base(Host, Parent)
        {
        }

        public override void Invoke(SpoolSpace Variant)
        {
            this.Condition = BreakLevel.Break;
            this.CommunicateBreak();
        }

    }

    public class ActionExpressionReturnVoid : ActionExpression
    {

        public ActionExpressionReturnVoid(Host Host, ActionExpression Parent)
            : base(Host, Parent)
        {
        }

        public override void Invoke(SpoolSpace Variant)
        {
            this.Condition = BreakLevel.Return;
            this.CommunicateReturn();
        }

    }

    public class ActionExpressionReturnScalar : ActionExpression
    {

        private string _MemName;
        private string _VarName;
        private ScalarExpression _Var;

        public ActionExpressionReturnScalar(Host Host, ActionExpression Parent, string MemName, string ReturnVar, ScalarExpression ReturnExp)
            : base(Host, Parent)
        {
            this._MemName = MemName;
            this._VarName = ReturnVar;
            this._Var = ReturnExp;
        }

        public override void Invoke(SpoolSpace Variant)
        {
            this.Condition = BreakLevel.Return;
            this.CommunicateReturn();
            Cell c = this._Var.Evaluate(Variant);
            Variant[this._MemName].SetScalar(this._VarName, c);
        }

    }

    public class ActionExpressionReturnMatrix : ActionExpression
    {

        private string _MemName;
        private string _VarName;
        private MatrixExpression _Var;

        public ActionExpressionReturnMatrix(Host Host, ActionExpression Parent, string MemName, string ReturnVar, MatrixExpression ReturnExp)
            : base(Host, Parent)
        {
            this._MemName = MemName;
            this._VarName = ReturnVar;
            this._Var = ReturnExp;
        }

        public override void Invoke(SpoolSpace Variant)
        {
            this.Condition = BreakLevel.Return;
            this.CommunicateReturn();
            CellMatrix c = this._Var.Evaluate(Variant);
            Variant[this._MemName].SetMatrix(this._VarName, c);
        }

    }

    public class ActionExpressionReturnRecord : ActionExpression
    {

        private string _MemName;
        private string _VarName;
        private RecordExpression _Var;

        public ActionExpressionReturnRecord(Host Host, ActionExpression Parent, string MemName, string ReturnVar, RecordExpression ReturnExp)
            : base(Host, Parent)
        {
            this._MemName = MemName;
            this._VarName = ReturnVar;
            this._Var = ReturnExp;
        }

        public override void Invoke(SpoolSpace Variant)
        {
            this.Condition = BreakLevel.Return;
            this.CommunicateReturn();
            AssociativeRecord c = this._Var.EvaluateAssociative(Variant);
            Variant[this._MemName].SetRecord(this._VarName, c);
        }

    }

    public class ActionExpressionReturnTable : ActionExpression
    {

        private string _MemName;
        private string _VarName;
        private TableExpression _Var;

        public ActionExpressionReturnTable(Host Host, ActionExpression Parent, string MemName, string ReturnVar, TableExpression ReturnExp)
            : base(Host, Parent)
        {
            this._MemName = MemName;
            this._VarName = ReturnVar;
            this._Var = ReturnExp;
        }

        public override void Invoke(SpoolSpace Variant)
        {
            this.Condition = BreakLevel.Return;
            this.CommunicateReturn();
            Table c = this._Var.Select(Variant);
            Variant[this._MemName].SetTable(this._VarName, c);
        }

    }

}
