using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.ActionExpressions;
using ArcticWind.Expressions.TableExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Expressions.Aggregates;
using ArcticWind.Libraries;
using ArcticWind.Tables;
using ArcticWind.Expressions;

namespace ArcticWind.Scripting
{

    //public sealed class ExpressionVisitor : PulseParserBaseVisitor<IExpression>
    //{

    //    private Host _Host;
    //    private IExpression _Master;
    //    private FieldResolver _Map;
    //    private string _DefaultStoreContext = null;
    //    private string _DefaultRecordContext = null;

    //    // CTOR //
    //    public ExpressionVisitor(Host Host, FieldResolver Mapping)
    //    {
    //        this._Host = Host;
    //        this._Map = Mapping;
    //        this._Master = null;
    //    }

    //    public ExpressionVisitor(Host Host)
    //        : this(Host, new FieldResolver(Host))
    //    {
    //    }

    //    // Properties //
    //    public Host Host
    //    {
    //        get { return this._Host; }
    //    }

    //    public IExpression Master
    //    {
    //        get { return this._Master; }
    //    }

    //    public FieldResolver Map
    //    {
    //        get { return this._Map; }
    //    }

    //    public ObjectStore DefaultStore
    //    {
    //        get { return this._DefaultStoreContext == null ? null : this._Map[this._DefaultStoreContext]; }
    //    }

    //    // Literals //
    //    public override IExpression VisitExpressionScalarLiteral(PulseParser.ExpressionScalarLiteralContext context)
    //    {

    //        Cell c = CellValues.NullINT;
    //        if (context.sliteral().LITERAL_BOOL() != null)
    //        {
    //            c = CellParser.Parse(context.sliteral().LITERAL_BOOL().GetText(), CellAffinity.BOOL);
    //        }
    //        else if (context.sliteral().LITERAL_DATE() != null)
    //        {
    //            c = CellParser.Parse(context.sliteral().LITERAL_DATE().GetText(), CellAffinity.DATE_TIME);
    //        }
    //        else if (context.sliteral().LITERAL_BYTE() != null)
    //        {
    //            c = CellParser.Parse(context.sliteral().LITERAL_BYTE().GetText(), CellAffinity.BYTE);
    //        }
    //        else if (context.sliteral().LITERAL_SHORT() != null)
    //        {
    //            c = CellParser.Parse(context.sliteral().LITERAL_SHORT().GetText(), CellAffinity.SHORT);
    //        }
    //        else if (context.sliteral().LITERAL_INT() != null)
    //        {
    //            c = CellParser.Parse(context.sliteral().LITERAL_INT().GetText(), CellAffinity.INT);
    //        }
    //        else if (context.sliteral().LITERAL_LONG() != null)
    //        {
    //            c = CellParser.Parse(context.sliteral().LITERAL_LONG().GetText(), CellAffinity.LONG);
    //        }
    //        else if (context.sliteral().LITERAL_FLOAT() != null)
    //        {
    //            c = CellParser.Parse(context.sliteral().LITERAL_FLOAT().GetText(), CellAffinity.SINGLE);
    //        }
    //        else if (context.sliteral().LITERAL_DOUBLE() != null)
    //        {
    //            c = CellParser.Parse(context.sliteral().LITERAL_DOUBLE().GetText(), CellAffinity.DOUBLE);
    //        }
    //        else if (context.sliteral().LITERAL_BLOB() != null)
    //        {
    //            c = CellParser.Parse(context.sliteral().LITERAL_BLOB().GetText(), CellAffinity.BINARY);
    //        }
    //        else if (context.sliteral().LITERAL_TEXT() != null)
    //        {
    //            c = CellParser.Parse(context.sliteral().LITERAL_TEXT().GetText(), CellAffinity.BSTRING);
    //        }
    //        else if (context.sliteral().LITERAL_STRING() != null)
    //        {
    //            c = CellParser.Parse(context.sliteral().LITERAL_STRING().GetText(), CellAffinity.CSTRING);
    //        }
            
    //        return new ScalarExpressionConstant(null, c);

    //    }

    //    public override IExpression VisitExpressionRecordLiteral(PulseParser.ExpressionRecordLiteralContext context)
    //    {

    //        RecordExpressionLiteral rel = new RecordExpressionLiteral(this._Host, null);

    //        foreach (PulseParser.NelementContext x in context.nframe().nelement())
    //        {

    //            string alias = (x.IDENTIFIER() == null ? "F" + rel.Columns.Count.ToString() : x.IDENTIFIER().GetText());
    //            IExpression ec = this.Visit(x.expr());
    //            if (ec.SuperAffinity != SuperExpressionAffinity.Scalar)
    //                throw new Exception("Expression must be a scalar");
    //            rel.Add(ec.Scalar, alias);

    //        }

    //        return rel;

    //    }

    //    public override IExpression VisitExpressionMatrixLiteral(PulseParser.ExpressionMatrixLiteralContext context)
    //    {

    //        List<ScalarExpressionSet> Values = new List<ScalarExpressionSet>();

    //        foreach (PulseParser.NframeContext ctx in context.nframe())
    //        {
    //            Values.Add(this.RenderNFrame(ctx));
    //        }

    //        return new MatrixExpressionLiteral(null, Values);

    //    }

    //    // CTOR //
    //    public override IExpression VisitExpressionCTORMatrix(PulseParser.ExpressionCTORMatrixContext context)
    //    {
            
    //        ScalarExpression rows = this.Visit(context.expr()[0]).Scalar;
    //        ScalarExpression cols = (context.expr().Length > 1 ? this.Visit(context.expr()[1]).Scalar : ScalarExpression.OneINT);
    //        CellAffinity t = ScriptingHelper.GetTypeAffinity(context.type());
    //        int size = ScriptingHelper.GetTypeSize(context.type());

    //        return new MatrixExpressionCTOR(this._Master.Matrix, rows, cols, t, size);

    //    }

    //    public override IExpression VisitExpressionCTORRecord(PulseParser.ExpressionCTORRecordContext context)
    //    {

    //        Schema cols = new Schema();
    //        for (int i = 0; i < context.IDENTIFIER().Length; i++)
    //        {
    //            cols.Add(context.IDENTIFIER()[i].GetText(), ScriptingHelper.GetTypeAffinity(context.type()[i]), ScriptingHelper.GetTypeSize(context.type()[i]));
    //        }
    //        return new RecordExpressionCTOR(this._Host, this._Master.Record, cols);

    //    }

    //    public override IExpression VisitExpressionCTORTable(PulseParser.ExpressionCTORTableContext context)
    //    {

    //        Schema cols = new Schema();
    //        for (int i = 0; i < context.IDENTIFIER().Length; i++)
    //        {
    //            cols.Add(context.IDENTIFIER()[i].GetText(), ScriptingHelper.GetTypeAffinity(context.type()[i]), ScriptingHelper.GetTypeSize(context.type()[i]));
    //        }
    //        string db = ScriptingHelper.GetLibName(context.var_name());
    //        string name = ScriptingHelper.GetVarName(context.var_name());

    //        return new TableExpressionCTOR(this._Host, null, cols, db, name, new Key());

    //    }

    //    // Arithmetic and Logic //
    //    public override IExpression VisitExpressionUniary(PulseParser.ExpressionUniaryContext context)
    //    {

    //        // Get the current element //
    //        IExpression ie = this.Visit(context.expr());
            
    //        // Scalar //
    //        if (ie.SuperAffinity == SuperExpressionAffinity.Scalar)
    //        {
    //            ScalarExpression se = null;
    //            if (context.NOT() != null)
    //                se = new ScalarExpressionFunction.ExpressionNot();
    //            else if (context.PLUS() != null)
    //                se = new ScalarExpressionFunction.ExpressionPlus();
    //            else if (context.MINUS() != null)
    //                se = new ScalarExpressionFunction.ExpressionMinus();
    //            se.AddChildNode(ie.Scalar);
    //            return se;
    //        }

    //        throw new ArgumentException("Unary operations not supported");

    //    }

    //    public override IExpression VisitExpressionPower(PulseParser.ExpressionPowerContext context)
    //    {

    //        // Get the current element //
    //        IExpression a = this.Visit(context.expr()[0]);
    //        IExpression b = this.Visit(context.expr()[1]);

    //        // Scalar //
    //        if (a.SuperAffinity == SuperExpressionAffinity.Scalar && b.SuperAffinity == SuperExpressionAffinity.Scalar)
    //        {
    //            ScalarExpression x = new ScalarExpressionFunction.ExpressionPower();
    //            x.AddChildren(a.Scalar, b.Scalar);
    //            return x;
    //        }

    //        throw new ArgumentException("Unary operations not supported");

    //    }

    //    public override IExpression VisitExpressionMultDivMod(PulseParser.ExpressionMultDivModContext context)
    //    {

    //        // Get the current element //
    //        IExpression a = this.Visit(context.expr()[0]);
    //        IExpression b = this.Visit(context.expr()[1]);

    //        // Scalar //
    //        if (a.SuperAffinity == SuperExpressionAffinity.Scalar && b.SuperAffinity == SuperExpressionAffinity.Scalar)
    //        {
                
    //            ScalarExpression x = null;
    //            if (context.MUL() != null)
    //                x = new ScalarExpressionFunction.ExpressionMultiply();
    //            else if (context.DIV() != null)
    //                x = new ScalarExpressionFunction.ExpressionDivide();
    //            else if (context.DIV2() != null)
    //                x = new ScalarExpressionFunction.ExpressionCheckedDivide();
    //            else if (context.MOD() != null)
    //                x = new ScalarExpressionFunction.ExpressionModulo();

    //            x.AddChildren(a.Scalar, b.Scalar);
    //            return x;

    //        }

    //        // Matrix - Matrix //
    //        if (a.SuperAffinity == SuperExpressionAffinity.Matrix && b.SuperAffinity == SuperExpressionAffinity.Matrix)
    //        {

    //            MatrixExpression x = null;
    //            if (context.MUL() != null)
    //                x = new MatrixExpressionMultiply(null);
    //            else if (context.DIV() != null)
    //                x = new MatrixExpressionDivide(null);
    //            else if (context.DIV2() != null)
    //                x = new MatrixExpressionCheckDivide(null);

    //            x.AddChildren(a.Matrix, b.Matrix);
    //            return x;

    //        }

    //        // Matrix - Scalar //
    //        if (a.SuperAffinity == SuperExpressionAffinity.Matrix && b.SuperAffinity == SuperExpressionAffinity.Scalar)
    //        {

    //            MatrixExpression x = null;
    //            if (context.MUL() != null)
    //                x = new MatrixExpressionMultiplyScalar(null, b.Scalar, 1);
    //            else if (context.DIV() != null)
    //                x = new MatrixExpressionDivideScalar(null, b.Scalar, 1);
    //            else if (context.DIV2() != null)
    //                x = new MatrixExpressionCheckDivideScalar(null, b.Scalar, 1);

    //            x.AddChildren(a.Matrix);
    //            return x;

    //        }

    //        // Scalar - Matrix //
    //        if (a.SuperAffinity == SuperExpressionAffinity.Scalar && b.SuperAffinity == SuperExpressionAffinity.Matrix)
    //        {

    //            MatrixExpression x = null;
    //            if (context.MUL() != null)
    //                x = new MatrixExpressionMultiplyScalar(null, a.Scalar, 0);
    //            else if (context.DIV() != null)
    //                x = new MatrixExpressionDivideScalar(null, a.Scalar, 0);
    //            else if (context.DIV2() != null)
    //                x = new MatrixExpressionCheckDivideScalar(null, a.Scalar, 0);

    //            x.AddChildren(b.Matrix);
    //            return x;

    //        }

    //        throw new ArgumentException("Operation not supported");

    //    }

    //    public override IExpression VisitExpressionAddSub(PulseParser.ExpressionAddSubContext context)
    //    {

    //        // Get the current element //
    //        IExpression a = this.Visit(context.expr()[0]);
    //        IExpression b = this.Visit(context.expr()[1]);

    //        // Scalar //
    //        if (a.SuperAffinity == SuperExpressionAffinity.Scalar && b.SuperAffinity == SuperExpressionAffinity.Scalar)
    //        {

    //            ScalarExpression x = null;
    //            if (context.PLUS() != null)
    //                x = new ScalarExpressionFunction.ExpressionAdd();
    //            else if (context.MINUS() != null)
    //                x = new ScalarExpressionFunction.ExpressionSubtract();

    //            x.AddChildren(a.Scalar, b.Scalar);
    //            return x;

    //        }

    //        // Matrix - Matrix //
    //        if (a.SuperAffinity == SuperExpressionAffinity.Matrix && b.SuperAffinity == SuperExpressionAffinity.Matrix)
    //        {

    //            MatrixExpression x = null;
    //            if (context.PLUS() != null)
    //                x = new MatrixExpressionAdd(null);
    //            else if (context.MINUS() != null)
    //                x = new MatrixExpressionSubtract(null);

    //            x.AddChildren(a.Matrix, b.Matrix);
    //            return x;

    //        }

    //        // Matrix - Scalar //
    //        if (a.SuperAffinity == SuperExpressionAffinity.Matrix && b.SuperAffinity == SuperExpressionAffinity.Scalar)
    //        {

    //            MatrixExpression x = null;
    //            if (context.PLUS() != null)
    //                x = new MatrixExpressionAddScalar(null, b.Scalar, 1);
    //            else if (context.MINUS() != null)
    //                x = new MatrixExpressionSubtractScalar(null, b.Scalar, 1);

    //            x.AddChildren(a.Matrix);
    //            return x;

    //        }

    //        // Scalar - Matrix //
    //        if (a.SuperAffinity == SuperExpressionAffinity.Scalar && b.SuperAffinity == SuperExpressionAffinity.Matrix)
    //        {

    //            MatrixExpression x = null;
    //            if (context.PLUS() != null)
    //                x = new MatrixExpressionAddScalar(null, a.Scalar, 0);
    //            else if (context.MINUS() != null)
    //                x = new MatrixExpressionSubtractScalar(null, a.Scalar, 0);

    //            x.AddChildren(b.Matrix);
    //            return x;

    //        }

    //        throw new ArgumentException("Operation not supported");

    //    }

    //    public override IExpression VisitExpressionGreaterLesser(PulseParser.ExpressionGreaterLesserContext context)
    //    {

    //        // Get the current element //
    //        IExpression a = this.Visit(context.expr()[0]);
    //        IExpression b = this.Visit(context.expr()[1]);

    //        // Scalar //
    //        if (a.SuperAffinity == SuperExpressionAffinity.Scalar && b.SuperAffinity == SuperExpressionAffinity.Scalar)
    //        {

    //            ScalarExpression x = null;
    //            if (context.GT() != null)
    //                x = new ScalarExpressionFunction.ExpressionGreaterThan();
    //            else if (context.GTE() != null)
    //                x = new ScalarExpressionFunction.ExpressionGreaterThanOrEqualTo();
    //            else if (context.LT() != null)
    //                x = new ScalarExpressionFunction.ExpressionLessThan();
    //            else if (context.LTE() != null)
    //                x = new ScalarExpressionFunction.ExpressionLessThanOrEqualTo();

    //            x.AddChildren(a.Scalar, b.Scalar);
    //            return x;

    //        }

    //        throw new ArgumentException("Operation not supported");

    //    }

    //    public override IExpression VisitExpressionEquality(PulseParser.ExpressionEqualityContext context)
    //    {

    //        // Get the current element //
    //        IExpression a = this.Visit(context.expr()[0]);
    //        IExpression b = this.Visit(context.expr()[1]);

    //        // Scalar //
    //        if (a.SuperAffinity == SuperExpressionAffinity.Scalar && b.SuperAffinity == SuperExpressionAffinity.Scalar)
    //        {

    //            ScalarExpression x = null;
    //            if (context.EQ() != null)
    //                x = new ScalarExpressionFunction.ExpressionEquals();
    //            else if (context.NEQ() != null)
    //                x = new ScalarExpressionFunction.ExpressionNotEquals();

    //            x.AddChildren(a.Scalar, b.Scalar);
    //            return x;

    //        }

    //        throw new ArgumentException("Operation not supported");

    //    }

    //    public override IExpression VisitExpressionLogicalAnd(PulseParser.ExpressionLogicalAndContext context)
    //    {
    //        return base.VisitExpressionLogicalAnd(context);
    //    }

    //    public override IExpression VisitExpressionLogicalOr(PulseParser.ExpressionLogicalOrContext context)
    //    {

    //        // Get the current element //
    //        IExpression a = this.Visit(context.expr()[0]);
    //        IExpression b = this.Visit(context.expr()[1]);

    //        // Scalar //
    //        if (a.SuperAffinity == SuperExpressionAffinity.Scalar && b.SuperAffinity == SuperExpressionAffinity.Scalar)
    //        {

    //            ScalarExpression x = null;
    //            if (context.OR() != null)
    //                x = new ScalarExpressionFunction.ExpressionOr();
    //            else if (context.XOR() != null)
    //                x = new ScalarExpressionFunction.ExpressionXor();

    //            x.AddChildren(a.Scalar, b.Scalar);
    //            return x;

    //        }

    //        throw new ArgumentException("Operation not supported");

    //    }

    //    // Scalar members //
    //    public override IExpression VisitExpressionMatrixMember(PulseParser.ExpressionMatrixMemberContext context)
    //    {
            
    //        IExpression val = this.Visit(context.expr()[0]);
    //        if (val.SuperAffinity != SuperExpressionAffinity.Matrix)
    //            throw new Exception("Expression must be a matrix");
    //        ScalarExpressionMatrixRef saex = new ScalarExpressionMatrixRef(null, val.Matrix);
    //        IExpression row = this.Visit(context.expr()[1]);
    //        IExpression col = (context.expr().Length >= 3 ? this.Visit(context.expr()[2]) : ScalarExpression.OneINT);
    //        if (row.SuperAffinity != SuperExpressionAffinity.Scalar || col.SuperAffinity != SuperExpressionAffinity.Scalar)
    //            throw new Exception("Operation is invalid");
    //        saex.AddChildren(row.Scalar, col.Scalar);
    //        return saex;

    //    }

    //    public override IExpression VisitExpressionName1(PulseParser.ExpressionName1Context context)
    //    {

    //        /*
    //         * Can be: 
    //         * -- Table: Name
    //         * -- Record: Name
    //         * -- Matrix: Name
    //         * -- Scalar: DecaultContext.DefaultRecord.Name
    //         * -- Scalar: Name
    //         * 
    //         */

    //        string name = context.IDENTIFIER().GetText();

    //        // Check if we're dealing with a variable in a table expression //
    //        if (this.DefaultStore != null && this._DefaultRecordContext != null && this.DefaultStore.ExistsRecord(this._DefaultRecordContext))
    //        {

    //            // Check if the name exists in the record //
    //            if (this.DefaultStore.Records[this._DefaultRecordContext].Columns.ColumnIndex(name) != -1)
    //            {
    //                Schema cols = this.DefaultStore.Records[this._DefaultRecordContext].Columns;
    //                return new ScalarExpressionRecordRef(null, this._DefaultStoreContext, this._DefaultRecordContext, name, cols.ColumnAffinity(name), cols.ColumnSize(name));
    //            }

    //        }

    //        // Figure out what we're working with //
    //        // Object is a strict named scalar Add.Name
    //        if (this._Map.Global.Exists(name, ObjectStore.ObjectAffinity.Scalar))
    //        {
    //            return new ScalarExpressionStoreRef(null, FieldResolver.GLOBAL, name, this._Map.Global.Scalars[name].Affinity, this._Map.Global.Scalars[name].Length);
    //        }
    //        // Object is a matrix in a heap
    //        else if (this._Map.Global.Exists(name, ObjectStore.ObjectAffinity.Matrix))
    //        {
    //            return new MatrixExpressionStoreRef(null, FieldResolver.GLOBAL, name, this._Map.Global.Matrixes[name].Affinity, this._Map.Global.Matrixes[name].Size);
    //        }
    //        // Object is a record in a heap
    //        else if (this._Map.Global.Exists(name, ObjectStore.ObjectAffinity.Record))
    //        {
    //            return new RecordExpressionStoreRef(this._Host, null, Elements.Host.GLOBAL, name, this._Host.Add.GetRecord(name).Columns);
    //        }
    //        // Object is a table
    //        else if (this._Map.Global.Exists(name, ObjectStore.ObjectAffinity.Table))
    //        {
    //            return new TableExpressionStoreRef(this._Host, null, name);
    //        }

    //        throw new Exception(string.Format("Cannot find field or variable '{0}'", name));

    //    }

    //    public override IExpression VisitExpressionName2(PulseParser.ExpressionName2Context context)
    //    {

    //        /*
    //         * Can be: 
    //         * -- Scalar: DecaultContext.Add.Name
    //         * -- Table: SpoolName.Name
    //         * -- Record: Add.Name
    //         * -- Matrix: Add.Name
    //         * -- Scalar: Add.Name
    //         * -- Scalar: LOCAL.Add.Name
    //         * -- Scalar: GLOBAL.Add.Name
    //         * 
    //         */

    //        string store = context.lib_name().GetText();
    //        string name = context.IDENTIFIER().GetText();

    //        // Check the default context first: looking for Alias.Name, which translates to Local.RecordName.FieldName //
    //        if (this.DefaultStore != null)
    //        {

    //            // Check if the store contains 'store' //
    //            if (!this.DefaultStore.ExistsRecord(store))
    //            {
    //                Schema cols = this.DefaultStore.Records[store].Columns;
    //                return new ScalarExpressionRecordRef(null, this._DefaultStoreContext, store, name, cols.ColumnAffinity(name), cols.ColumnSize(name));

    //            }

    //        }

    //        // Check for a strictly named object //
    //        if (this._Map.StoreExists(store))
    //        {

    //            ObjectStore os = this._Map[store];

    //            // Figure out what we're working with //
    //            // Object is a strict named scalar Add.Name
    //            if (os.Exists(name, ObjectStore.ObjectAffinity.Scalar))
    //            {
    //                return new ScalarExpressionStoreRef(null, store, name, os.Scalars[name].Affinity, os.Scalars[name].Length);
    //            }
    //            // Object is a matrix in a heap
    //            else if (os.Exists(name, ObjectStore.ObjectAffinity.Matrix))
    //            {
    //                return new MatrixExpressionStoreRef(null, store, name, os.Matrixes[name].Affinity, os.Matrixes[name].Size);
    //            }
    //            // Object is a record in a heap
    //            else if (os.Exists(name, ObjectStore.ObjectAffinity.Record))
    //            {
    //                return new RecordExpressionStoreRef(this._Host, null, store, name, os.GetRecord(name).Columns);
    //            }
    //            // Object is a table
    //            else if (os.Exists(name, ObjectStore.ObjectAffinity.Table))
    //            {
    //                if (StringComparer.OrdinalIgnoreCase.Compare(store, FieldResolver.GLOBAL) != 0)
    //                    throw new Exception("Tables can only exist in the global context");
    //                return new TableExpressionStoreRef(this._Host, null, name);
    //            }

    //        }

    //        // Check if we're actually looking at a record member //
    //        if (this._Map.Global.Exists(store, ObjectStore.ObjectAffinity.Record))
    //        {
    //            Schema s = this._Map.Global.GetRecord(store).Columns;
    //            return new ScalarExpressionRecordRef(null, FieldResolver.GLOBAL, store, name, s.ColumnAffinity(name), s.ColumnSize(name));
    //        }
    //        else if (this._Map.Local.Exists(store, ObjectStore.ObjectAffinity.Record))
    //        {
    //            Schema s = this._Map.Local.GetRecord(store).Columns;
    //            return new ScalarExpressionRecordRef(null, FieldResolver.GLOBAL, store, name, s.ColumnAffinity(name), s.ColumnSize(name));
    //        }

    //        // Check if we're looking at a connected table //
    //        if (this._Host.Connections.Exists(store))
    //        {

    //            // Buffer the table //
    //            Table t = this._Host.OpenTable(store, name);

    //            return new TableExpressionValue(this._Host, null, t);

    //        }

    //        throw new Exception(string.Format("Variable '{0}' does not exist in '{1}'", name, store));

    //    }

    //    public override IExpression VisitExpressionName3(PulseParser.ExpressionName3Context context)
    //    {

    //        /*
    //         * Can only be a sctrictly named record member: StoreName.RecordName.MemberName
    //         * 
    //         */

    //        string lname = context.lib_name().GetText();
    //        string rname = context.IDENTIFIER()[0].GetText();
    //        string fname = context.IDENTIFIER()[1].GetText();

    //        Schema s = this._Map[lname].GetRecord(rname).Columns;
    //        return new ScalarExpressionRecordRef(null, lname, rname, fname, s.ColumnAffinity(fname), s.ColumnSize(fname));

    //    }

    //    //public override IExpression VisitExpressionName(PulseParser.ExpressionNameContext context)
    //    //{

    //    //    string lib = context.var_name().lib_name() == null && this._DefaultStoreContext != null ? this._DefaultStoreContext : ScriptingHelper.GetLibName(context.var_name());
    //    //    string name = ScriptingHelper.GetVarName(context.var_name());

    //    //    // Check if we're looking at an object in a store //
    //    //    if (this._Map.StoreExists(lib))
    //    //    {

    //    //        ObjectStore os = this._Map[lib];

    //    //        // Figure out what we're working with //
    //    //        // Object is a strict named scalar Add.Name
    //    //        if (os.Exists(name, ObjectStore.ObjectAffinity.Scalar))
    //    //        {
    //    //            return new ScalarExpressionStoreRef(null, name, os);
    //    //        }
    //    //        // Object is a matrix in a heap
    //    //        else if (os.Exists(name, ObjectStore.ObjectAffinity.Matrix))
    //    //        {
    //    //            return new MatrixExpressionStoreRef(null, name, os);
    //    //        }
    //    //        // Object is a record in a heap
    //    //        else if (os.Exists(name, ObjectStore.ObjectAffinity.Record))
    //    //        {
    //    //            return new RecordExpressionStoreRef(this._Host, null, name, os);
    //    //        }
    //    //        // Object is a table
    //    //        else if (os.Exists(name, ObjectStore.ObjectAffinity.Table))
    //    //        {
    //    //            return new TableExpressionStoreRef(this._Host, null, name, os);
    //    //        }

    //    //    }

    //    //    // Check if the record exists in the default context; records are the most common use of context switching //
    //    //    if (this._DefaultStoreContext != null && this._DefaultRecordContext != null && this._Map[this._DefaultStoreContext].ExistsRecord(this._DefaultRecordContext))
    //    //    {

    //    //        // Check if the name exists in the record //
    //    //        if (this._Map[this._DefaultStoreContext].Records[this._DefaultRecordContext].Columns.ColumnIndex(name) != -1)
    //    //        {
    //    //            RecordExpression rex = new RecordExpressionStoreRef(this._Host, null, this._DefaultStoreContext, this._Map[this._DefaultStoreContext]);
    //    //            return new ScalarExpressionRecordRef2(null, rex, name);
    //    //        }
                
    //    //    }

    //    //    // Check if the store exists in the default context; records are the most common use of context switching //
    //    //    else if (this._DefaultStoreContext != null && this._Map[this._DefaultStoreContext].ExistsRecord(name))
    //    //    {

    //    //        // Check if the name exists in the record //
    //    //        if (this._Map[this._DefaultStoreContext].Records[this._DefaultRecordContext].Columns.ColumnIndex(name) != -1)
    //    //        {
    //    //            return new RecordExpressionStoreRef(this._Host, null, name, this._Map[this._DefaultStoreContext]);
    //    //        }

    //    //    }

    //    //    // Check if we're actually looking at a record member //
    //    //    if (this._Map.Global.Exists(lib, ObjectStore.ObjectAffinity.Record))
    //    //    {
    //    //        RecordExpression rex = new RecordExpressionStoreRef(this._Host, null, lib, this._Map.Global);
    //    //        return new ScalarExpressionRecordRef2(null, rex, name);
    //    //    }
    //    //    else if (this._Map.Local.Exists(lib, ObjectStore.ObjectAffinity.Record))
    //    //    {
    //    //        RecordExpression rex = new RecordExpressionStoreRef(this._Host, null, lib, this._Map.Local);
    //    //        return new ScalarExpressionRecordRef2(null, rex, name);
    //    //    }

    //    //    // Check if we're looking at a connected table //
    //    //    if (this._Host.Connections.Exists(lib))
    //    //    {

    //    //        // Buffer the table //
    //    //        Table t = this._Host.OpenTable(lib, name);

    //    //        return new TableExpressionValue(this._Host, null, t);

    //    //    }

    //    //    throw new Exception(string.Format("Variable '{0}' does not exist in '{1}'", name, lib));

    //    //}

    //    //public override IExpression VisitExpressionStrictRecord(PulseParser.ExpressionStrictRecordContext context)
    //    //{

    //    //    string lname = context.IDENTIFIER()[0].GetText();
    //    //    string rname = context.IDENTIFIER()[1].GetText();
    //    //    string fname = context.IDENTIFIER()[2].GetText();

    //    //    ObjectStore os = this._Map[lname];
    //    //    RecordExpressionStoreRef raex = new RecordExpressionStoreRef(this._Host, null, rname, os);
    //    //    return new ScalarExpressionRecordRef2(null, raex, fname);

    //    //}

    //    // Tables //
    //    public override IExpression VisitExpressionTableOpen(PulseParser.ExpressionTableOpenContext context)
    //    {
    //        return base.VisitExpressionTableOpen(context);
    //    }

    //    public override IExpression VisitExpressionSelect(PulseParser.ExpressionSelectContext context)
    //    {

    //        // Get the base expression //
    //        IExpression ie = this.Visit(context.expr());
    //        if (ie.Table == null)
    //            throw new Exception("Expression must be a table");
    //        TableExpression From = ie.Table;
    //        From.Alias = From.Alias ?? From.Name;

    //        // Switch out the resolver //
    //        this._Map.Local.DeclareRecord(From.Alias, new AssociativeRecord(From.Columns));

    //        // Change the auto context to LOCAL and the table's name //
    //        this._DefaultStoreContext = FieldResolver.LOCAL;
    //        this._DefaultRecordContext = From.Alias;

    //        // Visit each of the fields //
    //        ScalarExpressionSet select = this.RenderNFrame(context.nframe());

    //        // Filter //
    //        Filter where = this.RenderWhere(context.where());

    //        // Create the expression //
    //        TableExpression x = new TableExpressionSelect(this._Host, null, select, where, From.Alias);
    //        x.AddChild(From);

    //        // Modifiers //
    //        Key order = this.RenderKey(context.oframe());
    //        if (order != null)
    //            x.OrderBy = order;

    //        // Remove Map //
    //        this._Map.Local.RemoveRecord(From.Alias);

    //        // Get the output database and name //
    //        x.SpoolName = this.RenderDB(context.var_name());
    //        x.Name = this.RenderName(context.var_name());

    //        // Step out of context //
    //        this._DefaultStoreContext = null;
    //        this._DefaultRecordContext = null;

    //        // Set the master //
    //        this._Master = x;

    //        return x;

    //    }

    //    public override IExpression VisitExpressionJoin(PulseParser.ExpressionJoinContext context)
    //    {

    //        // Render the expressions //
    //        IExpression LeftIE = this.Visit(context.expr()[0]);
    //        if (LeftIE.SuperAffinity != SuperExpressionAffinity.Table)
    //            throw new Exception("Expression must be a table");
    //        IExpression RightIE = this.Visit(context.expr()[1]);
    //        if (RightIE.SuperAffinity != SuperExpressionAffinity.Table)
    //            throw new Exception("Expression must be a table");

    //        // Get the tables //
    //        TableExpression t1 = LeftIE.Table;
    //        TableExpression t2 = RightIE.Table;

    //        // Get the aliases //
    //        string a1 = context.IDENTIFIER()[0].GetText();
    //        string a2 = context.IDENTIFIER()[1].GetText();

    //        // Build and load the keys //
    //        Tuple<Key, Key> joins = this.RenderJoinPredicate(context.jframe(), t1.Columns, a1, t2.Columns, a2);
    //        // Get the affinity //
    //        TableExpressionJoin.JoinType t = TableExpressionJoin.JoinType.INNER;
    //        if (context.NOT() != null)
    //            t = TableExpressionJoin.JoinType.ANTI_LEFT;
    //        else if (context.PIPE() != null)
    //            t = TableExpressionJoin.JoinType.LEFT;
            
    //        // Get the join engine //
    //        //TableExpressionJoin.JoinAlgorithm ja = TableExpressionJoin.Optimize(t1.EstimatedCount, t2.EstimatedCount, t1.IsIndexedBy(k1), t2.IsIndexedBy(k2));

    //        // Add these tables to the map //
    //        this._Map.Local.DeclareRecord(a1, new AssociativeRecord(t1.Columns));
    //        this._Map.Local.DeclareRecord(a2, new AssociativeRecord(t2.Columns));

    //        // Parse out the fields //
    //        ScalarExpressionSet select = this.RenderNFrame(context.nframe());

    //        // Get the filter //
    //        Filter f = this.RenderWhere(context.where());

    //        // Create the expression //
    //        TableExpressionJoin tej = new TableExpressionJoin.TableExpressionJoinSortMerge(this._Host, null, select, new RecordMatcher(joins.Item1, joins.Item2), f, t);
            
    //        // Handle the clustering, the distinct, and the ordering //
    //        Key Order = this.RenderKey(context.oframe());
    //        if (Order != null)
    //            tej.OrderBy = Order;

    //        // Add the children //
    //        tej.AddChild(t1);
    //        tej.AddChild(t2);

    //        // Remove Map //
    //        this._Map.Local.RemoveRecord(a1);
    //        this._Map.Local.RemoveRecord(a2);

    //        // Get the output database and name //
    //        tej.SpoolName = this.RenderDB(context.var_name());
    //        tej.Name = this.RenderName(context.var_name());

    //        // Point the master here //
    //        this._Master = tej;

    //        return tej;

    //    }

    //    public override IExpression VisitExpressionGroup1(PulseParser.ExpressionGroup1Context context)
    //    {

    //        // Get the source //
    //        IExpression ie = this.Visit(context.expr());
    //        if (ie.SuperAffinity != SuperExpressionAffinity.Table)
    //            throw new Exception("Expression must be a table");
    //        TableExpression From = ie.Table;
    //        From.Alias = From.Alias ?? From.Name;

    //        // Append the map //
    //        this._Map.Local.DeclareRecord(From.Alias, new AssociativeRecord(From.Columns));

    //        // Change the auto context to LOCAL and the table's name //
    //        this._DefaultStoreContext = FieldResolver.LOCAL;
    //        this._DefaultRecordContext = From.Alias;

    //        // Get the select //
    //        ScalarExpressionSet group = this.RenderNFrame(context.nframe());
    //        Filter w = this.RenderWhere(context.where());

    //        // Get the aggregate // 
    //        AggregateCollection agg = this.RenderAFrame(context.aframe());

    //        // Potentially get the final select //
    //        string salias = TableExpressionFold.SECOND_ALIAS_PREFIX + From.Alias;
    //        Schema cols = Schema.Join(group.Columns, agg.Columns);
    //        this._Map.Local.DeclareRecord(salias, new AssociativeRecord(cols));
    //        this._DefaultRecordContext = salias;
    //        ScalarExpressionSet select = new ScalarExpressionSet(cols, salias);
    //        //if (context.nframe().Length == 2)
    //        //    select = this.RenderNFrame(context.nframe()[1]);
            
    //        // Create the expression //
    //        TableExpressionFold x = new TableExpressionFold.TableExpressionFoldDictionary(this._Host, null, group, agg, w, select, From.Alias);
    //        x.AddChild(From);

    //        // Remove Map //
    //        this._Map.Local.RemoveRecord(From.Alias);
    //        this._Map.Local.RemoveRecord(salias);

    //        // Get the output database and name //
    //        x.SpoolName = this.RenderDB(context.var_name());
    //        x.Name = this.RenderName(context.var_name());

    //        // Step out of context //
    //        this._DefaultStoreContext = null;
    //        this._DefaultRecordContext = null;

    //        this._Master = x;

    //        return x;

    //    }

    //    public override IExpression VisitExpressionGroup2(PulseParser.ExpressionGroup2Context context)
    //    {

    //        // Get the source //
    //        IExpression ie = this.Visit(context.expr());
    //        if (ie.SuperAffinity != SuperExpressionAffinity.Table)
    //            throw new Exception("Expression must be a table");
    //        TableExpression From = ie.Table;
    //        From.Alias = From.Alias ?? From.Name;

    //        // Append the map //
    //        this._Map.Local.DeclareRecord(From.Alias, new AssociativeRecord(From.Columns));

    //        // Change the auto context to LOCAL and the table's name //
    //        this._DefaultStoreContext = FieldResolver.LOCAL;
    //        this._DefaultRecordContext = From.Alias;

    //        // Get the select //
    //        ScalarExpressionSet group = this.RenderNFrame(context.nframe());
    //        Filter w = this.RenderWhere(context.where());

    //        // Potentially get the final select //
    //        string salias = TableExpressionFold.SECOND_ALIAS_PREFIX + From.Alias;
    //        this._Map.Local.DeclareRecord(salias, new AssociativeRecord(group.Columns));
    //        ScalarExpressionSet select = new ScalarExpressionSet(group.Columns, salias);
    //        //if (context.nframe().Length == 2)
    //        //    select = this.RenderNFrame(context.nframe()[1]);

    //        // Create the expression //
    //        TableExpressionFold x = new TableExpressionFold.TableExpressionFoldDictionary(this._Host, null, group, new AggregateCollection(), w, select, From.Alias);
    //        x.AddChild(From);

    //        // Remove Map //
    //        this._Map.Local.RemoveRecord(From.Alias);
    //        this._Map.Local.RemoveRecord(salias);

    //        // Get the output database and name //
    //        x.SpoolName = this.RenderDB(context.var_name());
    //        x.Name = this.RenderName(context.var_name());

    //        // Step out of context //
    //        this._DefaultStoreContext = null;
    //        this._DefaultRecordContext = null;

    //        this._Master = x;

    //        return x;

    //    }
        
    //    public override IExpression VisitExpressionGroup3(PulseParser.ExpressionGroup3Context context)
    //    {

    //        // Get the source //
    //        IExpression ie = this.Visit(context.expr());
    //        if (ie.SuperAffinity != SuperExpressionAffinity.Table)
    //            throw new Exception("Expression must be a table");
    //        TableExpression From = ie.Table;
    //        From.Alias = From.Alias ?? From.Name;

    //        // Append the map //
    //        this._Map.Local.DeclareRecord(From.Alias, new AssociativeRecord(From.Columns));

    //        // Change the auto context to LOCAL and the table's name //
    //        this._DefaultStoreContext = FieldResolver.LOCAL;
    //        this._DefaultRecordContext = From.Alias;

    //        // Get the select //
    //        Filter w = this.RenderWhere(context.where());

    //        // Get the aggregate // 
    //        AggregateCollection agg = this.RenderAFrame(context.aframe());

    //        // Potentially get the final select //
    //        string salias = TableExpressionFold.SECOND_ALIAS_PREFIX + From.Alias;
    //        this._Map.Local.DeclareRecord(salias, new AssociativeRecord(agg.Columns));
    //        ScalarExpressionSet select = new ScalarExpressionSet(agg.Columns, salias);
    //        //if (context.nframe().Length == 2)
    //        //    select = this.RenderNFrame(context.nframe()[1]);

    //        // Create the expression //
    //        TableExpressionFold x = new TableExpressionFold.TableExpressionFoldDictionary(this._Host, null, new ScalarExpressionSet(), agg, w, select, From.Alias);
    //        x.AddChild(From);

    //        // Remove Map //
    //        this._Map.Local.RemoveRecord(From.Alias);
    //        this._Map.Local.RemoveRecord(salias);

    //        // Get the output database and name //
    //        x.SpoolName = this.RenderDB(context.var_name());
    //        x.Name = this.RenderName(context.var_name());

    //        // Step out of context //
    //        this._DefaultStoreContext = null;
    //        this._DefaultRecordContext = null;

    //        this._Master = x;

    //        return x;

    //    }

    //    // Records //
    //    public override IExpression VisitExpressionFrame(PulseParser.ExpressionFrameContext context)
    //    {
            
    //        ScalarExpressionSet ses = this.RenderNFrame(context.nframe());
    //        RecordExpressionLiteral rel = new RecordExpressionLiteral(this._Host, null);
    //        for (int i = 0; i < ses.Count; i++)
    //        {
    //            rel.Add(ses[i], ses.Alias(i));
    //        }
    //        return rel;

    //    }

    //    // Functional //
    //    public override IExpression VisitExpressionFunction(PulseParser.ExpressionFunctionContext context)
    //    {

    //        string LibName = ScriptingHelper.GetLibName(context.var_name());
    //        string VarName = ScriptingHelper.GetVarName(context.var_name());

    //        if (!this._Host.Libraries.Exists(LibName))
    //            throw new Exception(string.Format("Library '{0}' does not exist", LibName));

    //        Library l = this._Host.Libraries[LibName];

    //        if (!l.ScalarFunctionExists(VarName))
    //            throw new Exception(string.Format("Function '{0}' does not exist in '{1}'", VarName, LibName));

    //        ScalarExpressionFunction f = l.ScalarFunctionLookup(VarName);
    //        foreach (PulseParser.ExprContext ctx in context.expr())
    //        {
    //            ScalarExpression x = this.Visit(ctx).Scalar;
    //            f.AddChildNode(x);
    //        }

    //        return f;

    //    }
        
    //    public override IExpression VisitExpressionIf(PulseParser.ExpressionIfContext context)
    //    {

    //        ScalarExpression predicate = this.Visit(context.expr()[0]).Scalar;
    //        ScalarExpression true_val = this.Visit(context.expr()[1]).Scalar;
    //        ScalarExpression false_val = this.Visit(context.expr()[2]).Scalar;

    //        if (predicate == null || true_val == null || false_val == null)
    //            throw new Exception();

    //        ScalarExpressionFunction sef = new ScalarExpressionFunction.ExpressionIf();
    //        sef.AddChildren(predicate, true_val, false_val);

    //        return sef;

    //    }

    //    public override IExpression VisitExpressionCast(PulseParser.ExpressionCastContext context)
    //    {

    //        ScalarExpression s = this.Visit(context.expr()).Scalar;
    //        CellAffinity a = ScriptingHelper.GetTypeAffinity(context.type());
    //        ScalarExpressionFunction sef = new ScalarExpressionFunction.ExpressionCast(a);
    //        sef.AddChildren(s);

    //        return sef;

    //    }

    //    // Parenthesis //
    //    public override IExpression VisitExpressionParens(PulseParser.ExpressionParensContext context)
    //    {
    //        return this.Visit(context.expr());
    //    }

    //    // Final //
    //    public IExpression Render(PulseParser.ExprContext context)
    //    {
    //        this._Master = null;
    //        return this.Visit(context);
    //    }

    //    // Helpers //
    //    private ScalarExpressionSet RenderNFrame(PulseParser.NframeContext context)
    //    {

    //        ScalarExpressionSet ses = new ScalarExpressionSet();

    //        if (context == null)
    //            return ses;
            
    //        foreach (PulseParser.NelementContext x in context.nelement())
    //        {

    //            string alias = (x.IDENTIFIER() == null ? "F" + ses.Count.ToString() : x.IDENTIFIER().GetText());
    //            IExpression ie = this.Visit(x.expr());
    //            if (ie.SuperAffinity != SuperExpressionAffinity.Scalar)
    //                throw new Exception("Expression must be a scalar");
    //            if (ie.Scalar is ScalarExpressionRecordRef)
    //                alias = (ie.Scalar as ScalarExpressionRecordRef).FieldName;
    //            ses.Add(alias, ie.Scalar);

    //        }
    //        return ses;

    //    }

    //    private AggregateCollection RenderAFrame(PulseParser.AframeContext context)
    //    {

    //        AggregateCollection select = new AggregateCollection();

    //        foreach (PulseParser.AggContext ctx in context.agg())
    //        {
    //            Aggregate a = this.RenderAggregate(ctx);
    //            string alias = (ctx.IDENTIFIER() == null ? "AGG" + select.Count.ToString() : ctx.IDENTIFIER().GetText());
    //            select.Add(alias, a);
    //        }

    //        return select;

    //    }

    //    private Aggregate RenderAggregate(PulseParser.AggContext context)
    //    {

    //        ScalarExpressionSet frame = this.RenderNFrame(context.nframe());
    //        Filter f = this.RenderWhere(context.where());

    //        Aggregate a = (new AggregateLookup()).Lookup(context.SET_REDUCTIONS().GetText(), frame, f);

    //        return a;

    //    }

    //    private Filter RenderWhere(PulseParser.WhereContext context)
    //    {

    //        if (context == null)
    //            return Filter.TrueForAll;

    //        IExpression ie = this.Visit(context);
    //        if (ie.SuperAffinity != SuperExpressionAffinity.Scalar)
    //            throw new Exception("Expression must be a scalar");

    //        ScalarExpression scex = ie.Scalar;
    //        if (scex.ReturnAffinity() != CellAffinity.BOOL)
    //            throw new Exception("Expression must be a boolean");

    //        return new Filter(scex);

    //    }

    //    private Key RenderKey(PulseParser.OframeContext context)
    //    {

    //        if (context == null)
    //            return null;

    //        Key k = new Key();

    //        foreach (PulseParser.OrderContext ctx in context.order())
    //        {

    //            int index = int.Parse(ctx.LITERAL_INT().GetText());
    //            KeyAffinity t = KeyAffinity.Ascending;
    //            if (ctx.K_DESC() != null) t = KeyAffinity.Descending;
    //            k.Add(index, t);

    //        }

    //        return k;

    //    }

    //    private Tuple<Key, Key> RenderJoinPredicate(PulseParser.JframeContext context, Schema LColumns, string LAlias, Schema RColumns, string RAlias)
    //    {

    //        Key left = new Key();
    //        Key right = new Key();

    //        foreach (PulseParser.JelementContext ctx in context.jelement())
    //        {
                
    //            string LeftAlias = ScriptingHelper.GetLibName(ctx.var_name()[0]);
    //            string RightAlias = ScriptingHelper.GetLibName(ctx.var_name()[1]);

    //            string LeftName = ScriptingHelper.GetVarName(ctx.var_name()[0]);
    //            string RightName = ScriptingHelper.GetVarName(ctx.var_name()[1]);

    //            if (LeftAlias == LAlias && RightAlias == RAlias)
    //            {
    //                // Do nothing
    //            }
    //            else if (RightAlias == LAlias && LeftAlias == RAlias)
    //            {
    //                string t = LeftAlias;
    //                LeftAlias = RightAlias;
    //                RightAlias = t;
    //            }
    //            else
    //            {
    //                throw new Exception(string.Format("One of '{0}' or '{1}' is invalid", LeftAlias, RightAlias));
    //            }

    //            int LeftIndex = LColumns.ColumnIndex(LeftName);
    //            if (LeftIndex == -1)
    //                throw new Exception(string.Format("Field '{0}' does not exist in '{1}'", LeftName, LeftAlias));
    //            int RightIndex = RColumns.ColumnIndex(RightName);
    //            if (RightIndex == -1)
    //                throw new Exception(string.Format("Field '{0}' does not exist in '{1}'", RightName, RightAlias));

    //            left.Add(LeftIndex);
    //            right.Add(RightIndex);

    //        }

    //        return new Tuple<Key, Key>(left, right);

    //    }

    //    private string RenderDB(PulseParser.Var_nameContext context)
    //    {
    //        if (context == null) return Elements.Host.TEMP;
    //        return ScriptingHelper.GetLibName(context);
    //    }

    //    private string RenderName(PulseParser.Var_nameContext context)
    //    {
    //        if (context == null) return Elements.Host.RandomName;
    //        return ScriptingHelper.GetVarName(context);
    //    }

    //}



}
