using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.ActionExpressions;
using ArcticWind.Expressions.TableExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Libraries;
using ArcticWind.Tables;
using ArcticWind.Expressions;

namespace ArcticWind.Scripting
{

    public class ActionExpressionVisitor : ArcticWindParserBaseVisitor<ActionExpression>
    {

        private Host _Host;
        private Heap<RecordWriter> _OpenRecordStreams;
        private Heap<StreamWriter> _OpenTextStreams;

        private ActionExpression _Master;
        private ObjectFactory _Factory;
        
        public ActionExpressionVisitor(Host Host)
            : base()
        {
            this._Host = Host;
            this._OpenRecordStreams = new Heap<RecordWriter>();
            this._OpenTextStreams = new Heap<StreamWriter>();
            this._Factory = new ObjectFactory(this._Host);
        }

        // Properties //
        public Host Host
        {
            get { return this._Host; }
        }

        public Heap<RecordWriter> OpenRecordStreams
        {
            get { return this._OpenRecordStreams; }
        }

        public Heap<StreamWriter> OpenTextWriters
        {
            get { return this._OpenTextStreams; }
        }

        public override ActionExpression VisitDeclareScalar(ArcticWindParser.DeclareScalarContext context)
        {
            string Lib = (context.scalar_name().lib_name() == null ? this._Factory.Context.PrimaryContextName : context.scalar_name().lib_name().GetText());
            string Name = ScriptingHelper.GetVarName(context.scalar_name());
            ScalarExpression s = this._Factory.BaseScalarFactory.Render(context.scalar_expression());
            this._Factory.Context.AddScalar(Lib, Name, s.ReturnAffinity(), s.ReturnSize());
            return new ActionExpressionDeclareScalar(this._Host, this._Master, Lib, Name, s);
        }

        public override ActionExpression VisitDeclareMatrix(ArcticWindParser.DeclareMatrixContext context)
        {
            string Lib = (context.matrix_name().lib_name() == null ? this._Factory.Context.PrimaryContextName : context.matrix_name().lib_name().GetText());
            string Name = ScriptingHelper.GetVarName(context.matrix_name());
            MatrixExpression m = this._Factory.BaseMatrixFactory.Render(context.matrix_expression());
            this._Factory.Context.AddMatrix(Lib, Name, m.ReturnAffinity(), m.ReturnSize());
            return new ActionExpressionDeclareMatrix(this._Host, this._Master, Lib, Name, m);
        }

        public override ActionExpression VisitDeclareRecord(ArcticWindParser.DeclareRecordContext context)
        {
            string Lib = (context.record_name().lib_name() == null ? this._Factory.Context.PrimaryContextName : context.record_name().lib_name().GetText());
            string Name = ScriptingHelper.GetVarName(context.record_name());
            RecordExpression x = this._Factory.BaseRecordVisitor.Render(context.record_expression());
            this._Factory.Context.AddRecord(Lib, Name, x.Columns);
            return new ActionExpressionDeclareRecord(this._Host, this._Master, Lib, Name, x);
        }

        public override ActionExpression VisitDeclareTable1(ArcticWindParser.DeclareTable1Context context)
        {

            string SpoolName = context.table_name().lib_name() == null ? Host.GLOBAL : context.table_name().lib_name().GetText();
            string MemName = context.table_name().IDENTIFIER().GetText();

            ScalarExpression DirPath = ScalarExpression.Value(this._Host.TempDB);
            ScalarExpression DiskName = ScalarExpression.Value(MemName);

            Schema cols = ScriptingHelper.GetSchema(context.schema());

            TableExpressionCTOR x = new TableExpressionCTOR(this._Host, null, cols, new Key());

            ActionExpressionDeclareTable1 y = new ActionExpressionDeclareTable1(this._Host, this._Master, SpoolName, MemName, DirPath, DiskName, x);
            return y;

        }

        public override ActionExpression VisitDeclareTable2(ArcticWindParser.DeclareTable2Context context)
        {

            string SpoolName = context.table_name().lib_name() == null ? Host.GLOBAL : context.table_name().lib_name().GetText();
            string MemName = context.table_name().IDENTIFIER().GetText();
            ScalarExpression DirPath = ScalarExpression.Value(this._Host.TempDB);
            ScalarExpression DiskName = ScalarExpression.Value(MemName);
            TableExpression x = this._Factory.BaseTableVisitor.Render(context.table_expression());
            return new ActionExpressionDeclareTable1(this._Host, this._Master, SpoolName, MemName, DirPath, DiskName, x);

        }

        public override ActionExpression VisitDeclareTable3(ArcticWindParser.DeclareTable3Context context)
        {
            string SpoolName = context.table_name().lib_name() == null ? Host.GLOBAL : context.table_name().lib_name().GetText();
            string MemName = context.table_name().IDENTIFIER().GetText();
            ScalarExpression DirPath = ScalarExpression.Value(this._Host.TempDB);
            ScalarExpression DiskName = ScalarExpression.Value(MemName);
            ScalarExpression SourcePath = this._Factory.BaseScalarFactory.Render(context.scalar_expression()[0]);
            ScalarExpression SourceName = this._Factory.BaseScalarFactory.Render(context.scalar_expression()[1]);

            return new ActionExpressionDeclareTable2(this._Host, this._Master, SpoolName, MemName, DirPath, DiskName, SourcePath, SourceName);
        }

        public override ActionExpression VisitActionScalarAssign(ArcticWindParser.ActionScalarAssignContext context)
        {

            // Figure out what we're assigning //
            string Lib = (context.scalar_name().lib_name() == null ? this._Factory.Context.PrimaryContextName : context.scalar_name().lib_name().GetText());
            string Name = ScriptingHelper.GetVarName(context.scalar_name());
            Assignment x = ScriptingHelper.GetAssignment(context.assignment());

            // Get the expression //
            ScalarExpression s = this._Factory.BaseScalarFactory.Render(context.scalar_expression());
            return new ActionExpressionScalarAssign(this._Host, this._Master, Lib, Name, s, x);

        }

        public override ActionExpression VisitActionScalarIncrement(ArcticWindParser.ActionScalarIncrementContext context)
        {

            string Lib = (context.scalar_name().lib_name() == null ? this._Factory.Context.PrimaryContextName : context.scalar_name().lib_name().GetText());
            string Name = ScriptingHelper.GetVarName(context.scalar_name());
            Cell one = CellValues.One(this._Factory.Context[Lib].GetScalar(Name).Affinity);
            Assignment x = Assignment.PlusEquals;
            if (context.increment().PLUS() == null) 
                x = Assignment.MinusEquals;

            return new ActionExpressionScalarAssign(this._Host, this._Master, Lib, Name, new ScalarExpressionConstant(null, one), x);

        }

        public override ActionExpression VisitActionMatrixAssign(ArcticWindParser.ActionMatrixAssignContext context)
        {

            // Figure out what we're assigning //
            string Lib = (context.matrix_name().lib_name() == null ? this._Factory.Context.PrimaryContextName : context.matrix_name().lib_name().GetText());
            string Name = ScriptingHelper.GetVarName(context.matrix_name());
            Assignment x = ScriptingHelper.GetAssignment(context.assignment());

            // Get the expression //
            MatrixExpression m = this._Factory.BaseMatrixFactory.Render(context.matrix_expression());
            return new ActionExpressionMatrixAssign(this._Host, this._Master, Lib, Name, m, x);

        }

        public override ActionExpression VisitActionMatrixUnit1DAssign(ArcticWindParser.ActionMatrixUnit1DAssignContext context)
        {
            string Lib = (context.matrix_name().lib_name() == null ? this._Factory.Context.PrimaryContextName : context.matrix_name().lib_name().GetText());
            string Name = ScriptingHelper.GetVarName(context.matrix_name());
            ScalarExpression row = this._Factory.BaseScalarFactory.Render(context.scalar_expression()[0]);
            ScalarExpression col = ScalarExpression.ZeroINT;
            ScalarExpression val = this._Factory.BaseScalarFactory.Render(context.scalar_expression()[1]);
            Assignment asg = ScriptingHelper.GetAssignment(context.assignment());
            return new ActionExpressionMatrixUnitAssign(this._Host, this._Master, Lib, Name, row, col, val, asg);
        }

        public override ActionExpression VisitActionMatrixUnit1DIncrement(ArcticWindParser.ActionMatrixUnit1DIncrementContext context)
        {

            string Lib = (context.matrix_name().lib_name() == null ? this._Factory.Context.PrimaryContextName : context.matrix_name().lib_name().GetText());
            string Name = ScriptingHelper.GetVarName(context.matrix_name());
            ScalarExpression row = this._Factory.BaseScalarFactory.Render(context.scalar_expression());
            ScalarExpression col = ScalarExpression.ZeroINT;
            ScalarExpression val = new ScalarExpressionConstant(null, CellValues.One(this._Factory.Context[Lib].GetMatrix(Name).Affinity));
            Assignment asg = (context.increment().PLUS() == null ? Assignment.MinusEquals : Assignment.PlusEquals);
            return new ActionExpressionMatrixUnitAssign(this._Host, this._Master, Lib, Name, row, col, val, asg);

        }

        public override ActionExpression VisitActionMatrixUnit2DAssign(ArcticWindParser.ActionMatrixUnit2DAssignContext context)
        {

            string Lib = (context.matrix_name().lib_name() == null ? this._Factory.Context.PrimaryContextName : context.matrix_name().lib_name().GetText());
            string Name = ScriptingHelper.GetVarName(context.matrix_name());
            ScalarExpression row = this._Factory.BaseScalarFactory.Render(context.scalar_expression()[0]);
            ScalarExpression col = this._Factory.BaseScalarFactory.Render(context.scalar_expression()[1]);
            ScalarExpression val = this._Factory.BaseScalarFactory.Render(context.scalar_expression()[2]);
            Assignment asg = ScriptingHelper.GetAssignment(context.assignment());
            return new ActionExpressionMatrixUnitAssign(this._Host, this._Master, Lib, Name, row, col, val, asg);

        }

        public override ActionExpression VisitActionMatrixUnit2DIncrement(ArcticWindParser.ActionMatrixUnit2DIncrementContext context)
        {

            string Lib = (context.matrix_name().lib_name() == null ? this._Factory.Context.PrimaryContextName : context.matrix_name().lib_name().GetText());
            string Name = ScriptingHelper.GetVarName(context.matrix_name());
            ScalarExpression row = this._Factory.BaseScalarFactory.Render(context.scalar_expression()[0]);
            ScalarExpression col = this._Factory.BaseScalarFactory.Render(context.scalar_expression()[1]);
            ScalarExpression val = new ScalarExpressionConstant(null, this._Factory.Context[Lib].GetMatrix(Name).Affinity);
            Assignment asg = (context.increment().PLUS() == null ? Assignment.MinusEquals : Assignment.PlusEquals);
            return new ActionExpressionMatrixUnitAssign(this._Host, this._Master, Lib, Name, row, col, val, asg);

        }

        public override ActionExpression VisitActionRecordAssign(ArcticWindParser.ActionRecordAssignContext context)
        {

            // Figure out what we're assigning //
            string Lib = (context.record_name().lib_name() == null ? this._Factory.Context.PrimaryContextName : context.record_name().lib_name().GetText());
            string Name = ScriptingHelper.GetVarName(context.record_name());
            
            // Get the expression //
            RecordExpression r = this._Factory.BaseRecordVisitor.Render(context.record_expression());
            return new ActionExpressionRecordAssign(this._Host, this._Master, Lib, Name, r);

        }

        public override ActionExpression VisitActionRecordUnitAssign(ArcticWindParser.ActionRecordUnitAssignContext context)
        {

            string sName = (context.record_name().lib_name() == null ? this._Factory.Context.PrimaryContextName : context.record_name().lib_name().GetText());
            string rName = ScriptingHelper.GetVarName(context.record_name());
            string vName = context.IDENTIFIER().GetText();
            Assignment logic = ScriptingHelper.GetAssignment(context.assignment());
            ScalarExpression value = this._Factory.BaseScalarFactory.Render(context.scalar_expression());

            ActionExpression a = new ActionExpressionRecordMemberAssign(this._Host, this._Master, sName, rName, vName, value, logic);
            this._Master = a;

            return a;

        }

        public override ActionExpression VisitActionRecordUnitIncrement(ArcticWindParser.ActionRecordUnitIncrementContext context)
        {
            string sName = (context.record_name().lib_name() == null ? this._Factory.Context.PrimaryContextName : context.record_name().lib_name().GetText());
            string rName = ScriptingHelper.GetVarName(context.record_name());
            string vName = context.IDENTIFIER().GetText();
            CellAffinity q = this._Factory.Context[sName].GetRecord(rName).ColumnAffinity(vName);
            ScalarExpression value = new ScalarExpressionConstant(null, CellValues.One(q));
            Assignment logic = Assignment.PlusEquals;
            if (context.increment().PLUS() == null)
                logic = Assignment.MinusEquals;

            ActionExpression a = new ActionExpressionRecordMemberAssign(this._Host, this._Master, sName, rName, vName, value, logic);
            this._Master = a;

            return a;
        }

        public override ActionExpression VisitActionPrintScalar(ArcticWindParser.ActionPrintScalarContext context)
        {
            ScalarExpression element = this._Factory.BaseScalarFactory.Render(context.scalar_expression()[0]);
            ScalarExpression path = (context.scalar_expression().Length == 2) ? this._Factory.BaseScalarFactory.Render(context.scalar_expression()[1]) : null;
            
            if (path == null)
                return new ActionExpressionPrintConsole(this._Host, this._Master, element);
            else
                return new ActionExpressionPrintFile(this._Host, this._Master, element, this._OpenTextStreams, path);
        }

        public override ActionExpression VisitActionPrintMatrix(ArcticWindParser.ActionPrintMatrixContext context)
        {
            MatrixExpression element = this._Factory.BaseMatrixFactory.Render(context.matrix_expression());
            ScalarExpression path = (context.scalar_expression() != null) ? this._Factory.BaseScalarFactory.Render(context.scalar_expression()) : null;

            if (path == null)
                return new ActionExpressionPrintConsole(this._Host, this._Master, element);
            else
                return new ActionExpressionPrintFile(this._Host, this._Master, element, this._OpenTextStreams, path);
        }

        public override ActionExpression VisitActionPrintRecord(ArcticWindParser.ActionPrintRecordContext context)
        {
            RecordExpression element = this._Factory.BaseRecordVisitor.Render(context.record_expression());
            ScalarExpression path = (context.scalar_expression() != null) ? this._Factory.BaseScalarFactory.Render(context.scalar_expression()) : null;

            if (path == null)
                return new ActionExpressionPrintConsole(this._Host, this._Master, element);
            else
                return new ActionExpressionPrintFile(this._Host, this._Master, element, this._OpenTextStreams, path);
        }

        public override ActionExpression VisitActionPrintTable(ArcticWindParser.ActionPrintTableContext context)
        {

            TableExpression element = this._Factory.BaseTableVisitor.Render(context.table_expression());
            ScalarExpression path = (context.scalar_expression() != null) ? this._Factory.BaseScalarFactory.Render(context.scalar_expression()) : null;
            if (path == null)
                return new ActionExpressionPrintConsole(this._Host, this._Master, element);
            else
                return new ActionExpressionPrintFile(this._Host, this._Master, element, this._OpenTextStreams, path);

        }

        public override ActionExpression VisitActionTableInsertRecord(ArcticWindParser.ActionTableInsertRecordContext context)
        {

            Table t = this._Factory.BaseTableVisitor.GetTable(context.table_name());
            ActionExpressionInsert x = new ActionExpressionInsert(this._Host, this._Master, t.OpenWriter(), this._Factory.BaseRecordVisitor.Render(context.record_expression()));
            return x;

        }

        public override ActionExpression VisitActionTableInsertTable(ArcticWindParser.ActionTableInsertTableContext context)
        {

            Table t = this._Factory.BaseTableVisitor.GetTable(context.table_name());
            TableExpression te = this._Factory.BaseTableVisitor.Render(context.table_expression());
            using (RecordWriter w = t.OpenWriter())
            {
                return new ActionExpressionInsertSelect(this._Host, this._Master, w, te);
            }
        }

        public override ActionExpression VisitActionSet(ArcticWindParser.ActionSetContext context)
        {
            ActionExpressionDo a = new ActionExpressionDo(this._Host, this._Master);

            foreach (ArcticWindParser.Action_expressionContext ctx in context.action_expression())
            {
                a.AddChild(this.Visit(ctx));
            }

            this._Master = a;

            return a;
        }

        public override ActionExpression VisitActionDo(ArcticWindParser.ActionDoContext context)
        {

            ActionExpressionDo a = new ActionExpressionDo(this._Host, this._Master);

            foreach (ArcticWindParser.Action_expressionContext ctx in context.action_expression())
            {
                a.AddChild(this.Visit(ctx));
            }

            this._Master = a;

            return a;

        }

        public override ActionExpression VisitActionFor(ArcticWindParser.ActionForContext context)
        {
            
            // Get the control var name //
            string Lib = (context.scalar_name().lib_name() == null ? this._Factory.Context.PrimaryContextName : context.scalar_name().lib_name().GetText());
            string Name = ScriptingHelper.GetVarName(context.scalar_name());
            
            // If the library doesnt exist, then error out
            if (!this._Factory.Context.SpoolExists(Lib))
                throw new Exception(string.Format("Library '{0}' does not exist", Lib));

            // If the store exists, but the varible doesn't, add the variable //
            ScalarExpression start = this._Factory.BaseScalarFactory.Render(context.scalar_expression()[0]);
            if (!this._Factory.Context[Lib].ScalarExists(Name))
            {
                CellAffinity q = (context.type() == null) ? (start.ReturnAffinity()) : (ScriptingHelper.GetTypeAffinity(context.type()));
                this._Factory.Context[Lib].AddScalar(Name, q, CellSerializer.DefaultLength(q));
            }

            // Parse out the control expressions //
            ScalarExpression control = this._Factory.BaseScalarFactory.Render(context.scalar_expression()[1]);
            ActionExpression increment = this.Visit(context.action_expression()[0]);

            ActionExpressionFor a = new ActionExpressionFor(this._Host, this._Master, Lib, Name, start, control, increment);

            if (context.action_expression().Length == 1)
                throw new Exception("For loops must contain at least one statement");

            for (int i = 1; i < context.action_expression().Length; i++)
            {
                ActionExpression ae = this.Visit(context.action_expression()[i]);
                a.AddChild(ae);
            }

            this._Master = a;

            return a;

        }

        public override ActionExpression VisitActionWhile(ArcticWindParser.ActionWhileContext context)
        {

            ScalarExpression predicate = this._Factory.BaseScalarFactory.Render(context.scalar_expression());
            ActionExpressionWhile act = new ActionExpressionWhile(this._Host, this._Master, predicate);

            // Load the children //
            foreach (ArcticWindParser.Action_expressionContext ctx in context.action_expression())
            {
                ActionExpression ae = this.Visit(ctx);
                act.AddChild(ae);
            }

            this._Master = act;

            return act;

        }

        public override ActionExpression VisitActionIF(ArcticWindParser.ActionIFContext context)
        {

            ScalarExpressionSet sec = new ScalarExpressionSet(this._Host);
            foreach (ArcticWindParser.Scalar_expressionContext ctx in context.scalar_expression())
            {
                ScalarExpression se = this._Factory.BaseScalarFactory.Render(ctx);
                if (se == null) throw new Exception("Expecting a scalar expression");
                sec.Add("X" + sec.Count.ToString(), se);
            }

            ActionExpression aei = new ActionExpressionIf(this._Host, this._Master, sec);

            foreach (ArcticWindParser.Action_expressionContext ctx in context.action_expression())
            {
                ActionExpression ae = this.Visit(ctx);
                aei.AddChild(ae);
            }

            this._Master = aei;

            return aei;

        }

        public override ActionExpression VisitActionCallSeq(ArcticWindParser.ActionCallSeqContext context)
        {
            
            // Need to figure out the library //
            string MethodName = ScriptingHelper.GetVarName(context.scalar_name());
            string LibName = null;
            if (context.scalar_name().lib_name() == null)
            {
                if (this._Host.BaseLibrary.ActionExists(MethodName))
                    LibName = Host.GLOBAL;
                else if (this._Host.UserLibrary.ActionExists(MethodName))
                    LibName = Host.USER;
                else
                    throw new Exception(string.Format("Method '{0}' does not exist", MethodName));
            }
            else
            {
                LibName = context.scalar_name().IDENTIFIER().GetText();
            }

            // Pull the action //
            if (!this._Host.Libraries.Exists(LibName))
                throw new Exception(string.Format("Library '{0}' does not exist", LibName));
            ActionExpressionParameterized x = this._Host.Libraries[LibName].ActionLookup(MethodName);

            foreach (Parameter p in this._Factory.Render(context.param()))
            {
                x.AddParameter(p);
            }

            this._Master = x;
            return x;

        }

        public override ActionExpression VisitActionInline(ArcticWindParser.ActionInlineContext context)
        {

            ScalarExpression script = this._Factory.BaseScalarFactory.Visit(context.scalar_expression()[0]);
            ScalarExpressionSet parameters = new ScalarExpressionSet(Host);
            ScalarExpressionSet values = new ScalarExpressionSet(Host);

            for (int i = 1; i < context.scalar_expression().Length; i += 2)
            {
                ScalarExpression p = this._Factory.BaseScalarFactory.Visit(context.scalar_expression()[i]);
                ScalarExpression v = this._Factory.BaseScalarFactory.Visit(context.scalar_expression()[i + 1]);
                parameters.Add("P" + i.ToString(), p);
                values.Add("V" + i.ToString(), v);
            }

            return new ActionExpressionInline(this._Host, this._Master, script, parameters, values, this._Host.Engine ?? new Scripting.ScriptProcessor(this._Host));

        }

        // Defines //
        public override ActionExpression VisitActionDefineVoid(ArcticWindParser.ActionDefineVoidContext context)
        {

            // Check if we're already compiling a function //
            if (this._Factory.IsCompilingMethod)
                throw new Exception("Cannot compile a function within a function");
            this._Factory.IsCompilingMethod = true;

            // Get the name and library //
            string LibName = (context.scalar_name().lib_name() == null ? Host.USER : context.scalar_name().lib_name().GetText());
            string MethodName = context.scalar_name().IDENTIFIER().GetText();

            // Add 'LOCAL' to the context
            this._Factory.Context.AddSpool(AbstractMethod.LOCAL);
            string OldPrimary = this._Factory.Context.PrimaryContextName;
            this._Factory.Context.PrimaryContextName = AbstractMethod.LOCAL;

            // Create the expression //
            ActionExpressionDeclareVoidMethod m = new ActionExpressionDeclareVoidMethod(this._Host, this._Master, LibName, MethodName);

            // Add the parameters //
            foreach (ArcticWindParser.Param_defContext p in context.param_def())
            {

                ParameterAffinity x = ScriptingHelper.GetParameterAffinity(p);
                string y = ScriptingHelper.GetParameterName(p);
                m.AddParameter(y, x);
                if (x == ParameterAffinity.Scalar)
                {
                    CellAffinity a = ScriptingHelper.GetTypeAffinity(p.type());
                    int b = ScriptingHelper.GetTypeSize(p.type());
                    this._Factory.Context[AbstractMethod.LOCAL].AddScalar(y, a, b);
                }
                else if (x == ParameterAffinity.Matrix)
                {
                    CellAffinity a = ScriptingHelper.GetTypeAffinity(p.type());
                    int b = ScriptingHelper.GetTypeSize(p.type());
                    this._Factory.Context[AbstractMethod.LOCAL].AddMatrix(y, a, b);
                }

            }

            // Add the body //
            foreach (ArcticWindParser.Action_expressionContext a in context.action_expression())
            {
                ActionExpression z = this.Render(a);
                m.AddStatement(z);
            }

            // Add 'LOCAL' to the context
            this._Factory.Context.DropSpool(AbstractMethod.LOCAL);
            this._Factory.Context.PrimaryContextName = OldPrimary;
            this._Factory.IsCompilingMethod = false;


            return m;

        }

        public override ActionExpression VisitActionDefineScalar(ArcticWindParser.ActionDefineScalarContext context)
        {

            // Check if we're already compiling a function //
            if (this._Factory.IsCompilingMethod)
                throw new Exception("Cannot compile a function within a function");
            this._Factory.IsCompilingMethod = true;

            // Get the name and library //
            string LibName = (context.scalar_name().lib_name() == null ? Host.USER : context.scalar_name().lib_name().GetText());
            string MethodName = context.scalar_name().IDENTIFIER().GetText();

            // Add 'LOCAL' to the context
            this._Factory.Context.AddSpool(AbstractMethod.LOCAL);
            string OldPrimary = this._Factory.Context.PrimaryContextName;
            this._Factory.Context.PrimaryContextName = AbstractMethod.LOCAL;

            // Get the return affinity and type //
            CellAffinity affinity = ScriptingHelper.GetTypeAffinity(context.type());
            int Size = ScriptingHelper.GetTypeSize(context.type());

            // Create the expression //
            ActionExpressionDeclareScalarMethod m = new ActionExpressionDeclareScalarMethod(this._Host, this._Master, LibName, MethodName, affinity, Size);

            // Add the parameters //
            foreach (ArcticWindParser.Param_defContext p in context.param_def())
            {

                ParameterAffinity x = ScriptingHelper.GetParameterAffinity(p);
                string y = ScriptingHelper.GetParameterName(p);
                m.AddParameter(y, x);
                if (x == ParameterAffinity.Scalar)
                {
                    CellAffinity a = ScriptingHelper.GetTypeAffinity(p.type());
                    int b = ScriptingHelper.GetTypeSize(p.type());
                    this._Factory.Context[AbstractMethod.LOCAL].AddScalar(y, a, b);
                }
                else if (x == ParameterAffinity.Matrix)
                {
                    CellAffinity a = ScriptingHelper.GetTypeAffinity(p.type());
                    int b = ScriptingHelper.GetTypeSize(p.type());
                    this._Factory.Context[AbstractMethod.LOCAL].AddMatrix(y, a, b);
                }

            }

            // Add the body //
            foreach (ArcticWindParser.Action_expressionContext a in context.action_expression())
            {
                ActionExpression z = this.Render(a);
                m.AddStatement(z);
            }

            // Add 'LOCAL' to the context
            this._Factory.Context.DropSpool(AbstractMethod.LOCAL);
            this._Factory.Context.PrimaryContextName = OldPrimary;
            this._Factory.IsCompilingMethod = false;


            return m;

        }

        // For Each //
        public override ActionExpression VisitActionForEachRecord(ArcticWindParser.ActionForEachRecordContext context)
        {


            // Get the source table //
            TableExpression t = this._Factory.BaseTableVisitor.Render(context.table_expression());
            string alias = context.record_name().IDENTIFIER().GetText();

            // Add the table to the map //
            this._Factory.Context.PrimaryContext.AddRecord(alias, t.Columns);

            // Now we can actually render the query! //
            ActionExpressionForEachTable aeq = new ActionExpressionForEachTable(this._Host, this._Master, t, Host.GLOBAL, alias);
            foreach (ArcticWindParser.Action_expressionContext x in context.action_expression())
            {
                ActionExpression ae = this.Visit(x);
                aeq.AddChild(ae);
            }

            this._Master = aeq;

            // Step out of the context //
            this._Factory.Context.PrimaryContext.Drop(alias);

            return aeq;

        }

        public override ActionExpression VisitActionForEachMatrix(ArcticWindParser.ActionForEachMatrixContext context)
        {

            string mLib = ScriptingHelper.GetLibName(context.matrix_name());
            string mName = ScriptingHelper.GetVarName(context.matrix_name());
            string sLib = ScriptingHelper.GetLibName(context.scalar_name());
            string sName = ScriptingHelper.GetVarName(context.scalar_name());

            this._Factory.Context[sLib].AddScalar(sName, this._Factory.Context[mLib].GetMatrix(mName).Affinity, this._Factory.Context[mLib].GetMatrix(mName).Size);

            ActionExpression y = new ActionExpressionForEachMatrix(this._Host, this._Master, mLib, mName, sLib, sName);
            foreach (ArcticWindParser.Action_expressionContext ctx in context.action_expression())
            {
                ActionExpression x = this.Visit(ctx);
                y.AddChild(x);
            }

            this._Factory.Context[sLib].Drop(sName);

            return y;

        }

        public override ActionExpression VisitActionForEachMatrixExpression(ArcticWindParser.ActionForEachMatrixExpressionContext context)
        {

            string sLib = ScriptingHelper.GetLibName(context.scalar_name());
            string sName = ScriptingHelper.GetVarName(context.scalar_name());
            MatrixExpression m = this._Factory.BaseMatrixFactory.Render(context.matrix_expression());

            this._Factory.Context[sLib].AddScalar(sName, m.ReturnAffinity(), m.ReturnSize());
            
            ActionExpression y = new ActionExpressionForEachMatrixExpression(this._Host, this._Master, m, sLib, sName);
            foreach (ArcticWindParser.Action_expressionContext ctx in context.action_expression())
            {
                ActionExpression x = this.Visit(ctx);
                y.AddChild(x);
            }

            return y;

        }

        // Returns //
        public override ActionExpression VisitActionReturnVoid(ArcticWindParser.ActionReturnVoidContext context)
        {
            ActionExpressionReturnVoid aerv = new ActionExpressionReturnVoid(this._Host, this._Master);
            return aerv;
        }

        public override ActionExpression VisitActionReturnScalar(ArcticWindParser.ActionReturnScalarContext context)
        {

            // If not compiling a function, throw an exception //
            if (!this._Factory.IsCompilingMethod)
                throw new Exception("Returning a scalar is invalid outside of a method");

            // Get a scalar //
            ScalarExpression se = this._Factory.BaseScalarFactory.Render(context.scalar_expression());

            // Create a return action //
            ActionExpressionReturnScalar aers = new ActionExpressionReturnScalar(this._Host, this._Master, this._Factory.Context.PrimaryContextName, TypedAbstractMethod.RETURN_NAME, se);
            return aers;

        }

        public override ActionExpression VisitActionReturnMatrix(ArcticWindParser.ActionReturnMatrixContext context)
        {

            // If not compiling a function, throw an exception //
            if (!this._Factory.IsCompilingMethod)
                throw new Exception("Returning a matrix is invalid outside of a method");

            // Get a scalar //
            MatrixExpression x = this._Factory.BaseMatrixFactory.Render(context.matrix_expression());

            // Create a return action //
            ActionExpressionReturnMatrix aers = new ActionExpressionReturnMatrix(this._Host, this._Master, this._Factory.Context.PrimaryContextName, TypedAbstractMethod.RETURN_NAME, x);
            return aers;

        }

        public override ActionExpression VisitActionReturnRecord(ArcticWindParser.ActionReturnRecordContext context)
        {

            // If not compiling a function, throw an exception //
            if (!this._Factory.IsCompilingMethod)
                throw new Exception("Returning a record is invalid outside of a method");

            // Get a scalar //
            RecordExpression x = this._Factory.BaseRecordVisitor.Render(context.record_expression());

            // Create a return action //
            ActionExpressionReturnRecord aers = new ActionExpressionReturnRecord(this._Host, this._Master, this._Factory.Context.PrimaryContextName, ColumnAbstractMethod.RETURN_NAME, x);
            return aers;

        }

        public override ActionExpression VisitActionReturnTable(ArcticWindParser.ActionReturnTableContext context)
        {

            // If not compiling a function, throw an exception //
            if (!this._Factory.IsCompilingMethod)
                throw new Exception("Returning a record is invalid outside of a method");

            // Get a scalar //
            TableExpression x = this._Factory.BaseTableVisitor.Render(context.table_expression());

            // Create a return action //
            ActionExpressionReturnTable aers = new ActionExpressionReturnTable(this._Host, this._Master, this._Factory.Context.PrimaryContextName, ColumnAbstractMethod.RETURN_NAME, x);
            return aers;

        }

        // Render //
        public ActionExpression Render(ArcticWindParser.Action_expressionContext context)
        {
            this._Master = null;
            return this.Visit(context);
        }

        // Method calls //
        //public override ActionExpression VisitActionCallNamed(ArcticWindParser.ActionCallNamedContext context)
        //{

        //    string lib = ScriptingHelper.GetLibName(context.var_name());
        //    string name = ScriptingHelper.GetVarName(context.var_name());

        //    ActionExpressionParameterized x = this._Host.Libraries[lib].ActionLookup(name);

        //    ScalarExpressionVisitor s = this._ScalarBuilder;
        //    this._ScalarBuilder = this._ScalarBuilder.CloneOfMe();

        //    string[] names = new string[] { };
        //    ActionExpressionParameterized.Parameter[] p = new ActionExpressionParameterized.Parameter[] { };

        //    this.Render(context.parameter_name(), out p, out names);

        //    for (int i = 0; i < names.Length; i++)
        //    {
        //        x.AddParameter(names[i], p[i]);
        //    }
        //    this._ScalarBuilder = s;

        //    return x;

        //}

        //public override ActionExpression VisitActionCallSeq(ArcticWindParser.ActionCallSeqContext context)
        //{

        //    string lib = ScriptingHelper.GetLibName(context.var_name());
        //    string name = ScriptingHelper.GetVarName(context.var_name());

        //    if (!this._Host.Libraries.Exists(lib))
        //        throw new Exception(string.Format("Library '{0}' does not exist", lib));
        //    if (!this._Host.Libraries[lib].ActionExists(name))
        //        throw new Exception(string.Format("Action '{0}' does not exist in '{1}'", name, lib));

        //    ActionExpressionParameterized x = this._Host.Libraries[lib].ActionLookup(name);

        //    ScalarExpressionVisitor s = this._ScalarBuilder;
        //    this._ScalarBuilder = this._ScalarBuilder.CloneOfMe();

        //    foreach (ActionExpressionParameterized.Parameter p in this.Render(context.parameter()))
        //    {
        //        x.AddParameter(p);
        //    }
        //    this._ScalarBuilder = s;

        //    return x;

        //}

        //private bool IsTabular(ArcticWindParser.ParameterContext context)
        //{
        //    if (context.var_name() != null)
        //    {
        //        string lib = ScriptingHelper.GetLibName(context.var_name());
        //        string name = ScriptingHelper.GetVarName(context.var_name());
        //        return this._Host.TableExists(lib, name);
        //    }
        //    else if (context.table_expression() != null)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        //private ActionExpressionParameterized.Parameter Render(ArcticWindParser.ParameterContext context)
        //{

        //    // Var Name //
        //    if (context.var_name() != null)
        //    {

        //        string lib = ScriptingHelper.GetLibName(context.var_name());
        //        string name = ScriptingHelper.GetVarName(context.var_name());

        //        if (this._Host.TableExists(lib, name))
        //        {

        //            TableExpression t = new TableExpressionValue(this._Host, null, this._Host.OpenTable(lib, name));
        //            ActionExpressionParameterized.Parameter p = new ActionExpressionParameterized.Parameter(t);
        //            int idx = this._ScalarBuilder.ColumnCube.Count;
        //            this._ScalarBuilder = this._ScalarBuilder.CloneOfMe();
        //            this._ScalarBuilder.AddSchema(t.Alias, t.Columns);
        //            this._TableBuilder.SeedVisitor = this._ScalarBuilder;
        //            this._MatrixBuilder.SeedVisitor = this._ScalarBuilder;
        //            p.HeapRef = idx;
        //            return p;

        //        }
        //        else if (this._Host.ScalarExists(lib, name))
        //        {

        //            int href = this._Host.Libraries.GetPointer(lib);
        //            int sref = this._Host.Libraries[href].Values.GetPointer(name);
        //            CellAffinity stype = this._Host.Libraries[href].Values[sref].Affinity;
        //            int ssize = CellSerializer.Length(this._Host.Libraries[href].Values[sref]);
        //            ScalarExpressionStoreRef x = new ScalarExpressionStoreRef(null, href, sref, stype, ssize);
        //            ActionExpressionParameterized.Parameter p = new ActionExpressionParameterized.Parameter(x);
        //            return p;
        //        }
        //        else if (this._ScalarBuilder.ColumnCube.Exists(lib))
        //        {

        //            int href = this._ScalarBuilder.ColumnCube.GetPointer(lib);
        //            int sref = this._ScalarBuilder.ColumnCube[lib].ColumnIndex(name);
        //            if (sref == -1) throw new Exception(string.Format("Field '{0}' does not exist", name));
        //            CellAffinity stype = this._ScalarBuilder.ColumnCube[lib].ColumnAffinity(sref);
        //            int ssize = this._ScalarBuilder.ColumnCube[lib].ColumnSize(sref);
        //            ScalarExpressionFieldRef x = new ScalarExpressionFieldRef(null, href, sref, stype, ssize);
        //            ActionExpressionParameterized.Parameter p = new ActionExpressionParameterized.Parameter(x);
        //            return p;

        //        }

        //        throw new Exception(string.Format("Table or value '{0}.{1}' does not exist", lib, name));

        //    }
        //    // Table expression //
        //    else if (context.table_expression() != null)
        //    {

        //        TableExpression t = this._TableBuilder.Visit(context.table_expression());
        //        ActionExpressionParameterized.Parameter p = new ActionExpressionParameterized.Parameter(t);
        //        int idx = this._ScalarBuilder.ColumnCube.Count;
        //        this._ScalarBuilder = this._ScalarBuilder.CloneOfMe();
        //        this._ScalarBuilder.AddSchema(t.Alias, t.Columns);
        //        this._TableBuilder.SeedVisitor = this._ScalarBuilder;
        //        this._MatrixBuilder.SeedVisitor = this._ScalarBuilder;
        //        p.HeapRef = idx;
        //        return p;

        //    }
        //    // Matrix expression //
        //    else if (context.matrix_expression() != null)
        //    {

        //        MatrixExpression m = this._MatrixBuilder.Visit(context.matrix_expression());
        //        ActionExpressionParameterized.Parameter p = new ActionExpressionParameterized.Parameter(m);
        //        return p;

        //    }
        //    // Record //
        //    else if (context.record_expression() != null)
        //    {

        //        ScalarExpressionSet r = RecordExpressionVisitor.Render(this._ScalarBuilder, context.record_expression());
        //        ActionExpressionParameterized.Parameter p = new ActionExpressionParameterized.Parameter(r);
        //        return p;

        //    }
        //    else if (context.scalar_expression() != null)
        //    {
        //        ScalarExpression x = this._ScalarBuilder.Render(context.scalar_expression());
        //        ActionExpressionParameterized.Parameter p = new ActionExpressionParameterized.Parameter(x);
        //        return p;
        //    }

        //    throw new Exception(string.Format("Parameter '{0}' is invalid", context.GetText()));
            
        //}

        //private ActionExpressionParameterized.Parameter[] Render(ArcticWindParser.ParameterContext[] context)
        //{

        //    if (context.Length == 0)
        //        return new ActionExpressionParameterized.Parameter[] { };

        //    ActionExpressionParameterized.Parameter[] s = new ActionExpressionParameterized.Parameter[context.Length];
        //    Queue<ArcticWindParser.ParameterContext> p = new Queue<ArcticWindParser.ParameterContext>();
        //    Queue<int> q = new Queue<int>();
        //    int i = 0;

        //    foreach (ArcticWindParser.ParameterContext ctx in context)
        //    {

        //        if (IsTabular(ctx))
        //        {
        //            s[i] = this.Render(ctx);
        //        }
        //        else
        //        {
        //            p.Enqueue(ctx);
        //            q.Enqueue(i);
        //        }
        //        i++;

        //    }

        //    while (p.Count != 0)
        //    {
        //        s[q.Dequeue()] = this.Render(p.Dequeue());
        //    }

        //    return s;

        //}

        //private void Render(ArcticWindParser.Parameter_nameContext[] context, out ActionExpressionParameterized.Parameter[] Parameters, out string[] Names)
        //{

        //    Names = context.Select((x) => { return x.lib_name().GetText(); }).ToArray();

        //    ArcticWindParser.ParameterContext[] b = context.Select((x) => { return x.parameter(); }).ToArray();

        //    Parameters = this.Render(b);

        //}

    }

}
