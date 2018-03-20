using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Tables;
using ArcticWind.Expressions;

namespace ArcticWind.Util
{
    
    
    public static class StringUtil
    {

        // Splitter //
        public static string[] Split(string Text, char[] Delim, char Escape, bool KeepDelims)
        {

            if (Delim.Contains(Escape))
                throw new Exception("The deliminators cannot contain the escape token");

            List<string> TempArray = new List<string>();
            StringBuilder sb = new StringBuilder();
            bool InEscape = false;

            // Go through each char in string //
            foreach (char c in Text)
            {

                // turn on escaping //
                if (c == Escape)
                    InEscape = (!InEscape);

                // Slipt //
                if (!InEscape)
                {

                    // We found a deliminator //
                    if (Delim.Contains(c))
                    {

                        string s = sb.ToString();

                        // Check the size of the current cache and add the string, which would happend if we had 'A,B,,C,D' //
                        if (s.Length == 0)
                            TempArray.Add(null);
                        else
                            TempArray.Add(s);

                        // Check to see if we need to keep our delims //
                        if (KeepDelims)
                            TempArray.Add(c.ToString());

                        sb = new StringBuilder();

                    }
                    else if (c != Escape)
                    {
                        sb.Append(c);
                    }

                }// end the string building phase //
                else if (c != Escape)
                {
                    sb.Append(c);
                }

            }

            if (InEscape)
                throw new ArgumentOutOfRangeException("Unclosed escape sequence");

            // Now do clean up //
            string t = sb.ToString();

            // The string has some Value //
            if (t.Length != 0)
            {

                // Check that we didn't end on a delim Value, but if we did and we want delims, then keep it //
                if (!(t.Length == 1 && Delim.Contains(t[0])) || KeepDelims)
                    TempArray.Add(sb.ToString());

            }
            // Check if we end on a delim, such as A,B,C,D, where ',' is a delim; we want our array to be {A , B , C , D , null}
            else if (Delim.Contains(Text.Last()))
            {
                TempArray.Add(null);
            }
            return TempArray.ToArray();

        }

        public static string[] Split(string Text, char Delim, char Escape, bool KeepDelims)
        {
            return Split(Text, new char[] { Delim }, Escape, KeepDelims);
        }

        public static string[] Split(string Text, char[] Delim, char Escape)
        {
            return Split(Text, Delim, Escape, false);
        }

        public static string[] Split(string Text, char Delim, char Escape)
        {
            return Split(Text, Delim, Escape, false);
        }

        public static string[] Split(string Text, char[] Delim)
        {
            return Split(Text, Delim, char.MaxValue);
        }

        public static string[] Split(string Text, char Delim)
        {
            return Split(Text, Delim, char.MaxValue);
        }

        // Splicer //
        public static string[] Splice(string Text, int[] Map)
        {

            int idx = 0;
            List<string> vals = new List<string>();
            foreach (int j in Map)
            {
                string s = Text.Substring(idx, j - idx);
                vals.Add(s);
                idx = j + 1;
            }
            return vals.ToArray();

        }

        // Records //
        public static Record ToRecord(string Text, Schema Columns, char[] Delims, char Escape)
        {

            // Split the data //
            string[] t = StringUtil.Split(Text, Delims, Escape, false);

            // Check the length //
            if (t.Length != Columns.Count)
                throw new ArgumentException(string.Format("BString has {0} fields, but schema has {1} fields", t.Length, Columns.Count));

            // Build the record //
            RecordBuilder rb = new RecordBuilder();
            for (int i = 0; i < t.Length; i++)
            {
                rb.Add(CellParser.Parse(t[i], Columns.ColumnAffinity(i)));
            }

            return rb.ToRecord();

        }

        public static Record ToRecord(string Text, Schema Columns, char[] Delims)
        {
            return ToRecord(Text, Columns, Delims, char.MaxValue);
        }

        // Strings //
        public static string ToString(Record Value, string Delim, string Escape)
        {
            
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Value.Count; i++)
            {
                sb.Append(Escape);
                sb.Append(Value[i].ToString());
                sb.Append(Escape);
                if (i != Value.Count - 1)
                    sb.Append(Delim);
            }
            return sb.ToString();

        }

        public static string ToString(Schema Columns, string Delim, string Escape)
        {

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Columns.Count; i++)
            {
                sb.Append(Escape);
                sb.Append(Columns.ColumnName(i));
                sb.Append(Escape);
                if (i != Columns.Count - 1)
                    sb.Append(Delim);
            }
            return sb.ToString();

        }

        public static string ToString(Record Value, char Delim, char Escape)
        {

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Value.Count; i++)
            {
                if (Escape != char.MaxValue) sb.Append(Escape);
                sb.Append(Value[i].ToString());
                if (Escape != char.MaxValue) sb.Append(Escape);
                if (i != Value.Count - 1)
                    sb.Append(Delim);
            }
            return sb.ToString();

        }

        public static string ToString(Schema Columns, char Delim, char Escape)
        {

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Escape != char.MaxValue) sb.Append(Escape);
                sb.Append(Columns.ColumnName(i));
                if (Escape != char.MaxValue) sb.Append(Escape);
                if (i != Columns.Count - 1)
                    sb.Append(Delim);
            }
            return sb.ToString();

        }


    }


}
