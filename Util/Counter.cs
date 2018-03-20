using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticWind.Util
{

    public sealed class Counter
    {

        private long _x = 0;

        public Counter(long Start)
        {
            this._x = Start;
        }

        public Counter()
            : this(0)
        {
        }

        public long Count
        {
            get { return this._x; }
        }

        public void Click(long Step)
        {
            this._x += Step;
        }

        public void Click()
        {
            this._x++;
        }

    }

}
