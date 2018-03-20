using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticWind.Elements
{
    
    /// <summary>
    /// Represents an immutible 8-bit string
    /// </summary>
    public sealed class BString 
    {

        public byte[] _elements;

        public static readonly BString Empty = new BString(0);

        public BString(byte[] Data)
        {
            if (Data == null)
            {
                this._elements = new byte[] { };
                return;
            }
            this._elements = new byte[Data.Length];
            Array.Copy(Data, 0, this._elements, 0, Data.Length);
        }

        public BString(byte Value, int Length)
        {
            if (Length < 0)
                throw new Exception("Length must be greater than 0");
            this._elements = new byte[Length];
            for (int i = 0; i < Length; i++)
                this._elements[i] = Value;
        }

        public BString(string Text)
            : this(BStringEncoding.StringToBytes(Text ?? ""))
        {
        }

        public BString(int Length)
            : this(Length == 0 ? new byte[] {} : new byte[Length])
        {
        }

        public int Length
        {
            get 
            {
                return this._elements.Length; 
            }
        }

        public bool IsEmpty
        {
            get { return this._elements.Length == 0; }
        }

        public byte[] ToByteArray
        {
            get
            {
                byte[] b = new byte[this.Length];
                //Array.Copy(this._elements, 0, b, 0, b.Length);
                Array.Copy(this._elements, b, b.Length);
                return b;
            }
        }

        public byte this[int Index]
        {
            get { return this._elements[Index]; }
        }

        // String Functions //
        public BString Trim(byte[] Elements)
        {

            List<byte> vals = new List<byte>();
            foreach (byte b in this._elements)
            {
                if (!Elements.Contains(b))
                    vals.Add(b);
            }
            return new BString(vals.ToArray());

        }

        public BString Trim()
        {
            return this.Trim(BStringEncoding.WhiteSpace);
        }

        public BString Substring(int Start, int Length)
        {

            if (Start + Length > this.Length)
                throw new IndexOutOfRangeException();

            byte[] b = new byte[Length];
            Array.Copy(this._elements, Start, b, 0, Length);
            return new BString(b);

        }

        public BString Left(int Length)
        {
            return this.Substring(0, Length);
        }

        public BString Right(int Length)
        {
            return this.Substring(this.Length - Length, Length);
        }

        public BString ToLower()
        {
            byte[] b = new byte[this.Length];
            for (int i = 0; i < this.Length; i++)
            {
                b[i] = BStringEncoding.ToLower(this._elements[i]);
            }
            return new BString(b);
        }

        public BString ToUpper()
        {
            byte[] b = new byte[this.Length];
            for (int i = 0; i < this.Length; i++)
            {
                b[i] = BStringEncoding.ToUpper(this._elements[i]);
            }
            return new BString(b);
        }

        public BString[] Split(byte[] Delim, byte Escape, bool KeepDelims)
        {

            if (Delim.Contains(Escape))
                throw new Exception("The deliminators cannot contain the escape token");

            List<BString> TempArray = new List<BString>();
            BStringBuilder sb = new BStringBuilder();
            bool InEscape = false;

            // Go through each char in string //
            foreach (byte b in this._elements)
            {

                // turn on escaping //
                if (b == Escape)
                    InEscape = (!InEscape);

                // Slipt //
                if (!InEscape)
                {

                    // We found a deliminator //
                    if (Delim.Contains(b))
                    {

                        BString s = sb.ToBString();

                        // Check the size of the current cache and add the string, which would happend if we had 'A,B,,C,D' //
                        if (s.Length == 0)
                            TempArray.Add(null);
                        else
                            TempArray.Add(s);

                        // Check to see if we need to keep our delims //
                        if (KeepDelims)
                            TempArray.Add(new BString(b));

                        sb = new BStringBuilder();

                    }
                    else if (b != Escape)
                    {
                        sb.Append(b);
                    }

                }// end the string building phase //
                else if (b != Escape)
                {
                    sb.Append(b);
                }

            }

            if (InEscape)
                throw new ArgumentOutOfRangeException("Unclosed escape sequence");

            // Now do clean up //
            BString t = sb.ToBString();

            // The string has some Value //
            if (t.Length != 0)
            {

                // Check that we didn't end on a delim Value, but if we did and we want delims, then keep it //
                if (!(t.Length == 1 && Delim.Contains(t[0])) || KeepDelims)
                    TempArray.Add(sb.ToBString());

            }
            // Check if we end on a delim, such as A,B,C,D, where ',' is a delim; we want our array to be {A , B , C , D , null}
            else if (Delim.Contains(this._elements.Last()))
            {
                TempArray.Add(null);
            }
            return TempArray.ToArray();


        }

        public BString[] Split(byte[] Delim, byte Escape)
        {
            return this.Split(Delim, Escape, false);
        }

        public BString[] Split(byte[] Delim)
        {
            return this.Split(Delim, byte.MinValue, false);
        }

        public int Find(BString Value, int StartAt)
        {

            if (Value.Length > this.Length - StartAt)
                return -1;

            bool found = false;
            for (int i = StartAt; i < this.Length - Value.Length; i++)
            {

                found = true;
                for (int j = 0; j < Value.Length; j++)
                {
                    if (this[i + j] != Value[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found) return i;

            }

            return -1;

        }

        public int Find(BString Value)
        {
            return this.Find(Value, 0);
        }

        public int[] FindAll(BString Value, int StartAt)
        {

            if (Value.Length > this.Length - StartAt)
                return new int[] {};

            List<int> indexes = new List<int>();
            bool found = false;
            for (int i = StartAt; i < this.Length - Value.Length; i++)
            {

                found = true;
                for (int j = 0; j < Value.Length; j++)
                {
                    if (this[i + j] != Value[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found) 
                    indexes.Add(i);

            }

            return indexes.ToArray();

        }

        public BString Replace(BString Old, BString New, int StartAt)
        {

            if (StartAt >= this.Length)
                throw new Exception("Starting postition cannot be greater than Length");

            if (Old.Length > this.Length)
                return new BString(this._elements);
            
            BStringBuilder builder = new BStringBuilder();

            bool found = false;
            for (int i = StartAt; i < this.Length; i++)
            {

                found = (i >= this.Length - Old.Length ? false : BString.Equals(this, i, Old, 0, Old.Length));
                if (!found)
                {
                    builder.Append(this[i]);
                }
                else
                {
                    builder.Append(New);
                    i += Old.Length - 1;
                }

            }

            return builder.ToBString();

        }

        public BString Replace(BString Old, BString New)
        {
            return this.Replace(Old, New, 0);
        }

        public BString Remove(BString Old, int StartAt)
        {
            return this.Replace(Old, BString.Empty, StartAt);
        }

        public BString Remove(BString Old)
        {
            return this.Replace(Old, BString.Empty, 0);
        }

        // Overrides //
        public override string ToString()
        {
            return BStringEncoding.BytesToString(this._elements);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return BString.GetHashCode(this);
        }

        // Opperators //
        public static BString operator +(BString A, BString B)
        {
            return BString.Concat(A, B);
        }

        public static bool operator ==(BString A, BString B)
        {
            return BString.CompareStrict(A, B) == 0;
        }

        public static bool operator !=(BString A, BString B)
        {
            return BString.CompareStrict(A, B) != 0;
        }

        public static bool operator <(BString A, BString B)
        {
            return BString.CompareStrict(A, B) < 0;
        }

        public static bool operator <=(BString A, BString B)
        {
            return BString.CompareStrict(A, B) <= 0;
        }

        public static bool operator >(BString A, BString B)
        {
            return BString.CompareStrict(A, B) > 0;
        }

        public static bool operator >=(BString A, BString B)
        {
            return BString.CompareStrict(A, B) >= 0;
        }

        public static implicit operator BString(string Value)
        {
            return new BString(Value);
        }

        // Interfaces //
        
        // Compare Functions //
        public static int CompareStrict(BString A, BString B)
        {

            if (object.ReferenceEquals(A, null) || object.ReferenceEquals(B, null))
                return -1;

            if (A.Length != B.Length)
                return A.Length - B.Length;

            for (int i = 0; i < A.Length; i++)
            {
                if (A[i] != B[i]) return A[i] < B[i] ? -1 : 1;
            }

            return 0;

        }

        public static int CompareWeak(BString A, BString B)
        {

            if (object.ReferenceEquals(A, null) || object.ReferenceEquals(B, null))
                return -1;

            for (int i = 0; i < Math.Min(A.Length, B.Length); i++)
            {
                if (A[i] != B[i]) return A[i] < B[i] ? -1 : 1;
            }

            return 0;

        }

        public static int CompareStrictIgnoreCase(BString A, BString B)
        {

            if (object.ReferenceEquals(A, null) || object.ReferenceEquals(B, null))
                return -1;

            if (A.Length != B.Length)
                return A.Length - B.Length;

            for (int i = 0; i < A.Length; i++)
            {
                if (A[i] != B[i]) return BStringEncoding.ToLower(A[i]) < BStringEncoding.ToLower(B[i]) ? -1 : 1;
            }

            return 0;

        }

        public static int CompareWeakIgnoreCase(BString A, BString B)
        {

            if (object.ReferenceEquals(A, null) || object.ReferenceEquals(B, null))
                return -1;

            for (int i = 0; i < Math.Min(A.Length, B.Length); i++)
            {
                if (A[i] != B[i]) return BStringEncoding.ToLower(A[i]) < BStringEncoding.ToLower(B[i]) ? -1 : 1;
            }

            return 0;

        }

        public static int GetHashCode(BString A)
        {

            if (object.ReferenceEquals(A, null))
                return 0;

            int hash = 0;
            foreach (byte b in A.ToByteArray)
            {
                hash += ((int)b) * 127 % 17;
            }

            return hash;

        }

        public static int GetHashCodeIgnoreCase(BString A)
        {

            if (object.ReferenceEquals(A, null))
                return 0;

            int hash = 0;
            foreach (byte b in A.ToByteArray)
            {
                hash += ((int)BStringEncoding.ToUpper(b)) * 127 % 17;
            }

            return hash;

        }

        public static bool Equals(BString A, int StartAtA, BString B, int StartAtB, int Length)
        {

            if (object.ReferenceEquals(A, null) || object.ReferenceEquals(B, null))
                return false;

            if (A.Length - StartAtA < Length || B.Length - StartAtB < Length)
                throw new Exception("Length and starting positions are invalid");

            for (int i = 0; i < Length; i++)
            {
                if (A._elements[i + StartAtA] != B._elements[i + StartAtB])
                    return false;
            }

            return true;

        }

        // Concatenations //
        public static BString Concat(BString A, BString B)
        {
            BStringBuilder builder = new BStringBuilder();
            builder.Append(A);
            builder.Append(B);
            return builder.ToBString();
        }

        public static BString Concat(IEnumerable<BString> Elements)
        {
            BStringBuilder builder = new BStringBuilder();
            foreach(BString u in Elements)
            {
                builder.Append(u);
            }
            return builder.ToBString();
        }

        // Helpers //
        public sealed class BStringBuilder
        {

            private List<byte> _Values;

            public BStringBuilder()
            {
                this._Values = new List<byte>();
            }
            
            public void AppendLine()
            {
                this._Values.Add(BStringEncoding.CarriageReturn);
                this._Values.Add(BStringEncoding.LineFeed);
            }
            
            public void Append(byte Value)
            {
                this._Values.Add(Value);
            }

            public void AppendLine(byte Value)
            {
                this.Append(Value);
                this.AppendLine();
            }
            
            public void Append(char Value)
            {
                this._Values.Add(BStringEncoding.CharToByte(Value));
            }

            public void AppendLine(char Value)
            {
                this.Append(Value);
                this.AppendLine();
            }

            public void Append(BString Value)
            {
                foreach(byte b in Value._elements)
                {
                    this._Values.Add(b);
                }
            }

            public void AppendLine(BString Value)
            {
                this.Append(Value);
                this.AppendLine();
            }
            
            public void Append(string Value)
            {
                this.Append(BStringEncoding.StringToBytes(Value));
            }
            
            public void AppendLine(string Value)
            {
                this.Append(BStringEncoding.StringToBytes(Value));
                this.AppendLine();
            }

            public void Append(byte[] Value)
            {
                foreach(byte b in Value)
                {
                    this._Values.Add(b);
                }
            }
            
            public void AppendLine(byte[] Value)
            {
                this.Append(Value);
                this.AppendLine();
            }
            
            public void Append(char[] Value)
            {
                foreach(char c in Value)
                {
                    this.Append(c);
                }
            }
            
            public void AppendLine(char[] Value)
            {
                this.Append(Value);
                this.AppendLine();
            }

            public BString ToBString()
            {
                return new BString(this._Values.ToArray());
            }

        }

        public sealed class BStringComparerOrdinal : IEqualityComparer, IEqualityComparer<BString>, IComparer, IComparer<BString>
        {

            bool IEqualityComparer.Equals(object A, object B)
            {
                return BString.CompareStrict(A as BString, B as BString) == 0;
            }

            int IEqualityComparer.GetHashCode(object A)
            {
                return BString.GetHashCode(A as BString);
            }

            bool IEqualityComparer<BString>.Equals(BString A, BString B)
            {
                return BString.CompareStrict(A, B) == 0;
            }

            int IEqualityComparer<BString>.GetHashCode(BString A)
            {
                return BString.GetHashCode(A);
            }

            int IComparer.Compare(object A, object B)
            {
                return BString.CompareStrict(A as BString, B as BString);
            }

            int IComparer<BString>.Compare(BString A, BString B)
            {
                return BString.CompareStrict(A, B);
            }


        }

        public sealed class BStringComparerOrdinalIgnoreCase : IEqualityComparer, IEqualityComparer<BString>, IComparer, IComparer<BString>
        {

            bool IEqualityComparer.Equals(object A, object B)
            {
                return BString.CompareStrictIgnoreCase(A as BString, B as BString) == 0;
            }

            int IEqualityComparer.GetHashCode(object A)
            {
                return BString.GetHashCodeIgnoreCase(A as BString);
            }

            bool IEqualityComparer<BString>.Equals(BString A, BString B)
            {
                return BString.CompareStrictIgnoreCase(A, B) == 0;
            }

            int IEqualityComparer<BString>.GetHashCode(BString A)
            {
                return BString.GetHashCodeIgnoreCase(A);
            }

            int IComparer.Compare(object A, object B)
            {
                return BString.CompareStrictIgnoreCase(A as BString, B as BString);
            }

            int IComparer<BString>.Compare(BString A, BString B)
            {
                return BString.CompareStrictIgnoreCase(A, B);
            }


        }

        public static class BStringEncoding
        {

            public static byte Tab
            {
                get { return 9; }
            }

            public static byte LineFeed
            {
                get { return 10; }
            }

            public static byte CarriageReturn
            {
                get { return 13; }
            }

            public static byte Space
            {
                get { return 32; }
            }

            public static byte[] WhiteSpace
            {
                get { return new byte[] { 9, 10, 11, 12, 13, 32, 133, 160 }; }
            }

            public static byte CharToByte(char Value)
            {
                return (byte)(Value & 255);
            }

            public static char ByteToChar(byte Value)
            {
                return (char)Value;
            }

            public static bool IsLatinChar(byte Value)
            {
                return (Value >= 65 && Value <= 90) || (Value >= 97 && Value <= 122);
            }

            public static bool IsNumeric(byte Value)
            {
                return (Value >= 48 && Value <= 57);
            }

            public static bool IsWhiteSpace(byte Value)
            {
                return (Value >= 9 && Value <= 13) || Value == 32 || Value == 133 || Value == 160;
            }

            public static byte[] StringToBytes(string Text)
            {

                byte[] b = new byte[Text.Length];
                int i = 0;
                foreach (char c in Text)
                {
                    b[i] = (byte)(c & 255);
                    i++;
                }
                return b;

            }

            public static string BytesToString(byte[] Binary)
            {

                if (Binary == null) throw new Exception();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < Binary.Length; i++)
                {
                    sb.Append((char)Binary[i]);
                }
                return sb.ToString();
            }

            public static byte ToUpper(byte Value)
            {
                return (byte)(Value >= 97 && Value <= 122 ? Value - 32 : Value);
            }

            public static byte ToLower(byte Value)
            {
                return (byte)(Value >= 65 && Value <= 90 ? Value + 32 : Value);
            }

        }

    }

}
