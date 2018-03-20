using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Expressions.TableExpressions;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Elements;
using ArcticWind.Expressions.Aggregates;
using ArcticWind.Tables;
using ArcticWind.Expressions;
using ArcticWind.Expressions.RecordExpressions;

namespace ArcticWind.Scripting
{

    /// <summary>
    /// The base class that renders expressions
    /// </summary>
    public class TableExpressionVisitor : ArcticWindParserBaseVisitor<TableExpression>
    {

        public const string ALIAS2_PREFIX = TableExpressionFold.SECOND_ALIAS_PREFIX;

        private Host _Host;
        private TableExpression _Master;
        private ObjectFactory _Factory;
        
        public TableExpressionVisitor(Host Host)
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

        // Other Support //
        public Table GetTable(ArcticWindParser.Table_nameContext context)
        {

            string SpoolName = ScriptingHelper.GetLibName(context);
            string TableName = ScriptingHelper.GetVarName(context);
            Table t = this._Host.Spools[SpoolName].GetTable2(TableName);
            return t;

        }

        // Expressions //
        public override TableExpression VisitTableExpressionJoin(ArcticWindParser.TableExpressionJoinContext context)
        {

            // Get the tables //
            TableExpression t1 = this.Visit(context.table_expression()[0]);
            TableExpression t2 = this.Visit(context.table_expression()[1]);

            // Get the aliases //
            string a1 = context.IDENTIFIER()[0].GetText();
            string a2 = context.IDENTIFIER()[1].GetText();

            // Build and load the keys //
            Tuple<Key, Key> x = this.RenderJoinPredicate(context.jframe(), t1.Columns, a1, t2.Columns, a2);
            RecordMatcher predicate = new RecordMatcher(x.Item1, x.Item2);

            // Get the affinity //
            TableExpressionJoin.JoinType t = TableExpressionJoin.JoinType.INNER;
            if (context.NOT() != null) t = TableExpressionJoin.JoinType.ANTI_LEFT;
            else if (context.PIPE() != null) t = TableExpressionJoin.JoinType.LEFT;

            // Get the join engine //
            TableExpressionJoin.JoinAlgorithm ja = TableExpressionJoin.Optimize(t1.EstimatedCount, t2.EstimatedCount, t1.IsIndexedBy(x.Item1), t2.IsIndexedBy(x.Item2));

            // Set up the spool space //
            this._Factory.Context.AddSpool("JOIN");
            string LastContext = this._Factory.Context.PrimaryContextName;
            this._Factory.Context.PrimaryContextName = "JOIN";
            this._Factory.Context.PrimaryContext.AddRecord(a1, t1.Columns);
            this._Factory.Context.PrimaryContext.AddRecord(a2, t2.Columns);

            // Get the expressions //
            ScalarExpressionSet sec = this._Factory.BaseScalarFactory.Render(context.nframe());
            Filter f = this._Factory.BaseScalarFactory.Render(context.where());

            // Create the expression //
            TableExpressionJoin tej = new TableExpressionJoin.TableExpressionJoinSortMerge(this._Host, this._Master, sec, predicate, f, t, "JOIN", a1, a2);
            
            // Add the children //
            tej.AddChild(t1);
            tej.AddChild(t2);

            // Remove Map //
            this._Factory.Context.DropSpool("JOIN");
            this._Factory.Context.PrimaryContextName = LastContext;

            // Point the master here //
            this._Master = tej;

            return tej;

        }

        public override TableExpression VisitTableExpressionFold1(ArcticWindParser.TableExpressionFold1Context context)
        {

            TableExpression t = this.Visit(context.table_expression());
            string alias = t.Name;
            string salias = ALIAS2_PREFIX + alias;

            // Set up the spool space //
            this._Factory.Context.AddSpool("AGG");
            string LastContext = this._Factory.Context.PrimaryContextName;
            this._Factory.Context.PrimaryContextName = "AGG";
            this._Factory.Context.PrimaryContext.AddRecord(alias, t.Columns);

            ScalarExpressionSet grouper = this._Factory.BaseScalarFactory.Render(context.nframe());
            AggregateCollection aggs = this._Factory.BaseScalarFactory.Render(context.aframe());
            Filter f = this._Factory.BaseScalarFactory.Render(context.where());

            this._Factory.Context.PrimaryContext.AddRecord(salias, Schema.Join(grouper.Columns, aggs.Columns));
            ScalarExpressionSet select = new ScalarExpressionSet(this._Host, Schema.Join(grouper.Columns, aggs.Columns), "AGG", salias);

            TableExpressionFold x = new TableExpressionFold.TableExpressionFoldDictionary(this._Host, this._Master, grouper, aggs, f, select, "AGG", alias);
            x.AddChild(t);

            // Revert back 
            this._Factory.Context.DropSpool("AGG");
            this._Factory.Context.PrimaryContextName = LastContext;
            
            this._Master = x;

            return x;

        }

        public override TableExpression VisitTableExpressionFold2(ArcticWindParser.TableExpressionFold2Context context)
        {

            TableExpression t = this.Visit(context.table_expression());
            string alias = t.Name;
            string salias = ALIAS2_PREFIX + alias;

            // Set up the spool space //
            this._Factory.Context.AddSpool("AGG");
            string LastContext = this._Factory.Context.PrimaryContextName;
            this._Factory.Context.PrimaryContextName = "AGG";
            this._Factory.Context.PrimaryContext.AddRecord(alias, t.Columns);

            ScalarExpressionSet grouper = this._Factory.BaseScalarFactory.Render(context.nframe());
            AggregateCollection aggs = new AggregateCollection();
            Filter f = this._Factory.BaseScalarFactory.Render(context.where());

            this._Factory.Context.PrimaryContext.AddRecord(salias, Schema.Join(grouper.Columns, aggs.Columns));
            ScalarExpressionSet select = new ScalarExpressionSet(this._Host, Schema.Join(grouper.Columns, aggs.Columns), "AGG", salias);

            TableExpressionFold x = new TableExpressionFold.TableExpressionFoldDictionary(this._Host, this._Master, grouper, aggs, f, select, "AGG", alias);
            x.AddChild(t);

            // Revert back 
            this._Factory.Context.DropSpool("AGG");
            this._Factory.Context.PrimaryContextName = LastContext;
            
            this._Master = x;

            return x;

        }

        public override TableExpression VisitTableExpressionFold3(ArcticWindParser.TableExpressionFold3Context context)
        {

            TableExpression t = this.Visit(context.table_expression());
            string alias = t.Name;
            string salias = ALIAS2_PREFIX + alias;

            // Set up the spool space //
            this._Factory.Context.AddSpool("AGG");
            string LastContext = this._Factory.Context.PrimaryContextName;
            this._Factory.Context.PrimaryContextName = "AGG";
            this._Factory.Context.PrimaryContext.AddRecord(alias, t.Columns);

            ScalarExpressionSet grouper = new ScalarExpressionSet(this._Host);
            AggregateCollection aggs = this._Factory.BaseScalarFactory.Render(context.aframe());
            Filter f = this._Factory.BaseScalarFactory.Render(context.where());

            this._Factory.Context.PrimaryContext.AddRecord(salias, Schema.Join(grouper.Columns, aggs.Columns));
            ScalarExpressionSet select = new ScalarExpressionSet(this._Host, Schema.Join(grouper.Columns, aggs.Columns), "AGG", salias);

            TableExpressionFold x = new TableExpressionFold.TableExpressionFoldDictionary(this._Host, this._Master, grouper, aggs, f, select, "AGG", alias);
            x.AddChild(t);

            // Revert back 
            this._Factory.Context.DropSpool("AGG");
            this._Factory.Context.PrimaryContextName = LastContext;
            
            this._Master = x;

            return x;

        }

        public override TableExpression VisitTableExpressionUnion(ArcticWindParser.TableExpressionUnionContext context)
        {

            // Create a union expression //
            TableExpressionUnion teu = new TableExpressionUnion(this._Host, this._Master);

            // Load all the tables //
            foreach (ArcticWindParser.Table_expressionContext ctx in context.table_expression())
            {
                TableExpression t_first = this.Visit(ctx);
                teu.AddChild(t_first);
            }

            this._Master = teu;

            return teu;

        }

        public override TableExpression VisitTableExpressionSelect1(ArcticWindParser.TableExpressionSelect1Context context)
        {

            // Get the base expression //
            TableExpression t = this.Visit(context.table_expression());
            string alias = t.Name;

            // Set up the spool space //
            this._Factory.Context.AddSpool("SELECT");
            string LastContext = this._Factory.Context.PrimaryContextName;
            this._Factory.Context.PrimaryContextName = "SELECT";
            this._Factory.Context.PrimaryContext.AddRecord(alias, t.Columns);

            // Get the where and the fields //
            ScalarExpressionSet select = this._Factory.BaseScalarFactory.Render(context.nframe());
            Filter where = this._Factory.BaseScalarFactory.Render(context.where());

            // Create the expression //
            TableExpression x = new TableExpressionSelect(this._Host, this._Master, select, where, "SELECT", alias);
            x.AddChild(t);

            // Get the order by //
            if (context.oframe() != null)
            {
                Key k = this.RenderKey(context.oframe());
            }

            // Revert back 
            this._Factory.Context.DropSpool("SELECT");
            this._Factory.Context.PrimaryContextName = LastContext;
            
            // Set the master //
            this._Master = x;

            return x;

        }

        //public override TableExpression VisitTableExpressionSelect2(ArcticWindParser.TableExpressionSelect2Context context)
        //{

        //    // Get the base expression //
        //    TableExpression t = this.Visit(context.table_expression());
        //    string alias = t.Name;

        //    // Set up the spool space //
        //    this._Factory.Context.AddSpool("SELECT");
        //    string LastContext = this._Factory.Context.PrimaryContextName;
        //    this._Factory.Context.PrimaryContextName = "SELECT";
        //    this._Factory.Context.PrimaryContext.AddRecord(alias, t.Columns);

        //    // Get the where and the fields //
        //    ScalarExpressionSet select = new ScalarExpressionSet(this._Host, t.Columns, "SELECT", alias);
        //    Filter where = this._Factory.BaseScalarFactory.Render(context.where());

        //    // Create the expression //
        //    TableExpression x = new TableExpressionSelect(this._Host, this._Master, select, where, "SELECT", alias);
        //    x.AddChild(t);

        //    // Revert back 
        //    this._Factory.Context.DropSpool("SELECT");
        //    this._Factory.Context.PrimaryContextName = LastContext;
            
        //    // Set the master //
        //    this._Master = x;

        //    return x;

        //}

        //public override TableExpression VisitTableExpressionSelect3(ArcticWindParser.TableExpressionSelect3Context context)
        //{

        //    // Get the base expression //
        //    TableExpression t = this.Visit(context.table_expression());
        //    string alias = t.Name;

        //    // Set up the spool space //
        //    this._Factory.Context.AddSpool("SELECT");
        //    string LastContext = this._Factory.Context.PrimaryContextName;
        //    this._Factory.Context.PrimaryContextName = "SELECT";
        //    this._Factory.Context.PrimaryContext.AddRecord(alias, t.Columns);

        //    // Get the where and the fields //
        //    ScalarExpressionSet select = new ScalarExpressionSet(this._Host, t.Columns, "SELECT", alias);
        //    Filter where = Filter.TrueForAll;

        //    // Create the expression //
        //    TableExpression x = new TableExpressionSelect(this._Host, this._Master, select, where, "SELECT", alias);
        //    x.AddChild(t);

        //    // Revert back 
        //    this._Factory.Context.DropSpool("SELECT");
        //    this._Factory.Context.PrimaryContextName = LastContext;
            
        //    // Set the master //
        //    this._Master = x;

        //    return x;

        //}

        public override TableExpression VisitTableExpressionLookup(ArcticWindParser.TableExpressionLookupContext context)
        {
            string lib = ScriptingHelper.GetLibName(context.table_name());
            string name = ScriptingHelper.GetVarName(context.table_name());
            //Table t = this._Host.OpenTable(lib, name);
            TableExpression x = new TableExpressionStoreRef(this._Host, this._Master, lib, name);
            x.Alias = name;
            return x;
        }

        public override TableExpression VisitTableExpressionLiteral(ArcticWindParser.TableExpressionLiteralContext context)
        {

            List<ScalarExpressionSet> z = new List<ScalarExpressionSet>();
            foreach (ArcticWindParser.NframeContext ctx in context.nframe())
            {
                ScalarExpressionSet v = this._Factory.BaseScalarFactory.Render(ctx);
                z.Add(v);
            }
            return new TableExpressionLiteral(this._Host, this._Master, z, z.First().Columns);

        }

        //public override TableExpression VisitTableExpressionCTOR(ArcticWindParser.TableExpressionCTORContext context)
        //{

        //    Schema cols = new Schema();
        //    for (int i = 0; i < context.IDENTIFIER().Length; i++)
        //    {
        //        cols.Add(context.IDENTIFIER()[i].GetText(), ScriptingHelper.GetTypeAffinity(context.type()[i]), ScriptingHelper.GetTypeSize(context.type()[i]));
        //    }
        //    string db = context.db_name().IDENTIFIER()[0].GetText();
        //    string name = context.db_name().IDENTIFIER()[1].GetText();

        //    return new TableExpressionCTOR(this._Host, null, cols, db, name, new Key());

        //}

        public override TableExpression VisitTableExpressionFunction(ArcticWindParser.TableExpressionFunctionContext context)
        {

            string LibName = ScriptingHelper.GetLibName(context.table_name());
            string FuncName = ScriptingHelper.GetVarName(context.table_name());

            if (!this._Host.Libraries.Exists(LibName))
                throw new Exception(string.Format("Library does not exist '{0}'", LibName));

            if (!this._Host.Libraries[LibName].ScalarFunctionExists(FuncName))
                throw new Exception(string.Format("Function '{0}' does not exist in '{1}'", FuncName, LibName));

            TableExpressionFunction f = this._Host.Libraries[LibName].TableFunctionLookup(FuncName);
            foreach (ArcticWindParser.ParamContext ctx in context.param())
            {
                Parameter p = this._Factory.Render(ctx);
                f.AddParameter(p);
            }

            this._Master = f;

            return f;

        }

        public override TableExpression VisitTableExpressionParens(ArcticWindParser.TableExpressionParensContext context)
        {
            return this.Visit(context.table_expression());
        }

        // Main //
        public TableExpression Render(ArcticWindParser.Table_expressionContext context)
        {
            return this.Visit(context);
        }

       //  Support //
        private Key RenderKey(ArcticWindParser.OframeContext context)
        {

            if (context == null)
                return null;

            Key k = new Key();

            foreach (ArcticWindParser.OrderContext ctx in context.order())
            {

                int index = int.Parse(ctx.LITERAL_INT().GetText());
                KeyAffinity t = KeyAffinity.Ascending;
                if (ctx.K_DESC() != null) t = KeyAffinity.Descending;
                k.Add(index, t);

            }

            return k;

        }

        private Tuple<Key, Key> RenderJoinPredicate(ArcticWindParser.JframeContext context, Schema LColumns, string LAlias, Schema RColumns, string RAlias)
        {

            Key left = new Key();
            Key right = new Key();

            foreach (ArcticWindParser.JelementContext ctx in context.jelement())
            {

                string LeftAlias = ctx.IDENTIFIER()[0].GetText();
                string RightAlias = ctx.IDENTIFIER()[2].GetText();

                string LeftName = ctx.IDENTIFIER()[1].GetText();
                string RightName = ctx.IDENTIFIER()[3].GetText();

                if (LeftAlias == LAlias && RightAlias == RAlias)
                {
                    // Do nothing
                }
                else if (RightAlias == LAlias && LeftAlias == RAlias)
                {
                    string t = LeftAlias;
                    LeftAlias = RightAlias;
                    RightAlias = t;
                }
                else
                {
                    throw new Exception(string.Format("One of '{0}' or '{1}' is invalid", LeftAlias, RightAlias));
                }

                int LeftIndex = LColumns.ColumnIndex(LeftName);
                if (LeftIndex == -1)
                    throw new Exception(string.Format("Field '{0}' does not exist in '{1}'", LeftName, LeftAlias));
                int RightIndex = RColumns.ColumnIndex(RightName);
                if (RightIndex == -1)
                    throw new Exception(string.Format("Field '{0}' does not exist in '{1}'", RightName, RightAlias));

                left.Add(LeftIndex);
                right.Add(RightIndex);

            }

            return new Tuple<Key, Key>(left, right);

        }
    
    
    
    }

}
