using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Libraries;
using ArcticWind.Expressions.Aggregates;
using ArcticWind.Tables;
using ArcticWind.Expressions;
using ArcticWind.Expressions.RecordExpressions;


namespace ArcticWind.Scripting
{

    /// <summary>
    /// Represents a visitor for expressions
    /// </summary>
    public class ScalarExpressionVisitor : ArcticWindParserBaseVisitor<ScalarExpression>
    {

        public const string STRING_TAG1 = "'";
        public const string STRING_TAG2 = "\"";
        public const string STRING_TAG3 = "$$";
        public const string STRING_TAB = "TAB";
        public const string STRING_CRLF = "CRLF";
        public const string DATE_TAG1 = "T";
        public const string DATE_TAG2 = "t";
        public const string NUM_TAG1 = "D";
        public const string NUM_TAG2 = "d";

        private Host _Host;
        private ScalarExpression _Master;
        private Heap<int> _PointerSize;
        private Heap<CellAffinity> _PointerAffinity;
        private ObjectFactory _Factory;

        public ScalarExpressionVisitor(Host Host)
            : base()
        {
            this._Host = Host;
            this._PointerSize = new Elements.Heap<int>();
            this._PointerAffinity = new Elements.Heap<CellAffinity>();
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

        // Support Filed Methods //
        //public void AddSchema(string Alias, Schema Columns, out int Pointer) 
        //{
        //    Pointer = this._Map.Local.Records.Count;
        //    this._Map.Local.DeclareRecord(Alias, new AssociativeRecord(Columns));
        //}

        //public void AddSchema(string Alias, Schema Columns)
        //{
        //    int x = 0;
        //    this.AddSchema(Alias, Columns, out x);
        //}

        public void AddPointer(string Alias, CellAffinity Affinity, int Size)
        {
            this._PointerSize.Allocate(Alias, Size);
            this._PointerAffinity.Allocate(Alias, Affinity);
        }

        // Tree walkers //
        public override ScalarExpression VisitPointer(ArcticWindParser.PointerContext context)
        {

            // Get the name, size and affinity //
            string Name = context.IDENTIFIER().GetText();
            int Size = ScriptingHelper.GetTypeSize(context.type());
            CellAffinity Type = ScriptingHelper.GetTypeAffinity(context.type());

            return new ScalarExpressionPointer(this._Master, Name, Type, Size);

        }

        public override ScalarExpression VisitUniary(ArcticWindParser.UniaryContext context)
        {

            ScalarExpression a = this.Visit(context.scalar_expression());
            a.ParentNode = this._Master;

            ScalarExpression f = null;
            if (context.PLUS() != null)
                f = +a;
            else if (context.MINUS() != null)
                f = -a;
            else if (context.NOT() != null)
                f = !a;
            else
                throw new Exception("Unknow opperation");

            this._Master = f;

            return f;

        }

        public override ScalarExpression VisitPower(ArcticWindParser.PowerContext context)
        {

            ScalarExpression a = this.Visit(context.scalar_expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.scalar_expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression f = new ScalarExpressionBinary.ScalarExpressionPower(a,b);
            this._Master = f;
            return f;

        }

        public override ScalarExpression VisitMultDivMod(ArcticWindParser.MultDivModContext context)
        {

            ScalarExpression a = this.Visit(context.scalar_expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.scalar_expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression f = null;

            if (context.MUL() != null)
                f = a * b;
            else if (context.DIV() != null)
                f = a / b;
            else if (context.MOD() != null)
                f = a % b;
            else if (context.DIV2() != null)
                f = ScalarExpression.CDIV(a, b);
            else
                throw new Exception("Unknow opperation");

            this._Master = f;

            return f;

        }

        public override ScalarExpression VisitAddSub(ArcticWindParser.AddSubContext context)
        {

            ScalarExpression a = this.Visit(context.scalar_expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.scalar_expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression f = null;

            if (context.PLUS() != null)
                f = a + b;
            else if (context.MINUS() != null)
                f = a - b;
            else
                throw new Exception("Unknow opperation");

            this._Master = f;

            return f;

        }

        public override ScalarExpression VisitGreaterLesser(ArcticWindParser.GreaterLesserContext context)
        {

            ScalarExpression a = this.Visit(context.scalar_expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.scalar_expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression f = null;

            if (context.GT() != null)
                f = ScalarExpression.GT(a, b);
            else if (context.LT() != null)
                f = ScalarExpression.LT(a, b);
            else if (context.GTE() != null)
                f = ScalarExpression.GTE(a, b);
            else if (context.LTE() != null)
                f = ScalarExpression.LTE(a, b);
            else
                throw new Exception("Unknow opperation");

            this._Master = f;
            return f;

        }

        public override ScalarExpression VisitLogicalAnd(ArcticWindParser.LogicalAndContext context)
        {

            ScalarExpression a = this.Visit(context.scalar_expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.scalar_expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression f = ScalarExpression.AND(a, b);
            this._Master = f;
            return f;

        }

        public override ScalarExpression VisitEquality(ArcticWindParser.EqualityContext context)
        {

            ScalarExpression a = this.Visit(context.scalar_expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.scalar_expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression f = null;

            if (context.EQ() != null)
                f = ScalarExpression.EQ(a, b);
            else if (context.NEQ() != null)
                f = ScalarExpression.NEQ(a, b);
            else
                throw new Exception("Unknow opperation");

            this._Master = f;

            return f;

        }

        public override ScalarExpression VisitLogicalOr(ArcticWindParser.LogicalOrContext context)
        {

            ScalarExpression a = this.Visit(context.scalar_expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.scalar_expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression f = null;

            if (context.OR() != null)
                f = ScalarExpression.OR(a, b);
            else if (context.XOR() != null)
                f = ScalarExpression.XOR(a, b);
            else
                throw new Exception("Unknow opperation");

            this._Master = f;

            return f;

        }

        public override ScalarExpression VisitBitShiftRotate(ArcticWindParser.BitShiftRotateContext context)
        {

            ScalarExpression l = this.Visit(context.scalar_expression()[0]);
            ScalarExpression r = this.Visit(context.scalar_expression()[1]);

            ScalarExpression e = null;
            if (context.L_SHIFT() != null)
            {
                e = new ScalarExpressionBinary.ScalarExpressionLeftShift(l, r);
            }
            else if (context.L_ROTATE() != null)
            {
                e = new ScalarExpressionBinary.ScalarExpressionLeftRotate(l, r);
            }
            else if (context.R_SHIFT() != null)
            {
                e = new ScalarExpressionBinary.ScalarExpressionRightShift(l, r);
            }
            else if (context.R_ROTATE() != null)
            {
                e = new ScalarExpressionBinary.ScalarExpressionRightRotate(l, r);
            }
            else
            {
                throw new Exception();
            }

            this._Master = e;

            return e;


        }

        public override ScalarExpression VisitScalarMember(ArcticWindParser.ScalarMemberContext context)
        {
            
            string Major = (context.scalar_name().lib_name() == null ? this._Factory.Context.PrimaryContextName : context.scalar_name().lib_name().GetText());
            string Minor = context.scalar_name().IDENTIFIER().GetText();

            if (this._Factory.Context.SpoolExists(Major) && this._Factory.Context[Major].ScalarExists(Minor))
            {
                return new ScalarExpressionStoreRef(this._Host, this._Master, Major, Minor, this._Factory.Context[Major].GetScalar(Minor).Affinity, this._Factory.Context[Major].GetScalar(Minor).Size);
            }
            else if (!this._Factory.Context.SpoolExists(Major))
            {
                throw new Exception(string.Format("Spool '{0}' does not exist", Major));
            }
            throw new Exception(string.Format("Field '{0}.{1}' is invalid", Major, Minor));

        }

        public override ScalarExpression VisitRecordMember(ArcticWindParser.RecordMemberContext context)
        {
            string Major = (context.record_name().lib_name() == null ? this._Factory.Context.PrimaryContextName : context.record_name().lib_name().GetText());
            string Medium = ScriptingHelper.GetVarName(context.record_name());
            string Minor = context.IDENTIFIER().GetText();
            return new ScalarExpressionRecordRef(this._Host, this._Master, Major, Medium, Minor, this._Factory.Context[Major].GetRecord(Medium).ColumnAffinity(Minor), this._Factory.Context[Major].GetRecord(Medium).ColumnSize(Minor));
        }

        public override ScalarExpression VisitMatrixMember(ArcticWindParser.MatrixMemberContext context)
        {

            ScalarExpression row = this.Visit(context.scalar_expression()[0]);
            ScalarExpression col = (context.scalar_expression().Length >= 2 ? this.Visit(context.scalar_expression()[1]) : new ScalarExpressionConstant(null, CellValues.ZeroINT));
            MatrixExpression m = this._Factory.BaseMatrixFactory.Render(context.matrix_expression());
            ScalarExpressionMatrixRef s = new ScalarExpressionMatrixRef(this._Host, this._Master, m, row, col);
            return s;

        }

        public override ScalarExpression VisitLiteralBool(ArcticWindParser.LiteralBoolContext context)
        {
            string s = context.GetText();
            return new ScalarExpressionConstant(this._Master, CellParser.Parse(s, CellAffinity.BOOL));
        }

        public override ScalarExpression VisitLiteralDateTime(ArcticWindParser.LiteralDateTimeContext context)
        {
            string s = context.GetText();
            s = s.Replace("T", "").Replace("t", "");
            s = CleanString(s);
            return new ScalarExpressionConstant(this._Master, CellParser.Parse(s, CellAffinity.DATE_TIME));
        }

        public override ScalarExpression VisitLiteralByte(ArcticWindParser.LiteralByteContext context)
        {
            string s = context.GetText();
            return new ScalarExpressionConstant(this._Master, CellParser.Parse(s, CellAffinity.BYTE));
        }

        public override ScalarExpression VisitLiteralShort(ArcticWindParser.LiteralShortContext context)
        {
            string s = context.GetText();
            return new ScalarExpressionConstant(this._Master, CellParser.Parse(s, CellAffinity.SHORT));
        }

        public override ScalarExpression VisitLiteralInt(ArcticWindParser.LiteralIntContext context)
        {
            string s = context.GetText();
            return new ScalarExpressionConstant(this._Master, CellParser.Parse(s, CellAffinity.INT));
        }

        public override ScalarExpression VisitLiteralLong(ArcticWindParser.LiteralLongContext context)
        {
            string s = context.GetText();
            return new ScalarExpressionConstant(this._Master, CellParser.Parse(s, CellAffinity.LONG));
        }

        public override ScalarExpression VisitLiteralSingle(ArcticWindParser.LiteralSingleContext context)
        {
            string s = context.GetText();
            s = s.Replace("F", "").Replace("f", "");
            return new ScalarExpressionConstant(this._Master, CellParser.Parse(s, CellAffinity.SINGLE));
        }

        public override ScalarExpression VisitLiteralDouble(ArcticWindParser.LiteralDoubleContext context)
        {
            string s = context.GetText();
            s = s.Replace("D", "").Replace("d", "");
            return new ScalarExpressionConstant(this._Master, CellParser.Parse(s, CellAffinity.DOUBLE));
        }

        public override ScalarExpression VisitLiteralBinary(ArcticWindParser.LiteralBinaryContext context)
        {
            string s = context.GetText();
            return new ScalarExpressionConstant(this._Master, CellParser.Parse(s, CellAffinity.BINARY));
        }

        public override ScalarExpression VisitLiteralBString(ArcticWindParser.LiteralBStringContext context)
        {
            string s = context.GetText();
            if (s.Last() == 'b' || s.Last() == 'B')
                s = s.Substring(0, s.Length - 1);
            s = CleanString(s);
            return new ScalarExpressionConstant(this._Master, CellParser.Parse(s, CellAffinity.BSTRING));
        }

        public override ScalarExpression VisitLiteralCString(ArcticWindParser.LiteralCStringContext context)
        {
            string s = context.GetText();
            if (s.Last() == 'c' || s.Last() == 'C')
                s = s.Substring(0, s.Length - 1);
            s = CleanString(s);
            return new ScalarExpressionConstant(this._Master, CellParser.Parse(s, CellAffinity.CSTRING));
        }

        public override ScalarExpression VisitLiteralNull(ArcticWindParser.LiteralNullContext context)
        {
            return new ScalarExpressionConstant(this._Master, CellValues.NullLONG);
        }

        public override ScalarExpression VisitExpressionType(ArcticWindParser.ExpressionTypeContext context)
        {
            CellAffinity x = ScriptingHelper.GetTypeAffinity(context.type());
            byte y = (byte)x;
            return new ScalarExpressionConstant(this._Master, new Cell(y));
        }

        public override ScalarExpression VisitIfNullOp(ArcticWindParser.IfNullOpContext context)
        {

            ScalarExpression a = this.Visit(context.scalar_expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.scalar_expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression f = ScalarExpression.IFNULL(a, b);
            this._Master = f;
            return f;

        }

        public override ScalarExpression VisitIfOp(ArcticWindParser.IfOpContext context)
        {

            ScalarExpression a = this.Visit(context.scalar_expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.scalar_expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression c = (context.scalar_expression().Length == 3 ? this.Visit(context.scalar_expression()[2]) : new ScalarExpressionConstant(this._Master, new Cell(b.ReturnAffinity())));
            c.ParentNode = this._Master;

            ScalarExpression f = ScalarExpression.IF(a, b, c);

            this._Master = f;
            return f;

        }

        public override ScalarExpression VisitScalarExpressionFunction(ArcticWindParser.ScalarExpressionFunctionContext context)
        {

            // Need to figure out the library //
            string FuncName = ScriptingHelper.GetVarName(context.scalar_name());
            string LibName = null;
            if (context.scalar_name().lib_name() == null)
            {
                if (this._Host.BaseLibrary.ScalarFunctionExists(FuncName))
                    LibName = Host.GLOBAL;
                else if (this._Host.UserLibrary.ScalarFunctionExists(FuncName))
                    LibName = Host.USER;
                else
                    throw new Exception(string.Format("Scalar '{0}' does not exist", FuncName));
            }
            else
            {
                LibName = context.scalar_name().lib_name().GetText();
            }

            if (!this._Host.Libraries.Exists(LibName))
                throw new Exception(string.Format("Library does not exist '{0}'", LibName));

            if (!this._Host.Libraries[LibName].ScalarFunctionExists(FuncName))
                throw new Exception(string.Format("Function '{0}' does not exist in '{1}'", FuncName, LibName));

            ScalarExpressionFunction f = this._Host.Libraries[LibName].ScalarFunctionLookup(FuncName);
            foreach (Parameter p in this._Factory.Render(context.param()))
            {
                f.AddParameter(p);
            }

            this._Master = f;

            return f;
        }

        public override ScalarExpression VisitCast(ArcticWindParser.CastContext context)
        {

            CellAffinity t = ScriptingHelper.GetTypeAffinity(context.type());
            ScalarExpression s = this.Visit(context.scalar_expression());
            ScalarExpression x = ScalarExpression.CAST(s, t);

            this._Master = x;

            return x;

        }

        public override ScalarExpression VisitParens(ArcticWindParser.ParensContext context)
        {
            return this.Visit(context.scalar_expression());
        }

        // Visit Expressions //
        public ScalarExpression Render(ArcticWindParser.Scalar_expressionContext context)
        {
            this._Master = null;
            return this.Visit(context);
        }

        public ScalarExpressionSet Render(ArcticWindParser.NframeContext context)
        {
            ScalarExpressionSet rex = new ScalarExpressionSet(this._Host);
            if (context != null)
            {
                foreach (ArcticWindParser.NelementContext s in context.nelement())
                {
                    ScalarExpression se = this.Render(s.scalar_expression());
                    string alias = (s.IDENTIFIER() == null ? se.BuildAlias() : s.IDENTIFIER().GetText());

                    rex.Add(alias, se);
                }
            }
            return rex;
        }

        public Filter Render(ArcticWindParser.WhereContext context)
        {
            if (context == null)
            {
                return Filter.TrueForAll;
            };
            ScalarExpression se = this.Render(context.scalar_expression());
            return new Filter(se);
        }

        // Visit Aggregate //
        public Aggregate Render(ArcticWindParser.AggContext context)
        {

            // Get the paramters //
            ScalarExpressionSet rex = this.Render(context.nframe());

            // Get the filter //
            Filter f = this.Render(context.where());

            // Get the aggregate //
            string name = context.SET_REDUCTIONS().GetText();
            AggregateLookup al = new AggregateLookup();

            // Check if it exists //
            if (!al.Exists(name))
                throw new Exception(string.Format("Aggregate '{0}' does not exist", name));

            return al.Lookup(name, rex, f);

        }

        // Visit Aggregate Collection //
        public AggregateCollection Render(ArcticWindParser.AframeContext context)
        {

            AggregateCollection ac = new AggregateCollection();

            foreach (ArcticWindParser.AggContext ctx in context.agg())
            {
                string alias = (ctx.IDENTIFIER() == null ? "A" + ac.Count.ToString() : ctx.IDENTIFIER().GetText());
                Aggregate a = this.Render(ctx);
                ac.Add(alias, a);
            }

            return ac;

        }

        // Cloning //
        public ScalarExpressionVisitor CloneOfMe()
        {

            // Create the visitor //
            ScalarExpressionVisitor sev = new ScalarExpressionVisitor(this._Host);

            // Add all the pointers //
            for (int i = 0; i < this._PointerAffinity.Count; i++)
            {
                this.AddPointer(this._PointerAffinity.Name(i), this._PointerAffinity[i], this._PointerSize[i]);
            }

            return sev;

        }

        // Statics //
        /// <summary>
        /// Cleans an incoming string
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static string CleanString(string Value)
        {

            // Check for empty text //
            if (Value == "" || Value == "''" || Value == "$$$$" || Value == "\"\"")
                return "";

            // Check for tab //
            if (Value.ToUpper() == "TAB")
                return "\t";

            // Check for newline //
            if (Value.ToUpper() == "CRLF")
                return "\n";

            // Check for "'", $$'$$, '"', $$"$$ //
            if (Value == "\"'\"" || Value == "$$'$$")
                return "'";
            if (Value == "'\"'" || Value == "$$\"$$")
                return "\"";

            // Check for lengths less than two //
            if (Value.Length < 2)
            {
                return Value.Replace("\\n", "\n").Replace("\\t", "\t");
            }

            // Handle 'ABC' to ABC //
            if (Value.First() == '\'' && Value.Last() == '\'' && Value.Length >= 2)
            {
                Value = Value.Substring(1, Value.Length - 2);
                while (Value.Contains("''"))
                {
                    Value = Value.Replace("''", "'");
                }
            }

            // Handle "ABC" to ABC //
            if (Value.First() == '"' && Value.Last() == '"' && Value.Length >= 2)
            {
                Value = Value.Substring(1, Value.Length - 2);
                while (Value.Contains("\"\""))
                {
                    Value = Value.Replace("\"\"", "\"");
                }
            }

            // Check for lengths less than four //
            if (Value.Length < 4)
            {
                return Value.Replace("\\n", "\n").Replace("\\t", "\t");
            }

            // Handle $$ABC$$ to ABC //
            int Len = Value.Length;
            if (Value[0] == '$' && Value[1] == '$' && Value[Len - 2] == '$' && Value[Len - 1] == '$')
            {
                Value = Value.Substring(2, Value.Length - 4);
                while (Value.Contains("$$$$"))
                {
                    Value = Value.Replace("$$$$", "$$");
                }
            }

            // Otherwise, return Value //
            return Value.Replace("\\n", "\n").Replace("\\t", "\t");

        }

    }


}
