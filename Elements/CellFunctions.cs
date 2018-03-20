using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticWind.Elements
{

    /// <summary>
    /// Contains functions for cells
    /// </summary>
    public static class CellFunctions
    {

        /// <summary>
        /// Performs the log base Value; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Log(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Log(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the log base 2; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Log2(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Log(C.valueDOUBLE) / Math.Log(2D);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the log base 10; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Log10(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Log10(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the exponential base Value; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Exp(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Exp(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the exponential base 2; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Exp2(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Pow(2, C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the exponential base 10; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Exp10(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Pow(10, C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the square root; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Sqrt(Cell C)
        {

            if (C.NULL == 1)
                return C;

            //if (C.AFFINITY == CellAffinity.LONG)
            //{

            //    if (C.LONG <= 0)
            //    {
            //        C.NULL = 1;
            //        return C;
            //    }

            //    C.LONG = Cell.IntRoot(C.LONG);
            //    return C;

            //}

            double d = Math.Sqrt(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the power; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C1">The base</param>
        /// <param name="C2">The exponent</param>
        /// <returns>Cell Value</returns>
        public static Cell Power(Cell C1, Cell C2)
        {

            if (C1.NULL == 1)
                return C1;
            else if (C2.NULL == 1)
                return C2;

            //if (C1.AFFINITY == CellAffinity.LONG && C2.AFFINITY == CellAffinity.LONG)
            //{
            //    C1.LONG = Cell.IntPower(C1.LONG, C2.LONG);
            //    return C1;
            //}

            double d = Math.Pow(C1.valueDOUBLE, C2.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C1.NULL = 1;
                return C1;
            }

            if (C1.AFFINITY == CellAffinity.DOUBLE) C1.DOUBLE = d;
            else if (C1.AFFINITY == CellAffinity.SINGLE) C1.SINGLE = (float)d;
            else if (C1.AFFINITY == CellAffinity.LONG) C1.LONG = (long)d;
            else if (C1.AFFINITY == CellAffinity.INT) C1.INT = (int)d;
            else if (C1.AFFINITY == CellAffinity.SHORT) C1.SHORT = (short)d;
            else if (C1.AFFINITY == CellAffinity.BYTE) C1.BYTE = (byte)d;
            else C1.NULL = 1;

            return C1;

        }

        // Basic Trig Functions //
        /// <summary>
        /// Performs the trigonomic sine; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Sin(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Sin(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the trigonomic cosine; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Cos(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Cos(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the trigonomic tangent; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Tan(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Tan(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell Csc(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = 1D / Math.Sin(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell Sec(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = 1D / Math.Cos(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell Cot(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = 1D / Math.Tan(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }
        
        // Inverse trig functions //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell ArcSin(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Asin(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell ArcCos(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Acos(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell ArcTan(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Atan(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell ArcCsc(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Asin(1D / C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell ArcSec(Cell C)
        {

            if (C.NULL == 1)
                return C;


            double d = Math.Acos(1D / C.valueDOUBLE); 
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell ArcCot(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Atan(1D / C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        // Hyperbolic functions //
        /// <summary>
        /// Performs the hyperbolic sine; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Sinh(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Sinh(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the hyperbolic cosine; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Cosh(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Cosh(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the hyperbolic tangent; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Tanh(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Tanh(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell Csch(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = 1D / Math.Sinh(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell Sech(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = 1D / Math.Cosh(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell Coth(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = 1D / Math.Tanh(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        // Inverse hyperbolic
        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell ArcSinh(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = C.valueDOUBLE;
            d = Math.Log(d + Math.Sqrt(d * d + 1));
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell ArcCosh(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = C.valueDOUBLE;
            d = Math.Log(d + Math.Sqrt(d * d - 1));
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell ArcTanh(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = C.valueDOUBLE;
            d = Math.Log((1+d)/(1-d)) * 0.5;
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell ArcCsch(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = 1D / C.valueDOUBLE;
            d = Math.Log(d + Math.Sqrt(d * d + 1));
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell ArcSech(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = 1D / C.valueDOUBLE; 
            d = Math.Log(d + Math.Sqrt(d * d + 1));
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell ArcCoth(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = 1D / C.valueDOUBLE; 
            d = Math.Log(d + Math.Sqrt(d * d + 1));
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell Logit(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = 1D / (1 + Math.Exp(-C.valueDOUBLE));
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        public static Cell ModPow(Cell Base, Cell Exp, Cell Mod)
        {

            Cell u = LongModPow(Base.valueLONG, Exp.valueLONG, Mod.valueLONG);
            return CellConverter.Cast(u, Base.Affinity);

        }

        // Other //
        /// <summary>
        /// Returns the absolute Value of a cell's numeric Value; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Abs(Cell C)
        {

            if (C.NULL == 1)
                return C;

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = Math.Abs(C.DOUBLE);
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = Math.Abs(C.SINGLE);
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = Math.Abs(C.LONG);
            else if (C.AFFINITY == CellAffinity.INT) C.INT = Math.Abs(C.INT);
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = Math.Abs(C.SHORT);
            else if (C.AFFINITY == CellAffinity.BYTE) C.LONG = Math.Abs(C.BYTE);
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Returns the sign of a cell's numeric Value; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value, NULL, +1, -1, or 0</returns>
        public static Cell Sign(Cell C)
        {

            if (C.NULL == 1)
                return C;

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = Math.Sign(C.DOUBLE);
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = Math.Sign(C.SINGLE);
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = Math.Sign(C.LONG);
            else if (C.AFFINITY == CellAffinity.INT) C.INT = Math.Sign(C.INT);
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)Math.Sign(C.SHORT);
            else if (C.AFFINITY == CellAffinity.BYTE) C.LONG = Math.Sign(C.BYTE);
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <param name="Percision"></param>
        /// <returns></returns>
        public static Cell Round(Cell C, int Percision)
        {

            if (C.NULL == 1)
                return C;

            if (C.AFFINITY == CellAffinity.DOUBLE) 
                C.DOUBLE = Math.Round(C.DOUBLE, Percision);
            else if (C.AFFINITY == CellAffinity.SINGLE) 
                C.SINGLE = (Single)Math.Round((double)C.SINGLE, Percision);
            else if (C.AFFINITY != CellAffinity.BYTE && C.AFFINITY != CellAffinity.SHORT && C.AFFINITY != CellAffinity.INT && C.AFFINITY != CellAffinity.LONG) 
                C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the logic 'IF'
        /// </summary>
        /// <param name="A">Predicate: uses A.BOOL to perform the logical if</param>
        /// <param name="B">The Value returned if A is true</param>
        /// <param name="C">The Value returned if A is false</param>
        /// <returns>Aither B or C</returns>
        public static Cell If(Cell A, Cell B, Cell C)
        {
            if (A.BOOL)
                return B;
            if (B.AFFINITY != C.AFFINITY)
                return CellConverter.Cast(C, B.Affinity);
            return C;
        }

        /// <summary>
        /// Returns the smallest Value of an array of cells
        /// </summary>
        /// <param name="Elements">A collection of cells</param>
        /// <returns>A cell</returns>
        public static Cell Min(params Cell[] Data)
        {

            // Empty //
            if (Data == null) new Cell(CellAffinity.LONG);
            if (Data.Length == 0) new Cell(CellAffinity.LONG);

            // One Value //
            if (Data.Length == 1) return Data[0];

            // Two values //
            if (Data.Length == 2 && Data[0] < Data[1]) return Data[0];
            if (Data.Length == 2) return Data[1];

            // Three or more //
            Cell c = Data[0];
            for (int i = 1; i < Data.Length; i++)
            {
                if (Data[i] < c)
                    c = Data[i];
            }
            if (Data[0].Affinity != c.Affinity)
                return CellConverter.Cast(c, Data[0].Affinity);
            return c;

        }

        /// <summary>
        /// Returns the largest Value of an array of cells
        /// </summary>
        /// <param name="Elements">A collection of cells</param>
        /// <returns>A cell</returns>
        public static Cell Max(params Cell[] Data)
        {

            // Empty //
            if (Data == null) new Cell(CellAffinity.LONG);
            if (Data.Length == 0) new Cell(CellAffinity.LONG);

            // One Value //
            if (Data.Length == 1) return Data[0];

            // Two values //
            if (Data.Length == 2 && Data[0] > Data[1]) return Data[0];
            if (Data.Length == 2) return Data[1];

            // Three or more //
            Cell c = Data[0];
            for (int i = 1; i < Data.Length; i++)
            {
                if (Data[i] > c) c = Data[i];
            }
            if (Data[0].Affinity != c.Affinity)
                return CellConverter.Cast(c, Data[0].Affinity);
            return c;

        }

        /// <summary>
        /// Returns the most extreme Value in a sequence
        /// </summary>
        /// <param name="Elements">The sequence to evaluate; the first Value is the radix, the next N values are compared to the radix</param>
        /// <returns>The Value with the greatest distance from the radix</returns>
        public static Cell Extreme(params Cell[] Data)
        {

            // Handle invalid argument structures //
            if (Data.Length == 0)
                return CellValues.NullINT;

            CellAffinity t = Data[0].AFFINITY;

            // Handle invalid types //
            if (t == CellAffinity.BINARY || t == CellAffinity.BOOL || t == CellAffinity.CSTRING)
                return new Cell(t);

            // Handle arrays too small //
            if (Data.Length < 2)
                return new Cell(t);

            // Get the radix //
            Cell radix = Data[0];
            Cell MostExtreme = CellValues.Zero(t);
            Cell GreatestDistance = CellValues.Zero(t);

            // Cycle through looking for the most extreme Value //
            for (int i = 1; i < Data.Length; i++)
            {

                Cell distance = CellFunctions.Abs(Data[i] - radix);

                if (distance > GreatestDistance)
                {
                    GreatestDistance = distance;
                    MostExtreme = Data[i];
                }

            }

            if (MostExtreme.AFFINITY != t)
                return CellConverter.Cast(MostExtreme, t);
            return MostExtreme;

        }

        /// <summary>
        /// Returns the cumulative AND Value of an array of cells
        /// </summary>
        /// <param name="Elements">A collection of cells</param>
        /// <returns>A cell</returns>
        public static Cell And(params Cell[] Data)
        {

            // Empty //
            if (Data == null) new Cell(false);
            if (Data.Length == 0) new Cell(false);

            // Three or more //
            bool b = true;
            for (int i = 0; i < Data.Length; i++)
            {
                b = b && Data[i].valueBOOL;
                if (!b) return new Cell(b);
            }
            return new Cell(b);

        }

        /// <summary>
        /// Returns the cumulative OR Value of an array of cells
        /// </summary>
        /// <param name="Elements">A collection of cells</param>
        /// <returns>A cell</returns>
        public static Cell Or(params Cell[] Data)
        {

            // Empty //
            if (Data == null) new Cell(false);
            if (Data.Length == 0) new Cell(false);

            // Three or more //
            bool b = false;
            for (int i = 0; i < Data.Length; i++)
            {
                b = b || Data[i].valueBOOL;
                if (b) return new Cell(b);
            }
            return new Cell(b);

        }

        /// <summary>
        /// Returns the sum of an array of cells
        /// </summary>
        /// <param name="Elements">A collection of cells</param>
        /// <returns>A cell</returns>
        public static Cell Sum(params Cell[] Data)
        {

            // Empty //
            if (Data == null) new Cell(CellAffinity.LONG);
            if (Data.Length == 0) new Cell(CellAffinity.LONG);

            // One Value //
            if (Data.Length == 1) return Data[0];

            // Two values //
            if (Data.Length == 2) return Data[0] + Data[1];

            // Three or more //
            Cell c = Data[0];
            for (int i = 1; i < Data.Length; i++)
            {
                c += Data[i];
            }
            return c;

        }

        /// <summary>
        /// Returns the first non-null cell in a collection
        /// </summary>
        /// <param name="Elements">A collection of cells</param>
        /// <returns>A cell</returns>
        public static Cell Coalesce(params Cell[] Data)
        {

            // Empty //
            if (Data == null) new Cell(CellAffinity.LONG);
            if (Data.Length == 0) new Cell(CellAffinity.LONG);

            for (int i = 0; i < Data.Length; i++)
            {
                if (!Data[i].IsNull)
                {
                    if (Data[i].Affinity != Data[0].Affinity)
                        return CellConverter.Cast(Data[i], Data[0].Affinity);
                    return Data[i];
                }
            }
            return new Cell(Data[0].Affinity);

        }

        /// <summary>
        /// Extracts the year Value of a date time cell, returns null for non-date cells
        /// </summary>
        /// <param name="C">A cell Value</param>
        /// <returns>An integer cell</returns>
        public static Cell Year(Cell C)
        {
            if (C.Affinity != CellAffinity.DATE_TIME)
                return new Cell(CellAffinity.INT);
            return new Cell(C.valueDATE.Year);
        }

        /// <summary>
        /// Extracts the month Value of a date time cell, returns null for non-date cells
        /// </summary>
        /// <param name="C">A cell Value</param>
        /// <returns>An integer cell</returns>
        public static Cell Month(Cell C)
        {
            if (C.Affinity != CellAffinity.DATE_TIME) return new Cell(CellAffinity.INT);
            return new Cell(C.valueDATE.Month);
        }

        /// <summary>
        /// Extracts the day Value of a date time cell, returns null for non-date cells
        /// </summary>
        /// <param name="C">A cell Value</param>
        /// <returns>An integer cell</returns>
        public static Cell Day(Cell C)
        {
            if (C.Affinity != CellAffinity.DATE_TIME) return new Cell(CellAffinity.INT);
            return new Cell(C.valueDATE.Day);
        }

        /// <summary>
        /// Extracts the hour Value of a date time cell, returns null for non-date cells
        /// </summary>
        /// <param name="C">A cell Value</param>
        /// <returns>An integer cell</returns>
        public static Cell Hour(Cell C)
        {
            if (C.Affinity != CellAffinity.DATE_TIME) return new Cell(CellAffinity.INT);
            return new Cell(C.valueDATE.Hour);
        }

        /// <summary>
        /// Extracts the minute Value of a date time cell, returns null for non-date cells
        /// </summary>
        /// <param name="C">A cell Value</param>
        /// <returns>An integer cell</returns>
        public static Cell Minute(Cell C)
        {
            if (C.Affinity != CellAffinity.DATE_TIME) return new Cell(CellAffinity.INT);
            return new Cell(C.valueDATE.Minute);
        }

        /// <summary>
        /// Extracts the second Value of a date time cell, returns null for non-date cells
        /// </summary>
        /// <param name="C">A cell Value</param>
        /// <returns>An integer cell</returns>
        public static Cell Second(Cell C)
        {
            if (C.Affinity != CellAffinity.DATE_TIME) return new Cell(CellAffinity.INT);
            return new Cell(C.valueDATE.Second);
        }

        /// <summary>
        /// Extracts the millisecond Value of a date time cell, returns null for non-date cells
        /// </summary>
        /// <param name="C">A cell Value</param>
        /// <returns>An integer cell</returns>
        public static Cell Millisecond(Cell C)
        {
            if (C.Affinity != CellAffinity.DATE_TIME) return new Cell(CellAffinity.INT);
            return new Cell(C.valueDATE.Millisecond);
        }

        /// <summary>
        /// Manipulates the ticks Value
        /// </summary>
        /// <param name="C"></param>
        /// <param name="Ticks"></param>
        /// <returns></returns>
        public static Cell AddTicks(Cell C, Cell Ticks)
        {

            if (C.AFFINITY != CellAffinity.DATE_TIME)
                return CellValues.NullDATE;

            C.LONG += Ticks.LONG;

            return C;

        }

        /// <summary>
        /// Trims a given string Value
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell with a stirng affinity</returns>
        public static Cell Trim(Cell C)
        {
            if (C.IsNull) return C;
            return (C.Affinity == CellAffinity.BSTRING ? new Cell(C.valueBSTRING.Trim()) : new Cell(C.valueCSTRING.Trim()));
        }

        /// <summary>
        /// Converts a given string to uppercase
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell with a stirng affinity</returns>
        public static Cell ToUpper(Cell C)
        {
            if (C.IsNull) return C;
            return (C.Affinity == CellAffinity.BSTRING ? new Cell(C.valueBSTRING.ToUpper()) : new Cell(C.valueCSTRING.ToUpper()));
        }

        /// <summary>
        /// Converts a given string to lowercase
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell with a stirng affinity</returns>
        public static Cell ToLower(Cell C)
        {
            if (C.IsNull) return C;
            return (C.Affinity == CellAffinity.BSTRING ? new Cell(C.valueBSTRING.ToLower()) : new Cell(C.valueCSTRING.ToLower()));
        }

        /// <summary>
        /// Returns all characters/bytes left of given point
        /// </summary>
        /// <param name="C">The string or BINARY Value</param>
        /// <param name="Length">The maximum number of chars/bytes</param>
        /// <returns>A string or blob cell</returns>
        public static Cell Left(Cell C, long Length)
        {
            int len = Math.Min(C.AFFINITY == CellAffinity.BINARY ? C.BINARY.Length : C.valueCSTRING.Length, (int)Length);
            return CellFunctions.Substring(C, 0, len);
        }

        /// <summary>
        /// Returns all characters/bytes right of given point
        /// </summary>
        /// <param name="C">The string or BINARY Value</param>
        /// <param name="Length">The maximum number of chars/bytes</param>
        /// <returns>A string or blob cell</returns>
        public static Cell Right(Cell C, long Length)
        {
            int l = C.AFFINITY == CellAffinity.BINARY ? C.BINARY.Length : C.valueCSTRING.Length;
            int begin = Math.Max(l - (int)Length, 0);
            int len = (int)Length;
            if (begin + Length > l) len = l - begin;
            return CellFunctions.Substring(C, begin, len);
        }

        /// <summary>
        /// Checks if a given string contains another string
        /// </summary>
        /// <param name="Source">The string to be checked</param>
        /// <param name="Check">The string being check for</param>
        /// <returns>Cell with boolean type</returns>
        public static Cell Contains(Cell Source, Cell Check)
        {
            return new Cell(Source.valueCSTRING.Contains(Check.valueCSTRING));
        }

        /// <summary>
        /// Returns either the sub stirng or sub blob
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <param name="Position">The starting point</param>
        /// <param name="Length">The maximum length of the new string</param>
        /// <returns>Either a string or blob Value</returns>
        public static Cell Substring(Cell C, long Position, long Length)
        {

            if (C.AFFINITY == CellAffinity.BINARY)
            {
                if (Position + Length > C.BINARY.Length || Position < 0 || Length < 0)
                    return CellValues.NullBLOB;
                byte[] b = new byte[Length];
                Array.Copy(C.BINARY, Position, b, 0, Length);
                C.BINARY = b;
                return C;
            }
            else if (C.AFFINITY == CellAffinity.BSTRING)
            {
                if (Position + Length > C.valueBSTRING.Length || Position < 0 || Length < 0)
                    return CellValues.NullBSTRING;
                return new Cell(C.valueBSTRING.Substring((int)Position, (int)Length));
            }
            else
            {
                if (Position + Length > C.valueCSTRING.Length || Position < 0 || Length < 0)
                    return CellValues.NullCSTRING;
                return new Cell(C.valueCSTRING.Substring((int)Position, (int)Length));
            }

        }

        /// <summary>
        /// Replaces all occurances of a string/BINARY Value with another string/BINARY Value
        /// </summary>
        /// <param name="Source">The string to be searched</param>
        /// <param name="LookFor">The string being searched for</param>
        /// <param name="ReplaceWith">The string that serves as the replacement</param>
        /// <returns>Cell string Value</returns>
        public static Cell Replace(Cell Source, Cell LookFor, Cell ReplaceWith)
        {

            if (CellAffinityHelper.IsVariableLength(Source.Affinity)) return new Cell(Source.AFFINITY);

            if (!(Source.AFFINITY == CellAffinity.BINARY && LookFor.AFFINITY == CellAffinity.BINARY && ReplaceWith.AFFINITY == CellAffinity.BINARY))
            {

                if (Source.Affinity == CellAffinity.BSTRING)
                {
                    Source.BSTRING = Source.valueBSTRING.Replace(LookFor.valueBSTRING, ReplaceWith.valueBSTRING);
                    Source.AFFINITY = CellAffinity.BSTRING;
                    Source.ULONG = 0;
                    return Source;
                }
                else
                {
                    Source.CSTRING = Source.valueCSTRING.Replace(LookFor.valueCSTRING, ReplaceWith.valueCSTRING);
                    Source.AFFINITY = CellAffinity.CSTRING;
                    Source.ULONG = 0;
                    return Source;
                }

            }

            string t = BitConverter.ToString(Source.BINARY);
            string u = BitConverter.ToString(LookFor.BINARY);
            string v = BitConverter.ToString(ReplaceWith.BINARY);
            t = t.Replace(u, v).Replace("-", "");
            Source.BINARY = CellParser.Parse(t, CellAffinity.BINARY).BINARY;
            return Source;

        }

        /// <summary>
        /// Given a pattern and a string or blob, this function will try to seek out the starting position of a patern
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Pattern"></param>
        /// <param name="StartAt"></param>
        /// <returns>Either an integer indicating the position of the pattern or NULL if the pattern wasn't found</returns>
        public static Cell Position(Cell Source, Cell Pattern, int StartAt)
        {

            if (StartAt < 0)
                StartAt = 0;

            if (StartAt > CellSerializer.Length(Source))
                return CellValues.NullINT;

            if (Source.AFFINITY == CellAffinity.CSTRING)
            {
                int idx = Source.CSTRING.IndexOf(Pattern.valueCSTRING, StartAt);
                if (idx == -1)
                    return CellValues.NullINT;
                return new Cell(Source.CSTRING.IndexOf(Pattern.valueCSTRING, StartAt));
            }

            if (Source.AFFINITY == CellAffinity.BSTRING)
            {
                int idx = Source.BSTRING.Find(Pattern.valueBSTRING, StartAt);
                if (idx == -1)
                    return CellValues.NullINT;
                return new Cell(Source.BSTRING.Find(Pattern.valueBSTRING, StartAt));
            }

            if (Source.AFFINITY == CellAffinity.BINARY)
            {

                byte[] data = Source.BINARY;
                byte[] pattern = Pattern.valueBINARY;
                bool match = false;
                for (int i = StartAt; i < data.Length; i++)
                {

                    match = true;
                    for (int j = 0; j < pattern.Length; j++)
                    {
                        if (i + j >= data.Length)
                        {
                            match = false;
                            break;
                        }
                        if (data[i + j] != pattern[j])
                        {
                            match = false;
                            break;
                        }

                    }

                    if (match)
                    {
                        return new Cell(i);
                    }

                }

            }

            return CellValues.NullINT;

        }

        /// <summary>
        /// Converts a byte array to a string using UTF16 encoding
        /// </summary>
        /// <param name="Hash"></param>
        /// <returns></returns>
        internal static string ByteArrayToUTF16String(byte[] Hash)
        {

            byte[] to_convert = Hash;
            if (Hash.Length % 2 != 0)
            {
                to_convert = new byte[Hash.Length + 1];
                Array.Copy(Hash, to_convert, Hash.Length);
            }

            return ASCIIEncoding.BigEndianUnicode.GetString(to_convert);

        }

        /// <summary>
        /// Performs an optimized integer power
        /// </summary>
        /// <param name="Base">The base Value</param>
        /// <param name="Exp">The exponent</param>
        /// <returns>Another integer: Base ^ Exp</returns>
        internal static long IntPower(long Base, long Exp)
        {

            if (Exp == 0)
                return 1;
            else if (Exp == 1)
                return Base;

            if ((Exp % 2) == 1)
                return IntPower(Base * Base, Exp / 2) * Base;
            else
                return IntPower(Base * Base, Exp / 2);

        }

        /// <summary>
        /// Calculates the integer root
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        internal static long IntRoot(long Value)
        {

            if (Value <= 0)
                return 0;

            long x = (Value / 2) + 1;
            long y = (x + Value / x) / 2;
            while (y < x)
            {
                x = y;
                y = (x + Value / x) / 2;
            }

            return x;

        }

        /// <summary>
        /// Gets the index of the highest bit //
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        internal static int HighestBit(long Value)
        {

            long mask = 1;

            for (int i = 0; i < 63; i++)
            {

                if ((mask & Value) == mask)
                    return i;

                mask = mask << 1;

            }

            return 0;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Base"></param>
        /// <param name="Exp"></param>
        /// <param name="Mod"></param>
        /// <returns></returns>
        internal static long LongModPow(long Base, long Exp, long Mod)
        {

            long x = 1;

            while (Exp != 0)
            {
                if ((Exp & 1) == 1)
                {
                    x = (x * Base) % Mod;
                }
                Base = (Base * Base) % Mod;
                Exp >>= 1;
            }

            return x;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="M"></param>
        /// <param name="F"></param>
        /// <returns></returns>
        public static CellMatrix Shadow(CellMatrix M, Func<Cell, Cell> F)
        {
            CellMatrix X = M.Shell();
            for (int i = 0; i < M.RowCount; i++)
            {
                for (int j = 0; j < M.ColumnCount; j++)
                {
                    X[i, j] = F(M[i, j]);
                }
            }
            return X;
        }

    }

}
