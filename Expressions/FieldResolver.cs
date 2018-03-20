using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Libraries;
using ArcticWind.Elements;
using ArcticWind.Tables;

namespace ArcticWind.Expressions
{

    /// <summary>
    /// 
    /// </summary>
    //public sealed class FieldResolver
    //{

    //    public const string GLOBAL = Host.GLOBAL;
    //    public const string LOCAL = "LOCAL";

    //    private Host _Host;
    //    private Heap<ObjectStore> _Stores;

    //    public FieldResolver(Host Host)
    //    {
    //        this._Host = Host;
    //        this._Stores = new Heap<ObjectStore>();
    //        this._Stores.Allocate(Host.GLOBAL, Host.Spools);
    //        this._Stores.Allocate(LOCAL, new ObjectStore(this._Host));
    //    }

    //    // Stores //
    //    public Heap<ObjectStore> Stores
    //    {
    //        get { return this._Stores; }
    //    }

    //    public ObjectStore this[int Index]
    //    {
    //        get { return this._Stores[Index]; }
    //    }

    //    public ObjectStore this[string Name]
    //    {
    //        get { return this._Stores[Name]; }
    //    }

    //    public bool StoreExists(string Name)
    //    {
    //        return this._Stores.Exists(Name);
    //    }

    //    public ObjectStore Global
    //    {
    //        get { return this._Stores[GLOBAL]; }
    //    }

    //    public ObjectStore Local
    //    {
    //        get { return this._Stores[LOCAL]; }
    //    }

    //    // Scalars //
    //    public Cell GetScalar(string Major, string Minor)
    //    {
    //        return this._Stores[Major].GetScalar(Minor);
    //    }
        
    //    public void DeclareScalar(string Major, string Minor, Cell Value)
    //    {
    //        this._Stores[Major].DeclareScalar(Minor, Value);
    //    }

    //    public void SetScalar(string Major, string Minor, Cell Value)
    //    {
    //        this._Stores[Major].Scalars[Minor] = Value;
    //    }

    //    public Cell GetScalar(int Major, int Minor)
    //    {
    //        return this._Stores[Major].Scalars[Minor];
    //    }
        
    //    public void SetScalar(int Major, int Minor, Cell Value)
    //    {
    //        this._Stores[Major].Scalars[Minor] = Value;
    //    }

    //    // Matrixes //
    //    public CellMatrix GetMatrix(string Major, string Minor)
    //    {
    //        return this._Stores[Major].Matrixes[Minor];
    //    }
        
    //    public void DeclareMatrix(string Major, string Minor, CellMatrix Value)
    //    {
    //        this._Stores[Major].DeclareMatrix(Minor, Value);
    //    }

    //    public void SetMatrix(string Major, string Minor, CellMatrix Value)
    //    {
    //        this._Stores[Major].Matrixes[Minor] = Value;
    //    }
        
    //    public CellMatrix GetMatrix(int Major, int Minor)
    //    {
    //        return this._Stores[Major].Matrixes[Minor];
    //    }
        
    //    public void SetMatrix(int Major, int Minor, CellMatrix Value)
    //    {
    //        this._Stores[Major].Matrixes[Minor] = Value;
    //    }

    //    // Records //
    //    public AssociativeRecord GetRecord(string Major, string Minor)
    //    {
    //        return this._Stores[Major].Records[Minor];
    //    }

    //    public AssociativeRecord GetRecord(int Major, int Minor)
    //    {
    //        return this._Stores[Major].Records[Minor];
    //    }
        
    //    public Schema GetSchema(string Major, string Minor)
    //    {
    //        return this._Stores[Major].Records[Minor].Columns;
    //    }

    //    public Schema GetSchema(int Major, int Minor)
    //    {
    //        return this._Stores[Major].Records[Minor].Columns;
    //    }
        
    //    public void DeclareRecord(string Major, string Minor, AssociativeRecord Value)
    //    {
    //        this._Stores[Major].DeclareRecord(Minor, Value);
    //    }

    //    public void DeclareRecord(string Major, string Minor, Schema Value)
    //    {
    //        this._Stores[Major].DeclareRecord(Minor, new AssociativeRecord(Value, Value.NullRecord));
    //    }

    //    public void SetRecord(string Major, string Minor, AssociativeRecord Value)
    //    {
    //        this._Stores[Major].Records[Minor] = Value;
    //    }
        
    //    public void SetRecord(int Major, int Minor, AssociativeRecord Value)
    //    {
    //        this._Stores[Major].Records[Minor] = Value;
    //    }

    //    public void RemoveRecord(string Major, string Minor)
    //    {
    //        this._Stores[Major].RemoveRecord(Minor);
    //    }

    //}
    
}
