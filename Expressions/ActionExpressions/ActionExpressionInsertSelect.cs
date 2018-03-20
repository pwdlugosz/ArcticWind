using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.TableExpressions;
using ArcticWind.Tables;
using ArcticWind.Expressions;

namespace ArcticWind.Expressions.ActionExpressions
{

    /// <summary>
    /// Represents an action that appends records to a table 
    /// </summary>
    public sealed class ActionExpressionInsertSelect : ActionExpression
    {

        private RecordWriter _Writer;
        private TableExpression _Select;
        
        public ActionExpressionInsertSelect(Host Host, ActionExpression Parent, RecordWriter Writer, TableExpression Select)
            : base(Host, Parent)
        {

            // Need to check that the input and output table are compatible //
            if (Writer.Columns.Count != Select.Columns.Count)
                throw new Exception("Destination table and input expression have different field counts");

            this._Writer = Writer;
            this._Select = Select;
        }

        public override void Invoke(SpoolSpace Variant)
        {

            // Write the data to a table //
            this._Select.Append(Variant, this._Writer);

        }

        public override void EndInvoke(SpoolSpace Variant)
        {
            this._Writer.Close();
        }

        public override SpoolSpace CreateResolver()
        {
            return new SpoolSpace();
        }

    }


}
