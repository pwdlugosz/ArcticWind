using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Expressions;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Expressions.TableExpressions;
using ArcticWind.Expressions.ActionExpressions;
using ArcticWind.Tables;
using ArcticWind.Elements;
using System.IO;
using System.IO.Compression;

namespace ArcticWind.Libraries
{

    public sealed class StreamCache
    {

        private enum StreamAffinity
        {
            Read,
            Write
        }

        private struct StreamEntry
        {

            public StreamEntry(StreamAffinity Affinity, int Location)
                :this()
            {
                this.Affinity = Affinity;
                this.Location = Location;
            }

            public StreamAffinity Affinity 
            { 
                get; 
                set; 
            }

            public int Location 
            { 
                get; 
                set; 
            }

        }

        private Dictionary<Cell, StreamEntry> _Map;
        private List<StreamReader> _sr;
        private List<StreamWriter> _sw;

        public StreamCache()
        {
            this._Map = new Dictionary<Cell, StreamEntry>();
            this._sr = new List<StreamReader>();
            this._sw = new List<StreamWriter>();
        }

        public void AddStream(Cell Key, StreamReader Stream)
        {
            if (this._Map.ContainsKey(Key))
                throw new Exception("Stream already interned");
            int idx = this._sr.Count;
            this._sr.Add(Stream);
            this._Map.Add(Key, new StreamEntry(StreamAffinity.Read, idx));
        }

        public void AddStream(Cell Key, StreamWriter Stream)
        {
            if (this._Map.ContainsKey(Key))
                throw new Exception("Stream already interned");
            int idx = this._sw.Count;
            this._sw.Add(Stream);
            this._Map.Add(Key, new StreamEntry(StreamAffinity.Write, idx));
        }

        public bool StreamExists(Cell Key)
        {
            return this._Map.ContainsKey(Key);
        }

        public bool ReaderExists(Cell Key)
        {
            return this._Map.ContainsKey(Key) && this._Map[Key].Affinity == StreamAffinity.Read;
        }

        public bool WriterExists(Cell Key)
        {
            return this._Map.ContainsKey(Key) && this._Map[Key].Affinity == StreamAffinity.Write;
        }

        public StreamReader GetReader(Cell Key)
        {
            StreamEntry x = this._Map[Key];
            if (x.Affinity == StreamAffinity.Read)
                return this._sr[x.Location];
            throw new Exception(string.Format("Stream '{0}' does not exist"));
        }

        public StreamWriter GetWriter(Cell Key)
        {
            StreamEntry x = this._Map[Key];
            if (x.Affinity == StreamAffinity.Write)
                return this._sw[x.Location];
            throw new Exception(string.Format("Stream '{0}' does not exist"));
        }

        public void Close(Cell Key)
        {

            StreamEntry x = this._Map[Key];
            if (x.Affinity == StreamAffinity.Read)
            {
                StreamReader sr = this._sr[x.Location];
                sr.Close();
                this._sr[x.Location] = null;
            }
            else if (x.Affinity == StreamAffinity.Write)
            {
                StreamWriter sw = this._sw[x.Location];
                sw.Flush();
                sw.Close();
                this._sw[x.Location] = null;
            }
            this._Map.Remove(Key);

        }

        public void Shutdown()
        {

            foreach (StreamReader sr in this._sr)
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }

            foreach (StreamWriter sw in this._sw)
            {
                if (sw != null)
                {
                    sw.Flush();
                    sw.Close();
                }
            }

        }

    }

    public sealed class StreamLibrary : Library
    {

        public const string DEFLATE = "DEFLATE";
        public const string INFLATE = "INFLATE";
        public const string OPEN_READ = "OPEN_READ";
        public const string OPEN_WRITE = "OPEN_WRITE";
        public const string CLOSE = "CLOSE";
        public const string ADVANCE = "ADVANCE";
        public const string WRITE_LINE = "WRITE_LINE";
        public const string WRITE_BLOCK = "WRITE_BLOCK";
        public const string READ_LINE = "READ_LINE";
        public const string READ_BLOCK = "READ_BLOCK";
        public const string EOS = "EOS";

        private static readonly string[] ActionNames = { OPEN_READ, OPEN_WRITE, CLOSE, ADVANCE, WRITE_LINE, WRITE_BLOCK };
        private static readonly string[] ScalarNames = { DEFLATE, INFLATE, READ_LINE, READ_BLOCK, EOS };
        private static readonly string[] MatrixNames = { };
        private static readonly string[] RecordNames = { };
        private static readonly string[] TableNames = { };

        private StreamCache _Stream;
        
        public StreamLibrary(Host Host)
            : base(Host, "STREAM")
        {
            this._Stream = new StreamCache();
        }

        public override bool ActionExists(string Name)
        {
            return ActionNames.Contains(Name);
        }

        public override ActionExpressionParameterized ActionLookup(string Name)
        {
            
            switch (Name.ToUpper())
            {

                case OPEN_READ: return new aeOPEN_READ(this._Host, this._Stream);
                case OPEN_WRITE: return new aeOPEN_WRITE(this._Host, this._Stream);
                case WRITE_LINE: return new aeWRITE_LINE(this._Host, this._Stream);
                case WRITE_BLOCK: return new aeWRITE_BLOCK(this._Host, this._Stream);
                case CLOSE: return new aeCLOSE(this._Host, this._Stream);
                case ADVANCE: return new aeADVANCE(this._Host, this._Stream);
            
            }

            throw new Exception(string.Format("Action does not exist '{0}'", Name));

        }

        public override bool MatrixFunctionExists(string Name)
        {
            return MatrixNames.Contains(Name);
        }

        public override MatrixExpressionFunction MatrixFunctionLookup(string Name)
        {

            throw new Exception(string.Format("Matrix does not exist '{0}'", Name));

        }

        public override bool RecordFunctionExists(string Name)
        {
            return RecordNames.Contains(Name);
        }

        public override RecordExpressionFunction RecordFunctionLookup(string Name)
        {
            throw new Exception(string.Format("Record does not exist '{0}'", Name));
        }

        public override bool ScalarFunctionExists(string Name)
        {
            return ScalarNames.Contains(Name);
        }

        public override ScalarExpressionFunction ScalarFunctionLookup(string Name)
        {

            switch(Name.ToUpper())
            {
                case READ_LINE: return new sfREAD_LINE(this._Host, this._Stream);
                case READ_BLOCK: return new sfREAD_BLOCK(this._Host, this._Stream);
                case DEFLATE: return new sfDeflate(this._Host);
                case INFLATE: return new sfInflate(this._Host);
                case EOS: return new sfEOS(this._Host, this._Stream);
            }

            throw new Exception(string.Format("Scalar does not exist '{0}'", Name));
        }

        public override bool TableFunctionExists(string Name)
        {
            return TableNames.Contains(Name);
        }

        public override TableExpressionFunction TableFunctionLookup(string Name)
        {
            throw new Exception(string.Format("Table does not exist '{0}'", Name));
        }

        // Actions //
        public sealed class aeOPEN_READ : ActionExpressionParameterized
        {

            private StreamCache _Cache;

            public aeOPEN_READ(Host Host, StreamCache Cache)
                : base(Host, null, OPEN_READ, 2)
            {
                this._Cache = Cache;
            }

            public override void Invoke(SpoolSpace Variant)
            {

                if (!this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                    throw new Exception("Expecting two scalar parameters");

                Cell Key = this._Parameters[0].Scalar.Evaluate(Variant);
                Cell Path = this._Parameters[1].Scalar.Evaluate(Variant);

                StreamReader sr = new StreamReader(Path);
                this._Cache.AddStream(Key, sr);
                
            }

        }

        public sealed class aeOPEN_WRITE : ActionExpressionParameterized
        {

            private StreamCache _Cache;

            public aeOPEN_WRITE(Host Host, StreamCache Cache)
                : base(Host, null, OPEN_WRITE, 2)
            {
                this._Cache = Cache;
            }

            public override void Invoke(SpoolSpace Variant)
            {

                if (!this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                    throw new Exception("Expecting two scalar parameters");

                Cell Key = this._Parameters[0].Scalar.Evaluate(Variant);
                Cell Path = this._Parameters[1].Scalar.Evaluate(Variant);

                StreamWriter sw = new StreamWriter(Path);
                this._Cache.AddStream(Key, sw);

            }

        }

        public sealed class aeCLOSE : ActionExpressionParameterized
        {

            private StreamCache _Cache;

            public aeCLOSE(Host Host, StreamCache Cache)
                : base(Host, null, CLOSE, 1)
            {
                this._Cache = Cache;
            }

            public override void Invoke(SpoolSpace Variant)
            {

                if (!this.CheckSigniture(ParameterAffinity.Scalar))
                    throw new Exception("Expecting two scalar parameters");

                Cell Key = this._Parameters[0].Scalar.Evaluate(Variant);
                this._Cache.Close(Key);

            }

        }

        public sealed class aeADVANCE : ActionExpressionParameterized
        {

            private StreamCache _Cache;

            public aeADVANCE(Host Host, StreamCache Cache)
                : base(Host, null, ADVANCE, 1)
            {
                this._Cache = Cache;
            }

            public override void Invoke(SpoolSpace Variant)
            {

                if (!this.CheckSigniture(ParameterAffinity.Scalar))
                    throw new Exception("Expecting two scalar parameters");

                Cell Key = this._Parameters[0].Scalar.Evaluate(Variant);
                StreamReader sr = this._Cache.GetReader(Key);
                if (!sr.EndOfStream)
                {
                    string x = sr.ReadLine();
                }

            }

        }

        public sealed class aeWRITE_LINE : ActionExpressionParameterized
        {

            private StreamCache _Cache;

            public aeWRITE_LINE(Host Host, StreamCache Cache)
                : base(Host, null, WRITE_LINE, 1)
            {
                this._Cache = Cache;
            }

            public override void Invoke(SpoolSpace Variant)
            {

                if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                {
                    Cell Key = this._Parameters[0].Scalar.Evaluate(Variant);
                    Cell c = this._Parameters[1].Scalar.Evaluate(Variant);
                    this._Cache.GetWriter(Key).WriteLine(c.valueCSTRING);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Matrix))
                {
                    Cell Key = this._Parameters[0].Scalar.Evaluate(Variant);
                    CellMatrix m = this._Parameters[1].Matrix.Evaluate(Variant);
                    this._Cache.GetWriter(Key).WriteLine(m.ToString());
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Record))
                {
                    Cell Key = this._Parameters[0].Scalar.Evaluate(Variant);
                    Record r = this._Parameters[1].Record.Evaluate(Variant);
                    this._Cache.GetWriter(Key).WriteLine(r.ToString());
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Table))
                {
                    Cell Key = this._Parameters[0].Scalar.Evaluate(Variant);
                    Table t = this._Parameters[1].Table.Select(Variant);
                    using (RecordReader rr = t.OpenReader())
                    {
                        this._Cache.GetWriter(Key).WriteLine(rr.ReadNext().ToString());
                    }
                }



            }

        }

        public sealed class aeWRITE_BLOCK : ActionExpressionParameterized
        {

            private StreamCache _Cache;

            public aeWRITE_BLOCK(Host Host, StreamCache Cache)
                : base(Host, null, WRITE_BLOCK, 1)
            {
                this._Cache = Cache;
            }

            public override void Invoke(SpoolSpace Variant)
            {

                if (!this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                    throw new Exception("Expecting two scalar parameters");

                Cell Key = this._Parameters[0].Scalar.Evaluate(Variant);
                Cell Value = this._Parameters[1].Scalar.Evaluate(Variant);
                byte[] buff = Value.valueBINARY;

                this._Cache.GetWriter(Key).BaseStream.Write(buff, 0, buff.Length);

            }

        }

        // Scalars //
        public sealed class sfEOS : ScalarExpressionFunction
        {

            private StreamCache _Cache;

            public sfEOS(Host Host, StreamCache Cache)
                : base(Host, null, EOS, 1, CellAffinity.BOOL)
            {
                this._Cache = Cache;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                
                if (!this.CheckSigniture(ParameterAffinity.Scalar))
                    throw new Exception("Expecting a single scalar");
                Cell Key = this._Params[0].Scalar.Evaluate(Variants);
                bool b = this._Cache.GetReader(Key).EndOfStream;
                return (b ? CellValues.True : CellValues.False);

            }

        }

        public sealed class sfREAD_LINE : ScalarExpressionFunction
        {

            private StreamCache _Cache;

            public sfREAD_LINE(Host Host, StreamCache Cache)
                : base(Host, null, READ_LINE, 1, CellAffinity.CSTRING)
            {
                this._Cache = Cache;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (!this.CheckSigniture(ParameterAffinity.Scalar))
                    throw new Exception("Expecting a single scalar");
                Cell Key = this._Params[0].Scalar.Evaluate(Variants);
                if (this._Cache.GetReader(Key).EndOfStream)
                    return CellValues.NullCSTRING;
                string x = this._Cache.GetReader(Key).ReadLine();
                return new Cell(x);

            }

        }

        public sealed class sfREAD_BLOCK : ScalarExpressionFunction
        {

            private StreamCache _Cache;

            public sfREAD_BLOCK(Host Host, StreamCache Cache)
                : base(Host, null, READ_BLOCK, 2, CellAffinity.BINARY)
            {
                this._Cache = Cache;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (!this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                    throw new Exception("Expecting a single scalar");
                Cell Key = this._Params[0].Scalar.Evaluate(Variants);
                int size = this._Params[1].Scalar.Evaluate(Variants).valueINT;
                if (this._Cache.GetReader(Key).EndOfStream)
                    return CellValues.NullBLOB;
                byte[] b = new byte[size];
                size = Math.Min(size, (int)(this._Cache.GetReader(Key).BaseStream.Length - this._Cache.GetReader(Key).BaseStream.Position));
                int x = this._Cache.GetReader(Key).BaseStream.Read(b, 0, size);
                return new Cell(b);

            }

        }

        public sealed class sfDeflate : ScalarExpressionFunction
        {

            public sfDeflate(Host Host)
                : base(Host, null, DEFLATE, 1, CellAffinity.BINARY)
            {
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfDeflate(this._Host);
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (!this.CheckSigniture(ParameterAffinity.Scalar))
                    throw new Exception("Expecting a single parameter");

                Cell c = this._Params[0].Scalar.Evaluate(Variants);
                if (c.IsNull)
                    return CellValues.NullBLOB;
                byte[] buffer = c.valueBINARY;

                MemoryStream ms = new MemoryStream();
                using (DeflateStream ds = new DeflateStream(ms, CompressionLevel.Optimal))
                {
                    ds.Write(buffer, 0, buffer.Length);
                }
                return new Cell(ms.ToArray());

            }

        }

        public sealed class sfInflate : ScalarExpressionFunction
        {

            public sfInflate(Host Host)
                : base(Host, null, DEFLATE, 1, CellAffinity.BINARY)
            {
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfInflate(this._Host);
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (!this.CheckSigniture(ParameterAffinity.Scalar))
                    throw new Exception("Expecting a single parameter");

                Cell c = this._Params[0].Scalar.Evaluate(Variants);
                if (c.IsNull)
                    return CellValues.NullBLOB;
                byte[] buffer = c.valueBINARY;

                MemoryStream ms_in = new MemoryStream(buffer);
                MemoryStream ms_out = new MemoryStream();
                using (DeflateStream ds = new DeflateStream(ms_in, CompressionMode.Decompress))
                {
                    ds.CopyTo(ms_out);
                }
                return new Cell(ms_out.ToArray());

            }

        }

    }

}
