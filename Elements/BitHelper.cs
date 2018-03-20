using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticWind.Elements
{
    
    public static class BitHelper
    {

        //public static readonly int[] BitOnMask = new int[] { 1, 2, 4, 8, 16, 32, 64, 128 };
        public static readonly int[] BitOnMask = new int[] { 128, 64, 32, 16, 8, 4, 2, 1 };
        public static readonly int[] BitOffMask = new int[] { 254, 253, 251, 247, 239, 223, 191, 127 };

        public static bool BitIsOn(byte B, int Element)
        {
            if (Element < 0 || Element >= 8) throw new Exception();
            return (BitOnMask[Element] & B) == BitOnMask[Element];
        }

        public static byte FlipOn(byte B, int Element)
        {
            if (Element < 0 || Element >= 8) throw new Exception();
            return (byte)(BitOnMask[Element] | B);
        }

        public static byte FlipOff(byte B, int Element)
        {
            if (Element < 0 || Element >= 8) throw new Exception();
            return (byte)(BitOffMask[Element] & B);
        }

        public static byte[] ShiftLeft(byte[] B)
        {

            byte[] x = new byte[B.Length];
            bool flip = false;
            for (int i = B.Length - 1; i >= 0; i--)
            {
                byte z = B[i];
                x[i] = (byte)(z << 1);
                if (flip) x[i] = FlipOn(x[i], 7);
                flip = BitIsOn(z, 0);
            }
            return x;

        }

        public static byte[] ShiftLeft(byte[] B, int Count)
        {
            if (Count < 0 || Count >= B.Length * 8) throw new Exception();
            for (int i = 0; i < Count; i++)
                B = ShiftLeft(B);
            return B;
        }

        public static byte[] RotateLeft(byte[] B)
        {

            byte[] x = new byte[B.Length];
            bool flip = false;
            for (int i = B.Length - 1; i >= 0; i--)
            {
                byte z = B[i];
                x[i] = (byte)(z << 1);
                if (flip) x[i] = FlipOn(x[i], 7);
                flip = BitIsOn(z, 0);
            }
            byte q = x[x.Length - 1];
            if (BitHelper.BitIsOn(B[0], 0))
            {
                x[x.Length - 1] = BitHelper.FlipOn(x[x.Length - 1], 7);
            }
            return x;

        }

        public static byte[] RotateLeft(byte[] B, int Count)
        {
            if (Count < 0 || Count >= B.Length * 8) throw new Exception();
            for (int i = 0; i < Count; i++)
                B = RotateLeft(B);
            return B;
        }

        public static byte[] ShiftRight(byte[] B)
        {

            byte[] x = new byte[B.Length];
            bool flip = false;
            for (int i = 0; i < B.Length; i++)
            {
                byte z = B[i];
                x[i] = (byte)(z >> 1);
                if (flip) x[i] = FlipOn(x[i], 0);
                flip = BitIsOn(z, 7);
            }
            return x;

        }

        public static byte[] ShiftRight(byte[] B, int Count)
        {
            if (Count < 0 || Count >= B.Length * 8) throw new Exception();
            for (int i = 0; i < Count; i++)
                B = ShiftRight(B);
            return B;
        }

        public static byte[] RotateRight(byte[] B)
        {

            byte[] x = new byte[B.Length];
            bool flip = false;
            for (int i = 0; i < B.Length; i++)
            {
                byte z = B[i];
                x[i] = (byte)(z >> 1);
                if (flip) x[i] = FlipOn(x[i], 0);
                flip = BitIsOn(z, 7);
            }
            byte q = x[0];
            if (BitHelper.BitIsOn(B.Last(), 7))
            {
                x[0] = BitHelper.FlipOn(x[0], 0);
            }
            return x;

        }

        public static byte[] RotateRight(byte[] B, int Count)
        {
            if (Count < 0 || Count >= B.Length * 8) throw new Exception();
            for (int i = 0; i < Count; i++)
                B = RotateRight(B);
            return B;
        }

        public static string ToString(byte B)
        {
            return Convert.ToString(B, 2).PadLeft(8,'0');
        }

        public static string ToString(byte[] B)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte x in B)
            {
                sb.AppendLine(Convert.ToString(x, 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }

        public static byte RotateLeft(byte X, int Count)
        {
            int len = sizeof(byte) * 8;
            Count = Count % len;
            return (byte)((X << Count) | (Count >> (len - Count)));
        }

        public static byte RotateRight(byte X, int Count)
        {
            int len = sizeof(byte) * 8;
            Count = Count % len;
            return (byte)((X >> Count) | (Count << (len - Count)));
        }

        public static short RotateLeft(short X, int Count)
        {
            int len = sizeof(short) * 8;
            Count = Count % len;
            return (short)((X << Count) | (Count >> (len - Count)));
        }

        public static short RotateRight(short X, int Count)
        {
            int len = sizeof(short) * 8;
            Count = Count % len;
            return (short)((X >> Count) | (Count << (len - Count)));
        }

        public static int RotateLeft(int X, int Count)
        {
            int len = sizeof(int) * 8;
            Count = Count % len;
            return (int)((X << Count) | (Count >> (len - Count)));
        }

        public static int RotateRight(int X, int Count)
        {
            int len = sizeof(int) * 8;
            Count = Count % len;
            return (int)((X >> Count) | (Count << (len - Count)));
        }

        public static long RotateLeft(long X, int Count)
        {
            int len = sizeof(long) * 8;
            Count = Count % len;
            return (long)((X << Count) | (Count >> (len - Count)));
        }

        public static long RotateRight(long X, int Count)
        {
            int len = sizeof(long) * 8;
            Count = Count % len;
            return (long)((X >> Count) | (Count << (len - Count)));
        }

    }

}
