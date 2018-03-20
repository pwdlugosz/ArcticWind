using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Tables;

namespace ArcticWind.Elements
{

    public sealed class ObjectStore
    {

        public enum ObjectAffinity
        {
            Scalar,
            Matrix,
            Record,
            Table
        }

        public enum ObjectState
        {
            ReadWrite,
            ReadOnly,
            WriteOnly,
            Locked,
            OutOfScope
        }

        private Heap<ObjectAffinity> _Names;
        private Heap<ObjectState> _States;

        private Heap<Cell> _Scalars;
        private Heap<AssociativeRecord> _Records;
        private Heap<CellMatrix> _Matrixes;
        private Heap<string> _Tables;

        private Host _Host;

        public ObjectStore(Host Host)
        {

            this._Host = Host;
            this._Names = new Heap<ObjectAffinity>();
            this._States = new Heap<ObjectState>();
            this._Scalars = new Heap<Cell>();
            this._Records = new Heap<AssociativeRecord>();
            this._Matrixes = new Heap<CellMatrix>();
            this._Tables = new Heap<string>();

        }

        // Meta Data //
        public bool Exists(string Name, ObjectAffinity Type)
        {
            if (!this._Names.Exists(Name))
                return false;
            return this._Names[Name] == Type;
        }

        public bool ExistsAny(string Name)
        {
            return this._Names.Exists(Name);
        }

        public ObjectAffinity Affinty(string Name)
        {
            return this._Names[Name];
        }

        internal Heap<Cell> Scalars
        {
            get { return this._Scalars; }
        }

        internal Heap<CellMatrix> Matrixes
        {
            get { return this._Matrixes; }
        }

        internal Heap<AssociativeRecord> Records
        {
            get { return this._Records; }
        }

        public bool CanRead(string Name)
        {
            if (!this._States.Exists(Name))
                throw new ObjectDoesNotExistException(Name);
            ObjectState os = this._States[Name];
            return os == ObjectState.ReadOnly || os == ObjectState.ReadWrite;
        }

        public bool CanWrite(string Name)
        {
            if (!this._States.Exists(Name))
                throw new ObjectDoesNotExistException(Name);
            ObjectState os = this._States[Name];
            return os == ObjectState.WriteOnly || os == ObjectState.ReadWrite;
        }

        // Scalars //
        public bool ExistsScalar(string Name)
        {
            return this.Exists(Name, ObjectAffinity.Scalar);
        }

        public bool ExistsNotScalar(string Name)
        {
            if (this._Names.Exists(Name))
                return this._Names[Name] != ObjectAffinity.Scalar;
            return false;
        }

        public void DeclareScalar(string Name, Cell Value)
        {
            if (this.ExistsNotScalar(Name))
                throw new ObjectAlreadyExistsException(Name);
            this._Scalars.Reallocate(Name, Value);
            this._Names.Allocate(Name, ObjectAffinity.Scalar);
            this._States.Reallocate(Name, ObjectState.ReadWrite);
        }

        public Cell GetScalar(string Name)
        {
            if (!this.ExistsScalar(Name))
                throw new ObjectDoesNotExistException(Name);
            if (!this.CanRead(Name))
                throw new ObjectLockException(Name);
            return this._Scalars[Name];
        }

        public void SetScalar(string Name, Cell Value)
        {
            if (!this.ExistsScalar(Name))
                throw new ObjectDoesNotExistException(Name);
            if (!this.CanWrite(Name))
                throw new ObjectLockException(Name);
            this._Scalars[Name] = Value;
        }

        public void RemoveScalar(string Name)
        {
            if (!this.ExistsScalar(Name))
                return;
            this._Scalars.Deallocate(Name);
        }

        // Matrixes //
        public bool ExistsMatrix(string Name)
        {
            return this.Exists(Name, ObjectAffinity.Matrix);
        }

        public bool ExistsNotMatrix(string Name)
        {
            if (this._Names.Exists(Name))
                return this._Names[Name] != ObjectAffinity.Matrix;
            return false;
        }

        public void DeclareMatrix(string Name, CellMatrix Value)
        {
            if (this.ExistsNotMatrix(Name))
                throw new ObjectAlreadyExistsException(Name);
            this._Matrixes.Reallocate(Name, Value);
            this._Names.Reallocate(Name, ObjectAffinity.Matrix);
            this._States.Reallocate(Name, ObjectState.ReadWrite);
        }

        public CellMatrix GetMatrix(string Name)
        {
            if (!this.ExistsMatrix(Name))
                throw new ObjectDoesNotExistException(Name);
            if (!this.CanRead(Name))
                throw new ObjectLockException(Name);
            return this._Matrixes[Name];
        }

        public void SetMatrix(string Name, CellMatrix Value)
        {
            if (!this.ExistsMatrix(Name))
                throw new ObjectDoesNotExistException(Name);
            if (!this.CanWrite(Name))
                throw new ObjectLockException(Name);
            this._Matrixes[Name] = Value;
        }

        public void RemoveMatrix(string Name)
        {
            if (!this.ExistsMatrix(Name))
                return;
            this._Matrixes.Deallocate(Name);
        }

        // Records //
        public bool ExistsRecord(string Name)
        {
            return this.Exists(Name, ObjectAffinity.Record);
        }

        public bool ExistsNotRecord(string Name)
        {
            if (this._Names.Exists(Name))
                return this._Names[Name] != ObjectAffinity.Record;
            return false;
        }

        public void DeclareRecord(string Name, AssociativeRecord Value)
        {
            if (this.ExistsNotRecord(Name))
                throw new ObjectAlreadyExistsException(Name);
            this._Records.Reallocate(Name, Value);
            this._Names.Reallocate(Name, ObjectAffinity.Record);
            this._States.Reallocate(Name, ObjectState.ReadWrite);
        }

        public AssociativeRecord GetRecord(string Name)
        {
            if (!this.ExistsRecord(Name))
                throw new ObjectDoesNotExistException(Name);
            if (!this.CanRead(Name))
                throw new ObjectLockException(Name);
            return this._Records[Name];
        }

        public Schema GetColumns(string Name)
        {
            if (!this.ExistsRecord(Name))
                throw new ObjectDoesNotExistException(Name);
            if (!this.CanRead(Name))
                throw new ObjectLockException(Name);
            return this._Records[Name].Columns;
        }

        public void SetRecord(string Name, AssociativeRecord Value)
        {
            if (!this.ExistsRecord(Name))
                throw new ObjectDoesNotExistException(Name);
            if (!this.CanWrite(Name))
                throw new ObjectLockException(Name);
            this._Records[Name] = Value;
        }

        public void RemoveRecord(string Name)
        {
            if (!this.ExistsRecord(Name))
                return;
            this._Records.Deallocate(Name);
        }

        // Tables //
        public bool ExistsTable(string Name)
        {
            return this.Exists(Name, ObjectAffinity.Table);
        }

        public bool ExistsNotTable(string Name)
        {
            if (this._Names.Exists(Name))
                return this._Names[Name] != ObjectAffinity.Table;
            return false;
        }

        public void DeclareTable(string Name, string Value)
        {
            if (this.ExistsNotRecord(Name))
                throw new ObjectAlreadyExistsException(Name);
            this._Tables.Reallocate(Name, Value);
            this._Names.Allocate(Name, ObjectAffinity.Table);
            this._States.Reallocate(Name, ObjectState.ReadWrite);
        }

        public string GetTable(string Name)
        {
            if (!this.ExistsTable(Name))
                throw new ObjectDoesNotExistException(Name);
            if (!this.CanRead(Name))
                throw new ObjectLockException(Name);
            return this._Tables[Name];
        }

        public void SetTable(string Name, string Value)
        {
            if (!this.ExistsTable(Name))
                throw new ObjectDoesNotExistException(Name);
            if (!this.CanWrite(Name))
                throw new ObjectLockException(Name);
            this._Tables[Name] = Value;
        }

        public void RemoveTable(string Name)
        {
            if (!this.ExistsTable(Name))
                return;
            this._Tables.Deallocate(Name);
        }

        // Exceptions //
        public sealed class ObjectLockException : Exception
        {

            public ObjectLockException(string Name)
                : base(string.Format("Object '{0}' does not support access level", Name))
            {
            }

        }

        public sealed class ObjectAlreadyExistsException : Exception
        {

            public ObjectAlreadyExistsException(string Name)
                : base(string.Format("An object by the name of '{0}' already exists"))
            {

            }

        }

        public sealed class ObjectDoesNotExistException : Exception
        {
            public ObjectDoesNotExistException(string Name)
                : base(string.Format("Object '{0}' does not exist", Name))
            {
            }
        }

    }



}
