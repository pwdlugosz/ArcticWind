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
    
    public abstract class RecordExpression
    {

        protected Host _Host;
        protected RecordExpression _Parent;
        
        public RecordExpression(Host Host, RecordExpression Parent)
        {
            this._Host = Host;
            this._Parent = Parent;
        }

        public RecordExpression Parent
        {
            get { return this._Parent; }
        }

        public abstract Schema Columns
        {
            get;
        }

        public abstract RecordExpression CloneOfMe();

        public abstract AssociativeRecord EvaluateAssociative(SpoolSpace Variants);

        public virtual void Bind(string Name, Parameter Value)
        {
            // Do something
        }

        public virtual Record Evaluate(SpoolSpace Variants)
        {
            return this.EvaluateAssociative(Variants);
        }

    }

}
