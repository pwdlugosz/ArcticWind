using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticWind.Elements
{

    /// <summary>
    /// Method set that supports converting cell affinities to and from strings
    /// </summary>
    public static class CellAffinityHelper
    {

        public const CellAffinity LOWEST_AFFINITY = CellAffinity.BOOL;
        public const CellAffinity HIGHEST_AFFINITY = CellAffinity.CSTRING;
        public const int AFFINITY_COUNT = 11;

        /// <summary>
        /// Convets text to a cell affinity
        /// </summary>
        /// <param name="BString">String to be parsed</param>
        /// <returns>Cell affinity</returns>
        public static CellAffinity Parse(string Text)
        {

            switch (Text.Trim().ToUpper())
            {

                case "BOOL":
                case "BOOLEAN":
                    return CellAffinity.BOOL;

                case "DATE_TIME":
                    return CellAffinity.DATE_TIME;

                case "BYTE":
                    return CellAffinity.BYTE;

                case "SHORT":
                    return CellAffinity.SHORT;

                case "INT":
                    return CellAffinity.INT;

                case "LONG":
                    return CellAffinity.LONG;

                case "FLOAT":
                case "SINGLE":
                    return CellAffinity.SINGLE;

                case "DOUBLE":
                case "NUM":
                    return CellAffinity.DOUBLE;

                case "BINARY":
                    return CellAffinity.BINARY;

                case "CSTRING":
                    return CellAffinity.CSTRING;

                case "BSTRING":
                    return CellAffinity.BSTRING;

                case "VAR":
                case "VARIANT":
                    return CellAffinity.VARIANT;

                default:
                    throw new Exception("Value is not a valid affinity: " + Text);

            }

        }

        /// <summary>
        /// Convets a cell affinity to a string
        /// </summary>
        /// <param name="Affinity">Cell affinity</param>
        /// <returns>String version of a cell affinity</returns>
        public static string ToString(CellAffinity Affinity)
        {
            return Affinity.ToString();
        }

        /// <summary>
        /// Returns the highest precedence data type
        /// </summary>
        /// <param name="A1">First type to compare</param>
        /// <param name="A2">Second type to compare</param>
        /// <returns>The highest cell precedence</returns>
        public static CellAffinity Highest(CellAffinity A1, CellAffinity A2)
        {
            return (CellAffinity)Math.Max((byte)A1, (byte)A2);
        }

        /// <summary>
        /// Returns the highest precedence data type
        /// </summary>
        /// <param name="Affinity">A collection of cell afffinities</param>
        /// <returns>The highest cell precedence</returns>
        public static CellAffinity Highest(IEnumerable<CellAffinity> Affinity)
        {

            if (Affinity.Count() == 0)
                return CellAffinity.BOOL;
            else if (Affinity.Count() == 1)
                return Affinity.First();

            CellAffinity a = CellAffinity.BOOL;
            foreach (CellAffinity b in Affinity)
            {
                a = CellAffinityHelper.Highest(a, b);
            }
            return a;

        }

        /// <summary>
        /// Returns the lowest precedence data type
        /// </summary>
        /// <param name="A1">First type to compare</param>
        /// <param name="A2">Second type to compare</param>
        /// <returns>The lowest cell precedence</returns>
        public static CellAffinity Lowest(CellAffinity A1, CellAffinity A2)
        {
            return (CellAffinity)Math.Min((byte)A1, (byte)A2);
        }

        /// <summary>
        /// Returns the lowest precedence data type
        /// </summary>
        /// <param name="Affinity">A collection of cell afffinities</param>
        /// <returns>The lowest cell precedence</returns>
        public static CellAffinity Lowest(IEnumerable<CellAffinity> Affinity)
        {

            if (Affinity.Count() == 0)
                return CellAffinity.CSTRING;
            else if (Affinity.Count() == 1)
                return Affinity.First();

            CellAffinity a = CellAffinity.CSTRING;
            foreach (CellAffinity b in Affinity)
            {
                a = CellAffinityHelper.Lowest(a, b);
            }
            return a;

        }

        /// <summary>
        /// Checks if the type can be converted to a ArcticWind type
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static bool IsValidType(Type T)
        {

            if (T == typeof(byte) || T == typeof(ushort) || T == typeof(uint) || T == typeof(ulong))
                return true;
            else if (T == typeof(sbyte) || T == typeof(short) || T == typeof(int) || T == typeof(long))
                return true;
            else if (T == typeof(float) || T == typeof(double))
                return true;
            else if (T == typeof(string))
                return true;
            else if (T == typeof(byte[]))
                return true;
            else if (T == typeof(DateTime))
                return true;
            else if (T == typeof(bool))
                return true;

            return false;

        }

        /// <summary>
        /// Casts a generic type to a pulse type
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static CellAffinity Render(Type T)
        {

            if (T == typeof(byte) || T == typeof(sbyte))
                return CellAffinity.BYTE;
            else if (T == typeof(ushort) || T == typeof(short))
                return CellAffinity.SHORT;
            else if (T == typeof(uint) || T == typeof(int))
                return CellAffinity.INT;
            else if (T == typeof(ulong) || T == typeof(long))
                return CellAffinity.LONG;
            else if (T == typeof(float))
                return CellAffinity.SINGLE;
            else if (T == typeof(double))
                return CellAffinity.DOUBLE;
            else if (T == typeof(string))
                return CellAffinity.CSTRING;
            else if (T == typeof(byte[]))
                return CellAffinity.BINARY;
            else if (T == typeof(DateTime))
                return CellAffinity.DATE_TIME;
            else if (T == typeof(bool))
                return CellAffinity.BOOL;

            return CellAffinity.LONG;

        }

        /// <summary>
        /// Checks if a datatype is either LONG or NUM
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public static bool IsNumeric(CellAffinity A)
        {
            return A == CellAffinity.BYTE || A == CellAffinity.SHORT || A == CellAffinity.INT 
                || A == CellAffinity.LONG || A == CellAffinity.SINGLE || A == CellAffinity.DOUBLE;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public static bool IsFloatingPoint(CellAffinity A)
        {
            return (A == CellAffinity.DOUBLE) || (A == CellAffinity.SINGLE);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public static bool IsIntegral(CellAffinity A)
        {
            return A == CellAffinity.BYTE || A == CellAffinity.SHORT || A == CellAffinity.INT || A == CellAffinity.LONG;
        }

        /// <summary>
        /// Returns true if the type has a variable length
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public static bool IsVariableLength(CellAffinity A)
        {
            return (A == CellAffinity.BINARY || A == CellAffinity.CSTRING || A == CellAffinity.BSTRING);
        }

    }

}
