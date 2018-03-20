using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.TableExpressions;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Expressions;

namespace ArcticWind.Expressions.ActionExpressions
{

    public abstract class ActionExpressionParameterized : ActionExpression
    {

        protected string _Name;
        protected int _MaxParamterCount;
        protected string _InfoString;
        protected List<Parameter> _Parameters;

        public ActionExpressionParameterized(Host Host, ActionExpression Parent, string Name, int MaxParameters, string InfoString)
            : base(Host, Parent)
        {

            this._Name = Name;
            this._InfoString = InfoString;
            this._Parameters = new List<Parameter>();
            this._MaxParamterCount = MaxParameters;

        }

        public ActionExpressionParameterized(Host Host, ActionExpression Parent, string Name, int MaxParameters)
            : this(Host, Parent, Name, MaxParameters, Name)
        {
        }

        // Properties //
        public string Name
        {
            get { return this._Name; }
        }

        public string InfoString
        {
            get { return this._InfoString; }
        }

        // Other //
        /// <summary>
        /// 
        /// </summary>
        public void CheckParameters()
        {

            if (this._MaxParamterCount < 0 && this._Parameters.Count > (-this._MaxParamterCount))
            {
                throw new Exception(string.Format("Function '{0}' can have at most '{1}' parameter(s) but was passed '{2}'", this._Name, -this._MaxParamterCount, this._Parameters.Count));
            }
            else if (this._Parameters.Count != this._MaxParamterCount)
            {
                throw new Exception(string.Format("Function '{0}' can have exactly '{1}' parameter(s) but was passed '{2}'", this._Name, -this._MaxParamterCount, this._Parameters.Count));
            }

        }

        public virtual void AddParameter(Parameter Value)
        {
            this._Parameters.Add(Value);
        }

        public bool AllScalars()
        {
            return this._Parameters.TrueForAll((x) => { return x.Affinity == ParameterAffinity.Scalar; });
        }

        public bool AllMatrixes()
        {
            return this._Parameters.TrueForAll((x) => { return x.Affinity == ParameterAffinity.Matrix; });
        }

        public bool AllRecords()
        {
            return this._Parameters.TrueForAll((x) => { return x.Affinity == ParameterAffinity.Record; });
        }

        public bool AllTables()
        {
            return this._Parameters.TrueForAll((x) => { return x.Affinity == ParameterAffinity.Table; });
        }

        public bool CheckSigniture(params ParameterAffinity[] Paramters)
        {

            if (Paramters == null)
            {
                return (this._Parameters == null || this._Parameters.Count == 0);
            }

            if (this._Parameters.Count != Paramters.Length)
                return false;

            for (int i = 0; i < this._Parameters.Count; i++)
            {
                if (this._Parameters[i].Affinity != Paramters[i])
                    return false;
            }

            return true;

        }


    }

}
