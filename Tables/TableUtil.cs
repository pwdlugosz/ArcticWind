using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;


namespace ArcticWind.Tables
{
    
    public static class TableUtil
    {

        // Sort support //
        private static void Sort(Page Element, RecordMatcher Comparer)
        {

            if (Element.PageType != Page.BASE_PAGE_TYPE)
                throw new ArgumentException("Can only sort a base page");

            Element.Cache.Sort(Comparer);
            Element.Version++;

        }

        private static void Merge(Page A, Page B, RecordMatcher Comparer)
        {

            List<Record> x = new List<Record>();
            int ptrA = 0, ptrB = 0;

            while (ptrA < A.Count && ptrB < B.Count)
            {

                if (Comparer.Compare(A.Cache[ptrA], B.Cache[ptrB]) < 0)
                {
                    x.Add(A.Cache[ptrA]);
                    ptrA++;
                }
                else
                {
                    x.Add(B.Cache[ptrB]);
                    ptrB++;
                }

            }

            while (ptrA < A.Count)
            {
                x.Add(A.Cache[ptrA]);
                ptrA++;
            }

            while (ptrB < B.Count)
            {
                x.Add(B.Cache[ptrB]);
                ptrB++;
            }

            A.Cache = x.GetRange(0, A.Count);
            B.Cache = x.GetRange(A.Count, B.Count);

        
        }

        private static void SortEach(Table Element, RecordMatcher Comparer)
        {

            if (Element.Header.ClusterKey.Count != 0)
                throw new Exception("Can only sort a heap table");

            if (Element.RecordCount == 0)
                return;

            Table.PageWalker walker = new Table.PageWalker(Element);

            while (walker.CanAdvance)
            {

                Page p = walker.SelectNext();
                Sort(p, Comparer);
                Element.SetPage(p);

            }


        }

        public static void Sort(Table Element, RecordMatcher Comparer)
        {

            // Check that this is a heap table //
            if (!(Element is HeapTable))
                throw new Exception("Can only sort heap tables");
            if (Element.Header.IndexHeaders.Count != 0)
                throw new Exception("Cannot sort a table with indexes");

            // Sort each element //
            SortEach(Element, Comparer);
            
            // Merge sort //
            Table.PageWalker x = new Table.PageWalker(Element);
            Table.PageWalker y = new Table.PageWalker(Element);

            while (x.CanAdvance)
            {

                // Get x's page //
                Page A = x.SelectNext();

                // Break if we on the last page //
                if (A.NextPageID == -1) break;

                // Open a new page walker that starts at the next page //
                y.ToPage(A.NextPageID);

                // Loop through y //
                while (y.CanAdvance)
                {

                    // Get the second page //
                    Page B = y.SelectNext();

                    // Merge //
                    Merge(A, B, Comparer);

                    // Set B //
                    Element.SetPage(B);
                    

                }

                // A is final at this point //
                Element.SetPage(A);


            }


        }

        public static void Sort(Table Element, Key Columns)
        {
            Sort(Element, new RecordMatcher(Columns));
        }

        // Distinct Support //
        public static void Distinct(Table Element, RecordWriter Writer, RecordMatcher Comparer)
        {

            // Check if the table is a heap //
            if (!(Element is HeapTable))
                throw new Exception("Can only sort a HeapTable");
            if (Element.Header.IndexHeaders.Count != 0)
                throw new Exception("Can not sort a table with indexes");

            // Step 1: sort table //
            Sort(Element, Comparer);

            // Step 2: create a shell //
            //Table t = Element.Host.CreateTempTable(Element.Columns);

            // Step 3: open a reader //
            RecordReader rr = Element.OpenReader();

            // Step 4: write to the shell //
            Record lag = null;
            while (rr.CanAdvance)
            {

                Record current = rr.ReadNext();

                if (lag == null)
                    Writer.Insert(current);
                else if (Comparer.Compare(lag, current) != 0)
                    Writer.Insert(current);

                lag = current;

            }

        }

        public static void Distinct(Table Element, RecordMatcher Comparer)
        {

            // Check if the table is a heap //
            if (!(Element is HeapTable))
                throw new Exception("Can only sort a HeapTable");
            if (Element.Header.IndexHeaders.Count != 0)
                throw new Exception("Can not sort a table with indexes");

            // Step 1: create a shell //
            Table t = Element.Host.CreateTempTable(Element.Columns);

            // Step 4: write to the shell //
            using (RecordWriter Writer = t.OpenWriter())
            {
                Distinct(Element, Writer, Comparer);
            }
            
            // Step 5: drop the table //
            string dir = Element.Header.Directory, name = Element.Header.Name;
            Drop(Element.Host, Element.Key);

            // Step 6: rename the table //
            Rename(t, dir, name);

            // Step 7: reset Element's header //
            Element.Header.RecordCount = t.RecordCount;
            Element.Header.PageCount = t.PageCount;
            Element.Header.OriginPageID = t.OriginPageID;
            Element.Header.TerminalPageID = t.TerminalPageID;

        }

        public static void Distinct(Table Element, RecordWriter Writer, Key Columns)
        {
            Distinct(Element, Writer, new RecordMatcher(Columns));
        }

        public static void Distinct(Table Element, Key Columns)
        {
            Distinct(Element, new RecordMatcher(Columns));
        }

        // Drop Table //
        public static void Drop(Host Host, string Key)
        {
            Host.TableStore.DropTable(Key);
        }

        public static void Drop(Host Host, string DB, string Name)
        {
            string k = TableHeader.DeriveV1Path(Host.Connections[DB], Name);
            Drop(Host, k);
        }

        // Rename Table //
        public static void Rename(Table Element, string NewDirectory, string NewName)
        {
            Element.Host.TableStore.RenameTable(Element, NewDirectory, NewName);
        }

        // Copy Table //
        public static void Copy(Table Element, string NewDirectory, string NewName)
        {
            Element.Host.TableStore.CopyTable(Element, NewDirectory, NewName);
        }

        // Clones //
        public static Table Clone(Table Element, string NewDirectory, string NewName)
        {
            Element.Host.TableStore.DropTable(TableHeader.DeriveV1Path(NewDirectory, NewName));
            return new HeapTable(Element.Host, NewName, NewDirectory, Element.Columns, Element.PageSize);
        }

    }


}
