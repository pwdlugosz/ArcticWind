using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;


namespace ArcticWind.Tables
{


    /// <summary>
    /// Represents a Key and a PageID
    /// </summary>
    public class PageUID
    {

        public PageUID(string Key, int PageID)
        {
            this.Key = Key;
            this.PageID = PageID;
        }

        public string Key
        {
            get;
            set;
        }

        public int PageID
        {
            get;
            set;
        }

        public override string ToString()
        {
            return this.Key + " :: " + this.PageID.ToString();
        }

        public static IEqualityComparer<PageUID> DefaultComparer
        {
            get { return new PageUIDComparer(); }
        }

        private sealed class PageUIDComparer : IEqualityComparer<PageUID>
        {

            public bool Equals(PageUID A, PageUID B)
            {

                return (StringComparer.OrdinalIgnoreCase.Compare(A.Key, B.Key) == 0 && A.PageID == B.PageID);

            }

            public int GetHashCode(PageUID A)
            {
                return StringComparer.OrdinalIgnoreCase.GetHashCode(A.Key) ^ A.PageID;
            }

        }

    }


}
