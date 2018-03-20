using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Expressions.TableExpressions;
using ArcticWind.Expressions.ActionExpressions;
using ArcticWind.Expressions;
using ArcticWind.Tables;
using ArcticWind.Elements;
using ArcticWind.Elements.Structures;

namespace ArcticWind.Libraries
{


    public sealed class ChronoLibrary : Library
    {

        public enum DatePart : byte
        {
            Year = 0,
            Month = 1,
            Day = 2,
            Hour = 3,
            Minute = 4,
            Second = 5,
            Milisecond = 6
        }

        public const string NOW = "NOW";
        public const string NOWD = "NOWD";
        public const string NOWT = "NOWT";
        public const string YEAR = "YEAR";
        public const string MONTH = "MONTH";
        public const string DAY = "DAY";
        public const string HOUR = "HOUR";
        public const string MINUTE = "MINUTE";
        public const string SECOND = "SECOND";
        public const string MILISECOND = "MILISECOND";
        public const string BUILD = "BUILD";
        public const string ADD = "ADD";
        public const string DIF = "DIF";
        public const string DOW = "DOW";
        public const string DOY = "DOY";
        public const string DATE_STRING = "DATE_STRING";
        public const string TIME_STRING = "TIME_STRING";

        public readonly string[] ActionNames = { };
        public readonly string[] ScalarNames = { NOW, NOWD, NOWT, YEAR, MONTH, DAY, HOUR, MINUTE, SECOND, MILISECOND, BUILD, ADD, DIF, DOW, DOY, DATE_STRING, TIME_STRING };
        public readonly string[] MatrixNames = { };
        public readonly string[] RecordNames = { NOW, NOWD, NOWT };
        public readonly string[] TableNames = { };

        public ChronoLibrary(Host Host)
            : base(Host, "CHRONO")
        {
        }

        public override bool ActionExists(string Name)
        {
            return ActionNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override bool ScalarFunctionExists(string Name)
        {
            return ScalarNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override bool MatrixFunctionExists(string Name)
        {
            return MatrixNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override bool RecordFunctionExists(string Name)
        {
            return RecordNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override bool TableFunctionExists(string Name)
        {
            return TableNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override ActionExpressionParameterized ActionLookup(string Name)
        {
            throw new Exception(string.Format("Element does not exist '{0}'", Name));
        }

        public override ScalarExpressionFunction ScalarFunctionLookup(string Name)
        {

            switch (Name.ToUpper())
            {

                case NOW:
                    return new sfNOW(this._Host);
                case NOWD:
                    return new sfNOWD(this._Host);
                case NOWT:
                    return new sfNOWT(this._Host);
                case YEAR:
                    return new sfYEAR(this._Host);
                case MONTH:
                    return new sfMONTH(this._Host);
                case DAY:
                    return new sfDAY(this._Host);
                case HOUR:
                    return new sfHOUR(this._Host);
                case MINUTE:
                    return new sfMINUTE(this._Host);
                case SECOND:
                    return new sfSECOND(this._Host);
                case MILISECOND:
                    return new sfMILISECOND(this._Host);
                case BUILD:
                    return new sfBUILD(this._Host);
                case ADD:
                    return new sfADD(this._Host);
                case DIF:
                    return new sfDIF(this._Host);
                case DOW:
                    return new sfDOW(this._Host);
                case DOY:
                    return new sfDOY(this._Host);
                case DATE_STRING:
                    return new sfDATE_STRING(this._Host);
                case TIME_STRING:
                    return new sfTIME_STRING(this._Host);

            }

            throw new Exception(string.Format("Element does not exist '{0}'", Name));

        }

        public override MatrixExpressionFunction MatrixFunctionLookup(string Name)
        {
            throw new Exception(string.Format("Element does not exist '{0}'", Name));
        }

        public override RecordExpressionFunction RecordFunctionLookup(string Name)
        {
            switch (Name.ToUpper())
            {
                case NOW: return new rfNOW(this._Host);
            }
            throw new Exception(string.Format("Element does not exist '{0}'", Name));
        }

        public override TableExpressionFunction TableFunctionLookup(string Name)
        {
            throw new Exception(string.Format("Element does not exist '{0}'", Name));
        }

        // Records //
        public sealed class rfNOW : RecordExpressionFunction
        {

            public rfNOW(Host Host)
                : base(Host, null, NOW, 0)
            {
            }

            public override RecordExpression CloneOfMe()
            {
                return new rfNOW(this._Host);
            }

            public override Schema Columns
            {
                
                get 
                {
                    Schema s = new Schema();
                    s.Add("YEAR", CellAffinity.INT);
                    s.Add("MONTH", CellAffinity.INT);
                    s.Add("DAY", CellAffinity.INT);
                    s.Add("HOUR", CellAffinity.INT);
                    s.Add("MINUTE", CellAffinity.INT);
                    s.Add("SECOND", CellAffinity.INT);
                    s.Add("MILISECOND", CellAffinity.INT);
                    return s;
                }

            }

            public override AssociativeRecord EvaluateAssociative(SpoolSpace Variants)
            {
                DateTime dt = DateTime.Now;
                AssociativeRecord x = new AssociativeRecord(this.Columns);
                x[0] = dt.Year;
                x[1] = dt.Month;
                x[2] = dt.Day;
                x[3] = dt.Hour;
                x[4] = dt.Minute;
                x[5] = dt.Second;
                x[6] = dt.Millisecond;
                return x;
            }

        }

        // Scalars //
        public sealed class sfNOW : ScalarExpressionFunction
        {

            public sfNOW(Host Host)
                : base(Host, null, NOW, 0, CellAffinity.DATE_TIME)
            {
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                return new Cell(DateTime.Now);
            }

        }

        public sealed class sfNOWD : ScalarExpressionFunction
        {

            public sfNOWD(Host Host)
                : base(Host, null, NOWD, 0, CellAffinity.DATE_TIME)
            {
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                return new Cell(DateTime.Now.Date);
            }

        }

        public sealed class sfNOWT : ScalarExpressionFunction
        {

            public sfNOWT(Host Host)
                : base(Host, null, NOWT, 0, CellAffinity.LONG)
            {
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                return new Cell(DateTime.Now.TimeOfDay.Ticks);
            }

        }

        public sealed class sfYEAR : ScalarExpressionFunction
        {

            public sfYEAR(Host Host)
                : base(Host, null, YEAR, 1, CellAffinity.INT)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (!this.CheckSigniture(ParameterAffinity.Scalar))
                    throw new Exception("Invalid argument passed");
                Cell c = this._Params[0].Scalar.Evaluate(Variants);
                return CellFunctions.Year(c);
            }

        }

        public sealed class sfMONTH : ScalarExpressionFunction
        {

            public sfMONTH(Host Host)
                : base(Host, null, MONTH, 1, CellAffinity.INT)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (!this.CheckSigniture(ParameterAffinity.Scalar))
                    throw new Exception("Invalid argument passed");
                Cell c = this._Params[0].Scalar.Evaluate(Variants);
                return CellFunctions.Month(c);
            }

        }

        public sealed class sfDAY : ScalarExpressionFunction
        {

            public sfDAY(Host Host)
                : base(Host, null, DAY, 1, CellAffinity.INT)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (!this.CheckSigniture(ParameterAffinity.Scalar))
                    throw new Exception("Invalid argument passed");
                Cell c = this._Params[0].Scalar.Evaluate(Variants);
                return CellFunctions.Day(c);
            }

        }

        public sealed class sfHOUR : ScalarExpressionFunction
        {

            public sfHOUR(Host Host)
                : base(Host, null, HOUR, 1, CellAffinity.INT)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (!this.CheckSigniture(ParameterAffinity.Scalar))
                    throw new Exception("Invalid argument passed");
                Cell c = this._Params[0].Scalar.Evaluate(Variants);
                return CellFunctions.Hour(c);
            }

        }

        public sealed class sfMINUTE : ScalarExpressionFunction
        {

            public sfMINUTE(Host Host)
                : base(Host, null, MINUTE, 1, CellAffinity.INT)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (!this.CheckSigniture(ParameterAffinity.Scalar))
                    throw new Exception("Invalid argument passed");
                Cell c = this._Params[0].Scalar.Evaluate(Variants);
                return CellFunctions.Minute(c);
            }

        }

        public sealed class sfSECOND : ScalarExpressionFunction
        {

            public sfSECOND(Host Host)
                : base(Host, null, SECOND, 1, CellAffinity.INT)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (!this.CheckSigniture(ParameterAffinity.Scalar))
                    throw new Exception("Invalid argument passed");
                Cell c = this._Params[0].Scalar.Evaluate(Variants);
                return CellFunctions.Second(c);
            }

        }

        public sealed class sfMILISECOND : ScalarExpressionFunction
        {

            public sfMILISECOND(Host Host)
                : base(Host, null, MILISECOND, 1, CellAffinity.INT)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {
                if (!this.CheckSigniture(ParameterAffinity.Scalar))
                    throw new Exception("Invalid argument passed");
                Cell c = this._Params[0].Scalar.Evaluate(Variants);
                return CellFunctions.Millisecond(c);
            }

        }

        public sealed class sfBUILD : ScalarExpressionFunction
        {

            public sfBUILD(Host Host)
                : base(Host, null, BUILD, -7, CellAffinity.DATE_TIME)
            {
            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                if (this._Params.Count == 0)
                    return new Cell(DateTime.Now);

                if (!this._Params.TrueForAll((x) => { return x.Affinity == ParameterAffinity.Scalar; }))
                    throw new Exception("This function requies all scalars");

                int year = this._Params[0].Scalar.Evaluate(Variants).valueINT;
                int month = (this._Params.Count >= 2 ? this._Params[1].Scalar.Evaluate(Variants).valueINT : 1);
                int day = (this._Params.Count >= 3 ? this._Params[2].Scalar.Evaluate(Variants).valueINT : 1);
                int hour = (this._Params.Count >= 4 ? this._Params[3].Scalar.Evaluate(Variants).valueINT : 0);
                int minute = (this._Params.Count >= 5 ? this._Params[4].Scalar.Evaluate(Variants).valueINT : 0);
                int second = (this._Params.Count >= 6 ? this._Params[5].Scalar.Evaluate(Variants).valueINT : 0);
                int milisecond = (this._Params.Count >= 7 ? this._Params[6].Scalar.Evaluate(Variants).valueINT : 0);

                DateTime t = new DateTime(year, month, day, hour, minute, second, milisecond);
                return new Cell(t);

            }

        }

        public sealed class sfADD : ScalarExpressionFunction
        {

            public sfADD(Host Host)
                : base(Host, null, ADD, 3, CellAffinity.DATE_TIME)
            {

            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                this.CheckParameters();

                if (!this._Params.TrueForAll((x) => { return x.Affinity == ParameterAffinity.Scalar; }))
                    throw new Exception("This function requies all scalars");

                DateTime dt = this._Params[0].Scalar.Evaluate(Variants).valueDATE;
                int val = this._Params[1].Scalar.Evaluate(Variants).valueINT;
                DatePart dp = ChronoLibrary.ImputeDatePart(this._Params[2].Scalar.Evaluate(Variants));

                switch (dp)
                {
                    case DatePart.Year: dt = dt.AddYears(val);
                        break;
                    case DatePart.Month: dt = dt.AddMonths(val);
                        break;
                    case DatePart.Day: dt = dt.AddDays(val);
                        break;
                    case DatePart.Hour: dt = dt.AddHours(val);
                        break;
                    case DatePart.Minute: dt = dt.AddMinutes(val);
                        break;
                    case DatePart.Second: dt = dt.AddSeconds(val);
                        break;
                    case DatePart.Milisecond: dt = dt.AddMilliseconds(val);
                        break;
                }

                return new Cell(dt);

            }


        }

        public sealed class sfDIF : ScalarExpressionFunction
        {

            public sfDIF(Host Host)
                : base(Host, null, DIF, 2, CellAffinity.LONG)
            {

            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                this.CheckParameters();

                if (!this._Params.TrueForAll((x) => { return x.Affinity == ParameterAffinity.Scalar; }))
                    throw new Exception("This function requies all scalars");

                Cell a = this._Params[0].Scalar.Evaluate(Variants);
                Cell b = this._Params[1].Scalar.Evaluate(Variants);
                if (a.IsNull || b.IsNull || a.Affinity != CellAffinity.DATE_TIME || b.Affinity != CellAffinity.DATE_TIME)
                    return CellValues.NullLONG;
                return new Cell((a.valueDATE - b.valueDATE).Ticks);

            }


        }

        public sealed class sfDOW : ScalarExpressionFunction
        {

            public sfDOW(Host Host)
                : base(Host, null, DOW, 1, CellAffinity.BYTE)
            {

            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                this.CheckParameters();

                if (!this._Params.TrueForAll((x) => { return x.Affinity == ParameterAffinity.Scalar; }))
                    throw new Exception("This function requies all scalars");

                Cell a = this._Params[0].Scalar.Evaluate(Variants);
                if (a.IsNull)
                    return CellValues.NullBYTE;

                return new Cell((byte)a.valueDATE.DayOfWeek);

            }

        }

        public sealed class sfDOY : ScalarExpressionFunction
        {

            public sfDOY(Host Host)
                : base(Host, null, DOY, 1, CellAffinity.SHORT)
            {

            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                this.CheckParameters();

                if (!this._Params.TrueForAll((x) => { return x.Affinity == ParameterAffinity.Scalar; }))
                    throw new Exception("This function requies all scalars");

                Cell a = this._Params[0].Scalar.Evaluate(Variants);
                if (a.IsNull)
                    return CellValues.NullSHORT;

                return new Cell((short)a.valueDATE.DayOfYear);

            }

        }

        public sealed class sfDATE_STRING : ScalarExpressionFunction
        {

            public sfDATE_STRING(Host Host)
                : base(Host, null, DATE_STRING, 1, CellAffinity.BSTRING)
            {

            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                this.CheckParameters();

                if (!this._Params.TrueForAll((x) => { return x.Affinity == ParameterAffinity.Scalar; }))
                    throw new Exception("This function requies all scalars");

                Cell a = this._Params[0].Scalar.Evaluate(Variants);
                if (a.IsNull)
                    return CellValues.NullBSTRING;
                DateTime dt = a.valueDATE;

                return new Cell(string.Format("{0}-{1}-{2}", dt.Year, dt.Month, dt.Day));

            }

        }

        public sealed class sfTIME_STRING : ScalarExpressionFunction
        {

            public sfTIME_STRING(Host Host)
                : base(Host, null, TIME_STRING, 1, CellAffinity.BSTRING)
            {

            }

            public override Cell Evaluate(SpoolSpace Variants)
            {

                this.CheckParameters();

                if (!this._Params.TrueForAll((x) => { return x.Affinity == ParameterAffinity.Scalar; }))
                    throw new Exception("This function requies all scalars");

                Cell a = this._Params[0].Scalar.Evaluate(Variants);
                if (a.IsNull)
                    return CellValues.NullBSTRING;
                TimeSpan ts = new TimeSpan();
                if (a.Affinity == CellAffinity.DATE_TIME)
                    ts = a.valueDATE.TimeOfDay;
                else if (CellAffinityHelper.IsIntegral(a.Affinity))
                    ts = new TimeSpan(a.valueLONG);

                return new Cell(new BString(ts.ToString()));

            }

        }

        // --------- Support --------- 
        private static DatePart ImputeDatePart(Cell Parameter)
        {

            if (Parameter.IsNull)
                return DatePart.Milisecond;
            if (Parameter.Affinity == CellAffinity.CSTRING || Parameter.Affinity == CellAffinity.BSTRING)
            {

                switch (Parameter.ToString().ToUpper())
                {

                    case "YEAR":
                    case "YYYY":
                    case "YR":
                    case "YY":
                    case "Y":
                        return DatePart.Year;

                    case "MONTH":
                    case "MNTH":
                    case "MM":
                    case "M":
                        return DatePart.Month;

                    case "DAY":
                    case "DY":
                    case "DD":
                    case "D":
                        return DatePart.Day;

                    case "HOUR":
                    case "HR":
                    case "H":
                        return DatePart.Hour;

                    case "MINUTE":
                    case "MIN":
                    case "MN":
                        return DatePart.Minute;

                    case "SECOND":
                    case "SEC":
                    case "S":
                        return DatePart.Second;

                    case "MILISECOND":
                    case "MILLISECOND":
                    case "MILI":
                    case "MS":
                        return DatePart.Milisecond;


                }

            }
            else if (CellAffinityHelper.IsNumeric(Parameter.Affinity))
            {
                byte b = Parameter.valueBYTE;
                if (b > 6) throw new Exception("DateParts from integral parameters must be between 0 and 6");
                return (DatePart)b;
            }

            return DatePart.Milisecond;


        }


    }


}
