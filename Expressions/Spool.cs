using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind;
using ArcticWind.Elements;
using ArcticWind.Tables;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.RecordExpressions;

namespace ArcticWind.Expressions
{
    
    /// <summary>
    /// Represents a collection of memory
    /// </summary>
    public sealed class Spool
    {

        public enum ElementType
        {
            Scalar,
            Matrix,
            Record,
            Table
        }

        private static long _CurrentXID = 0;

        private Host _Host;
        private string _Name;
        private Heap<ElementType> _Names;
        private Heap<Cell> _Scalar;
        private Heap<CellMatrix> _Matrix;
        private Heap<AssociativeRecord> _Record;
        private Heap<string> _Table;
        private long _XID = 0;

        public Spool(Host Host, string Name)
        {
            this._Host = Host;
            this._Names = new Heap<ElementType>();
            this._Scalar = new Heap<Cell>();
            this._Matrix = new Heap<CellMatrix>();
            this._Record = new Heap<AssociativeRecord>();
            this._Table = new Heap<string>();
            this._XID = XID();
            this._Name = Name;
        }
        
        // Names //
        public string Name
        {
            get { return this._Name; }
        }

        public List<string> ScalarNames
        {
            get { return this._Scalar.Entries.Keys.ToList(); }
        }

        public List<string> MatrixesNames
        {
            get { return this._Matrix.Entries.Keys.ToList(); }
        }

        public List<string> RecordNames
        {
            get { return this._Record.Entries.Keys.ToList(); }
        }

        public List<string> TableNames
        {
            get { return this._Table.Entries.Keys.ToList(); }
        }

        // Scalar Methods //
        public void SetScalar(string Name, Cell Value)
        {

            if (this._Names.Exists(Name) && this._Names[Name] != ElementType.Scalar)
                throw new Exception(string.Format("Object '{0}' alreadty exists as '{1}'", Name, this._Names[Name]));
            else
                this._Names.Reallocate(Name, ElementType.Scalar);


            if (this._Scalar.Exists(Name))
            {
                if (Value.Affinity != this._Scalar[Name].Affinity)
                    Value = CellConverter.Cast(Value, this._Scalar[Name].Affinity);
                this._Scalar.Reallocate(Name, Value);
            }
            else
            {
                this._Scalar.Allocate(Name, Value);
            }

        }

        public void DeleteScalar(string Name)
        {
            this._Scalar.Deallocate(Name);
            this._Names.Deallocate(Name);
        }

        public Cell GetScalar(string Name)
        {
            return this._Scalar[Name];
        }

        public CellAffinity GetScalarAffinity(string Name)
        {
            return this._Scalar[Name].Affinity;
        }

        public int GetScalarSize(string Name)
        {
            return this._Scalar[Name].Length;
        }

        public bool ScalarExists(string Name)
        {
            return this._Scalar.Exists(Name);
        }

        // Matrix Methods //
        public void SetMatrix(string Name, CellMatrix Value)
        {

            if (this._Names.Exists(Name) && this._Names[Name] != ElementType.Matrix)
                throw new Exception(string.Format("Object '{0}' alreadty exists as '{1}'", Name, this._Names[Name]));
            else
                this._Names.Reallocate(Name, ElementType.Matrix);

            this._Matrix.Reallocate(Name, Value);

        }

        public void SetMatrixElement(string Name, int Row, int Col, Cell Value)
        {

            if (this._Names.Exists(Name) && this._Names[Name] != ElementType.Matrix)
                throw new Exception(string.Format("Object '{0}' alreadty exists as '{1}'", Name, this._Names[Name]));

            if (Value.Affinity != this._Matrix[Name].Affinity)
                Value = CellConverter.Cast(Value, this._Matrix[Name].Affinity);

            this._Matrix[Name][Row, Col] = Value;

        }

        public void DeleteMatrix(string Name)
        {
            this._Matrix.Deallocate(Name);
            this._Names.Deallocate(Name);
        }

        public CellMatrix GetMatrix(string Name)
        {
            return this._Matrix[Name];
        }

        public CellAffinity GetMatrixAffinity(string Name)
        {
            return this._Matrix[Name].Affinity;
        }

        public int GetMatrixSize(string Name)
        {
            return this._Matrix[Name].Size;
        }

        public bool MatrixExists(string Name)
        {
            return this._Matrix.Exists(Name);
        }

        // Record Methods //
        public void SetRecord(string Name, AssociativeRecord Value)
        {

            if (this._Names.Exists(Name) && this._Names[Name] != ElementType.Record)
                throw new Exception(string.Format("Object '{0}' alreadty exists as '{1}'", Name, this._Names[Name]));
            else
                this._Names.Reallocate(Name, ElementType.Record);

            this._Record.Reallocate(Name, Value);

        }

        public void SetRecordElement(string Name, string Member, Cell Value)
        {

            if (this._Names.Exists(Name) && this._Names[Name] != ElementType.Record)
                throw new Exception(string.Format("Object '{0}' alreadty exists as '{1}'", Name, this._Names[Name]));

            this._Record[Name][Member] = Value;

        }

        public void DeleteRecord(string Name)
        {
            this._Record.Deallocate(Name);
            this._Names.Deallocate(Name);
        }

        public AssociativeRecord GetRecord(string Name)
        {
            return this._Record[Name];
        }

        public Schema GetColumns(string Name)
        {
            return this._Record[Name].Columns;
        }

        public bool RecordExists(string Name)
        {
            return this._Record.Exists(Name);
        }

        // Table Methods //
        public void SetTable(string Name, string Value)
        {

            if (this._Names.Exists(Name) && this._Names[Name] != ElementType.Table)
                throw new Exception(string.Format("Object '{0}' alreadty exists as '{1}'", Name, this._Names[Name]));
            else
                this._Names.Reallocate(Name, ElementType.Table);

            this._Table.Reallocate(Name, Value);

        }

        public void SetTable(string Name, Table Value)
        {
            this.SetTable(Name, Value.Header.Path);
        }

        public void DeleteTable(string Name)
        {
            this._Table.Deallocate(Name);
            this._Names.Deallocate(Name);
        }

        public string GetTable(string Name)
        {
            return this._Table[Name];
        }

        public Table GetTable2(string Name)
        {
            string x = this.GetTable(Name);
            return this._Host.TableStore.RequestTable(x);
        }

        public bool TableExists(string Name)
        {
            return this._Table.Exists(Name);
        }

        public void ResetSpool()
        {

            this._Names = new Heap<ElementType>();
            this._Scalar = new Heap<Cell>();
            this._Matrix = new Heap<CellMatrix>(); 
            this._Record = new Heap<AssociativeRecord>();
            this._Table = new Heap<string>();

        }

        private static long XID()
        {
            long x = _CurrentXID;
            _CurrentXID++;
            return x;
        }

    }

    /// <summary>
    /// Represents a collection of spools
    /// </summary>
    public sealed class SpoolSpace
    {

        private Heap<Spool> _Spools;

        public SpoolSpace()
        {
            this._Spools = new Heap<Spool>();
        }

        public SpoolSpace(SpoolSpace Seed)
            : this()
        {
            foreach (KeyValuePair<string, Spool> s in Seed._Spools.Entries)
            {
                this._Spools.Allocate(s.Key, s.Value);
            }
        }

        public Spool this[string Name]
        {
            get { return this._Spools[Name]; }
            private set { this._Spools[Name] = value; }
        }

        public List<Spool> Spools
        {
            get { return this._Spools.Values; }
        }

        public bool Exists(string Name)
        {
            return this._Spools.Exists(Name);
        }

        public void Add(string Name, Spool Value)
        {
            this._Spools.Allocate(Name, Value);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class SpoolSpaceContext
    {

        public class SpoolContextScalarEntry
        {

            public SpoolContextScalarEntry(string Name, CellAffinity Affinity, int Size)
            {
                this.Name = Name;
                this.Affinity = Affinity;
                this.Size = Size;
            }
            
            public string Name
            {
                get;
                private set;
            }

            public CellAffinity Affinity
            {
                get;
                private set;
            }

            public int Size
            {
                get;
                private set;
            }
        
        }

        public class SpoolContext
        {

            private Heap<Spool.ElementType> _Names;
            private Heap<SpoolContextScalarEntry> _Scalars;
            private Heap<SpoolContextScalarEntry> _Matrixes;
            private Heap<Schema> _Records;

            public SpoolContext()
            {
                this._Names = new Heap<Spool.ElementType>();
                this._Scalars = new Heap<SpoolContextScalarEntry>();
                this._Matrixes = new Heap<SpoolContextScalarEntry>();
                this._Records = new Heap<Schema>();
            }

            // Deletes //
            public void Drop(string Name)
            {
                
                if (!this._Names.Exists(Name))
                    return;

                if (this._Names[Name] == Spool.ElementType.Scalar)
                    this._Scalars.Deallocate(Name);
                else if (this._Names[Name] == Spool.ElementType.Matrix)
                    this._Matrixes.Deallocate(Name);
                else if (this._Names[Name] == Spool.ElementType.Record)
                    this._Records.Deallocate(Name);

                this._Names.Deallocate(Name);

            }

            // Scalars //
            public void AddScalar(string Name, CellAffinity Affinity, int Size)
            {
                this._Names.Allocate(Name, Spool.ElementType.Scalar);
                this._Scalars.Allocate(Name, new SpoolContextScalarEntry(Name, Affinity, Size));
            }

            public bool ScalarExists(string Name)
            {
                return this._Names.Exists(Name) && this._Names[Name] == Spool.ElementType.Scalar;
            }

            public SpoolContextScalarEntry GetScalar(string Name)
            {
                if (!this._Names.Exists(Name) || this._Names[Name] != Spool.ElementType.Scalar)
                    throw new Exception(string.Format("Scalar '{0}' does not exist", Name));
                return this._Scalars[Name];
            }

            // Matrixes //
            public void AddMatrix(string Name, CellAffinity Affinity, int Size)
            {
                this._Names.Allocate(Name, Spool.ElementType.Matrix);
                this._Matrixes.Allocate(Name, new SpoolContextScalarEntry(Name, Affinity, Size));
            }

            public bool MatrixExists(string Name)
            {
                return this._Names.Exists(Name) && this._Names[Name] == Spool.ElementType.Matrix;
            }

            public SpoolContextScalarEntry GetMatrix(string Name)
            {
                if (!this._Names.Exists(Name) || this._Names[Name] != Spool.ElementType.Matrix)
                    throw new Exception(string.Format("Matrix '{0}' does not exist", Name));
                return this._Matrixes[Name];
            }

            // Records //
            public void AddRecord(string Name, Schema Rec)
            {
                this._Names.Allocate(Name, Spool.ElementType.Record);
                this._Records.Allocate(Name, Rec);
            }

            public bool RecordExists(string Name)
            {
                return this._Names.Exists(Name) && this._Names[Name] == Spool.ElementType.Record;
            }

            public Schema GetRecord(string Name)
            {
                if (!this._Names.Exists(Name) || this._Names[Name] != Spool.ElementType.Record)
                    throw new Exception(string.Format("Record '{0}' does not exist", Name));
                return this._Records[Name];
            }



        }

        private Heap<SpoolContext> _Conext;
        private Host _Host;

        public SpoolSpaceContext(Host Host)
        {
            this._Host = Host;
            this._Conext = new Heap<SpoolContext>();
            
        }

        public string PrimaryContextName
        {
            get;
            set;
        }

        public IEnumerable<SpoolContext> Contexes
        {
            get { return this._Conext.Values; }
        }

        public SpoolContext this[string Name]
        {
            get { return this._Conext[Name]; }
        }

        public SpoolContext PrimaryContext
        {
            get { return this._Conext[this.PrimaryContextName]; }
        }

        public void AddSpool(string Name)
        {
            this._Conext.Allocate(Name, new SpoolContext());
        }

        public void DropSpool(string Name)
        {
            this._Conext.Deallocate(Name);
        }

        public void AddScalar(string SpoolName, string VarName, CellAffinity Affinity, int Size)
        {
            this._Conext[SpoolName].AddScalar(VarName, Affinity, Size);
        }

        public void AddMatrix(string SpoolName, string VarName, CellAffinity Affinity, int Size)
        {
            this._Conext[SpoolName].AddMatrix(VarName, Affinity, Size);
        }

        public void AddRecord(string SpoolName, string VarName, Schema Columns)
        {
            this._Conext[SpoolName].AddRecord(VarName, Columns);
        }

        public void AddRecordElement(string SpoolName, string VarName, string FieldName, CellAffinity Affinity, int Size)
        {
            this._Conext[SpoolName].GetRecord(VarName).Add(FieldName, Affinity, Size);
        }

        public bool SpoolExists(string Name)
        {
            return this._Conext.Exists(Name);
        }

        public void Import(Spool Value)
        {

            // Create the spool //
            this.AddSpool(Value.Name);

            // Add scalars //
            foreach (string ns in Value.ScalarNames)
            {
                this.AddScalar(Value.Name, ns, Value.GetScalarAffinity(ns), Value.GetScalarSize(ns));
            }

            // Add matrixes //
            foreach (string nm in Value.MatrixesNames)
            {
                this.AddMatrix(Value.Name, nm, Value.GetMatrixAffinity(nm), Value.GetMatrixSize(nm));
            }

            // Add records //
            foreach (string nr in Value.RecordNames)
            {
                this.AddRecord(Value.Name, nr, Value.GetColumns(nr));
            }

        }

    }

}
