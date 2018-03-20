using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Tables;
using ArcticWind.Expressions;

namespace ArcticWind.Elements
{


    /// <summary>
    /// Converting strings to cells
    /// </summary>
    public static class CellParser
    {

        private static readonly string[] TrueTokens = { "TRUE", "True", "true" };
        private static readonly string[] FalseTokens = { "FALSE", "False", "false" };
        private static readonly string[] NullTokens = { "NULL", "@NULL", "#NULL", "Null", "@Null", "#Null", "null", "@null", "#null", "", "\0" };
        private static readonly char[] IntegralTokens = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private static readonly char[] NumericTokens = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.' };
        private static readonly char[] ByteSuffix = { 'B','b'};
        private static readonly char[] ShortSuffix = { 'S','s'};
        private static readonly char[] IntSuffix = { 'I','i'};
        private static readonly char[] LongSuffix = { 'L','l'};
        private static readonly char[] SingleSuffix = { 'R','r'};
        private static readonly char[] DoubleSuffix = { 'D','d'};
        private static readonly string[] BinaryPrefix = { "0x", "0X" };
        private static readonly char[] ByteStringSuffix = { 'B','b'};
        private static readonly char[] CharacterStringSuffix = { 'C','c'};


        public static Cell Parse(string Value, CellAffinity Affinity)
        {

            if (Affinity == CellAffinity.VARIANT)
                throw new Exception("The variant affinity is invalid");

            if (Value == null || string.Compare(Cell.NULL_STRING_TEXT, Value, true) == 0)
                return CellValues.Null(Affinity);

            switch (Affinity)
            {
                case CellAffinity.BOOL:
                    return ParseBOOL(Value);
                case CellAffinity.DATE_TIME:
                    return ParseDATE(Value);
                case CellAffinity.BYTE:
                    return ParseBYTE(Value);
                case CellAffinity.SHORT:
                    return ParseSHORT(Value);
                case CellAffinity.INT:
                    return ParseINT(Value);
                case CellAffinity.LONG:
                    return ParseLONG(Value);
                case CellAffinity.SINGLE:
                    return ParseSINGLE(Value);
                case CellAffinity.DOUBLE:
                    return ParseDOUBLE(Value);
                case CellAffinity.BINARY:
                    return ParseBINARY(Value);
                case CellAffinity.BSTRING:
                    return ParseBSTRING(Value);
                case CellAffinity.CSTRING:
                    return ParseCSTRING(Value);
            }

            throw new Exception(string.Format("Affinity '{0}' is invalid", Affinity));

        }

        public static Record Parse(string Value, Schema Columns, char Delim, char Escape)
        {
            string[] t = Util.StringUtil.Split(Value, Delim, Escape);
            if (t.Length != Columns.Count)
                throw new Exception();
            RecordBuilder rb = new RecordBuilder();
            for (int i = 0; i < t.Length; i++)
            {
                rb.Add(Parse(t[i], Columns.ColumnAffinity(i)));
            }
            return rb.ToRecord();
        }

        public static Cell ParseBOOL(string Value)
        {

            if (Value == null || NullTokens.Contains(Value))
                return CellValues.NullBOOL;

            if (TrueTokens.Contains(Value))
                return CellValues.True; 
            if (FalseTokens.Contains(Value))
                return CellValues.False;

            return CellValues.NullBOOL;

        }

        public static Cell ParseBYTE(string Value)
        {
            if (Value == null || NullTokens.Contains(Value))
                return CellValues.NullBYTE;

            string t = RemoveSuffix(Value, ByteSuffix);
            byte x = 0;
            if (byte.TryParse(t, out x))
                return new Cell(x);
            return CellValues.NullBYTE;
        }

        public static Cell ParseSHORT(string Value)
        {
            if (Value == null || NullTokens.Contains(Value))
                return CellValues.NullSHORT;

            string t = RemoveSuffix(Value, ShortSuffix);
            short x = 0;
            if (short.TryParse(t, out x))
                return new Cell(x);
            return CellValues.NullSHORT;
        }

        public static Cell ParseINT(string Value)
        {
            if (Value == null || NullTokens.Contains(Value))
                return CellValues.NullINT;

            string t = RemoveSuffix(Value, IntSuffix);
            int x = 0;
            if (int.TryParse(t, out x))
                return new Cell(x);
            return CellValues.NullINT;
        }

        public static Cell ParseLONG(string Value)
        {
            if (Value == null || NullTokens.Contains(Value))
                return CellValues.NullLONG;

            string t = RemoveSuffix(Value, LongSuffix);
            long x = 0;
            if (long.TryParse(t, out x))
                return new Cell(x);
            return CellValues.NullLONG;
        }

        public static Cell ParseSINGLE(string Value)
        {
            if (Value == null || NullTokens.Contains(Value))
                return CellValues.NullSINGLE;

            string t = RemoveSuffix(Value, SingleSuffix);
            float x = 0;
            if (float.TryParse(t, out x))
                return new Cell(x);
            return CellValues.NullSINGLE;
        }

        public static Cell ParseDOUBLE(string Value)
        {
            if (Value == null || NullTokens.Contains(Value))
                return CellValues.NullDOUBLE;

            string t = RemoveSuffix(Value, DoubleSuffix);
            double x = 0;
            if (double.TryParse(t, out x))
                return new Cell(x);
            return CellValues.NullDOUBLE;
        }

        public static Cell ParseDATE(string Value)
        {
            if (Value == null || NullTokens.Contains(Value))
                return CellValues.NullDATE;

            string t = Remove(Value.ToUpper(), " tT'");
            return DateParse(t);
        }

        public static Cell ParseBINARY(string Value)
        {
            if (Value == null || NullTokens.Contains(Value))
                return CellValues.NullBLOB;

            string t = Value.ToUpper().Trim();
            return ByteParse(t);
        }

        public static Cell ParseBSTRING(string Value)
        {
            if (Value == null || NullTokens.Contains(Value))
                return CellValues.NullBSTRING;

            return new Cell(new BString(Value));
        }

        public static Cell ParseCSTRING(string Value)
        {
            if (Value == null || NullTokens.Contains(Value))
                return CellValues.NullCSTRING;

            return new Cell(Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="RemoveChars"></param>
        /// <returns></returns>
        public static string Remove(string Value, string RemoveChars)
        {
            char[] t = RemoveChars.ToCharArray();
            StringBuilder sb = new StringBuilder();
            foreach (char c in Value)
            {
                if (!t.Contains(c)) sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Removes a suffix from a string
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Tokens"></param>
        /// <returns></returns>
        public static string RemoveSuffix(string Value, char[] Tokens)
        {
            if (Value.Length == 0)
                return Value;
            if (Tokens.Contains(Value.Last()))
                return Value.Substring(0, Value.Length - 1);
            return Value;
        }

        /// <summary>
        /// Parses a string into a date time variable with the form YYYY-MM-DD or YYYY-MM-DD:HH:MM:SS or YYYY-MM-DD:HH:MM:SS:LL, where '-' may be '-','\','/', or '#'
        /// </summary>
        /// <param name="Value">The string to be parsed</param>
        /// <returns>A date time cell</returns>
        public static Cell DateParse(string Value)
        {

            char delim = '-';
            if (Value.Contains('-'))
                delim = '-';
            else if (Value.Contains('\\'))
                delim = '\\';
            else if (Value.Contains('/'))
                delim = '/';
            else if (Value.Contains('#'))
                delim = '#';
            else
                throw new FormatException("Expecting the data string to contain either -, \\, / or #");

            string[] s = Value.Replace("'", "").Split(delim, ':', '.', ' ');
            int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0, millisecond = 0;
            if (s.Length == 3)
            {
                year = int.Parse(s[0]);
                month = int.Parse(s[1]);
                day = int.Parse(s[2]);
            }
            else if (s.Length == 6)
            {
                year = int.Parse(s[0]);
                month = int.Parse(s[1]);
                day = int.Parse(s[2]);
                hour = int.Parse(s[3]);
                minute = int.Parse(s[4]);
                second = int.Parse(s[5]);
            }
            else if (s.Length == 7)
            {
                year = int.Parse(s[0]);
                month = int.Parse(s[1]);
                day = int.Parse(s[2]);
                hour = int.Parse(s[3]);
                minute = int.Parse(s[4]);
                second = int.Parse(s[5]);
                millisecond = int.Parse(s[6]);
            }
            else
                return CellValues.NullDATE;

            if (year >= 1 && year <= 9999 && month >= 1 && month <= 12 && day >= 1 && day <= 31 && hour >= 0 && minute >= 0 && second >= 0 && millisecond >= 0)
            {
                return new Cell(new DateTime(year, month, day, hour, minute, second, millisecond));
            }
            return CellValues.NullDATE;

        }

        /// <summary>
        /// Converts a hex literal string '0x0000' to a byte array
        /// </summary>
        /// <param name="Value">Hexidecimal string</param>
        /// <returns>Byte array</returns>
        public static Cell ByteParse(string Value)
        {

            if (Value.Length == 0 || BinaryPrefix.Contains(Value))
                return new Cell(CellAffinity.BINARY);



            Value = Value.Replace("0x", "").Replace("0X", "");
            byte[] b = new byte[(Value.Length) / 2];

            for (int i = 0; i < Value.Length; i += 2)
                b[i / 2] = Convert.ToByte(Value.Substring(i, 2), 16);

            return new Cell(b);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static string Clean(string Value)
        {

            if (Value.Length < 2)
                return Value;

            if (Value.Last() == 'C' || Value.Last() == 'c' || Value.Last() == 'B' || Value.Last() == 'b')
                Value = Value.Substring(0, Value.Length - 1);

            if (Value.First() == '\'' && Value.Last() == '\'')
                return Value.Substring(1, Value.Length - 2);

            if (Value.First() == '"' && Value.Last() == '"')
                return Value.Substring(1, Value.Length - 2);

            if (Value.Length < 4)
                return Value;

            if (Value[0] == '$' && Value[1] == '$' && Value[Value.Length - 2] == '$' && Value[Value.Length - 1] == '$')
                return Value.Substring(2, Value.Length - 4);

            return Value;

        }



    }


}
