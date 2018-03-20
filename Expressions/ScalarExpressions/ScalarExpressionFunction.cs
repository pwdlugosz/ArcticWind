using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Libraries;

namespace ArcticWind.Expressions.ScalarExpressions
{

    /// <summary>
    /// Represents a functional expression
    /// </summary>
    public abstract class ScalarExpressionFunction : ScalarExpression
    {

        protected int _MaxParamterCount;
        protected bool _DynamicReturn;
        protected CellAffinity _ReturnAffinity;
        protected List<Parameter> _Params;
        protected Host _Host;

        public ScalarExpressionFunction(Host Host, ScalarExpression Parent, string Name, int ParameterCount)
            : base(Parent, ScalarExpressionAffinity.Function)
        {
            this.Name = Name;
            this._MaxParamterCount = ParameterCount;
            this._DynamicReturn = true;
            this._Host = Host;
            this._Params = new List<Parameter>();
        }

        public ScalarExpressionFunction(Host Host, ScalarExpression Parent, string Name, int ParameterCount, CellAffinity ReturnAffinity)
            : base(Parent, ScalarExpressionAffinity.Function)
        {
            this.Name = Name;
            this._MaxParamterCount = ParameterCount;
            this._ReturnAffinity = ReturnAffinity;
            this._DynamicReturn = false;
            this._Host = Host;
            this._Params = new List<Parameter>();
        }

        /// <summary>
        /// Unparses an expression
        /// </summary>
        /// <param name="Variants"></param>
        /// <returns></returns>
        public override string Unparse(SpoolSpace Variants)
        {
            return null;
        }

        /// <summary>
        /// Returns the cell affinity
        /// </summary>
        /// <param name="Variants"></param>
        /// <returns></returns>
        public override CellAffinity ReturnAffinity()
        {
            return this.MaxReturnAffinityOfScalarOrMatrixParameters().Affinity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Variants"></param>
        /// <returns></returns>
        public override int ReturnSize()
        {
            return this.MaxReturnAffinityOfScalarOrMatrixParameters().Size;
        }

        /// <summary>
        /// Gets the parameter count; a negative number indicates the paramters are variable; a positive number means the paramter count must be exact
        /// </summary>
        public int MaxParameterCount
        {
            get { return this._MaxParamterCount; }
        }

        /// <summary>
        /// Checks if any of the child nodes are volatile
        /// </summary>
        public override bool IsVolatile
        {
            get
            {
                foreach (Parameter p in this._Params)
                {
                    if (p.Affinity == ParameterAffinity.Scalar && p.Scalar.IsVolatile)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        public virtual void AddParameter(Parameter Value)
        {
            this._Params.Add(Value);
        }

        /// <summary>
        /// 
        /// </summary>
        public void CheckParameters()
        {

            if (this._MaxParamterCount < 0 && this._Params.Count > (-this._MaxParamterCount))
            {
                throw new Exception(string.Format("Function '{0}' can have at most '{1}' parameter(s) but was passed '{2}'", this._name, -this._MaxParamterCount, this._Params.Count));
            }
            else if (this._Params.Count != this._MaxParamterCount)
            {
                throw new Exception(string.Format("Function '{0}' can have exactly '{1}' parameter(s) but was passed '{2}'", this._name, -this._MaxParamterCount, this._Params.Count));
            }

        }

        public override void Bind(string Name, Parameter Value)
        {

            foreach (Parameter p in this._Params)
            {
                if (p.Affinity == ParameterAffinity.Scalar)
                    p.Scalar.Bind(Name, Value);
                else if (p.Affinity == ParameterAffinity.Matrix)
                    p.Matrix.Bind(Name, Value);
                else if (p.Affinity == ParameterAffinity.Record)
                    p.Record.Bind(Name, Value);
                else if (p.Affinity == ParameterAffinity.Table)
                    p.Table.Bind(Name, Value);
            }

        }

        public bool AllScalars()
        {
            return this._Params.TrueForAll((x) => { return x.Affinity == ParameterAffinity.Scalar; });
        }

        public bool AllMatrixes()
        {
            return this._Params.TrueForAll((x) => { return x.Affinity == ParameterAffinity.Matrix; });
        }

        public bool AllRecords()
        {
            return this._Params.TrueForAll((x) => { return x.Affinity == ParameterAffinity.Record; });
        }

        public bool AllTables()
        {
            return this._Params.TrueForAll((x) => { return x.Affinity == ParameterAffinity.Table; });
        }

        public bool CheckSigniture(params ParameterAffinity[] Paramters)
        {

            if (Paramters == null)
            {
                return (this._Params == null || this._Params.Count == 0);
            }

            if (this._Params.Count != Paramters.Length)
                return false;

            for (int i = 0; i < this._Params.Count; i++)
            {
                if (this._Params[i].Affinity != Paramters[i])
                    return false;
            }

            return true;

        }

        protected FunctionMeta MaxReturnAffinityOfScalarParamters()
        {

            FunctionMeta f = new FunctionMeta();
            foreach (Parameter p in this._Params)
            {
                if (p.Affinity == ParameterAffinity.Scalar)
                    f = FunctionMeta.Max(f, new FunctionMeta(p.Scalar));
            }
            return f;

        }

        protected FunctionMeta MaxReturnAffinityOfMatrixParameters()
        {

            FunctionMeta f = new FunctionMeta();
            foreach (Parameter p in this._Params)
            {
                if (p.Affinity == ParameterAffinity.Matrix)
                    f = FunctionMeta.Max(f, new FunctionMeta(p.Matrix));
            }
            return f;

        }

        protected FunctionMeta MaxReturnAffinityOfScalarOrMatrixParameters()
        {

            FunctionMeta f = new FunctionMeta();
            foreach (Parameter p in this._Params)
            {
                if (p.Affinity == ParameterAffinity.Scalar)
                    f = FunctionMeta.Max(f, new FunctionMeta(p.Scalar));
                else if (p.Affinity == ParameterAffinity.Matrix)
                    f = FunctionMeta.Max(f, new FunctionMeta(p.Matrix));
            }
            return f;

        }

        protected struct FunctionMeta
        {

            private CellAffinity _A;
            private int _S;

            public FunctionMeta(CellAffinity Affinity, int Size)
            {
                this._A = Affinity;
                this._S = (CellAffinityHelper.IsVariableLength(Affinity) ? Size : CellSerializer.DefaultLength(Affinity));
            }

            public FunctionMeta(ScalarExpression X)
                :this(X.ReturnAffinity(), X.ReturnSize())
            {
            }

            public FunctionMeta(MatrixExpressions.MatrixExpression X)
                : this(X.ReturnAffinity(), X.ReturnSize())
            {
            }

            public CellAffinity Affinity 
            {
                get { return this._A; }
                private set { this._A = value; } 
            }

            public int Size 
            {
                get { return this._S; }
                private set { this._S = value; }
            }

            public static FunctionMeta MinValue
            {
                get { return new FunctionMeta(CellAffinity.BOOL, -1); }
            }

            public static FunctionMeta MaxValue
            {
                get { return new FunctionMeta(CellAffinity.CSTRING, 4096); }
            }

            public static bool operator ==(FunctionMeta A, FunctionMeta B)
            {
                return A.Affinity == B.Affinity && A.Size == B.Size;
            }

            public static bool operator !=(FunctionMeta A, FunctionMeta B)
            {
                return A.Affinity != B.Affinity || A.Size != B.Size;
            }

            public static bool operator <(FunctionMeta A, FunctionMeta B)
            {
                if (A.Affinity < B.Affinity)
                    return true;
                return A.Size < B.Size;
            }

            public static bool operator <=(FunctionMeta A, FunctionMeta B)
            {
                if (A.Affinity <= B.Affinity)
                    return true;
                return A.Size <= B.Size;
            }

            public static bool operator >(FunctionMeta A, FunctionMeta B)
            {
                if (A.Affinity > B.Affinity)
                    return true;
                return A.Size > B.Size;
            }

            public static bool operator >=(FunctionMeta A, FunctionMeta B)
            {
                if (A.Affinity >= B.Affinity)
                    return true;
                return A.Size >= B.Size;
            }

            public static FunctionMeta Min(FunctionMeta A, FunctionMeta B)
            {
                return (A > B ? B : A);
            }

            public static FunctionMeta Max(FunctionMeta A, FunctionMeta B)
            {
                return (A > B ? A : B);
            }

        }

        //---------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------

        // Unary Opperations //
        //public const string NAME_NOT = "NOT";
        //public const string NAME_PLUS = "PLUS";
        //public const string NAME_MINUS = "MINUS";

        //// Binary Opperations //
        //public const string NAME_ADD = "ADD";
        //public const string NAME_SUB = "SUB";
        //public const string NAME_MULT = "MULT";
        //public const string NAME_DIV = "DIV";
        //public const string NAME_CDIV = "CDIV";
        //public const string NAME_MOD = "MOD";
        //public const string NAME_POWER = "POWER";
        //public const string NAME_EQ = "EQ";
        //public const string NAME_NEQ = "NEQ";
        //public const string NAME_GT = "GT";
        //public const string NAME_GTE = "GTE";
        //public const string NAME_LT = "LT";
        //public const string NAME_LTE = "LTE";
        //public const string NAME_AND = "AND";
        //public const string NAME_OR = "OR";
        //public const string NAME_XOR = "XOR";

        //// Special Opperations //
        //public const string NAME_IF = "IF";
        //public const string NAME_IFNULL = "IF_NULL";
        //public const string NAME_CAST = "CAST";

        //// Special Functions //
        //public const string NAME_LIKE = "LIKE";
        //public const string NAME_MATCH = "MATCH";
        //public const string NAME_GUID = "GUID";
        //public const string NAME_THREAD_ID = "THREAD_ID";
        //public const string NAME_RAND_BOOL = "RAND_BOOL";
        //public const string NAME_RAND_DATE = "RAND_DATE";
        //public const string NAME_RAND_INT = "RAND_INT";
        //public const string NAME_RAND_NUM = "RAND_NUM";
        //public const string NAME_RAND_BLOB = "RAND_BLOB";
        //public const string NAME_RAND_STRING = "RAND_STRING";

        //// Date Functions //
        //public const string NAME_DATE_BUILD = "DATE_BUILD";
        //public const string NAME_NOW = "NOW";
        //public const string NAME_YEAR = "YEAR";
        //public const string NAME_MONTH = "MONTH";
        //public const string NAME_DAY = "DAY";
        //public const string NAME_HOUR = "HOUR";
        //public const string NAME_MINUTE = "MINUTE";
        //public const string NAME_SECOND = "SECOND";
        //public const string NAME_MILISECOND = "MILISECOND";
        //public const string NAME_TICKS = "TICKS";
        //public const string NAME_ELAPSED = "ELAPSED";
        //public const string NAME_TIME_SPAN = "TIME_SPAN";

        //// String Functions //
        //public const string NAME_SUBSTR = "SUBSTR";
        //public const string NAME_REPLACE = "REPLACE";
        //public const string NAME_POSITION = "POSITION";
        //public const string NAME_LENGTH = "LENGTH";
        //public const string NAME_TRIM = "TRIM";
        //public const string NAME_TO_UTF16 = "TO_UTF16";
        //public const string NAME_TO_UTF8 = "TO_UTF8";
        //public const string NAME_TO_HEX = "TO_HEX";
        //public const string NAME_FROM_UTF16 = "FROM_UTF16";
        //public const string NAME_FROM_UTF8 = "FROM_UTF8";
        //public const string NAME_FROM_HEX = "FROM_HEX";

        //// Numerics //
        //public const string NAME_LOG = "LOG";
        //public const string NAME_EXP = "EXP";
        //public const string NAME_SQRT = "SQRT";
        //public const string NAME_SIN = "SIN";
        //public const string NAME_COS = "COS";
        //public const string NAME_TAN = "TAN";
        //public const string NAME_SINH = "SINH";
        //public const string NAME_COSH = "COSH";
        //public const string NAME_TANH = "TANH";
        //public const string NAME_LOGIT = "LOGIT";
        //public const string NAME_NORMAL = "NORMAL";
        //public const string NAME_ABS = "ABS";
        //public const string NAME_SIGN = "SIGN";
        //public const string NAME_ROUND = "ROUND";

        //// Meta //
        //public const string NAME_FORMULA = "FORMULA";
        //public const string NAME_TYPEOF = "TYPEOF";
        //public const string NAME_SIZEOF = "SIZEOF";

        //// Uninary Opperations //
        //public class ExpressionNot : ScalarExpressionFunction
        //{

        //    public ExpressionNot(Host Host)
        //        : base(Host, null, NAME_NOT, 1)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionNot(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return !this._ChildNodes[0].Evaluate(Variants);

        //    }

        //}

        //public class ExpressionPlus : ScalarExpressionFunction
        //{

        //    public ExpressionPlus(Host Host)
        //        : base(Host, null, NAME_ADD, 1)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionPlus(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return +this._ChildNodes[0].Evaluate(Variants);

        //    }

        //}

        //public class ExpressionMinus : ScalarExpressionFunction
        //{

        //    public ExpressionMinus(Host Host)
        //        : base(Host, null, NAME_MINUS, 1)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionMinus(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return -this._ChildNodes[0].Evaluate(Variants);

        //    }

        //}

        //// Binary Opperations //
        //public class ExpressionAdd : ScalarExpressionFunction
        //{

        //    public ExpressionAdd(Host Host)
        //        : base(Host, null, NAME_ADD, 2)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionAdd(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return this._ChildNodes[0].Evaluate(Variants) + this._ChildNodes[1].Evaluate(Variants);

        //    }

        //}

        //public class ExpressionSubtract : ScalarExpressionFunction
        //{

        //    public ExpressionSubtract(Host Host)
        //        : base(Host, null, NAME_SUB, 2)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionSubtract(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return this._ChildNodes[0].Evaluate(Variants) - this._ChildNodes[1].Evaluate(Variants);

        //    }

        //}

        //public class ExpressionMultiply : ScalarExpressionFunction
        //{

        //    public ExpressionMultiply(Host Host)
        //        : base(Host, null, NAME_MULT, 2)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionMultiply(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return this._ChildNodes[0].Evaluate(Variants) * this._ChildNodes[1].Evaluate(Variants);

        //    }

        //}

        //public class ExpressionDivide : ScalarExpressionFunction
        //{

        //    public ExpressionDivide(Host Host)
        //        : base(Host, null, NAME_DIV, 2)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionDivide(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return this._ChildNodes[0].Evaluate(Variants) / this._ChildNodes[1].Evaluate(Variants);

        //    }

        //}

        //public class ExpressionCheckedDivide : ScalarExpressionFunction
        //{

        //    public ExpressionCheckedDivide(Host Host)
        //        : base(Host, null, NAME_CDIV, 2)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionCheckedDivide(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return Cell.CheckDivide(this._ChildNodes[0].Evaluate(Variants), this._ChildNodes[1].Evaluate(Variants));

        //    }

        //}

        //public class ExpressionModulo : ScalarExpressionFunction
        //{

        //    public ExpressionModulo(Host Host)
        //        : base(Host, null, NAME_MOD, 2)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionModulo(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return this._ChildNodes[0].Evaluate(Variants) % this._ChildNodes[1].Evaluate(Variants);

        //    }

        //}

        //public class ExpressionPower : ScalarExpressionFunction
        //{

        //    public ExpressionPower(Host Host)
        //        : base(Host, null, NAME_POWER, 2)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionPower(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return CellFunctions.Power(this._ChildNodes[0].Evaluate(Variants), this._ChildNodes[1].Evaluate(Variants));

        //    }

        //}

        //public class ExpressionEquals : ScalarExpressionFunction
        //{

        //    public ExpressionEquals(Host Host)
        //        : base(Host, null, NAME_EQ, 2, CellAffinity.BOOL)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionEquals(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return this._ChildNodes[0].Evaluate(Variants) == this._ChildNodes[1].Evaluate(Variants) ? CellValues.True : CellValues.False;

        //    }

        //}

        //public class ExpressionNotEquals : ScalarExpressionFunction
        //{

        //    public ExpressionNotEquals(Host Host)
        //        : base(Host, null, NAME_EQ, 2, CellAffinity.BOOL)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionNotEquals(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return this._ChildNodes[0].Evaluate(Variants) != this._ChildNodes[1].Evaluate(Variants) ? CellValues.True : CellValues.False;

        //    }

        //}

        //public class ExpressionLessThan : ScalarExpressionFunction
        //{

        //    public ExpressionLessThan(Host Host)
        //        : base(Host, null, NAME_LT, 2, CellAffinity.BOOL)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionLessThan(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return this._ChildNodes[0].Evaluate(Variants) < this._ChildNodes[1].Evaluate(Variants) ? CellValues.True : CellValues.False;

        //    }

        //}

        //public class ExpressionLessThanOrEqualTo : ScalarExpressionFunction
        //{

        //    public ExpressionLessThanOrEqualTo(Host Host)
        //        : base(Host, null, NAME_LTE, 2, CellAffinity.BOOL)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionLessThanOrEqualTo(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return this._ChildNodes[0].Evaluate(Variants) <= this._ChildNodes[1].Evaluate(Variants) ? CellValues.True : CellValues.False;

        //    }

        //}

        //public class ExpressionGreaterThan : ScalarExpressionFunction
        //{

        //    public ExpressionGreaterThan(Host Host)
        //        : base(Host, null, NAME_GT, 2, CellAffinity.BOOL)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionGreaterThan(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return this._ChildNodes[0].Evaluate(Variants) > this._ChildNodes[1].Evaluate(Variants) ? CellValues.True : CellValues.False;

        //    }

        //}

        //public class ExpressionGreaterThanOrEqualTo : ScalarExpressionFunction
        //{

        //    public ExpressionGreaterThanOrEqualTo(Host Host)
        //        : base(Host, null, NAME_GTE, 2, CellAffinity.BOOL)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionGreaterThanOrEqualTo(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return this._ChildNodes[0].Evaluate(Variants) >= this._ChildNodes[1].Evaluate(Variants) ? CellValues.True : CellValues.False;

        //    }

        //}

        //public class ExpressionAnd : ScalarExpressionFunction
        //{

        //    public ExpressionAnd(Host Host)
        //        : base(Host, null, NAME_AND, 2)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionAnd(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return this._ChildNodes[0].Evaluate(Variants) & this._ChildNodes[1].Evaluate(Variants);

        //    }

        //}

        //public class ExpressionOr : ScalarExpressionFunction
        //{

        //    public ExpressionOr(Host Host)
        //        : base(Host, null, NAME_OR, 2)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionOr(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return this._ChildNodes[0].Evaluate(Variants) | this._ChildNodes[1].Evaluate(Variants);

        //    }

        //}

        //public class ExpressionXor : ScalarExpressionFunction
        //{

        //    public ExpressionXor(Host Host)
        //        : base(Host, null, NAME_XOR, 2)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionXor(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return this._ChildNodes[0].Evaluate(Variants) ^ this._ChildNodes[1].Evaluate(Variants);

        //    }

        //}

        //// Special Opperations //
        //public class ExpressionIf : ScalarExpressionFunction
        //{

        //    public ExpressionIf(Host Host)
        //        : base(Host, null, NAME_IF, 3)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionIf(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return CellFunctions.If(this._ChildNodes[0].Evaluate(Variants), this._ChildNodes[1].Evaluate(Variants), this._ChildNodes[2].Evaluate(Variants));

        //    }

        //}

        //public class ExpressionIfNull : ScalarExpressionFunction
        //{

        //    public ExpressionIfNull(Host Host)
        //        : base(Host, null, NAME_IFNULL, 2)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionIfNull(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell x = this._ChildNodes[0].Evaluate(Variants);
        //        return x.IsNull ? this._ChildNodes[1].Evaluate(Variants) : x;

        //    }

        //}

        //public class ExpressionCast : ScalarExpressionFunction
        //{

        //    private CellAffinity _t;

        //    public ExpressionCast(Host Host, CellAffinity Type)
        //        : base(Host, null, NAME_CAST, 1)
        //    {
        //        this._t = Type;
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionCast(this._Host, this._t);
        //    }

        //    public override CellAffinity ReturnAffinity()
        //    {
        //        return this._t;
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return CellConverter.Cast(this._ChildNodes[0].Evaluate(Variants), this._t);

        //    }

        //}

        //// Special Functions //
        //public class ExpressionLike : ScalarExpressionFunction
        //{

        //    public const char WILD_CARD = '*';

        //    public ExpressionLike(Host Host)
        //        : base(Host, null, NAME_LIKE, 2, CellAffinity.BOOL)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionLike(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell cText = this._ChildNodes[0].Evaluate(Variants);
        //        Cell cPatern = this._ChildNodes[1].Evaluate(Variants);
        //        string Text = cText.valueCSTRING;
        //        string Patern = cPatern.valueCSTRING;

        //        if (cText.IsNull || cPatern.IsNull)
        //            return CellValues.False;

        //        bool x = false, y = false, z = false;

        //        if (Patern.First() == WILD_CARD)
        //        {
        //            Patern = Patern.Remove(0, 1);
        //            x = true;
        //        }

        //        if (Patern.Last() == WILD_CARD)
        //        {
        //            Patern = Patern.Remove(Patern.Length - 1, 1);
        //            y = true;
        //        }

        //        if (x && y) // '*Hello World*' //
        //        {
        //            z = Text.ToUpper().Contains(Patern.ToUpper());
        //        }
        //        else if (x && !y) // '*Hello World' //
        //        {
        //            z = Text.EndsWith(Patern, StringComparison.OrdinalIgnoreCase);
        //        }
        //        else if (!x && y) // 'Hello World*' //
        //        {
        //            z = Text.StartsWith(Patern, StringComparison.OrdinalIgnoreCase);
        //        }
        //        else // !OriginalPage && !NewNode // 'Hello World' //
        //        {
        //            z = string.Equals(Text, Patern, StringComparison.OrdinalIgnoreCase);
        //        }

        //        return new Cell(z);

        //    }

        //}

        //public class ExpressionMatch : ScalarExpressionFunction
        //{

        //    public ExpressionMatch(Host Host)
        //        : base(Host, null, NAME_MATCH, -128, CellAffinity.LONG)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionMatch(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count <= 1)
        //            return CellValues.NullLONG;

        //        Cell x = this._ChildNodes[0].Evaluate(Variants);
        //        for (int i = 1; i < this._ChildNodes.Count; i++)
        //        {

        //            Cell y = this._ChildNodes[i].Evaluate(Variants);
        //            if (x == y)
        //                return new Cell(i - 1);

        //        }

        //        return CellValues.NullLONG;

        //    }

        //}

        //public class ExpressionGUID : ScalarExpressionFunction
        //{

        //    public ExpressionGUID(Host Host)
        //        : base(Host, null, NAME_GUID, 0, CellAffinity.BINARY)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionGUID(this._Host);
        //    }

        //    public override bool IsVolatile
        //    {
        //        get { return true; }
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {
        //        return new Cell(Guid.NewGuid().ToByteArray());
        //    }

        //}

        //public class ExpressionThreadID : ScalarExpressionFunction
        //{

        //    public ExpressionThreadID(Host Host)
        //        : base(Host, null, NAME_THREAD_ID, 0, CellAffinity.LONG)
        //    {
        //    }

        //    public override bool IsVolatile
        //    {
        //        get { return true; }
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionThreadID(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {
        //        return new Cell(System.Threading.Thread.CurrentThread.ManagedThreadId);
        //    }

        //}

        //// Date Functions //
        //public class ExpressionDateBuild : ScalarExpressionFunction
        //{

        //    public ExpressionDateBuild(Host Host)
        //        : base(Host, null, NAME_DATE_BUILD, -7, CellAffinity.DATE_TIME)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionDateBuild(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count == 0)
        //            return CellValues.NullDATE;

        //        int[] xdate = new int[7];

        //        for (int i = 0; i < 7; i++)
        //        {

        //            if (this._ChildNodes.Count >= i)
        //            {
        //                xdate[i] = (int)this._ChildNodes[i].Evaluate(Variants).valueLONG;
        //            }

        //        }

        //        DateTime x = new DateTime(xdate[0], xdate[1], xdate[2], xdate[3], xdate[4], xdate[5], xdate[6]);

        //        return new Cell(x);

        //    }

        //}

        //public class ExpressionNow : ScalarExpressionFunction
        //{

        //    public ExpressionNow(Host Host)
        //        : base(Host, null, NAME_NOW, 0, CellAffinity.DATE_TIME)
        //    {
        //    }

        //    public override bool IsVolatile
        //    {
        //        get { return true; }
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionNow(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {
        //        return new Cell(DateTime.Now);
        //    }

        //}

        //public class ExpressionYear : ScalarExpressionFunction
        //{

        //    public ExpressionYear(Host Host)
        //        : base(Host, null, NAME_YEAR, 1, CellAffinity.LONG)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionYear(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return CellFunctions.Year(this._ChildNodes[0].Evaluate(Variants));

        //    }

        //}

        //public class ExpressionMonth : ScalarExpressionFunction
        //{

        //    public ExpressionMonth(Host Host)
        //        : base(Host, null, NAME_MONTH, 1, CellAffinity.LONG)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionMonth(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return CellFunctions.Month(this._ChildNodes[0].Evaluate(Variants));

        //    }

        //}

        //public class ExpressionDay : ScalarExpressionFunction
        //{

        //    public ExpressionDay(Host Host)
        //        : base(Host, null, NAME_DAY, 1, CellAffinity.LONG)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionDay(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return CellFunctions.Day(this._ChildNodes[0].Evaluate(Variants));

        //    }

        //}

        //public class ExpressionHour : ScalarExpressionFunction
        //{

        //    public ExpressionHour(Host Host)
        //        : base(Host, null, NAME_HOUR, 1, CellAffinity.LONG)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionHour(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return CellFunctions.Hour(this._ChildNodes[0].Evaluate(Variants));

        //    }

        //}

        //public class ExpressionMinute : ScalarExpressionFunction
        //{

        //    public ExpressionMinute(Host Host)
        //        : base(Host, null, NAME_MINUTE, 1, CellAffinity.LONG)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionMinute(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return CellFunctions.Minute(this._ChildNodes[0].Evaluate(Variants));

        //    }

        //}

        //public class ExpressionSecond : ScalarExpressionFunction
        //{

        //    public ExpressionSecond(Host Host)
        //        : base(Host, null, NAME_SECOND, 1, CellAffinity.LONG)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionSecond(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return CellFunctions.Second(this._ChildNodes[0].Evaluate(Variants));

        //    }

        //}

        //public class ExpressionMillisecond : ScalarExpressionFunction
        //{

        //    public ExpressionMillisecond(Host Host)
        //        : base(Host, null, NAME_MILISECOND, 1, CellAffinity.LONG)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionMillisecond(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return CellFunctions.Millisecond(this._ChildNodes[0].Evaluate(Variants));

        //    }

        //}

        //public class ExpressionElapsed : ScalarExpressionFunction
        //{

        //    private Host _Host;

        //    public ExpressionElapsed(Host Host)
        //        : base(Host, null, NAME_ELAPSED, 0, CellAffinity.LONG)
        //    {
        //        this._Host = Host;
        //    }

        //    public override bool IsVolatile
        //    {
        //        get { return true; }
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionElapsed(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        return new Cell(DateTime.Now.Ticks - this._Host.StartTicks);

        //    }

        //}

        //public class ExpressionTimeSpan : ScalarExpressionFunction
        //{

        //    public ExpressionTimeSpan(Host Host)
        //        : base(Host, null, NAME_TIME_SPAN, 1, CellAffinity.CSTRING)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionTimeSpan(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        TimeSpan ts = new TimeSpan(this._ChildNodes[0].Evaluate(Variants).valueLONG);

        //        return new Cell(ts.ToString());

        //    }

        //}

        //// Strings //
        //public class ExpressionSubstring : ScalarExpressionFunction
        //{

        //    public ExpressionSubstring(Host Host)
        //        : base(Host, null, NAME_SUBSTR, 3)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionSubstring(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell value = this._ChildNodes[0].Evaluate(Variants);
        //        Cell start = this._ChildNodes[1].Evaluate(Variants);
        //        Cell length = this._ChildNodes[2].Evaluate(Variants);

        //        return CellFunctions.Substring(value, start.valueLONG, length.valueLONG);

        //    }

        //}

        //public class ExpressionReplace : ScalarExpressionFunction
        //{

        //    public ExpressionReplace(Host Host)
        //        : base(Host, null, NAME_REPLACE, 3)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionReplace(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != this._MaxParamterCount)
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell value = this._ChildNodes[0].Evaluate(Variants);
        //        Cell pattern = this._ChildNodes[1].Evaluate(Variants);
        //        Cell new_pattern = this._ChildNodes[2].Evaluate(Variants);

        //        return CellFunctions.Replace(value, pattern, new_pattern);

        //    }

        //}

        //public class ExpressionPosition : ScalarExpressionFunction
        //{

        //    public ExpressionPosition(Host Host)
        //        : base(Host, null, NAME_POSITION, -2, CellAffinity.LONG)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionPosition(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count < Math.Abs(this._MaxParamterCount))
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell value = this._ChildNodes[0].Evaluate(Variants);
        //        Cell pattern = this._ChildNodes[1].Evaluate(Variants);
        //        int start = (this._ChildNodes.Count >= 3 ? (int)this._ChildNodes[2].Evaluate(Variants).valueLONG : 0);

        //        return CellFunctions.Position(value, pattern, start);

        //    }

        //}

        //public class ExpressionLength : ScalarExpressionFunction
        //{

        //    public ExpressionLength(Host Host)
        //        : base(Host, null, NAME_LENGTH, 1, CellAffinity.INT)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionLength(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != Math.Abs(this._MaxParamterCount))
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell x = this._ChildNodes[0].Evaluate(Variants);
        //        if (x.IsNull)
        //            return CellValues.NullINT;

        //        return new Cell(CellSerializer.Length(x));

        //    }

        //}

        //public class ExpressionTrim : ScalarExpressionFunction
        //{

        //    public ExpressionTrim(Host Host)
        //        : base(Host, null, NAME_TRIM, 1)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionTrim(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != Math.Abs(this._MaxParamterCount))
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell x = this._ChildNodes[0].Evaluate(Variants);
        //        if (x.IsNull)
        //            return new Cell(this.ReturnAffinity());

        //        return CellFunctions.Trim(x);

        //    }

        //}

        //public class ExpressionToUTF16 : ScalarExpressionFunction
        //{

        //    public ExpressionToUTF16(Host Host)
        //        : base(Host, null, NAME_TO_UTF16, 1, CellAffinity.CSTRING)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionToUTF16(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != Math.Abs(this._MaxParamterCount))
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell x = this._ChildNodes[0].Evaluate(Variants);

        //        if (x.AFFINITY != CellAffinity.BINARY)
        //            return CellValues.NullCSTRING;

        //        return new Cell(CellFunctions.ByteArrayToUTF16String(x.valueBINARY));

        //    }

        //    public override int ReturnSize()
        //    {
        //        return base.ReturnSize() * 2;
        //    }

        //}

        //public class ExpressionToUTF8 : ScalarExpressionFunction
        //{

        //    public ExpressionToUTF8(Host Host)
        //        : base(Host, null, NAME_TO_UTF8, 1, CellAffinity.CSTRING)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionToUTF8(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != Math.Abs(this._MaxParamterCount))
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell x = this._ChildNodes[0].Evaluate(Variants);

        //        if (x.AFFINITY != CellAffinity.BINARY)
        //            return CellValues.NullCSTRING;

        //        return new Cell(ASCIIEncoding.ASCII.GetString(x.valueBINARY));

        //    }

        //}

        //public class ExpressionToHex : ScalarExpressionFunction
        //{

        //    public ExpressionToHex(Host Host)
        //        : base(Host, null, NAME_TO_HEX, 1, CellAffinity.CSTRING)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionToHex(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != Math.Abs(this._MaxParamterCount))
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell x = this._ChildNodes[0].Evaluate(Variants);

        //        if (x.AFFINITY != CellAffinity.BINARY)
        //            return CellValues.NullCSTRING;

        //        return new Cell(BitConverter.ToString(x.valueBINARY));

        //    }

        //    public override int ReturnSize()
        //    {
        //        return base.ReturnSize() * 4;
        //    }

        //}

        //public class ExpressionFromUTF16 : ScalarExpressionFunction
        //{

        //    public ExpressionFromUTF16(Host Host)
        //        : base(Host, null, NAME_FROM_UTF16, 1, CellAffinity.BINARY)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionFromUTF16(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != Math.Abs(this._MaxParamterCount))
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell x = this._ChildNodes[0].Evaluate(Variants);

        //        if (x.AFFINITY != CellAffinity.CSTRING || x.NULL == 1)
        //            return CellValues.NullBLOB;

        //        x.BINARY = ASCIIEncoding.BigEndianUnicode.GetBytes(x.CSTRING);
        //        x.AFFINITY = CellAffinity.BINARY;

        //        return x;

        //    }

        //    public override int ReturnSize()
        //    {
        //        return base.ReturnSize();
        //    }

        //}

        //public class ExpressionFromUTF8 : ScalarExpressionFunction
        //{

        //    public ExpressionFromUTF8(Host Host)
        //        : base(Host, null, NAME_FROM_UTF8, 1, CellAffinity.BINARY)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionFromUTF8(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != Math.Abs(this._MaxParamterCount))
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell x = this._ChildNodes[0].Evaluate(Variants);

        //        if (x.AFFINITY != CellAffinity.CSTRING || x.NULL == 1)
        //            return CellValues.NullBLOB;

        //        x.BINARY = ASCIIEncoding.ASCII.GetBytes(x.CSTRING.ToCharArray());
        //        x.AFFINITY = CellAffinity.BINARY;

        //        return x;

        //    }

        //}

        //public class ExpressionFromHex : ScalarExpressionFunction
        //{

        //    public ExpressionFromHex(Host Host)
        //        : base(Host, null, NAME_FROM_HEX, 1, CellAffinity.BINARY)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionFromHex(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != Math.Abs(this._MaxParamterCount))
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell x = this._ChildNodes[0].Evaluate(Variants);

        //        if (x.AFFINITY != CellAffinity.CSTRING || x.NULL == 1)
        //            return CellValues.NullBLOB;

        //        return CellParser.ByteParse(x.CSTRING);

        //    }

        //    public override int ReturnSize()
        //    {
        //        return base.ReturnSize() / 4;
        //    }

        //}

        //// Numerical Functions //
        //public class ExpressionLog : ScalarExpressionFunction
        //{

        //    public ExpressionLog(Host Host)
        //        : base(Host, null, NAME_LOG, 1)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionLog(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != Math.Abs(this._MaxParamterCount))
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell x = this._ChildNodes[0].Evaluate(Variants);

        //        return CellFunctions.Log(x);

        //    }

        //}

        //public class ExpressionExp : ScalarExpressionFunction
        //{

        //    public ExpressionExp(Host Host)
        //        : base(Host, null, NAME_EXP, 1)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionExp(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != Math.Abs(this._MaxParamterCount))
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell x = this._ChildNodes[0].Evaluate(Variants);

        //        return CellFunctions.Exp(x);

        //    }

        //}

        //public class ExpressionSine : ScalarExpressionFunction
        //{

        //    public ExpressionSine(Host Host)
        //        : base(Host, null, NAME_SIN, 1)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionSine(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != Math.Abs(this._MaxParamterCount))
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell x = this._ChildNodes[0].Evaluate(Variants);

        //        return CellFunctions.Sin(x);

        //    }

        //}

        //public class ExpressionCosine : ScalarExpressionFunction
        //{

        //    public ExpressionCosine(Host Host)
        //        : base(Host, null, NAME_COS, 1)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionCosine(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != Math.Abs(this._MaxParamterCount))
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell x = this._ChildNodes[0].Evaluate(Variants);

        //        return CellFunctions.Cos(x);

        //    }

        //}

        //public class ExpressionTangent : ScalarExpressionFunction
        //{

        //    public ExpressionTangent(Host Host)
        //        : base(Host, null, NAME_TAN, 1)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionTangent(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != Math.Abs(this._MaxParamterCount))
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell x = this._ChildNodes[0].Evaluate(Variants);

        //        return CellFunctions.Tan(x);

        //    }

        //}

        //public class ExpressionHyperbolicSine : ScalarExpressionFunction
        //{

        //    public ExpressionHyperbolicSine(Host Host)
        //        : base(Host, null, NAME_SINH, 1)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionHyperbolicSine(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != Math.Abs(this._MaxParamterCount))
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell x = this._ChildNodes[0].Evaluate(Variants);

        //        return CellFunctions.Sinh(x);

        //    }

        //}

        //public class ExpressionHyperbolicCosine : ScalarExpressionFunction
        //{

        //    public ExpressionHyperbolicCosine(Host Host)
        //        : base(Host, null, NAME_COSH, 1)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionHyperbolicCosine(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != Math.Abs(this._MaxParamterCount))
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell x = this._ChildNodes[0].Evaluate(Variants);

        //        return CellFunctions.Cosh(x);

        //    }

        //}

        //public class ExpressionHyperbolicTangent : ScalarExpressionFunction
        //{

        //    public ExpressionHyperbolicTangent(Host Host)
        //        : base(Host, null, NAME_TANH, 1)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionHyperbolicTangent(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != Math.Abs(this._MaxParamterCount))
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell x = this._ChildNodes[0].Evaluate(Variants);

        //        return CellFunctions.Tanh(x);

        //    }

        //}

        //public class ExpressionAbsoluteValue : ScalarExpressionFunction
        //{

        //    public ExpressionAbsoluteValue(Host Host)
        //        : base(Host, null, NAME_ABS, 1)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionAbsoluteValue(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != Math.Abs(this._MaxParamterCount))
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell x = this._ChildNodes[0].Evaluate(Variants);

        //        return CellFunctions.Abs(x);

        //    }

        //}

        //public class ExpressionSquareRoot : ScalarExpressionFunction
        //{

        //    public ExpressionSquareRoot(Host Host)
        //        : base(Host, null, NAME_SQRT, 1)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionSquareRoot(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != Math.Abs(this._MaxParamterCount))
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell x = this._ChildNodes[0].Evaluate(Variants);

        //        return CellFunctions.Sqrt(x);

        //    }

        //}

        //public class ExpressionSign : ScalarExpressionFunction
        //{

        //    public ExpressionSign(Host Host)
        //        : base(Host, null, NAME_SIGN, 1)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionSign(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != Math.Abs(this._MaxParamterCount))
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell x = this._ChildNodes[0].Evaluate(Variants);

        //        return CellFunctions.Sign(x);

        //    }

        //}

        //public class ExpressionRound : ScalarExpressionFunction
        //{

        //    public ExpressionRound(Host Host)
        //        : base(Host, null, NAME_ROUND, 2)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionRound(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != Math.Abs(this._MaxParamterCount))
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        Cell x = this._ChildNodes[0].Evaluate(Variants);
        //        if (x.AFFINITY != CellAffinity.DOUBLE)
        //            return new Cell(this.ReturnAffinity());

        //        Cell y = this._ChildNodes[0].Evaluate(Variants);

        //        return new Cell(Math.Round(x.DOUBLE, (int)y.valueLONG));

        //    }

        //}

        //public class ExpressionLogit : ScalarExpressionFunction
        //{

        //    public ExpressionLogit(Host Host)
        //        : base(Host, null, NAME_LOGIT, 1)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionLogit(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != Math.Abs(this._MaxParamterCount))
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        throw new NotImplementedException();

        //    }

        //}

        //public class ExpressionNormal : ScalarExpressionFunction
        //{

        //    public ExpressionNormal(Host Host)
        //        : base(Host, null, NAME_NORMAL, 1)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionNormal(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {

        //        if (this._ChildNodes.Count != Math.Abs(this._MaxParamterCount))
        //            throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

        //        throw new NotImplementedException();

        //    }

        //}

        //// Meta Functions //
        //public class ExpressionMeta_Formula : ScalarExpressionFunction
        //{

        //    public ExpressionMeta_Formula(Host Host)
        //        : base(Host, null, NAME_FORMULA, 1)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionMeta_Formula(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {
        //        return new Cell(this._ChildNodes[0].Unparse(Variants));
        //    }

        //}

        //public class ExpressionTypeOf : ScalarExpressionFunction
        //{

        //    public ExpressionTypeOf(Host Host)
        //        : base(Host, null, NAME_TYPEOF, 1, CellAffinity.BYTE)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionTypeOf(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {
        //        return new Cell((byte)this._ChildNodes[0].ReturnAffinity());
        //    }


        //}

        //public class ExpressionSizeOf : ScalarExpressionFunction
        //{

        //    public ExpressionSizeOf(Host Host)
        //        : base(Host, null, NAME_SIZEOF, 1, CellAffinity.INT)
        //    {
        //    }

        //    public override ScalarExpression CloneOfMe()
        //    {
        //        return new ExpressionSizeOf(this._Host);
        //    }

        //    public override Cell Evaluate(FieldResolver Variants)
        //    {
        //        return new Cell(CellSerializer.Length(this._ChildNodes[0].Evaluate(Variants)));
        //    }


        //}

        ////---------------------------------------------------------------------------------------------
        ////---------------------------------------------------------------------------------------------
        ////---------------------------------------------------------------------------------------------

        //public class BaseLibraryFunctions : IScalarExpressionLookup
        //{

        //    private Host _Host;

        //    public BaseLibraryFunctions(Host Host)
        //    {
        //        this._Host = Host;
        //    }

        //    public ScalarExpressionFunction Lookup(string Name)
        //    {

        //        switch (Name.ToUpper())
        //        {
        //            case NAME_NOT: return new ExpressionNot(this._Host);
        //            case NAME_PLUS: return new ExpressionPlus(this._Host);
        //            case NAME_MINUS: return new ExpressionMinus(this._Host);
        //            case NAME_ADD: return new ExpressionAdd(this._Host);
        //            case NAME_SUB: return new ExpressionSubtract(this._Host);
        //            case NAME_MULT: return new ExpressionMultiply(this._Host);
        //            case NAME_DIV: return new ExpressionDivide(this._Host);
        //            case NAME_CDIV: return new ExpressionCheckedDivide(this._Host);
        //            case NAME_MOD: return new ExpressionModulo(this._Host);
        //            case NAME_POWER: return new ExpressionPower(this._Host);
        //            case NAME_EQ: return new ExpressionEquals(this._Host);
        //            case NAME_NEQ: return new ExpressionNotEquals(this._Host);
        //            case NAME_GT: return new ExpressionGreaterThan(this._Host);
        //            case NAME_GTE: return new ExpressionGreaterThanOrEqualTo(this._Host);
        //            case NAME_LT: return new ExpressionLessThan(this._Host);
        //            case NAME_LTE: return new ExpressionLessThanOrEqualTo(this._Host);
        //            case NAME_AND: return new ExpressionAnd(this._Host);
        //            case NAME_OR: return new ExpressionOr(this._Host);
        //            case NAME_XOR: return new ExpressionXor(this._Host);
        //            case NAME_IF: return new ExpressionIf(this._Host);
        //            case NAME_IFNULL: return new ExpressionIfNull(this._Host);
        //            //case NAME_CAST: return new ExpressionCast(this._Host); // Note: CAST uses the C# syntax and is handeled in the parser not as a function
        //            case NAME_LIKE: return new ExpressionLike(this._Host);
        //            case NAME_MATCH: return new ExpressionMatch(this._Host);
        //            case NAME_GUID: return new ExpressionGUID(this._Host);
        //            case NAME_THREAD_ID: return new ExpressionThreadID(this._Host);
        //            case NAME_DATE_BUILD: return new ExpressionDateBuild(this._Host);
        //            case NAME_NOW: return new ExpressionNow(this._Host);
        //            case NAME_YEAR: return new ExpressionYear(this._Host);
        //            case NAME_MONTH: return new ExpressionMonth(this._Host);
        //            case NAME_DAY: return new ExpressionDay(this._Host);
        //            case NAME_HOUR: return new ExpressionHour(this._Host);
        //            case NAME_MINUTE: return new ExpressionMinute(this._Host);
        //            case NAME_SECOND: return new ExpressionSecond(this._Host);
        //            case NAME_MILISECOND: return new ExpressionMillisecond(this._Host);
        //            case NAME_ELAPSED: return new ExpressionElapsed(this._Host);
        //            case NAME_TIME_SPAN: return new ExpressionTimeSpan(this._Host);
        //            case NAME_SUBSTR: return new ExpressionSubstring(this._Host);
        //            case NAME_REPLACE: return new ExpressionReplace(this._Host);
        //            case NAME_POSITION: return new ExpressionPosition(this._Host);
        //            case NAME_LENGTH: return new ExpressionLength(this._Host);
        //            case NAME_TRIM: return new ExpressionTrim(this._Host);
        //            case NAME_TO_UTF16: return new ExpressionToUTF16(this._Host);
        //            case NAME_TO_UTF8: return new ExpressionToUTF8(this._Host);
        //            case NAME_TO_HEX: return new ExpressionToHex(this._Host);
        //            case NAME_FROM_UTF16: return new ExpressionFromUTF16(this._Host);
        //            case NAME_FROM_UTF8: return new ExpressionFromUTF8(this._Host);
        //            case NAME_FROM_HEX: return new ExpressionFromHex(this._Host);
        //            case NAME_LOG: return new ExpressionLog(this._Host);
        //            case NAME_EXP: return new ExpressionExp(this._Host);
        //            case NAME_SQRT: return new ExpressionSquareRoot(this._Host);
        //            case NAME_SIN: return new ExpressionSine(this._Host);
        //            case NAME_COS: return new ExpressionCosine(this._Host);
        //            case NAME_TAN: return new ExpressionTangent(this._Host);
        //            case NAME_SINH: return new ExpressionHyperbolicSine(this._Host);
        //            case NAME_COSH: return new ExpressionHyperbolicCosine(this._Host);
        //            case NAME_TANH: return new ExpressionHyperbolicTangent(this._Host);
        //            case NAME_LOGIT: return new ExpressionLogit(this._Host);
        //            case NAME_NORMAL: return new ExpressionNormal(this._Host);
        //            case NAME_ABS: return new ExpressionAbsoluteValue(this._Host);
        //            case NAME_SIGN: return new ExpressionSign(this._Host);
        //            case NAME_ROUND: return new ExpressionRound(this._Host);
        //            case NAME_FORMULA: return new ExpressionMeta_Formula(this._Host);
        //            case NAME_TYPEOF: return new ExpressionTypeOf(this._Host);
        //            case NAME_SIZEOF: return new ExpressionSizeOf(this._Host);

        //        }

        //        return null;

        //    }

        //    public bool Exists(string Name)
        //    {
        //        return this.Lookup(Name) != null;
        //    }

        //}

    }

    public sealed class ScalarExpressionFunctionScripted : ScalarExpressionFunction
    {

        private TypedAbstractMethod _Method;

        public ScalarExpressionFunctionScripted(Host Host, ScalarExpression Parent, TypedAbstractMethod Method)
            : base(Host, Parent, Method.Name, Method.ParameterCount)
        {
            this._Method = Method;
        }

        public override CellAffinity ReturnAffinity()
        {
            return this._Method.Affinity;
        }

        public override int ReturnSize()
        {
            return this._Method.Size;
        }

        public override bool IsVolatile
        {
            get
            {
                return true;
            }
        }

        public override void AddParameter(Parameter Value)
        {
            base.AddParameter(Value);
            this._Method.AddParameter(Value);
        }

        public override Cell Evaluate(SpoolSpace Variants)
        {
            SpoolSpace s = this._Method.MemorySpace(Variants);
            this._Method.BindParameters(s);
            this._Method.Invoke(s);
            return this._Method.Memory.GetScalar(TypedAbstractMethod.RETURN_NAME);
        }

    }

}
