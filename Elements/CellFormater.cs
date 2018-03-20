using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticWind.Elements
{


    /// <summary>
    /// 
    /// </summary>
    public enum CellFormat : byte
    {

        // Overall //
        NoFormat,

        // Booleans //
        TrueFalse,
        YesNo,
        OnOff,

        // Numerics //
        Round0,
        Round1,
        Round2,
        Round3,
        Round4,
        Round5,
        Money,
        Percent0,
        Percent1,
        Percent2,

        // Dates //
        ShortDate,
        LongDate,

        // BLOBs //
        Base2,
        Base16

    }

    /// <summary>
    /// Formats cells to strings
    /// </summary>
    public static class CellFormater
    {

        public static string Format(Cell Value, CellFormat Format)
        {

            if (Value.IsNull)
                return Cell.NULL_STRING_TEXT;

            switch (Format)
            {

                case CellFormat.NoFormat:
                    return Value.valueCSTRING;
                case CellFormat.TrueFalse:
                    return (Value.valueBOOL ? "TRUE" : "FALSE");
                case CellFormat.YesNo:
                    return (Value.valueBOOL ? "YES" : "NO");
                case CellFormat.OnOff:
                    return (Value.valueBOOL ? "ON" : "OFF");

                case CellFormat.Round0:
                    return Math.Round(Value.valueDOUBLE, 0).ToString();
                case CellFormat.Round1:
                    return Math.Round(Value.valueDOUBLE, 1).ToString();
                case CellFormat.Round2:
                    return Math.Round(Value.valueDOUBLE, 2).ToString();
                case CellFormat.Round3:
                    return Math.Round(Value.valueDOUBLE, 3).ToString();
                case CellFormat.Round4:
                    return Math.Round(Value.valueDOUBLE, 4).ToString();
                case CellFormat.Round5:
                    return Math.Round(Value.valueDOUBLE, 5).ToString();
                case CellFormat.Money:
                    return "$" + Math.Round(Value.valueDOUBLE, 2).ToString();
                case CellFormat.Percent0:
                    return Math.Round(Value.valueDOUBLE * 100D, 0) + "%";
                case CellFormat.Percent1:
                    return Math.Round(Value.valueDOUBLE * 100D, 1) + "%";
                case CellFormat.Percent2:
                    return Math.Round(Value.valueDOUBLE * 100D, 2) + "%";

                case CellFormat.ShortDate:
                    return ToShortDate(Value.valueDATE);
                case CellFormat.LongDate:
                    return ToLongDate(Value.valueDATE);

                case CellFormat.Base2:
                    return BytesToBase2(Value.valueBINARY);
                case CellFormat.Base16:
                    return BytesToBase16(Value.valueBINARY);

            }

            return Value.valueCSTRING;

        }

        public static string Format(Cell Value)
        {

            switch (Value.Affinity)
            {

                case CellAffinity.BOOL: return Format(Value, CellFormat.TrueFalse);
                case CellAffinity.DATE_TIME: return Format(Value, CellFormat.LongDate);
                case CellAffinity.BINARY:
                    return Format(Value, CellFormat.Base16);
                case CellAffinity.SINGLE:
                case CellAffinity.DOUBLE:
                    return Format(Value, CellFormat.Round4);
                default:
                    return Value.valueCSTRING;

            }

        }

        internal static string ToShortDate(DateTime T)
        {
            return string.Format("{0}-{1}-{2}", T.Year.ToString().PadLeft(4, '0'), T.Month.ToString().PadLeft(2, '0'), T.Day.ToString().PadLeft(2, '0'));
        }

        internal static string ToLongDate(DateTime T)
        {
            return string.Format("{0}-{1}-{2}:{3}:{4}:{5}.{6}",
                T.Year.ToString().PadLeft(4, '0'),
                T.Month.ToString().PadLeft(2, '0'),
                T.Day.ToString().PadLeft(2, '0'),
                T.Hour.ToString().PadLeft(2, '0'),
                T.Minute.ToString().PadLeft(2, '0'),
                T.Second.ToString().PadLeft(2, '0'),
                T.Millisecond.ToString().PadLeft(5, '0'));
        }

        internal static string ZeroPad(int Value, int Padding)
        {
            return Value.ToString().PadLeft(Padding, '0');
        }

        internal static string BytesToBase2(byte[] b)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte x in b)
                sb.Append(Convert.ToString(x, 2));
            return sb.ToString();
        }

        internal static string BytesToBase16(byte[] b)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte x in b)
                sb.Append(x.ToString("X"));
            return sb.ToString();
        }


    }

}
