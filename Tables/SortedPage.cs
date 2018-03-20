using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ArcticWind.Elements;


namespace ArcticWind.Tables
{

    /// <summary>
    /// Represents a page that stores data in a sequental, sorted, order
    /// </summary>
    public class SortedPage : Page
    {

        public const int ELEMENT_NOT_FOUND = -1;

        private IRecordMatcher _Matcher;

        public SortedPage(int PageSize, int PageID, int LastPageID, int NextPageID, int FieldCount, int UsedSpace, IRecordMatcher Matcher)
            : base(PageSize, PageID, LastPageID, NextPageID, FieldCount, UsedSpace)
        {
            this._Matcher = Matcher;
        }

        public SortedPage(Page Primitive, IRecordMatcher Matcher)
            : this(Primitive.PageSize, Primitive.PageID, Primitive.LastPageID, Primitive.NextPageID, Primitive.FieldCount, Primitive.UsedSpace, Matcher)
        {
            this._Elements = Primitive.Cache;
        }

        /// <summary>
        /// 
        /// </summary>
        public override int PageType
        {
            get { return Page.SORTED_PAGE_TYPE; }
        }

        /// <summary>
        /// Inserts a given record at it's correct position in the page
        /// </summary>
        /// <param name="Key"></param>
        public override void Insert(Record Element)
        {

            int IndexOf = this.Search(Element);
            base.Insert(Element, IndexOf);

        }

        /// <summary>
        /// Turn off inserting at a given point
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="RowID"></param>
        public override void Insert(Record Element, int RowID)
        {
            throw new InvalidDataException("Cannot insert a record at specific point in a sorted datapage");
        }

        /// <summary>
        /// Turn off sorting
        /// </summary>
        /// <param name="ClusterKey"></param>
        public override void Sort(IRecordMatcher SortKey)
        {
            throw new ArgumentException("Cannot sort a sorted page; it's already sorted and the key cannot be changed");
        }

        /// <summary>
        /// Checks if this record belongs in this domain
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool InDomain(Record Element)
        {

            if (this.Count == 0)
                return false;

            return this._Matcher.Between(Element, this.OriginRecord, this.TerminalRecord) == 0;

        }

        /// <summary>
        /// Generates a sorted data page; overwrites the base method which returns just a vanilla page
        /// </summary>
        /// <param name="PageID"></param>
        /// <param name="LastPageID"></param>
        /// <param name="NextPageID"></param>
        /// <returns></returns>
        public override Page Generate(int PageID, int LastPageID, int NextPageID)
        {
            return new SortedPage(this.PageSize, PageID, LastPageID, NextPageID, this._FieldCount, 0, this._Matcher);
        }

    }

}
