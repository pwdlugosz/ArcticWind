using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions;
using ArcticWind.Expressions.ScalarExpressions;


namespace ArcticWind.Expressions.ActionExpressions
{

    public abstract class ActionExpression 
    {

        public enum BreakLevel : byte
        {
            NoBreak = 0,
            Break = 1,
            Return = 2
        }

        protected Host _Host;
        protected ActionExpression _Parent;
        protected List<ActionExpression> _Children;
        protected BreakLevel _Break = BreakLevel.NoBreak;
        
        public ActionExpression(Host Host, ActionExpression Parent)
        {
            this._Host = Host;
            this._Parent = Parent;
            this._Children = new List<ActionExpression>();
        }

        public ActionExpression Parent
        {
            get { return this._Parent; }
        }

        public List<ActionExpression> Children
        {
            get { return this._Children; }
        }

        public void AddChild(ActionExpression Node)
        {
            Node._Parent = this;
            this._Children.Add(Node);
        }

        public void AddChilren(List<ActionExpression> Nodes)
        {
            foreach(ActionExpression ae in Nodes)
            {
                this.AddChild(ae);
            }
        }

        // Breaks //
        public BreakLevel Condition
        {

            get
            {
                return this._Break;
            }
            set
            {
                this._Break = value;
            }

        }

        public virtual bool IsBreakAble
        {
            get { return false; }
        }

        public void CommunicateBreak()
        {

            ActionExpression ae = this._Parent;
            while (ae != null)
            {

                if (ae.IsBreakAble)
                {
                    ae._Break = BreakLevel.Break;
                    break;
                }
                else
                {
                    ae = ae._Parent;
                }

            }

        }

        public void CommunicateReturn()
        {

            ActionExpression ae = this._Parent;
            while (ae != null)
            {

                if (ae._Parent == null)
                {
                    ae._Break = BreakLevel.Return;
                    break;
                }

                ae = ae._Parent;

            }

        }

        // Virtuals and Abstracts //
        public virtual void BeginInvoke(SpoolSpace Variant)
        {
        }

        public virtual void EndInvoke(SpoolSpace Variant)
        {
        }

        public virtual void Bind(string PointerRef, ScalarExpression Value)
        {
            this._Children.ForEach((x) => { x.Bind(PointerRef, Value); });
        }

        public virtual SpoolSpace CreateResolver()
        {

            SpoolSpace f = new SpoolSpace();
            return f;

        }

        public abstract void Invoke(SpoolSpace Variant);

    }

}
