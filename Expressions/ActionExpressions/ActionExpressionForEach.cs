using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Tables;
using ArcticWind.Expressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.TableExpressions;

namespace ArcticWind.Expressions.ActionExpressions
{
    
    public class ActionExpressionForEachTable : ActionExpression
    {

        private TableExpression _t;
        private string _SpoolName;
        private string _RecordName;

        public ActionExpressionForEachTable(Host Host, ActionExpression Parent, TableExpression Data, string NameSpace, string Alias)
            : base(Host, Parent)
        {
            this._t = Data;
            this._SpoolName = NameSpace;
            this._RecordName = Alias;
        }

        public override void BeginInvoke(SpoolSpace Variant)
        {
            this._Children.ForEach((x) => x.BeginInvoke(Variant));
        }

        public override void EndInvoke(SpoolSpace Variant)
        {
            this._Children.ForEach((x) => x.EndInvoke(Variant));
        }

        public override void Invoke(SpoolSpace Variant)
        {

            Table t = this._t.Select(Variant);
            RecordReader rr = t.OpenReader();

            while (rr.CanAdvance)
            {
                Variant[this._SpoolName].SetRecord(this._RecordName, new AssociativeRecord(t.Columns, rr.ReadNext())); 
                this._Children.ForEach((x) => { x.Invoke(Variant); });
            }

            if (this._Host.IsSystemTemp(t))
                this._Host.TableStore.DropTable(t.Key);

        }

        public override SpoolSpace CreateResolver()
        {
            SpoolSpace f = base.CreateResolver();
            return f;
        }

    }

    public class ActionExpressionForEachMatrixExpression : ActionExpression
    {

        private MatrixExpression _Val;
        private string _Lib;
        private string _Name;
        private bool _Trigger = false;

        public ActionExpressionForEachMatrixExpression(Host Host, ActionExpression Parent, MatrixExpression Enumerator, string VarLib, string VarName)
            : base(Host, Parent)
        {
            this._Val = Enumerator;
            this._Lib = VarLib;
            this._Name = VarName;
        }

        public override void BeginInvoke(SpoolSpace Variant)
        {

            if (!Variant.Exists(this._Lib))
            {
                throw new Exception(string.Format("Object store '{0}' does not exist", this._Lib));
            }
            if (!Variant[this._Lib].ScalarExists(this._Name))
            {
                Variant[this._Lib].SetScalar(this._Name, new Cell(this._Val.ReturnAffinity()));
                this._Trigger = true;
            }

        }

        public override void Invoke(SpoolSpace Variant)
        {

            CellMatrix m = this._Val.Evaluate(Variant);

            for (int i = 0; i < m.RowCount; i++)
            {
                for (int j = 0; j < m.ColumnCount; j++)
                {
                    Cell c = m[i, j];
                    Variant[this._Lib].SetScalar(this._Name, c);
                    this._Children.ForEach((x) => { x.Invoke(Variant); });
                }
            }

        }

        public override void EndInvoke(SpoolSpace Variant)
        {
            if (this._Trigger)
            {
                Variant[this._Lib].DeleteScalar(this._Name);
                this._Trigger = false;
            }
        }

    }

    public class ActionExpressionForEachMatrix : ActionExpression
    {

        private string _MatLib;
        private string _MatName;
        private string _VarLib;
        private string _VarName;
        private bool _Trigger = false;

        public ActionExpressionForEachMatrix(Host Host, ActionExpression Parent, string MatrixLib, string MatrixName, string VarLib, string VarName)
            : base(Host, Parent)
        {
            this._MatLib = MatrixLib;
            this._MatName = MatrixName;
            this._VarLib = VarLib;
            this._VarName = VarName;
        }

        public override void BeginInvoke(SpoolSpace Variant)
        {

            if (!Variant.Exists(this._VarLib))
            {
                throw new Exception(string.Format("Object store '{0}' does not exist", this._VarLib));
            }
            if (!Variant[this._VarLib].ScalarExists(this._VarName))
            {
                Variant[this._VarLib].SetScalar(this._VarName, new Cell(Variant[this._MatLib].GetMatrixAffinity(this._MatName)));
                this._Trigger = true;
            }


        }

        public override void Invoke(SpoolSpace Variant)
        {

            CellMatrix m = Variant[this._MatLib].GetMatrix(this._MatName);

            for (int i = 0; i < m.RowCount; i++)
            {
                for (int j = 0; j < m.ColumnCount; j++)
                {
                    Cell c = m[i, j];
                    Variant[this._VarLib].SetScalar(this._VarName,c);
                    this._Children.ForEach((x) => { x.Invoke(Variant); });
                    m[i, j] = Variant[this._VarLib].GetScalar(this._VarName);
                }
            }
            
        }

        public override void EndInvoke(SpoolSpace Variant)
        {
            if (this._Trigger)
            {
                Variant[this._VarLib].DeleteScalar(this._VarName);
                this._Trigger = false;
            }
        }

    }

    //public class ActionExpressionForEachByteInBinary : ActionExpression
    //{
    //}

    //public class ActionExpressionForEachBStringInBString : ActionExpression
    //{
    //}

    //public class ActionExpressionForEachCStringInCString : ActionExpression
    //{
    //}

}
