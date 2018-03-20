using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ArcticWind.Elements
{

    /// <summary>
    /// The basic unit of in memory data
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit)]
    public struct Cell : IComparable<Cell>, IComparer<Cell>
    {

        // Cell constants //
        public const String NULL_STRING_TEXT = "@@NULL"; // the null Value text
        public const string HEX_LITERARL = "0x"; // the expected qualifier for a hex string 
        public const int MAX_STRING_LENGTH = 64 * 1024 * 1024; // maximum length of a string, 64k

        // Cell internal statics //
        internal static int NUMERIC_ROUNDER = 5; // used for rounding double values 
        internal static int DATE_FORMAT = 1; // 0 = full date time, 1 = date only, 2 = time only
        internal static string TRUE_STRING = "TRUE";
        internal static string FALSE_STRING = "FALSE";
        internal static BString TRUE_BSTRING = "TRUE";
        internal static BString FALSE_BSTRING = "FALSE";

        // Static common cells //

        #region Runtime_Variables

        /* Offset:      0   1   2   3   4   5   6   7   8   9   10  11  12  13  14  15
         * 
         * NullFlag     OriginalPage
         * Affinity         OriginalPage
         * INT64                OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage
         * DATE_TIME                 OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage
         * DOUBLE               OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage
         * BOOL                 OriginalPage
         * CSTRING                                                       OriginalPage   OriginalPage   OriginalPage   OriginalPage
         * BINARY                                                         OriginalPage   OriginalPage   OriginalPage   OriginalPage
         * INT32A               OriginalPage   OriginalPage   OriginalPage   OriginalPage   
         * INT32B                               OriginalPage   OriginalPage   OriginalPage   OriginalPage
         * ULONG                OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage
         * 
         */

        // Metadata elements //
        /// <summary>
        /// The cell affinity, offset 9
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(9)]
        internal CellAffinity AFFINITY;

        /// <summary>
        /// The null byte indicator, offset 10
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(10)]
        internal byte NULL;

        // Elements variables //
        /// <summary>
        /// The .Net bool Value, offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal bool BOOL;

        /// <summary>
        /// The .Net short Value, offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal byte BYTE;

        /// <summary>
        /// The .Net short Value, offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal short SHORT;

        /// <summary>
        /// The .Net int Value, offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal int INT;

        /// <summary>
        /// The .Net long Value, offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal long LONG;

        /// <summary>
        /// The .Net float Value, offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal float SINGLE;

        /// <summary>
        /// The .Net double Value, offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal double DOUBLE;

        /// <summary>
        /// The .Net DateTime variable, offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal DateTime DATE;

        /// <summary>
        /// Represents an immutable 8-bit string
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(12)]
        internal BString BSTRING;

        /// <summary>
        /// The .Net string variable, offset 12
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(12)]
        internal string CSTRING;

        /// <summary>
        /// The .Net byte[] variable, offset 12
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(12)]
        internal byte[] BINARY;

        // Extended elements //
        /// <summary>
        /// The .Net integer Value at offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal int INT_A;

        /// <summary>
        /// The .Net integer Value at offset 4
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(4)]
        internal int INT_B;

        /// <summary>
        /// The .Net float Value at offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal float FLOAT_A;

        /// <summary>
        /// The .Net float Value at offset 4
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(4)]
        internal float FLOAT_B;

        /// <summary>
        /// The .Net ulong Value at offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal ulong ULONG;

        [System.Runtime.InteropServices.FieldOffset(0)]
        internal byte B0;

        [System.Runtime.InteropServices.FieldOffset(1)]
        internal byte B1;

        [System.Runtime.InteropServices.FieldOffset(2)]
        internal byte B2;

        [System.Runtime.InteropServices.FieldOffset(3)]
        internal byte B3;

        [System.Runtime.InteropServices.FieldOffset(4)]
        internal byte B4;

        [System.Runtime.InteropServices.FieldOffset(5)]
        internal byte B5;

        [System.Runtime.InteropServices.FieldOffset(6)]
        internal byte B6;

        [System.Runtime.InteropServices.FieldOffset(7)]
        internal byte B7;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a boolean cell
        /// </summary>
        /// <param name="Value">A .Net bool</param>
        public Cell(bool Value)
            : this()
        {
            this.BOOL = Value;
            this.AFFINITY = CellAffinity.BOOL;
            this.NULL = 0;
        }

        /// <summary>
        /// Creates a 8 bit integer cell
        /// </summary>
        /// <param name="Value">A .Net long or int8</param>
        public Cell(byte Value)
            : this()
        {

            this.BYTE = Value;
            this.AFFINITY = CellAffinity.BYTE;
            this.NULL = 0;
        }

        /// <summary>
        /// Creates a 16 bit integer cell
        /// </summary>
        /// <param name="Value">A .Net long or int16</param>
        public Cell(short Value)
            : this()
        {

            this.SHORT = Value;
            this.AFFINITY = CellAffinity.SHORT;
            this.NULL = 0;
        }

        /// <summary>
        /// Creates a 32 bit integer cell
        /// </summary>
        /// <param name="Value">A .Net long or int32</param>
        public Cell(int Value)
            : this()
        {

            this.INT = Value;
            this.AFFINITY = CellAffinity.INT;
            this.NULL = 0;
        }

        /// <summary>
        /// Creates a 64 bit integer cell
        /// </summary>
        /// <param name="Value">A .Net long or int64</param>
        public Cell(long Value)
            : this()
        {

            this.LONG = Value;
            this.AFFINITY = CellAffinity.LONG;
            this.NULL = 0;
        }

        /// <summary>
        /// Creates a 32 bit numeric cell
        /// </summary>
        /// <param name="Value">A .Net double or Double</param>
        public Cell(float Value)
            : this()
        {
            this.SINGLE = Value;
            this.AFFINITY = CellAffinity.SINGLE;
            this.NULL = 0;
        }

        /// <summary>
        /// Creates a 64 bit numeric cell
        /// </summary>
        /// <param name="Value">A .Net double or Double</param>
        public Cell(double Value)
            : this()
        {
            this.DOUBLE = Value;
            this.AFFINITY = CellAffinity.DOUBLE;
            this.NULL = 0;
        }

        /// <summary>
        /// Creates a 64 bit date-time cell
        /// </summary>
        /// <param name="Value">A .Net DateTime</param>
        public Cell(DateTime Value)
            : this()
        {
            this.DATE = Value;
            this.AFFINITY = CellAffinity.DATE_TIME;
            this.NULL = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        public Cell(BString Value)
            : this()
        {

            this.AFFINITY = CellAffinity.BSTRING;

            // Handle null strings //
            if (Value == null)
            {
                this.BSTRING = BString.Empty;
                this.NULL = 1;
                return;
            }

            // Fix the values
            if (Value.Length == 0) // fix instances that are zero length
                Value = BString.Empty;
            else if (Value.Length >= MAX_STRING_LENGTH) // Fix strings that are too long
                Value = Value.Substring(0, MAX_STRING_LENGTH);

            this.BSTRING = Value;
            this.NULL = 0;

            this.INT_A = Value.GetHashCode();
            this.INT_B = Value.Length;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        public Cell(string Value)
            : this()
        {

            // Set the affinity //
            this.AFFINITY = CellAffinity.CSTRING;

            // Handle null strings //
            if (Value == null)
            {
                this.CSTRING = "\0";
                this.NULL = 1;
                return;
            }

            // Fix the values
            if (Value.Length == 0) // fix instances that are zero length
                Value = "\0";
            else if (Value.Length >= MAX_STRING_LENGTH) // Fix strings that are too long
                Value = Value.Substring(0, MAX_STRING_LENGTH);

            this.CSTRING = Value;
            this.NULL = 0;

            this.INT_A = Value.GetHashCode();
            this.INT_B = Value.Length;

        }

        /// <summary>
        /// Creats a BINARY cell
        /// </summary>
        /// <param name="Value">A .Net array of bytes</param>
        public Cell(byte[] Value)
            : this()
        {
            this.BINARY = Value;
            this.NULL = 0;
            this.AFFINITY = CellAffinity.BINARY;
            for (int i = 0; i < Value.Length; i++)
                this.INT_A += Value[i] * i;
            this.INT_A = this.INT_A ^ Value.Length;
            this.INT_B = Value.Length;
        }

        /// <summary>
        /// Creates a cell of a given affinity that is null
        /// </summary>
        /// <param name="Type">An affinity of the new cell</param>
        public Cell(CellAffinity Type)
            : this()
        {

            if (Type == CellAffinity.VARIANT)
                throw new Exception("Cannot create a null variant cell");

            this.AFFINITY = Type;
            this.NULL = 1;
            if (Type == CellAffinity.CSTRING)
                this.CSTRING = "";
            else if (Type == CellAffinity.BSTRING)
                this.BSTRING = BString.Empty;
            else if (Type == CellAffinity.BINARY)
                this.BINARY = new byte[0];
        }

        // -- Auto Casts -- //
        /// <summary>
        /// Creates a 64 bit integer cell
        /// </summary>
        /// <param name="ValueA">A .Net 32 bit integer that will make up the first 4 bytes of integer</param>
        /// <param name="ValueB"></param>
        internal Cell(int ValueA, int ValueB)
            : this()
        {

            // Set these values //
            this.INT_A = ValueA;
            this.INT_B = ValueB;
            this.AFFINITY = CellAffinity.LONG;
            this.NULL = 0;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Affinity"></param>
        internal Cell(ulong Value, CellAffinity Affinity)
            : this()
        {
            this.AFFINITY = Affinity;
            this.ULONG = Value;
            this.NULL = 0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The cell's Spike affinity
        /// </summary>
        public CellAffinity Affinity
        {
            get { return this.AFFINITY; }
        }

        /// <summary>
        /// True == null, False == not null
        /// </summary>
        public bool IsNull
        {
            get { return NULL == 1; }
        }

        /// <summary>
        /// True if the numeric Value is 0 or if the variable length Value has a zero length
        /// </summary>
        public bool IsZero
        {
            get
            {
                if (this.IsNull) return false;
                switch (this.Affinity)
                {
                    case CellAffinity.BYTE:
                    case CellAffinity.SHORT:
                    case CellAffinity.INT:
                    case CellAffinity.LONG: return this.LONG == 0;
                    case CellAffinity.SINGLE:
                    case CellAffinity.DOUBLE: return this.DOUBLE == 0;
                    case CellAffinity.BOOL: return !this.BOOL;
                    case CellAffinity.BSTRING: return this.BSTRING.Length == 0;
                    case CellAffinity.CSTRING: return this.CSTRING.Length == 0;
                    case CellAffinity.BINARY: return this.BINARY.Length == 0;
                    default: return false;
                }
            }

        }

        /// <summary>
        /// Returns true if the integer Value or double Value is 1, or if the boolean is true, false otherwise
        /// </summary>
        public bool IsOne
        {

            get
            {
                if (this.IsNull) return false;
                switch (this.Affinity)
                {
                    case CellAffinity.BYTE:
                    case CellAffinity.SHORT:
                    case CellAffinity.INT:
                    case CellAffinity.LONG: return this.LONG == 1;
                    case CellAffinity.SINGLE:
                    case CellAffinity.DOUBLE: return this.DOUBLE == 1;
                    case CellAffinity.BOOL: return this.BOOL;
                    default: return false;
                }
            }

        }

        /// <summary>
        /// Returns a hash Value as a long integer
        /// </summary>
        public long LASH
        {

            get
            {

                if (this.NULL == 1)
                {
                    return 0L;
                }
                else if (this.AFFINITY != CellAffinity.CSTRING && this.AFFINITY != CellAffinity.BINARY)
                {
                    return this.LONG;
                }
                else if (this.AFFINITY == CellAffinity.CSTRING)
                {
                    
                    long l = 0;
                    for (int i = 0; i < this.CSTRING.Length; i++)
                    {
                        l += (i + 1) * (this.CSTRING[i] + 1);
                    }
                    return l;

                }
                else if (this.AFFINITY == CellAffinity.BSTRING)
                {

                    long l = 0;
                    for (int i = 0; i < this.BSTRING.Length; i++)
                    {
                        l += (i + 1) * (this.BSTRING[i] + 1);
                    }
                    return l;

                }
                else if (this.AFFINITY == CellAffinity.BINARY)
                {

                    long l = 0;
                    for (int i = 0; i < this.BINARY.Length; i++)
                    {
                        l += (i + 1) * (this.BINARY[i] + 1);
                    }
                    return l;

                }

                return 0;

            }

        }

        /// <summary>
        /// Returns the length of the cell
        /// </summary>
        public int Length
        {
            get
            {

                int len = 0;
                if (this.AFFINITY == CellAffinity.CSTRING && this.CSTRING != null)
                    len = this.CSTRING.Length;
                if (this.AFFINITY == CellAffinity.BSTRING && this.BSTRING != null)
                    len = this.BSTRING.Length;
                else if (this.AFFINITY == CellAffinity.BINARY && this.BINARY != null)
                    len = this.BINARY.Length;

                return CellSerializer.Length(this.AFFINITY, len);
                    
            }
        }

        #endregion

        #region SafeValues

        /// <summary>
        /// Returns the bool Value if the affinity is 'BOOL', true if the LONG property is 0, false otherwise
        /// </summary>
        public bool valueBOOL
        {
            get
            {
                if (this.AFFINITY == CellAffinity.BOOL) return this.BOOL;
                return this.LONG == 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte valueBYTE
        {
            get
            {

                if (this.AFFINITY == CellAffinity.BYTE)
                    return this.BYTE;

                if (this.AFFINITY == CellAffinity.SHORT)
                    return (byte)(this.SHORT & 255);
                if (this.AFFINITY == CellAffinity.INT)
                    return (byte)(this.INT & 255);
                if (this.AFFINITY == CellAffinity.LONG || this.AFFINITY == CellAffinity.DATE_TIME)
                    return (byte)(this.LONG & 255);
                if (this.AFFINITY == CellAffinity.SINGLE)
                    return (byte)(this.SINGLE);
                if (this.AFFINITY == CellAffinity.DOUBLE)
                    return (byte)this.DOUBLE;
                if (this.AFFINITY == CellAffinity.BOOL)
                    return this.BOOL ? (byte)1 : (byte)0;

                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public short valueSHORT
        {
            get
            {

                if (this.AFFINITY == CellAffinity.SHORT)
                    return this.SHORT;

                if (this.AFFINITY == CellAffinity.BYTE)
                    return (short)this.BYTE;
                if (this.AFFINITY == CellAffinity.INT)
                    return (short)(this.INT & 255);
                if (this.AFFINITY == CellAffinity.LONG || this.AFFINITY == CellAffinity.DATE_TIME)
                    return (short)(this.LONG & 255);
                if (this.AFFINITY == CellAffinity.SINGLE)
                    return (short)(this.SINGLE);
                if (this.AFFINITY == CellAffinity.DOUBLE)
                    return (short)this.DOUBLE;
                if (this.AFFINITY == CellAffinity.BOOL)
                    return this.BOOL ? (short)1 : (short)0;

                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int valueINT
        {
            get
            {
                
                if (this.AFFINITY == CellAffinity.INT)
                    return this.INT;

                if (this.AFFINITY == CellAffinity.BYTE)
                    return (int)this.BYTE;
                if (this.AFFINITY == CellAffinity.SHORT)
                    return (int)this.SHORT;
                if (this.AFFINITY == CellAffinity.LONG || this.AFFINITY == CellAffinity.DATE_TIME)
                    return (int)(this.LONG & 255);
                if (this.AFFINITY == CellAffinity.SINGLE)
                    return (int)(this.SINGLE);
                if (this.AFFINITY == CellAffinity.DOUBLE)
                    return (int)this.DOUBLE;
                if (this.AFFINITY == CellAffinity.BOOL)
                    return this.BOOL ? (int)1 : (int)0;

                return 0;
            }
        }

        /// <summary>
        /// Return the LONG Value if the affinity is LONG, casts the DOUBLE as an LONG if the affinity is a DOUBLE, 0 otherwise
        /// </summary>
        public long valueLONG
        {
            get
            {

                if (this.AFFINITY == CellAffinity.LONG || this.AFFINITY == CellAffinity.DATE_TIME)
                    return this.LONG;

                if (this.AFFINITY == CellAffinity.BYTE)
                    return (long)this.BYTE;
                if (this.AFFINITY == CellAffinity.SHORT)
                    return (long)this.SHORT;
                if (this.AFFINITY == CellAffinity.INT)
                    return (long)this.INT;
                if (this.AFFINITY == CellAffinity.SINGLE)
                    return (long)(this.SINGLE);
                if (this.AFFINITY == CellAffinity.DOUBLE)
                    return (long)this.DOUBLE;
                if (this.AFFINITY == CellAffinity.BOOL)
                    return this.BOOL ? (long)1 : (long)0;

                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public float valueSINGLE
        {
            get
            {
                if (this.AFFINITY == CellAffinity.SINGLE)
                    return this.SINGLE;

                if (this.AFFINITY == CellAffinity.BYTE)
                    return (float)this.BYTE;
                if (this.AFFINITY == CellAffinity.SHORT)
                    return (float)this.SHORT;
                if (this.AFFINITY == CellAffinity.INT)
                    return (float)this.INT;
                if (this.AFFINITY == CellAffinity.LONG || this.AFFINITY == CellAffinity.DATE_TIME)
                    return (float)this.LONG;
                if (this.AFFINITY == CellAffinity.DOUBLE)
                    return (float)this.DOUBLE;
                if (this.AFFINITY == CellAffinity.BOOL)
                    return this.BOOL ? (float)1 : (float)0;

                return 0;

            }
        }

        /// <summary>
        /// Return the DOUBLE Value if the affinity is DOUBLE, casts the LONG as an DOUBLE if the affinity is a LONG, 0 otherwise
        /// </summary>
        public double valueDOUBLE
        {
            get
            {

                if (this.AFFINITY == CellAffinity.DOUBLE)
                    return this.DOUBLE;
                
                if (this.AFFINITY == CellAffinity.BYTE)
                    return (double)this.BYTE;
                if (this.AFFINITY == CellAffinity.SHORT)
                    return (double)this.SHORT;
                if (this.AFFINITY == CellAffinity.INT)
                    return (double)this.INT;
                if (this.AFFINITY == CellAffinity.LONG || this.AFFINITY == CellAffinity.DATE_TIME)
                    return (double)this.LONG;
                if (this.AFFINITY == CellAffinity.SINGLE)
                    return (double)this.SINGLE;
                if (this.AFFINITY == CellAffinity.BOOL)
                    return this.BOOL ? (double)1 : (double)0;

                return 0;

            }
        }

        /// <summary>
        /// Returns the Spike DATE_TIME if the affinity is DATE_TIME, otherwise return the minimum date time .Net Value
        /// </summary>
        public DateTime valueDATE
        {
            get
            {
                if (this.Affinity == CellAffinity.DATE_TIME) return this.DATE;
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public BString valueBSTRING
        {
            get
            {

                if (this.IsNull)
                    return new BString(Cell.NULL_STRING_TEXT);

                switch (this.Affinity)
                {

                    case CellAffinity.BYTE:
                        return new BString(this.BYTE.ToString());
                    case CellAffinity.SHORT:
                        return new BString(this.SHORT.ToString());
                    case CellAffinity.INT:
                        return new BString(this.INT.ToString());
                    case CellAffinity.LONG:
                        return new BString(this.LONG.ToString());

                    case CellAffinity.SINGLE:
                        return new BString(Math.Round(this.SINGLE, NUMERIC_ROUNDER).ToString());
                    case CellAffinity.DOUBLE:
                        return new BString(Math.Round(this.DOUBLE, NUMERIC_ROUNDER).ToString());

                    case CellAffinity.BOOL:
                        return new BString(this.BOOL ? TRUE_STRING : FALSE_STRING);

                    case CellAffinity.DATE_TIME:
                        return new BString(CellFormater.ToLongDate(this.DATE));

                    case CellAffinity.BSTRING:
                        return this.BSTRING;
                    case CellAffinity.CSTRING:
                        return new BString(this.CSTRING);

                    case CellAffinity.BINARY:
                        return new BString(HEX_LITERARL + BitConverter.ToString(this.BINARY).Replace("-", ""));

                    default:
                        return BString.Empty;

                }

            }

        }

        /// <summary>
        /// If the cell is null, returns '@@NULL'; otherwise, casts the Value as a string
        /// </summary>
        public string valueCSTRING
        {
            get
            {

                if (this.IsNull)
                    return Cell.NULL_STRING_TEXT;

                switch (this.Affinity)
                {

                    case CellAffinity.BYTE:
                        return this.BYTE.ToString();
                    case CellAffinity.SHORT:
                        return this.SHORT.ToString();
                    case CellAffinity.INT:
                        return this.INT.ToString();
                    case CellAffinity.LONG:
                        return this.LONG.ToString();

                    case CellAffinity.SINGLE:
                        return Math.Round(this.SINGLE, NUMERIC_ROUNDER).ToString();
                    case CellAffinity.DOUBLE:
                        return Math.Round(this.DOUBLE, NUMERIC_ROUNDER).ToString();

                    case CellAffinity.BOOL:
                        return this.BOOL ? TRUE_STRING : FALSE_STRING;

                    case CellAffinity.DATE_TIME:
                        return CellFormater.ToShortDate(this.DATE);

                    case CellAffinity.BSTRING:
                        return this.BSTRING.ToString();
                    case CellAffinity.CSTRING:
                        return this.CSTRING;

                    case CellAffinity.BINARY:
                        return HEX_LITERARL + BitConverter.ToString(this.BINARY).Replace("-", "");

                    default:
                        return "";

                }

            }

        }

        /// <summary>
        /// If the affinity is null, returns an empty byte array; if the Value is a BINARY, returns the BINARY; if the Value is a stirng, returns the string as a byte array, unless the string has a hex prefix, then it converts the hex string to a byte array; otherwise it converts an LONG, DOUBLE, BOOL to a byte array.
        /// </summary>
        public byte[] valueBINARY
        {
            get
            {

                if (this.AFFINITY == CellAffinity.BINARY)
                    return this.NULL == 1 ? new byte[0] : this.BINARY;

                if (this.AFFINITY == CellAffinity.BOOL)
                    return this.BOOL == true ? new byte[1] { 1 } : new byte[1] { 0 };
                else if (this.AFFINITY == CellAffinity.BYTE)
                    return new byte[1] { this.BYTE };
                else if (this.AFFINITY == CellAffinity.SHORT)
                    return BitConverter.GetBytes(this.SHORT);
                else if (this.AFFINITY == CellAffinity.SINGLE || this.AFFINITY == CellAffinity.INT)
                    return BitConverter.GetBytes(this.INT);
                else if (this.AFFINITY == CellAffinity.DATE_TIME || this.AFFINITY == CellAffinity.LONG || this.AFFINITY == CellAffinity.DOUBLE)
                    return BitConverter.GetBytes(this.LONG);
                else if (this.AFFINITY == CellAffinity.BSTRING)
                    return this.BSTRING.ToByteArray;
                else // CSTRING
                    return ASCIIEncoding.BigEndianUnicode.GetBytes(this.CSTRING);

            }
        }

        /// <summary>
        /// Gets the Value of the cell as an object
        /// </summary>
        public object valueObject
        {

            get
            {
                switch(this.AFFINITY)
                {
                    case CellAffinity.BOOL: return this.BOOL;
                    case CellAffinity.BYTE: return this.BYTE;
                    case CellAffinity.SHORT: return this.SHORT;
                    case CellAffinity.INT: return this.INT;
                    case CellAffinity.LONG: return this.LONG;
                    case CellAffinity.SINGLE: return this.SINGLE;
                    case CellAffinity.DOUBLE: return this.DOUBLE;
                    case CellAffinity.DATE_TIME: return this.DATE;
                    case CellAffinity.BSTRING:
                    case CellAffinity.CSTRING: return this.CSTRING;
                    case CellAffinity.BINARY: return this.BINARY;
                    
                }

                return CellValues.NullINT;
            }

        }

        #endregion

        #region Overrides

        /// <summary>
        /// Returns the valueString property
        /// </summary>
        /// <returns>A string reprsentation of a cell</returns>
        public override string ToString()
        {

            // Check if null //
            if (this.IsNull == true) return Cell.NULL_STRING_TEXT;

            return this.valueCSTRING;

        }

        /// <summary>
        /// Casts an object as a cell then compares it to the Spike instance
        /// </summary>
        /// <param name="obj">The object being tested for equality</param>
        /// <returns>A boolean indicating both objects are equal</returns>
        public override bool Equals(object obj)
        {
            return CellComparer.Compare(this, (Cell)obj) == 0;
        }

        /// <summary>
        /// If null, return int.MinValue, for LONG, DOUBLE, BOOL, and DATE_TIME, return INT_A; for blobs, returns the sum of all bytes; for strings, returns the sum of the (i + 1) OriginalPage char[i]
        /// </summary>
        /// <returns>An integer hash code</returns>
        public override int GetHashCode()
        {

            if (this.NULL == 1)
                return int.MinValue;

            if (this.Affinity != CellAffinity.CSTRING && this.AFFINITY != CellAffinity.BINARY)
                return this.INT_A;

            if (this.AFFINITY == CellAffinity.BINARY)
                return this.BINARY.Sum<byte>((x) => { return (int)x; });

            int t = 0;
            for (int i = 0; i < this.CSTRING.Length; i++)
                t += (i + 1) * this.CSTRING[i];

            return t;

        }

        #endregion

        #region Operators

        /*
         * if any of the below say they 'dont work', that just means they return a null Value
         * 
         * All opperations returning a Cell will return a null Cell if either A or B are null (the affinity based on A)
         * +: add, returns null for date, blob and boolean; returns concatenate for strings
         * -: subtract, returns null for boolean and blob and string; resturn a long for ticks difference for dates
         * *: multiply, returns null for date, blob, string and boolean
         * /: divide, returns null for date, blob, string and boolean
         * %: mod, returns null for date, blob, string, boolean, and double
         * ^: xor, works for all types, may return nonsense for double/datetime, good for encrypting strings
         * &: and, works for all types, may return nonsense for double/datetime, good for encrypting strings
         * |: or, works for all types, may return nonsense for double/datetime, good for encrypting strings
         * ==: equals, works for all types
         * !=: not equals, works for all types
         * <: less than, works for all types
         * <=: less than or equals, works for all types
         * >: greater than, works for all types
         * >=: greater than or equals, works for all types
         * true/false: all types
         * ++/--: only for numerics
         */

        /// <summary>
        /// Performs the 'NOT' opperation, will return for null for DATE_TIME, CSTRING, and BLOBs
        /// </summary>
        /// <param name="C">A cell</param>
        /// <returns>A cell</returns>
        public static Cell operator !(Cell C)
        {

            // Check nulls //
            if (C.NULL == 1)
                return C;

            if (C.AFFINITY == CellAffinity.SINGLE)
                C.SINGLE = -C.SINGLE;
            else if (C.AFFINITY == CellAffinity.DOUBLE)
                C.DOUBLE = -C.DOUBLE;
            else if (C.AFFINITY == CellAffinity.SHORT)
                C.SHORT = (short)(-C.SHORT);
            else if (C.AFFINITY == CellAffinity.INT)
                C.INT = -C.INT;
            else if (C.AFFINITY == CellAffinity.LONG)
                C.LONG = -C.LONG;
            else if (C.AFFINITY == CellAffinity.BOOL)
                C.BOOL = !C.BOOL;
            else
                C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Adds two cells together for LONG and DOUBLE, concatentates strings, returns null otherwise
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Value cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator +(Cell C1, Cell C2)
        {

            // Check nulls //
            if (C1.NULL == 1)
                return C1;
            else if (C2.NULL == 1)
                return C2;

            // If affinities match //
            if (C1.AFFINITY == C2.AFFINITY)
            {

                if (C1.AFFINITY == CellAffinity.DOUBLE)
                {
                    C1.DOUBLE += C2.DOUBLE;
                }
                else if (C1.AFFINITY == CellAffinity.SINGLE)
                {
                    C1.SINGLE += C2.SINGLE;
                }
                else if (C1.AFFINITY == CellAffinity.BYTE)
                {
                    C1.BYTE += C2.BYTE;
                }
                else if (C1.AFFINITY == CellAffinity.SHORT)
                {
                    C1.SHORT += C2.SHORT;
                }
                else if (C1.AFFINITY == CellAffinity.INT)
                {
                    C1.INT += C2.INT;
                }
                else if (C1.AFFINITY == CellAffinity.LONG)
                {
                    C1.LONG += C2.LONG;
                }
                else if (C1.AFFINITY == CellAffinity.BSTRING)
                {
                    C1.BSTRING += C2.BSTRING;
                }
                else if (C1.AFFINITY == CellAffinity.CSTRING)
                {
                    C1.CSTRING += C2.CSTRING;
                }
                else if (C1.AFFINITY == CellAffinity.BINARY)
                {
                    byte[] b = new byte[C1.BINARY.Length + C2.BINARY.Length];
                    Array.Copy(C1.BINARY, 0, b, 0, C1.BINARY.Length);
                    Array.Copy(C2.BINARY, 0, b, C1.BINARY.Length, C2.BINARY.Length);
                    C1 = new Cell(b);
                }
                else
                    C1.NULL = 1;

            }
            else
            {

                if (C1.AFFINITY == CellAffinity.CSTRING || C2.AFFINITY == CellAffinity.CSTRING)
                {
                    C1.CSTRING = C1.valueCSTRING + C2.valueCSTRING;
                    C1.AFFINITY = CellAffinity.CSTRING;
                }
                else if (C1.AFFINITY == CellAffinity.BSTRING || C2.AFFINITY == CellAffinity.BSTRING)
                {
                    C1.BSTRING = C1.valueBSTRING + C2.valueBSTRING;
                    C1.AFFINITY = CellAffinity.BSTRING;
                }
                else if (C1.AFFINITY == CellAffinity.BINARY || C2.AFFINITY == CellAffinity.BINARY)
                {
                    byte[] b = new byte[C1.valueBINARY.Length + C2.valueBINARY.Length];
                    Array.Copy(C1.valueBINARY, 0, b, 0, C1.valueBINARY.Length);
                    Array.Copy(C2.valueBINARY, 0, b, C1.valueBINARY.Length, C2.valueBINARY.Length);
                    C1 = new Cell(b);
                }
                else if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
                {
                    C1.DOUBLE = C1.valueDOUBLE + C2.valueDOUBLE;
                    C1.AFFINITY = CellAffinity.DOUBLE;
                }
                else if (C1.AFFINITY == CellAffinity.SINGLE || C2.AFFINITY == CellAffinity.SINGLE)
                {
                    C1.SINGLE = C1.valueSINGLE + C2.valueSINGLE;
                    C1.AFFINITY = CellAffinity.SINGLE;
                }
                else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
                {
                    C1.LONG = C1.valueLONG + C2.valueLONG;
                    C1.AFFINITY = CellAffinity.LONG;
                }
                else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
                {
                    C1.INT = C1.valueINT + C2.valueINT;
                    C1.AFFINITY = CellAffinity.INT;
                }
                else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
                {
                    C1.SHORT = (short)(C1.valueSHORT + C2.valueSHORT);
                    C1.AFFINITY = CellAffinity.SHORT;
                }
                else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
                {
                    C1.BYTE = (byte)(C1.valueBYTE + C2.valueBYTE);
                    C1.AFFINITY = CellAffinity.BYTE;
                }
                else if (C1.AFFINITY == CellAffinity.DATE_TIME || C2.AFFINITY == CellAffinity.DATE_TIME)
                {
                    C1.AFFINITY = CellAffinity.DATE_TIME;
                    C1.NULL = 1;
                }
                else
                {
                    C1.AFFINITY = CellAffinity.BOOL;
                    C1.NULL = 1;
                }

            }

            // Fix nulls //
            if (C1.NULL == 1)
            {
                C1.ULONG = 0;
                C1.CSTRING = "";
            }
            return C1;

        }

        /// <summary>
        /// Converts either an LONG or DOUBLE to a positve Value, returns the cell passed otherwise
        /// </summary>
        /// <param name="C">A cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator +(Cell C)
        {

            // Check nulls //
            if (C.NULL == 1)
                return C;

            if (C.AFFINITY == CellAffinity.SINGLE)
                C.SINGLE = +C.SINGLE;
            else if (C.AFFINITY == CellAffinity.DOUBLE)
                C.DOUBLE = +C.DOUBLE;
            else if (C.AFFINITY == CellAffinity.SHORT)
                C.SHORT = (short)(+C.SHORT);
            else if (C.AFFINITY == CellAffinity.INT)
                C.INT = +C.INT;
            else if (C.AFFINITY == CellAffinity.LONG)
                C.LONG = +C.LONG;
            else
                C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Adds one to the given cell for an LONG or DOUBLE, returns the cell passed otherwise
        /// </summary>
        /// <param name="C">The cell argument</param>
        /// <returns>Cell result</returns>
        public static Cell operator ++(Cell C)
        {
            if (C.NULL == 1)
                return C;
            if (C.AFFINITY == CellAffinity.SINGLE)
                C.SINGLE++;
            else if (C.AFFINITY == CellAffinity.DOUBLE)
                C.DOUBLE++;
            else if (C.AFFINITY == CellAffinity.BYTE)
                C.BYTE++;
            else if (C.AFFINITY == CellAffinity.SHORT)
                C.SHORT++;
            else if (C.AFFINITY == CellAffinity.INT)
                C.INT++;
            else if (C.AFFINITY == CellAffinity.LONG)
                C.LONG++;
            return C;
        }

        /// <summary>
        /// Subtracts two cells together for LONG and DOUBLE, repalces instances of C2 in C1, for date times, return the tick count difference as an LONG
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Value cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator -(Cell C1, Cell C2)
        {

            // Check nulls //
            if (C1.NULL == 1) return C1;
            else if (C2.NULL == 1) return C2;

            // If affinities match //
            if (C1.AFFINITY == C2.AFFINITY)
            {
                if (C1.AFFINITY == CellAffinity.SINGLE)
                {
                    C1.SINGLE -= C2.SINGLE;
                }
                else if (C1.AFFINITY == CellAffinity.DOUBLE)
                {
                    C1.DOUBLE -= C2.DOUBLE;
                }
                else if (C1.AFFINITY == CellAffinity.BYTE)
                {
                    C1.BYTE -= C2.BYTE;
                }
                else if (C1.AFFINITY == CellAffinity.SHORT)
                {
                    C1.SHORT -= C2.SHORT;
                }
                else if (C1.AFFINITY == CellAffinity.INT)
                {
                    C1.INT -= C2.INT;
                }
                else if (C1.AFFINITY == CellAffinity.LONG)
                {
                    C1.LONG -= C2.LONG;
                }
                else if (C1.AFFINITY == CellAffinity.DATE_TIME)
                {
                    C1.LONG = C1.LONG - C2.LONG;
                    C1.AFFINITY = CellAffinity.LONG;
                }
                else if (C1.AFFINITY == CellAffinity.BSTRING)
                {
                    C1.BSTRING = C1.BSTRING.Remove(C2.BSTRING);
                }
                else if (C1.AFFINITY == CellAffinity.CSTRING)
                {
                    C1.CSTRING = C1.CSTRING.Replace(C2.CSTRING, "");
                }
                else
                {
                    C1.NULL = 1;
                }

            }
            else
            {

                if (C1.AFFINITY == CellAffinity.CSTRING || C2.AFFINITY == CellAffinity.CSTRING)
                {
                    C1.CSTRING = C1.valueCSTRING.Replace(C2.valueCSTRING, "");
                    C1.AFFINITY = CellAffinity.CSTRING;
                }
                else if (C1.AFFINITY == CellAffinity.BSTRING || C2.AFFINITY == CellAffinity.BSTRING)
                {
                    C1.BSTRING = C1.valueBSTRING.Replace(C2.valueBSTRING, "");
                    C1.AFFINITY = CellAffinity.BSTRING;
                }
                else if (C1.AFFINITY == CellAffinity.BINARY || C2.AFFINITY == CellAffinity.BINARY)
                {
                    C1.AFFINITY = CellAffinity.BINARY;
                    C1.NULL = 1;
                }
                else if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
                {
                    C1.DOUBLE = C1.valueDOUBLE - C2.valueDOUBLE;
                    C1.AFFINITY = CellAffinity.DOUBLE;
                }
                else if (C1.AFFINITY == CellAffinity.SINGLE || C2.AFFINITY == CellAffinity.SINGLE)
                {
                    C1.SINGLE = C1.valueSINGLE - C2.valueSINGLE;
                    C1.AFFINITY = CellAffinity.SINGLE;
                }
                else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
                {
                    C1.LONG = C1.valueLONG - C2.valueLONG;
                    C1.AFFINITY = CellAffinity.LONG;
                }
                else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
                {
                    C1.INT = C1.valueINT - C2.valueINT;
                    C1.AFFINITY = CellAffinity.INT;
                }
                else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
                {
                    C1.SHORT = (short)(C1.valueSHORT - C2.valueSHORT);
                    C1.AFFINITY = CellAffinity.SHORT;
                }
                else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
                {
                    C1.BYTE = (byte)(C1.valueBYTE - C2.valueBYTE);
                    C1.AFFINITY = CellAffinity.BYTE;
                }
                else if (C1.AFFINITY == CellAffinity.DATE_TIME || C2.AFFINITY == CellAffinity.DATE_TIME)
                {
                    C1.AFFINITY = CellAffinity.DATE_TIME;
                    C1.NULL = 1;
                }
                else
                {
                    C1.AFFINITY = CellAffinity.BOOL;
                    C1.NULL = 1;
                }

            }

            // Fix nulls //
            if (C1.NULL == 1)
            {
                C1.ULONG = 0;
                C1.CSTRING = "";
            }

            return C1;

        }

        /// <summary>
        /// Converts either an LONG or DOUBLE to a negative Value, returns the cell passed otherwise
        /// </summary>
        /// <param name="C">A cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator -(Cell C)
        {

            // Check nulls //
            if (C.NULL == 1)
                return C;

            if (C.AFFINITY == CellAffinity.DOUBLE)
                C.DOUBLE = -C.DOUBLE;
            else if (C.AFFINITY == CellAffinity.SINGLE)
                C.SINGLE = -C.SINGLE;
            else if (C.AFFINITY == CellAffinity.SHORT)
                C.SHORT = (short)(-C.SHORT);
            else if (C.AFFINITY == CellAffinity.INT)
                C.INT = -C.INT;
            else if (C.AFFINITY == CellAffinity.LONG)
                C.LONG = -C.LONG;
            else if (C.AFFINITY == CellAffinity.CSTRING || C.AFFINITY == CellAffinity.BSTRING)
                C.CSTRING = new string(C.CSTRING.Reverse().ToArray());
            else
                C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Subtracts one to the given cell for an LONG or DOUBLE, returns the cell passed otherwise
        /// </summary>
        /// <param name="C">The cell argument</param>
        /// <returns>Cell result</returns>
        public static Cell operator --(Cell C)
        {
            if (C.NULL == 1)
                return C;
            if (C.AFFINITY == CellAffinity.DOUBLE)
                C.DOUBLE--;
            else if (C.AFFINITY == CellAffinity.SINGLE)
                C.SINGLE--;
            else if (C.AFFINITY == CellAffinity.SHORT)
                C.SHORT--;
            else if (C.AFFINITY == CellAffinity.INT)
                C.INT--;
            else if (C.AFFINITY == CellAffinity.LONG)
                C.LONG--;
            return C;
        }

        /// <summary>
        /// Multiplies two cells together for LONG and DOUBLE; if C1 is a string and C2 is either int/double, repeats the string C2 times; 
        /// otherwise, returns the cell passed otherwise
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Value cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator *(Cell C1, Cell C2)
        {

            // Check nulls //
            if (C1.NULL == 1) return C1;
            else if (C2.NULL == 1) return C2;

            // If affinities match //
            if (C1.AFFINITY == C2.AFFINITY)
            {

                if (C1.AFFINITY == CellAffinity.DOUBLE)
                {
                    C1.DOUBLE *= C2.DOUBLE;
                }
                else if (C1.AFFINITY == CellAffinity.SINGLE)
                {
                    C1.SINGLE *= C2.SINGLE;
                }
                else if (C1.AFFINITY == CellAffinity.BYTE)
                {
                    C1.BYTE *= C2.BYTE;
                }
                else if (C1.AFFINITY == CellAffinity.SHORT)
                {
                    C1.SHORT *= C2.SHORT;
                }
                else if (C1.AFFINITY == CellAffinity.INT)
                {
                    C1.INT *= C2.INT;
                }
                else if (C1.AFFINITY == CellAffinity.LONG)
                {
                    C1.LONG *= C2.LONG;
                }
                else
                {
                    C1.NULL = 1;
                }

            }
            else
            {

                if (C1.AFFINITY == CellAffinity.CSTRING || C2.AFFINITY == CellAffinity.CSTRING)
                {
                    C1.AFFINITY = CellAffinity.CSTRING;
                    C1.NULL = 1;
                }
                if (C1.AFFINITY == CellAffinity.BSTRING || C2.AFFINITY == CellAffinity.BSTRING)
                {
                    C1.AFFINITY = CellAffinity.BSTRING;
                    C1.NULL = 1;
                }
                else if (C1.AFFINITY == CellAffinity.BINARY || C2.AFFINITY == CellAffinity.BINARY)
                {
                    C1.AFFINITY = CellAffinity.BINARY;
                    C1.NULL = 1;
                }
                else if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
                {
                    C1.DOUBLE = C1.valueDOUBLE * C2.valueDOUBLE;
                    C1.AFFINITY = CellAffinity.DOUBLE;
                }
                else if (C1.AFFINITY == CellAffinity.SINGLE || C2.AFFINITY == CellAffinity.SINGLE)
                {
                    C1.SINGLE = C1.valueSINGLE * C2.valueSINGLE;
                    C1.AFFINITY = CellAffinity.SINGLE;
                }
                else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
                {
                    C1.LONG = C1.valueLONG * C2.valueLONG;
                    C1.AFFINITY = CellAffinity.LONG;
                }
                else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
                {
                    C1.INT = C1.valueINT * C2.valueINT;
                    C1.AFFINITY = CellAffinity.INT;
                }
                else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
                {
                    C1.SHORT = (short)(C1.valueSHORT * C2.valueSHORT);
                    C1.AFFINITY = CellAffinity.SHORT;
                }
                else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
                {
                    C1.BYTE = (byte)(C1.valueBYTE * C2.valueBYTE);
                    C1.AFFINITY = CellAffinity.BYTE;
                }
                else if (C1.AFFINITY == CellAffinity.DATE_TIME || C2.AFFINITY == CellAffinity.DATE_TIME)
                {
                    C1.AFFINITY = CellAffinity.DATE_TIME;
                    C1.NULL = 1;
                }
                else
                {
                    C1.AFFINITY = CellAffinity.BOOL;
                    C1.NULL = 1;
                }

            }

            // Fix nulls //
            if (C1.NULL == 1)
            {
                C1.ULONG = 0;
                C1.CSTRING = "";
            }

            return C1;

        }

        /// <summary>
        /// Divides two cells together for LONG and DOUBLE, returns the cell passed otherwise as null
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Value cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator /(Cell C1, Cell C2)
        {

            // Check nulls //
            if (C1.NULL == 1) return C1;
            else if (C2.NULL == 1) return C2;

            // If affinities match //
            if (C1.AFFINITY == C2.AFFINITY)
            {

                if (C1.AFFINITY == CellAffinity.DOUBLE && C2.DOUBLE != 0)
                {
                    C1.DOUBLE /= C2.DOUBLE;
                }
                else if (C1.AFFINITY == CellAffinity.SINGLE && C2.SINGLE != 0)
                {
                    C1.SINGLE /= C2.SINGLE;
                }
                else if (C1.AFFINITY == CellAffinity.LONG && C2.LONG != 0)
                {
                    C1.LONG /= C2.LONG;
                }
                else if (C1.AFFINITY == CellAffinity.INT && C2.INT != 0)
                {
                    C1.INT /= C2.INT;
                }
                else if (C1.AFFINITY == CellAffinity.SHORT && C2.SHORT != 0)
                {
                    C1.SHORT /= C2.SHORT;
                }
                else if (C1.AFFINITY == CellAffinity.BYTE && C2.BYTE != 0)
                {
                    C1.BYTE /= C2.BYTE;
                }
                else
                {
                    C1.NULL = 1;
                }

            }
            else
            {

                if (C1.AFFINITY == CellAffinity.CSTRING || C2.AFFINITY == CellAffinity.CSTRING)
                {
                    C1.AFFINITY = CellAffinity.CSTRING;
                    C1.NULL = 1;
                }
                if (C1.AFFINITY == CellAffinity.BSTRING || C2.AFFINITY == CellAffinity.BSTRING)
                {
                    C1.AFFINITY = CellAffinity.BSTRING;
                    C1.NULL = 1;
                }
                else if (C1.AFFINITY == CellAffinity.BINARY || C2.AFFINITY == CellAffinity.BINARY)
                {
                    C1.AFFINITY = CellAffinity.BINARY;
                    C1.NULL = 1;
                }
                else if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
                {

                    if (C2.valueDOUBLE != 0)
                    {
                        C1.DOUBLE = C1.valueDOUBLE / C2.valueDOUBLE;
                    }
                    else
                    {
                        C1.NULL = 1;
                    }
                    C1.AFFINITY = CellAffinity.DOUBLE;

                }
                else if (C1.AFFINITY == CellAffinity.SINGLE || C2.AFFINITY == CellAffinity.SINGLE)
                {

                    if (C2.valueSINGLE != 0)
                    {
                        C1.SINGLE = C1.valueSINGLE / C2.valueSINGLE;
                    }
                    else
                    {
                        C1.NULL = 1;
                    }
                    C1.AFFINITY = CellAffinity.SINGLE;

                }
                else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
                {

                    if (C2.valueLONG != 0)
                    {
                        C1.LONG = C1.valueLONG / C2.valueLONG;
                    }
                    else
                    {
                        C1.NULL = 1;
                    }
                    C1.AFFINITY = CellAffinity.LONG;

                }
                else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
                {

                    if (C2.valueINT != 0)
                    {
                        C1.INT = C1.valueINT / C2.valueINT;
                    }
                    else
                    {
                        C1.NULL = 1;
                    }
                    C1.AFFINITY = CellAffinity.INT;

                }
                else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
                {

                    if (C2.valueSHORT != 0)
                    {
                        C1.SHORT = (short)(C1.valueSHORT / C2.valueSHORT);
                    }
                    else
                    {
                        C1.NULL = 1;
                    }
                    C1.AFFINITY = CellAffinity.SHORT;

                }
                else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
                {

                    if (C2.valueBYTE != 0)
                    {
                        C1.BYTE = (byte)(C1.valueBYTE / C2.valueBYTE);
                    }
                    else
                    {
                        C1.NULL = 1;
                    }
                    C1.AFFINITY = CellAffinity.BYTE;

                }
                else if (C1.AFFINITY == CellAffinity.DATE_TIME || C2.AFFINITY == CellAffinity.DATE_TIME)
                {
                    C1.AFFINITY = CellAffinity.DATE_TIME;
                    C1.NULL = 1;
                }
                else
                {
                    C1.AFFINITY = CellAffinity.BOOL;
                    C1.NULL = 1;
                }

            }

            // Fix nulls //
            if (C1.NULL == 1)
            {
                C1.ULONG = 0;
                C1.CSTRING = "";
            }

            return C1;

        }

        /// <summary>
        /// Divides two cells together for LONG and DOUBLE, returns the cell passed otherwise as null; if C2 is 0, then it returns 0
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Value cell</param>
        /// <returns>Cell result</returns>
        public static Cell CheckDivide(Cell C1, Cell C2)
        {

            // Check nulls //
            if (C1.NULL == 1)
                return C1;
            else if (C2.NULL == 1)
                return C2;

            // If affinities match //
            if (C1.AFFINITY == C2.AFFINITY)
            {

                if (C1.AFFINITY == CellAffinity.DOUBLE && C2.DOUBLE != 0)
                {
                    C1.DOUBLE /= C2.DOUBLE;
                }
                else if (C1.AFFINITY == CellAffinity.SINGLE && C2.SINGLE != 0)
                {
                    C1.SINGLE /= C2.SINGLE;
                }
                else if (C1.AFFINITY == CellAffinity.LONG && C2.LONG != 0)
                {
                    C1.LONG /= C2.LONG;
                }
                else if (C1.AFFINITY == CellAffinity.INT && C2.INT != 0)
                {
                    C1.INT /= C2.INT;
                }
                else if (C1.AFFINITY == CellAffinity.SHORT && C2.SHORT != 0)
                {
                    C1.SHORT /= C2.SHORT;
                }
                else if (C1.AFFINITY == CellAffinity.BYTE && C2.BYTE != 0)
                {
                    C1.BYTE /= C2.BYTE;
                }
                else if (C1.AFFINITY == CellAffinity.BYTE || C1.AFFINITY == CellAffinity.SHORT || C1.AFFINITY == CellAffinity.INT 
                    || C1.AFFINITY == CellAffinity.LONG || C1.AFFINITY == CellAffinity.SINGLE || C1.AFFINITY == CellAffinity.DOUBLE)
                {
                    C1.ULONG = 0;
                }
                else
                {
                    C1.NULL = 1;
                }

            }
            else
            {

                if (C1.AFFINITY == CellAffinity.CSTRING || C2.AFFINITY == CellAffinity.CSTRING)
                {
                    C1.AFFINITY = CellAffinity.CSTRING;
                    C1.NULL = 1;
                }
                else if (C1.AFFINITY == CellAffinity.BSTRING || C2.AFFINITY == CellAffinity.BSTRING)
                {
                    C1.AFFINITY = CellAffinity.BSTRING;
                    C1.NULL = 1;
                }
                else if (C1.AFFINITY == CellAffinity.BINARY || C2.AFFINITY == CellAffinity.BINARY)
                {
                    C1.AFFINITY = CellAffinity.BINARY;
                    C1.NULL = 1;
                }
                else if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
                {

                    if (C2.valueDOUBLE != 0)
                    {
                        C1.DOUBLE = C1.valueDOUBLE / C2.valueDOUBLE;
                    }
                    else
                    {
                        C1.DOUBLE = 0D;
                    }
                    C1.AFFINITY = CellAffinity.DOUBLE;

                }
                else if (C1.AFFINITY == CellAffinity.SINGLE || C2.AFFINITY == CellAffinity.SINGLE)
                {

                    if (C2.valueSINGLE != 0)
                    {
                        C1.SINGLE = C1.valueSINGLE / C2.valueSINGLE;
                    }
                    else
                    {
                        C1.SINGLE = 0F;
                    }
                    C1.AFFINITY = CellAffinity.SINGLE;

                }
                else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
                {

                    if (C2.valueLONG != 0)
                    {
                        C1.LONG = C1.valueLONG / C2.valueLONG;
                    }
                    else
                    {
                        C1.LONG = 0;
                    }
                    C1.AFFINITY = CellAffinity.LONG;

                }
                else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
                {

                    if (C2.valueINT != 0)
                    {
                        C1.INT = C1.valueINT / C2.valueINT;
                    }
                    else
                    {
                        C1.INT = 0;
                    }
                    C1.AFFINITY = CellAffinity.INT;

                }
                else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
                {

                    if (C2.valueSHORT != 0)
                    {
                        C1.SHORT = (short)(C1.valueSHORT / C2.valueSHORT);
                    }
                    else
                    {
                        C1.SHORT = 0;
                    }
                    C1.AFFINITY = CellAffinity.SHORT;

                }
                else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
                {

                    if (C2.valueBYTE != 0)
                    {
                        C1.BYTE = (byte)(C1.valueBYTE / C2.valueBYTE);
                    }
                    else
                    {
                        C1.BYTE = 0;
                    }
                    C1.AFFINITY = CellAffinity.BYTE;

                }
                else if (C1.AFFINITY == CellAffinity.DATE_TIME || C2.AFFINITY == CellAffinity.DATE_TIME)
                {
                    C1.AFFINITY = CellAffinity.DATE_TIME;
                    C1.NULL = 1;
                }
                else
                {
                    C1.AFFINITY = CellAffinity.BOOL;
                    C1.NULL = 1;
                }

            }

            // Fix nulls //
            if (C1.NULL == 1)
            {
                C1.ULONG = 0;
                C1.CSTRING = "";
            }

            return C1;

        }

        /// <summary>
        /// Perform modulo between two cells together for LONG and DOUBLE, returns the cell passed otherwise
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Value cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator %(Cell C1, Cell C2)
        {

            // Check nulls //
            if (C1.NULL == 1) return C1;
            else if (C2.NULL == 1) return C2;

            // If affinities match //
            if (C1.AFFINITY == C2.AFFINITY)
            {

                if (C1.AFFINITY == CellAffinity.DOUBLE && C2.DOUBLE != 0)
                {
                    C1.DOUBLE %= C2.DOUBLE;
                }
                else if (C1.AFFINITY == CellAffinity.SINGLE && C2.SINGLE != 0)
                {
                    C1.SINGLE %= C2.SINGLE;
                }
                else if (C1.AFFINITY == CellAffinity.LONG && C2.LONG != 0)
                {
                    C1.LONG %= C2.LONG;
                }
                else if (C1.AFFINITY == CellAffinity.INT && C2.INT != 0)
                {
                    C1.INT %= C2.INT;
                }
                else if (C1.AFFINITY == CellAffinity.SHORT && C2.SHORT != 0)
                {
                    C1.SHORT %= C2.SHORT;
                }
                else if (C1.AFFINITY == CellAffinity.BYTE && C2.BYTE != 0)
                {
                    C1.BYTE %= C2.BYTE;
                }
                else
                {
                    C1.NULL = 1;
                }

            }
            else
            {

                if (C1.AFFINITY == CellAffinity.CSTRING || C2.AFFINITY == CellAffinity.CSTRING)
                {
                    C1.AFFINITY = CellAffinity.CSTRING;
                    C1.NULL = 1;
                }
                else if (C1.AFFINITY == CellAffinity.BSTRING || C2.AFFINITY == CellAffinity.BSTRING)
                {
                    C1.AFFINITY = CellAffinity.BSTRING;
                    C1.NULL = 1;
                }
                else if (C1.AFFINITY == CellAffinity.BINARY || C2.AFFINITY == CellAffinity.BINARY)
                {
                    C1.AFFINITY = CellAffinity.BINARY;
                    C1.NULL = 1;
                }
                else if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
                {

                    if (C2.valueDOUBLE != 0)
                    {
                        C1.DOUBLE = C1.valueDOUBLE % C2.valueDOUBLE;
                    }
                    else
                    {
                        C1.NULL = 1;
                    }
                    C1.AFFINITY = CellAffinity.DOUBLE;

                }
                else if (C1.AFFINITY == CellAffinity.SINGLE || C2.AFFINITY == CellAffinity.SINGLE)
                {

                    if (C2.valueSINGLE != 0)
                    {
                        C1.SINGLE = C1.valueSINGLE % C2.valueSINGLE;
                    }
                    else
                    {
                        C1.NULL = 1;
                    }
                    C1.AFFINITY = CellAffinity.SINGLE;

                }
                else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
                {

                    if (C2.valueLONG != 0)
                    {
                        C1.LONG = C1.valueLONG % C2.valueLONG;
                    }
                    else
                    {
                        C1.NULL = 1;
                    }
                    C1.AFFINITY = CellAffinity.LONG;

                }
                else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
                {

                    if (C2.valueINT != 0)
                    {
                        C1.INT = C1.valueINT % C2.valueINT;
                    }
                    else
                    {
                        C1.NULL = 1;
                    }
                    C1.AFFINITY = CellAffinity.INT;

                }
                else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
                {

                    if (C2.valueSHORT != 0)
                    {
                        C1.SHORT = (short)(C1.valueSHORT % C2.valueSHORT);
                    }
                    else
                    {
                        C1.NULL = 1;
                    }
                    C1.AFFINITY = CellAffinity.SHORT;

                }
                else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
                {

                    if (C2.valueBYTE != 0)
                    {
                        C1.BYTE = (byte)(C1.valueBYTE % C2.valueBYTE);
                    }
                    else
                    {
                        C1.NULL = 1;
                    }
                    C1.AFFINITY = CellAffinity.BYTE;

                }
                else if (C1.AFFINITY == CellAffinity.DATE_TIME || C2.AFFINITY == CellAffinity.DATE_TIME)
                {
                    C1.AFFINITY = CellAffinity.DATE_TIME;
                    C1.NULL = 1;
                }
                else
                {
                    C1.AFFINITY = CellAffinity.BOOL;
                    C1.NULL = 1;
                }

            }

            // Fix nulls //
            if (C1.NULL == 1)
            {
                C1.ULONG = 0;
                C1.CSTRING = "";
            }

            return C1;

        }

        /// <summary>
        /// Return the bitwise AND for all types
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Value cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator &(Cell C1, Cell C2)
        {

            // Check nulls //
            if (C1.NULL == 1)
                return C1;
            else if (C2.NULL == 1)
                return C2;

            // Handle AND two bools //
            if (C1.AFFINITY == CellAffinity.BOOL && C2.AFFINITY == CellAffinity.BOOL)
                return (C1.BOOL && C2.BOOL ? CellValues.True : CellValues.False);

            // If neither a string or blob //
            if (C1.AFFINITY != CellAffinity.CSTRING && C2.AFFINITY != CellAffinity.CSTRING
                && C1.AFFINITY != CellAffinity.BSTRING && C2.AFFINITY != CellAffinity.BSTRING
                && C1.AFFINITY != CellAffinity.BINARY && C2.AFFINITY != CellAffinity.BINARY)
            {

                C1.LONG = C1.LONG & C2.LONG;
                if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
                    C1.AFFINITY = CellAffinity.DOUBLE;
                else if (C1.AFFINITY == CellAffinity.SINGLE || C2.AFFINITY == CellAffinity.SINGLE)
                    C1.AFFINITY = CellAffinity.SINGLE;
                else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
                    C1.AFFINITY = CellAffinity.LONG;
                else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
                    C1.AFFINITY = CellAffinity.INT;
                else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
                    C1.AFFINITY = CellAffinity.SHORT;
                else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
                    C1.AFFINITY = CellAffinity.BYTE;
                else if (C1.AFFINITY == CellAffinity.DATE_TIME || C2.AFFINITY == CellAffinity.DATE_TIME)
                    C1.AFFINITY = CellAffinity.DATE_TIME;
                else if (C1.AFFINITY == CellAffinity.BOOL || C2.AFFINITY == CellAffinity.BOOL)
                    C1.AFFINITY = CellAffinity.BOOL;

            }
            else if (C1.AFFINITY == CellAffinity.CSTRING || C2.AFFINITY == CellAffinity.CSTRING)
            {

                StringBuilder sb = new StringBuilder();
                int t = 0;
                for (int i = 0; i < C1.CSTRING.Length; i++)
                {
                    if (t >= C2.valueCSTRING.Length)
                        t = 0;
                    sb.Append((char)(C1.valueCSTRING[i] & C2.valueCSTRING[t]));
                    t++;
                }
                C1.CSTRING = sb.ToString();

            }
            else if (C1.AFFINITY == CellAffinity.BSTRING || C2.AFFINITY == CellAffinity.BSTRING)
            {

                StringBuilder sb = new StringBuilder();
                int t = 0;
                for (int i = 0; i < C1.CSTRING.Length; i++)
                {
                    if (t >= C2.valueCSTRING.Length)
                        t = 0;
                    sb.Append((char)(C1.valueCSTRING[i] & C2.valueCSTRING[t]));
                    t++;
                }
                C1.CSTRING = sb.ToString();

            }
            else
            {

                int t = 0;
                byte[] b = C2.valueBINARY;
                for (int i = 0; i < C1.BINARY.Length; i++)
                {
                    if (t >= b.Length)
                        t = 0;
                    C1.BINARY[i] = (byte)(C1.BINARY[i] & b[t]);
                    t++;
                }

            }

            return C1;

        }

        /// <summary>
        /// Returns the bitwise OR for all types
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Value cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator |(Cell C1, Cell C2)
        {

            // Check nulls //
            if (C1.NULL == 1) return C1;
            else if (C2.NULL == 1) return C2;

            // Handle AND two bools //
            if (C1.AFFINITY == CellAffinity.BOOL && C2.AFFINITY == CellAffinity.BOOL)
                return (C1.BOOL || C2.BOOL ? CellValues.True : CellValues.False);

            // If neither a string or blob //
            if (C1.AFFINITY != CellAffinity.CSTRING && C2.AFFINITY != CellAffinity.CSTRING
                && C1.AFFINITY != CellAffinity.BSTRING && C2.AFFINITY != CellAffinity.BSTRING
                && C1.AFFINITY != CellAffinity.BINARY && C2.AFFINITY != CellAffinity.BINARY)
            {

                C1.LONG = C1.LONG | C2.LONG;
                if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
                    C1.AFFINITY = CellAffinity.DOUBLE;
                else if (C1.AFFINITY == CellAffinity.SINGLE || C2.AFFINITY == CellAffinity.SINGLE)
                    C1.AFFINITY = CellAffinity.SINGLE;
                else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
                    C1.AFFINITY = CellAffinity.LONG;
                else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
                    C1.AFFINITY = CellAffinity.INT;
                else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
                    C1.AFFINITY = CellAffinity.SHORT;
                else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
                    C1.AFFINITY = CellAffinity.BYTE;
                else if (C1.AFFINITY == CellAffinity.DATE_TIME || C2.AFFINITY == CellAffinity.DATE_TIME)
                    C1.AFFINITY = CellAffinity.DATE_TIME;
                else if (C1.AFFINITY == CellAffinity.BOOL || C2.AFFINITY == CellAffinity.BOOL)
                    C1.AFFINITY = CellAffinity.BOOL;

            }
            else if (C1.AFFINITY == CellAffinity.CSTRING || C2.AFFINITY == CellAffinity.CSTRING)
            {
                StringBuilder sb = new StringBuilder();
                int t = 0;
                for (int i = 0; i < C1.CSTRING.Length; i++)
                {
                    if (t >= C2.valueCSTRING.Length)
                        t = 0;
                    sb.Append((char)(C1.CSTRING[i] | C2.valueCSTRING[t]));
                    t++;
                }
                C1.CSTRING = sb.ToString();

            }
            else if (C1.AFFINITY == CellAffinity.BSTRING || C2.AFFINITY == CellAffinity.BSTRING)
            {
                StringBuilder sb = new StringBuilder();
                int t = 0;
                for (int i = 0; i < C1.CSTRING.Length; i++)
                {
                    if (t >= C2.valueCSTRING.Length)
                        t = 0;
                    sb.Append((char)(C1.CSTRING[i] | C2.valueCSTRING[t]));
                    t++;
                }
                C1.CSTRING = sb.ToString();

            }
            else
            {

                int t = 0;
                byte[] b = C2.valueBINARY;
                for (int i = 0; i < C1.BINARY.Length; i++)
                {
                    if (t >= b.Length) t = 0;
                    C1.BINARY[i] = (byte)(C1.BINARY[i] | b[t]);
                    t++;
                }

            }

            return C1;

        }

        /// <summary>
        /// Returns the bitwise XOR for all types
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Value cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator ^(Cell C1, Cell C2)
        {

            // Check nulls //
            if (C1.NULL == 1) return C1;
            else if (C2.NULL == 1) return C2;

            // Handle AND two bools //
            if (C1.AFFINITY == CellAffinity.BOOL && C2.AFFINITY == CellAffinity.BOOL)
                return (C1.BOOL ^ C2.BOOL ? CellValues.True : CellValues.False);

            // If neither a string or blob //
            if (C1.AFFINITY != CellAffinity.CSTRING && C2.AFFINITY != CellAffinity.CSTRING
                && C1.AFFINITY != CellAffinity.BSTRING && C2.AFFINITY != CellAffinity.BSTRING
                && C1.AFFINITY != CellAffinity.BINARY && C2.AFFINITY != CellAffinity.BINARY)
            {

                C1.LONG = C1.LONG ^ C2.LONG;
                if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
                    C1.AFFINITY = CellAffinity.DOUBLE;
                else if (C1.AFFINITY == CellAffinity.SINGLE || C2.AFFINITY == CellAffinity.SINGLE)
                    C1.AFFINITY = CellAffinity.SINGLE;
                else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
                    C1.AFFINITY = CellAffinity.LONG;
                else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
                    C1.AFFINITY = CellAffinity.INT;
                else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
                    C1.AFFINITY = CellAffinity.SHORT;
                else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
                    C1.AFFINITY = CellAffinity.BYTE;
                else if (C1.AFFINITY == CellAffinity.DATE_TIME || C2.AFFINITY == CellAffinity.DATE_TIME)
                    C1.AFFINITY = CellAffinity.DATE_TIME;
                else if (C1.AFFINITY == CellAffinity.BOOL || C2.AFFINITY == CellAffinity.BOOL)
                    C1.AFFINITY = CellAffinity.BOOL;

            }
            else if (C1.AFFINITY == CellAffinity.CSTRING || C2.AFFINITY == CellAffinity.CSTRING)
            {

                StringBuilder sb = new StringBuilder();
                int t = 0;
                for (int i = 0; i < C1.CSTRING.Length; i++)
                {
                    if (t >= C2.valueCSTRING.Length) t = 0;
                    sb.Append((char)(C1.CSTRING[i] ^ C2.valueCSTRING[t]));
                    t++;
                }
                C1.CSTRING = sb.ToString();

            }
            else if (C1.AFFINITY == CellAffinity.BSTRING || C2.AFFINITY == CellAffinity.BSTRING)
            {

                StringBuilder sb = new StringBuilder();
                int t = 0;
                for (int i = 0; i < C1.CSTRING.Length; i++)
                {
                    if (t >= C2.valueCSTRING.Length) t = 0;
                    sb.Append((char)(C1.CSTRING[i] ^ C2.valueCSTRING[t]));
                    t++;
                }
                C1.CSTRING = sb.ToString();

            }
            else
            {

                int t = 0;
                byte[] a = C2.valueBINARY;
                byte[] b = C2.valueBINARY;
                for (int i = 0; i < a.Length; i++)
                {
                    if (t >= b.Length) t = 0;
                    a[i] = (byte)(a[i] ^ b[t]);
                    t++;
                }
                C1.AFFINITY = CellAffinity.BINARY;
                C1.BINARY = a;
            
            }
            return C1;

        }

        /// <summary>
        /// Checks if two cells are equal
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Value cell</param>
        /// <returns>A boolean</returns>
        public static bool operator ==(Cell C1, Cell C2)
        {

            if (C1.NULL == 1 && C2.NULL == 1)
                return true;
            
            if (C1.AFFINITY != CellAffinity.CSTRING && C1.AFFINITY != CellAffinity.BSTRING && C1.AFFINITY != CellAffinity.BINARY)
                return C1.LONG == C2.LONG;
            else if (C1.AFFINITY == CellAffinity.CSTRING || C1.AFFINITY == CellAffinity.BSTRING)
                return C1.CSTRING == C2.valueCSTRING;

            return CellComparer.Compare(C1, C2) == 0;

        }

        /// <summary>
        /// Checks if two cells are not equal
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Value cell</param>
        /// <returns>A boolean</returns>
        public static bool operator !=(Cell C1, Cell C2)
        {

            if (C1.NULL != C2.NULL)
                return true;

            if (C1.AFFINITY != CellAffinity.CSTRING && C1.AFFINITY != CellAffinity.BINARY)
                return C1.LONG != C2.LONG;
            else if (C1.AFFINITY == CellAffinity.CSTRING || C1.AFFINITY == CellAffinity.BSTRING)
                return C1.CSTRING != C2.CSTRING;

            return CellComparer.Compare(C1, C2) != 0;

        }

        /// <summary>
        /// Checks if C1 is less than C2
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Value cell</param>
        /// <returns>A boolean</returns>
        public static bool operator <(Cell C1, Cell C2)
        {
            return CellComparer.Compare(C1, C2) < 0;
        }

        /// <summary>
        /// Checks if C1 is less than or equal to C2
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Value cell</param>
        /// <returns>A boolean</returns>
        public static bool operator <=(Cell C1, Cell C2)
        {
            return CellComparer.Compare(C1, C2) <= 0;
        }

        /// <summary>
        /// Checks if C1 is greater than C2
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Value cell</param>
        /// <returns>A boolean</returns>
        public static bool operator >(Cell C1, Cell C2)
        {
            return CellComparer.Compare(C1, C2) > 0;
        }

        /// <summary>
        /// Checks if C1 is greater than or equal to C2
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Value cell</param>
        /// <returns>A boolean</returns>
        public static bool operator >=(Cell C1, Cell C2)
        {
            return CellComparer.Compare(C1, C2) >= 0;
        }

        /// <summary>
        /// Determines whether or not a cell is 'TRUE'; if the cell is not null it returns the boolean Value
        /// </summary>
        /// <param name="C">The cell Value</param>
        /// <returns></returns>
        public static bool operator true(Cell C)
        {
            return C.NULL == 0 && C.BOOL;
        }

        /// <summary>
        /// Determines whether or not a cell is 'FALSE'; if the cell is null or the BOOL Value is false, returns false
        /// </summary>
        /// <param name="C">The cell Value</param>
        /// <returns></returns>
        public static bool operator false(Cell C)
        {
            return !(C.NULL == 0 && C.BOOL);
        }

        public static Cell LeftShift(Cell C, int X)
        {

            if (C.IsNull)
                return C;

            switch (C.Affinity)
            {

                case CellAffinity.BOOL:
                    return CellValues.NullBOOL;
                case CellAffinity.DATE_TIME:
                    return CellValues.NullDATE;
                case CellAffinity.BYTE:
                    C.B0 = (byte)(C.B0 << X);
                    return C;
                case CellAffinity.SHORT:
                    C.SHORT = (short)(C.SHORT << X);
                    return C;
                case CellAffinity.SINGLE:
                case CellAffinity.INT:
                    C.INT = (int)(C.INT << X);
                    return C;
                case CellAffinity.DOUBLE:
                case CellAffinity.LONG:
                    C.LONG = (long)(C.LONG << X);
                    return C;
                case CellAffinity.BINARY:
                    C.BINARY = BitHelper.ShiftLeft(C.BINARY, X);
                    return C;
                case CellAffinity.BSTRING:
                    C.BSTRING = new BString(BitHelper.ShiftLeft(C.BSTRING._elements, X));
                    return C;
                case CellAffinity.CSTRING:
                    C.CSTRING = UTF8Encoding.Unicode.GetString(BitHelper.ShiftLeft(UTF8Encoding.Unicode.GetBytes(C.CSTRING), X));
                    return C;
                
            }

            throw new Exception();

        }

        public static Cell RightShift(Cell C, int X)
        {

            if (C.IsNull)
                return C;

            switch (C.Affinity)
            {

                case CellAffinity.BOOL:
                    return CellValues.NullBOOL;
                case CellAffinity.DATE_TIME:
                    return CellValues.NullDATE;
                case CellAffinity.BYTE:
                    C.B0 = (byte)(C.B0 >> X);
                    return C;
                case CellAffinity.SHORT:
                    C.SHORT = (short)(C.SHORT >> X);
                    return C;
                case CellAffinity.SINGLE:
                case CellAffinity.INT:
                    C.INT = (int)(C.INT >> X);
                    return C;
                case CellAffinity.DOUBLE:
                case CellAffinity.LONG:
                    C.LONG = (long)(C.LONG >> X);
                    return C;
                case CellAffinity.BINARY:
                    C.BINARY = BitHelper.ShiftRight(C.BINARY, X);
                    return C;
                case CellAffinity.BSTRING:
                    C.BSTRING = new BString(BitHelper.ShiftRight(C.BSTRING._elements, X));
                    return C;
                case CellAffinity.CSTRING:
                    C.CSTRING = UTF8Encoding.Unicode.GetString(BitHelper.ShiftRight(UTF8Encoding.Unicode.GetBytes(C.CSTRING), X));
                    return C;

            }

            throw new Exception();

        }

        public static Cell LeftRotate(Cell C, int X)
        {

            if (C.IsNull)
                return C;

            switch (C.Affinity)
            {

                case CellAffinity.BOOL:
                    return CellValues.NullBOOL;
                case CellAffinity.DATE_TIME:
                    return CellValues.NullDATE;
                case CellAffinity.BYTE:
                    C.B0 = BitHelper.RotateLeft(C.B0, X);
                    return C;
                case CellAffinity.SHORT:
                    C.SHORT = BitHelper.RotateLeft(C.SHORT, X);
                    return C;
                case CellAffinity.SINGLE:
                case CellAffinity.INT:
                    C.INT = BitHelper.RotateLeft(C.INT, X);
                    return C;
                case CellAffinity.DOUBLE:
                case CellAffinity.LONG:
                    C.LONG = BitHelper.RotateLeft(C.LONG, X);
                    return C;
                case CellAffinity.BINARY:
                    C.BINARY = BitHelper.RotateLeft(C.BINARY, X);
                    return C;
                case CellAffinity.BSTRING:
                    C.BSTRING = new BString(BitHelper.RotateLeft(C.BSTRING._elements, X));
                    return C;
                case CellAffinity.CSTRING:
                    C.CSTRING = UTF8Encoding.Unicode.GetString(BitHelper.RotateLeft(UTF8Encoding.Unicode.GetBytes(C.CSTRING), X));
                    return C;

            }

            throw new Exception();

        }

        public static Cell RightRotate(Cell C, int X)
        {

            if (C.IsNull)
                return C;

            switch (C.Affinity)
            {

                case CellAffinity.BOOL:
                    return CellValues.NullBOOL;
                case CellAffinity.DATE_TIME:
                    return CellValues.NullDATE;
                case CellAffinity.BYTE:
                    C.B0 = BitHelper.RotateRight(C.B0, X);
                    return C;
                case CellAffinity.SHORT:
                    C.SHORT = BitHelper.RotateRight(C.SHORT, X);
                    return C;
                case CellAffinity.SINGLE:
                case CellAffinity.INT:
                    C.INT = BitHelper.RotateRight(C.INT, X);
                    return C;
                case CellAffinity.DOUBLE:
                case CellAffinity.LONG:
                    C.LONG = BitHelper.RotateRight(C.LONG, X);
                    return C;
                case CellAffinity.BINARY:
                    C.BINARY = BitHelper.RotateRight(C.BINARY, X);
                    return C;
                case CellAffinity.BSTRING:
                    C.BSTRING = new BString(BitHelper.RotateRight(C.BSTRING._elements, X));
                    return C;
                case CellAffinity.CSTRING:
                    C.CSTRING = UTF8Encoding.Unicode.GetString(BitHelper.RotateRight(UTF8Encoding.Unicode.GetBytes(C.CSTRING), X));
                    return C;

            }

            throw new Exception();

        }

        #endregion

        #region Implementations

        /// <summary>
        /// IComparable implementation
        /// </summary>
        /// <param name="C">A cell to compare to the Spike instance</param>
        /// <returns>An integer Value</returns>
        int IComparable<Cell>.CompareTo(Cell C)
        {
            return CellComparer.Compare(this, C);
        }

        /// <summary>
        /// IComparer implementation that compares two cells
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Value cell</param>
        /// <returns>An integer representing </returns>
        int IComparer<Cell>.Compare(Cell C1, Cell C2)
        {
            return CellComparer.Compare(C1, C2);
        }

        #endregion

        #region ImplicitConversion

        public static implicit operator bool(Cell C)
        {
            return C.valueBOOL;
        }

        public static implicit operator Cell(bool Value)
        {
            return new Cell(Value);
        }

        public static implicit operator DateTime(Cell C)
        {
            return C.valueDATE;
        }

        public static implicit operator Cell(DateTime Value)
        {
            return new Cell(Value);
        }

        public static implicit operator byte(Cell C)
        {
            return C.valueBYTE;
        }

        public static implicit operator Cell(byte Value)
        {
            return new Cell(Value);
        }

        public static implicit operator short(Cell C)
        {
            return C.valueSHORT;
        }

        public static implicit operator Cell(short Value)
        {
            return new Cell(Value);
        }

        public static implicit operator int(Cell C)
        {
            return C.valueINT;
        }

        public static implicit operator Cell(int Value)
        {
            return new Cell(Value);
        }

        public static implicit operator long(Cell C)
        {
            return C.valueLONG;
        }

        public static implicit operator Cell(long Value)
        {
            return new Cell(Value);
        }

        public static implicit operator float(Cell C)
        {
            return C.valueSINGLE;
        }

        public static implicit operator Cell(float Value)
        {
            return new Cell(Value);
        }

        public static implicit operator double(Cell C)
        {
            return C.valueDOUBLE;
        }

        public static implicit operator Cell(double Value)
        {
            return new Cell(Value);
        }

        public static implicit operator byte[](Cell C)
        {
            return C.valueBINARY;
        }

        public static implicit operator Cell(byte[] Value)
        {
            return new Cell(Value);
        }

        public static implicit operator BString(Cell C)
        {
            return C.valueBSTRING;
        }

        public static implicit operator Cell(BString Value)
        {
            return new Cell(Value);
        }

        public static implicit operator string(Cell C)
        {
            return C.valueCSTRING;
        }

        public static implicit operator Cell(string Value)
        {
            return new Cell(Value);
        }

        #endregion

        #region ExplicitConversion

        //public static explicit operator bool(Cell C)
        //{
        //    return C.valueBOOL;
        //}

        //public static explicit operator Cell(bool Value)
        //{
        //    return new Cell(Value);
        //}

        //public static explicit operator DateTime(Cell C)
        //{
        //    return C.valueDATE;
        //}

        //public static explicit operator Cell(DateTime Value)
        //{
        //    return new Cell(Value);
        //}

        //public static explicit operator byte(Cell C)
        //{
        //    return C.valueBYTE;
        //}

        //public static explicit operator Cell(byte Value)
        //{
        //    return new Cell(Value);
        //}

        //public static explicit operator short(Cell C)
        //{
        //    return C.valueSHORT;
        //}

        //public static explicit operator Cell(short Value)
        //{
        //    return new Cell(Value);
        //}

        //public static explicit operator int(Cell C)
        //{
        //    return C.valueINT;
        //}

        //public static explicit operator Cell(int Value)
        //{
        //    return new Cell(Value);
        //}

        //public static explicit operator long(Cell C)
        //{
        //    return C.valueLONG;
        //}

        //public static explicit operator Cell(long Value)
        //{
        //    return new Cell(Value);
        //}

        //public static explicit operator float(Cell C)
        //{
        //    return C.valueSINGLE;
        //}

        //public static explicit operator Cell(float Value)
        //{
        //    return new Cell(Value);
        //}

        //public static explicit operator double(Cell C)
        //{
        //    return C.valueDOUBLE;
        //}

        //public static explicit operator Cell(double Value)
        //{
        //    return new Cell(Value);
        //}

        //public static explicit operator byte[](Cell C)
        //{
        //    return C.valueBINARY;
        //}

        //public static explicit operator Cell(byte[] Value)
        //{
        //    return new Cell(Value);
        //}

        //public static explicit operator string(Cell C)
        //{
        //    return (C.Affinity == CellAffinity.BSTRING ? C.valueBSTRING : C.valueCSTRING);
        //}

        //public static explicit operator Cell(string Value)
        //{
        //    return new Cell(Value);
        //}

        #endregion

        #region Debug

        /// <summary>
        /// A string representing: Affinity : IsNull : Value
        /// </summary>
        internal string Chime
        {
            get
            {
                return this.Affinity.ToString() + " : " + this.IsNull.ToString() + " : " + this.ToString();
            }
        }


        #endregion

    }

}
