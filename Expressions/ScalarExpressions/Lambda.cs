using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind;
using ArcticWind.Elements;

namespace ArcticWind.Expressions.ScalarExpressions
{
    
    //public sealed class Lambda
    //{

    //    private ScalarExpression _Expression;
    //    private string _Name;
    //    private List<string> _Pointers;

    //    // Constructor //
    //    public Lambda(string Name, ScalarExpression Expression, List<string> Parameters)
    //    {
            
    //        this._Expression = Expression;
    //        this._Name = Name;
    //        this._Pointers = Parameters;
    //        this.Count = Parameters.Count;
            
    //    }

    //    public Lambda(string Name, ScalarExpression Expression)
    //        : this(Name, Expression, Analytics.AllPointersRefs(Expression).Distinct().ToList())
    //    {
    //    }

    //    public Lambda(string Name, ScalarExpression Expression, string Parameter)
    //        : this(Name, Expression, new List<string>() { Parameter })
    //    {
    //    }

    //    // Properties //
    //    public ScalarExpression InnerNode
    //    {
    //        get { return this._Expression; }
    //    }

    //    public string Name
    //    {
    //        get { return this._Name; }
    //    }

    //    public List<string> Pointers
    //    {
    //        get { return this._Pointers; }
    //    }

    //    public int Count
    //    {
    //        get;
    //        private set;
    //    }

    //    // Methods //
    //    public ScalarExpression Bind(List<ScalarExpression> Bindings)
    //    {

    //        ScalarExpression node = this._Expression.CloneOfMe();
    //        List<ScalarExpressionPointer> refs = Analytics.AllPointers(node);

    //        Dictionary<string, int> idx = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    //        int i = 0;
    //        foreach (string s in this._Pointers)
    //        {
    //            idx.Add(s, i);
    //            i++;
    //        }

    //        foreach (ScalarExpressionPointer n in refs)
    //        {
    //            int node_ref = idx[n.Name];
    //            ScalarExpression t = Bindings[node_ref];
    //            Analytics.ReplaceNode(n, t);
    //        }

    //        return node; 

    //    }

    //    public Lambda CloneOfMe()
    //    {

    //        return new Lambda(this._Name, this._Expression.CloneOfMe(), this._Pointers);

    //    }

    //}

}
