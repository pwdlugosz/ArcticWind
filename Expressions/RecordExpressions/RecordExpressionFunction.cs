using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Expressions.TableExpressions;
using ArcticWind.Tables;

namespace ArcticWind.Expressions.RecordExpressions
{


    public abstract class RecordExpressionFunction : RecordExpression
    {

        private string _Name;
        private int _ParamCount = -1;
        protected List<Parameter> _Parameters;

        public RecordExpressionFunction(Host Host, RecordExpression Parent, string Name, int Parameters)
            : base(Host, Parent)
        {
            this._Name = Name;
            this._ParamCount = Parameters;
            this._Parameters = new List<Parameter>();
        }

        public string FunctionName
        {
            get { return this._Name; }
        }

        public virtual bool IsVolatile
        {
            get { return true; }
        }

        public int ParameterCount
        {
            get { return this._ParamCount; }
        }

        public void AddParameter(Parameter Value)
        {
            this._Parameters.Add(Value);
        }

        public void CheckParameters()
        {

            if (this._ParamCount < 0 && this._Parameters.Count > (-this._ParamCount))
            {
                throw new Exception(string.Format("Function '{0}' can have at most '{1}' parameter(s) but was passed '{2}'", this._Name, -this._ParamCount, this._Parameters.Count));
            }
            else if (this._Parameters.Count != this._ParamCount)
            {
                throw new Exception(string.Format("Function '{0}' can have exactly '{1}' parameter(s) but was passed '{2}'", this._Name, -this._ParamCount, this._Parameters.Count));
            }

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
