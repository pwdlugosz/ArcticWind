using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.RecordExpressions;

namespace ArcticWind.Expressions.Aggregates
{

    public sealed class AggregateLookup
    {

        public const string NAME_SUM = "SUM";
        public const string NAME_COUNT = "COUNT";
        public const string NAME_COUNT_NULL = "COUNT_NULL";
        public const string NAME_MIN = "MIN";
        public const string NAME_MAX = "MAX";
        public const string NAME_FIRST = "FIRST";
        public const string NAME_LAST = "LAST";
        public const string NAME_AVG = "AVG";
        public const string NAME_VARP = "VARP";
        public const string NAME_STDEVP = "STDEVP";
        public const string NAME_VARS = "VARS";
        public const string NAME_STDEVS = "STDEVS";
        public const string NAME_COVAR = "COVAR";
        public const string NAME_CORR = "CORR";
        public const string NAME_INTERCPT = "INTERCEPT";
        public const string NAME_SLOPE = "SLOPE";

        #region ParameterCountTable
        private static Dictionary<string, Tuple<int, int>> _Parameters = new Dictionary<string, Tuple<int, int>>(StringComparer.OrdinalIgnoreCase)
        {

            { NAME_SUM, new Tuple<int,int>(1,1) },
            { NAME_COUNT, new Tuple<int,int>(0,1) },
            { NAME_COUNT_NULL, new Tuple<int,int>(1,1) },
            { NAME_MIN, new Tuple<int,int>(1,2) },
            { NAME_MAX, new Tuple<int,int>(1,2) },
            { NAME_FIRST, new Tuple<int,int>(1,1) },
            { NAME_LAST, new Tuple<int,int>(1,1) },
            { NAME_AVG, new Tuple<int,int>(1,2) },
            { NAME_VARP, new Tuple<int,int>(1,2) },
            { NAME_STDEVP, new Tuple<int,int>(1,2) },
            { NAME_VARS, new Tuple<int,int>(1,2) },
            { NAME_STDEVS, new Tuple<int,int>(1,2) },
            { NAME_COVAR, new Tuple<int,int>(2,3) },
            { NAME_CORR, new Tuple<int,int>(2,3) },
            { NAME_INTERCPT, new Tuple<int,int>(2,3) },
            { NAME_SLOPE, new Tuple<int,int>(2,3) },

        };
        #endregion

        public AggregateLookup()
        {
        }

        public bool Exists(string Name)
        {

            switch (Name.ToUpper().Trim())
            {
                case NAME_SUM:
                case NAME_COUNT:
                case NAME_COUNT_NULL:
                case NAME_MIN:
                case NAME_MAX:
                case NAME_FIRST:
                case NAME_LAST:
                case NAME_AVG:
                case NAME_VARP:
                case NAME_STDEVP:
                case NAME_VARS:
                case NAME_STDEVS:
                case NAME_COVAR:
                case NAME_CORR:
                case NAME_INTERCPT:
                case NAME_SLOPE:
                    return true;
            }

            return false;

        }

        public Aggregate Lookup(string Name, ScalarExpressionSet Parameters, Filter Where)
        {

            // Check the parameters //
            if (!_Parameters.ContainsKey(Name))
                throw new ArgumentException(string.Format("Aggregate '{0}' does not exist", Name));
            Tuple<int, int> p = _Parameters[Name];
            if (Parameters.Count > p.Item2 || Parameters.Count < p.Item1)
                throw new ArgumentException(string.Format("Aggregate '{0}' expects between {1} and {2} parameters", Name, p.Item1, p.Item2));

            switch (Name.ToUpper())
            {

                case NAME_SUM:
                    return new AggregateSum(Parameters[0], Where);
                case NAME_COUNT:
                    if (Parameters.Count == 0)
                        return new AggregateCountStar(Where);
                    return new AggregateCount(Parameters[0], Where);
                case NAME_COUNT_NULL:
                    return new AggregateCountNull(Parameters[0], Where);
                case NAME_MIN:
                    if (Parameters.Count == 2)
                        return new AggregateMinOf(Parameters[0], Parameters[1], Where);
                    return new AggregateMin(Parameters[0], Where);
                case NAME_MAX:
                    if (Parameters.Count == 2)
                        return new AggregateMaxOf(Parameters[0], Parameters[1], Where);
                    return new AggregateMax(Parameters[0], Where);
                case NAME_FIRST:
                    return new AggregateFirst(Parameters[0], Where);
                case NAME_LAST:
                    return new AggregateLast(Parameters[0], Where);
                case NAME_AVG:
                    if (Parameters.Count == 2)
                        return new AggregateAvg(Parameters[0], Parameters[1], Where);
                    return new AggregateAvg(Parameters[0], Where);
                case NAME_VARP:
                    if (Parameters.Count == 2)
                        return new AggregateVarP(Parameters[0], Parameters[1], Where);
                    return new AggregateVarP(Parameters[0], Where);
                case NAME_STDEVP:
                    if (Parameters.Count == 2)
                        return new AggregateStdevP(Parameters[0], Parameters[1], Where);
                    return new AggregateStdevP(Parameters[0], Where);
                case NAME_VARS:
                    if (Parameters.Count == 2)
                        return new AggregateVarS(Parameters[0], Parameters[1], Where);
                    return new AggregateVarS(Parameters[0], Where);
                case NAME_STDEVS:
                    if (Parameters.Count == 2)
                        return new AggregateStdevS(Parameters[0], Parameters[1], Where);
                    return new AggregateStdevS(Parameters[0], Where);
                case NAME_COVAR:
                    if (Parameters.Count == 3)
                        return new AggregateCovariance(Parameters[0], Parameters[1], Parameters[2], Where);
                    return new AggregateCovariance(Parameters[0], Parameters[1], ScalarExpression.OneNUM, Where);
                case NAME_CORR:
                    if (Parameters.Count == 3)
                        return new AggregateCorrelation(Parameters[0], Parameters[1], Parameters[2], Where);
                    return new AggregateCorrelation(Parameters[0], Parameters[1], ScalarExpression.OneNUM, Where);
                case NAME_INTERCPT:
                    if (Parameters.Count == 3)
                        return new AggregateIntercept(Parameters[0], Parameters[1], Parameters[2], Where);
                    return new AggregateIntercept(Parameters[0], Parameters[1], ScalarExpression.OneNUM, Where);
                case NAME_SLOPE:
                    if (Parameters.Count == 3)
                        return new AggregateSlope(Parameters[0], Parameters[1], Parameters[2], Where);
                    return new AggregateSlope(Parameters[0], Parameters[1], ScalarExpression.OneNUM, Where);

            }

            // Shouldn't have made it this far //
            throw new ArgumentException(string.Format("Aggregate '{0}' does not exist", Name));

        }

    }

}
