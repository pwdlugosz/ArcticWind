using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Expressions.TableExpressions;
using ArcticWind.Tables;

namespace ArcticWind.Scripting
{

    /// <summary>
    /// 
    /// </summary>
    public sealed class ObjectFactory
    {

        private ScalarExpressionVisitor _sFactory;
        private MatrixExpressionVisitor _mFactory;
        private RecordExpressionVisitor _rFactory;
        private TableExpressionVisitor _tFactory;
        private SpoolSpaceContext _Context;
        private Host _Host;

        public ObjectFactory(Host Host)
        {

            this._Host = Host;

            this._Context = new SpoolSpaceContext(this._Host);
            this._sFactory = new ScalarExpressionVisitor(this._Host);
            this._mFactory = new MatrixExpressionVisitor(this._Host);
            this._rFactory = new RecordExpressionVisitor(this._Host);
            this._tFactory = new TableExpressionVisitor(this._Host);

            this._sFactory.Factory = this;
            this._mFactory.Factory = this;
            this._rFactory.Factory = this;
            this._tFactory.Factory = this;

            this._Context.PrimaryContextName = Host.GLOBAL;

            foreach (Spool s in this._Host.Spools.Spools)
            {
                this._Context.Import(s);
            }

            this.IsCompilingMethod = false;
            this.IsCompilingInline = false;
            this.IsCompilingLoop = false;

        }

        public bool IsCompilingMethod
        {
            get;
            set;
        }

        public bool IsCompilingInline
        {
            get;
            set;
        }

        public bool IsCompilingLoop
        {
            get;
            set;
        }

        public ScalarExpressionVisitor BaseScalarFactory
        {
            get { return this._sFactory; }
            set { this._sFactory = value; }
        }

        public MatrixExpressionVisitor BaseMatrixFactory
        {
            get { return this._mFactory; }
            set { this._mFactory = value; }
        }

        public RecordExpressionVisitor BaseRecordVisitor
        {
            get { return this._rFactory; }
            set { this._rFactory = value; }
        }

        public TableExpressionVisitor BaseTableVisitor
        {
            get { return this._tFactory; }
            set { this._tFactory = value; }
        }

        public SpoolSpaceContext Context
        {
            get { return this._Context; }
            set { this._Context = value; }
        }

        public Parameter Render(ArcticWindParser.ParamContext context)
        {

            // If null, treat as missing //
            if (context == null)
                return new Parameter();
            else if (context.scalar_expression() != null)
                return new Parameter(this._sFactory.Render(context.scalar_expression()));
            else if (context.matrix_expression() != null)
                return new Parameter(this._mFactory.Render(context.matrix_expression()));
            else if (context.record_expression() != null)
                return new Parameter(this._rFactory.Render(context.record_expression()));
            else if (context.table_expression() != null)
                return new Parameter(this._tFactory.Render(context.table_expression()));

            throw new Exception("Context is invalid");

        }

        public Parameter[] Render(ArcticWindParser.ParamContext[] context)
        {

            Parameter[] parameters = new Parameter[context.Length];
            
            int i = 0;

            // First look for all the tables //
            foreach (ArcticWindParser.ParamContext ctx in context)
            {

                if (ctx.table_expression() != null)
                {
                    TableExpression tex = this._tFactory.Render(ctx.table_expression());
                    this._Context.AddRecord(Host.GLOBAL, tex.Alias, tex.Columns);
                    parameters[i] = new Parameter(tex);
                }
                i++;

            }

            // Next look for all the non-tables //
            i = 0;
            foreach (ArcticWindParser.ParamContext ctx in context)
            {

                if (ctx.table_expression() == null)
                {
                    parameters[i] = this.Render(ctx);
                }
                i++;

            }

            return parameters;

        }

    }


}
