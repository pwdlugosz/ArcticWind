using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticWind.Elements
{


    /// <summary>
    /// An array of cells with the same type
    /// </summary>
    public class CellMatrix : IEnumerable<Cell>, IEnumerable
    {

        // Private Variables //
        protected int _Rows = -1; // Rows
        protected int _Columns = -1; // Columns
        protected Cell[,] _Data;
        protected CellAffinity _Affinity;
        protected int _Size;

        // Constructors //
        /// <summary>
        /// Create a matrix with a default Value
        /// </summary>
        /// <param name="Rows"></param>
        /// <param name="Columns"></param>
        /// <param name="Value"></param>
        public CellMatrix(int Rows, int Columns, Cell Value)
        {

            // Check to see if the rows and columns are valid //
            if (Rows < 1 || Columns < 1)
            {
                throw new IndexOutOfRangeException("Row " + Rows.ToString() + " or column  " + Columns.ToString() + "  submitted is invalid");
            }

            this._Size = CellSerializer.Length(Value);

            // Build Matrix //
            this._Rows = Rows;
            this._Columns = Columns;

            this._Data = new Cell[this._Rows, this._Columns];
            for (int i = 0; i < this._Rows; i++)
                for (int j = 0; j < this._Columns; j++)
                    this._Data[i, j] = Value;
            this._Affinity = Value.AFFINITY;

        }

        /// <summary>
        /// Builds a new matrix
        /// </summary>
        /// <param name="Rows"></param>
        /// <param name="Columns"></param>
        public CellMatrix(int Rows, int Columns, CellAffinity UseAffinity, int Size)
            : this(Rows, Columns, new Cell(UseAffinity))
        {
            this._Size = Size;
        }

        /// <summary>
        /// Create a cell matrix from another matrix, effectively cloning the matrix
        /// </summary>
        /// <param name="A"></param>
        public CellMatrix(CellMatrix A)
        {

            // Build Matrix //
            this._Rows = A._Rows;
            this._Columns = A._Columns;
            this._Size = A._Size;

            this._Data = new Cell[this._Rows, this._Columns];
            for (int i = 0; i < this._Rows; i++)
                for (int j = 0; j < this._Columns; j++)
                    this._Data[i, j] = A[i, j];
            this._Affinity = A.Affinity;

        }

        /// <summary>
        /// Creates a matrix given a record
        /// </summary>
        /// <param name="Elements"></param>
        /// <param name="UseAffinity"></param>
        public CellMatrix(Record Data, CellAffinity UseAffinity, int Size)
            :this(Data.Count, 1, UseAffinity, Size)
        {
            for (int i = 0; i < Data.Count; i++)
            {
                this[i] = Data[i];
            }
        }

        // Public Properties //
        /// <summary>
        /// Gets count of matrix rows
        /// </summary>
        public int RowCount
        {
            get
            {
                return this._Rows;
            }
        }

        /// <summary>
        /// Gets count of matrix columns
        /// </summary>
        public int ColumnCount
        {
            get
            {
                return this._Columns;
            }
        }

        /// <summary>
        /// Gets or sets the element of a matrix
        /// </summary>
        /// <param name="Row"></param>
        /// <param name="Column"></param>
        /// <returns></returns>
        public virtual Cell this[int Row, int Column]
        {

            get
            {
                return this._Data[Row, Column];
            }
            set
            {
                if (value.AFFINITY == this._Affinity)
                    this._Data[Row, Column] = value;
                else
                    this._Data[Row, Column] = CellConverter.Cast(value, this._Affinity);
            }

        }

        /// <summary>
        /// Gets or sets an element
        /// </summary>
        /// <param name="Row"></param>
        /// <returns></returns>
        public virtual Cell this[int Row]
        {

            get
            {
                return this._Data[Row, 0];
            }
            set
            {
                if (value.AFFINITY == this._Affinity)
                    this._Data[Row, 0] = value;
                else
                    this._Data[Row, 0] = CellConverter.Cast(value, this._Affinity);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSquare
        {
            get
            {
                return this.RowCount == this.ColumnCount;
            }
        }

        /// <summary>
        /// Gets the matrix determinate
        /// </summary>
        public Cell Determinate
        {
            get { return new LUDecomposition(this).det(); }
        }

        /// <summary>
        /// True if any cell Value is null
        /// </summary>
        public bool AnyNull
        {
            get
            {
                for (int i = 0; i < this.RowCount; i++)
                {
                    for (int j = 0; j < this.ColumnCount; j++)
                    {
                        if (this[i, j].IsNull)
                            return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Returns the affinity of the matrix
        /// </summary>
        public CellAffinity Affinity
        {
            get { return this._Affinity; }
            set
            {
                if (value == this._Affinity)
                    return;
                for (int i = 0; i < this._Rows; i++)
                {
                    for (int j = 0; j < this._Columns; j++)
                    {
                        this._Data[i, j] = CellConverter.Cast(this._Data[i, j], value);
                    }
                }
                this._Affinity = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Size
        {
            get { return this._Size; }
        }

        /// <summary>
        /// Computes the inverse of a matrix
        /// </summary>
        public CellMatrix Inverse
        {
            get
            {
                return CellMatrix.Invert(this);
            }
        }

        /// <summary>
        /// Comuptes the transposition of a matrix
        /// </summary>
        public CellMatrix Transposition
        {
            get
            {
                return ~this;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Matrix"></param>
        /// <param name="SearchValue"></param>
        /// <returns></returns>
        public Cell Match(Cell SearchValue)
        {
            Cell x = CellValues.NullLONG;
            for (int i = 0; i < this.RowCount; i++)
            {

                for (int j = 0; j < this.ColumnCount; j++)
                {
                    if (this[i, j] == SearchValue)
                        return new Cell(i, j);
                }

            }

            return x;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Matrix"></param>
        /// <param name="SearchValue"></param>
        /// <returns></returns>
        public CellMatrix Match2(Cell SearchValue)
        {

            List<Cell> x = new List<Cell>();
            for (int i = 0; i < this.RowCount; i++)
            {

                for (int j = 0; j < this.ColumnCount; j++)
                {
                    if (this[i, j] == SearchValue)
                        x.Add(new Cell(i, j));
                }

            }

            return new CellMatrix(new Record(x.ToArray()), this.Affinity, this.Size);

        }

        // Methods //
        /// <summary>
        /// Checks to see if the given rows/columns are valid
        /// </summary>
        /// <param name="Row"></param>
        /// <param name="Column"></param>
        /// <returns></returns>
        private bool CheckBounds(int Row, int Column)
        {
            if (Row >= 0 && Row < this.RowCount && Column >= 0 && Column < this.ColumnCount)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public CellMatrix Shell()
        {
            return new CellMatrix(this._Rows, this._Columns, this._Affinity, this._Size);
        }

        /// <summary>
        /// Gets a string Value of the matrix
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this._Rows * this._Columns);
            for (int i = 0; i < this.RowCount; i++)
            {

                for (int j = 0; j < this.ColumnCount; j++)
                {
                    Cell x = this[i, j];
                    //string v = x.valueCSTRING;
                    BString q = x.valueBSTRING;

                    sb.Append(q);
                    if (j != this.ColumnCount - 1)
                        sb.Append(",");
                }
                if (i != this.RowCount - 1)
                    sb.Append('\n');

            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public CellMatrix CloneOfMe()
        {

            return new CellMatrix(this);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new MatrixEnumerator(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator<Cell> IEnumerable<Cell>.GetEnumerator()
        {
            return new MatrixEnumerator(this);
        }

        #region Opperators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        private static bool CheckDimensions(CellMatrix A, CellMatrix B)
        {
            return (A.RowCount == B.RowCount && A.ColumnCount == B.ColumnCount);
        }

        /// <summary>
        /// Use for checking matrix multiplication
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        private static bool CheckDimensions2(CellMatrix A, CellMatrix B)
        {
            return (A.ColumnCount == B.RowCount);
        }

        /// <summary>
        /// Adds two matricies together
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static CellMatrix operator +(CellMatrix A)
        {

            // Build a matrix //
            CellMatrix C = new CellMatrix(A.RowCount, A.ColumnCount, A.Affinity, A.Size);

            // Main loop //
            for (int i = 0; i < A.RowCount; i++)
            {
                for (int j = 0; j < A.ColumnCount; j++)
                {
                    C[i, j] = +A[i, j];
                }
            }

            // Return //
            return C;

        }

        /// <summary>
        /// Adds two matricies together
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static CellMatrix operator +(CellMatrix A, CellMatrix B)
        {

            // Check bounds are the same //
            if (CellMatrix.CheckDimensions(A, B) == false)
            {
                throw new Exception(string.Format("Dimension mismatch A {0}OriginalPage{1} B {2}OriginalPage{3}", A.RowCount, A.ColumnCount, B.RowCount, B.ColumnCount));
            }

            // Build a matrix //
            CellMatrix C = new CellMatrix(A.RowCount, A.ColumnCount, A.Affinity, A.Size);

            // Main loop //
            for (int i = 0; i < A.RowCount; i++)
            {
                for (int j = 0; j < A.ColumnCount; j++)
                {
                    C[i, j] = A[i, j] + B[i, j];
                }
            }

            // Return //
            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static CellMatrix operator +(Cell A, CellMatrix B)
        {
            CellMatrix C = new CellMatrix(B.RowCount, B.ColumnCount, A.AFFINITY, B.Size);
            for (int i = 0; i < B.RowCount; i++)
            {
                for (int j = 0; j < B.ColumnCount; j++)
                {
                    C._Data[i, j] = A + B._Data[i, j];
                }
            }
            return C;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static CellMatrix operator +(CellMatrix A, Cell B)
        {
            CellMatrix C = new CellMatrix(A.RowCount, A.ColumnCount, A.Affinity, A.Size);
            for (int i = 0; i < A.RowCount; i++)
            {
                for (int j = 0; j < A.ColumnCount; j++)
                {
                    C._Data[i, j] = A._Data[i, j] + B;
                }
            }
            return C;
        }

        /// <summary>
        /// Negates a matrix
        /// </summary>
        /// <param name="A">Matrix to negate</param>
        /// <returns>0 - A</returns>
        public static CellMatrix operator -(CellMatrix A)
        {

            // Build a matrix //
            CellMatrix C = new CellMatrix(A.RowCount, A.ColumnCount, A.Affinity, A.Size);

            // Main loop //
            for (int i = 0; i < A.RowCount; i++)
            {
                for (int j = 0; j < A.ColumnCount; j++)
                {
                    C[i, j] = -A[i, j];
                }
            }

            // Return //
            return C;

        }

        /// <summary>
        /// Subtracts two matricies
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static CellMatrix operator -(CellMatrix A, CellMatrix B)
        {

            // Check bounds are the same //
            if (CellMatrix.CheckDimensions(A, B) == false)
            {
                throw new Exception(string.Format("Dimension mismatch A {0}OriginalPage{1} B {2}OriginalPage{3}", A.RowCount, A.ColumnCount, B.RowCount, B.ColumnCount));
            }

            // Build a matrix //
            CellMatrix C = new CellMatrix(A.RowCount, A.ColumnCount, A.Affinity, A.Size);

            // Main loop //
            for (int i = 0; i < A.RowCount; i++)
            {
                for (int j = 0; j < A.ColumnCount; j++)
                {
                    C[i, j] = A[i, j] - B[i, j];
                }
            }

            // Return //
            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static CellMatrix operator -(Cell A, CellMatrix B)
        {
            CellMatrix C = new CellMatrix(B.RowCount, B.ColumnCount, A.AFFINITY, B.Size);
            for (int i = 0; i < B.RowCount; i++)
            {
                for (int j = 0; j < B.ColumnCount; j++)
                {
                    C._Data[i, j] = A - B._Data[i, j];
                }
            }
            return C;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static CellMatrix operator -(CellMatrix A, Cell B)
        {
            CellMatrix C = new CellMatrix(A.RowCount, A.ColumnCount, A.Affinity, A.Size);
            for (int i = 0; i < A.RowCount; i++)
            {
                for (int j = 0; j < A.ColumnCount; j++)
                {
                    C._Data[i, j] = A._Data[i, j] - B;
                }
            }
            return C;
        }

        /// <summary>
        /// Multiplies each element in a matrix by each other element in another matrix
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static CellMatrix operator *(CellMatrix A, CellMatrix B)
        {

            // Check bounds are the same //
            if (CellMatrix.CheckDimensions(A, B) == false)
            {
                throw new Exception(string.Format("Dimension mismatch A {0}OriginalPage{1} B {2}OriginalPage{3}", A.RowCount, A.ColumnCount, B.RowCount, B.ColumnCount));
            }

            // Build a matrix //
            CellMatrix C = new CellMatrix(A.RowCount, A.ColumnCount, A.Affinity, A.Size);

            // Main loop //
            for (int i = 0; i < A.RowCount; i++)
            {
                for (int j = 0; j < A.ColumnCount; j++)
                {
                    C[i, j] = A[i, j] * B[i, j];
                }
            }

            // Return //
            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static CellMatrix operator *(Cell A, CellMatrix B)
        {
            CellMatrix C = new CellMatrix(B.RowCount, B.ColumnCount, A.AFFINITY, B.Size);
            for (int i = 0; i < B.RowCount; i++)
            {
                for (int j = 0; j < B.ColumnCount; j++)
                {
                    C._Data[i, j] = A * B._Data[i, j];
                }
            }
            return C;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static CellMatrix operator *(CellMatrix A, Cell B)
        {
            CellMatrix C = new CellMatrix(A.RowCount, A.ColumnCount, A.Affinity, A.Size);
            for (int i = 0; i < A.RowCount; i++)
            {
                for (int j = 0; j < A.ColumnCount; j++)
                {
                    C._Data[i, j] = A._Data[i, j] * B;
                }
            }
            return C;
        }

        /// <summary>
        /// Dividies each element in a matrix by each other element in another matrix
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static CellMatrix operator /(CellMatrix A, CellMatrix B)
        {

            // Check bounds are the same //
            if (CellMatrix.CheckDimensions(A, B) == false)
            {
                throw new Exception(string.Format("Dimension mismatch A {0}OriginalPage{1} B {2}OriginalPage{3}", A.RowCount, A.ColumnCount, B.RowCount, B.ColumnCount));
            }

            // Build a matrix //
            CellMatrix C = new CellMatrix(A.RowCount, A.ColumnCount, A.Affinity, A.Size);

            // Main loop //
            for (int i = 0; i < A.RowCount; i++)
            {
                for (int j = 0; j < A.ColumnCount; j++)
                {
                    C[i, j] = A[i, j] / B[i, j];
                }
            }

            // Return //
            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static CellMatrix operator /(Cell A, CellMatrix B)
        {
            CellMatrix C = new CellMatrix(B.RowCount, B.ColumnCount, A.AFFINITY, B.Size);
            for (int i = 0; i < B.RowCount; i++)
            {
                for (int j = 0; j < B.ColumnCount; j++)
                {
                    C._Data[i, j] = A / B._Data[i, j];
                }
            }
            return C;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static CellMatrix operator /(CellMatrix A, Cell B)
        {
            CellMatrix C = new CellMatrix(A.RowCount, A.ColumnCount, A.Affinity, A.Size);
            for (int i = 0; i < A.RowCount; i++)
            {
                for (int j = 0; j < A.ColumnCount; j++)
                {
                    C._Data[i, j] = A._Data[i, j] / B;
                }
            }
            return C;
        }

        /// <summary>
        /// Divides a matrix by another matrix, but checks for N / 0 erros
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static CellMatrix CheckDivide(CellMatrix A, CellMatrix B)
        {

            // Check bounds are the same //
            if (CellMatrix.CheckDimensions(A, B) == false)
            {
                throw new Exception(string.Format("Dimension mismatch A {0}OriginalPage{1} B {2}OriginalPage{3}", A.RowCount, A.ColumnCount, B.RowCount, B.ColumnCount));
            }

            // Build a matrix //
            CellMatrix C = new CellMatrix(A.RowCount, A.ColumnCount, A.Affinity, A.Size);

            // Main loop //
            for (int i = 0; i < A.RowCount; i++)
            {
                for (int j = 0; j < A.ColumnCount; j++)
                {
                    C[i, j] = B[i, j].IsZero ? CellValues.Zero(A[i, j].Affinity) : A[i, j] / B[i, j];
                }
            }

            // Return //
            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static CellMatrix CheckDivide(Cell A, CellMatrix B)
        {
            CellMatrix C = new CellMatrix(B.RowCount, B.ColumnCount, A.AFFINITY, B.Size);
            for (int i = 0; i < B.RowCount; i++)
            {
                for (int j = 0; j < B.ColumnCount; j++)
                {
                    C._Data[i, j] = Cell.CheckDivide(A, B._Data[i, j]);
                }
            }
            return C;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static CellMatrix CheckDivide(CellMatrix A, Cell B)
        {
            CellMatrix C = new CellMatrix(A.RowCount, A.ColumnCount, A.Affinity, A.Size);
            for (int i = 0; i < A.RowCount; i++)
            {
                for (int j = 0; j < A.ColumnCount; j++)
                {
                    C._Data[i, j] = Cell.CheckDivide(A._Data[i, j], B);
                }
            }
            return C;
        }

        /// <summary>
        /// Dividies each element in a matrix by each other element in another matrix
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static CellMatrix operator %(CellMatrix A, CellMatrix B)
        {

            // Check bounds are the same //
            if (CellMatrix.CheckDimensions(A, B) == false)
            {
                throw new Exception(string.Format("Dimension mismatch A {0}OriginalPage{1} B {2}OriginalPage{3}", A.RowCount, A.ColumnCount, B.RowCount, B.ColumnCount));
            }

            // Build a matrix //
            CellMatrix C = new CellMatrix(A.RowCount, A.ColumnCount, A.Affinity, A.Size);

            // Main loop //
            for (int i = 0; i < A.RowCount; i++)
            {
                for (int j = 0; j < A.ColumnCount; j++)
                {
                    C[i, j] = A[i, j] % B[i, j];
                }
            }

            // Return //
            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static CellMatrix operator %(Cell A, CellMatrix B)
        {
            CellMatrix C = new CellMatrix(B.RowCount, B.ColumnCount, A.AFFINITY, B.Size);
            for (int i = 0; i < B.RowCount; i++)
            {
                for (int j = 0; j < B.ColumnCount; j++)
                {
                    C._Data[i, j] = A % B._Data[i, j];
                }
            }
            return C;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static CellMatrix operator %(CellMatrix A, Cell B)
        {
            CellMatrix C = new CellMatrix(A.RowCount, A.ColumnCount, A.Affinity, A.Size);
            for (int i = 0; i < A.RowCount; i++)
            {
                for (int j = 0; j < A.ColumnCount; j++)
                {
                    C._Data[i, j] = A._Data[i, j] % B;
                }
            }
            return C;
        }

        /// <summary>
        /// Performs the true matrix multiplication between two matricies
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static CellMatrix operator ^(CellMatrix A, CellMatrix B)
        {

            if (CellMatrix.CheckDimensions2(A, B) == false)
            {
                throw new Exception(string.Format("Dimension mismatch A {0}:{1} B {2}:{3}", A.RowCount, A.ColumnCount, B.RowCount, B.ColumnCount));
            }

            CellMatrix C = new CellMatrix(A.RowCount, B.ColumnCount, CellValues.Zero(A.Affinity));

            // Main Loop //
            for (int i = 0; i < A.RowCount; i++)
            {

                // Sub Loop One //
                for (int j = 0; j < B.ColumnCount; j++)
                {

                    // Sub Loop Two //
                    for (int k = 0; k < A.ColumnCount; k++)
                    {

                        C[i, j] = C[i, j] + A[i, k] * B[k, j];

                    }

                }

            }

            // Return C //
            return C;

        }

        /// <summary>
        /// Inverts a matrix
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public static CellMatrix operator !(CellMatrix A)
        {
            return CellMatrix.Invert(A);
        }

        /// <summary>
        /// Transposes a matrix
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public static CellMatrix operator ~(CellMatrix A)
        {

            // Create Another Matrix //
            CellMatrix B = new CellMatrix(A.ColumnCount, A.RowCount, A.Affinity, A.Size);

            // Loop through A and copy element to B //
            for (int i = 0; i < A.RowCount; i++)
                for (int j = 0; j < A.ColumnCount; j++)
                    B[j, i] = A[i, j];

            // Return //
            return B;

        }

        /// <summary>
        /// Returns the identity matrix given a dimension
        /// </summary>
        /// <param name="Dimension"></param>
        /// <returns></returns>
        public static CellMatrix Identity(int Dimension, CellAffinity Affinity)
        {

            // Check that a positive number was passed //
            if (Dimension < 1)
                throw new Exception("Dimension must be greater than or equal to 1");

            // Create a matrix //
            CellMatrix A = new CellMatrix(Dimension, Dimension, Affinity, 8);

            for (int i = 0; i < Dimension; i++)
            {
                for (int j = 0; j < Dimension; j++)
                {
                    if (i != j)
                        A[i, j] = CellValues.Zero(A.Affinity);
                    else
                        A[i, j] = CellValues.One(A.Affinity);
                }
            }

            return A;

        }

        public static int Compare(CellMatrix A, CellMatrix B)
        {

            if (A.RowCount != B.RowCount)
                return A.RowCount - B.RowCount;
            else if (A.ColumnCount != B.ColumnCount)
                return A.ColumnCount - B.ColumnCount;

            for (int i = 0; i < A.RowCount; i++)
            {
                for (int j = 0; j < A.RowCount; j++)
                {
                    int c = CellComparer.Compare(A[i, j], B[i, j]);
                    if (c != 0) return c;
                }
            }

            return 0;

        }

        public static bool operator ==(CellMatrix A, CellMatrix B)
        {

            if (A.RowCount != B.RowCount || A.ColumnCount != B.ColumnCount)
                return false;

            for (int i = 0; i < A.RowCount; i++)
            {
                for (int j = 0; j < A.ColumnCount; j++)
                {
                    if (A[i, j] != B[i, j])
                        return false;
                }
            }
            return true;

        }

        public static bool operator !=(CellMatrix A, CellMatrix B)
        {

            if (A.RowCount != B.RowCount || A.ColumnCount != B.ColumnCount)
                return true;

            for (int i = 0; i < A.RowCount; i++)
            {
                for (int j = 0; j < A.ColumnCount; j++)
                {
                    if (A[i, j] != B[i, j])
                        return true;
                }
            }
            return false;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public static CellMatrix Invert(CellMatrix A)
        {

            // New decomposition //
            LUDecomposition lu = new LUDecomposition(A);

            // New identity //
            CellMatrix I = CellMatrix.Identity(A.RowCount, A.Affinity);

            return lu.solve(I);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public static Cell Sum(CellMatrix A)
        {

            Cell d = CellValues.Zero(A.Affinity);
            for (int i = 0; i < A.RowCount; i++)
                for (int j = 0; j < A.ColumnCount; j++)
                    d += A[i, j];
            return d;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public static Cell Min(CellMatrix A)
        {

            Cell d = CellValues.Max(A.Affinity);
            for (int i = 0; i < A.RowCount; i++)
                for (int j = 0; j < A.ColumnCount; j++)
                    d = CellFunctions.Min(A[i, j], d);
            return d;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public static Cell Max(CellMatrix A)
        {

            Cell d = CellValues.Min(A.Affinity);
            for (int i = 0; i < A.RowCount; i++)
                for (int j = 0; j < A.ColumnCount; j++)
                    d = CellFunctions.Max(A[i, j], d);
            return d;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public static Cell Coalesce(CellMatrix A)
        {

            Cell d = CellValues.Null(A.Affinity);
            for (int i = 0; i < A.RowCount; i++)
                for (int j = 0; j < A.ColumnCount; j++)
                    d = (!A[i, j].IsNull ? A[i, j] : d);
            return d;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public static Cell SumSquare(CellMatrix A)
        {

            Cell d = CellValues.Zero(A.Affinity);
            for (int i = 0; i < A.RowCount; i++)
                for (int j = 0; j < A.ColumnCount; j++)
                    d += A[i, j] * A[i, j];
            return d;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public static CellMatrix Trace(CellMatrix A)
        {

            if (!A.IsSquare)
                throw new Exception(string.Format("Cannot trace a non-square matrix : {0} OriginalPage {1}", A.RowCount, A.ColumnCount));

            CellMatrix B = new CellMatrix(A.RowCount, A.RowCount, CellValues.Zero(A.Affinity));

            for (int i = 0; i < A.RowCount; i++)
                B[i, i] = A[i, i];

            return B;

        }

        #endregion

        /// <summary>
        /// This class was 'borrowed' from NIST's matrix numerics library, which was re-coded from Java
        /// </summary>
        private class LUDecomposition
        {

            private CellMatrix LU;
            private int m, n, pivsign;
            private int[] piv;
            private Cell _zero;

            public LUDecomposition(CellMatrix A)
            {

                // Use a "left-looking", dot-product, Crout/Doolittle algorithm.

                this._zero = CellValues.Zero(A.Affinity);
                LU = new CellMatrix(A);
                m = A.RowCount;
                n = A.ColumnCount;
                piv = new int[m];
                for (int i = 0; i < m; i++)
                {
                    piv[i] = i;
                }
                pivsign = 1;
                Cell[] LUcolj = new Cell[m];

                // Outer loop.

                for (int j = 0; j < n; j++)
                {

                    // Make a copy of the j-th column to localize references.

                    for (int i = 0; i < m; i++)
                    {
                        LUcolj[i] = LU[i, j];
                    }

                    // Apply previous transformations.

                    for (int i = 0; i < m; i++)
                    {

                        // Most of the time is spent in the following dot product.

                        int kmax = Math.Min(i, j);
                        Cell s = this._zero;
                        for (int k = 0; k < kmax; k++)
                        {
                            s += LU[i, k] * LUcolj[k];
                        }

                        LU[i, j] = LUcolj[i] -= s;

                    }

                    // Find pivot and exchange if necessary.

                    int p = j;
                    for (int i = j + 1; i < m; i++)
                    {
                        if (CellFunctions.Abs(LUcolj[i]) > CellFunctions.Abs(LUcolj[p]))
                        {
                            p = i;
                        }
                    }
                    if (p != j)
                    {
                        for (int k = 0; k < n; k++)
                        {
                            Cell t = LU[p, k];
                            LU[p, k] = LU[j, k];
                            LU[j, k] = t;
                        }
                        int l = piv[p];
                        piv[p] = piv[j];
                        piv[j] = l;
                        pivsign = -pivsign;
                    }

                    // Compute multipliers.

                    if (j < m & LU[j, j] != this._zero)
                    {
                        for (int i = j + 1; i < m; i++)
                        {
                            LU[i, j] /= LU[j, j];
                        }
                    }
                }
            }

            public bool isNonsingular()
            {
                for (int j = 0; j < n; j++)
                {
                    if (LU[j, j] == this._zero)
                        return false;
                }
                return true;
            }

            public CellMatrix getL()
            {

                CellMatrix L = new CellMatrix(m, n, LU.Affinity, LU.Size);
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (i > j)
                        {
                            L[i, j] = LU[i, j];
                        }
                        else if (i == j)
                        {
                            L[i, j] = CellValues.One(LU.Affinity);
                        }
                        else
                        {
                            L[i, j] = CellValues.Zero(LU.Affinity);
                        }
                    }
                }
                return L;

            }

            public CellMatrix getU()
            {

                CellMatrix X = new CellMatrix(n, n, LU.Affinity, LU.Size);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (i <= j)
                        {
                            X[i, j] = LU[i, j];
                        }
                        else
                        {
                            X[i, j] = CellValues.Zero(LU.Affinity);
                        }
                    }
                }
                return X;

            }

            public int[] getPivot()
            {

                int[] p = new int[m];
                for (int i = 0; i < m; i++)
                {
                    p[i] = piv[i];
                }
                return p;

            }

            public Cell det()
            {

                if (m != n)
                {
                    throw new Exception("Matrix must be square.");
                }
                Cell d = CellConverter.Cast(new Cell(pivsign), this.LU.Affinity);
                for (int j = 0; j < n; j++)
                {
                    d *= LU[j, j];
                }
                return d;

            }

            public CellMatrix solve(CellMatrix B)
            {

                if (B.RowCount != m)
                {
                    throw new Exception("Matrix row dimensions must agree.");
                }
                if (!this.isNonsingular())
                {
                    throw new Exception("Matrix is singular.");
                }

                // Copy right hand side with pivoting
                int nx = B.ColumnCount;
                CellMatrix X = this.getMatrix(B, piv, 0, nx - 1);

                // Solve L*Y = B(piv,:)
                for (int k = 0; k < n; k++)
                {

                    for (int i = k + 1; i < n; i++)
                    {

                        for (int j = 0; j < nx; j++)
                        {
                            X[i, j] -= X[k, j] * LU[i, k];
                        }

                    }

                }
                // Solve U*X = Y;
                for (int k = n - 1; k >= 0; k--)
                {

                    for (int j = 0; j < nx; j++)
                    {
                        X[k, j] /= LU[k, k];
                    }

                    for (int i = 0; i < k; i++)
                    {

                        for (int j = 0; j < nx; j++)
                        {
                            X[i, j] -= X[k, j] * LU[i, k];
                        }

                    }

                }

                return X;

            }

            public CellMatrix getMatrix(CellMatrix A, int[] r, int j0, int j1)
            {

                CellMatrix X = new CellMatrix(r.Length, j1 - j0 + 1, A.Affinity, A.Size);

                for (int i = 0; i < r.Length; i++)
                {
                    for (int j = j0; j <= j1; j++)
                    {
                        X[i, j - j0] = A[r[i], j];
                    }
                }

                return X;

            }

        }

        private struct MatrixEnumerator : IEnumerator<Cell>, IEnumerator, IDisposable
        {

            private CellMatrix _M;
            private int _i;
            private int _j;
            private Cell _v;

            public MatrixEnumerator(CellMatrix M)
            {
                this._M = M;
                this._i = 0;
                this._j = 0;
                this._v = new Cell(M.Affinity);
            }

            Cell IEnumerator<Cell>.Current
            {
                get { return this._M[this._i, this._j]; }
            }

            object IEnumerator.Current
            {
                get { return this._M[this._i, this._j]; }
            }

            public bool MoveNext()
            {
                if (this._i < this._M.RowCount)
                    this._i++;
                if (this._j < this._M.ColumnCount)
                    this._j++;
                if (this._i < this._M.RowCount && this._j < this._M.ColumnCount)
                {
                    this._v = this._M[this._i, this._j];
                    return true;
                }
                else
                {
                    this._v = new Cell(this._M.Affinity);
                    return false;
                }
            }

            public void Reset()
            {
                this._i = 0;
                this._j = 0;
            }

            public void Dispose()
            {

            }

        }

    }

}
