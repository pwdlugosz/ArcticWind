using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Tables;
using ArcticWind.Expressions;


namespace ArcticWind.Elements.Views
{
    

    //public class ViewedRecordReader : RecordReader
    //{

    //    protected Table _Storage;
    //    protected ScalarExpressionSet _Select;
    //    protected Filter _Where;
    //    protected FieldResolver _Resolver;
    //    protected Page _CurrentPage;
    //    protected int _CurrentPageID = -1;
    //    protected int _CurrentRecordIndex = -1;
    //    protected RecordKey _Lower;
    //    protected RecordKey _Upper;
    //    protected int _Ticks = 0;

    //    public ViewedRecordReader(Table Data, ScalarExpressionSet Select, Filter Where, FieldResolver Resolver, RecordKey LKey, RecordKey UKey)
    //        : base()
    //    {

    //        this._Lower = LKey;
    //        this._Upper = UKey;
    //        this._Storage = Data;
    //        this._Select = Select;
    //        this._Where = Where;
    //        if (this._Storage.PageCount == 0)
    //        {
    //            this._CurrentPage = null;
    //            this._CurrentPageID = -1;
    //            this._CurrentRecordIndex = -1;
    //        }
    //        else
    //        {
    //            this._CurrentPage = this._Storage.GetPage(this._Lower.PAGE_ID);
    //            this._CurrentPageID = this._CurrentPage.PageID;
    //            this._CurrentRecordIndex = this._Lower.ROW_ID;
    //        }

    //    }

    //    public ViewedRecordReader(Table Data, ScalarExpressionSet Select, Filter Where, FieldResolver Resolver)
    //        : this(Data, Select, Where, Resolver, RecordReaderBase.OriginKey(Data, Data.Header), RecordReaderBase.TerminalKey(Data, Data.Header))
    //    {
    //    }

    //    public override bool IsFirst
    //    {
    //        get { return this._CurrentRecordIndex == this._Lower.ROW_ID && this._CurrentPageID <= this._Lower.PAGE_ID; }
    //    }

    //    public override bool IsLast
    //    {
    //        get { return this._Upper.ROW_ID == this._CurrentRecordIndex && this._Upper.PAGE_ID <= this._CurrentPageID; }
    //    }

    //    public override bool CanAdvance
    //    {

    //        get
    //        {

    //            if (this._Lower.IsNotFound || this._Upper.IsNotFound)
    //                return false;

    //            if (this._CurrentPage == null)
    //                return false;
    //            else if (this._CurrentPageID == -1)
    //                return false;
    //            return !(this._CurrentPageID == this._Upper.PAGE_ID && this._CurrentRecordIndex > this._Upper.ROW_ID);

    //        }

    //    }

    //    public override bool CanRevert
    //    {
    //        get
    //        {
    //            if (this._CurrentPage == null)
    //                return false;
    //            else if (this._CurrentPageID == -1)
    //                return false;
    //            return !(this._CurrentPageID == this._Lower.PAGE_ID && this._CurrentRecordIndex < this._Lower.ROW_ID);
    //        }
    //    }

    //    public override Schema Columns
    //    {
    //        get { return this._Select.Columns; }
    //    }

    //    // Advance //
    //    public void UnfilteredAdvance()
    //    {

    //        this._Ticks++;

    //        this._CurrentRecordIndex++;

    //        if (this._CurrentRecordIndex >= this._CurrentPage.Count)
    //        {

    //            if (this._CurrentPageID == this._Upper.PAGE_ID)
    //                return;

    //            this._CurrentRecordIndex = 0;
    //            this._CurrentPageID = this._CurrentPage.NextPageID;

    //            if (this._CurrentPageID != -1)
    //                this._CurrentPage = this._Storage.GetPage(this._CurrentPageID);

    //        }

    //        if (!this.CanAdvance)
    //            return;

    //        Record x = this._CurrentPage.Select(this._CurrentRecordIndex);
    //        this._Resolver.SetValue(0, x);

    //    }

    //    public void FilteredAdvance()
    //    {

    //        // Break if end of stream //
    //        if (!this.CanAdvance)
    //            return;
    //        // While the filter is false, advance, but advance at lease once //
    //        do
    //            this.UnfilteredAdvance();
    //        while (!this.CheckFilter() && this.CanAdvance);

    //    }

    //    public override void Advance()
    //    {

    //        if (this._Where == null)
    //            this.UnfilteredAdvance();
    //        else
    //            this.FilteredAdvance();

    //    }

    //    public override void Advance(int Itterations)
    //    {
    //        for (int i = 0; i < Itterations; i++)
    //            this.Advance();
    //    }

    //    // Revert //
    //    public void UnfilteredRevert()
    //    {

    //        this._CurrentRecordIndex--;
    //        if (this._CurrentRecordIndex < 0)
    //        {

    //            this._CurrentPageID = this._CurrentPage.LastPageID;
    //            if (this._CurrentPageID != -1)
    //            {
    //                this._CurrentPage = this._Storage.GetPage(this._CurrentPageID);
    //                this._CurrentRecordIndex = this._CurrentPage.Count - 1;
    //            }

    //        }

    //        this._Ticks--;

    //        if (!this.CanRevert)
    //            return;

    //        Record x = this._CurrentPage.Select(this._CurrentRecordIndex);
    //        this._Resolver.SetValue(0, x);

    //    }

    //    public void FilteredRevert()
    //    {

    //        // Break if end of stream //
    //        if (!this.CanRevert)
    //            return;
    //        // While the filter is false, revert, but revert at lease once //
    //        do
    //            this.UnfilteredRevert();
    //        while (!this.CheckFilter() && this.CanRevert);

    //    }

    //    public override void Revert()
    //    {
    //        if (this._Where == null)
    //            this.UnfilteredRevert();
    //        else
    //            this.FilteredRevert();
    //    }

    //    public override void Revert(int Itterations)
    //    {
    //        for (int i = 0; i < Itterations; i++)
    //            this.Revert();
    //    }

    //    // Read //
    //    public bool CheckFilter()
    //    {
    //        if (!this.CanAdvance) 
    //            return false;
    //        if (this._Where == null)
    //            return true;
    //        return this._Where.Evaluate(this._Resolver);
    //    }

    //    public override Record Read()
    //    {
    //        return this._Select.Evaluate(this._Resolver);
    //    }

    //    public override Record ReadNext()
    //    {
    //        Record r = this.Read();
    //        this.Advance();
    //        return r;
    //    }

    //    public override int PageID()
    //    {
    //        return this._CurrentPage.PageID;
    //    }

    //    public override int RecordID()
    //    {
    //        return this._CurrentRecordIndex;
    //    }

    //    public override long Position()
    //    {
    //        return this._Ticks;
    //    }

    //    public override void Reset()
    //    {

    //        this._Ticks = 0;
    //        if (this._Storage.PageCount == 0)
    //        {
    //            this._CurrentPage = null;
    //            this._CurrentPageID = -1;
    //            this._CurrentRecordIndex = -1;
    //        }
    //        else
    //        {
    //            this._CurrentPage = this._Storage.GetPage(this._Lower.PAGE_ID);
    //            this._CurrentPageID = this._CurrentPage.PageID;
    //            this._CurrentRecordIndex = this._Lower.ROW_ID;
    //        }

    //    }



    //}


}
