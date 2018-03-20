using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ArcticWind.Elements;
using ArcticWind;

namespace ArcticWind.Tables
{

    /// <summary>
    /// 
    /// </summary>
    public static class TableImporter
    {

        private static char[] _Integrals = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private static char[] _Numerics = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.', ',', '$' };
        
        public static void Import(RecordWriter Writer, string RawPath, char[] Delims, char Escape, int Skip)
        {

            // Open the stream //
            using (StreamReader sr = new StreamReader(RawPath))
            {

                // Handle skips //
                for (int i = 0; i < Skip && !sr.EndOfStream; i++)
                {
                    string s = sr.ReadLine();
                }

                // Loop through the table //
                while (!sr.EndOfStream)
                {

                    string s = sr.ReadLine();
                    Record r = Util.StringUtil.ToRecord(s, Writer.Columns, Delims, Escape);
                    Writer.Insert(r);

                }


            }

        }

        public static void ImportLines(Host Host, string Dir, string Name, string RawPath, int Skip, int LineSize)
        {

            // Create the table //
            Schema q = new Schema();
            q.Add("ROW_ID", CellAffinity.LONG);
            q.Add("RAW_TEXT", CellAffinity.CSTRING, LineSize);
            Table t = Host.CreateTable(Dir, Name, q);

            // Read in the lines //
            using (RecordWriter rw = t.OpenWriter())
            {
                Import(rw, RawPath, new char[] { }, char.MaxValue, Skip);
            }


        }

        public static void Export(RecordReader Reader, string RawPath, bool Headers, char Delim, char Escape)
        {

            using (FileStream fs = File.Create(RawPath))
            {
                
                using (StreamWriter sw = new StreamWriter(fs))
                {

                    if (Headers)
                        sw.WriteLine(Util.StringUtil.ToString(Reader.Columns, Delim, Escape));

                    while (Reader.CanAdvance)
                    {

                        string s = Util.StringUtil.ToString(Reader.ReadNext(), Delim, Escape);
                        sw.WriteLine(s);

                    }

                }

            }

        }



    //    public static CellAffinity GuessBest()
    //    {
    //    }

    //    public static bool IsIntegral(string Value)
    //    {

    //    }

    //    public static bool IsNumeric(string Value)
    //    {
    //    }

    //    public static bool IsDate(string Value)
    //    {
    //    }

    //    public static bool IsNull(string Value)
    //    {
    //        return (Value == null || Value.Trim() == "");
    //    }

    //    public static bool IsHex(string Value)
    //    {
    //    }

    //    public static bool IsUTF16(string Value)
    //    {
    //    }





    }


}
