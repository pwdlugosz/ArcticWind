using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.ActionExpressions;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Tables;
using ArcticWind.Expressions;


namespace ArcticWind.Libraries
{
    
    /// <summary>
    /// A collection of methods and functions supporting tables
    /// </summary>
    //public sealed class TableLibrary : Library
    //{

    //    public const string A_RENAME = "RENAME";
    //    public const string A_IMPORT = "IMPORT";
    //    public const string A_EXPORT = "EXPORT";
    //    public const string A_DROP = "DROP";
        
    //    public const string S_PAGE_COUNT = "PAGE_COUNT";
    //    public const string S_ROW_COUNT = "ROW_COUNT";
    //    public const string S_FIRST_PAGE = "FIRST_PAGE";
    //    public const string S_LAST_PAGE = "LAST_PAGE";
    //    public const string S_ROOT_PAGE = "ROOT_PAGE";
    //    public const string S_PATH = "PATH";
    //    public const string S_DIR = "DIR";
    //    public const string S_META_DATA = "META_DATA";
    //    public const string S_PAGE = "PAGE";
    //    public const string S_EXISTS = "EXISTS";
    //    public const string S_INFO = "INFO";
        
    //    public TableLibrary(Host Host)
    //        :base(Host, "TABLE")
    //    {
    //        this._Host = Host;
    //    }

    //    public override ActionExpressionParameterized ActionLookup(string Name)
    //    {

    //        switch (Name.ToUpper())
    //        {

    //            case A_RENAME:
    //                return new ActionExpressionRename(this._Host, null);
    //            case A_IMPORT:
    //                return new ActionExpressionImport(this._Host, null);
    //            case A_EXPORT:
    //                return new ActionExpressionExport(this._Host, null);
    //            case A_DROP:
    //                return new ActionExpressionDrop(this._Host, null);

    //        }

    //        return null;

    //    }

    //    public override bool ActionExists(string Name)
    //    {
    //        return this.ActionLookup(Name) != null;
    //    }

    //    public override ScalarExpressionFunction ScalarFunctionLookup(string Name)
    //    {

    //        switch (Name.ToUpper())
    //        {

    //            case S_PAGE_COUNT:
    //                return new ScalarExpressionPageCount(this._Host);
    //            case S_ROW_COUNT:
    //                return new ScalarExpressionRowCount(this._Host);
    //            case S_FIRST_PAGE:
    //                return new ScalarExpressionFirstPage(this._Host);
    //            case S_LAST_PAGE:
    //                return new ScalarExpressionLastPage(this._Host);

    //            case S_ROOT_PAGE:
    //                return new ScalarExpressionRootPage(this._Host);
    //            case S_PATH:
    //                return new ScalarExpressionPath(this._Host);
    //            case S_DIR:
    //                return new ScalarExpressionDir(this._Host);
    //            case S_META_DATA:
    //                return new ScalarExpressionMetaData(this._Host);

    //            case S_PAGE:
    //                return new ScalarExpressionPage(this._Host);
    //            case S_EXISTS:
    //                return new ScalarExpressionExists(this._Host);
    //            case S_INFO:
    //                return new ScalarExpressionInfo(this._Host);


    //        }

    //        return null;

    //    }

    //    public override bool ScalarFunctionExists(string Name)
    //    {
    //        return this.ScalarFunctionLookup(Name) != null;
    //    }

    //    // Actions //
    //    private sealed class ActionExpressionRename : ActionExpressionParameterized
    //    {

    //        public ActionExpressionRename(Host Host, ActionExpression Parent)
    //            : base(Host, Parent, A_RENAME, "OLD_DB.S.R;OLD_NAME.S.R;NEW_DB.S.R;NEW_NAME.S.R", "Renames a given table, possibly pointing to a new library")
    //        {
    //        }

    //        public override void Invoke(FieldResolver Variant)
    //        {
                
    //            this.CheckRequired();

    //            string old_db = this._Parameters[0].Scalar.Evaluate(Variant).valueCSTRING;
    //            string old_name = this._Parameters[1].Scalar.Evaluate(Variant).valueCSTRING;
    //            string new_db = this._Parameters[2].Scalar.Evaluate(Variant).valueCSTRING;
    //            string new_name = this._Parameters[3].Scalar.Evaluate(Variant).valueCSTRING;

    //            string path = TableHeader.DeriveV1Path(this._Host.Connections[old_db], old_name);
    //            TableUtil.Rename(this._Host.OpenTable(path), new_db, new_name);

    //        }

    //    }

    //    private sealed class ActionExpressionImport : ActionExpressionParameterized
    //    {

    //        public ActionExpressionImport(Host Host, ActionExpression Parent)
    //            : base(Host, Parent, A_IMPORT, "TABLE.T.R;PATH.S.R;DELIM.S.O;ESCAPE.S.O;SKIP.S.O", "Imports a delimited text file into a table")
    //        {
    //        }

    //        public override void Invoke(FieldResolver Variant)
    //        {

    //            this.CheckRequired();

    //            Table t = this._Parameters[0].Table.Select(Variant);
    //            Cell Path = this._Parameters[1].Scalar.Evaluate(Variant);
    //            Cell Delim = this._Parameters[2].Scalar.Evaluate(Variant);
    //            Cell Escape = this._Parameters[3].Scalar.Evaluate(Variant);
    //            Cell Skip = this._Parameters[4].Scalar.Evaluate(Variant);

    //            char[] del = (Delim.IsNull ? (Path.valueCSTRING.Split('.').Last().ToUpper() == "CSV" ? new char[] {','} : new char[]{'\t'}) : Delim.valueCSTRING.ToCharArray());
    //            char esc = (Escape.IsNull ? char.MaxValue : Escape.valueCSTRING.ToCharArray().First());
    //            int sk = (Skip.IsNull ? 0 : (int)Skip.valueLONG);

    //            using (System.IO.StreamReader sr = new System.IO.StreamReader(Path.valueCSTRING))
    //            {

    //                int i = 0;
    //                while (i < sk)
    //                {
    //                    string v = sr.ReadLine();
    //                    i++;
    //                }

    //                using (RecordWriter rw = t.OpenWriter())
    //                {

    //                    while (!sr.EndOfStream)
    //                    {

    //                        string s = sr.ReadLine();
    //                        Record r = Util.StringUtil.ToRecord(s, t.Columns, del, esc);
    //                        rw.Insert(r);

    //                    }

    //                }

    //            }

    //        }

    //    }

    //    private sealed class ActionExpressionExport : ActionExpressionParameterized
    //    {

    //        public ActionExpressionExport(Host Host, ActionExpression Parent)
    //            : base(Host, Parent, A_EXPORT, "TABLE.T.R;PATH.S.R;DELIM.S.O;ESCAPE.S.O", "Exports a table to a text file")
    //        {
    //        }

    //        public override void Invoke(FieldResolver Variant)
    //        {

    //            this.CheckRequired();

    //            Table t = this._Parameters[0].Table.Select(Variant);
    //            Cell Path = this._Parameters[1].Scalar.Evaluate(Variant);
    //            Cell Delim = (this._Parameters[2].Scalar == null ? CellValues.NullCSTRING : this._Parameters[2].Scalar.Evaluate(Variant));
    //            Cell Escape = (this._Parameters[3].Scalar == null ? CellValues.NullCSTRING : this._Parameters[3].Scalar.Evaluate(Variant));
                
    //            string del = (Delim.IsNull ? (Path.valueCSTRING.Split('.').Last().ToUpper() == "CSV" ? "," : "\t" ) : Delim.valueCSTRING);
    //            string esc = (Escape.IsNull ? "" : Escape.valueCSTRING);

    //            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Path.valueCSTRING))
    //            {
    //                sw.WriteLine(Util.StringUtil.ToString(t.Columns, del, esc));
    //                RecordReader rr = t.OpenReader();
    //                while (rr.CanAdvance)
    //                {
    //                    sw.WriteLine(Util.StringUtil.ToString(rr.ReadNext(), del, esc));
    //                }
    //            }

    //        }

    //    }

    //    private sealed class ActionExpressionDrop : ActionExpressionParameterized
    //    {

    //        public ActionExpressionDrop(Host Host, ActionExpression Parent)
    //            : base(Host, Parent, A_DROP, "LIB.S.R;NAME.S.R", "Exports a table to a text file")
    //        {
    //        }

    //        public override void Invoke(FieldResolver Variant)
    //        {

    //            this.CheckRequired();

    //            string lib = this._Parameters[0].Scalar.Evaluate(Variant).valueCSTRING;
    //            string name = this._Parameters[1].Scalar.Evaluate(Variant).valueCSTRING;
    //            string dir = this._Host.Connections[lib];
    //            string path = TableHeader.DeriveV1Path(dir, name);

    //            this._Host.TableStore.DropTable(path);

    //        }

    //    }

    //    // Scalars //
    //    private sealed class ScalarExpressionPageCount : ScalarExpressionFunction
    //    {

    //        private Host _Host;
            
    //        public ScalarExpressionPageCount(Host Host)
    //            : base(null, S_PAGE_COUNT, 1)
    //        {
    //            this._Host = Host;
    //        }

    //        public override CellAffinity ReturnAffinity()
    //        {
    //            return CellAffinity.LONG;
    //        }

    //        public override ScalarExpression CloneOfMe()
    //        {
    //            return new ScalarExpressionPageCount(this._Host);
    //        }

    //        public override Cell Evaluate(FieldResolver Variants)
    //        {
    //            string value = this._ChildNodes[0].Evaluate(Variants).valueCSTRING;
    //            Table t = this._Host.OpenTable(value.Split('.')[0].Trim(), value.Split('.')[1].Trim());
    //            return new Cell(t.PageCount);
    //        }

    //    }

    //    private sealed class ScalarExpressionRowCount : ScalarExpressionFunction
    //    {

    //        private Host _Host;

    //        public ScalarExpressionRowCount(Host Host)
    //            : base(null, S_ROW_COUNT, 1)
    //        {
    //            this._Host = Host;
    //        }

    //        public override CellAffinity ReturnAffinity()
    //        {
    //            return CellAffinity.LONG;
    //        }

    //        public override ScalarExpression CloneOfMe()
    //        {
    //            return new ScalarExpressionRowCount(this._Host);
    //        }

    //        public override Cell Evaluate(FieldResolver Variants)
    //        {
    //            string value = this._ChildNodes[0].Evaluate(Variants).valueCSTRING;
    //            Table t = this._Host.OpenTable(value.Split('.')[0].Trim(), value.Split('.')[1].Trim());
    //            return new Cell(t.RecordCount);
    //        }

    //    }

    //    private sealed class ScalarExpressionFirstPage : ScalarExpressionFunction
    //    {

    //        private Host _Host;

    //        public ScalarExpressionFirstPage(Host Host)
    //            : base(null, S_FIRST_PAGE, 1)
    //        {
    //            this._Host = Host;
    //        }

    //        public override CellAffinity ReturnAffinity()
    //        {
    //            return CellAffinity.LONG;
    //        }

    //        public override ScalarExpression CloneOfMe()
    //        {
    //            return new ScalarExpressionFirstPage(this._Host);
    //        }

    //        public override Cell Evaluate(FieldResolver Variants)
    //        {
    //            string value = this._ChildNodes[0].Evaluate(Variants).valueCSTRING;
    //            Table t = this._Host.OpenTable(value.Split('.')[0].Trim(), value.Split('.')[1].Trim());
    //            return new Cell(t.OriginPageID);
    //        }

    //    }

    //    private sealed class ScalarExpressionLastPage : ScalarExpressionFunction
    //    {

    //        private Host _Host;

    //        public ScalarExpressionLastPage(Host Host)
    //            : base(null, S_LAST_PAGE, 1)
    //        {
    //            this._Host = Host;
    //        }

    //        public override CellAffinity ReturnAffinity()
    //        {
    //            return CellAffinity.LONG;
    //        }

    //        public override ScalarExpression CloneOfMe()
    //        {
    //            return new ScalarExpressionLastPage(this._Host);
    //        }

    //        public override Cell Evaluate(FieldResolver Variants)
    //        {
    //            string value = this._ChildNodes[0].Evaluate(Variants).valueCSTRING;
    //            Table t = this._Host.OpenTable(value.Split('.')[0].Trim(), value.Split('.')[1].Trim());
    //            return new Cell(t.TerminalPageID);
    //        }

    //    }

    //    private sealed class ScalarExpressionRootPage : ScalarExpressionFunction
    //    {

    //        private Host _Host;

    //        public ScalarExpressionRootPage(Host Host)
    //            : base(null, S_ROOT_PAGE, 1)
    //        {
    //            this._Host = Host;
    //        }

    //        public override CellAffinity ReturnAffinity()
    //        {
    //            return CellAffinity.LONG;
    //        }

    //        public override ScalarExpression CloneOfMe()
    //        {
    //            return new ScalarExpressionRootPage(this._Host);
    //        }

    //        public override Cell Evaluate(FieldResolver Variants)
    //        {
    //            string value = this._ChildNodes[0].Evaluate(Variants).valueCSTRING;
    //            Table t = this._Host.OpenTable(value.Split('.')[0].Trim(), value.Split('.')[1].Trim());
    //            return new Cell(t.Header.RootPageID);
    //        }

    //    }

    //    private sealed class ScalarExpressionPath : ScalarExpressionFunction
    //    {

    //        private Host _Host;

    //        public ScalarExpressionPath(Host Host)
    //            : base(null, S_PATH, 1)
    //        {
    //            this._Host = Host;
    //        }

    //        public override CellAffinity ReturnAffinity()
    //        {
    //            return CellAffinity.CSTRING;
    //        }

    //        public override ScalarExpression CloneOfMe()
    //        {
    //            return new ScalarExpressionPath(this._Host);
    //        }

    //        public override Cell Evaluate(FieldResolver Variants)
    //        {
    //            string value = this._ChildNodes[0].Evaluate(Variants).valueCSTRING;
    //            Table t = this._Host.OpenTable(value.Split('.')[0].Trim(), value.Split('.')[1].Trim());
    //            return new Cell(t.Header.Path);
    //        }

    //    }

    //    private sealed class ScalarExpressionDir : ScalarExpressionFunction
    //    {

    //        private Host _Host;

    //        public ScalarExpressionDir(Host Host)
    //            : base(null, S_DIR, 1)
    //        {
    //            this._Host = Host;
    //        }

    //        public override CellAffinity ReturnAffinity()
    //        {
    //            return CellAffinity.CSTRING;
    //        }

    //        public override ScalarExpression CloneOfMe()
    //        {
    //            return new ScalarExpressionDir(this._Host);
    //        }

    //        public override Cell Evaluate(FieldResolver Variants)
    //        {
    //            string value = this._ChildNodes[0].Evaluate(Variants).valueCSTRING;
    //            Table t = this._Host.OpenTable(value.Split('.')[0].Trim(), value.Split('.')[1].Trim());
    //            return new Cell(t.Header.Directory);
    //        }

    //    }

    //    private sealed class ScalarExpressionMetaData : ScalarExpressionFunction
    //    {

    //        private Host _Host;

    //        public ScalarExpressionMetaData(Host Host)
    //            : base(null, S_META_DATA, 1)
    //        {
    //            this._Host = Host;
    //        }

    //        public override CellAffinity ReturnAffinity()
    //        {
    //            return CellAffinity.CSTRING;
    //        }

    //        public override ScalarExpression CloneOfMe()
    //        {
    //            return new ScalarExpressionMetaData(this._Host);
    //        }

    //        public override Cell Evaluate(FieldResolver Variants)
    //        {
    //            string value = this._ChildNodes[0].Evaluate(Variants).valueCSTRING;
    //            Table t = this._Host.OpenTable(value.Split('.')[0].Trim(), value.Split('.')[1].Trim());
    //            return new Cell(t.MetaData());
    //        }

    //    }

    //    private sealed class ScalarExpressionPage : ScalarExpressionFunction
    //    {

    //        private Host _Host;

    //        public ScalarExpressionPage(Host Host)
    //            : base(null, S_PAGE, 2)
    //        {
    //            this._Host = Host;
    //        }

    //        public override CellAffinity ReturnAffinity()
    //        {
    //            return CellAffinity.BINARY;
    //        }

    //        public override ScalarExpression CloneOfMe()
    //        {
    //            return new ScalarExpressionPage(this._Host);
    //        }

    //        public override Cell Evaluate(FieldResolver Variants)
    //        {
    //            string value = this._ChildNodes[0].Evaluate(Variants).valueCSTRING;
    //            int id = (int)this._ChildNodes[1].Evaluate(Variants).valueLONG;
    //            Table t = this._Host.OpenTable(value.Split('.')[0].Trim(), value.Split('.')[1].Trim());
    //            byte[] b = new byte[t.PageSize];
    //            Page.Write(b, 0, t.GetPage(id));
    //            return new Cell(b);
    //        }

    //    }

    //    private sealed class ScalarExpressionExists : ScalarExpressionFunction
    //    {

    //        private Host _Host;

    //        public ScalarExpressionExists(Host Host)
    //            : base(null, S_EXISTS, 1)
    //        {
    //            this._Host = Host;
    //        }

    //        public override CellAffinity ReturnAffinity()
    //        {
    //            return CellAffinity.BOOL;
    //        }

    //        public override ScalarExpression CloneOfMe()
    //        {
    //            return new ScalarExpressionExists(this._Host);
    //        }

    //        public override Cell Evaluate(FieldResolver Variants)
    //        {
    //            string value = this._ChildNodes[0].Evaluate(Variants).valueCSTRING;
    //            string db = value.Split('.')[0].Trim();
    //            string name = value.Split('.')[0].Trim();
    //            if (!this._Host.Connections.Exists(db))
    //                return CellValues.False;
    //            if (!this._Host.TableExists(db, name))
    //                return CellValues.False;
    //            return CellValues.True;
    //        }

    //    }

    //    private sealed class ScalarExpressionInfo : ScalarExpressionFunction
    //    {

    //        private Host _Host;

    //        public ScalarExpressionInfo(Host Host)
    //            : base(null, S_INFO, 1)
    //        {
    //            this._Host = Host;
    //        }

    //        public override CellAffinity ReturnAffinity()
    //        {
    //            return CellAffinity.BSTRING;
    //        }

    //        public override ScalarExpression CloneOfMe()
    //        {
    //            return new ScalarExpressionInfo(this._Host);
    //        }

    //        public override Cell Evaluate(FieldResolver Variants)
    //        {
    //            string value = this._ChildNodes[0].Evaluate(Variants).valueCSTRING;
    //            Table t = this._Host.OpenTable(value.Split('.')[0].Trim(), value.Split('.')[1].Trim());
    //            return new Cell(t.Header.DebugPrint(), true);
    //        }

    //    }


    //}

}
