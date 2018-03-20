using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticWind.Elements
{

    /// <summary>
    /// Support for special cell values
    /// </summary>
    public static class CellValues
    {

        public static Cell Null(CellAffinity Affinty)
        {

            if (Affinty == CellAffinity.VARIANT)
                throw new Exception("The variant affinity is invalid");

            switch (Affinty)
            {

                case CellAffinity.BYTE:
                    return NullBYTE;
                case CellAffinity.SHORT:
                    return NullSHORT;
                case CellAffinity.INT:
                    return NullINT;
                case CellAffinity.LONG:
                    return NullLONG;
                case CellAffinity.SINGLE:
                    return NullSINGLE;
                case CellAffinity.DOUBLE:
                    return NullDOUBLE;
                case CellAffinity.DATE_TIME:
                    return NullDATE;
                case CellAffinity.BINARY:
                    return NullBLOB;
                case CellAffinity.BSTRING:
                    return NullBSTRING;
                case CellAffinity.CSTRING:
                    return NullCSTRING;

            }

            throw new Exception(string.Format("Affinity '{0}' is invalid", Affinty));

        }

        public static Cell Zero(CellAffinity Affinty)
        {

            if (Affinty == CellAffinity.VARIANT)
                throw new Exception("The variant affinity is invalid");

            switch (Affinty)
            {

                case CellAffinity.BYTE:
                    return ZeroBYTE;
                case CellAffinity.SHORT:
                    return ZeroSHORT;
                case CellAffinity.INT:
                    return ZeroINT;
                case CellAffinity.LONG:
                    return ZeroLONG;
                case CellAffinity.SINGLE:
                    return ZeroSINGLE;
                case CellAffinity.DOUBLE:
                    return ZeroDOUBLE;
                case CellAffinity.DATE_TIME:
                    return ZeroDATE;
                case CellAffinity.BINARY:
                    return ZeroBLOB;
                case CellAffinity.BSTRING:
                    return ZeroBSTRING;
                case CellAffinity.CSTRING:
                    return ZeroCSTRING;

            }

            throw new Exception(string.Format("Affinity '{0}' is invalid", Affinty));

        }

        public static Cell One(CellAffinity Affinty)
        {

            if (Affinty == CellAffinity.VARIANT)
                throw new Exception("The variant affinity is invalid");

            switch (Affinty)
            {

                case CellAffinity.BYTE:
                    return OneBYTE;
                case CellAffinity.SHORT:
                    return OneSHORT;
                case CellAffinity.INT:
                    return OneINT;
                case CellAffinity.LONG:
                    return OneLONG;
                case CellAffinity.SINGLE:
                    return OneSINGLE;
                case CellAffinity.DOUBLE:
                    return OneDOUBLE;
                case CellAffinity.DATE_TIME:
                    return OneDATE;
                case CellAffinity.BINARY:
                    return OneBLOB;
                case CellAffinity.BSTRING:
                    return OneBSTRING;
                case CellAffinity.CSTRING:
                    return OneCSTRING;

            }

            throw new Exception(string.Format("Affinity '{0}' is invalid", Affinty));

        }

        public static Cell Min(CellAffinity Affinty)
        {

            if (Affinty == CellAffinity.VARIANT)
                throw new Exception("The variant affinity is invalid");

            switch (Affinty)
            {

                case CellAffinity.BYTE:
                    return MinBYTE;
                case CellAffinity.SHORT:
                    return MinSHORT;
                case CellAffinity.INT:
                    return MinINT;
                case CellAffinity.LONG:
                    return MinLONG;
                case CellAffinity.SINGLE:
                    return MinSINGLE;
                case CellAffinity.DOUBLE:
                    return MinDOUBLE;
                case CellAffinity.DATE_TIME:
                    return MinDATE;
                case CellAffinity.BINARY:
                    return MinBLOB;
                case CellAffinity.BSTRING:
                    return MinBSTRING;
                case CellAffinity.CSTRING:
                    return MinCSTRING;

            }

            throw new Exception(string.Format("Affinity '{0}' is invalid", Affinty));

        }

        public static Cell Max(CellAffinity Affinty)
        {

            if (Affinty == CellAffinity.VARIANT)
                throw new Exception("The variant affinity is invalid");

            switch (Affinty)
            {

                case CellAffinity.BYTE:
                    return MaxBYTE;
                case CellAffinity.SHORT:
                    return MaxSHORT;
                case CellAffinity.INT:
                    return MaxINT;
                case CellAffinity.LONG:
                    return MaxLONG;
                case CellAffinity.SINGLE:
                    return MaxSINGLE;
                case CellAffinity.DOUBLE:
                    return MaxDOUBLE;
                case CellAffinity.DATE_TIME:
                    return MaxDATE;
                case CellAffinity.BINARY:
                    return MaxBLOB;
                case CellAffinity.BSTRING:
                    return MaxBSTRING;
                case CellAffinity.CSTRING:
                    return MaxCSTRING;

            }

            throw new Exception(string.Format("Affinity '{0}' is invalid", Affinty));

        }

        public static Cell Empty(CellAffinity Affinty)
        {

            if (Affinty == CellAffinity.VARIANT)
                throw new Exception("The variant affinity is invalid");

            switch (Affinty)
            {

                case CellAffinity.BYTE:
                    return EmptyBYTE;
                case CellAffinity.SHORT:
                    return EmptySHORT;
                case CellAffinity.INT:
                    return EmptyINT;
                case CellAffinity.LONG:
                    return EmptyLONG;
                case CellAffinity.SINGLE:
                    return EmptySINGLE;
                case CellAffinity.DOUBLE:
                    return EmptyDOUBLE;
                case CellAffinity.DATE_TIME:
                    return EmptyDATE;
                case CellAffinity.BINARY:
                    return EmptyBLOB;
                case CellAffinity.BSTRING:
                    return EmptyBSTRING;
                case CellAffinity.CSTRING:
                    return EmptyCSTRING;

            }

            throw new Exception(string.Format("Affinity '{0}' is invalid", Affinty));

        }

        // BOOL
        public static Cell NullBOOL = new Cell(CellAffinity.BOOL);
        public static Cell ZeroBOOL = new Cell(false);
        public static Cell OneBOOL = new Cell(true);
        public static Cell MinBOOL = new Cell(false);
        public static Cell MaxBOOL = new Cell(true);
        public static Cell EmptyBOOL = new Cell(false);

        // BYTE
        public static Cell NullBYTE = new Cell(CellAffinity.BYTE);
        public static Cell ZeroBYTE = new Cell((byte)0);
        public static Cell OneBYTE = new Cell((byte)1);
        public static Cell MinBYTE = new Cell(byte.MinValue);
        public static Cell MaxBYTE = new Cell(byte.MaxValue);
        public static Cell EmptyBYTE = new Cell((byte)0);

        // SHORT
        public static Cell NullSHORT = new Cell(CellAffinity.SHORT);
        public static Cell ZeroSHORT = new Cell((short)0);
        public static Cell OneSHORT = new Cell((short)1);
        public static Cell MinSHORT = new Cell(short.MinValue);
        public static Cell MaxSHORT = new Cell(short.MaxValue);
        public static Cell EmptySHORT = new Cell((short)0);

        // INT
        public static Cell NullINT = new Cell(CellAffinity.INT);
        public static Cell ZeroINT = new Cell((int)0);
        public static Cell OneINT = new Cell((int)1);
        public static Cell MinINT = new Cell(int.MinValue);
        public static Cell MaxINT = new Cell(int.MaxValue);
        public static Cell EmptyINT = new Cell((int)0);

        // LONG
        public static Cell NullLONG = new Cell(CellAffinity.LONG);
        public static Cell ZeroLONG = new Cell((long)0);
        public static Cell OneLONG = new Cell((long)1);
        public static Cell MinLONG = new Cell(long.MinValue);
        public static Cell MaxLONG = new Cell(long.MaxValue);
        public static Cell EmptyLONG = new Cell((long)0);

        // SINGLE
        public static Cell NullSINGLE = new Cell(CellAffinity.SINGLE);
        public static Cell ZeroSINGLE = new Cell((float)0);
        public static Cell OneSINGLE = new Cell((float)1);
        public static Cell MinSINGLE = new Cell(float.MinValue);
        public static Cell MaxSINGLE = new Cell(float.MaxValue);
        public static Cell EmptySINGLE = new Cell((float)0);

        // DOUBLE
        public static Cell NullDOUBLE = new Cell(CellAffinity.DOUBLE);
        public static Cell ZeroDOUBLE = new Cell((double)0);
        public static Cell OneDOUBLE = new Cell((double)1);
        public static Cell MinDOUBLE = new Cell(double.MinValue);
        public static Cell MaxDOUBLE = new Cell(double.MaxValue);
        public static Cell EmptyDOUBLE = new Cell((double)0);

        // DATE_TIME
        public static Cell NullDATE = new Cell(CellAffinity.DATE_TIME);
        public static Cell ZeroDATE = new Cell(DateTime.MinValue);
        public static Cell OneDATE = new Cell(DateTime.MaxValue);
        public static Cell MinDATE = new Cell(DateTime.MinValue);
        public static Cell MaxDATE = new Cell(DateTime.MaxValue);
        public static Cell EmptyDATE = new Cell(DateTime.MinValue);

        // BINARY
        public static Cell NullBLOB = new Cell(CellAffinity.BINARY);
        public static Cell ZeroBLOB = new Cell(new byte[1] { 0 });
        public static Cell OneBLOB = new Cell(new byte[1] { 1 });
        public static Cell MinBLOB = new Cell(new byte[0] { });
        public static Cell MaxBLOB = new Cell(System.Text.ASCIIEncoding.UTF8.GetBytes(new string(char.MaxValue, 2048)));
        public static Cell EmptyBLOB = new Cell(new byte[0] { });

        // CSTRING
        public static Cell NullCSTRING = new Cell(CellAffinity.CSTRING);
        public static Cell ZeroCSTRING = new Cell("ZERO");
        public static Cell OneCSTRING = new Cell("ONE");
        public static Cell MinCSTRING = new Cell("");
        public static Cell MaxCSTRING = new Cell(new string(char.MaxValue, 4096));
        public static Cell EmptyCSTRING = new Cell("");

        // BSTRING
        public static Cell NullBSTRING = new Cell(CellAffinity.BSTRING);
        public static Cell ZeroBSTRING = new Cell(new BString("ZERO"));
        public static Cell OneBSTRING = new Cell(new BString("ONE"));
        public static Cell MinBSTRING = new Cell(BString.Empty);
        public static Cell MaxBSTRING = new Cell(new BString(255, 4096));
        public static Cell EmptyBSTRING = new Cell(BString.Empty);

        // Special Values //
        public static Cell Pi = new Cell(Math.PI);
        public static Cell E = new Cell(Math.E);
        public static Cell Epsilon = new Cell(double.Epsilon);
        public static Cell True = new Cell(true);
        public static Cell False = new Cell(false);


    }


}
