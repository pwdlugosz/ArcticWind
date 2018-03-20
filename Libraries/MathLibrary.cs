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


    public sealed class MathLibrary : Library
    {

        public const string LOG = "LOG";
        public const string LOG2 = "LOG2";
        public const string LOG10 = "LOG10";
        public const string EXP = "EXP";
        public const string EXP2 = "EXP2";
        public const string EXP10 = "EXP10";
        public const string SIN = "SIN";
        public const string COS = "COS";
        public const string TAN = "TAN";
        public const string ARCSIN = "ARCSIN";
        public const string ARCCOS = "ARCCOS";
        public const string ARCTAN = "ARCTAN";
        public const string SINH = "SINH";
        public const string COSH = "COSH";
        public const string TANH = "TANH";
        public const string ARCSINH = "ARCSINH";
        public const string ARCCOSH = "ARCCOSH";
        public const string ARCTANH = "ARCTANH";
        public const string ABS = "ABS";
        public const string SIGN = "SIGN";
        public const string ROUND = "ROUND";
        public const string SQRT = "SQRT";
        public const string LOGIT = "LOGIT";
        public const string MODPOW = "MODPOW";

        public static readonly string[] ActionNames = { };
        public static readonly string[] ScalarNames = { LOG, LOG2, LOG10, EXP, EXP2, EXP10, SIN, COS, TAN, ARCSIN, ARCCOS, ARCTAN, SINH, COSH, TANH, ARCSINH, ARCCOSH, ARCTANH, ABS, SIGN, ROUND, SQRT, LOGIT, MODPOW };
        public static readonly string[] MatrixNames = { };
        public static readonly string[] RecordNames = { };
        public static readonly string[] TableNames = { };

        public MathLibrary(Host Host)
            : base(Host, "MATH")
        {
        }

        public override bool ActionExists(string Name)
        {
            return ActionNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override ActionExpressionParameterized ActionLookup(string Name)
        {
            throw new Exception(string.Format("Action '{0}' does not exist", Name));
        }

        public override bool ScalarFunctionExists(string Name)
        {
            return ScalarNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override ScalarExpressionFunction ScalarFunctionLookup(string Name)
        {

            switch (Name.ToUpper())
            {
                case LOG:
                    return new sfLOG(this._Host);
                case LOG2:
                    return new sfLOG2(this._Host);
                case LOG10:
                    return new sfLOG10(this._Host);
                case EXP:
                    return new sfEXP(this._Host);
                case EXP2:
                    return new sfEXP2(this._Host);
                case EXP10:
                    return new sfEXP10(this._Host);
                case SIN:
                    return new sfSIN(this._Host);
                case COS:
                    return new sfCOS(this._Host);
                case TAN:
                    return new sfTAN(this._Host);
                case ARCSIN:
                    return new sfARCSIN(this._Host);
                case ARCCOS:
                    return new sfARCCOS(this._Host);
                case ARCTAN:
                    return new sfARCTAN(this._Host);
                case SINH:
                    return new sfSINH(this._Host);
                case COSH:
                    return new sfCOSH(this._Host);
                case TANH:
                    return new sfTANH(this._Host);
                case ARCSINH:
                    return new sfARCSINH(this._Host);
                case ARCCOSH:
                    return new sfARCCOSH(this._Host);
                case ARCTANH:
                    return new sfARCTANH(this._Host);
                case ABS:
                    return new sfABS(this._Host);
                case SIGN:
                    return new sfSIGN(this._Host);
                case ROUND:
                    return new sfROUND(this._Host);
                case SQRT:
                    return new sfSQRT(this._Host);
                case LOGIT:
                    return new sfLOGIT(this._Host);
                case MODPOW:
                    return new sfMODPOW(this._Host);
            }

            throw new Exception(string.Format("Scalar function '{0}' does not exist", Name));

        }

        public override bool MatrixFunctionExists(string Name)
        {
            return MatrixNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override MatrixExpressionFunction MatrixFunctionLookup(string Name)
        {
            throw new Exception(string.Format("Matrix function '{0}' does not exist", Name));
        }

        public override bool RecordFunctionExists(string Name)
        {
            return RecordNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override RecordExpressionFunction RecordFunctionLookup(string Name)
        {
            throw new Exception(string.Format("Record function '{0}' does not exist", Name));
        }

        public override bool TableFunctionExists(string Name)
        {
            return TableNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override TableExpressionFunction TableFunctionLookup(string Name)
        {
            throw new Exception(string.Format("Table function '{0}' does not exist", Name));
        }

        public sealed class sfLOG : ScalarExpressionFunction
        {

            public sfLOG(Host Host)
                : base(Host, null, LOG, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return this.MaxReturnAffinityOfScalarParamters().Affinity;
            }

            public override int ReturnSize()
            {
                return this.MaxReturnAffinityOfScalarParamters().Size;
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this.CheckSigniture(ParameterAffinity.Scalar))
                    return CellFunctions.Log(this._Params[0].Scalar.Evaluate(Variants));
                throw new Exception("Invalid parameter passed");
            }

        }

        public sealed class sfLOG2 : ScalarExpressionFunction
        {

            public sfLOG2(Host Host)
                : base(Host, null, LOG2, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return this.MaxReturnAffinityOfScalarParamters().Affinity;
            }

            public override int ReturnSize()
            {
                return this.MaxReturnAffinityOfScalarParamters().Size;
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this.CheckSigniture(ParameterAffinity.Scalar))
                    return CellFunctions.Log2(this._Params[0].Scalar.Evaluate(Variants));
                throw new Exception("Invalid parameter passed");
            }

        }

        public sealed class sfLOG10 : ScalarExpressionFunction
        {

            public sfLOG10(Host Host)
                : base(Host, null, LOG10, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return this.MaxReturnAffinityOfScalarParamters().Affinity;
            }

            public override int ReturnSize()
            {
                return this.MaxReturnAffinityOfScalarParamters().Size;
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this.CheckSigniture(ParameterAffinity.Scalar))
                    return CellFunctions.Log10(this._Params[0].Scalar.Evaluate(Variants));
                throw new Exception("Invalid parameter passed");
            }

        }

        public sealed class sfEXP : ScalarExpressionFunction
        {

            public sfEXP(Host Host)
                : base(Host, null, EXP, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return this.MaxReturnAffinityOfScalarParamters().Affinity;
            }

            public override int ReturnSize()
            {
                return this.MaxReturnAffinityOfScalarParamters().Size;
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this.CheckSigniture(ParameterAffinity.Scalar))
                    return CellFunctions.Exp(this._Params[0].Scalar.Evaluate(Variants));
                throw new Exception("Invalid parameter passed");
            }

        }

        public sealed class sfEXP2 : ScalarExpressionFunction
        {

            public sfEXP2(Host Host)
                : base(Host, null, EXP2, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return this.MaxReturnAffinityOfScalarParamters().Affinity;
            }

            public override int ReturnSize()
            {
                return this.MaxReturnAffinityOfScalarParamters().Size;
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this.CheckSigniture(ParameterAffinity.Scalar))
                    return CellFunctions.Exp2(this._Params[0].Scalar.Evaluate(Variants));
                throw new Exception("Invalid parameter passed");
            }

        }

        public sealed class sfEXP10 : ScalarExpressionFunction
        {

            public sfEXP10(Host Host)
                : base(Host, null, EXP10, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return this.MaxReturnAffinityOfScalarParamters().Affinity;
            }

            public override int ReturnSize()
            {
                return this.MaxReturnAffinityOfScalarParamters().Size;
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this.CheckSigniture(ParameterAffinity.Scalar))
                    return CellFunctions.Exp10(this._Params[0].Scalar.Evaluate(Variants));
                throw new Exception("Invalid parameter passed");
            }

        }

        public sealed class sfSIN : ScalarExpressionFunction
        {

            public sfSIN(Host Host)
                : base(Host, null, SIN, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return this.MaxReturnAffinityOfScalarParamters().Affinity;
            }

            public override int ReturnSize()
            {
                return this.MaxReturnAffinityOfScalarParamters().Size;
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this.CheckSigniture(ParameterAffinity.Scalar))
                    return CellFunctions.Sin(this._Params[0].Scalar.Evaluate(Variants));
                throw new Exception("Invalid parameter passed");
            }

        }

        public sealed class sfCOS : ScalarExpressionFunction
        {

            public sfCOS(Host Host)
                : base(Host, null, COS, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return this.MaxReturnAffinityOfScalarParamters().Affinity;
            }

            public override int ReturnSize()
            {
                return this.MaxReturnAffinityOfScalarParamters().Size;
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this.CheckSigniture(ParameterAffinity.Scalar))
                    return CellFunctions.Cos(this._Params[0].Scalar.Evaluate(Variants));
                throw new Exception("Invalid parameter passed");
            }

        }

        public sealed class sfTAN : ScalarExpressionFunction
        {

            public sfTAN(Host Host)
                : base(Host, null, TAN, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return this.MaxReturnAffinityOfScalarParamters().Affinity;
            }

            public override int ReturnSize()
            {
                return this.MaxReturnAffinityOfScalarParamters().Size;
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this.CheckSigniture(ParameterAffinity.Scalar))
                    return CellFunctions.Tan(this._Params[0].Scalar.Evaluate(Variants));
                throw new Exception("Invalid parameter passed");
            }

        }

        public sealed class sfARCSIN : ScalarExpressionFunction
        {

            public sfARCSIN(Host Host)
                : base(Host, null, ARCSIN, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return this.MaxReturnAffinityOfScalarParamters().Affinity;
            }

            public override int ReturnSize()
            {
                return this.MaxReturnAffinityOfScalarParamters().Size;
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this.CheckSigniture(ParameterAffinity.Scalar))
                    return CellFunctions.ArcSin(this._Params[0].Scalar.Evaluate(Variants));
                throw new Exception("Invalid parameter passed");
            }

        }

        public sealed class sfARCCOS : ScalarExpressionFunction
        {

            public sfARCCOS(Host Host)
                : base(Host, null, ARCCOS, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return this.MaxReturnAffinityOfScalarParamters().Affinity;
            }

            public override int ReturnSize()
            {
                return this.MaxReturnAffinityOfScalarParamters().Size;
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this.CheckSigniture(ParameterAffinity.Scalar))
                    return CellFunctions.ArcCos(this._Params[0].Scalar.Evaluate(Variants));
                throw new Exception("Invalid parameter passed");
            }

        }

        public sealed class sfARCTAN : ScalarExpressionFunction
        {

            public sfARCTAN(Host Host)
                : base(Host, null, ARCTAN, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return this.MaxReturnAffinityOfScalarParamters().Affinity;
            }

            public override int ReturnSize()
            {
                return this.MaxReturnAffinityOfScalarParamters().Size;
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this.CheckSigniture(ParameterAffinity.Scalar))
                    return CellFunctions.ArcTan(this._Params[0].Scalar.Evaluate(Variants));
                throw new Exception("Invalid parameter passed");
            }

        }

        public sealed class sfSINH : ScalarExpressionFunction
        {

            public sfSINH(Host Host)
                : base(Host, null, SINH, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return this.MaxReturnAffinityOfScalarParamters().Affinity;
            }

            public override int ReturnSize()
            {
                return this.MaxReturnAffinityOfScalarParamters().Size;
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this.CheckSigniture(ParameterAffinity.Scalar))
                    return CellFunctions.Sinh(this._Params[0].Scalar.Evaluate(Variants));
                throw new Exception("Invalid parameter passed");
            }

        }

        public sealed class sfCOSH : ScalarExpressionFunction
        {

            public sfCOSH(Host Host)
                : base(Host, null, COSH, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return this.MaxReturnAffinityOfScalarParamters().Affinity;
            }

            public override int ReturnSize()
            {
                return this.MaxReturnAffinityOfScalarParamters().Size;
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this.CheckSigniture(ParameterAffinity.Scalar))
                    return CellFunctions.Cosh(this._Params[0].Scalar.Evaluate(Variants));
                throw new Exception("Invalid parameter passed");
            }

        }

        public sealed class sfTANH : ScalarExpressionFunction
        {

            public sfTANH(Host Host)
                : base(Host, null, TANH, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return this.MaxReturnAffinityOfScalarParamters().Affinity;
            }

            public override int ReturnSize()
            {
                return this.MaxReturnAffinityOfScalarParamters().Size;
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this.CheckSigniture(ParameterAffinity.Scalar))
                    return CellFunctions.Tanh(this._Params[0].Scalar.Evaluate(Variants));
                throw new Exception("Invalid parameter passed");
            }

        }

        public sealed class sfARCSINH : ScalarExpressionFunction
        {

            public sfARCSINH(Host Host)
                : base(Host, null, ARCSINH, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return this.MaxReturnAffinityOfScalarParamters().Affinity;
            }

            public override int ReturnSize()
            {
                return this.MaxReturnAffinityOfScalarParamters().Size;
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this.CheckSigniture(ParameterAffinity.Scalar))
                    return CellFunctions.ArcSin(this._Params[0].Scalar.Evaluate(Variants));
                throw new Exception("Invalid parameter passed");
            }

        }

        public sealed class sfARCCOSH : ScalarExpressionFunction
        {

            public sfARCCOSH(Host Host)
                : base(Host, null, ARCCOSH, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return this.MaxReturnAffinityOfScalarParamters().Affinity;
            }

            public override int ReturnSize()
            {
                return this.MaxReturnAffinityOfScalarParamters().Size;
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this.CheckSigniture(ParameterAffinity.Scalar))
                    return CellFunctions.ArcCosh(this._Params[0].Scalar.Evaluate(Variants));
                throw new Exception("Invalid parameter passed");
            }

        }

        public sealed class sfARCTANH : ScalarExpressionFunction
        {

            public sfARCTANH(Host Host)
                : base(Host, null, ARCTANH, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return this.MaxReturnAffinityOfScalarParamters().Affinity;
            }

            public override int ReturnSize()
            {
                return this.MaxReturnAffinityOfScalarParamters().Size;
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this.CheckSigniture(ParameterAffinity.Scalar))
                    return CellFunctions.ArcTanh(this._Params[0].Scalar.Evaluate(Variants));
                throw new Exception("Invalid parameter passed");
            }

        }

        public sealed class sfABS : ScalarExpressionFunction
        {

            public sfABS(Host Host)
                : base(Host, null, ABS, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return this.MaxReturnAffinityOfScalarParamters().Affinity;
            }

            public override int ReturnSize()
            {
                return this.MaxReturnAffinityOfScalarParamters().Size;
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this.CheckSigniture(ParameterAffinity.Scalar))
                    return CellFunctions.Abs(this._Params[0].Scalar.Evaluate(Variants));
                throw new Exception("Invalid parameter passed");
            }

        }

        public sealed class sfSIGN : ScalarExpressionFunction
        {

            public sfSIGN(Host Host)
                : base(Host, null, SIGN, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return this.MaxReturnAffinityOfScalarParamters().Affinity;
            }

            public override int ReturnSize()
            {
                return this.MaxReturnAffinityOfScalarParamters().Size;
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this.CheckSigniture(ParameterAffinity.Scalar))
                    return CellFunctions.Sign(this._Params[0].Scalar.Evaluate(Variants));
                throw new Exception("Invalid parameter passed");
            }

        }

        public sealed class sfROUND : ScalarExpressionFunction
        {

            public sfROUND(Host Host)
                : base(Host, null, ROUND, 2)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return this.MaxReturnAffinityOfScalarParamters().Affinity;
            }

            public override int ReturnSize()
            {
                return this.MaxReturnAffinityOfScalarParamters().Size;
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this.CheckSigniture(ParameterAffinity.Scalar))
                    return CellFunctions.Round(this._Params[0].Scalar.Evaluate(Variants), 0);
                if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                    return CellFunctions.Round(this._Params[0].Scalar.Evaluate(Variants), this._Params[1].Scalar.Evaluate(Variants).valueINT);
                throw new Exception("Invalid parameter passed");
            }

        }

        public sealed class sfSQRT : ScalarExpressionFunction
        {

            public sfSQRT(Host Host)
                : base(Host, null, SQRT, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return this.MaxReturnAffinityOfScalarParamters().Affinity;
            }

            public override int ReturnSize()
            {
                return this.MaxReturnAffinityOfScalarParamters().Size;
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this.CheckSigniture(ParameterAffinity.Scalar))
                    return CellFunctions.Sqrt(this._Params[0].Scalar.Evaluate(Variants));
                throw new Exception("Invalid parameter passed");
            }

        }

        public sealed class sfLOGIT : ScalarExpressionFunction
        {

            public sfLOGIT(Host Host)
                : base(Host, null, LOGIT, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return this.MaxReturnAffinityOfScalarParamters().Affinity;
            }

            public override int ReturnSize()
            {
                return this.MaxReturnAffinityOfScalarParamters().Size;
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this.CheckSigniture(ParameterAffinity.Scalar))
                    return CellFunctions.Logit(this._Params[0].Scalar.Evaluate(Variants));
                throw new Exception("Invalid parameter passed");
            }

        }

        public sealed class sfMODPOW : ScalarExpressionFunction
        {

            public sfMODPOW(Host Host)
                : base(Host, null, MODPOW, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return this.MaxReturnAffinityOfScalarParamters().Affinity;
            }

            public override int ReturnSize()
            {
                return this.MaxReturnAffinityOfScalarParamters().Size;
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                    return CellFunctions.ModPow(this._Params[0].Scalar.Evaluate(Variants), this._Params[1].Scalar.Evaluate(Variants), this._Params[2].Scalar.Evaluate(Variants));
                throw new Exception("Invalid parameter passed");
            }

        }


    }


}
