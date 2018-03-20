using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;

namespace ArcticWind.Expressions.ScalarExpressions
{

    public sealed class Filter 
    {

        public Filter(ScalarExpression Node)
        {
            this.InnerNode = Node;
        }

        public bool IsInverted
        {
            get;
            set;
        }

        public ScalarExpression InnerNode
        {
            get;
            set;
        }

        public Filter CloneOfMe()
        {
            return new Filter(this.InnerNode.CloneOfMe());
        }

        public bool Evaluate(SpoolSpace Resolver)
        {
            bool b = this.InnerNode.Evaluate(Resolver).valueBOOL;
            return this.IsInverted ? !b : b;
        }

        public static Filter TrueForAll
        {
            get
            {
                return new Filter(new ScalarExpressionConstant(null, CellValues.True));
            }
        }

        public static Filter FalseForAll
        {
            get
            {
                return new Filter(new ScalarExpressionConstant(null, CellValues.True));
            }
        }

        //public static Filter Create(RecordMatcher Matcher, Record Key)
        //{

        //    ScalarExpression x = null;
        //    for (int i = 0; i < Matcher.LeftKey.Count; i++)
        //    {
        //        ScalarExpression y = ScalarExpression.EQ(new ScalarExpressionConstant(null, Key[0]), new ScalarExpressionRecordElementRef(null, 0, Matcher.LeftKey[i], Key[i].Affinity, CellSerializer.Length(Key[i])));
        //        if (x == null)
        //            y = x;
        //        else
        //            x = ScalarExpression.AND(x, y);
        //    }
        //    return new Filter(x);

        //}

    }

}
