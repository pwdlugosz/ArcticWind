using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticWind.Elements
{

    /// <summary>
    /// Support for converting cells to differnt cell types
    /// </summary>
    public static class CellConverter
    {

        /// <summary>
        /// Casts a cell to a given affinity
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Affinity"></param>
        /// <returns></returns>
        public static Cell Cast(Cell Value, CellAffinity Affinity)
        {

            if (Affinity == CellAffinity.VARIANT)
                throw new Exception("Cannot cast to a variant");

            switch (Affinity)
            {
                case CellAffinity.BOOL:
                    return ToBOOL(Value);
                case CellAffinity.DATE_TIME:
                    return ToDATE(Value);
                case CellAffinity.BYTE:
                    return ToBYTE(Value);
                case CellAffinity.SHORT:
                    return ToSHORT(Value);
                case CellAffinity.INT:
                    return ToINT(Value);
                case CellAffinity.LONG:
                    return ToLONG(Value);
                case CellAffinity.SINGLE:
                    return ToFLOAT(Value);
                case CellAffinity.DOUBLE:
                    return ToDOUBLE(Value);
                case CellAffinity.BINARY:
                    return ToBLOB(Value);
                case CellAffinity.BSTRING:
                    return ToBSTRING(Value);
                case CellAffinity.CSTRING:
                    return ToCSTRING(Value);
            }

            throw new Exception(string.Format("Affinity '{0}' is invalid", Affinity));

        }

        /// <summary>
        /// Converts a cell value to a bool cell
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToBOOL(Cell Value)
        {

            if (Value.Affinity == CellAffinity.BOOL)
                return Value;

            if (Value.IsNull)
                return CellValues.NullBOOL;

            if (Value.BINARY == null && Value.AFFINITY == CellAffinity.BINARY)
                return CellValues.NullBOOL;

            if ((Value.CSTRING == null) && (Value.AFFINITY == CellAffinity.CSTRING || Value.AFFINITY == CellAffinity.BSTRING))
                return CellValues.NullBOOL;
    
            switch (Value.AFFINITY)
            {

                case CellAffinity.BYTE:
                    if (Value.BYTE != 0)
                        return CellValues.True;
                    else
                        return CellValues.False;
                case CellAffinity.SHORT:
                    if (Value.SHORT != 0)
                        return CellValues.True;
                    else
                        return CellValues.False;
                case CellAffinity.INT:
                    if (Value.INT != 0)
                        return CellValues.True;
                    else
                        return CellValues.False;
                case CellAffinity.LONG:
                    if (Value.LONG != 0)
                        return CellValues.True;
                    else
                        return CellValues.False;
                case CellAffinity.SINGLE:
                    if (Value.SINGLE != 0)
                        return CellValues.True;
                    else
                        return CellValues.False;
                case CellAffinity.DOUBLE:
                    if (Value.DOUBLE != 0)
                        return CellValues.True;
                    else
                        return CellValues.False;
                case CellAffinity.BINARY:
                    if (Value.BINARY.Length == 0)
                        return CellValues.NullBOOL;
                    if (Value.BINARY[0] != 0)
                        return CellValues.True;
                    else
                        return CellValues.False;
                case CellAffinity.BSTRING:
                    if (BString.CompareStrictIgnoreCase(Value.BSTRING, Cell.TRUE_BSTRING) == 0)
                        Value.BOOL = true;
                    else if (BString.CompareStrictIgnoreCase(Value.BSTRING, Cell.FALSE_BSTRING) == 0)
                        Value.BOOL = false;
                    else
                        return CellValues.NullBOOL;
                    break;
                case CellAffinity.CSTRING:
                    if (StringComparer.OrdinalIgnoreCase.Compare(Value.CSTRING, Cell.TRUE_STRING) == 0)
                        Value.BOOL = true;
                    else if (StringComparer.OrdinalIgnoreCase.Compare(Value.CSTRING, Cell.FALSE_STRING) == 0)
                        Value.BOOL = false;
                    else
                        return CellValues.NullBOOL;
                    break;

            }

            return CellValues.NullBOOL;

        }

        /// <summary>
        /// Converts a cell to a byte value
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToBYTE(Cell Value)
        {

            if (Value.Affinity == CellAffinity.BYTE)
                return Value;

            if (Value.IsNull)
                return CellValues.NullBYTE;

            if (Value.BINARY == null && Value.AFFINITY == CellAffinity.BINARY)
                return CellValues.NullBYTE;

            if (Value.BSTRING == null && Value.AFFINITY == CellAffinity.BSTRING)
                return CellValues.NullBYTE;

            if (Value.CSTRING == null && Value.AFFINITY == CellAffinity.CSTRING)
                return CellValues.NullBYTE;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell((byte)(Value.BOOL ? 1 : 0));
                case CellAffinity.SHORT:
                    return new Cell((byte)(Value.SHORT & byte.MaxValue));
                case CellAffinity.INT:
                    return new Cell((byte)(Value.INT & byte.MaxValue));
                case CellAffinity.DATE_TIME:
                case CellAffinity.LONG:
                    return new Cell((byte)(Value.LONG & byte.MaxValue));
                case CellAffinity.SINGLE:
                    return new Cell((byte)(Value.SINGLE));
                case CellAffinity.DOUBLE:
                    return new Cell((byte)(Value.DOUBLE));
                case CellAffinity.BINARY:
                    if (Value.BINARY.Length == 0)
                        return CellValues.NullBYTE;
                    return new Cell(Value.BINARY[1]);
                case CellAffinity.BSTRING:
                case CellAffinity.CSTRING:
                    byte b = 0;
                    if (byte.TryParse(Value.valueCSTRING, out b))
                        return new Cell(b);
                    else
                        return CellValues.NullBYTE;
            }

            return CellValues.NullBYTE;

        }

        /// <summary>
        /// Converts a value to a short
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToSHORT(Cell Value)
        {

            if (Value.Affinity == CellAffinity.SHORT)
                return Value;

            if (Value.IsNull)
                return CellValues.NullSHORT;

            if (Value.BINARY == null && Value.AFFINITY == CellAffinity.BINARY)
                return CellValues.NullSHORT;

            if (Value.BSTRING == null && Value.AFFINITY == CellAffinity.BSTRING)
                return CellValues.NullSHORT;

            if (Value.CSTRING == null && Value.AFFINITY == CellAffinity.CSTRING)
                return CellValues.NullSHORT;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell((short)(Value.BOOL ? 1 : 0));
                case CellAffinity.BYTE:
                    return new Cell((short)(Value.BYTE));
                case CellAffinity.INT:
                    return new Cell((short)(Value.INT & short.MaxValue));
                case CellAffinity.DATE_TIME:
                case CellAffinity.LONG:
                    return new Cell((short)(Value.LONG & short.MaxValue));
                case CellAffinity.SINGLE:
                    return new Cell((short)(Value.SINGLE));
                case CellAffinity.DOUBLE:
                    return new Cell((short)(Value.DOUBLE));
                case CellAffinity.BINARY:
                    if (Value.BINARY.Length < 2)
                        return CellValues.NullSHORT;
                    return new Cell(BitConverter.ToInt16(Value.BINARY, 0));
                case CellAffinity.BSTRING:
                case CellAffinity.CSTRING:
                    short b = 0;
                    if (short.TryParse(Value.valueCSTRING, out b))
                        return new Cell(b);
                    else
                        return CellValues.NullSHORT;
            }

            return CellValues.NullSHORT;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToINT(Cell Value)
        {

            if (Value.Affinity == CellAffinity.INT)
                return Value;

            if (Value.IsNull)
                return CellValues.NullINT;

            if (Value.BINARY == null && Value.AFFINITY == CellAffinity.BINARY)
                return CellValues.NullINT;

            if (Value.BSTRING == null && Value.AFFINITY == CellAffinity.BSTRING)
                return CellValues.NullINT;

            if (Value.CSTRING == null && Value.AFFINITY == CellAffinity.CSTRING)
                return CellValues.NullINT;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell((int)(Value.BOOL ? 1 : 0));
                case CellAffinity.BYTE:
                    return new Cell((int)(Value.BYTE));
                case CellAffinity.SHORT:
                    return new Cell((int)(Value.SHORT));
                case CellAffinity.DATE_TIME:
                case CellAffinity.LONG:
                    return new Cell((int)(Value.LONG & int.MaxValue));
                case CellAffinity.SINGLE:
                    return new Cell((int)(Value.SINGLE));
                case CellAffinity.DOUBLE:
                    return new Cell((int)(Value.DOUBLE));
                case CellAffinity.BINARY:
                    if (Value.BINARY.Length < 4)
                        return CellValues.NullINT;
                    return new Cell(BitConverter.ToInt32(Value.BINARY, 0));
                case CellAffinity.BSTRING:
                case CellAffinity.CSTRING:
                    int b = 0;
                    if (int.TryParse(Value.valueCSTRING, out b))
                        return new Cell(b);
                    else
                        return CellValues.NullINT;
            }

            return CellValues.NullINT;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToLONG(Cell Value)
        {

            if (Value.Affinity == CellAffinity.LONG)
                return Value;

            if (Value.IsNull)
                return CellValues.NullLONG;

            if (Value.BINARY == null && Value.AFFINITY == CellAffinity.BINARY)
                return CellValues.NullLONG;

            if (Value.BSTRING == null && Value.AFFINITY == CellAffinity.BSTRING)
                return CellValues.NullLONG;

            if (Value.CSTRING == null && Value.AFFINITY == CellAffinity.CSTRING)
                return CellValues.NullLONG;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell((long)(Value.BOOL ? 1 : 0));
                case CellAffinity.BYTE:
                    return new Cell((long)(Value.BYTE));
                case CellAffinity.SHORT:
                    return new Cell((long)(Value.SHORT));
                case CellAffinity.INT:
                    return new Cell((long)(Value.INT));
                case CellAffinity.DATE_TIME:
                    return new Cell((long)(Value.LONG));
                case CellAffinity.SINGLE:
                    return new Cell((long)(Value.SINGLE));
                case CellAffinity.DOUBLE:
                    return new Cell((long)(Value.DOUBLE));
                case CellAffinity.BINARY:
                    if (Value.BINARY.Length < 8)
                        return CellValues.NullLONG;
                    return new Cell(BitConverter.ToInt64(Value.BINARY, 0));
                case CellAffinity.BSTRING:
                case CellAffinity.CSTRING:
                    long b = 0;
                    if (long.TryParse(Value.valueCSTRING, out b))
                        return new Cell(b);
                    else
                        return CellValues.NullLONG;
            }

            return CellValues.NullLONG;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToFLOAT(Cell Value)
        {

            if (Value.Affinity == CellAffinity.SINGLE)
                return Value;

            if (Value.IsNull)
                return CellValues.NullSINGLE;

            if (Value.BINARY == null && Value.AFFINITY == CellAffinity.BINARY)
                return CellValues.NullSINGLE;

            if (Value.BSTRING == null && Value.AFFINITY == CellAffinity.BSTRING)
                return CellValues.NullSINGLE;

            if (Value.CSTRING == null && Value.AFFINITY == CellAffinity.CSTRING)
                return CellValues.NullSINGLE;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell((float)(Value.BOOL ? 1 : 0));
                case CellAffinity.BYTE:
                    return new Cell((float)(Value.BYTE));
                case CellAffinity.SHORT:
                    return new Cell((float)(Value.SHORT));
                case CellAffinity.INT:
                    return new Cell((float)(Value.INT));
                case CellAffinity.DATE_TIME:
                case CellAffinity.LONG:
                    return new Cell((float)(Value.LONG));
                case CellAffinity.SINGLE:
                    return new Cell((float)(Value.SINGLE));
                case CellAffinity.DOUBLE:
                    return new Cell((float)(Value.DOUBLE));
                case CellAffinity.BINARY:
                    if (Value.BINARY.Length < 4)
                        return CellValues.NullSINGLE;
                    return new Cell(BitConverter.ToSingle(Value.BINARY, 0));
                case CellAffinity.BSTRING:
                case CellAffinity.CSTRING:
                    float b = 0;
                    if (float.TryParse(Value.valueCSTRING, out b))
                        return new Cell(b);
                    else
                        return CellValues.NullSINGLE;
            }

            return CellValues.NullSINGLE;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToDOUBLE(Cell Value)
        {

            if (Value.Affinity == CellAffinity.DOUBLE)
                return Value;

            if (Value.IsNull)
                return CellValues.NullDOUBLE;

            if (Value.BINARY == null && Value.AFFINITY == CellAffinity.BINARY)
                return CellValues.NullDOUBLE;

            if (Value.BSTRING == null && Value.AFFINITY == CellAffinity.BSTRING)
                return CellValues.NullDOUBLE;

            if (Value.CSTRING == null && Value.AFFINITY == CellAffinity.CSTRING)
                return CellValues.NullDOUBLE;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell((double)(Value.BOOL ? 1 : 0));
                case CellAffinity.BYTE:
                    return new Cell((double)(Value.BYTE));
                case CellAffinity.SHORT:
                    return new Cell((double)(Value.SHORT));
                case CellAffinity.INT:
                    return new Cell((double)(Value.INT));
                case CellAffinity.DATE_TIME:
                case CellAffinity.LONG:
                    return new Cell((double)(Value.LONG));
                case CellAffinity.SINGLE:
                    return new Cell((double)(Value.SINGLE));
                case CellAffinity.BINARY:
                    if (Value.BINARY.Length < 8)
                        return CellValues.NullDOUBLE;
                    return new Cell(BitConverter.ToDouble(Value.BINARY, 0));
                case CellAffinity.BSTRING:
                case CellAffinity.CSTRING:
                    double b = 0;
                    if (double.TryParse(Value.valueCSTRING, out b))
                        return new Cell(b);
                    else
                        return CellValues.NullDOUBLE;
            }

            return CellValues.NullDOUBLE;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToDATE(Cell Value)
        {

            if (Value.Affinity == CellAffinity.DATE_TIME)
                return Value;

            if (Value.IsNull)
                return CellValues.NullDATE;

            if (Value.BINARY == null && Value.AFFINITY == CellAffinity.BINARY)
                return CellValues.NullDATE;

            if (Value.BSTRING == null && Value.AFFINITY == CellAffinity.BSTRING)
                return CellValues.NullDATE;

            if (Value.CSTRING == null && Value.AFFINITY == CellAffinity.CSTRING)
                return CellValues.NullDATE;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return CellValues.NullDATE;
                case CellAffinity.BYTE:
                    return new Cell(DateTime.FromBinary((long)Value.BYTE));
                case CellAffinity.SHORT:
                    return new Cell(DateTime.FromBinary((long)Value.SHORT));
                case CellAffinity.INT:
                    return new Cell(DateTime.FromBinary((long)Value.INT));
                case CellAffinity.LONG:
                    return new Cell(DateTime.FromBinary((long)Value.LONG));
                case CellAffinity.SINGLE:
                    return new Cell(DateTime.FromBinary((long)Value.SINGLE));
                case CellAffinity.DOUBLE:
                    return new Cell(DateTime.FromBinary((long)Value.DOUBLE));
                case CellAffinity.BINARY:
                    if (Value.BINARY.Length < 8)
                        return CellValues.NullDATE;
                    return new Cell(DateTime.FromBinary(BitConverter.ToInt64(Value.BINARY, 0)));
                case CellAffinity.BSTRING:
                case CellAffinity.CSTRING:
                    return CellParser.DateParse(Value.valueCSTRING);
            }

            return CellValues.NullDATE;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToBLOB(Cell Value)
        {

            if (Value.Affinity == CellAffinity.BINARY)
                return Value;

            if (Value.IsNull)
                return CellValues.NullBLOB;

            if (Value.BINARY == null && Value.AFFINITY == CellAffinity.BINARY)
                return CellValues.NullBLOB;

            if (Value.BSTRING == null && Value.AFFINITY == CellAffinity.BSTRING)
                return CellValues.NullBLOB;

            if (Value.CSTRING == null && Value.AFFINITY == CellAffinity.CSTRING)
                return CellValues.NullBLOB;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell(Value.BOOL ? new byte[1] { 1 } : new byte[1] { 0 });
                case CellAffinity.BYTE:
                    return new Cell(new byte[1] { Value.BYTE });
                case CellAffinity.SHORT:
                    return new Cell(BitConverter.GetBytes(Value.SHORT));
                case CellAffinity.INT:
                    return new Cell(BitConverter.GetBytes(Value.INT));
                case CellAffinity.DATE_TIME:
                case CellAffinity.LONG:
                    return new Cell(BitConverter.GetBytes(Value.LONG));
                case CellAffinity.SINGLE:
                    return new Cell(BitConverter.GetBytes(Value.SINGLE));
                case CellAffinity.DOUBLE:
                    return new Cell(BitConverter.GetBytes(Value.DOUBLE));
                case CellAffinity.BSTRING:
                    return new Cell(Value.BSTRING.ToByteArray);
                case CellAffinity.CSTRING:
                    return new Cell(System.Text.ASCIIEncoding.Unicode.GetBytes(Value.CSTRING));

            }

            return CellValues.NullBLOB;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToBSTRING(Cell Value)
        {

            if (Value.Affinity == CellAffinity.BSTRING)
                return Value;

            if (Value.IsNull)
                return CellValues.NullBSTRING;

            if (Value.BINARY == null && Value.AFFINITY == CellAffinity.BINARY)
                return CellValues.NullBSTRING;

            if (Value.BSTRING == null && Value.AFFINITY == CellAffinity.BSTRING)
                return CellValues.NullBSTRING;

            if (Value.CSTRING == null && Value.AFFINITY == CellAffinity.CSTRING)
                return CellValues.NullBSTRING;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell(new BString(Value.BOOL ? Cell.TRUE_STRING : Cell.FALSE_STRING));
                case CellAffinity.BYTE:
                    return new Cell(new BString(Value.BYTE.ToString()));
                case CellAffinity.SHORT:
                    return new Cell(new BString(Value.SHORT.ToString()));
                case CellAffinity.INT:
                    return new Cell(new BString(Value.INT.ToString()));
                case CellAffinity.LONG:
                    return new Cell(new BString(Value.LONG.ToString()));
                case CellAffinity.DATE_TIME:
                    return new Cell(new BString(Value.DATE.ToString()));
                case CellAffinity.SINGLE:
                    return new Cell(new BString(Value.SINGLE.ToString()));
                case CellAffinity.DOUBLE:
                    return new Cell(new BString(Value.DOUBLE.ToString()));
                case CellAffinity.BINARY:
                    return new Cell(new BString(Value.BINARY));
                case CellAffinity.CSTRING:
                    return new Cell(new BString(Value.CSTRING));
            }

            return CellValues.NullBSTRING;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToCSTRING(Cell Value)
        {

            if (Value.Affinity == CellAffinity.CSTRING)
                return Value;

            if (Value.IsNull)
                return CellValues.NullCSTRING;

            if (Value.BINARY == null && Value.AFFINITY == CellAffinity.BINARY)
                return CellValues.NullCSTRING;

            if (Value.BSTRING == null && Value.AFFINITY == CellAffinity.BSTRING)
                return CellValues.NullCSTRING;

            if (Value.CSTRING == null && Value.AFFINITY == CellAffinity.CSTRING)
                return CellValues.NullCSTRING;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell(Value.BOOL ? Cell.TRUE_STRING : Cell.FALSE_STRING);
                case CellAffinity.BYTE:
                    return new Cell(Value.BYTE.ToString());
                case CellAffinity.SHORT:
                    return new Cell(Value.SHORT.ToString());
                case CellAffinity.INT:
                    return new Cell(Value.INT.ToString());
                case CellAffinity.LONG:
                    return new Cell(Value.LONG.ToString());
                case CellAffinity.DATE_TIME:
                    return new Cell(Value.DATE.ToString());
                case CellAffinity.SINGLE:
                    return new Cell(Value.SINGLE.ToString());
                case CellAffinity.DOUBLE:
                    return new Cell(Value.DOUBLE.ToString());
                case CellAffinity.BINARY:
                    return new Cell(System.Text.ASCIIEncoding.Unicode.GetString(Value.BINARY, 0, Value.BINARY.Length / 2));
                case CellAffinity.BSTRING:
                    return new Cell(Value.CSTRING);
            }

            return CellValues.NullCSTRING;


        }

        public static int CastSizeHelper(CellAffinity Old, CellAffinity New, int PriorSize)
        {

            if (Old == New) 
                return PriorSize;

            bool NewIsFixedLength = !CellAffinityHelper.IsVariableLength(New);
            bool OldIsFixedLength = !CellAffinityHelper.IsVariableLength(Old);

            if (NewIsFixedLength) return CellSerializer.DefaultLength(New);
           
            if (New == CellAffinity.BINARY)
            {

                if (Old == CellAffinity.CSTRING)
                    return PriorSize * 2;
                else if (Old == CellAffinity.BSTRING)
                    return PriorSize;
                else
                    return CellSerializer.DefaultLength(New);

            }
            else if (New == CellAffinity.BSTRING)
            {

                if (Old == CellAffinity.BSTRING || Old == CellAffinity.CSTRING || Old == CellAffinity.BINARY)
                    return PriorSize;
                else
                    return CellValues.Max(Old).ToString().Length;

            }
            else if (New == CellAffinity.CSTRING)
            {

                if (Old == CellAffinity.BSTRING || Old == CellAffinity.CSTRING)
                    return PriorSize;
                else if (Old == CellAffinity.BINARY)
                    return PriorSize / 2;
                else
                    return CellValues.Max(Old).ToString().Length;

            }

            throw new Exception("Invalid combination");

        }

    }

}
