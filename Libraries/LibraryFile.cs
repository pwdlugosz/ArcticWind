using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.ActionExpressions;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Expressions.TableExpressions;
using ArcticWind.Expressions;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using ArcticWind.Tables;

namespace ArcticWind.Libraries
{

    /// <summary>
    /// File support library
    /// </summary>
    public class LibraryFile : Library
    {

        public const string DELETE = "DELETE";
        public const string COPY = "COPY";
        public const string MOVE = "MOVE";
        public const string WRITE = "WRITE";
        public const string DOWNLOAD = "DOWNLOAD";
        public const string ZIP = "ZIP";
        public const string UNZIP = "UNZIP";
        public static readonly string[] Actions = { DELETE, COPY, MOVE, WRITE, DOWNLOAD, ZIP, UNZIP };

        public const string EXISTS = "EXISTS";
        public const string READ_ALL_TEXT = "READ_ALL_TEXT";
        public const string READ_ALL_TEXTB = "READ_ALL_TEXTB";
        public const string READ_ALL_BYTES = "READ_ALL_BYTES";
        public const string MD5 = "MD5";
        public const string SHA1 = "SHA1";
        public const string SHA256 = "SHA256";
        public const string SHA384 = "SHA384";
        public const string SHA512 = "SHA512";
        public static readonly string[] ScalarFunctions = { EXISTS, READ_ALL_TEXT, READ_ALL_TEXTB, READ_ALL_BYTES, MD5, SHA1, SHA256, SHA384, SHA512 };

        public const string INFO = "INFO";
        public static readonly string[] RecordFunctions = { INFO };

        public const string READ_LINES = "READ_LINES";
        public const string READ_LINESB = "READ_LINESB";
        public static readonly string[] MatrixFunctions = { READ_LINES, READ_LINESB };

        public LibraryFile(Host Host)
            : base(Host, "FILE")
        {
        }

        /// <summary>
        /// Releases all streams
        /// </summary>
        public override void ShutDown()
        {
        }

        public override ActionExpressionParameterized ActionLookup(string Name)
        {

            string n = Name.Trim().ToUpper();

            switch (n)
            {
                case DELETE:
                    return new ActionExpressionDelete(this._Host, null);
                case COPY:
                    return new ActionExpressionCopy(this._Host, null);
                case MOVE:
                    return new ActionExpressionMove(this._Host, null);
                case WRITE:
                    return new ActionExpressionWrite(this._Host, null);
                case DOWNLOAD:
                    return new ActionExpressionDownload(this._Host, null);
                case ZIP:
                    return new ActionExpressionZip(this._Host, null);
                case UNZIP:
                    return new ActionExpressionUnzip(this._Host, null);
            }

            throw new Exception(string.Format("Action '{0}' does not exist", Name));

        }

        public override bool ActionExists(string Name)
        {
            return Actions.Contains(Name.ToUpper());
        }

        public override ScalarExpressionFunction ScalarFunctionLookup(string Name)
        {

            string t = Name.ToUpper().Trim();

            switch (t)
            {

                case EXISTS:
                    return new ScalarExpressionExists(this._Host);
                case READ_ALL_TEXT:
                    return new ScalarExpressionReadAllText(this._Host);
                case READ_ALL_TEXTB:
                    return new ScalarExpressionReadAllTextB(this._Host);
                case READ_ALL_BYTES:
                    return new ScalarExpressionReadAllBytes(this._Host);
                case MD5:
                    return new ScalarExpressionHash(this._Host,MD5, new MD5CryptoServiceProvider());
                case SHA1:
                    return new ScalarExpressionHash(this._Host, SHA1, new SHA1CryptoServiceProvider());
                case SHA256:
                    return new ScalarExpressionHash(this._Host, SHA256, new SHA256CryptoServiceProvider());
                case SHA384:
                    return new ScalarExpressionHash(this._Host, SHA384, new SHA384CryptoServiceProvider());
                case SHA512:
                    return new ScalarExpressionHash(this._Host, SHA512, new SHA512CryptoServiceProvider());


            }

            throw new Exception(string.Format("Scalar function '{0}' does not exist", Name));

        }

        public override bool ScalarFunctionExists(string Name)
        {
            return ScalarFunctions.Contains(Name.ToUpper());
        }

        public override RecordExpressionFunction RecordFunctionLookup(string Name)
        {

            string t = Name.ToUpper().Trim();

            switch (t)
            {

                case INFO:
                    return new RecordExpressionInfo(this._Host);

            }

            throw new Exception(string.Format("Record function '{0}' does not exist", Name));

        }

        public override bool RecordFunctionExists(string Name)
        {
            return RecordFunctions.Contains(Name.ToUpper());
        }

        public override MatrixExpressionFunction MatrixFunctionLookup(string Name)
        {

            string t = Name.ToUpper().Trim();

            switch (t)
            {

                case READ_LINES:
                    return new MatrixExpressionReadLines();
                case READ_LINESB:
                    return new MatrixExpressionReadLinesB();

            }

            throw new Exception(string.Format("Matrix function '{0}' does not exist", Name));

        }

        public override bool MatrixFunctionExists(string Name)
        {
            return MatrixFunctions.Contains(Name.ToUpper());
        }

        public override TableExpressionFunction TableFunctionLookup(string Name)
        {

            string t = Name.ToUpper().Trim();

            throw new Exception(string.Format("Table function '{0}' does not exist", Name));

        }

        public override bool TableFunctionExists(string Name)
        {
            return MatrixFunctions.Contains(Name.ToUpper());
        }

        // Actions //
        public class ActionExpressionDelete : ActionExpressionParameterized
        {

            public ActionExpressionDelete(Host Host, ActionExpression Parent)
                : base(Host, Parent, DELETE, 1, "Deletes a give file")
            {
            }

            public override void Invoke(SpoolSpace Variant)
            {

                if (!this.CheckSigniture(ParameterAffinity.Scalar))
                    throw new Exception("Expecting a scalar");

                string path = this._Parameters[0].Scalar.Evaluate(Variant).valueCSTRING;

                if (File.Exists(path))
                    File.Delete(path);

            }


        }

        public class ActionExpressionCopy : ActionExpressionParameterized
        {

            public ActionExpressionCopy(Host Host, ActionExpression Parent)
                : base(Host, Parent, COPY, 2, "Copies a given file")
            {
            }

            public override void Invoke(SpoolSpace Variant)
            {

                if (!this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                    throw new Exception("Expecting a scalar");

                string old_path = this._Parameters[0].Scalar.Evaluate(Variant).valueCSTRING;
                string new_path = this._Parameters[1].Scalar.Evaluate(Variant).valueCSTRING;

                if (File.Exists(new_path))
                    File.Delete(new_path);
                File.Copy(old_path, new_path);

            }


        }

        public class ActionExpressionMove : ActionExpressionParameterized
        {

            public ActionExpressionMove(Host Host, ActionExpression Parent)
                : base(Host, Parent, MOVE, 2, "Moves a given file")
            {
            }

            public override void Invoke(SpoolSpace Variant)
            {

                if (!this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                    throw new Exception("Expecting a scalar");

                string old_path = this._Parameters[0].Scalar.Evaluate(Variant).valueCSTRING;
                string new_path = this._Parameters[1].Scalar.Evaluate(Variant).valueCSTRING;

                if (File.Exists(new_path))
                    File.Delete(new_path);
                File.Move(old_path, new_path);

            }


        }

        public class ActionExpressionWrite : ActionExpressionParameterized
        {

            public ActionExpressionWrite(Host Host, ActionExpression Parent)
                : base(Host, Parent, WRITE, -3, "Writes data to the end of an existing file; if the file does not exist, it will be created")
            {
            }

            public override void Invoke(SpoolSpace Variant)
            {

                if (!this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                    throw new Exception("Expecting scalars");

                string path = this._Parameters[0].Scalar.Evaluate(Variant).valueCSTRING;
                Cell value = this._Parameters[1].Scalar.Evaluate(Variant);
                bool OverWrite = (this._Parameters.Count == 3 ? this._Parameters[2].Scalar.Evaluate(Variant).valueBOOL : false);

                if (File.Exists(path) && !OverWrite)
                {
                    if (value.Affinity == CellAffinity.CSTRING)
                    {
                        File.AppendAllText(path, value.valueCSTRING);
                    }
                    else
                    {
                        byte[] b = value.valueBINARY;
                        using (FileStream fs = File.OpenWrite(path))
                        {
                            fs.Write(b, 0, b.Length);
                        }
                        
                    }
                }
                else
                {
                    if (value.Affinity == CellAffinity.CSTRING)
                    {
                        File.WriteAllText(path, value.valueCSTRING);
                    }
                    else
                    {
                        File.WriteAllBytes(path, value.valueBINARY);
                    }
                }

            }

        }

        public class ActionExpressionDownload : ActionExpressionParameterized
        {

            public ActionExpressionDownload(Host Host, ActionExpression Parent)
                : base(Host, Parent, DOWNLOAD, 2, "Downloads a given file")
            {
            }

            public override void Invoke(SpoolSpace Variant)
            {

                if (!this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                    throw new Exception("Expecting three scalars");

                string path = this._Parameters[0].Scalar.Evaluate(Variant).valueCSTRING;
                string uri = this._Parameters[1].Scalar.Evaluate(Variant).valueCSTRING;
                string message = (this._Parameters.Count > 2 ? this._Parameters[2].Scalar.Evaluate(Variant).valueCSTRING : null);

                // Download the data
                try
                {

                    HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
                    req.CookieContainer = new CookieContainer();
                    req.Method = "GET";
                    using (WebResponse res = req.GetResponse())
                    {

                        using (Stream s = res.GetResponseStream())
                        {

                            string html = "";
                            using (StreamReader sr = new StreamReader(s))
                            {
                                html = sr.ReadToEnd();
                            }

                            using (StreamWriter sw = new StreamWriter(path))
                            {
                                sw.Write(html);
                            }

                        }

                    }

                    // Message //
                    if (message != null)
                    {
                        this._Host.IO.WriteLine(string.Format("File.Download : Sucess : {0}", message));
                    }


                }
                catch (Exception e)
                {

                    // Message //
                    if (message != null)
                    {
                        this._Host.IO.WriteLine(string.Format("File.Download : Fail : {0}", message));
                    }

                }




            }

        }

        public class ActionExpressionZip : ActionExpressionParameterized
        {

            public ActionExpressionZip(Host Host, ActionExpression Parent)
                : base(Host, Parent, ZIP, 2, "Zips a given file or directory")
            {
            }

            public override void Invoke(SpoolSpace Variant)
            {

                if (!this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                    throw new Exception("Expecting scalars");

                string in_path = this._Parameters[0].Scalar.Evaluate(Variant).valueCSTRING;
                string out_path = this._Parameters[1].Scalar.Evaluate(Variant).valueCSTRING;

                System.IO.Compression.ZipFile.CreateFromDirectory(in_path, out_path);

            }

        }

        public class ActionExpressionUnzip : ActionExpressionParameterized
        {

            public ActionExpressionUnzip(Host Host, ActionExpression Parent)
                : base(Host, Parent, UNZIP, 2, "Unzips a given file or directory")
            {
            }

            public override void Invoke(SpoolSpace Variant)
            {

                if (!this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                    throw new Exception("Expecting scalars");

                string in_path = this._Parameters[0].Scalar.Evaluate(Variant).valueCSTRING;
                string out_path = this._Parameters[1].Scalar.Evaluate(Variant).valueCSTRING;

                System.IO.Compression.ZipFile.ExtractToDirectory(in_path, out_path);

            }

        }

        // Scalars //
        public class ScalarExpressionExists : ScalarExpressionFunction
        {

            public ScalarExpressionExists(Host Host)
                : base(Host, null, EXISTS, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return CellAffinity.BOOL;
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionExists(this._Host);
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (!this.CheckSigniture(ParameterAffinity.Scalar))
                    throw new Exception("Expecting a scalar");
                string path = this._Params[0].Scalar.Evaluate(Variants).valueCSTRING;
                if (File.Exists(path))
                    return CellValues.True;
                return CellValues.False;

            }

        }

        public class ScalarExpressionReadAllText : ScalarExpressionFunction
        {

            public ScalarExpressionReadAllText(Host Host)
                : base(Host, null, READ_ALL_TEXT, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return CellAffinity.CSTRING;
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionReadAllText(this._Host);
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (!this.CheckSigniture(ParameterAffinity.Scalar))
                    throw new Exception("Expecting a scalar");
                string path = this._Params[0].Scalar.Evaluate(Variants).valueCSTRING;
                return new Cell(File.ReadAllText(path));

            }

        }

        public class ScalarExpressionReadAllTextB : ScalarExpressionFunction
        {

            public ScalarExpressionReadAllTextB(Host Host)
                : base(Host, null, READ_ALL_TEXTB, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return CellAffinity.BSTRING;
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionReadAllTextB(this._Host);
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (!this.CheckSigniture(ParameterAffinity.Scalar))
                    throw new Exception("Expecting a scalar");
                string path = this._Params[0].Scalar.Evaluate(Variants).valueCSTRING;
                using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    return new Cell(new BString(buffer));
                }

            }

        }

        public class ScalarExpressionReadAllBytes : ScalarExpressionFunction
        {

            public ScalarExpressionReadAllBytes(Host Host)
                : base(Host, null, READ_ALL_BYTES, 1)
            {
            }

            public override CellAffinity ReturnAffinity()
            {
                return CellAffinity.BINARY;
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionReadAllBytes(this._Host);
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (!this.CheckSigniture(ParameterAffinity.Scalar))
                    throw new Exception("Expecting a scalar");
                string path = this._Params[0].Scalar.Evaluate(Variants).valueCSTRING;
                using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    return new Cell(buffer);
                }

            }

        }

        private sealed class ScalarExpressionHash : ScalarExpressionFunction
        {

            private Host _Host;
            private HashAlgorithm _Hash;

            public ScalarExpressionHash(Host Host, string Name, HashAlgorithm Hash)
                : base(Host, null, Name, 1)
            {
                this._Host = Host;
                this._Hash = Hash;
            }

            public override CellAffinity ReturnAffinity()
            {
                return CellAffinity.BINARY;
            }

            public override ScalarExpression CloneOfMe()
            {
                return new ScalarExpressionHash(this._Host, this._name, this._Hash);
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (!this.CheckSigniture(ParameterAffinity.Scalar))
                    throw new Exception("Expecting a scalar");
                string path = this._Params[0].Scalar.Evaluate(Variants).valueCSTRING;
                byte[] b = null;
                using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    b = this._Hash.ComputeHash(fs);
                }

                return new Cell(b);

            }

        }

        // Matricies //
        public sealed class MatrixExpressionReadLines : MatrixExpressionFunction
        {

            private int _Size = -1;

            public MatrixExpressionReadLines()
                : base(null, READ_LINES, 1, CellAffinity.CSTRING)
            {
            }

            public override MatrixExpression CloneOfMe()
            {
                return new MatrixExpressionReadLines();
            }

            public override int ReturnSize()
            {
                return (this._Size == -1 ? 256 : this._Size);
            }

            public override CellMatrix Evaluate(SpoolSpace Variant)
            {

                if (!this.CheckSigniture(ParameterAffinity.Scalar))
                    throw new Exception("Expecting a single scalar");

                try
                {
                    string[] x = File.ReadAllLines(this._Parameters[0].Scalar.Evaluate(Variant).valueCSTRING);
                    this._Size = x.Max((z) => { return z.Length; });
                    CellMatrix v = new CellMatrix(x.Length, 1, CellAffinity.CSTRING, this._Size);
                    for (int i = 0; i < x.Length; i++)
                    {
                        v[i, 0] = new Cell(x[i]);
                    }
                    return v;
                }
                catch
                {
                    return new CellMatrix(1, 1, CellAffinity.CSTRING, 32);
                }

            }

        }

        public sealed class MatrixExpressionReadLinesB : MatrixExpressionFunction
        {

            private int _Size = -1;

            public MatrixExpressionReadLinesB()
                : base(null, READ_LINESB, 1, CellAffinity.BSTRING)
            {
            }

            public override MatrixExpression CloneOfMe()
            {
                return new MatrixExpressionReadLinesB();
            }

            public override int ReturnSize()
            {
                return (this._Size == -1 ? 256 : this._Size);
            }

            public override CellMatrix Evaluate(SpoolSpace Variant)
            {

                if (!this.CheckSigniture(ParameterAffinity.Scalar))
                    throw new Exception("Expecting a single scalar");

                try
                {
                    string[] x = File.ReadAllLines(this._Parameters[0].Scalar.Evaluate(Variant).valueCSTRING);
                    this._Size = x.Max((z) => { return z.Length; });
                    CellMatrix v = new CellMatrix(x.Length, 1, CellAffinity.BSTRING, this._Size);
                    for (int i = 0; i < x.Length; i++)
                    {
                        v[i, 0] = new Cell(new BString(x[i]));
                    }
                    return v;
                }
                catch
                {
                    return new CellMatrix(1, 1, CellAffinity.BSTRING, 32);
                }

            }

        }

        // Records //
        public class RecordExpressionInfo : RecordExpressionFunction
        {

            public RecordExpressionInfo(Host Host)
                : base(Host, null, INFO, 1)
            {
            }

            public override Schema Columns
            {
                get
                {
                    Schema s = new Schema();
                    s.Add("PATH", CellAffinity.CSTRING, 128);
                    s.Add("DIR", CellAffinity.CSTRING, 64);
                    s.Add("FULL_NAME", CellAffinity.CSTRING, 64);
                    s.Add("NAME", CellAffinity.CSTRING, 64);
                    s.Add("EXTENSION", CellAffinity.CSTRING, 16);
                    s.Add("CREATE_DATE", CellAffinity.DATE_TIME);
                    s.Add("MODIFY_DATE", CellAffinity.DATE_TIME);
                    s.Add("ACCESS_DATE", CellAffinity.DATE_TIME);
                    s.Add("SIZE", CellAffinity.LONG);
                    s.Add("READONLY", CellAffinity.BOOL);
                    return s;
                }
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override RecordExpression CloneOfMe()
            {
                return new RecordExpressionInfo(this._Host);
            }

            public override AssociativeRecord EvaluateAssociative(SpoolSpace Variants)
            {

                if (!this.CheckSigniture(ParameterAffinity.Scalar))
                    throw new Exception("Invalid parameters; expecting a single scalar");

                string s = this._Parameters[0].Scalar.Evaluate(Variants).valueCSTRING;
                AssociativeRecord r = new AssociativeRecord(this.Columns);

                if (!File.Exists(s))
                    return r;

                FileInfo f = new FileInfo(s);

                r[0] = new Cell(s);
                r[1] = new Cell(f.DirectoryName);
                r[2] = new Cell(f.Name + "." + f.Extension);
                r[3] = new Cell(f.Name);
                r[4] = new Cell(f.Extension);
                r[5] = new Cell(f.CreationTime);
                r[6] = new Cell(f.LastWriteTime);
                r[7] = new Cell(f.LastAccessTime);
                r[8] = new Cell(f.Length);
                r[9] = new Cell(f.IsReadOnly);

                return r;

            }

        }

        // Tables //


    }



}
