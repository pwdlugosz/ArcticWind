using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using ArcticWind.Libraries;
using ArcticWind.Tables;
using ArcticWind.Expressions;

namespace ArcticWind.Elements
{

    /// <summary>
    /// Represents an envrioment to that core pulse features run off of
    /// </summary>
    public sealed class Host
    {

        public const int DEBUG_STATE = 1;
        private StreamWriter _DebugWriter;
        private Scripting.ScriptProcessor _Engine;

        public const string HOST_NAME = "PULSE";
        public const string HOST_VERSION = "0.5.0";

        /// <summary>
        /// The name of the disk based temp schema
        /// </summary>
        public const string TEMP = "TEMP";
        public const string GLOBAL = "GLOBAL";
        public const string USER = "USER";
        public const string RANDOM_NAME_PREFIX = "TEMP";
        
        private const string DIR_MAIN = "Pulse_Projects";
        private const string DIR_TEMP = "Temp";
        private const string DIR_TEST = "Test";
        private const string DIR_LOG = "Log";

        private Communicator _IO;
        private RandomCell _RNG;
        private Stopwatch _Timer;
        private Heap<Library> _Libraries;
        private SpoolSpace _Store;
        private Heap<string> _Connections;
        private Library _BaseNative;
        private ScriptedLibrary _BaseScripted;
        public readonly long StartTicks = DateTime.Now.Ticks;
        private TableStore _Cache;
        private static long _Tocks = 0;
        
        /// <summary>
        /// Creates a host
        /// </summary>
        public Host()
        {

            Host.CheckDir();

            this._IO = new CommandLineCommunicator();

            this._RNG = new RandomCell();
            this._Timer = Stopwatch.StartNew();
            this._Libraries = new Heap<Library>();

            // Tables //
            this._Cache = new TableStore(this);

            // Objects //
            this._Store = new SpoolSpace();
            this._Store.Add(GLOBAL, new Spool(this, GLOBAL));
            this._Connections = new Heap<string>();
            this._Connections.Allocate(TEMP, TempDir);

            // Add the base library to the library collection //
            this._BaseNative = new BaseLibrary(this);
            this._Libraries.Allocate(GLOBAL, this._BaseNative);
            this._BaseScripted = new ScriptedLibrary(this, USER);
            this._Libraries.Allocate(USER, this._BaseScripted);

            // Possibly create a log writer //
            if (DEBUG_STATE == 1)
            {
                this._DebugWriter = new StreamWriter(DebugLogPath);
            }

        }

        /// <summary>
        /// Shuts down the host
        /// </summary>
        public void ShutDown()
        {

            this.TableStore.ShutDown();

            foreach (Library x in this._Libraries.Values)
            {
                x.ShutDown();
            }

            if (DEBUG_STATE == 1)
            {
                this._DebugWriter.Flush();
                this._DebugWriter.Close();
                this._DebugWriter = null;
            }

        }

        /// <summary>
        /// Internal table store
        /// </summary>
        public SpoolSpace Spools
        {
            get { return this._Store; }
        }

        /// <summary>
        /// Internal non-debugger log
        /// </summary>
        public Communicator IO
        {
            get { return this._IO; }
        }

        /// <summary>
        /// Gets the elapsed time since the host was launched
        /// </summary>
        public long Elapsed
        {
            get { return this._Timer.ElapsedMilliseconds; }
        }

        /// <summary>
        /// All the connections
        /// </summary>
        public Heap<string> Connections
        {
            get { return this._Connections; }
        }

        /// <summary>
        /// Gets the connection to TempDB
        /// </summary>
        public string TempDB
        {
            get { return this._Connections[TEMP]; }
        }

        /// <summary>
        /// 
        /// </summary>
        public TableStore TableStore
        {
            get { return this._Cache; }
        }

        /// <summary>
        /// 
        /// </summary>
        internal Scripting.ScriptProcessor Engine
        {
            get { return this._Engine; }
            set { this._Engine = value; }
        }

        // Random Number Generator //
        /// <summary>
        /// Base random number generator
        /// </summary>
        public RandomCell BaseRNG
        {
            get { return this._RNG; }
        }
        
        /// <summary>
        /// Sets the RNG seed
        /// </summary>
        /// <param name="Seed"></param>
        public void SetSeed(int Seed)
        {
            this._RNG = new RandomCell(Seed);
        }

        // Library //
        /// <summary>
        /// Collection of libraries; this does NOT include the base library
        /// </summary>
        public Heap<Library> Libraries
        {
            get { return this._Libraries; }
        }

        /// <summary>
        /// Base library, which includes the base function library, the base action library and the scalar / matrix heap the user's can alter
        /// </summary>
        public Library BaseLibrary
        {
            get { return this._BaseNative; }
        }

        /// <summary>
        /// User scripting library
        /// </summary>
        public ScriptedLibrary UserLibrary
        {
            get { return this._BaseScripted; }
        }

        // Table Support //
        /// <summary>
        /// Opens a table given a path
        /// </summary>
        /// <param name="Key">The path to the file</param>
        /// <returns></returns>
        public Table OpenTable(string Key)
        {
            return this._Cache.RequestTable(Key);
        }

        /// <summary>
        /// Opens a table given an aliast and a name
        /// </summary>
        /// <param name="Alias">The alias; the alias must be an active connection</param>
        /// <param name="Name">The name of the table</param>
        /// <returns></returns>
        public Table OpenTable(string Alias, string Name)
        {
            if (this._Connections.Exists(Alias))
                return this.OpenTable(TableHeader.DeriveV1Path(this._Connections[Alias], Name));
            throw new Exception(string.Format("Connection '{0}' does not exist", Alias));
        }

        /// <summary>
        /// Opens a table given a string of the form 'DB.Name'
        /// </summary>
        /// <param name="ScriptingName"></param>
        /// <returns></returns>
        public Table OpenTableUI(string ScriptingName)
        {
            string[] vars = ScriptingName.Split('.');
            string db = (vars.Length == 1 ? TEMP : vars[0]);
            string name = vars.Last();
            string path = TableHeader.DeriveV1Path(this._Connections[db], name);
            return this.OpenTable(path);
        }

        /// <summary>
        /// Creates a table with a cluster index
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Name"></param>
        /// <param name="Columns"></param>
        /// <param name="ClusterColumns"></param>
        /// <param name="State"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public ClusteredTable CreateTable(string Alias, string Name, Schema Columns, Key ClusterColumns, ClusterState State, int PageSize)
        {
            this._Cache.DropTable(TableHeader.DeriveV1Path(this._Connections[Alias], Name));
            ClusteredTable t = new ClusteredTable(this, Name, this._Connections[Alias], Columns, ClusterColumns, State, PageSize);
            return t;
        }

        /// <summary>
        /// Creates a table with a given index; the page size is defaulted
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Name"></param>
        /// <param name="Columns"></param>
        /// <param name="ClusterColumns"></param>
        /// <param name="State"></param>
        /// <returns></returns>
        public ClusteredTable CreateTable(string Alias, string Name, Schema Columns, Key ClusterColumns, ClusterState State)
        {
            this._Cache.DropTable(TableHeader.DeriveV1Path(this._Connections[Alias], Name));
            ClusteredTable t = new ClusteredTable(this, Name, this._Connections[Alias], Columns, ClusterColumns, State, Page.DEFAULT_SIZE);
            return t;
        }

        /// <summary>
        /// Creates a table with a clustered index; this assumes a default page size and a universal index
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Name"></param>
        /// <param name="Columns"></param>
        /// <param name="ClusterColumns"></param>
        /// <returns></returns>
        public ClusteredTable CreateTable(string Alias, string Name, Schema Columns, Key ClusterColumns)
        {
            this._Cache.DropTable(TableHeader.DeriveV1Path(this._Connections[Alias], Name));
            ClusteredTable t = new ClusteredTable(this, Name, this._Connections[Alias], Columns, ClusterColumns, ClusterState.Universal, Page.DEFAULT_SIZE);
            return t;
        }

        /// <summary>
        /// Creates a heap table
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Name"></param>
        /// <param name="Columns"></param>
        /// <returns></returns>
        public HeapTable CreateTable(string Alias, string Name, Schema Columns)
        {
            this._Cache.DropTable(TableHeader.DeriveV1Path(this._Connections[Alias], Name));
            return new HeapTable(this, Name, this._Connections[Alias], Columns, Page.DEFAULT_SIZE);
        }

        /// <summary>
        /// Creates a dictionary table
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Name"></param>
        /// <param name="KeyColumns"></param>
        /// <param name="ValueColumns"></param>
        /// <returns></returns>
        public DictionaryTable CreateTable(string Alias, string Name, Schema KeyColumns, Schema ValueColumns)
        {
            this._Cache.DropTable(TableHeader.DeriveV1Path(this._Connections[Alias], Name));
            return new DictionaryTable(this, Name, this._Connections[Alias], KeyColumns, ValueColumns, Page.DEFAULT_SIZE);
        }

        /// <summary>
        /// Creates a temporary table and adds to the recycle bin
        /// </summary>
        /// <param name="Columns"></param>
        /// <returns></returns>
        public HeapTable CreateTempTable(Schema Columns)
        {
            HeapTable t = this.CreateTable(TEMP, Host.RandomName, Columns);
            this._Cache.PlaceInRecycleBin(t.Key);
            return t;
        }

        /// <summary>
        /// Checks if a table exists
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool TableExists(string Alias, string Name)
        {
            if (!this._Connections.Exists(Alias))
                return false;
            return this._Cache.TableExists(TableHeader.DeriveV1Path(this._Connections[Alias], Name));
        }

        /// <summary>
        /// Checks if the table is system generated temporary table
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public bool IsSystemTemp(Table T)
        {
            return T.Name.StartsWith(RANDOM_NAME_PREFIX) && T.Header.Directory == this.TempDB;
        }

        // Debugging //
        public void DebugPrint(string Text, params object[] Obj)
        {

            if (DEBUG_STATE == 0)
                return;

            string x = new string('\t', Math.Max(0, this.DebugDepth));
            this._DebugWriter.WriteLine(string.Format(x + Text, Obj));

        }

        public int DebugDepth
        {
            get;
            set;
        }

        public static long GetXID()
        {
            Cell x = new Cell(0);
            byte[] y = Guid.NewGuid().ToByteArray();
            x.B0 = (byte)(y[0] ^ y[8]);
            x.B1 = (byte)(y[1] ^ y[9]);
            x.B2 = (byte)(y[2] ^ y[10]);
            x.B3 = (byte)(y[3] ^ y[11]);
            x.B4 = (byte)(y[4] ^ y[12]);
            x.B5 = (byte)(y[5] ^ y[13]);
            x.B6 = (byte)(y[6] ^ y[14]);
            x.B7 = (byte)(y[7] ^ y[15]);
            return x.LONG < 0 ? ~x.LONG : x.LONG;
        }

        public static long Tocks()
        {
            return _Tocks++;
        }

        // Directories //
        public static string MainDir
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + DIR_MAIN + "\\"; }
        }

        public static string TempDir
        {
            get { return MainDir + DIR_TEMP + "\\"; }
        }

        public static string LogDir
        {
            get { return MainDir + DIR_LOG + "\\"; }
        }

        public static string TestDir
        {
            get { return MainDir + DIR_TEST + "\\"; }
        }

        public static void CheckDir()
        {

            if (!Directory.Exists(MainDir)) 
                Directory.CreateDirectory(MainDir);
            if (!Directory.Exists(TempDir)) 
                Directory.CreateDirectory(TempDir);
            if (!Directory.Exists(LogDir)) 
                Directory.CreateDirectory(LogDir);
            if (!Directory.Exists(TestDir)) 
                Directory.CreateDirectory(TestDir);

        }

        public static string RandomName
        {
            get
            {
                Guid x = Guid.NewGuid();
                return RANDOM_NAME_PREFIX + "_" + x.ToString().Replace("-", "");
            }
        }

        public static string LongDateString
        {
            
            get
            {
                DateTime dt = DateTime.Now;
                return string.Format("{0}{1}{2}_{3}{4}{5}{6}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
            }

        }

        internal static string DebugLogPath
        {

            get
            {
                return LogDir + "_DEBUG_LOG_" + LongDateString + ".txt";
            }

        }

    }

}
