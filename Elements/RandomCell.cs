using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Tables;

namespace ArcticWind.Elements
{

    public class RandomCell
    {

        private static string ASCIIPrintable = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
        private static string ASCIIPrintableNoSpace = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
        private static string UpperLowerNumText = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private static string UpperNumText = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static string LowerNumText = "0123456789abcdefghijklmnopqrstuvwxyz";
        private static string UpperText = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static string LowerText = "abcdefghijklmnopqrstuvwxyz";
        private static string Num = "0123456789";

        protected Random _base;
        private object _lock;

        public RandomCell(int Seed)
        {
            this._base = new Random(Seed);
            this._lock = new object();
        }

        public RandomCell()
            : this(RandomCell.TruelyRandomSeed())
        {
        }

        // ### THREAD SAFE ###
        public void Remix(int Seed)
        {

            lock (this._lock)
            {
                this._base = new Random(Seed);
            }

        }

        public void Remix()
        {
            this.Remix(RandomCell.TruelyRandomSeed());
        }

        // Booleans //
        public Cell NextBool()
        {
            return new Cell(this.BaseDouble() < 0.50);
        }

        public Cell NextBool(double Likelyhood)
        {
            return new Cell(this.BaseDouble() < Likelyhood);
        }

        // Bytes //
        public Cell NextByte()
        {
            return new Cell(this.BaseByte());
        }

        public Cell NextByte(byte Lower, byte Upper)
        {
            byte x = (byte)(this.BaseByte() % (Upper - Lower) + Lower);
            return new Cell(x);
        }

        // Short //
        public Cell NextShort()
        {
            return new Cell(this.BaseShort());
        }

        public Cell NextShort(short Lower, short Upper)
        {
            short x = (short)(this.BaseShort() % (Upper - Lower) + Lower);
            return new Cell(x);
        }

        // Int //
        public Cell NextInt()
        {
            return new Cell(this.BaseInt());
        }

        public Cell NextInt(int Lower, int Upper)
        {
            int x = (int)(this.BaseInt() % (Upper - Lower) + Lower);
            return new Cell(x);
        }
        
        // Long //
        public Cell NextLong()
        {
            return new Cell(this.BaseLong());
        }

        public Cell NextLong(long Lower, long Upper)
        {
            long x = (long)(this.BaseLong() % (Upper - Lower) + Lower);
            return new Cell(x);
        }

        // Dates //
        public Cell NextDate()
        {

            byte[] x = this.ByteArrayBase(4);

            DateTime y = DateTime.Now;

            int year = (int)BitConverter.ToUInt16(x, 0) % y.Year;

            int month = (int)x[2] % 12 + 1;

            int divisor = 31;

            bool isLeap = (year % 4 == 0);

            if (year % 100 == 0)
                isLeap = false;

            if (year % 400 == 0)
                isLeap = true;

            if (isLeap && month == 2)
                divisor = 29;
            else if (month == 2)
                divisor = 28;

            if (month == 4 || month == 6 || month == 9 || month == 11)
                divisor = 30;

            int day = x[3] % divisor + 1;

            DateTime z = new DateTime(year, month, day);

            return new Cell(z);

        }

        public Cell NextDate(DateTime Lower, DateTime Upper)
        {

            long span = (Upper.Ticks - Lower.Ticks) / TimeSpan.TicksPerDay;
            long t = this.BaseLong() % span;
            DateTime x = new DateTime(t * TimeSpan.TicksPerDay + Lower.Ticks);
            return new Cell(x);

        }

        // Single //
        public Cell NextSingle()
        {
            return new Cell(this.BaseSingle());
        }

        public Cell NextSingle(Single Lower, Single Upper)
        {
            double d = (Upper - Lower) * this.BaseSingle() + Lower;
            return new Cell(d);
        }

        public Cell NextSingleGauss()
        {
            double u = this.BaseSingle();
            double v = this.BaseSingle();
            double x = Math.Sqrt(-Math.Log(u) * 2f) * Math.Cos(2f * v * Math.PI);
            return new Cell(x);
        }

        // Doubles //
        public Cell NextDouble()
        {
            return new Cell(this.BaseDouble());
        }

        public Cell NextDouble(double Lower, double Upper)
        {
            double d = (Upper - Lower) * this.BaseDouble() + Lower;
            return new Cell(d);
        }

        public Cell NextDoubleGauss()
        {
            double u = this.BaseDouble();
            double v = this.BaseDouble();
            double x = Math.Sqrt(-Math.Log(u) * 2D) * Math.Cos(2D * v * Math.PI);
            return new Cell(x);
        }

        public Cell NextPoisson(double Lambda)
        {
            double L = Math.Exp(-Lambda), K = 0, P = 1;
            do
            {
                K++;
                P *= this._base.NextDouble();
            } while (P > L);
            return new Cell(K-1);
        }

        // B-Strings //
        public Cell NextBString(int Len, BString Corpus)
        {

            if (Len <= 0)
                return CellValues.NullBSTRING;

            Len = Len % Schema.DEFAULT_STRING_SIZE;

            BString.BStringBuilder bb = new BString.BStringBuilder();
            for (int i = 0; i < Len; i++)
            {
                int idx = this.BaseInt() % Corpus.Length;
                bb.Append(Corpus[idx]);
            }

            return new Cell(bb.ToBString());

        }

        public Cell NextBString(int Len)
        {

            if (Len <= 0)
                return CellValues.NullBSTRING;

            Len = Len % Schema.DEFAULT_STRING_SIZE;

            BString.BStringBuilder bb = new BString.BStringBuilder();
            for (int i = 0; i < Len; i++)
            {
                byte b = this.NextByte();
                bb.Append(b);
            }

            return new Cell(bb.ToBString());

        }

        // C-Strings //
        public Cell NextUTF16CString(int Len)
        {
            return this.NextCString(Len, char.MaxValue);
        }

        public Cell NextUTF8CString(int Len)
        {
            return this.NextCString(Len, 255);
        }

        public Cell NextUTF7CString(int Len)
        {
            return this.NextCString(Len, 127);
        }

        public Cell NextCStringASCIIPrintable(int Len)
        {
            return this.NextCString(Len, ASCIIPrintable);
        }

        public Cell NextCStringASCIIPrintableNoSpace(int Len)
        {
            return this.NextCString(Len, ASCIIPrintableNoSpace);
        }

        public Cell NextCStringUpperLowerNumText(int Len)
        {
            return this.NextCString(Len, UpperLowerNumText);
        }

        public Cell NextCStringUpperNumText(int Len)
        {
            return this.NextCString(Len, UpperNumText);
        }

        public Cell NextCStringLowerNumText(int Len)
        {
            return this.NextCString(Len, LowerNumText);
        }

        public Cell NextCStringUpperText(int Len)
        {
            return this.NextCString(Len, UpperText);
        }

        public Cell NextCStringLowerText(int Len)
        {
            return this.NextCString(Len, LowerText);
        }

        public Cell NextCStringNum(int Len)
        {
            return this.NextCString(Len, Num);
        }
        
        public Cell NextCString(int Len, string Corpus)
        {

            if (Len <= 0)
                return CellValues.NullCSTRING;

            Len = Len % Schema.DEFAULT_STRING_SIZE;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Len; i++)
            {
                int idx = this.BaseInt() % Corpus.Length;
                sb.Append(Corpus[idx]);
            }

            return new Cell(sb.ToString());

        }

        public Cell NextCString(int Len, int Max)
        {

            if (Len <= 0)
                return CellValues.NullCSTRING;

            Len = Len % Schema.DEFAULT_STRING_SIZE;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Len; i++)
            {
                char c = (char)(this.BaseInt() % Max);
                sb.Append(c);
            }

            return new Cell(sb.ToString());

        }

        // Binary //
        public Cell NextBinary(int Len)
        {

            if (Len <= 0)
                return CellValues.NullBLOB;

            Len = Len % CellSerializer.MAX_BLOB_LEN;

            byte[] b = new byte[Len];
            this._base.NextBytes(b);

            return new Cell(b);

        }

        public Cell NextBinary(int Len, byte[] Corpus)
        {

            if (Len <= 0)
                return CellValues.NullBLOB;

            Len = Len % CellSerializer.MAX_BLOB_LEN;

            byte[] b = new byte[Len];
            for (int i = 0; i < b.Length; i++)
            {
                int idx = this.NextInt() % b.Length;
                b[i] = Corpus[idx];
            }

            return new Cell(b);

        }

        // Empirical //
        public Cell NextEmperical(CellMatrix Probabilities)
        {

            double rand = this.BaseDouble();
            for (int i = 0; i < Probabilities.RowCount; i++)
            {
                if (Probabilities[i,0].valueDOUBLE >= rand)
                    return new Cell(i);
            }
            return CellValues.NullINT;

        }

        // ### THREAD SAFE ###
        // ----------------------------------------------------------------- //
        public short BaseByte()
        {
            lock (this._lock)
            {
                byte x = (byte)(this._base.Next() & byte.MaxValue);
                return x;
            }
        }

        public short BaseShort()
        {
            lock (this._lock)
            {
                short a = (short)(this._base.Next() & short.MaxValue);
                return (short)(a < 0 ? ~a : a);
            }
        }

        public int BaseInt()
        {

            lock (this._lock)
            {
                int a = this._base.Next();
                return (a < 0 ? ~a : a);
            }

        }

        public long BaseLong()
        {

            lock (this._lock)
            {
                long a = (long)this._base.Next();
                long b = (long)this._base.Next();
                a = (a << 32) | (b);
                return (a < 0 ? ~a : a);
            }
            
        }

        public Single BaseSingle()
        {

            lock (this._lock)
            {
                return (Single)this._base.NextDouble();
            }

        }

        public double BaseDouble()
        {

            lock (this._lock)
            {
                return this._base.NextDouble();
            }

        }

        public char BaseChar()
        {
            lock (this._lock)
            {
                char a = (char)(this._base.Next() & char.MaxValue);
                return (char)(a < 0 ? ~a : a);
            }
        }

        public byte[] ByteArrayBase(int Len)
        {

            lock (this._lock)
            {
                byte[] b = new byte[Len];
                this._base.NextBytes(b);
                return b;
            }

        }
        
        // ----------------------------------------------------------------- //
        public long NextPrimeBase()
        {

            long x = this.BaseLong();
            if (x < 0) x = -x;

            while (!RandomCell.IsPrime(x))
            {
                x = this.BaseLong();
                if (x < 0) x = -x;
            }

            return x;

        }

        // Statics //
        public static int TruelyRandomSeed()
        {

            byte[] a = Guid.NewGuid().ToByteArray();
            
            int b = System.Threading.Thread.CurrentThread.ManagedThreadId;
            byte[] c = BitConverter.GetBytes(b * b * b * b);

            int d = ((short)DateTime.Now.Ticks % short.MaxValue);
            byte[] e = BitConverter.GetBytes(d * d);

            ushort f = 0;

            while (f == 0)
            {
                f = (ushort)(BitConverter.ToUInt16(Guid.NewGuid().ToByteArray(), 7) ^ (ushort.MaxValue));
            }

            byte[] g = new byte[24];
            Array.Copy(a, g, 16);
            Array.Copy(c, 0, g, 16, 4);
            Array.Copy(e, 0, g, 20, 4);

            using (System.Security.Cryptography.SHA1Managed sha1 = new System.Security.Cryptography.SHA1Managed())
            {

                for (UInt16 h = 0; h < f; h++)
                {
                    g = sha1.ComputeHash(g);
                }

            }

            return BitConverter.ToInt32(g, 9);

        }

        public static bool IsPrime(long Value)
        {

            if (Value <= 1)
                return false;

            if (Value < 6)
                return (Value == 2 || Value == 3 || Value == 5) ? true : false;

            if (((Value + 1) % 6 != 0) && ((Value - 1) % 6 != 0))
                return false;

            for (long i = 2; i <= (long)Math.Sqrt(Value) + 1; i++)
                if (Value % i == 0)
                    return false;
            return true;

        }

    }


}
