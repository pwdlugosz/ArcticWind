using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Expressions;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Elements;
using ArcticWind.Tables;

namespace ArcticWind.Expressions.ActionExpressions
{

    public sealed class ActionExpressionTableCTOR : ActionExpression
    {

        private ScalarExpression _DB;
        private ScalarExpression _Name;
        private Schema _Columns;
        private Key _ClusterColumns;

        public ActionExpressionTableCTOR(Host Host, ActionExpression Parent, ScalarExpression DB, ScalarExpression Name, Schema Columns, Key Cluster)
            : base(Host, Parent)
        {
            this._DB = DB;
            this._Name = Name;
            this._Columns = Columns;
            this._ClusterColumns = Cluster;
        }

        public ActionExpressionTableCTOR(Host Host, ActionExpression Parent, string DB, string Name, Schema Columns, Key Cluster)
            : this(Host, Parent, new ScalarExpressionConstant(null, DB), new ScalarExpressionConstant(null, Name), Columns, Cluster)
        {
        }

        public ActionExpressionTableCTOR(Host Host, ActionExpression Parent, ScalarExpression DB, ScalarExpression Name, Schema Columns)
            : this(Host, Parent, DB, Name, Columns, null)
        {
        }

        public ActionExpressionTableCTOR(Host Host, ActionExpression Parent, string DB, string Name, Schema Columns)
            : this(Host, Parent, DB, Name, Columns, null)
        {
        }

        public override void Invoke(SpoolSpace Variant)
        {

            string db = this._DB.Evaluate(Variant);
            string name = this._Name.Evaluate(Variant);
            if (this._ClusterColumns != null)
            {
                this._Host.CreateTable(db, name, this._Columns, this._ClusterColumns);
            }
            else
            {
                this._Host.CreateTable(db, name, this._Columns);
            }


        }

    }


}
