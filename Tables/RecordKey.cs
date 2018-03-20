using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using ArcticWind.Elements;


namespace ArcticWind.Tables
{

    [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit)]
    public struct RecordKey
    {

        [System.Runtime.InteropServices.FieldOffset(0)]
        internal long U_ID;

        [System.Runtime.InteropServices.FieldOffset(0)]
        internal int PAGE_ID;

        [System.Runtime.InteropServices.FieldOffset(4)]
        internal int ROW_ID;

        public RecordKey(int PageID, int RowID)
        {
            this.U_ID = 0;
            this.PAGE_ID = PageID;
            this.ROW_ID = RowID;
        }

        public RecordKey(long UID)
        {
            this.PAGE_ID = 0;
            this.ROW_ID = 0;
            this.U_ID = UID;
        }

        public RecordKey(Cell Element)
            : this(Element.INT_A, Element.INT_B)
        {
        }

        public long UID
        {
            get { return this.U_ID; }
        }

        public int PageID
        {
            get { return this.PAGE_ID; }
        }

        public int RowID
        {
            get { return this.ROW_ID; }
        }

        public Cell Element
        {
            get { return new Cell(this.PAGE_ID, this.ROW_ID); }
        }

        public bool IsNotFound
        {
            get { return this.PAGE_ID == -1 && this.ROW_ID == -1; }
        }

        public override string ToString()
        {
            return string.Format("<{0},{1}>", this.PAGE_ID, this.ROW_ID);
        }

        public static long GetUID(int PageID, int RowID)
        {
            return new RecordKey(PageID, RowID).U_ID;
        }

        public static int GetPageID(long UID)
        {
            return new RecordKey(UID).PAGE_ID;
        }

        public static int GetRowID(long UID)
        {
            return new RecordKey(UID).ROW_ID;
        }

        public static RecordKey RecordNotFound
        {
            get { return new RecordKey(-1, -1); }
        }

        public static bool operator == (RecordKey A, RecordKey B)
        {
            if (A.PAGE_ID == B.PAGE_ID && A.ROW_ID == B.ROW_ID)
                return true;
            return false;
        }

        public static bool operator !=(RecordKey A, RecordKey B)
        {
            if (A.PAGE_ID == B.PAGE_ID && A.ROW_ID == B.ROW_ID)
                return false;
            return true;
        }

        public static bool operator <(RecordKey A, RecordKey B)
        {
            if (A.PAGE_ID == B.PAGE_ID)
                return A.ROW_ID < B.ROW_ID;
            return A.PAGE_ID < B.PAGE_ID;
        }

        public static bool operator <=(RecordKey A, RecordKey B)
        {
            if (A.PAGE_ID == B.PAGE_ID)
                return A.ROW_ID <= B.ROW_ID;
            return A.PAGE_ID <= B.PAGE_ID;
        }

        public static bool operator >(RecordKey A, RecordKey B)
        {
            if (A.PAGE_ID == B.PAGE_ID)
                return A.ROW_ID > B.ROW_ID;
            return A.PAGE_ID > B.PAGE_ID;
        }

        public static bool operator >=(RecordKey A, RecordKey B)
        {
            if (A.PAGE_ID == B.PAGE_ID)
                return A.ROW_ID >= B.ROW_ID;
            return A.PAGE_ID >= B.PAGE_ID;
        }

        public static RecordKey Min(RecordKey A, RecordKey B)
        {
            return A < B ? A : B;
        }

        public static RecordKey Max(RecordKey A, RecordKey B)
        {
            return A > B ? A : B;
        }

    }


}
