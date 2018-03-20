using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;

namespace ArcticWind.Tables
{

    /// <summary>
    /// Represents the state of a cluster
    /// </summary>
    public enum ClusterState : byte
    {

        /// <summary>
        /// Allows duplicates
        /// </summary>
        Universal,

        /// <summary>
        /// Does not allow duplicates and errors out if duplicates are passed
        /// </summary>
        Unique,

        /// <summary>
        /// Does not allow duplicates, but doesnt error out; instead it ignores any duplicate records and does not insert them into the table
        /// </summary>
        Distinct

    }

}
