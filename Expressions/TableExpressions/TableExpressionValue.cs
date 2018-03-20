using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.Aggregates;
using ArcticWind.Tables;
using ArcticWind.Expressions;

namespace ArcticWind.Expressions.TableExpressions
{


    //public sealed class TableExpressionValue : TableExpression
    //{

    //    private Table _t;

    //    public TableExpressionValue(Host Host, TableExpression Parent, Table Value)
    //        : base(Host, Parent)
    //    {
    //        this._t = Value;
    //        this.Alias = "VALUE";
    //    }

    //    public override Schema Columns
    //    {
    //        get { return this._t.Columns; }
    //    }

    //    public override SpoolSpace CreateResolver(SpoolSpace Variants)
    //    {
    //        return Variants;
    //    }

    //    // Evaluates //
    //    public override void Evaluate(SpoolSpace Variants, RecordWriter Writer)
    //    {
    //        Writer.Consume(this._t.OpenReader());
    //    }

    //    public override Table Select(SpoolSpace Variants)
    //    {
    //        return this._t;
    //    }

    //    public override void Recycle()
    //    {
    //        // do nothing
    //    }

    //    public override long EstimatedCount
    //    {
    //        get
    //        {
    //            return this._t.RecordCount;
    //        }
    //    }

    //    public override bool IsIndexedBy(Key IndexColumns)
    //    {
    //        return this._t.IsIndexedBy(IndexColumns);
    //    }

    //}

    public sealed class TableExpressionStoreRef : TableExpression
    {

        public TableExpressionStoreRef(Host Host, TableExpression Parent, string LibName, string TableName)
            : base(Host, Parent)
        {
            this._DB = LibName;
            this._Name = TableName;
            this.Alias = TableName;
        }

        public override Schema Columns
        {
            get 
            {
                return this._Host.Spools[this._DB].GetTable2(this._Name).Columns;
                //return this._Host.OpenTable(this._DB, this._Name).Columns; 
            }
        }

        public override SpoolSpace CreateResolver(SpoolSpace Variants)
        {
            return Variants;
        }

        // Evaluates //
        public override void Evaluate(SpoolSpace Variants, RecordWriter Writer)
        {
            Table t = this._Host.Spools[this._DB].GetTable2(this._Name);
            Writer.Consume(t.OpenReader());
        }

        public override Table Select(SpoolSpace Variants)
        {
            return this._Host.Spools[this._DB].GetTable2(this._Name);
        }

        public override void Recycle()
        {
            // do nothing
        }

        public override long EstimatedCount
        {
            get
            {
                Table t = this._Host.Spools[this._DB].GetTable2(this._Name);
                return t.RecordCount;
            }
        }

        public override bool IsIndexedBy(Key IndexColumns)
        {
            Table t = this._Host.Spools[this._DB].GetTable2(this._Name);
            return t.IsIndexedBy(IndexColumns);
        }

    }

    public sealed class TableExpressionCTOR : TableExpression
    {

        private Schema _Columns;
        private Key _Cluster;

        public TableExpressionCTOR(Host Host, TableExpression Parent, Schema Columns, Key Cluster)
            : base(Host, Parent)
        {
            this._Columns = Columns;
            this._Cluster = Cluster;
        }

        public override Schema Columns
        {
            get { return this._Columns; }
        }

        public override SpoolSpace CreateResolver(SpoolSpace Variants)
        {
            return Variants;
        }

        // Evaluates //
        public override void Evaluate(SpoolSpace Variants, RecordWriter Writer)
        {
            // do nothing
        }

        public override Table Select(SpoolSpace Variants)
        {

            if (this._Cluster.Count == 0)
            {
                return this._Host.CreateTable(this._Host.TempDB, Host.RandomName, this._Columns);
                //return new HeapTable(this._Host, name, dir, Columns, Page.DEFAULT_SIZE);
            }
            else
            {
                return this._Host.CreateTable(this._Host.TempDB, Host.RandomName, this._Columns, this._Cluster);
                //return new ClusteredTable(this._Host, name, dir, this._Columns, this._Cluster, ClusterState.Unique, Page.DEFAULT_SIZE);
            }

        }

        public override void Recycle()
        {
            // do nothing
        }

        public override long EstimatedCount
        {
            get
            {
                return 0;
            }
        }

        public override bool IsIndexedBy(Key IndexColumns)
        {
            return Key.LeftSubsetStrong(this._Cluster, IndexColumns);
        }

    }

}
