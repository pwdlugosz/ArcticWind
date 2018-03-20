using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Expressions.TableExpressions;
using ArcticWind.Expressions.ActionExpressions;
using Antlr4;
using Antlr4.Runtime;
using ArcticWind.Tables;
using ArcticWind.Expressions;

namespace ArcticWind.Scripting
{

    public class ScriptProcessor
    {

        private Host _Host;

        public ScriptProcessor(Host Host)
        {
            this._Host = Host;
            this._Host.Engine = this;
        }

        public ScalarExpression ToScalar(string Script)
        {

            // Create a token stream and do lexal analysis //
            AntlrInputStream TextStream = new AntlrInputStream(Script);
            ArcticWindLexer Lex = new ArcticWindLexer(TextStream);

            // Parse the script //
            CommonTokenStream TokenStream = new CommonTokenStream(Lex);
            ArcticWindParser HeartBeat = new ArcticWindParser(TokenStream);

            // Create an executer object //
            ScalarExpressionVisitor processor = new ScalarExpressionVisitor(this._Host);

            ArcticWindParser.Scalar_expressionContext context = HeartBeat.compileUnit().scalar_expression();

            // Handle no expressions //
            if (context == null)
                return null;

            return processor.Render(context);

        }

        public MatrixExpression ToMatrixExpression(string Script)
        {

            // Create a token stream and do lexal analysis //
            AntlrInputStream TextStream = new AntlrInputStream(Script);
            ArcticWindLexer Lex = new ArcticWindLexer(TextStream);

            // Parse the script //
            CommonTokenStream TokenStream = new CommonTokenStream(Lex);
            ArcticWindParser HeartBeat = new ArcticWindParser(TokenStream);

            // Create an executer object //
            MatrixExpressionVisitor processor = new MatrixExpressionVisitor(this._Host);

            ArcticWindParser.Matrix_expressionContext context = HeartBeat.compileUnit().matrix_expression();

            // Handle no expressions //
            if (context == null)
                return null;

            return processor.Render(context);

        }

        public RecordExpression ToRecordExpression(string Script)
        {

            // Create a token stream and do lexal analysis //
            AntlrInputStream TextStream = new AntlrInputStream(Script);
            ArcticWindLexer Lex = new ArcticWindLexer(TextStream);

            // Parse the script //
            CommonTokenStream TokenStream = new CommonTokenStream(Lex);
            ArcticWindParser HeartBeat = new ArcticWindParser(TokenStream);

            // Create an executer object //
            RecordExpressionVisitor processor = new RecordExpressionVisitor(this._Host);

            ArcticWindParser.Record_expressionContext context = HeartBeat.compileUnit().record_expression();

            // Handle no expressions //
            if (context == null)
                return null;

            return processor.Render(context);

        }

        public TableExpression ToTableExpression(string Script)
        {

            // Create a token stream and do lexal analysis //
            AntlrInputStream TextStream = new AntlrInputStream(Script);
            ArcticWindLexer Lex = new ArcticWindLexer(TextStream);

            // Parse the script //
            CommonTokenStream TokenStream = new CommonTokenStream(Lex);
            ArcticWindParser HeartBeat = new ArcticWindParser(TokenStream);

            // Create an executer object //
            TableExpressionVisitor processor = new TableExpressionVisitor(this._Host);

            ArcticWindParser.Table_expressionContext context = HeartBeat.compileUnit().table_expression();

            // Handle no expressions //
            if (context == null)
                return null;

            return processor.Render(context);

        }

        public ActionExpression ToActionExpression(string Script)
        {

            // Create a token stream and do lexal analysis //
            AntlrInputStream TextStream = new AntlrInputStream(Script);
            ArcticWindLexer HorseLexer = new ArcticWindLexer(TextStream);

            // Parse the script //
            CommonTokenStream RyeTokenStream = new CommonTokenStream(HorseLexer);
            ArcticWindParser HeartBeat = new ArcticWindParser(RyeTokenStream);

            // Create an executer object //
            ActionExpressionVisitor processor = new ActionExpressionVisitor(this._Host);
            
            ArcticWindParser.Action_expressionContext[] actions = HeartBeat.compileUnit().action_expression();

            // Handle no expressions //
            if (actions == null)
                return null;
            
            ActionExpressionDo x = new ActionExpressionDo(this._Host, null);
            foreach (ArcticWindParser.Action_expressionContext ctx in actions)
            {
                x.AddChild(processor.Visit(ctx));
            }
            return x;

        }

        public Cell RenderScalar(string Script)
        {
            return this.ToScalar(Script).Evaluate(this._Host.Spools);
        }

        public CellMatrix RenderMatrix(string Script)
        {
            return this.ToMatrixExpression(Script).Evaluate(this._Host.Spools);
        }

        public Table RenderTable(string Script)
        {
            return this.ToTableExpression(Script).Select(this._Host.Spools);
        }

        public void RenderAction(string Script)
        {

            // Create a token stream and do lexal analysis //
            AntlrInputStream TextStream = new AntlrInputStream(Script);
            ArcticWindLexer HorseLexer = new ArcticWindLexer(TextStream);

            // Parse the script //
            CommonTokenStream RyeTokenStream = new CommonTokenStream(HorseLexer);
            ArcticWindParser HeartBeat = new ArcticWindParser(RyeTokenStream);

            // Create an executer object //
            ActionExpressionVisitor processor = new ActionExpressionVisitor(this._Host);

            ArcticWindParser.Action_expressionContext[] actions = HeartBeat.compileUnit().action_expression();

            // Handle no expressions //
            if (actions == null)
                return;

            // Create spool-space //
            SpoolSpace ss = this._Host.Spools;

            foreach (ArcticWindParser.Action_expressionContext ctx in actions)
            {

                ActionExpression x = processor.Visit(ctx);
                x.BeginInvoke(ss);
                x.Invoke(ss);
                x.EndInvoke(ss);

            }


        }

    }

}
