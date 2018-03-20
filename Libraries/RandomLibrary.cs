using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Expressions.TableExpressions;
using ArcticWind.Expressions.ActionExpressions;
using ArcticWind.Expressions;
using ArcticWind.Tables;
using ArcticWind.Elements;
using ArcticWind.Elements.Structures;

namespace ArcticWind.Libraries
{


    public sealed class RandomLibrary : Library
    {

        public const string NEXT_BOOL = "NEXT_BOOL";
        public const string NEXT_DATE_TIME = "NEXT_DATE_TIME";
        public const string NEXT_BYTE = "NEXT_BYTE";
        public const string NEXT_SHORT = "NEXT_SHORT";
        public const string NEXT_INT = "NEXT_INT";
        public const string NEXT_LONG = "NEXT_LONG";
        public const string NEXT_SINGLE = "NEXT_SINGLE";
        public const string NEXT_DOUBLE = "NEXT_DOUBLE";
        public const string NEXT_BINARY = "NEXT_BINARY";
        public const string NEXT_BSTRING = "NEXT_BSTRING";
        public const string NEXT_CSTRING = "NEXT_CSTRING";
        public const string NEXT_GAUSS = "NEXT_GAUSS";
        public const string NEXT_POISSON = "NEXT_POISSON";
        public const string NEXT_EMP = "NEXT_EMP";

        public static readonly string[] Scalar_Names = new string[] 
        { 
            NEXT_BOOL, NEXT_DATE_TIME, NEXT_BYTE, NEXT_SHORT, NEXT_INT, NEXT_LONG, NEXT_SINGLE, 
            NEXT_DOUBLE, NEXT_BINARY, NEXT_BSTRING, NEXT_CSTRING, NEXT_GAUSS, NEXT_POISSON, 
            NEXT_EMP
        };

        public const string SET_SEED = "SET_SEED";

        private RandomCell _Gen;

        public RandomLibrary(Host Host)
            : base(Host, "RANDOM")
        {
            this._Gen = new RandomCell(RandomCell.TruelyRandomSeed());
        }

        public override bool ActionExists(string Name)
        {
            throw new Exception(string.Format("Action does not exist '{0}'", Name));
        }

        public override ActionExpressionParameterized ActionLookup(string Name)
        {
            throw new Exception(string.Format("Action does not exist '{0}'", Name));
        }

        public override bool ScalarFunctionExists(string Name)
        {
            return Scalar_Names.Contains(Name.ToUpper());
        }

        public override ScalarExpressionFunction ScalarFunctionLookup(string Name)
        {

            switch (Name.ToUpper())
            {
                case NEXT_BOOL: return new sfNextBool(this._Host, this._Gen);
                case NEXT_DATE_TIME: return new sfNextDate(this._Host, this._Gen);
                case NEXT_BYTE: return new sfNextByte(this._Host, this._Gen);
                case NEXT_SHORT: return new sfNextShort(this._Host, this._Gen);
                case NEXT_INT: return new sfNextInt(this._Host, this._Gen);
                case NEXT_LONG: return new sfNextLong(this._Host, this._Gen);
                case NEXT_SINGLE: return new sfNextSingle(this._Host, this._Gen);
                case NEXT_DOUBLE: return new sfNextDouble(this._Host, this._Gen);
                case NEXT_BINARY: return new sfNextBinary(this._Host, this._Gen);
                case NEXT_BSTRING: return new sfNextBString(this._Host, this._Gen);
                case NEXT_CSTRING: return new sfNextCString(this._Host, this._Gen);
                case NEXT_EMP: return new sfNextEmperical(this._Host, this._Gen);
                case NEXT_GAUSS: return new sfNextGauss(this._Host, this._Gen);
                case NEXT_POISSON: return new sfNextPoisson(this._Host, this._Gen);
            }

            throw new Exception(string.Format("Scalar function does not exist '{0}'", Name));

        }

        public override bool MatrixFunctionExists(string Name)
        {
            throw new Exception(string.Format("Matrix does not exist '{0}'", Name));
        }

        public override MatrixExpressionFunction MatrixFunctionLookup(string Name)
        {
            throw new Exception(string.Format("Matrix does not exist '{0}'", Name));
        }

        public override bool RecordFunctionExists(string Name)
        {
            throw new Exception(string.Format("Record does not exist '{0}'", Name));
        }

        public override RecordExpressionFunction RecordFunctionLookup(string Name)
        {
            throw new Exception(string.Format("Record does not exist '{0}'", Name));
        }

        public override bool TableFunctionExists(string Name)
        {
            throw new Exception(string.Format("Table does not exist '{0}'", Name));
        }

        public override TableExpressionFunction TableFunctionLookup(string Name)
        {
            throw new Exception(string.Format("Table does not exist '{0}'", Name));
        }

        public sealed class sfNextBool : ScalarExpressionFunction
        {

            private RandomCell _Gen;

            public sfNextBool(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_BOOL, -1, CellAffinity.BOOL)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this.CheckSigniture())
                {
                    return this._Gen.NextBool(0.5);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar))
                {
                    return this._Gen.NextBool(this._Params[0].Scalar.Evaluate(Variants).valueSINGLE);
                }
                throw new Exception("Invalid parameters passed");

            }

        }

        public sealed class sfNextDate : ScalarExpressionFunction
        {

            private RandomCell _Gen;

            public sfNextDate(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_DATE_TIME, -2, CellAffinity.DATE_TIME)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this.CheckSigniture())
                {
                    return this._Gen.NextDate();
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar))
                {
                    return this._Gen.NextDate(DateTime.MinValue, this._Params[0].Scalar.Evaluate(Variants).valueDATE);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                {
                    return this._Gen.NextDate(this._Params[0].Scalar.Evaluate(Variants).valueDATE, this._Params[0].Scalar.Evaluate(Variants).valueDATE);
                }
                throw new Exception("Invalid parameters passed");

            }

        }

        public sealed class sfNextByte : ScalarExpressionFunction
        {

            private RandomCell _Gen;

            public sfNextByte(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_BYTE, -2, CellAffinity.BYTE)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this.CheckSigniture())
                {
                    return this._Gen.NextByte();
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar))
                {
                    return this._Gen.NextByte(0, this._Params[0].Scalar.Evaluate(Variants).valueBYTE);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                {
                    return this._Gen.NextByte(this._Params[0].Scalar.Evaluate(Variants).valueBYTE, this._Params[0].Scalar.Evaluate(Variants).valueBYTE);
                }
                throw new Exception("Invalid parameters passed");

            }

        }

        public sealed class sfNextShort : ScalarExpressionFunction
        {

            private RandomCell _Gen;

            public sfNextShort(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_SHORT, -2, CellAffinity.SHORT)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this.CheckSigniture())
                {
                    return this._Gen.NextShort();
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar))
                {
                    return this._Gen.NextShort(0, this._Params[0].Scalar.Evaluate(Variants).valueSHORT);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                {
                    return this._Gen.NextShort(this._Params[0].Scalar.Evaluate(Variants).valueSHORT, this._Params[0].Scalar.Evaluate(Variants).valueSHORT);
                }
                throw new Exception("Invalid parameters passed");

            }

        }

        public sealed class sfNextInt : ScalarExpressionFunction
        {

            private RandomCell _Gen;

            public sfNextInt(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_INT, -2, CellAffinity.INT)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this.CheckSigniture())
                {
                    return this._Gen.NextInt();
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar))
                {
                    return this._Gen.NextInt(0, this._Params[0].Scalar.Evaluate(Variants).valueINT);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                {
                    return this._Gen.NextInt(this._Params[0].Scalar.Evaluate(Variants).valueINT, this._Params[0].Scalar.Evaluate(Variants).valueINT);
                }
                throw new Exception("Invalid parameters passed");

            }

        }

        public sealed class sfNextLong : ScalarExpressionFunction
        {

            private RandomCell _Gen;

            public sfNextLong(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_LONG, -2, CellAffinity.LONG)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this.CheckSigniture())
                {
                    return this._Gen.NextLong();
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar))
                {
                    return this._Gen.NextLong(0, this._Params[0].Scalar.Evaluate(Variants).valueLONG);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                {
                    return this._Gen.NextLong(this._Params[0].Scalar.Evaluate(Variants).valueLONG, this._Params[0].Scalar.Evaluate(Variants).valueLONG);
                }
                throw new Exception("Invalid parameters passed");

            }

        }

        public sealed class sfNextSingle : ScalarExpressionFunction
        {

            private RandomCell _Gen;

            public sfNextSingle(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_SINGLE, -2, CellAffinity.SINGLE)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this.CheckSigniture())
                {
                    return this._Gen.NextSingle();
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar))
                {
                    return this._Gen.NextSingle(0, this._Params[0].Scalar.Evaluate(Variants).valueSINGLE);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                {
                    return this._Gen.NextSingle(this._Params[0].Scalar.Evaluate(Variants).valueSINGLE, this._Params[0].Scalar.Evaluate(Variants).valueSINGLE);
                }
                throw new Exception("Invalid parameters passed");

            }

        }

        public sealed class sfNextDouble : ScalarExpressionFunction
        {

            private RandomCell _Gen;

            public sfNextDouble(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_DOUBLE, -2, CellAffinity.DOUBLE)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this.CheckSigniture())
                {
                    return this._Gen.NextDouble();
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar))
                {
                    return this._Gen.NextDouble(0, this._Params[0].Scalar.Evaluate(Variants).valueDOUBLE);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                {
                    return this._Gen.NextDouble(this._Params[0].Scalar.Evaluate(Variants).valueDOUBLE, this._Params[0].Scalar.Evaluate(Variants).valueDOUBLE);
                }
                throw new Exception("Invalid parameters passed");

            }

        }

        public sealed class sfNextBinary : ScalarExpressionFunction
        {

            private RandomCell _Gen;
            private int _Size = Schema.DEFAULT_BLOB_SIZE;

            public sfNextBinary(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_BINARY, -2, CellAffinity.BINARY)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this.CheckSigniture())
                {
                    return this._Gen.NextBinary(this._Size);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar))
                {
                    this._Size = this._Params[0].Scalar.Evaluate(Variants).valueINT;
                    return this._Gen.NextBinary(this._Size);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                {
                    this._Size = this._Params[0].Scalar.Evaluate(Variants).valueINT;
                    byte[] corpus = this._Params[1].Scalar.Evaluate(Variants).valueBINARY;
                    return this._Gen.NextBinary(this._Size, corpus);
                }
                throw new Exception("Invalid parameters passed");

            }

        }

        public sealed class sfNextBString : ScalarExpressionFunction
        {

            private RandomCell _Gen;
            private int _Size = Schema.DEFAULT_STRING_SIZE;

            public sfNextBString(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_BSTRING, -2, CellAffinity.BSTRING)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this.CheckSigniture())
                {
                    return this._Gen.NextBString(this._Size);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar))
                {
                    this._Size = this._Params[0].Scalar.Evaluate(Variants).valueINT;
                    return this._Gen.NextBString(this._Size);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                {
                    this._Size = this._Params[0].Scalar.Evaluate(Variants).valueINT;
                    BString b = this._Params[1].Scalar.Evaluate(Variants).valueBSTRING;
                    return this._Gen.NextBString(this._Size, b);
                }
                throw new Exception("Invalid parameters passed");

            }

        }

        public sealed class sfNextCString : ScalarExpressionFunction
        {

            private RandomCell _Gen;
            private int _Size = Schema.DEFAULT_STRING_SIZE;

            public sfNextCString(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_CSTRING, -2, CellAffinity.CSTRING)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this.CheckSigniture())
                {
                    return this._Gen.NextCString(this._Size, (int)char.MaxValue);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar))
                {
                    this._Size = this._Params[0].Scalar.Evaluate(Variants).valueINT;
                    return this._Gen.NextCString(this._Size, (int)char.MaxValue);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                {
                    this._Size = this._Params[0].Scalar.Evaluate(Variants).valueINT;
                    string b = this._Params[1].Scalar.Evaluate(Variants).valueCSTRING;
                    return this._Gen.NextBString(this._Size, b);
                }
                throw new Exception("Invalid parameters passed");

            }

        }

        public sealed class sfNextEmperical : ScalarExpressionFunction
        {

            private RandomCell _Gen;

            public sfNextEmperical(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_EMP, 1, CellAffinity.INT)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this.CheckSigniture(ParameterAffinity.Matrix))
                {
                    CellMatrix m = this._Params[0].Matrix.Evaluate(Variants);
                    return this._Gen.NextEmperical(m);
                }
                throw new Exception("Invalid parameters passed");

            }


        }

        public sealed class sfNextGauss : ScalarExpressionFunction
        {

            private RandomCell _Gen;

            public sfNextGauss(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_GAUSS, 0, CellAffinity.DOUBLE)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this.CheckSigniture())
                {
                    return this._Gen.NextDoubleGauss();
                }
                throw new Exception("Invalid parameters passed");

            }

        }

        public sealed class sfNextPoisson : ScalarExpressionFunction
        {

            private RandomCell _Gen;

            public sfNextPoisson(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_POISSON, 1, CellAffinity.DOUBLE)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this.CheckSigniture(ParameterAffinity.Scalar))
                {
                    return this._Gen.NextPoisson(this._Params[0].Scalar.Evaluate(Variants).valueDOUBLE);
                }
                throw new Exception("Invalid parameters passed");

            }


        }

    }


}
