using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using ArcticWind.Elements;
using ArcticWind.Expressions;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Expressions.TableExpressions;
using ArcticWind.Expressions.Aggregates;
using ArcticWind.Tables;


namespace ArcticWind.Expressions.TableExpressions
{

    /// <summary>
    /// Represents the base class for all table expressions
    /// </summary>
    public abstract class TableExpression : IBindable, IDisposable, IColumns, IRecyclable
    {

        protected List<TableExpression> _Children;
        protected TableExpression _Parent;
        protected Host _Host;
        protected List<Table> _RecycleBin;
        protected Key _OrderBy;
        protected Stopwatch _Timer;

        protected string _DB;
        protected string _Name;
        
        public TableExpression(Host Host, TableExpression Parent)
        {
            this._Host = Host;
            this._Parent = Parent;
            this._Children = new List<TableExpression>();
            this._RecycleBin = new List<Table>();
            this._OrderBy = new Key();
            this.IsClustered = false;
            this.IsDistinct = false;
            this._DB = Host.TEMP;
            this._Name = Host.RandomName;
        }

        // Tree support //
        /// <summary>
        /// All child expressions
        /// </summary>
        public List<TableExpression> Children
        {
            get { return this._Children; }
        }

        /// <summary>
        /// Gets the parent table; null if this is the root node
        /// </summary>
        public TableExpression Parent
        {
            get { return this._Parent; }
        }

        /// <summary>
        /// True if this is the first nod ein the tree
        /// </summary>
        public bool IsRoot
        {
            get { return this._Parent == null; }
        }

        /// <summary>
        /// True if this is the last node in the tree
        /// </summary>
        public bool IsTerminal
        {
            get { return this._Children.Count == 0; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Alias
        {
            get;
            set;
        }

        // Optimizations //
        /// <summary>
        /// If true, will write only distinct results to the output record set
        /// </summary>
        public bool IsDistinct
        {
            get;
            set;
        }

        /// <summary>
        /// True if the expression requires an order by
        /// </summary>
        public bool IsOrdered
        {
            get { return !this.OrderBy.IsEmpty; }
        }

        /// <summary>
        /// If true, then the expression will be written to a clustered table using the OrderBy key
        /// </summary>
        public bool IsClustered
        {
            get;
            set;
        }

        /// <summary>
        /// If non-empty, then the 
        /// </summary>
        public Key OrderBy
        {
            get { return this._OrderBy; }
            set { this._OrderBy = value; }
        }

        /// <summary>
        /// Gets the database this table will be saved to
        /// </summary>
        public string Database
        {
            get { return this._DB; }
            set { this._DB = value; }
        }

        /// <summary>
        /// Gets the name of the table on disk
        /// </summary>
        public string Name
        {
            get { return this._Name; }
            set { this._Name = value; }
        }

        // Output methods //
        /// <summary>
        /// Creates an output stream to write the data to
        /// </summary>
        /// <param name="Elements"></param>
        /// <returns></returns>
        public RecordWriter CreateWriter(Table Data)
        {

            // Don't do an order by and a cluster becuase it'll performa pointless sort
            if (this.IsOrdered && !this.IsClustered)
            {
                return new RecordWriterClustered(Data, this.OrderBy);
            }
            else if (this.IsDistinct)
            {
                return new RecordWriterDistinct(Data);
            }
            else
            {
                return new RecordWriterBase(Data);
            }

        }

        /// <summary>
        /// Creates a hard table
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public Table CreateTable(string DB, string Name)
        {

            if (this.IsClustered)
                return this._Host.CreateTable(DB, Name, this.Columns, this.OrderBy);
            return this._Host.CreateTable(DB, Name, this.Columns);

        }

        /// <summary>
        /// Creates a temp table to output to
        /// </summary>
        /// <returns></returns>
        public Table CreateTempTable()
        {
            return this.CreateTable(Host.TEMP, Host.RandomName);
        }

        /// <summary>
        /// Creates and loads all data from the expression into the table
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public Table RenderTable(string DB, string Name, SpoolSpace Variants)
        {
            Table t = this.CreateTable(DB, Name);
            using (RecordWriter w = this.CreateWriter(t))
            {
                this.Evaluate(Variants, w);
            }
            return t;
        }

        /// <summary>
        /// All child tables
        /// </summary>
        public IEnumerable<Table> RenderChildTables(SpoolSpace Variants)
        {
            return new TableCollection(this._Children, Variants);
        }

        // Methods //
        /// <summary>
        /// Adds a child expression to the set
        /// </summary>
        /// <param name="Child"></param>
        public virtual void AddChild(TableExpression Child)
        {
            Child._Parent = this;
            this._Children.Add(Child);
        }

        /// <summary>
        /// Adds many children to the expression
        /// </summary>
        /// <param name="Children"></param>
        public void AddChildren(List<TableExpression> Children)
        {
            foreach (TableExpression t in Children)
            {
                this.AddChild(t);
            }
        }

        // Abstracts and virtuals //
        /// <summary>
        /// Gets the columns of the output table
        /// </summary>
        public abstract Schema Columns
        {
            get;
        }

        /// <summary>
        /// Creates and primes a resolver
        /// </summary>
        /// <param name="Variants"></param>
        /// <returns></returns>
        public abstract SpoolSpace CreateResolver(SpoolSpace Variants);

        // Appends //
        /// <summary>
        /// Writes the value of expression to a table
        /// </summary>
        /// <param name="Writer"></param>
        public abstract void Evaluate(SpoolSpace Variants, RecordWriter Writer);

        /// <summary>
        /// Evaluates the table expression, removing duplicates before appending
        /// </summary>
        /// <param name="Variants"></param>
        /// <param name="Writer"></param>
        public virtual void AppendDistinct(SpoolSpace Variants, RecordWriter Writer)
        {

            // Get the temp table //
            Table t = this._Host.CreateTable(Host.TEMP, Host.RandomName, this.Columns);

            // Write to the temp //
            using (RecordWriter rw = t.OpenWriter())
            {
                this.Evaluate(Variants, rw);
            }

            // Distinct the table //
            TableUtil.Distinct(t, Writer, this.Columns.GetKey());

        }

        /// <summary>
        /// Evaluates the table expression, sorting before appending
        /// </summary>
        /// <param name="Variants"></param>
        /// <param name="Writer"></param>
        public virtual void AppendOrdered(SpoolSpace Variants, RecordWriter Writer)
        {

            // Get the temp table //
            Table t = this._Host.CreateTable(Host.TEMP, Host.RandomName, this.Columns);

            // Write to the temp //
            using (RecordWriter rw = t.OpenWriter())
            {
                this.Evaluate(Variants, rw);
            }

            // Sort the table //
            TableUtil.Sort(t, Key.Build(this.Columns.Count));

            // Write to the source table //
            using (RecordReader rr = t.OpenReader())
            {
                Writer.Consume(rr);
            }

        }

        /// <summary>
        /// Evaluates the table expression, removing duplicates and sorting before appending
        /// </summary>
        /// <param name="Variants"></param>
        /// <param name="Writer"></param>
        public virtual void AppendDistinctOrdered(SpoolSpace Variants, RecordWriter Writer)
        {

            // Get the temp table //
            Table t = this._Host.CreateTable(Host.TEMP, Host.RandomName, this.Columns);

            // Write to the temp //
            using (RecordWriter rw = t.OpenWriter())
            {
                this.Evaluate(Variants, rw);
            }

            // Distinct the table //
            TableUtil.Distinct(t, Writer, t.Columns.GetKey(this.OrderBy));

        }

        /// <summary>
        /// Appends data to the table, sorting, distincting, or both, before inserting the records
        /// </summary>
        /// <param name="Variants"></param>
        /// <param name="Writer"></param>
        public virtual void Append(SpoolSpace Variants, RecordWriter Writer)
        {

            // Check the modifiers //
            if (this.IsDistinct && this.IsOrdered)
            {
                this.AppendDistinctOrdered(Variants, Writer);
            }
            else if (this.IsDistinct)
            {
                this.AppendDistinct(Variants, Writer);
            }
            else if (this.IsOrdered)
            {
                this.AppendOrdered(Variants, Writer);
            }
            else
            {
                this.Evaluate(Variants, Writer);
            }

        }

        public virtual void InitializeResolver(SpoolSpace Variants)
        {
            // Do nothing //
        }

        public virtual void CleanUpResolver(SpoolSpace Variants)
        {
            // Do nothing //
        }

        public virtual void Bind(string Name, Parameter Value)
        {
            // Do something
        }

        // Creates //
        public virtual Table CreateDistinct(SpoolSpace Variants, string DB, string Name)
        {

            Table t = this._Host.CreateTable(DB, Name, this.Columns);
            using (RecordWriter rw = t.OpenWriter())
            {
                this.AppendDistinct(Variants, rw);
            }
            return t;

        }

        public virtual Table CreateOrdered(SpoolSpace Variants, string DB, string Name)
        {

            Table t = this._Host.CreateTable(DB, Name, this.Columns);
            using (RecordWriter rw = t.OpenWriter())
            {
                this.AppendOrdered(Variants, rw);
            }
            return t;

        }

        public virtual Table CreateDistinctOrdered(SpoolSpace Variants, string DB, string Name)
        {

            Table t = this._Host.CreateTable(DB, Name, this.Columns);
            using (RecordWriter rw = t.OpenWriter())
            {
                this.AppendDistinctOrdered(Variants, rw);
            }
            return t;

        }

        public virtual Table Create(SpoolSpace Variants, string DB, string Name)
        {
            Table t = this._Host.CreateTable(DB, Name, this.Columns);
            using (RecordWriter rw = t.OpenWriter())
            {
                this.Append(Variants, rw);
            }
            return t;
        }

        public virtual Table Select(SpoolSpace Variants)
        {
            Table t = this.Create(Variants, this.Database, this.Name);
            this._RecycleBin.Add(t);
            return t;
        }

        // Recycling //
        /// <summary>
        /// Cleans up any resources no longer needed
        /// </summary>
        public virtual void Recycle()
        {

            // Recycle this node //
            foreach (Table t in this._RecycleBin)
            {
                this._Host.TableStore.DropTable(t.Key);
            }
            this._RecycleBin = new List<Table>();

        }

        /// <summary>
        /// Recycling
        /// </summary>
        public virtual void RecycleAll()
        {

            this.Recycle();

            // Recycle all children //
            foreach (TableExpression te in this._Children)
            {
                te.Recycle();
            }

        }

        /// <summary>
        /// Binds a pointer expression to a value expression
        /// </summary>
        /// <param name="PointerRef"></param>
        /// <param name="Value"></param>
        public virtual void Bind(string PointerRef, ScalarExpression Value)
        {
            this._Children.ForEach((x) => { x.Bind(PointerRef, Value); });
        }

        /// <summary>
        /// Gets an estimate of the records of underlying table
        /// </summary>
        public virtual long EstimatedCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Checks if an expression will yield an indexed table
        /// </summary>
        /// <param name="IndexColumns"></param>
        /// <returns></returns>
        public virtual bool IsIndexedBy(Key IndexColumns)
        {
            return Key.LeftSubsetStrong(IndexColumns, this.OrderBy);
        }

        /// <summary>
        /// Returns meta data about the expression rendering
        /// </summary>
        /// <returns></returns>
        public virtual string MetaData()
        {
            return "META_DATA";
        }

        public void Dispose()
        {
            this.RecycleAll();
        }

        // Internal Classes //
        /// <summary>
        /// Represents a way to enumerate over all the child tables
        /// </summary>
        public sealed class TableCollection : IEnumerable<Table>, IEnumerator<Table>, IEnumerable, IEnumerator, IDisposable
        {

            private int _Position = -1;
            private List<TableExpression> _Nodes;
            private SpoolSpace _Resolver;

            public TableCollection(List<TableExpression> Nodes, SpoolSpace Variants)
            {
                this._Nodes = Nodes;
                this._Resolver = Variants;
            }

            public bool MoveNext()
            {
                this._Position++;
                return (this._Position < this._Nodes.Count);
            }

            public void Reset()
            {
                this._Position = -1;
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public Table Current
            {
                get
                {
                    return this._Nodes[this._Position].Select(this._Resolver);
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this;
            }

            IEnumerator<Table> IEnumerable<Table>.GetEnumerator()
            {
                return this;
            }

            public void Dispose()
            {
            }

        }

    }

    public abstract class TableExpressionFunction : TableExpression
    {

        private string _Name;
        private int _ParamCount = -1;
        private List<Parameter> _Parameters;

        public TableExpressionFunction(Host Host, TableExpression Parent, string Name, int Parameters)
            : base(Host, Parent)
        {
            this._Name = Name;
            this._ParamCount = Parameters;
            this._Parameters = new List<Parameter>();
        }

        public virtual bool IsVolatile
        {
            get { return true; }
        }

        public int ParameterCount
        {
            get { return this._ParamCount; }
        }

        public void AddParameter(Parameter Value)
        {
            this._Parameters.Add(Value);
        }

        public void CheckParameters()
        {
            
            if (this._ParamCount < 0 && this._Parameters.Count > (-this._ParamCount))
            {
                throw new Exception(string.Format("Function '{0}' can have at most '{1}' parameter(s) but was passed '{2}'", this._Name, -this._ParamCount, this._Parameters.Count));
            }
            else if (this._Parameters.Count != this._ParamCount)
            {
                throw new Exception(string.Format("Function '{0}' can have exactly '{1}' parameter(s) but was passed '{2}'", this._Name, -this._ParamCount, this._Parameters.Count));
            }

        }



    }

}
