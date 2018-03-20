using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using ArcticWind.Tables;

namespace ArcticWind.Elements
{
    
    public static class DebugUtil
    {

        /// <summary>
        /// Checks if a table is sorted
        /// </summary>
        /// <param name="T">The table to check</param>
        /// <param name="Columns">The desied sort order</param>
        /// <returns>the first instance that a record is out of order; if the table is sorted, returns a record not found key </returns>
        public static RecordKey CheckSort(Table T, Key Columns)
        {

            if (T.RecordCount < 2)
                return RecordKey.RecordNotFound;

            RecordReader rs = T.OpenReader();
            RecordMatcher matcher = new RecordMatcher(Columns);
            Record First = rs.ReadNext();
            
            while (rs.CanAdvance)
            {

                // We want First <= Second
                Record Second = rs.ReadNext();
                int check = matcher.Compare(First, Second);
                if (check > 0) return rs.PositionKey;

            }

            return RecordKey.RecordNotFound;
        }

        /// <summary>
        /// Dumps a table to a flat file
        /// </summary>
        /// <param name="T"></param>
        /// <param name="FilePath"></param>
        public static void DumpTable(Table T, string FilePath)
        {

            using (StreamWriter sw = new StreamWriter(FilePath))
            {
                sw.WriteLine(T.Columns.ToNameString('\t'));
                RecordReader rs = T.OpenReader();
                while (rs.CanAdvance)
                {
                    sw.WriteLine(rs.ReadNext());
                }
            }

        }

        /// <summary>
        /// Dumps a cluster to a flat file; only the branch nodes are dumped to disk; only meta data for leaf is dumped to disk
        /// </summary>
        /// <param name="C"></param>
        /// <param name="FilePath"></param>
        public static void DumpRecordTree(Cluster C, string FilePath)
        {

            

            using (StreamWriter sw = new StreamWriter(FilePath))
            {

                if (C.Root.IsLeaf)
                {
                    sw.WriteLine("ROOT == LEAF");
                }
                else
                {

                }

            }

        }

        /// <summary>
        /// Not intended to called by the user; dumpes a cluster page to disk
        /// </summary>
        /// <param name="b"></param>
        /// <param name="ParentPage"></param>
        /// <param name="s"></param>
        private static void DumpClusterPage(ClusterPage c, Cluster z, StreamWriter s)
        {
            

            if (c.IsLeaf)
            {
                s.WriteLine("LeafPage: ID={0}; LastID={1}; NextID={2}; Parent={3}; Count={4}", c.PageID, c.LastPageID, c.NextPageID, c.ParentPageID, c.Count);
            }
            else
            {

                s.WriteLine("NodePage: ID={0}; LastID={1}; NextID={2}; Parent={3}; Count={4}", c.PageID, c.LastPageID, c.NextPageID, c.ParentPageID, c.Count);
                s.WriteLine("--------------------------------------------------------------------------");
                for (int i = 0; i < c.Count; i++)
                {
                    s.WriteLine(c.Select(i));
                }

                for (int i = 0; i < c.Count; i++)
                {
                    int PageID = c.GetPageID(i);
                    ClusterPage p = ClusterPage.Mutate(z.Storage.GetPage(PageID), z.IndexColumns);
                    DumpClusterPage(p, z, s);
                }


            }
            s.WriteLine("--------------------------------------------------------------------------");


            
        }

        /// <summary>
        /// Computes the MD5 hash of a table
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static byte[] TableHash(Table T)
        {

            // Need to first flush the table to disk, otherwise parts of the table may not be in memory
            T.Host.TableStore.FlushTable(T.Header.Key);

            // Build the MD5 hash
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] h;

            // Consume the stream
            using (Stream s = File.Open(T.Header.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                h = x.ComputeHash(s);
            }

            return h;

        }

        /// <summary>
        /// Returns the table hash as a string
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static string TableHashString(Table T)
        {
            return BitConverter.ToString(TableHash(T)).Replace("-","");
        }


    }

}
