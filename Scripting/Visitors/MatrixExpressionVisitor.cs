using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Expressions;

namespace ArcticWind.Scripting
{

    public class MatrixExpressionVisitor : ArcticWindParserBaseVisitor<MatrixExpression>
    {

        private Host _Host;
        private MatrixExpression _Master;
        private ObjectFactory _Factory;
        
        public MatrixExpressionVisitor(Host Host)
            : base()
        {
            this._Host = Host;
        }

        // Properties //
        public Host Host
        {
            get { return this._Host; }
        }

        public ObjectFactory Factory
        {
            get { return this._Factory; }
            set { this._Factory = value; }
        }

        public override MatrixExpression VisitMatrixInvert(ArcticWindParser.MatrixInvertContext context)
        {
            MatrixExpression m = new MatrixExpressionUnary.MatrixExpressionInverse(this._Master, this.Visit(context.matrix_expression()));
            this._Master = m;
            return m;
        }

        public override MatrixExpression VisitMatrixTranspose(ArcticWindParser.MatrixTransposeContext context)
        {
            MatrixExpression m = new MatrixExpressionUnary.MatrixExpressionTranspose(this._Master, this.Visit(context.matrix_expression()));
            this._Master = m;
            return m;
        }

        public override MatrixExpression VisitMatrixTrueMul(ArcticWindParser.MatrixTrueMulContext context)
        {
            MatrixExpression m = new MatrixExpressionBinary.MatrixExpressionBinaryMMultiply(this._Master, this.Visit(context.matrix_expression()[0]), this.Visit(context.matrix_expression()[1]));
            this._Master = m;
            return m;
        }

        public override MatrixExpression VisitMatrixMulDiv(ArcticWindParser.MatrixMulDivContext context)
        {

            MatrixExpression m;
            if (context.op.Type == ArcticWindParser.MUL)
                m = new MatrixExpressionBinary.MatrixExpressionBinaryMultiply(this._Master, this.Visit(context.matrix_expression()[0]), this.Visit(context.matrix_expression()[1]));
            else if (context.op.Type == ArcticWindParser.DIV)
                m = new MatrixExpressionBinary.MatrixExpressionBinaryDivide(this._Master, this.Visit(context.matrix_expression()[0]), this.Visit(context.matrix_expression()[1]));
            else
                m = new MatrixExpressionBinary.MatrixExpressionBinaryCheckDivide(this._Master, this.Visit(context.matrix_expression()[0]), this.Visit(context.matrix_expression()[1]));

            this._Master = m;
            return m;

        }

        public override MatrixExpression VisitMatrixAddSub(ArcticWindParser.MatrixAddSubContext context)
        {

            MatrixExpression m;
            if (context.op.Type == ArcticWindParser.PLUS)
                m = new MatrixExpressionBinary.MatrixExpressionBinaryAdd(this._Master, this.Visit(context.matrix_expression()[0]), this.Visit(context.matrix_expression()[1]));
            else
                m = new MatrixExpressionBinary.MatrixExpressionBinarySubtract(this._Master, this.Visit(context.matrix_expression()[0]), this.Visit(context.matrix_expression()[1]));

            this._Master = m;
            return m;

        }

        public override MatrixExpression VisitMatrixLookup(ArcticWindParser.MatrixLookupContext context)
        {

            string Major = (context.matrix_name().lib_name() == null ? this._Factory.Context.PrimaryContextName : context.matrix_name().lib_name().GetText());
            string Minor = context.matrix_name().IDENTIFIER().GetText();

            if (this._Factory.Context.SpoolExists(Major) && this._Factory.Context[Major].MatrixExists(Minor))
            {
                return new MatrixExpressionStoreRef(this._Master, Major, Minor, this._Factory.Context[Major].GetMatrix(Minor).Affinity, this._Factory.Context[Major].GetMatrix(Minor).Size);
            }
            
            throw new Exception(string.Format("Matrix '${0}.{1}' is invalid", Major, Minor));

        }

        public override MatrixExpression VisitMatrixLiteral(ArcticWindParser.MatrixLiteralContext context)
        {

            // Get the record expressions //
            List<ScalarExpressionSet> x = new List<ScalarExpressionSet>();
            foreach (ArcticWindParser.NframeContext ctx in context.nframe())
            {
                x.Add(this._Factory.BaseScalarFactory.Render(ctx));
            }

            // Get the highest affinity and record legnth //
            return new MatrixExpressionLiteral(this._Master, x);

        }

        public override MatrixExpression VisitMatrixCast1(ArcticWindParser.MatrixCast1Context context)
        {

            CellAffinity t = ScriptingHelper.GetTypeAffinity(context.type());
            int s = ScriptingHelper.GetTypeSize(context.type());
            MatrixExpression m = this.Visit(context.matrix_expression());

            MatrixExpression x = new MatrixExpression.MatrixExpressionCast(this._Master, m, t, s);
            this._Master = x;
            return x;

        }

        public override MatrixExpression VisitMatrixCast2(ArcticWindParser.MatrixCast2Context context)
        {
            CellAffinity t = ScriptingHelper.GetTypeAffinity(context.type());
            int s = ScriptingHelper.GetTypeSize(context.type());
            RecordExpression r = this._Factory.BaseRecordVisitor.Render(context.record_expression());

            MatrixExpression x = new MatrixExpression.MatrixExpressionRecordCast(this._Master, r, t, s);
            this._Master = x;
            return x;
        }

        public override MatrixExpression VisitMatrixExpressionFunction(ArcticWindParser.MatrixExpressionFunctionContext context)
        {

            string LibName = ScriptingHelper.GetLibName(context.matrix_name());
            string FuncName = ScriptingHelper.GetVarName(context.matrix_name());

            if (!this._Host.Libraries.Exists(LibName))
                throw new Exception(string.Format("Library does not exist '{0}'", LibName));

            if (!this._Host.Libraries[LibName].MatrixFunctionExists(FuncName))
                throw new Exception(string.Format("Function '{0}' does not exist in '{1}'", FuncName, LibName));

            MatrixExpressionFunction f = this._Host.Libraries[LibName].MatrixFunctionLookup(FuncName);
            foreach (ArcticWindParser.ParamContext ctx in context.param())
            {
                Parameter p = this._Factory.Render(ctx);
                f.AddParameter(p);
            }

            this._Master = f;

            return f;

        }

        public override MatrixExpression VisitMatrixParen(ArcticWindParser.MatrixParenContext context)
        {
            return this.Visit(context.matrix_expression());
        }

        public MatrixExpression Render(ArcticWindParser.Matrix_expressionContext context)
        {
            return this.Visit(context);
        }


    }

}
