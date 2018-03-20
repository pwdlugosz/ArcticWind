using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticWind.Alpha
{
    
    public struct Money
    {

        private long _Numeral;
        public const long LOG_BASE = 1000000;
        private const Single LOG_BASE_S = (Single)LOG_BASE;
        private const double LOG_BASE_D = (double)LOG_BASE;

        public Money(byte Value)
            : this()
        {
            this._Numeral = ((long)Value) * LOG_BASE;
        }

        public Money(ushort Value)
            : this()
        {
            this._Numeral = ((long)Value) * LOG_BASE;
        }

        public Money(uint Value)
            : this()
        {
            this._Numeral = ((long)Value) * LOG_BASE;
        }

        public Money(ulong Value)
            : this()
        {
            this._Numeral = ((long)Value) * LOG_BASE;
        }

        public Money(sbyte Value)
            : this()
        {
            this._Numeral = ((long)Value) * LOG_BASE;
        }

        public Money(short Value)
            : this()
        {
            this._Numeral = ((long)Value) * LOG_BASE;
        }

        public Money(int Value)
            : this()
        {
            this._Numeral = ((long)Value) * LOG_BASE;
        }

        public Money(long Value)
            : this()
        {
            this._Numeral = ((long)Value) * LOG_BASE;
        }

        public Money(float Value)
            : this()
        {
            this._Numeral = (long)(Value * LOG_BASE);
        }

        public Money(double Value)
        {
            this._Numeral = (long)(Value * LOG_BASE);
        }

        // Properties //
        public long Serial
        {
            get { return this._Numeral; }
        }

        public Single ToSingle()
        {
            return ((Single)this._Numeral) / LOG_BASE_S;
        }

        public double ToDougle()
        {
            return ((Single)this._Numeral) / LOG_BASE_S;
        }

        public long ToLong()
        {
            return this._Numeral / LOG_BASE;
        }

        // Operators //
        public static Money FromSerial(long X)
        {
            Money y = new Money();
            y._Numeral = X;
            return y;
        }

        public static Money operator +(Money A, Money B)
        {
            return Money.FromSerial(A._Numeral + B._Numeral);
        }

        public static Money operator -(Money A, Money B)
        {
            return Money.FromSerial(A._Numeral - B._Numeral);
        }

        public static Money operator *(Money A, Money B)
        {
            return Money.FromSerial((A._Numeral * B._Numeral) / LOG_BASE);
        }

        public static Money operator /(Money A, Money B)
        {
            return Money.FromSerial((A._Numeral * LOG_BASE) / B._Numeral);
        }

        public static Money operator %(Money A, Money B)
        {
            A._Numeral = A._Numeral - (B._Numeral * (A._Numeral / B._Numeral));
            return A;
        }


    }

}
