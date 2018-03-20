using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions.RecordExpressions;

namespace ArcticWind.Expressions.ScalarExpressions
{

    //public sealed class ScalarExpressionOptimizer
    //{

    //    private int _Ticks = 0;
    //    private int _Tocks = 0;
    //    private int _Cycles = 0;
    //    private Host _Host;

    //    public ScalarExpressionOptimizer(Host Host)
    //    {
    //        this._Host = Host;
    //    }

    //    // Compact all //
    //    public ScalarExpression Compact(ScalarExpression Node)
    //    {

    //        // Clone the Spike node //
    //        //Expressions t = Node.CloneOfMe();

    //        this._Tocks = 1;
    //        while (this._Tocks != 0)
    //        {

    //            // Reset the tock variables //
    //            this._Tocks = 0;

    //            // Compact the leaf node; note that we may need to do this again //
    //            Node = CompactUnit(Node);

    //            // Accumulate the ticks //
    //            this._Ticks += this._Tocks;

    //            // Accumulate the cycles //
    //            this._Cycles++;

    //        }

    //        // return the compacted node //
    //        return Node;

    //    }

    //    public int TotalCompacts
    //    {
    //        get { return this._Ticks; }
    //    }

    //    public int Cycles
    //    {
    //        get { return this._Cycles; }
    //    }

    //    private ScalarExpression CompactUnit(ScalarExpression Node)
    //    {

    //        for (int i = 0; i < Node.ChildNodes.Count; i++)
    //            Node.ChildNodes[i] = CompactUnit(Node.ChildNodes[i]);

    //        return CompactSingle(Node);

    //    }

    //    private ScalarExpression CompactSingle(ScalarExpression Node)
    //    {

    //        // The order we do these is optimized to reduce the number of tock loops //
    //        Node = CompactPower(Node);
    //        Node = CompactMultDivMod(Node);
    //        Node = CompactAddSub(Node);
    //        Node = CompactUni(Node);
    //        Node = CompactCancleOut(Node);
    //        Node = CompactStaticArguments(Node);

    //        return Node;

    //    }

    //    // A - A -> 0
    //    // A / A -> 1
    //    private ScalarExpression CompactCancleOut(ScalarExpression Node)
    //    {

    //        if (Node.Affinity != ScalarExpressionAffinity.Function)
    //            return Node;

    //        ScalarExpressionFunction x = (Node as ScalarExpressionFunction);
    //        string name = x.Name;

    //        // Check that the node is either - or /, /?
    //        if (name != ScalarExpressionFunction.NAME_SUB && name != ScalarExpressionFunction.NAME_DIV && name != ScalarExpressionFunction.NAME_CDIV)
    //            return Node;

    //        // Build an equality checker //
    //        IEqualityComparer<ScalarExpression> lne = new ExpressionComparer();

    //        // Check if A == B //
    //        if (!lne.Equals(Node.ChildNodes[0], Node.ChildNodes[1]))
    //            return Node;

    //        // Check for A - A -> 0 //
    //        if (name == ScalarExpressionFunction.NAME_SUB)
    //            return new ScalarExpressionConstant(Node.ParentNode, CellValues.Zero(CellAffinity.DOUBLE));

    //        // Check for A / A -> 0 //
    //        if (name == ScalarExpressionFunction.NAME_DIV || name == ScalarExpressionFunction.NAME_CDIV)
    //            return new ScalarExpressionConstant(Node.ParentNode, CellValues.One(CellAffinity.DOUBLE));

    //        return Node;

    //    }

    //    // -(-A) -> A
    //    // !(!A) -> A
    //    // -b -> -b where b is a constant
    //    // !b -> !b where b is a constant
    //    private ScalarExpression CompactUni(ScalarExpression Node)
    //    {

    //        if (Node.Affinity != ScalarExpressionAffinity.Function)
    //            return Node;

    //        ScalarExpressionFunction x = (Node as ScalarExpressionFunction);
    //        string name = x.Name;

    //        // Check that the node is either -A, +A, !A //
    //        if (name != ScalarExpressionFunction.NAME_MINUS && name != ScalarExpressionFunction.NAME_PLUS && name != ScalarExpressionFunction.NAME_NOT)
    //            return Node;

    //        // Check for the child being a constant //
    //        if (Node.ChildNodes[0].Affinity == ScalarExpressionAffinity.Value)
    //        {
    //            Cell c = (Node.ChildNodes[0] as ScalarExpressionConstant).Value;
    //            if (name == ScalarExpressionFunction.NAME_MINUS)
    //                c = -c;
    //            if (name == ScalarExpressionFunction.NAME_NOT)
    //                c = !c;
    //            return new ScalarExpressionConstant(Node.ParentNode, c);
    //        }

    //        // Check that A = F(B) //
    //        if (Node.ChildNodes[0].Affinity != ScalarExpressionAffinity.Function)
    //            return Node;

    //        // Get the name of the function of the child node //
    //        string sub_name = (Node.ChildNodes[0] as ScalarExpressionFunction).Name;

    //        // Check for -(-A) //
    //        if (name == ScalarExpressionFunction.NAME_MINUS && sub_name == ScalarExpressionFunction.NAME_MINUS)
    //            return Node.ChildNodes[0].ChildNodes[0];

    //        // Check for !(!A) //
    //        if (name == ScalarExpressionFunction.NAME_NOT && sub_name == ScalarExpressionFunction.NAME_NOT)
    //            return Node.ChildNodes[0].ChildNodes[0];

    //        return Node;

    //    }

    //    // A + 0 or 0 + A or A - 0 -> A
    //    // 0 - A -> -A 
    //    // A + -B -> A - B
    //    private ScalarExpression CompactAddSub(ScalarExpression Node)
    //    {

    //        if (Node.Affinity != ScalarExpressionAffinity.Function)
    //            return Node;

    //        ScalarExpressionFunction x = (Node as ScalarExpressionFunction);
    //        string name = x.Name;

    //        if (name != ScalarExpressionFunction.NAME_ADD && name != ScalarExpressionFunction.NAME_SUB)
    //            return Node;

    //        // Look for A + 0 or A - 0 -> A //
    //        if (IsStaticZero(Node.ChildNodes[1]))
    //        {
    //            this._Tocks++;
    //            return Node.ChildNodes[0];
    //        }

    //        // Look for 0 + A -> A //
    //        if (IsStaticZero(Node.ChildNodes[0]) && name == ScalarExpressionFunction.NAME_ADD)
    //        {
    //            this._Tocks++;
    //            return Node.ChildNodes[1];
    //        }

    //        // Look for 0 - A -> -A //
    //        if (IsStaticZero(Node.ChildNodes[0]) && name == ScalarExpressionFunction.NAME_SUB)
    //        {
    //            this._Tocks++;
    //            ScalarExpression t = new ScalarExpressionFunction.ExpressionMinus(this._Host);
    //            t.ParentNode = Node.ParentNode;
    //            t.AddChildNode(Node.ChildNodes[1]);
    //            return t;
    //        }

    //        // Look for A + -B -> A - B //
    //        if (IsUniNegative(Node.ChildNodes[1]) && name == ScalarExpressionFunction.NAME_ADD)
    //        {
    //            this._Tocks++;
    //            ScalarExpression t = new ScalarExpressionFunction.ExpressionSubtract(this._Host);
    //            t.ParentNode = Node.ParentNode;
    //            t.AddChildNode(Node.ChildNodes[0]);
    //            t.AddChildNode(Node.ChildNodes[1].ChildNodes[0]);
    //            return t;
    //        }

    //        // Look for -A + B -> B - A //
    //        if (IsUniNegative(Node.ChildNodes[0]) && name == ScalarExpressionFunction.NAME_ADD)
    //        {
    //            this._Tocks++;
    //            ScalarExpression t = new ScalarExpressionFunction.ExpressionSubtract(this._Host);
    //            t.ParentNode = Node.ParentNode;
    //            t.AddChildNode(Node.ChildNodes[0]);
    //            t.AddChildNode(Node.ChildNodes[0].ChildNodes[0]);
    //            return t;
    //        }

    //        // Look for A - -B -> A + B //
    //        if (IsUniNegative(Node.ChildNodes[1]) && name == ScalarExpressionFunction.NAME_SUB)
    //        {
    //            this._Tocks++;
    //            ScalarExpression t = new ScalarExpressionFunction.ExpressionPlus(this._Host);
    //            t.ParentNode = Node.ParentNode;
    //            t.AddChildNode(Node.ChildNodes[0]);
    //            t.AddChildNode(Node.ChildNodes[1].ChildNodes[0]);
    //            return t;
    //        }

    //        return Node;

    //    }

    //    // A * 1 or 1 * A or A / 1 or A /? 1 or A % 1 -> A 
    //    // A * -1 or -1 * A or A / -1 or A /? -1 or A % -1 -> -A 
    //    // A * 0, 0 * A, 0 / A, 0 /? A, A /? 0, 0 % A -> 0 
    //    // A / 0, A % 0 -> null
    //    private ScalarExpression CompactMultDivMod(ScalarExpression Node)
    //    {

    //        if (Node.Affinity != ScalarExpressionAffinity.Function)
    //            return Node;

    //        ScalarExpressionFunction x = (Node as ScalarExpressionFunction);
    //        string name = x.Name;

    //        if (name != ScalarExpressionFunction.NAME_MULT
    //            && name != ScalarExpressionFunction.NAME_DIV
    //            && name != ScalarExpressionFunction.NAME_CDIV
    //            && name != ScalarExpressionFunction.NAME_MOD)
    //            return Node;

    //        // A * 1 or A / 1 or A /? 1 or A % 1 //
    //        if (IsStaticOne(Node.ChildNodes[1]))
    //        {
    //            this._Tocks++;
    //            return Node.ChildNodes[0];
    //        }

    //        // 1 * A //
    //        if (IsStaticOne(Node.ChildNodes[0]) && name == ScalarExpressionFunction.NAME_MULT)
    //        {
    //            this._Tocks++;
    //            return Node.ChildNodes[1];
    //        }

    //        // A * -1 or A / -1 or A /? -1 or A % -1 //
    //        if (IsStaticMinusOne(Node.ChildNodes[1]))
    //        {
    //            this._Tocks++;
    //            ScalarExpression t = new ScalarExpressionFunction.ExpressionMinus(this._Host);
    //            t.ParentNode = Node.ParentNode;
    //            t.AddChildNode(Node.ChildNodes[0]);
    //            return t;
    //        }

    //        // -1 * A //
    //        if (IsStaticMinusOne(Node.ChildNodes[0]) && name == ScalarExpressionFunction.NAME_MULT)
    //        {
    //            this._Tocks++;
    //            ScalarExpression t = new ScalarExpressionFunction.ExpressionMinus(this._Host);
    //            t.ParentNode = Node.ParentNode;
    //            t.AddChildNode(Node.ChildNodes[1]);
    //            return t;
    //        }

    //        // Look 0 * A, 0 / A, 0 /? A, 0 % A //
    //        if (IsStaticZero(Node.ChildNodes[0]))
    //        {
    //            this._Tocks++;
    //            return new ScalarExpressionConstant(Node.ParentNode, new Cell(0.00));
    //        }

    //        // A * 0, A /? 0 //
    //        if (IsStaticZero(Node.ChildNodes[1]) && (name == ScalarExpressionFunction.NAME_MULT || name == ScalarExpressionFunction.NAME_CDIV))
    //        {
    //            this._Tocks++;
    //            return new ScalarExpressionConstant(Node.ParentNode, new Cell(0.00));
    //        }

    //        // A / 0, A % 0 //
    //        if (IsStaticZero(Node.ChildNodes[1]) && (name == ScalarExpressionFunction.NAME_DIV || name == ScalarExpressionFunction.NAME_MOD))
    //        {
    //            this._Tocks++;
    //            return new ScalarExpressionConstant(Node.ParentNode, CellValues.NullDOUBLE);
    //        }

    //        return Node;

    //    }

    //    // 1 * 2 + 3 -> 5
    //    private ScalarExpression CompactStaticArguments(ScalarExpression Node)
    //    {

    //        if (ChildrenAreAllStatic(Node))
    //        {
    //            this._Tocks++;
    //            return new ScalarExpressionConstant(Node.ParentNode, Node.Evaluate(new FieldResolver(this._Host)));
    //        }

    //        return Node;

    //    }

    //    // power(A,1) -> A
    //    // power(A,0) -> 1
    //    private ScalarExpression CompactPower(ScalarExpression Node)
    //    {

    //        if (Node.Affinity != ScalarExpressionAffinity.Function)
    //            return Node;

    //        ScalarExpressionFunction x = (Node as ScalarExpressionFunction);
    //        string name = x.Name;

    //        if (name != ScalarExpressionFunction.NAME_POWER)
    //            return Node;

    //        // Check the second argument of power(A,B) looking for B == 1 //
    //        if (IsStaticOne(Node.ChildNodes[1]))
    //            return Node.ChildNodes[0];

    //        // Check the second argumnet of power(A,B) looging for B == 0, if so return static 1.000, even power(0,0) = 1.000 //
    //        if (IsStaticZero(Node.ChildNodes[1]))
    //            return new ScalarExpressionConstant(Node.ParentNode, new Cell(1.000));

    //        return Node;

    //    }

    //    // Helpers //
    //    public static bool IsStaticZero(ScalarExpression Node)
    //    {
    //        if (Node.Affinity == ScalarExpressionAffinity.Value)
    //            return (Node as ScalarExpressionConstant).Value == CellValues.Zero(Node.ReturnAffinity());
    //        return false;
    //    }

    //    public static bool IsStaticOne(ScalarExpression Node)
    //    {
    //        if (Node.Affinity == ScalarExpressionAffinity.Value)
    //            return (Node as ScalarExpressionConstant).Value == CellValues.Zero(Node.ReturnAffinity());
    //        return false;
    //    }

    //    public static bool IsStaticMinusOne(ScalarExpression Node)
    //    {
    //        if (Node.Affinity == ScalarExpressionAffinity.Value)
    //            return (Node as ScalarExpressionConstant).Value == -CellValues.One(Node.ReturnAffinity());
    //        if (Node.Affinity == ScalarExpressionAffinity.Function)
    //        {
    //            ScalarExpressionFunction x = (Node as ScalarExpressionFunction);
    //            if (x.Name == ScalarExpressionFunction.NAME_MINUS && IsStaticOne(x.ChildNodes[0]))
    //                return true;
    //        }
    //        return false;
    //    }

    //    public static bool IsUniNegative(ScalarExpression Node)
    //    {
    //        if (Node.Affinity == ScalarExpressionAffinity.Function)
    //        {
    //            ScalarExpressionFunction x = (Node as ScalarExpressionFunction);
    //            return x.Name == ScalarExpressionFunction.NAME_MINUS;
    //        }
    //        return false;
    //    }

    //    public static bool ChildrenAreAllStatic(ScalarExpression Node)
    //    {

    //        if (Node.IsTerminal)
    //            return false;

    //        foreach (ScalarExpression n in Node.ChildNodes)
    //        {
    //            if (n.Affinity != ScalarExpressionAffinity.Value)
    //                return false;
    //        }
    //        return true;

    //    }

    //    // Opperands //
    //    public static ScalarExpression CompactNode(Host Host, ScalarExpression Node)
    //    {
    //        ScalarExpressionOptimizer lnc = new ScalarExpressionOptimizer(Host);
    //        return lnc.Compact(Node);
    //    }

    //    public static ScalarExpressionSet CompactTree(Host Host, ScalarExpressionSet Tree)
    //    {

    //        ScalarExpressionSet t = new ScalarExpressionSet(Host);
    //        ScalarExpressionOptimizer lnc = new ScalarExpressionOptimizer(Host);

    //        for (int i = 0; i < Tree.Count; i++)
    //        {
    //            ScalarExpression n = lnc.Compact(Tree[i]);
    //            t.Add(Tree.Alias(i), n);
    //        }

    //        return t;

    //    }


    //}

    //public class ExpressionComparer : IEqualityComparer<ScalarExpression>
    //{

    //    bool IEqualityComparer<ScalarExpression>.Equals(ScalarExpression T1, ScalarExpression T2)
    //    {

    //        if (T1.Affinity != T2.Affinity)
    //            return false;

    //        if (T1.Affinity == ScalarExpressionAffinity.Field)
    //            return (T1 as ScalarExpressionRecordElementRef).GetHashCode() == (T2 as ScalarExpressionRecordElementRef).GetHashCode();

    //        if (T1.Affinity == ScalarExpressionAffinity.Pointer)
    //            return (T1 as ScalarExpressionPointer).PointerName == (T2 as ScalarExpressionPointer).PointerName;

    //        //if (T1.Affinity == ExpressionAffinity.HeapReExpression)
    //        //    return (T1 as ExpressionHeapRef).HeapRef == (T2 as ExpressionHeapRef).HeapRef && (T1 as ExpressionHeapRef).Pointer == (T2 as ExpressionHeapRef).Pointer;

    //        if (T1.Affinity == ScalarExpressionAffinity.Value)
    //            return (T1 as ScalarExpressionConstant).Value == (T2 as ScalarExpressionConstant).Value;

    //        return T1.NodeID == T2.NodeID;

    //    }

    //    int IEqualityComparer<ScalarExpression>.GetHashCode(ScalarExpression T)
    //    {

    //        if (T.Affinity == ScalarExpressionAffinity.Field)
    //            return (T as ScalarExpressionRecordElementRef).GetHashCode();

    //        if (T.Affinity == ScalarExpressionAffinity.Pointer)
    //            return (T as ScalarExpressionPointer).PointerName.GetHashCode();

    //        //if (T.Affinity == ExpressionAffinity.HeapReExpression)
    //        //    return (T as ExpressionHeapRef).HeapRef.Columns.GetHashCode() ^ (T as ExpressionHeapRef).Pointer;

    //        if (T.Affinity == ScalarExpressionAffinity.Value)
    //            return (T as ScalarExpressionConstant).Value.GetHashCode();

    //        return T.NodeID.GetHashCode();

    //    }

    //}



}
