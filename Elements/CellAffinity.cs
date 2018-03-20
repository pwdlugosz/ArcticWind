using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticWind.Elements
{

    /// <summary>
    /// ArcticWind data type
    /// </summary>
    public enum CellAffinity : byte
    {
        BOOL = 0,
        //DATE = 1,
        //TIME = 2,
        DATE_TIME = 3,
        BYTE = 4,
        SHORT = 5,
        INT = 6,
        LONG = 7,
        //IPLANE = 8,
        //MONEY = 9, 
        SINGLE = 10,
        DOUBLE = 11,
        //SPLANE = 12,
        BINARY = 14,
        BSTRING = 15,
        CSTRING = 16,
        VARIANT = 255
    }

}
