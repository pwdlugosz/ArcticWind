using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Libraries;
using ArcticWind.Expressions.Aggregates;
using ArcticWind.Tables;
using ArcticWind.Expressions;


namespace ArcticWind.Scripting
{

    public class RecordExpressionVisitor : ArcticWindParserBaseVisitor<RecordExpression>
    {

        private Host _Host;
        private RecordExpression _Master;
        private ObjectFactory _Factory;
        
        public RecordExpressionVisitor(Host Host)
            : base()
        {
            this._Host = Host;
        }

        public Host Host
        {
            get { return this._Host; }
        }

        public ObjectFactory Factory
        {
            get { return this._Factory; }
            set { this._Factory = value; }
        }

        public override RecordExpression VisitRecordExpressionLiteral(ArcticWindParser.RecordExpressionLiteralContext context)
        {

            RecordExpressionLiteral rex = new RecordExpressionLiteral(this._Host, this._Master);
            int cnt = 0;

            foreach (ArcticWindParser.NelementContext ctx in context.nframe().nelement())
            {
                string name = (ctx.IDENTIFIER() == null ? "F" + cnt.ToString() : ctx.IDENTIFIER().GetText());
                ScalarExpression sx = this._Factory.BaseScalarFactory.Visit(ctx.scalar_expression());
                rex.Add(sx, name);
                cnt++;
            }

            return rex;
        }

        public override RecordExpression VisitRecordExpressionLookup(ArcticWindParser.RecordExpressionLookupContext context)
        {

            string Major = (context.record_name().lib_name() == null ? this._Factory.Context.PrimaryContextName : context.record_name().lib_name().GetText());
            string Minor = context.record_name().IDENTIFIER().GetText();

            if (this._Factory.Context.SpoolExists(Major) && this._Factory.Context[Major].RecordExists(Minor))
            {
                return new RecordExpressionStoreRef(this._Host, this._Master, Major, Minor, this._Factory.Context[Major].GetRecord(Minor));
            }

            throw new Exception(string.Format("Record '{0}.@{1}' is invalid", Major, Minor));

        }

        public override RecordExpression VisitRecordExpressionShell(ArcticWindParser.RecordExpressionShellContext context)
        {
            Schema s = ScriptingHelper.GetSchema(context.schema());
            return new RecordExpressionCTOR(this._Host, this._Master, s);
        }

        public override RecordExpression VisitRecordExpressionParens(ArcticWindParser.RecordExpressionParensContext context)
        {
            return this.Visit(context.record_expression());
        }

        public override RecordExpression VisitRecordExpressionFunction(ArcticWindParser.RecordExpressionFunctionContext context)
        {

            string LibName = ScriptingHelper.GetLibName(context.record_name());
            string FuncName = ScriptingHelper.GetVarName(context.record_name());

            if (!this._Host.Libraries.Exists(LibName))
                throw new Exception(string.Format("Library does not exist '{0}'", LibName));

            if (!this._Host.Libraries[LibName].ScalarFunctionExists(FuncName))
                throw new Exception(string.Format("Function '{0}' does not exist in '{1}'", FuncName, LibName));

            RecordExpressionFunction f = this._Host.Libraries[LibName].RecordFunctionLookup(FuncName);
            foreach (ArcticWindParser.ParamContext ctx in context.param())
            {
                Parameter p = this._Factory.Render(ctx);
                f.AddParameter(p);
            }

            this._Master = f;

            return f;

        }

        public RecordExpression Render(ArcticWindParser.Record_expressionContext context)
        {
            return this.Visit(context);
        }

    }

}
