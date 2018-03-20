using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind;
using ArcticWind.Expressions;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Expressions.TableExpressions;
using ArcticWind.Expressions.ActionExpressions;
using ArcticWind.Tables;
using ArcticWind.Elements;

namespace ArcticWind.Expressions
{

    public class AbstractMethod
    {

        public const string LOCAL = "LOCAL";

        protected string _Name;
        protected Host _Host;
        protected Heap<ParameterAffinity> _Signiture;
        protected Heap<Parameter> _Parameters;
        protected Spool _Spool;
        protected List<ActionExpression> _Body;

        public AbstractMethod(Host Host, string Name)
        {
            this._Host = Host;
            this._Name = Name;
            this._Signiture = new Heap<ParameterAffinity>();
            this._Parameters = new Heap<Parameter>();
            this._Spool = new Spool(Host, LOCAL);
            this._Body = new List<ActionExpression>();
        }

        /// <summary>
        /// The name of the abstract member
        /// </summary>
        public string Name
        {
            get { return this._Name; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ParameterCount
        {
            get { return this._Signiture.Count; }
        }

        /// <summary>
        /// The local host
        /// </summary>
        public Host Host
        {
            get { return this._Host; }
        }

        /// <summary>
        /// Local spool
        /// </summary>
        public Spool Memory
        {
            get { return this._Spool; }
        }

        /// <summary>
        /// Generates a local spool space
        /// </summary>
        /// <param name="Spools"></param>
        /// <returns></returns>
        public SpoolSpace MemorySpace(SpoolSpace Spools)
        {
            SpoolSpace ss = new SpoolSpace(Spools);
            ss.Add(this.Memory.Name, this.Memory);
            return ss;
        }

        /// <summary>
        /// Adds a required parameter to the method
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Type"></param>
        public void AddSigniture(string Name, ParameterAffinity Type)
        {
            this._Signiture.Allocate(Name, Type);
        }

        /// <summary>
        /// Adds a parameter to the method
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        public void AddParameter(string Name, Parameter Value)
        {

            if (!this._Signiture.Exists(Name))
                throw new Exception(string.Format("Parameter '{0}' does not exist", Name));

            if (this._Signiture[Name] != Value.Affinity)
                throw new Exception(string.Format("Paramter '{0}' expects '{1}' but was passed '{2}'", Name, this._Signiture[Name], Value.Affinity));

            this._Parameters.Allocate(Name, Value);

        }

        /// <summary>
        /// Adds a parameter, assumed to be sequential 
        /// </summary>
        /// <param name="Value"></param>
        public void AddParameter(Parameter Value)
        {
            int idx = this._Parameters.Count;
            string n = this._Signiture.Name(idx);
            this.AddParameter(n, Value);
        }

        /// <summary>
        /// Adds a statement to the body of the expression
        /// </summary>
        /// <param name="Statement"></param>
        public void AddStatement(ActionExpression Statement)
        {
            this._Body.Add(Statement);
        }

        /// <summary>
        /// Checks the parameters match the signiture
        /// </summary>
        /// <param name="ExceptionMessage"></param>
        /// <returns></returns>
        public bool CheckSigniture(out string ExceptionMessage)
        {

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, ParameterAffinity> kv in this._Signiture.Entries)
            {
                if (!this._Parameters.Exists(kv.Key))
                    sb.AppendLine(string.Format("Missing '{0}'", kv.Key));
                else if (this._Parameters[kv.Key].Affinity != kv.Value)
                    sb.AppendLine(string.Format("Invalid affinity for '{0}', expecting '{0}', got '{2}'", kv.Key, kv.Value, this._Parameters[kv.Key].Affinity));
            }

            ExceptionMessage = sb.ToString();

            return (sb.Length == 0 ? true : false);

        }

        /// <summary>
        /// Check the parameters match the signiture
        /// </summary>
        /// <returns></returns>
        public bool CheckSigniture()
        {
            string x = null;
            return this.CheckSigniture(out x);
        }

        /// <summary>
        /// Binds parameters to the local spool
        /// </summary>
        /// <param name="Variants"></param>
        public void BindParameters(SpoolSpace Variants)
        {

            // Add each parameter to the spool //
            foreach (KeyValuePair<string,Parameter> kv in this._Parameters.Entries)
            {

                if (kv.Value.Affinity == ParameterAffinity.Scalar)
                {
                    this._Spool.SetScalar(kv.Key, kv.Value.Scalar.Evaluate(Variants));
                }
                else if (kv.Value.Affinity == ParameterAffinity.Matrix)
                {
                    this._Spool.SetMatrix(kv.Key, kv.Value.Matrix.Evaluate(Variants));
                }
                else if (kv.Value.Affinity == ParameterAffinity.Record)
                {
                    this._Spool.SetRecord(kv.Key, kv.Value.Record.EvaluateAssociative(Variants));
                }
                else if (kv.Value.Affinity == ParameterAffinity.Table)
                {
                    this._Spool.SetTable(kv.Key, kv.Value.Table.Select(Variants));
                }

            }

        }
        
        /// <summary>
        /// Runs the method 
        /// </summary>
        /// <param name="Variants"></param>
        public void Invoke(SpoolSpace Variants)
        {

            if (this._Parameters.Count != this._Signiture.Count)
                throw new Exception("Incorrect paramter count submitted");

            foreach (ActionExpression ae in this._Body)
            {

                ae.BeginInvoke(Variants);
                ae.Invoke(Variants);
                ae.EndInvoke(Variants);

                if (ae.Condition == ActionExpression.BreakLevel.Return)
                    break;

            }

        }

    }

    public sealed class TypedAbstractMethod : AbstractMethod
    {

        public const string RETURN_NAME = "<RETURN>";

        public TypedAbstractMethod(Host Host, string Name, CellAffinity Affinity, int Size)
            : base(Host, Name)
        {
            this.Affinity = Affinity;
            this.Size = Size;
        }

        public CellAffinity Affinity
        {
            get;
            private set;
        }

        public int Size
        {
            get;
            private set;
        }

    }

    public sealed class ColumnAbstractMethod : AbstractMethod
    {

        public const string RETURN_NAME = "<RETURN>";

        public ColumnAbstractMethod(Host Host, string Name, Schema Columns)
            : base(Host, Name)
        {
            this.Columns = Columns;
        }

        public Schema Columns
        {
            get;
            private set;
        }

    }

}
