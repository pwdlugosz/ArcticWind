using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Tables;
using ArcticWind.Expressions;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Expressions.TableExpressions;

namespace ArcticWind.Expressions.ScalarExpressions
{

    /// <summary>
    /// Represents the base class for all expressions
    /// </summary>
    public abstract class ScalarExpression 
    {

        protected ScalarExpressionAffinity _Affinity;
        protected ScalarExpression _ParentNode;
        protected Guid _UID;
        protected string _name;

        public ScalarExpression(ScalarExpression Parent, ScalarExpressionAffinity Affinity)
        {
            this._Affinity = Affinity;
            this._name = Affinity.ToString();
            this._ParentNode = Parent;
            this._UID = Guid.NewGuid();
        }

        /// <summary>
        /// Gets the return affinity
        /// </summary>
        public ScalarExpressionAffinity Affinity
        {
            get { return _Affinity; }
        }

        /// <summary>
        /// The parent node in the expression tree
        /// </summary>
        public ScalarExpression ParentNode
        {
            get { return _ParentNode; }
            set { this._ParentNode = value; }
        }

        /// <summary>
        /// True if this node has no parent (top of the chain)
        /// </summary>
        public bool IsMaster
        {
            get { return _ParentNode == null; }
        }

        /// <summary>
        /// Gets a unique node ID for debugging
        /// </summary>
        public Guid NodeID
        {
            get { return this._UID; }
        }

        /// <summary>
        /// Gets an optional name parameter for debugging
        /// </summary>
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        // Abstracts //
        /// <summary>
        /// Gets the return affinity
        /// </summary>
        /// <param name="Variants"></param>
        /// <returns></returns>
        public abstract CellAffinity ReturnAffinity();

        /// <summary>
        /// Gets the return size
        /// </summary>
        /// <param name="Variants"></param>
        /// <returns></returns>
        public abstract int ReturnSize();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ScalarExpression CloneOfMe()
        {
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Variants"></param>
        /// <returns></returns>
        public abstract Cell Evaluate(SpoolSpace Variants);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual string Unparse(SpoolSpace Variants)
        {
            return "NULL";
        }

        // Virtuals //
        /// <summary>
        /// True if the expression returns different values with two different calls, all else the same
        /// </summary>
        public virtual bool IsVolatile
        {
            get { return false; }
        }

        /// <summary>
        /// Returns a field name to use as an alias; this is NOT gauranteed to be unique
        /// </summary>
        /// <returns></returns>
        public virtual string BuildAlias()
        {
            return "X" + Host.Tocks().ToString();
        }

        // Pointer Support //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        public virtual void Bind(string Name, Parameter Value)
        {
            // Do Nothing
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Names"></param>
        public virtual void AppendPointerName(List<string> Names)
        {
        }

        // Opperators //
        public static ScalarExpression operator -(ScalarExpression A)
        {
            return new ScalarExpressionUnary.SclarExpressionMinus(A);
        }

        public static ScalarExpression operator +(ScalarExpression A)
        {
            return new ScalarExpressionUnary.SclarExpressionPlus(A);
        }

        public static ScalarExpression operator !(ScalarExpression A)
        {
            return new ScalarExpressionUnary.SclarExpressionNot(A);
        }

        public static ScalarExpression operator +(ScalarExpression A, ScalarExpression B)
        {
            return new ScalarExpressionBinary.ScalarExpressionAdd(A, B);
        }

        public static ScalarExpression operator -(ScalarExpression A, ScalarExpression B)
        {
            return new ScalarExpressionBinary.ScalarExpressionSubtract(A, B);
        }

        public static ScalarExpression operator *(ScalarExpression A, ScalarExpression B)
        {
            return new ScalarExpressionBinary.ScalarExpressionMultiply(A, B);
        }

        public static ScalarExpression operator /(ScalarExpression A, ScalarExpression B)
        {
            return new ScalarExpressionBinary.ScalarExpressionDivide(A, B);
        }

        public static ScalarExpression operator %(ScalarExpression A, ScalarExpression B)
        {
            return new ScalarExpressionBinary.ScalarExpressionMod(A, B);
        }

        public static ScalarExpression CDIV(ScalarExpression A, ScalarExpression B)
        {
            return new ScalarExpressionBinary.ScalarExpressionCheckDivide(A, B);
        }

        public static ScalarExpression EQ(ScalarExpression A, ScalarExpression B)
        {
            return new ScalarExpressionLogical.ScalarExpressionEquals(A, B);
        }

        public static ScalarExpression NEQ(ScalarExpression A, ScalarExpression B)
        {
            return new ScalarExpressionLogical.ScalarExpressionNotEquals(A, B);
        }

        public static ScalarExpression GT(ScalarExpression A, ScalarExpression B)
        {
            return new ScalarExpressionLogical.ScalarExpressionGreaterThan(A, B);
        }

        public static ScalarExpression GTE(ScalarExpression A, ScalarExpression B)
        {
            return new ScalarExpressionLogical.ScalarExpressionGreaterThanOrEquals(A, B);
        }

        public static ScalarExpression LT(ScalarExpression A, ScalarExpression B)
        {
            return new ScalarExpressionLogical.ScalarExpressionLessThan(A, B);
        }

        public static ScalarExpression LTE(ScalarExpression A, ScalarExpression B)
        {
            return new ScalarExpressionLogical.ScalarExpressionLessThanOrEquals(A, B);
        }

        public static ScalarExpression AND(ScalarExpression A, ScalarExpression B)
        {
            return new ScalarExpressionBinary.ScalarExpressionAnd(A, B);
        }

        public static ScalarExpression OR(ScalarExpression A, ScalarExpression B)
        {
            return new ScalarExpressionBinary.ScalarExpressionOr(A, B);
        }

        public static ScalarExpression XOR(ScalarExpression A, ScalarExpression B)
        {
            return new ScalarExpressionBinary.ScalarExpressionXor(A, B);
        }

        public static ScalarExpression IF(ScalarExpression Predicate, ScalarExpression TrueValue, ScalarExpression FalseValue)
        {
            return new ScalarExpression.SclarExpressionIf(Predicate, TrueValue, FalseValue);
        }

        public static ScalarExpression IFNULL(ScalarExpression CheckValue, ScalarExpression IfNullValue)
        {
            return new ScalarExpression.SclarExpressionIfNull(CheckValue, IfNullValue);
        }

        public static ScalarExpression CAST(ScalarExpression Value, CellAffinity Type)
        {
            return new ScalarExpression.SclarExpressionCast(Value, Type);
        }
        
        public static ScalarExpression Value(bool Value)
        {
            return new ScalarExpressionConstant(null, new Cell(Value));
        }

        public static ScalarExpression Value(DateTime Value)
        {
            return new ScalarExpressionConstant(null, new Cell(Value));
        }

        public static ScalarExpression Value(byte Value)
        {
            return new ScalarExpressionConstant(null, new Cell(Value));
        }

        public static ScalarExpression Value(short Value)
        {
            return new ScalarExpressionConstant(null, new Cell(Value));
        }

        public static ScalarExpression Value(int Value)
        {
            return new ScalarExpressionConstant(null, new Cell(Value));
        }

        public static ScalarExpression Value(long Value)
        {
            return new ScalarExpressionConstant(null, new Cell(Value));
        }

        public static ScalarExpression Value(float Value)
        {
            return new ScalarExpressionConstant(null, new Cell(Value));
        }

        public static ScalarExpression Value(double Value)
        {
            return new ScalarExpressionConstant(null, new Cell(Value));
        }

        public static ScalarExpression Value(byte[] Value)
        {
            return new ScalarExpressionConstant(null, new Cell(Value));
        }

        public static ScalarExpression Value(string Value)
        {
            return new ScalarExpressionConstant(null, new Cell(Value));
        }

        public static ScalarExpression Value(Cell Value)
        {
            return new ScalarExpressionConstant(null, Value);
        }

        public static ScalarExpression Value(CellAffinity Affinity)
        {
            return new ScalarExpressionConstant(null, new Cell(Affinity));
        }

        // Constants //
        public static ScalarExpression True
        {
            get { return new ScalarExpressionConstant(null, new Cell(true)); }
        }

        public static ScalarExpression False
        {
            get { return new ScalarExpressionConstant(null, new Cell(false)); }
        }

        public static ScalarExpression ZeroNUM
        {
            get { return new ScalarExpressionConstant(null, new Cell(0D)); }
        }

        public static ScalarExpression OneNUM
        {
            get { return new ScalarExpressionConstant(null, new Cell(1D)); }
        }

        public static ScalarExpression ZeroINT
        {
            get { return new ScalarExpressionConstant(null, new Cell(0)); }
        }

        public static ScalarExpression OneINT
        {
            get { return new ScalarExpressionConstant(null, new Cell(1)); }
        }

        public static ScalarExpression EmptyString
        {
            get { return new ScalarExpressionConstant(null, new Cell("")); }
        }

        public static ScalarExpression NullBool
        {
            get { return new ScalarExpressionConstant(null, CellValues.NullBOOL); }
        }

        public static ScalarExpression NullInt
        {
            get { return new ScalarExpressionConstant(null, CellValues.NullLONG); }
        }

        public static ScalarExpression NullNum
        {
            get { return new ScalarExpressionConstant(null, CellValues.NullDOUBLE); }
        }

        public static ScalarExpression NullDate
        {
            get { return new ScalarExpressionConstant(null, CellValues.NullDATE); }
        }

        public static ScalarExpression NullString
        {
            get { return new ScalarExpressionConstant(null, CellValues.NullCSTRING); }
        }

        public static ScalarExpression NullBLOB
        {
            get { return new ScalarExpressionConstant(null, CellValues.NullBLOB); }
        }

        public sealed class SclarExpressionCast : ScalarExpression
        {

            private CellAffinity _Type;
            private ScalarExpression _Value;

            public SclarExpressionCast(ScalarExpression Value, CellAffinity Type)
                :base(null, ScalarExpressionAffinity.Function)
            {
                this._Type = Type;
                this._Value = Value;
            }

            public override CellAffinity ReturnAffinity()
            {
                return this._Type;
            }

            public override int ReturnSize()
            {
                return CellConverter.CastSizeHelper(this._Value.ReturnAffinity(), this._Type, this._Value.ReturnSize());
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                return CellConverter.Cast(this._Value.Evaluate(Variants), this._Type);
            }

            public override void Bind(string Name, Parameter Value)
            {

                if (Value.Affinity != ParameterAffinity.Scalar)
                    throw new Exception("Parameter must be a scalar");

                if (this._Value.Affinity == ScalarExpressionAffinity.Pointer && StringComparer.OrdinalIgnoreCase.Compare(Name, this._Value.Name) == 0)
                    this._Value = Value.Scalar;
                else
                    this._Value.Bind(Name, Value);

            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpression.SclarExpressionCast(this._Value.CloneOfMe(), this._Type);
            }

            public override void AppendPointerName(List<string> Names)
            {
                if (this._Value.Affinity == ScalarExpressionAffinity.Pointer)
                {
                    
                }
            }

        }

        public sealed class SclarExpressionIf : ScalarExpression
        {

            private ScalarExpression _Predicate;
            private ScalarExpression _ValueIfTrue;
            private ScalarExpression _ValueIfFalse;

            public SclarExpressionIf(ScalarExpression Predicate, ScalarExpression ValueIfTrue, ScalarExpression ValueIfFalse)
                : base(null, ScalarExpressionAffinity.Function)
            {
                this._Predicate = Predicate;
                this._ValueIfTrue = ValueIfTrue;
                this._ValueIfFalse = (ValueIfFalse ?? new ScalarExpressionConstant(null, new Cell(ValueIfTrue.ReturnAffinity())));
            }

            public override CellAffinity ReturnAffinity()
            {
                CellAffinity vt = this._ValueIfTrue.ReturnAffinity();
                CellAffinity vf = this._ValueIfFalse.ReturnAffinity();
                if (vt >= vf)
                    return vt;
                return vf;
            }

            public override int ReturnSize()
            {
                CellAffinity vt = this._ValueIfTrue.ReturnAffinity();
                CellAffinity vf = this._ValueIfFalse.ReturnAffinity();
                if (vt >= vf)
                    return this._ValueIfTrue.ReturnSize();
                return this._ValueIfFalse.ReturnSize();
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this._Predicate.Evaluate(Variants).valueBOOL == true)
                    return this._ValueIfTrue.Evaluate(Variants);
                else
                    return this._ValueIfFalse.Evaluate(Variants);
            }

            public override void Bind(string Name, Parameter Value)
            {

                if (Value.Affinity != ParameterAffinity.Scalar)
                    throw new Exception("Parameter must be a scalar");

                if (this._Predicate.Affinity == ScalarExpressionAffinity.Pointer && StringComparer.OrdinalIgnoreCase.Compare(Name, this._Predicate.Name) == 0)
                    this._Predicate = Value.Scalar;
                else
                    this._Predicate.Bind(Name, Value);

                if (this._ValueIfTrue.Affinity == ScalarExpressionAffinity.Pointer && StringComparer.OrdinalIgnoreCase.Compare(Name, this._ValueIfTrue.Name) == 0)
                    this._ValueIfTrue = Value.Scalar;
                else
                    this._ValueIfTrue.Bind(Name, Value);

                if (this._ValueIfFalse.Affinity == ScalarExpressionAffinity.Pointer && StringComparer.OrdinalIgnoreCase.Compare(Name, this._ValueIfFalse.Name) == 0)
                    this._ValueIfFalse = Value.Scalar;
                else
                    this._ValueIfFalse.Bind(Name, Value);

            }

            public override ScalarExpression CloneOfMe()
            {
                return new SclarExpressionIf(this._Predicate.CloneOfMe(), this._ValueIfTrue.CloneOfMe(), this._ValueIfFalse.CloneOfMe());
            }

        }

        public sealed class SclarExpressionIfNull : ScalarExpression
        {

            private ScalarExpression _TestValue;
            private ScalarExpression _ValueIfNull;

            public SclarExpressionIfNull(ScalarExpression TestValue, ScalarExpression ValueIfNull)
                : base(null, ScalarExpressionAffinity.Function)
            {
                this._TestValue = TestValue;
                this._ValueIfNull = ValueIfNull;
            }

            public override CellAffinity ReturnAffinity()
            {
                CellAffinity vt = this._TestValue.ReturnAffinity();
                CellAffinity vf = this._ValueIfNull.ReturnAffinity();
                if (vt >= vf)
                    return vt;
                return vf;
            }

            public override int ReturnSize()
            {
                CellAffinity vt = this._TestValue.ReturnAffinity();
                CellAffinity vf = this._ValueIfNull.ReturnAffinity();
                if (vt >= vf)
                    return this._TestValue.ReturnSize();
                return this._ValueIfNull.ReturnSize();
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                Cell Test = this._TestValue.Evaluate(Variants);
                Cell ValueIfNull = this._ValueIfNull.Evaluate(Variants);
                return (Test.IsNull ? ValueIfNull : Test);
            }

            public override void Bind(string Name, Parameter Value)
            {

                if (Value.Affinity != ParameterAffinity.Scalar)
                    throw new Exception("Parameter must be a scalar");

                if (this._TestValue.Affinity == ScalarExpressionAffinity.Pointer && StringComparer.OrdinalIgnoreCase.Compare(Name, this._TestValue.Name) == 0)
                    this._TestValue = Value.Scalar;
                else
                    this._TestValue.Bind(Name, Value);

                if (this._ValueIfNull.Affinity == ScalarExpressionAffinity.Pointer && StringComparer.OrdinalIgnoreCase.Compare(Name, this._ValueIfNull.Name) == 0)
                    this._ValueIfNull = Value.Scalar;
                else
                    this._ValueIfNull.Bind(Name, Value);

            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpression.SclarExpressionIfNull(this._TestValue.CloneOfMe(), this._ValueIfNull.CloneOfMe());
            }

        }

    
    }

    public abstract class ScalarExpressionUnary : ScalarExpression
    {

        private ScalarExpression _Value;

        public ScalarExpressionUnary(ScalarExpression Parent, string Symbol, string Name, ScalarExpression Value)
            : base(Parent, ScalarExpressionAffinity.Function)
        {
            this._Value = Value;
        }

        public override CellAffinity ReturnAffinity()
        {
            return this._Value.ReturnAffinity();
        }

        public override int ReturnSize()
        {
            return this._Value.ReturnSize();
        }

        public override void Bind(string Name, Parameter Value)
        {

            if (Value.Affinity != ParameterAffinity.Scalar)
                throw new Exception("Parameter must be a scalar");

            if (this._Value.Affinity == ScalarExpressionAffinity.Pointer && StringComparer.OrdinalIgnoreCase.Compare(Name, this._Value.Name) == 0)
                this._Value = Value.Scalar;
            else
                this._Value.Bind(Name, Value);

        }

        public sealed class SclarExpressionPlus : ScalarExpressionUnary
        {

            public SclarExpressionPlus(ScalarExpression Value)
                : base(null, "+", "PLUS", Value)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                return +this._Value.Evaluate(Variants);
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionUnary.SclarExpressionPlus(this._Value.CloneOfMe());
            }

        }

        public sealed class SclarExpressionMinus : ScalarExpressionUnary
        {

            public SclarExpressionMinus(ScalarExpression Value)
                : base(null, "-", "MINUS", Value)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                return -this._Value.Evaluate(Variants);
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionUnary.SclarExpressionMinus(this._Value.CloneOfMe());
            }

        }

        public sealed class SclarExpressionNot : ScalarExpressionUnary
        {

            public SclarExpressionNot(ScalarExpression Value)
                : base(null, "!", "NOT", Value)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                return !this._Value.Evaluate(Variants);
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionUnary.SclarExpressionNot(this._Value.CloneOfMe());
            }

        }

    }

    public abstract class ScalarExpressionBinary : ScalarExpression
    {

        private ScalarExpression _Left;
        private ScalarExpression _Right;
        private string _FuncName;
        private string _Symbol;

        public ScalarExpressionBinary(ScalarExpression Parent, string Symbol, string Name, ScalarExpression Left, ScalarExpression Right)
            : base(Parent, ScalarExpressionAffinity.Function)
        {
            this._Left = Left;
            this._Right = Right;
            this._FuncName = Name;
            this._Symbol = Symbol;
        }

        public override CellAffinity ReturnAffinity()
        {
            CellAffinity l = this._Left.ReturnAffinity();
            CellAffinity r = this._Right.ReturnAffinity();
            return (l > r ? l : r);
        }

        public override int ReturnSize()
        {
            CellAffinity l = this._Left.ReturnAffinity();
            CellAffinity r = this._Right.ReturnAffinity();
            return (l > r ? this._Left.ReturnSize() : this._Right.ReturnSize());
        }

        public override void Bind(string Name, Parameter Value)
        {

            if (Value.Affinity != ParameterAffinity.Scalar)
                throw new Exception("Parameter must be a scalar");

            if (this._Left.Affinity == ScalarExpressionAffinity.Pointer && StringComparer.OrdinalIgnoreCase.Compare(Name, this._Left.Name) == 0)
                this._Left = Value.Scalar;
            else
                this._Left.Bind(Name, Value);

            if (this._Right.Affinity == ScalarExpressionAffinity.Pointer && StringComparer.OrdinalIgnoreCase.Compare(Name, this._Right.Name) == 0)
                this._Right = Value.Scalar;
            else
                this._Right.Bind(Name, Value);

        }

        public sealed class ScalarExpressionAdd : ScalarExpressionBinary
        {

            public ScalarExpressionAdd(ScalarExpression Left, ScalarExpression Right)
                : base(null, "+", "ADD", Left, Right)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                return this._Left.Evaluate(Variants) + this._Right.Evaluate(Variants);
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionAdd(this._Left.CloneOfMe(), this._Right.CloneOfMe());
            }

        }

        public sealed class ScalarExpressionSubtract : ScalarExpressionBinary
        {

            public ScalarExpressionSubtract(ScalarExpression Left, ScalarExpression Right)
                : base(null, "-", "SUBTRACT", Left, Right)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                return this._Left.Evaluate(Variants) - this._Right.Evaluate(Variants);
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionSubtract(this._Left.CloneOfMe(), this._Right.CloneOfMe());
            }

        }

        public sealed class ScalarExpressionMultiply : ScalarExpressionBinary
        {

            public ScalarExpressionMultiply(ScalarExpression Left, ScalarExpression Right)
                : base(null, "*", "MULTIPLY", Left, Right)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                return this._Left.Evaluate(Variants) * this._Right.Evaluate(Variants);
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionMultiply(this._Left.CloneOfMe(), this._Right.CloneOfMe());
            }

        }

        public sealed class ScalarExpressionDivide : ScalarExpressionBinary
        {

            public ScalarExpressionDivide(ScalarExpression Left, ScalarExpression Right)
                : base(null, "/", "DIVIDE", Left, Right)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                return this._Left.Evaluate(Variants) / this._Right.Evaluate(Variants);
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionDivide(this._Left.CloneOfMe(), this._Right.CloneOfMe());
            }
        }

        public sealed class ScalarExpressionCheckDivide : ScalarExpressionBinary
        {

            public ScalarExpressionCheckDivide(ScalarExpression Left, ScalarExpression Right)
                : base(null, "/?", "CDIVIDE", Left, Right)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                return Cell.CheckDivide(this._Left.Evaluate(Variants), this._Right.Evaluate(Variants));
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionCheckDivide(this._Left.CloneOfMe(), this._Right.CloneOfMe());
            }

        }

        public sealed class ScalarExpressionMod : ScalarExpressionBinary
        {

            public ScalarExpressionMod(ScalarExpression Left, ScalarExpression Right)
                : base(null, "%", "MOD", Left, Right)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                return this._Left.Evaluate(Variants) % this._Right.Evaluate(Variants);
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionMod(this._Left.CloneOfMe(), this._Right.CloneOfMe());
            }

        }

        public sealed class ScalarExpressionPower : ScalarExpressionBinary
        {

            public ScalarExpressionPower(ScalarExpression Left, ScalarExpression Right)
                : base(null, "^", "POWER", Left, Right)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                return CellFunctions.Power(this._Left.Evaluate(Variants), this._Right.Evaluate(Variants));
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionPower(this._Left.CloneOfMe(), this._Right.CloneOfMe());
            }

        }

        public sealed class ScalarExpressionAnd : ScalarExpressionBinary
        {

            public ScalarExpressionAnd(ScalarExpression Left, ScalarExpression Right)
                : base(null, "&&", "And", Left, Right)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                return this._Left.Evaluate(Variants) & this._Right.Evaluate(Variants);
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionAnd(this._Left.CloneOfMe(), this._Right.CloneOfMe());
            }

        }

        public sealed class ScalarExpressionOr : ScalarExpressionBinary
        {

            public ScalarExpressionOr(ScalarExpression Left, ScalarExpression Right)
                : base(null, "||", "Or", Left, Right)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                return this._Left.Evaluate(Variants) | this._Right.Evaluate(Variants);
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionOr(this._Left.CloneOfMe(), this._Right.CloneOfMe());
            }

        }

        public sealed class ScalarExpressionXor : ScalarExpressionBinary
        {

            public ScalarExpressionXor(ScalarExpression Left, ScalarExpression Right)
                : base(null, "^^", "Xor", Left, Right)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                return this._Left.Evaluate(Variants) ^ this._Right.Evaluate(Variants);
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionXor(this._Left.CloneOfMe(), this._Right.CloneOfMe());
            }

        }

        public sealed class ScalarExpressionLeftShift : ScalarExpressionBinary
        {

            public ScalarExpressionLeftShift(ScalarExpression Left, ScalarExpression Right)
                : base(null, "<<", "LShift", Left, Right)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                Cell x = this._Left.Evaluate(Variants);
                Cell y = this._Right.Evaluate(Variants);
                if (x.IsNull || y.IsNull)
                {
                    x.NULL = 1;
                    return x;
                }
                return Cell.LeftShift(x, y.valueINT);
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionLeftShift(this._Left.CloneOfMe(), this._Right.CloneOfMe());
            }

        }

        public sealed class ScalarExpressionRightShift : ScalarExpressionBinary
        {

            public ScalarExpressionRightShift(ScalarExpression Left, ScalarExpression Right)
                : base(null, ">>", "RShift", Left, Right)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                Cell x = this._Left.Evaluate(Variants);
                Cell y = this._Right.Evaluate(Variants);
                if (x.IsNull || y.IsNull)
                {
                    x.NULL = 1;
                    return x;
                }
                return Cell.RightShift(x, y.valueINT);
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionRightShift(this._Left.CloneOfMe(), this._Right.CloneOfMe());
            }

        }

        public sealed class ScalarExpressionLeftRotate : ScalarExpressionBinary
        {

            public ScalarExpressionLeftRotate(ScalarExpression Left, ScalarExpression Right)
                : base(null, "<<<", "LRotate", Left, Right)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                Cell x = this._Left.Evaluate(Variants);
                Cell y = this._Right.Evaluate(Variants);
                if (x.IsNull || y.IsNull)
                {
                    x.NULL = 1;
                    return x;
                }
                return Cell.LeftRotate(x, y.valueINT);
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionLeftRotate(this._Left.CloneOfMe(), this._Right.CloneOfMe());
            }

        }

        public sealed class ScalarExpressionRightRotate : ScalarExpressionBinary
        {

            public ScalarExpressionRightRotate(ScalarExpression Left, ScalarExpression Right)
                : base(null, ">>>", "RRotate", Left, Right)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                Cell x = this._Left.Evaluate(Variants);
                Cell y = this._Right.Evaluate(Variants);
                if (x.IsNull || y.IsNull)
                {
                    x.NULL = 1;
                    return x;
                }
                return Cell.RightRotate(x, y.valueINT);
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionLeftRotate(this._Left.CloneOfMe(), this._Right.CloneOfMe());
            }

        }

    }

    public abstract class ScalarExpressionLogical : ScalarExpression
    {

        private ScalarExpression _Left;
        private ScalarExpression _Right;

        public ScalarExpressionLogical(ScalarExpression Parent, string Symbol, string Name, ScalarExpression Left, ScalarExpression Right)
            : base(Parent, ScalarExpressionAffinity.Function)
        {
            this._Left = Left;
            this._Right = Right;
        }

        public override CellAffinity ReturnAffinity()
        {
            return CellAffinity.BOOL;
        }

        public override int ReturnSize()
        {
            return CellSerializer.BOOL_SIZE;
        }
        
        public override void Bind(string Name, Parameter Value)
        {

            if (Value.Affinity != ParameterAffinity.Scalar)
                throw new Exception("Parameter must be a scalar");

            if (this._Left.Affinity == ScalarExpressionAffinity.Pointer && StringComparer.OrdinalIgnoreCase.Compare(Name, this._Left.Name) == 0)
                this._Left = Value.Scalar;
            else
                this._Left.Bind(Name, Value);

            if (this._Right.Affinity == ScalarExpressionAffinity.Pointer && StringComparer.OrdinalIgnoreCase.Compare(Name, this._Right.Name) == 0)
                this._Right = Value.Scalar;
            else
                this._Right.Bind(Name, Value);

        }

        public sealed class ScalarExpressionEquals : ScalarExpressionLogical
        {

            public ScalarExpressionEquals(ScalarExpression Left, ScalarExpression Right)
                : base(null, "==", "EQUALS", Left, Right)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                return (this._Left.Evaluate(Variants) == this._Right.Evaluate(Variants) ? CellValues.True : CellValues.False);
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionEquals(this._Left.CloneOfMe(), this._Right.CloneOfMe());
            }

        }

        public sealed class ScalarExpressionStrictEquals : ScalarExpressionLogical
        {

            public ScalarExpressionStrictEquals(ScalarExpression Left, ScalarExpression Right)
                : base(null, "===", "STRICTEQUALS", Left, Right)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                Cell l = this._Left.Evaluate(Variants);
                Cell r = this._Right.Evaluate(Variants);
                return (l == r && l.Affinity == r.AFFINITY ? CellValues.True : CellValues.False);
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionStrictEquals(this._Left.CloneOfMe(), this._Right.CloneOfMe());
            }

        }

        public sealed class ScalarExpressionNotEquals : ScalarExpressionLogical
        {

            public ScalarExpressionNotEquals(ScalarExpression Left, ScalarExpression Right)
                : base(null, "!=", "NOTEQUALS", Left, Right)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                return (this._Left.Evaluate(Variants) != this._Right.Evaluate(Variants) ? CellValues.True : CellValues.False);
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionNotEquals(this._Left.CloneOfMe(), this._Right.CloneOfMe());
            }

        }

        public sealed class ScalarExpressionStrictNotEquals : ScalarExpressionLogical
        {

            public ScalarExpressionStrictNotEquals(ScalarExpression Left, ScalarExpression Right)
                : base(null, "!==", "STRICTNOTEQUALS", Left, Right)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                Cell l = this._Left.Evaluate(Variants);
                Cell r = this._Right.Evaluate(Variants);
                return !(l == r && l.Affinity == r.AFFINITY ? CellValues.True : CellValues.False);
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionStrictNotEquals(this._Left.CloneOfMe(), this._Right.CloneOfMe());
            }

        }

        public sealed class ScalarExpressionLessThan : ScalarExpressionLogical
        {

            public ScalarExpressionLessThan(ScalarExpression Left, ScalarExpression Right)
                : base(null, "<", "LESSTHAN", Left, Right)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                return (this._Left.Evaluate(Variants) < this._Right.Evaluate(Variants) ? CellValues.True : CellValues.False);
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionLessThan(this._Left.CloneOfMe(), this._Right.CloneOfMe());
            }

        }

        public sealed class ScalarExpressionLessThanOrEquals : ScalarExpressionLogical
        {

            public ScalarExpressionLessThanOrEquals(ScalarExpression Left, ScalarExpression Right)
                : base(null, "<=", "LESSTHANOREQUALS", Left, Right)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                return (this._Left.Evaluate(Variants) <= this._Right.Evaluate(Variants) ? CellValues.True : CellValues.False);
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionLessThanOrEquals(this._Left.CloneOfMe(), this._Right.CloneOfMe());
            }

        }

        public sealed class ScalarExpressionGreaterThan : ScalarExpressionLogical
        {

            public ScalarExpressionGreaterThan(ScalarExpression Left, ScalarExpression Right)
                : base(null, ">", "GREATERTHAN", Left, Right)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                return (this._Left.Evaluate(Variants) > this._Right.Evaluate(Variants) ? CellValues.True : CellValues.False);
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionGreaterThan(this._Left.CloneOfMe(), this._Right.CloneOfMe());
            }

        }

        public sealed class ScalarExpressionGreaterThanOrEquals : ScalarExpressionLogical
        {

            public ScalarExpressionGreaterThanOrEquals(ScalarExpression Left, ScalarExpression Right)
                : base(null, ">=", "GREATERTHANOREQUALS", Left, Right)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                return (this._Left.Evaluate(Variants) >= this._Right.Evaluate(Variants) ? CellValues.True : CellValues.False);
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionGreaterThanOrEquals(this._Left.CloneOfMe(), this._Right.CloneOfMe());
            }

        }


    }

}
