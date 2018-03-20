using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Expressions.TableExpressions;
using ArcticWind.Expressions.ActionExpressions;
using ArcticWind.Expressions;
using ArcticWind.Tables;
using ArcticWind.Elements;
using ArcticWind.Elements.Structures;
using System.IO;

namespace ArcticWind.Libraries
{

    public sealed class BaseLibrary : Library
    {

        public const string SUBSTR = "SUBSTR";
        public const string REPLACE = "REPLACE";
        public const string FIND = "FIND";
        public const string TRIM = "TRIM";
        public const string LENGTHOF = "LENGTHOF";
        public const string TYPEOF = "TYPEOF";
        public const string TYPEOFS = "TYPEOFS";
        public const string SIZEOF = "SIZEOF";
        public const string NAMEOF = "NAMEOF";
        public const string ROW_COUNT = "ROW_COUNT";
        public const string COLUMN_COUNT = "COLUMN_COUNT";
        public const string GUID = "GUID";
        public const string SSUM = "SSUM";
        public const string SMIN = "SMIN";
        public const string SMAX = "SMAX";
        public const string COALESCE = "COALESCE";
        public const string DENSEA = "DENSEA";
        public const string DENSEB = "DENSEB";
        public const string DENSE = "DENSE";
        public const string MATCH = "MATCH";
        private static readonly string[] ScalarFunctionNames = { SUBSTR, REPLACE, FIND, TRIM, LENGTHOF, TYPEOF, TYPEOFS, SIZEOF, NAMEOF, ROW_COUNT, COLUMN_COUNT, GUID, SSUM, SMIN, SMAX, COALESCE, DENSEA, DENSEB, DENSE, MATCH };

        public const string SPLIT = "SPLIT";
        public const string SPLICE = "SPLICE";
        public const string BYTES = "BYTES";
        public const string CHARS = "CHARS";
        private static readonly string[] MatrixFunctionNames = { SPLIT, SPLICE, BYTES, CHARS };

        private static readonly string[] RecordFunctionNames = { };
        
        private static readonly string[] TableFunctionNames = { };

        public const string IMPORT = "IMPORT";
        public const string EXPORT = "EXPORT";
        private static readonly string[] ActionNames = { IMPORT, EXPORT };

        public BaseLibrary(Host Host)
            : base(Host, Host.GLOBAL)
        {
        }

        // Methods //
        public override bool ScalarFunctionExists(string Name)
        {
            return ScalarFunctionNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override bool MatrixFunctionExists(string Name)
        {
            return MatrixFunctionNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override bool RecordFunctionExists(string Name)
        {
            return RecordFunctionNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override bool TableFunctionExists(string Name)
        {
            return TableFunctionNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override bool ActionExists(string Name)
        {
            return ActionNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override ScalarExpressionFunction ScalarFunctionLookup(string Name)
        {

            switch (Name.ToUpper())
            {
                case SUBSTR: return new sfSUBSTR(this._Host);
                case REPLACE: return new sfREPLACE(this._Host);
                case FIND: return new sfFIND(this._Host);
                case TRIM: return new sfTRIM(this._Host);
                case LENGTHOF: return new sfLENGTHOF(this._Host);
                case TYPEOF: return new sfTYPEOF(this._Host);
                case TYPEOFS: return new sfTYPEOFS(this._Host);
                case SIZEOF: return new sfSIZEOF(this._Host);
                case NAMEOF: return new sfNAMEOF(this._Host);
                case ROW_COUNT: return new sfROW_COUNT(this._Host);
                case COLUMN_COUNT: return new sfCOLUMN_COUNT(this._Host);
                case GUID: return new sfGUID(this._Host);
                case SSUM: return new sfSSUM(this._Host);
                case SMIN: return new sfSMIN(this._Host);
                case SMAX: return new sfSMAX(this._Host);
                case COALESCE: return new sfCOALESCE(this._Host);
                case DENSEA: return new sfDENSEA(this._Host);
                case DENSEB: return new sfDENSEB(this._Host);
                case DENSE: return new sfDENSE(this._Host);
                case MATCH: return new sfMATCH(this._Host);
            }

            throw new Exception(string.Format("Scalar function '{0}' does not exist", Name));
        }

        public override MatrixExpressionFunction MatrixFunctionLookup(string Name)
        {

            switch (Name.ToUpper())
            {
                case SPLIT:
                    return new mfSPLIT();
                case SPLICE:
                    return new mfSPLICE();
                case BYTES:
                    return new mfBYTES();
                case CHARS:
                    return new mfCHARS();
            }
            throw new Exception(string.Format("Matrix function '{0}' does not exist", Name));

        }

        public override RecordExpressionFunction RecordFunctionLookup(string Name)
        {
            throw new Exception(string.Format("Record function '{0}' does not exist", Name));
        }

        public override TableExpressionFunction TableFunctionLookup(string Name)
        {
            throw new Exception(string.Format("Table function '{0}' does not exist", Name));
        }

        public override ActionExpressionParameterized ActionLookup(string Name)
        {

            switch (Name.ToUpper())
            {
                case EXPORT: return new aeEXPORT(this._Host);
                case IMPORT: return new aeIMPORT(this._Host);
            }
            throw new Exception(string.Format("Action '{0}' does not exist", Name));
        }

        public override void ShutDown()
        {
            base.ShutDown();
        }

        // Actions //
        public sealed class aeIMPORT : ActionExpressionParameterized
        {

            public aeIMPORT(Host Host)
                : base(Host, null, IMPORT, 5)
            {
            }

            /*
             * TABLE:
             * PATH:
             * DELIM:
             * ESCAPE:
             * SKIP:
             * 
             */

            public override void Invoke(SpoolSpace Variant)
            {
                
                // Check the signiture //
                if (!this.CheckSigniture(ParameterAffinity.Table, ParameterAffinity.Scalar, ParameterAffinity.Scalar, ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                    throw new Exception("Invalid parameters");

                // Get the table //
                TableExpression te = this._Parameters[0].Table;
                if (!(te is TableExpressionStoreRef))
                    throw new Exception("Table expression must refer to an existing table, not an expression");
                Table t = te.Select(Variant); // Just returns the source table 
                
                // Get the ancilary parameters //
                
                // Path //
                Cell Path = this._Parameters[1].Scalar.Evaluate(Variant);
                if (Path.IsNull) throw new Exception("Path must be non-null");
                string spath = Path.valueCSTRING;
                if (!File.Exists(spath))
                    throw new Exception(string.Format("File '{0}' does not exist", spath));

                // Delims
                Cell Delims = this._Parameters[2].Scalar.Evaluate(Variant);
                char[] cdelims = (Delims.IsNull ? new char[] { '\t' } : Delims.valueCSTRING.ToCharArray());
                
                // Escape //
                Cell Escape = this._Parameters[3].Scalar.Evaluate(Variant);
                char esc = (Escape.IsNull || Escape.valueCSTRING.Length == 0 ? char.MaxValue : Escape.valueCSTRING.First());
 
                // Skips //
                Cell Skip = this._Parameters[4].Scalar.Evaluate(Variant);
                int iskip = Math.Abs(Skip.valueINT);

                // Import //
                using (RecordWriter rw = t.OpenWriter())
                {
                    TableImporter.Import(rw, spath, cdelims, esc, iskip); 
                }

            }

        }

        public sealed class aeEXPORT : ActionExpressionParameterized
        {

            public aeEXPORT(Host Host)
                : base(Host, null, EXPORT, 4)
            {

            }

            public override void Invoke(SpoolSpace Variant)
            {


                // Check the signiture //
                if (!this.CheckSigniture(ParameterAffinity.Table, ParameterAffinity.Scalar, ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                    throw new Exception("Invalid parameters");

                // Get the table //
                TableExpression te = this._Parameters[0].Table;

                // Get the ancilary parameters //

                // Path //
                Cell Path = this._Parameters[1].Scalar.Evaluate(Variant);
                if (Path.IsNull) throw new Exception("Path must be non-null");
                string spath = Path.valueCSTRING;
                if (!File.Exists(spath))
                    throw new Exception(string.Format("File '{0}' does not exist", spath));

                // Delims
                Cell Delims = this._Parameters[2].Scalar.Evaluate(Variant);
                char cdelim = (Delims.IsNull || Delims.valueCSTRING.Length == 0 ? '\t' : Delims.valueCSTRING.First());

                // Escape //
                Cell Escape = this._Parameters[3].Scalar.Evaluate(Variant);
                char esc = (Escape.IsNull || Escape.valueCSTRING.Length == 0 ? char.MaxValue : Escape.valueCSTRING.First());

                using (RecordReader rr = te.Select(Variant).OpenReader())
                {
                    TableImporter.Export(rr, spath, true, cdelim, esc);
                }

            }

        }

        // Matrix Functions //
        public sealed class mfSPLIT : MatrixExpressionFunction
        {

            private int _Size = 0;

            public mfSPLIT()
                : base(null, SPLIT, -3, CellAffinity.CSTRING)
            {

            }

            public override int ReturnSize()
            {
                return this._Size;
            }

            public override MatrixExpression CloneOfMe()
            {
                return new mfSPLIT();
            }

            public override CellMatrix Evaluate(SpoolSpace Variant)
            {

                this.CheckParameters();

                bool x = this._Parameters.TrueForAll((y) => { return y.Affinity == ParameterAffinity.Scalar; });
                if (!x)
                    throw new Exception("This function requires all scalars");

                string val = this._Parameters[0].Scalar.Evaluate(Variant).valueCSTRING;
                string delims = this._Parameters[1].Scalar.Evaluate(Variant).valueCSTRING;
                char escape = (this._Parameters.Count > 2 ? this._Parameters[2].Scalar.Evaluate(Variant).valueCSTRING.FirstOrDefault() : char.MaxValue);

                string[] s = Util.StringUtil.Split(val, delims.ToCharArray(), escape);
                
                this._Size = val.Length;
                CellMatrix c = new CellMatrix(s.Length, 1, CellAffinity.CSTRING, s.Max((t) => { return (t ?? "").Length; }));
                for (int i = 0; i < s.Length; i++)
                {
                    c[i, 0] = new Cell(s[i]);
                }

                return c;

            }

        }

        public sealed class mfSPLICE : MatrixExpressionFunction
        {

            private int _Size = 0;

            public mfSPLICE()
                : base(null, SPLICE, 2, CellAffinity.CSTRING)
            {

            }

            public override int ReturnSize()
            {
                return this._Size;
            }

            public override MatrixExpression CloneOfMe()
            {
                return new mfSPLICE();
            }

            public override CellMatrix Evaluate(SpoolSpace Variant)
            {

                this.CheckParameters();

                if (this._Parameters[0].Affinity != ParameterAffinity.Scalar || this._Parameters[1].Affinity != ParameterAffinity.Matrix)
                    throw new Exception("This functions requires a scalar and a matrix");

                string s = this._Parameters[0].Scalar.Evaluate(Variant).valueCSTRING;
                CellMatrix m = this._Parameters[1].Matrix.Evaluate(Variant);
                List<int> indexes = new List<int>();
                foreach (Cell c in m)
                {
                    indexes.Add(c.valueINT);
                }


                string[] q = Util.StringUtil.Splice(s, indexes.ToArray());
                this._Size = s.Length;
                CellMatrix v = new CellMatrix(q.Length, 1, CellAffinity.CSTRING, q.Max((t) => { return t.Length; }));
                for (int i = 0; i < s.Length; i++)
                {
                    v[i, 0] = new Cell(s[i]);
                }

                return v;

            }

        }

        public sealed class mfBYTES : MatrixExpressionFunction
        {

            private int _Size = 0;

            public mfBYTES()
                : base(null, BYTES, 1, CellAffinity.BYTE)
            {

            }

            public override int ReturnSize()
            {
                return this._Size;
            }

            public override MatrixExpression CloneOfMe()
            {
                return new mfBYTES();
            }

            public override CellMatrix Evaluate(SpoolSpace Variant)
            {

                this.CheckParameters();

                if (this._Parameters[0].Affinity != ParameterAffinity.Scalar)
                    throw new Exception("This functions requires a scalar");

                byte[] b = this._Parameters[0].Scalar.Evaluate(Variant).valueBINARY;
                CellMatrix m = new CellMatrix(b.Length, 1, CellAffinity.BYTE, CellSerializer.BYTE_SIZE);
                for (int i = 0; i < b.Length; i++)
                {
                    m[i, 0] = new Cell(b[i]);
                }

                return m;

            }

        }

        public sealed class mfCHARS : MatrixExpressionFunction
        {

            private int _Size = 0;

            public mfCHARS()
                : base(null, CHARS, 1, CellAffinity.CSTRING)
            {

            }

            public override int ReturnSize()
            {
                return this._Size;
            }

            public override MatrixExpression CloneOfMe()
            {
                return new mfCHARS();
            }

            public override CellMatrix Evaluate(SpoolSpace Variant)
            {

                this.CheckParameters();

                if (this._Parameters[0].Affinity != ParameterAffinity.Scalar)
                    throw new Exception("This functions requires a scalar");

                char[] c = this._Parameters[0].Scalar.Evaluate(Variant).valueCSTRING.ToCharArray();
                CellMatrix m = new CellMatrix(c.Length, 1, CellAffinity.CSTRING, 1);
                for (int i = 0; i < c.Length; i++)
                {
                    m[i, 0] = new Cell(c[i].ToString());
                }

                return m;

            }

        }

        // Scalar Functions //
        public sealed class sfSUBSTR : ScalarExpressionFunction
        {

            public sfSUBSTR(Host Host)
                : base(Host, null, SUBSTR, 3)
            {
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfSUBSTR(this._Host);
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this._Params.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                if (!(this._Params[0].Affinity == ParameterAffinity.Scalar && this._Params[1].Affinity == ParameterAffinity.Scalar && this._Params[2].Affinity == ParameterAffinity.Scalar))
                    throw new Exception("This function requires all scalars");

                Cell value = this._Params[0].Scalar.Evaluate(Variants);
                Cell start = this._Params[1].Scalar.Evaluate(Variants);
                Cell length = this._Params[2].Scalar.Evaluate(Variants);

                return CellFunctions.Substring(value, start.valueLONG, length.valueLONG);

            }

        }

        public class sfREPLACE : ScalarExpressionFunction
        {

            public sfREPLACE(Host Host)
                : base(Host, null, REPLACE, 3)
            {
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfREPLACE(this._Host);
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                {
                    Cell value = this._Params[0].Scalar.Evaluate(Variants);
                    Cell pattern = this._Params[1].Scalar.Evaluate(Variants);
                    Cell new_pattern = this._Params[2].Scalar.Evaluate(Variants);
                    return CellFunctions.Replace(value, pattern, new_pattern);
                }

                if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Matrix, ParameterAffinity.Matrix))
                {
                    Cell value = this._Params[0].Scalar.Evaluate(Variants);
                    CellMatrix pattern = this._Params[1].Matrix.Evaluate(Variants);
                    CellMatrix new_pattern = this._Params[2].Matrix.Evaluate(Variants);
                    if (pattern.RowCount != new_pattern.RowCount || pattern.ColumnCount != 1 || new_pattern.ColumnCount != 1)
                        throw new Exception("The pattern and new pattern matricies must be the same legth and have a column rank of one");
                    for (int i = 0; i < pattern.RowCount; i++)
                    {
                        value = CellFunctions.Replace(value, pattern[i], new_pattern[i]);
                    }
                    return value;
                }

                if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Matrix, ParameterAffinity.Scalar))
                {
                    Cell value = this._Params[0].Scalar.Evaluate(Variants);
                    CellMatrix pattern = this._Params[1].Matrix.Evaluate(Variants);
                    Cell new_pattern = this._Params[2].Scalar.Evaluate(Variants);
                    foreach (Cell c in pattern)
                        value = CellFunctions.Replace(value, c, new_pattern);
                    return value;
                }

                throw new Exception("Expecting three parameters, of the form: (scalar,scalar,scalar) or (scalar,matrix,matrix) or (scalar,matrix,scalar)");

            }

        }

        public class sfFIND : ScalarExpressionFunction
        {

            public sfFIND(Host Host)
                : base(Host, null, FIND, -2, CellAffinity.LONG)
            {
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfFIND(this._Host);
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this._Params.Count < Math.Abs(this._MaxParamterCount))
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                if (!(this._Params[0].Affinity == ParameterAffinity.Scalar && this._Params[1].Affinity == ParameterAffinity.Scalar))
                    throw new Exception("This function requires all scalars");

                Cell value = this._Params[0].Scalar.Evaluate(Variants);
                Cell pattern = this._Params[1].Scalar.Evaluate(Variants);
                int start = (this._Params.Count >= 3 ? (int)this._Params[2].Scalar.Evaluate(Variants).valueLONG : 0);

                return CellFunctions.Position(value, pattern, start);

            }

        }

        public class sfTRIM : ScalarExpressionFunction
        {

            public sfTRIM(Host Host)
                : base(Host, null, TRIM, 1)
            {
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfTRIM(this._Host);
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this._Params.Count != Math.Abs(this._MaxParamterCount))
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));
                if (this._Params[0].Affinity != ParameterAffinity.Scalar)
                    throw new Exception("This function requires all scalars");

                Cell x = this._Params[0].Scalar.Evaluate(Variants);
                if (x.IsNull)
                    return new Cell(this.ReturnAffinity());

                return CellFunctions.Trim(x);

            }

        }

        public class sfLENGTHOF : ScalarExpressionFunction
        {

            public sfLENGTHOF(Host Host)
                : base(Host, null, LENGTHOF, 1, CellAffinity.INT)
            {
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfLENGTHOF(this._Host);
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this._Params.Count != Math.Abs(this._MaxParamterCount))
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                if (this._Params[0].Affinity == ParameterAffinity.Scalar)
                {
                    return CellSerializer.Length(this._Params[0].Scalar.Evaluate(Variants));
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Matrix)
                {
                    return new Cell(this._Params[0].Matrix.Evaluate(Variants).Size);
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Record)
                {
                    return new Cell(this._Params[0].Record.Columns.Count);
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Table)
                {
                    return new Cell(this._Params[0].Table.Columns.Count);
                }

                return CellValues.NullINT;

            }

        }

        public class sfTYPEOF : ScalarExpressionFunction
        {

            public sfTYPEOF(Host Host)
                : base(Host, null, TYPEOF, 1, CellAffinity.INT)
            {
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfTYPEOF(this._Host);
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this._Params.Count != Math.Abs(this._MaxParamterCount))
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                int t = -1;
                if (this._Params[0].Affinity == ParameterAffinity.Scalar)
                {
                    t = (int)this._Params[0].Scalar.ReturnAffinity();
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Matrix)
                {
                    t = (int)this._Params[0].Matrix.ReturnAffinity();
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Record)
                {
                    t = 100;
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Table)
                {
                    t = 200;
                }

                return new Cell(t);

            }

        }

        public class sfTYPEOFS : ScalarExpressionFunction
        {

            public sfTYPEOFS(Host Host)
                : base(Host, null, TYPEOFS, 1, CellAffinity.BSTRING)
            {
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfTYPEOF(this._Host);
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this._Params.Count != Math.Abs(this._MaxParamterCount))
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                BString x = "INVALID";
                if (this._Params[0].Affinity == ParameterAffinity.Scalar)
                {
                    x = this._Params[0].Scalar.ReturnAffinity().ToString();
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Matrix)
                {
                    x = this._Params[0].Matrix.ReturnAffinity().ToString();
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Record)
                {
                    x = "RECORD";
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Table)
                {
                    x = "TABLE";
                }
                return new Cell(x);

            }

        }

        public class sfSIZEOF : ScalarExpressionFunction
        {

            public sfSIZEOF(Host Host)
                : base(Host, null, SIZEOF, 1, CellAffinity.LONG)
            {
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfSIZEOF(this._Host);
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this._Params.Count != Math.Abs(this._MaxParamterCount))
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                long t = -1;
                if (this._Params[0].Affinity == ParameterAffinity.Scalar)
                {
                    t = CellSerializer.DiskSize((long)this._Params[0].Scalar.Evaluate(Variants));
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Matrix)
                {
                    CellMatrix m = this._Params[0].Matrix.Evaluate(Variants);
                    t = m.Size * m.RowCount * m.ColumnCount;
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Record)
                {
                    t = 100;
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Table)
                {
                    Table q = this._Params[0].Table.Select(Variants);
                    t = q.PageCount * q.PageSize + TableHeader.SIZE;
                }

                return new Cell(t);

            }

        }

        public class sfNAMEOF : ScalarExpressionFunction
        {

            public sfNAMEOF(Host Host)
                : base(Host, null, NAMEOF, 1, CellAffinity.BSTRING)
            {
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfNAMEOF(this._Host);
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this._Params.Count != Math.Abs(this._MaxParamterCount))
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                string n = null;
                if (this._Params[0].Affinity == ParameterAffinity.Scalar)
                {
                    if (this._Params[0].Scalar is ScalarExpressionFunction)
                        n = (this._Params[0].Scalar as ScalarExpressionFunction).Name;
                    else
                        n = this._Params[0].Scalar.BuildAlias();
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Matrix)
                {
                    if (this._Params[0].Matrix is MatrixExpressionFunction)
                        n = (this._Params[0].Matrix as MatrixExpressionFunction).FunctionName;
                    else if (this._Params[0].Matrix is MatrixExpressionStoreRef)
                        n = (this._Params[0].Matrix as MatrixExpressionStoreRef).MatrixName;
                    else
                        n = "$UNKNOWN";
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Record)
                {
                    if (this._Params[0].Record is RecordExpressionStoreRef)
                        n = (this._Params[0].Record as RecordExpressionStoreRef).RecordName;
                    else if (this._Params[0].Record is RecordExpressionFunction)
                        n = (this._Params[0].Record as RecordExpressionFunction).FunctionName;
                    else
                        n = "@UNKNOWN";
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Table)
                {

                    if (this._Params[0].Table is TableExpressionStoreRef)
                        n = (this._Params[0].Table as TableExpressionStoreRef).Select(Variants).Header.Path;
                    else
                        n = this._Params[0].Table.Alias;
                }

                return new Cell(n);

            }

        }

        public class sfROW_COUNT : ScalarExpressionFunction
        {

            public sfROW_COUNT(Host Host)
                : base(Host, null, ROW_COUNT, 1, CellAffinity.INT)
            {
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfROW_COUNT(this._Host);
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this._Params.Count != Math.Abs(this._MaxParamterCount))
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                long n = -1;
                if (this._Params[0].Affinity == ParameterAffinity.Scalar || this._Params[0].Affinity == ParameterAffinity.Record)
                {
                    n = 1;
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Matrix)
                {
                    n = this._Params[0].Matrix.Evaluate(Variants).RowCount;
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Table)
                {
                    n = this._Params[0].Table.EstimatedCount;
                }

                return new Cell(n);

            }

        }

        public class sfCOLUMN_COUNT : ScalarExpressionFunction
        {

            public sfCOLUMN_COUNT(Host Host)
                : base(Host, null, COLUMN_COUNT, 1, CellAffinity.INT)
            {
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfCOLUMN_COUNT(this._Host);
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this._Params.Count != Math.Abs(this._MaxParamterCount))
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                long n = -1;
                if (this._Params[0].Affinity == ParameterAffinity.Scalar)
                {
                    n = 1;
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Matrix)
                {
                    n = this._Params[0].Matrix.Evaluate(Variants).ColumnCount;
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Record)
                {
                    n = this._Params[0].Record.Columns.Count;
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Table)
                {
                    n = this._Params[0].Table.Columns.Count;
                }

                return new Cell(n);

            }

        }

        public sealed class sfGUID : ScalarExpressionFunction
        {

            public sfGUID(Host Host)
                : base(Host, null, GUID, 0, CellAffinity.BINARY)
            {
            }

            public override int ReturnSize()
            {
                return 16;
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfGUID(this._Host);
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(Expressions.SpoolSpace Variants)
            {
                return new Cell(Guid.NewGuid().ToByteArray());
            }

        }

        public sealed class sfSSUM : ScalarExpressionFunction
        {

            private int _size = 0;
            private bool _IsRun = false;
            private bool _IsVolitile = false;

            public sfSSUM(Host Host)
                : base(Host, null, SSUM, -127)
            {
            }

            public override int ReturnSize()
            {
                this.SetUpMetaData();
                return this._size;
            }

            public override CellAffinity ReturnAffinity()
            {
                this.SetUpMetaData();
                return base.ReturnAffinity();
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfSSUM(this._Host);
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(Expressions.SpoolSpace Variants)
            {

                this.SetUpMetaData();

                if (this._Params.Count == 0)
                {
                    return CellValues.Null(this.ReturnAffinity());
                }

                Cell c = CellValues.Zero(this.ReturnAffinity());

                foreach (Parameter p in this._Params)
                {
                    if (p.Affinity == ParameterAffinity.Scalar)
                        c += p.Scalar.Evaluate(Variants);
                    else if (p.Affinity == ParameterAffinity.Matrix)
                        c += CellMatrix.Sum(p.Matrix.Evaluate(Variants));
                    else
                        throw new Exception("This function requires a scalar or a matrix");
                }

                return c;


            }

            private void SetUpMetaData()
            {

                if (!this._IsRun)
                    return;

                foreach (Parameter p in this._Params)
                {

                    if (p.Affinity == ParameterAffinity.Scalar)
                    {
                        this._ReturnAffinity = CellAffinityHelper.Highest(this._ReturnAffinity, p.Scalar.ReturnAffinity());
                        this._size = Math.Max(this._size, p.Scalar.ReturnSize());
                        this._IsVolitile = p.Scalar.IsVolatile | this._IsVolitile;
                    }
                    else if (p.Affinity == ParameterAffinity.Matrix)
                    {
                        this._ReturnAffinity = CellAffinityHelper.Highest(this._ReturnAffinity, p.Matrix.ReturnAffinity());
                        this._size = Math.Max(this._size, p.Matrix.ReturnSize());
                    }
                    else
                    {
                        throw new Exception("This function requires a scalar or a matrix");
                    }

                }
                this._IsRun = true;

            }

        }

        public sealed class sfSMIN : ScalarExpressionFunction
        {

            private int _size = 0;
            private bool _IsRun = false;
            private bool _IsVolitile = false;

            public sfSMIN(Host Host)
                : base(Host, null, SMIN, -127)
            {
            }

            public override int ReturnSize()
            {
                this.SetUpMetaData();
                return this._size;
            }

            public override CellAffinity ReturnAffinity()
            {
                this.SetUpMetaData();
                return base.ReturnAffinity();
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfSSUM(this._Host);
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(Expressions.SpoolSpace Variants)
            {

                this.SetUpMetaData();

                if (this._Params.Count == 0)
                {
                    return CellValues.Null(this.ReturnAffinity());
                }

                Cell c = CellValues.Max(this.ReturnAffinity());

                foreach (Parameter p in this._Params)
                {
                    if (p.Affinity == ParameterAffinity.Scalar)
                        c = CellFunctions.Min(c, p.Scalar.Evaluate(Variants));
                    else if (p.Affinity == ParameterAffinity.Matrix)
                        c = CellFunctions.Min(c, CellMatrix.Min(p.Matrix.Evaluate(Variants)));
                    else
                        throw new Exception("This function requires a scalar or a matrix");
                }

                return c;


            }

            private void SetUpMetaData()
            {

                if (!this._IsRun)
                    return;

                foreach (Parameter p in this._Params)
                {

                    if (p.Affinity == ParameterAffinity.Scalar)
                    {
                        this._ReturnAffinity = CellAffinityHelper.Highest(this._ReturnAffinity, p.Scalar.ReturnAffinity());
                        this._size = Math.Max(this._size, p.Scalar.ReturnSize());
                        this._IsVolitile = p.Scalar.IsVolatile | this._IsVolitile;
                    }
                    else if (p.Affinity == ParameterAffinity.Matrix)
                    {
                        this._ReturnAffinity = CellAffinityHelper.Highest(this._ReturnAffinity, p.Matrix.ReturnAffinity());
                        this._size = Math.Max(this._size, p.Matrix.ReturnSize());
                    }
                    else
                    {
                        throw new Exception("This function requires a scalar or a matrix");
                    }

                }
                this._IsRun = true;

            }

        }

        public sealed class sfSMAX : ScalarExpressionFunction
        {

            private int _size = 0;
            private bool _IsRun = false;
            private bool _IsVolitile = false;

            public sfSMAX(Host Host)
                : base(Host, null, SMAX, -127)
            {
            }

            public override int ReturnSize()
            {
                this.SetUpMetaData();
                return this._size;
            }

            public override CellAffinity ReturnAffinity()
            {
                this.SetUpMetaData();
                return base.ReturnAffinity();
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfSMAX(this._Host);
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(Expressions.SpoolSpace Variants)
            {

                this.SetUpMetaData();

                if (this._Params.Count == 0)
                {
                    return CellValues.Null(this.ReturnAffinity());
                }

                Cell c = CellValues.Min(this.ReturnAffinity());

                foreach (Parameter p in this._Params)
                {
                    if (p.Affinity == ParameterAffinity.Scalar)
                        c = CellFunctions.Max(c, p.Scalar.Evaluate(Variants));
                    else if (p.Affinity == ParameterAffinity.Matrix)
                        c = CellFunctions.Max(c, CellMatrix.Min(p.Matrix.Evaluate(Variants)));
                    else
                        throw new Exception("This function requires a scalar or a matrix");
                }

                return c;


            }

            private void SetUpMetaData()
            {

                if (!this._IsRun)
                    return;

                foreach (Parameter p in this._Params)
                {

                    if (p.Affinity == ParameterAffinity.Scalar)
                    {
                        this._ReturnAffinity = CellAffinityHelper.Highest(this._ReturnAffinity, p.Scalar.ReturnAffinity());
                        this._size = Math.Max(this._size, p.Scalar.ReturnSize());
                        this._IsVolitile = p.Scalar.IsVolatile | this._IsVolitile;
                    }
                    else if (p.Affinity == ParameterAffinity.Matrix)
                    {
                        this._ReturnAffinity = CellAffinityHelper.Highest(this._ReturnAffinity, p.Matrix.ReturnAffinity());
                        this._size = Math.Max(this._size, p.Matrix.ReturnSize());
                    }
                    else
                    {
                        throw new Exception("This function requires a scalar or a matrix");
                    }

                }
                this._IsRun = true;

            }

        }

        public sealed class sfCOALESCE : ScalarExpressionFunction
        {

            private int _size = 0;
            private bool _IsRun = false;
            private bool _IsVolitile = false;

            public sfCOALESCE(Host Host)
                : base(Host, null, COALESCE, -127)
            {
            }

            public override int ReturnSize()
            {
                this.SetUpMetaData();
                return this._size;
            }

            public override CellAffinity ReturnAffinity()
            {
                this.SetUpMetaData();
                return base.ReturnAffinity();
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfSMAX(this._Host);
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(Expressions.SpoolSpace Variants)
            {

                this.SetUpMetaData();

                if (this._Params.Count == 0)
                {
                    return CellValues.Null(this.ReturnAffinity());
                }

                Cell c = CellValues.Null(this.ReturnAffinity());

                foreach (Parameter p in this._Params)
                {
                    if (p.Affinity == ParameterAffinity.Scalar)
                        c = CellFunctions.Coalesce(c, p.Scalar.Evaluate(Variants));
                    else if (p.Affinity == ParameterAffinity.Matrix)
                        c = CellFunctions.Coalesce(c, CellMatrix.Min(p.Matrix.Evaluate(Variants)));
                    else
                        throw new Exception("This function requires a scalar or a matrix");
                }

                return c;


            }

            private void SetUpMetaData()
            {

                if (!this._IsRun)
                    return;

                foreach (Parameter p in this._Params)
                {

                    if (p.Affinity == ParameterAffinity.Scalar)
                    {
                        this._ReturnAffinity = CellAffinityHelper.Highest(this._ReturnAffinity, p.Scalar.ReturnAffinity());
                        this._size = Math.Max(this._size, p.Scalar.ReturnSize());
                        this._IsVolitile = p.Scalar.IsVolatile | this._IsVolitile;
                    }
                    else if (p.Affinity == ParameterAffinity.Matrix)
                    {
                        this._ReturnAffinity = CellAffinityHelper.Highest(this._ReturnAffinity, p.Matrix.ReturnAffinity());
                        this._size = Math.Max(this._size, p.Matrix.ReturnSize());
                    }
                    else
                    {
                        throw new Exception("This function requires a scalar or a matrix");
                    }

                }
                this._IsRun = true;

            }

        }

        public sealed class sfDENSEA : ScalarExpressionFunction
        {

            private int _size = 0;
            private bool _IsRun = false;
            private bool _IsVolitile = false;

            public sfDENSEA(Host Host)
                : base(Host, null, DENSEA, 1, CellAffinity.INT)
            {
            }

            public override int ReturnSize()
            {
                return CellSerializer.INT_SIZE;
            }

            public override CellAffinity ReturnAffinity()
            {
                return CellAffinity.INT;
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfDENSEA(this._Host);
            }

            public override bool IsVolatile
            {
                get
                {
                    bool iv = false;
                    foreach (Parameter p in this._Params)
                    {
                        if (p.Affinity == ParameterAffinity.Scalar)
                        {
                            iv = iv | p.Scalar.IsVolatile;
                            if (iv) return true;
                        }
                    }
                    return false;
                }
            }

            public override Cell Evaluate(Expressions.SpoolSpace Variants)
            {

                if (this._Params.Count != 1 || this._Params[0].Affinity != ParameterAffinity.Scalar)
                    throw new Exception();

                Cell x = this._Params[0].Scalar.Evaluate(Variants);

                return new Cell(x.INT_A);

            }

        }

        public sealed class sfDENSEB : ScalarExpressionFunction
        {

            private int _size = 0;
            private bool _IsRun = false;
            private bool _IsVolitile = false;

            public sfDENSEB(Host Host)
                : base(Host, null, DENSEB, 1, CellAffinity.INT)
            {
            }

            public override int ReturnSize()
            {
                return CellSerializer.INT_SIZE;
            }

            public override CellAffinity ReturnAffinity()
            {
                return CellAffinity.INT;
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfDENSEB(this._Host);
            }

            public override bool IsVolatile
            {
                get
                {
                    bool iv = false;
                    foreach (Parameter p in this._Params)
                    {
                        if (p.Affinity == ParameterAffinity.Scalar)
                        {
                            iv = iv | p.Scalar.IsVolatile;
                            if (iv) return true;
                        }
                    }
                    return false;
                }
            }

            public override Cell Evaluate(Expressions.SpoolSpace Variants)
            {

                if (this._Params.Count != 1 || this._Params[0].Affinity != ParameterAffinity.Scalar)
                    throw new Exception();

                Cell x = this._Params[0].Scalar.Evaluate(Variants);

                return new Cell(x.INT_B);

            }

        }

        public sealed class sfDENSE : ScalarExpressionFunction
        {

            private int _size = 0;
            private bool _IsRun = false;
            private bool _IsVolitile = false;

            public sfDENSE(Host Host)
                : base(Host, null, DENSE, 2, CellAffinity.LONG)
            {
            }

            public override int ReturnSize()
            {
                return CellSerializer.LONG_SIZE;
            }

            public override CellAffinity ReturnAffinity()
            {
                return CellAffinity.LONG;
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfDENSEB(this._Host);
            }

            public override bool IsVolatile
            {
                get
                {
                    bool iv = false;
                    foreach (Parameter p in this._Params)
                    {
                        if (p.Affinity == ParameterAffinity.Scalar)
                        {
                            iv = iv | p.Scalar.IsVolatile;
                            if (iv) return true;
                        }
                    }
                    return false;
                }
            }

            public override Cell Evaluate(Expressions.SpoolSpace Variants)
            {

                if (this._Params.Count != 2 || this._Params[0].Affinity != ParameterAffinity.Scalar || this._Params[1].Affinity != ParameterAffinity.Scalar)
                    throw new Exception();

                Cell x = this._Params[0].Scalar.Evaluate(Variants);
                Cell y = this._Params[1].Scalar.Evaluate(Variants);

                return new Cell(x.valueINT, y.valueINT);

            }

        }

        public sealed class sfMATCH : ScalarExpressionFunction
        {

            public sfMATCH(Host Host)
                : base(Host, null, MATCH, 2, CellAffinity.LONG)
            {
            }

            public override int ReturnSize()
            {
                return CellSerializer.LONG_SIZE;
            }

            public override CellAffinity ReturnAffinity()
            {
                return CellAffinity.LONG;
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfDENSEB(this._Host);
            }

            public override bool IsVolatile
            {
                get
                {
                    bool iv = false;
                    foreach (Parameter p in this._Params)
                    {
                        if (p.Affinity == ParameterAffinity.Scalar)
                        {
                            iv = iv | p.Scalar.IsVolatile;
                            if (iv) return true;
                        }
                    }
                    return false;
                }
            }

            public override Cell Evaluate(Expressions.SpoolSpace Variants)
            {

                if (this._Params.Count != 2 || this._Params[0].Affinity != ParameterAffinity.Scalar || this._Params[1].Affinity != ParameterAffinity.Matrix)
                    throw new Exception();

                Cell x = this._Params[0].Scalar.Evaluate(Variants);
                CellMatrix y = this._Params[1].Matrix.Evaluate(Variants);

                return y.Match(x);

            }


        }

    }

}
