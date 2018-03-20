using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticWind.Util
{

    /// <summary>
    /// Contains functions to evaluate query costs
    /// </summary>
    public static class CostCalculator
    {

        public static double LogN(long N)
        {
            return Math.Log((double)N, 2D);
        }

        public static double NLogN(long N)
        {
            return (double)N * Math.Log((double)N, 2D);
        }

        public static double SumLogI(long N)
        {
            return SpecialFunctions.LogGamma((double)N) / SpecialFunctions.LN2;
        }

        // Index Costs //
        /// <summary>
        /// Returns the cost for searching an index
        /// </summary>
        /// <param name="RecordCount"></param>
        /// <returns></returns>
        public static double IndexSeekCost(long RecordCount)
        {
            return CostCalculator.LogN(RecordCount);
        }

        /// <summary>
        /// Returns the cost for building an index
        /// </summary>
        /// <param name="RecordCount"></param>
        /// <returns></returns>
        public static double IndexBuildCost(long RecordCount)
        {
            return CostCalculator.SumLogI(RecordCount);
        }

        // Join Costs //
        /// <summary>
        /// Returns the cost for performing a nested loop join
        /// </summary>
        /// <param name="ARecordCount"></param>
        /// <param name="BRecordCount"></param>
        /// <returns></returns>
        public static double NestedLoopJoinCost(long ARecordCount, long BRecordCount)
        {
            return ARecordCount * BRecordCount;
        }

        /// <summary>
        /// Returns the cost for performing a quasi-nested loop join
        /// </summary>
        /// <param name="ARecordCount"></param>
        /// <param name="BRecordCount"></param>
        /// <param name="BuildRightIndex"></param>
        /// <returns></returns>
        public static double QuasiNestedLoopJoinCost(long ARecordCount, long BRecordCount, bool BuildRightIndex)
        {
            return ARecordCount * CostCalculator.LogN(BRecordCount) + (BuildRightIndex ? CostCalculator.IndexBuildCost(BRecordCount) : 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ARecordCount"></param>
        /// <param name="BRecordCount"></param>
        /// <param name="BuildLeftIndex"></param>
        /// <param name="BuildRightIndex"></param>
        /// <returns></returns>
        public static double SortMergeNestedLoopJoinCost(long ARecordCount, long BRecordCount, bool BuildLeftIndex, bool BuildRightIndex)
        {
            return (double)Math.Max(ARecordCount, BRecordCount) 
                + (BuildLeftIndex ? CostCalculator.IndexBuildCost(ARecordCount) : 0) 
                + (BuildRightIndex ? CostCalculator.IndexBuildCost(BRecordCount) : 0);
        }

        // Group By Costs //
        /// <summary>
        /// Represents the cost to group data from an ordered stream
        /// </summary>
        /// <param name="RecordCount"></param>
        /// <param name="BuildIndex"></param>
        /// <returns></returns>
        public static double OrderedGroupByCost(long RecordCount, bool BuildIndex)
        {
            return (double)RecordCount + (BuildIndex ? CostCalculator.IndexBuildCost(RecordCount) : 0);
        }

        /// <summary>
        /// Represents the cost to group by using a dictionary
        /// </summary>
        /// <param name="RecordCount"></param>
        /// <returns></returns>
        public static double DictionaryGroupByCost(long RecordCount)
        {
            return (double)RecordCount + CostCalculator.IndexBuildCost(RecordCount);
        }

    }

}
