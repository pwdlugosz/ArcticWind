using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticWind.Util
{
    
    public class SpecialFunctions
    {

        // Constants //
        /// <summary>
        /// Represents PI
        /// </summary>
        public const double PI = 3.14159265358979;

        /// <summary>
        /// Represents the square root of PI
        /// </summary>
        public const double SQRTPI = 1.77245385090552;
        
        /// <summary>
        /// Represents the natural logarithm of 2
        /// </summary>
        public const double LN2 = 0.69314718055995;

        /// <summary>
        /// The log gamma function
        /// </summary>
        /// <param name="X"></param>
        /// <returns></returns>
        public static double LogGamma(double X)
        {
            // https://jamesmccaffrey.wordpress.com/2013/06/19/the-log-gamma-function-with-b/
            double x1 = (X - 0.5) * Math.Log(X);
            double x3 = 0.5 * Math.Log(2 * Math.PI);
            double x4 = 1 / (12 * X);
            double x5 = 1 / (360 * X * X * X);
            double x6 = 1 / (1260 * X * X * X * X * X);
            double x7 = 1 / (1680 * X * X * X * X * X * X * X);
            return x1 - X + x3 + x4 - x5 + x6 - x7;
        }

        /// <summary>
        /// The gamma function
        /// </summary>
        /// <param name="OriginalPage"></param>
        /// <returns></returns>
        public double Gamma(double x)
        {
            double[] p = {0.99999999999980993, 676.5203681218851, -1259.1392167224028,
			     	  771.32342877765313, -176.61502916214059, 12.507343278686905,
			     	  -0.13857109526572012, 9.9843695780195716e-6, 1.5056327351493116e-7};
            int g = 7;
            if (x < 0.5) return Math.PI / (Math.Sin(Math.PI * x) * Gamma(1 - x));

            x -= 1;
            double a = p[0];
            double t = x + g + 0.5;
            for (int i = 1; i < p.Length; i++)
            {
                a += p[i] / (x + i);
            }

            return Math.Sqrt(2 * Math.PI) * Math.Pow(t, x + 0.5) * Math.Exp(-t) * a;
        }

        /// <summary>
        /// Represents the factorial
        /// </summary>
        /// <param name="X"></param>
        /// <returns></returns>
        public static long Factorial(long X)
        {
            if (X <= 1)
                return 1;
            return Factorial(X - 1);
        }

        #region ProbabilityDistributions

        // Normal //
        public static double NormalPDF(double Value)
        {
            return Math.Exp(-0.5 * Value * Value) / SpecialFunctions.SQRTPI;
        }

        public static double NormalCDF(double Value)
        {

            // Variables //
            bool Inv = (Value < 0);
            Value = Math.Abs(Value);
            double[] b = { 0.2316419, 0.319381530, -0.356563782, 1.781477937, -1.821255978, 1.330274429 };
            double t = 1 / (1 + b[0] * Value);

            // Set b //
            double z = 1 - NormalPDF(Value) * (b[1] * t + b[2] * t * t + b[3] * t * t * t + b[4] * t * t * t * t + b[5] * t * t * t * t * t);
            return (Inv ? 1 - z : z);

        }

        public static double NormalINV(double PValue)
        {

            // Handle out of bounds //
            if (PValue >= 1) return double.PositiveInfinity;
            if (PValue <= 0) return double.NegativeInfinity;

            // Variables //
            double x = 0;
            double dx = 0;
            double ep = 0;
            double e = 0.0001;
            int maxitter = 10;

            for (int i = 0; i < maxitter; i++)
            {
                dx = NormalPDF(x);
                ep = (PValue - NormalCDF(x));
                if (Math.Abs(ep) <= e) break;
                x += (ep) / (dx);

            }

            return x;

        }

        // Log normal //
        public static double LogNormalPDF(double Value)
        {
            double x = Math.Log(Value);
            return Math.Exp(-0.5 * x * x) / (SpecialFunctions.SQRTPI * Value);
        }

        public static double LogNormalCDF(double Value)
        {

            Value = Math.Log(Value);

            // Variables //
            bool Inv = (Value < 0);
            Value = Math.Abs(Value);
            double[] b = { 0.2316419, 0.319381530, -0.356563782, 1.781477937, -1.821255978, 1.330274429 };
            double t = 1 / (1 + b[0] * Value);

            // Set b //
            double z = 1 - NormalPDF(Value) * (b[1] * t + b[2] * t * t + b[3] * t * t * t + b[4] * t * t * t * t + b[5] * t * t * t * t * t);
            return (Inv ? 1 - z : z);

        }

        public static double LogNormalINV(double PValue)
        {

            if (PValue < 0 || PValue > 1)
                return double.NaN;
            if (PValue == 0)
                return double.NegativeInfinity;
            if (PValue == 1)
                return double.PositiveInfinity;

            return Math.Exp(NormalINV(PValue));

        }

        // StudentsT //
        public static double StudentsTPDF(double Value, double DF)
        {

            return 0D;

        }

        public static double StudentsTCDF(double Value, double DF)
        {
            return 0D;
        }

        public static double StudentsTINV(double PValue, double DF)
        {

            // Handle out of bounds //
            if (PValue >= 1) return double.PositiveInfinity;
            if (PValue <= 0) return double.NegativeInfinity;

            // Variables //
            double x = 0;
            double dx = 0;
            double ep = 0;
            double e = 0.0001;
            int maxitter = 10;

            for (int i = 0; i < maxitter; i++)
            {
                dx = StudentsTPDF(x, DF);
                ep = (PValue - StudentsTCDF(x, DF));
                if (Math.Abs(ep) <= e) break;
                x += (ep) / (dx);

            }

            return x;

        }

        // Gamma //
        public static double GammaPDF(double Value, double Alpha, double Beta)
        {
            return 0D;
        }

        public static double GammaCDF(double Value, double Alpha, double Beta)
        {
            return 0D;
        }

        public static double GammaINV(double PValue, double Alpha, double Beta)
        {

            // Handle out of bounds //
            if (PValue >= 1) return double.PositiveInfinity;
            if (PValue <= 0) return double.NegativeInfinity;

            // Variables //
            double x = 0.5;
            double dx = 0;
            double ep = 0;
            double e = 0.0001;
            int maxitter = 10;

            for (int i = 0; i < maxitter; i++)
            {
                dx = GammaPDF(x, Alpha, Beta);
                ep = (PValue - GammaCDF(x, Alpha, Beta));
                if (Math.Abs(ep) <= e) break;
                x += (ep) / (dx);
            }

            return x;

        }

        // Exponential //
        public static double ExponetialPDF(double Value, double Lambda)
        {
            return Lambda * Math.Exp(-Value * Lambda);
        }

        public static double ExponetialCDF(double Value, double Lambda)
        {
            return 1 - Math.Exp(-Value * Lambda);
        }

        public static double ExponetialINV(double PValue, double Lambda)
        {
            return -Math.Log(1 - PValue) / Lambda;
        }

        // Chi-Square //
        public static double ChiSquarePDF(double Value, double DF)
        {
            return 0D;
        }

        public static double ChiSquareCDF(double Value, double DF)
        {
            return 0D;
        }

        public static double ChiSquareINV(double PValue, double DF)
        {

            // Handle out of bounds //
            if (PValue >= 1) return double.PositiveInfinity;
            if (PValue <= 0) return double.NegativeInfinity;

            // Variables //
            double x = 0.5;
            double dx = 0;
            double ep = 0;
            double e = 0.0001;
            int maxitter = 10;

            for (int i = 0; i < maxitter; i++)
            {

                dx = ChiSquarePDF(x, DF);
                ep = (PValue - ChiSquareCDF(x, DF));
                if (Math.Abs(ep) <= e) break;
                x += (ep) / (dx);

            }
            return x;

        }

        // Poisson //
        public static double PoissonPMF(double Value, double Lambda)
        {
            return Math.Pow(Lambda, Value) * Math.Exp(-Lambda) / SpecialFunctions.Factorial((long)Value);
        }

        public static double PoissonCDF(double Value, double Lambda)
        {
            double p = 0;
            for (int i = 0; i <= (int)Value; i++)
            {
                p += PoissonPMF(i, Lambda);
            }
            return p;
        }

        public static double PoissonINV(double PValue, double Lambda)
        {
            double x = -1, p = 0;
            while (p < PValue)
            {
                x += 1;
                p += PoissonPMF(x, Lambda);
            }
            return x;
        }

        #endregion

    }

}
