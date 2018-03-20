using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticWind.Elements.Structures
{
    
    public class BinaryTree<T>
    {

        /// <summary>
        /// Represents the method to search a tree
        /// </summary>
        public enum BinarySearchOption
        {

            /// <summary>
            /// Obtain the first value
            /// </summary>
            First,

            /// <summary>
            /// Obtain any value
            /// </summary>
            Any,

            /// <summary>
            /// Obtain the last value
            /// </summary>
            Last

        }

        /// <summary>
        /// Represents a binary tree option
        /// </summary>
        public enum BinaryTreeOption
        {

            /// <summary>
            /// The tree cannot contain duplicates, throws an exception if a duplicate is added
            /// </summary>
            Unique,

            /// <summary>
            /// The tree cannot contain duplicates, but won't throw an exception if a value is added
            /// </summary>
            Distinct,

            /// <summary>
            /// The tree can contain duplicates
            /// </summary>
            Wild

        }

        /// <summary>
        /// Represents a node in a tree
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public sealed class BinaryNode<T>
        {

            public BinaryNode(BinaryNode<T> Parent, T Value)
            {
                this.Parent = Parent;
                this.Value = Value;
            }

            /// <summary>
            /// The parent node
            /// </summary>
            public BinaryNode<T> Parent { get; private set; }

            /// <summary>
            /// The node with a greater than or equal to value
            /// </summary>
            public BinaryNode<T> Upper { get; private set; }

            /// <summary>
            /// The node with a less than or equal to value
            /// </summary>
            public BinaryNode<T> Lower { get; private set; }

            /// <summary>
            /// The value of the node
            /// </summary>
            public T Value { get; private set; }

            /// <summary>
            /// True if the node has no Lower or Upper children
            /// </summary>
            public bool IsTerminal
            {
                get { return this.Upper == null && this.Lower == null; }
            }

            /// <summary>
            /// True if the node has no parent node
            /// </summary>
            public bool IsRoot
            {
                get { return this.Parent == null; }
            }

            /// <summary>
            /// True if the element is the lower value on it's parent node
            /// </summary>
            public bool IsLower
            {
                get 
                {
                    if (this.Parent == null) return false;
                    if (this.Parent.Upper == null) return false;
                    return BinaryNode<T>.ReferenceEquals(this, this.Parent.Upper); 
                }
            }

            /// <summary>
            /// True if the element is the upper value on it's parent node
            /// </summary>
            public bool IsUpper
            {
                get
                {
                    if (this.Parent == null) return false;
                    if (this.Parent.Lower == null) return false;
                    return BinaryNode<T>.ReferenceEquals(this, this.Parent.Lower);
                }
            }

            /// <summary>
            /// Gets the count of values in this node
            /// </summary>
            public int Count
            {

                get
                {

                    int l = (this.Lower == null ? 0 : this.Lower.Count);
                    int u = (this.Upper == null ? 0 : this.Upper.Count);
                    return u + l + 1;

                }

            }

            // Operations //
            /// <summary>
            /// Inserts a value into a tree
            /// </summary>
            /// <param name="Radix"></param>
            /// <param name="Value"></param>
            /// <param name="Comparer"></param>
            public static void Insert(BinaryNode<T> Radix, BinaryNode<T> Element, IComparer<T> Comparer)
            {

                int c = Comparer.Compare(Element.Value, Radix.Value);

                // Value is greater than Radix.Value //
                if (c >= 0)
                {

                    // Radix.Upper is empty (null) //
                    if (Radix.Upper == null)
                    {
                        Radix.Upper = Element;
                        Radix.Upper.Parent = Radix;
                    }
                    // Otherwise, insert the value into the lower node //
                    else
                    {
                        BinaryNode<T>.Insert(Radix.Upper, Element, Comparer);
                    }

                }
                // Value is lower than Radix.Value //
                else
                {

                    // Radix.Lower is empty (null) //
                    if (Radix.Lower == null)
                    {
                        Radix.Lower = Element;
                        Radix.Lower.Parent = Radix;
                    }
                    // Otherwise, insert the value into the upper node //
                    else
                    {
                        BinaryNode<T>.Insert(Radix.Lower, Element, Comparer);
                    }

                }

            }

            /// <summary>
            /// Removes the node from the tree
            /// </summary>
            /// <param name="Element"></param>
            /// <param name="Comparer"></param>
            public static void Remove(BinaryNode<T> Element, IComparer<T> Comparer)
            {

                if (Element == null) 
                    return;

                if (Element.Parent == null && Element.Upper == null && Element.Lower == null) 
                    return;

                bool ParentIsNull = (Element.Parent == null);
                bool LowerIsNull = (Element.Upper == null); 
                bool UpperIsNull = (Element.Lower == null);

                BinaryNode<T> Parent = Element.Parent;

                if (LowerIsNull && UpperIsNull)
                {

                    if (Element.IsUpper)
                        Parent.Lower = null;
                    else
                        Parent.Upper = null;

                }
                else if (LowerIsNull)
                {

                    if (Element.IsUpper && !ParentIsNull)
                        Parent.Lower = Element.Lower;
                    else if (Element.IsLower && !ParentIsNull)
                        Parent.Upper = Element.Lower;
                    Element.Lower.Parent = Parent;

                }
                else if (UpperIsNull)
                {

                    if (Element.IsUpper && !ParentIsNull)
                        Parent.Lower = Element.Upper;
                    else if (Element.IsLower && !ParentIsNull)
                        Parent.Upper = Element.Upper;
                    Element.Upper.Parent = Parent;

                }
                else // Neither are null
                {

                    if (Element.IsUpper && !ParentIsNull)
                        Parent.Lower = Element.Lower;
                    else if (Element.IsLower && !ParentIsNull)
                        Parent.Upper = Element.Lower;
                    Element.Lower.Parent = Parent;

                    Insert(Element.Lower, Element.Upper, Comparer);

                }

            }

            /// <summary>
            /// Searches a node for a given value; returns null if the value is not found
            /// </summary>
            /// <param name="Radix"></param>
            /// <param name="Value"></param>
            /// <param name="Comparer"></param>
            /// <param name="Option"></param>
            /// <returns></returns>
            public static BinaryNode<T> Search(BinaryNode<T> Radix, T Value, IComparer<T> Comparer, BinarySearchOption Option)
            {

                
                // Check first if we're searching a null node //
                if (Radix == null)
                    return Radix;
                
                int c = Comparer.Compare(Value, Radix.Value);

                // We found a match //
                if (c == 0 && Option == BinarySearchOption.Any)
                {
                    return Radix;
                }
                
                // Handle the cases where Upper/Lower are null //
                if (c < 0 && Radix.Lower == null)
                    return null;
                else if (c > 0 && Radix.Upper == null)
                    return null;
                
                // If the lower node is null, then Value > Radix.Upper.Value //
                int clower = (Radix.Lower == null ? 1 : Comparer.Compare(Value, Radix.Lower.Value));

                // If the upper node is null, then Value < Radix.Lower.Value //
                int cupper = (Radix.Upper == null ? -1 : Comparer.Compare(Value, Radix.Upper.Value));

                // Check again for equality //
                if (c == 0)
                {
                    
                    // If we equal the lower node and we're looking for the first node, search the lower node //
                    if (clower == 0 && Option == BinarySearchOption.First)
                        return Search(Radix.Lower, Value, Comparer, Option);
                    
                    // If we are equal to the upper node and we're looking for the last node, search the upper node //
                    if (cupper == 0 && Option == BinarySearchOption.Last)
                        return Search(Radix.Upper, Value, Comparer, Option);
                    
                    // Otherwise, this is the only node, so return it //
                    return Radix;

                }

                // Search //
                BinaryNode<T> Element = Radix;
                while (Element != null)
                {

                    c = Comparer.Compare(Value, Element.Value);
                    if (c < 0)
                        Element = BinaryNode<T>.Search(Element.Lower, Value, Comparer, Option);
                    else if (c > 0)
                        Element = BinaryNode<T>.Search(Element.Upper, Value, Comparer, Option);
                    else
                        return Element;

                }

                // Return null //
                return null;

            }

            /// <summary>
            /// Checks if a value exists in a given node
            /// </summary>
            /// <param name="Radix"></param>
            /// <param name="Value"></param>
            /// <param name="Comparer"></param>
            /// <returns></returns>
            public static bool Exists(BinaryNode<T> Radix, T Value, IComparer<T> Comparer)
            {
                return Search(Radix, Value, Comparer, BinarySearchOption.Any) != null;
            }

            /// <summary>
            /// Gets a given value at a specified index
            /// </summary>
            /// <param name="Radix"></param>
            /// <param name="Index"></param>
            /// <returns></returns>
            public static BinaryNode<T> IndexOf(BinaryNode<T> Radix, int Index)
            {

                if (Radix == null)
                    throw new Exception("");

                if (Index >= Radix.Count)
                    throw new IndexOutOfRangeException("");

                if (Radix.Count == Index + 1)
                    return Radix;

                int lowCount = (Radix.Lower == null ? 0 : Radix.Lower.Count);
                int highCount = (Radix.Upper == null ? 0 : Radix.Upper.Count);

                if (lowCount < Index + 1)
                    return IndexOf(Radix.Upper, Index);
                else if (highCount > Index + 1)
                    return IndexOf(Radix.Lower, Index);
                else
                    return Radix;

            }

            /// <summary>
            /// Gets the lowest value in the tree
            /// </summary>
            /// <param name="Radix"></param>
            /// <returns></returns>
            public static BinaryNode<T> Lowest(BinaryNode<T> Radix)
            {

                if (Radix == null)
                    return null;

                BinaryNode<T> x = Radix;

                while (x.Lower != null)
                {
                    x = x.Lower;
                }

                return x;

            }

            /// <summary>
            /// Gets the highest value in the tree
            /// </summary>
            /// <param name="Radix"></param>
            /// <returns></returns>
            public static BinaryNode<T> Highest(BinaryNode<T> Radix)
            {

                if (Radix == null)
                    return null;

                BinaryNode<T> x = Radix;

                while (x.Upper != null)
                {
                    x = x.Upper;
                }

                return x;

            }

            /// <summary>
            /// Gets all child nodes in order from a given node
            /// </summary>
            /// <param name="Radix"></param>
            /// <returns></returns>
            public static List<T> Values(BinaryNode<T> Radix)
            {

                Stack<BinaryNode<T>> Intermediary = new Stack<BinaryNode<T>>();
                List<T> Sack = new List<T>();

                BinaryNode<T> Spike = Radix;
                bool SeekUp = true;

                Intermediary.Push(Spike);

                while (Intermediary.Count > 0)
                {
                    
                    if (SeekUp)
                    {

                        while (Spike.Lower != null)
                        {
                            Intermediary.Push(Spike);
                            Spike = Spike.Lower;
                        }

                    }

                    Sack.Add(Spike.Value);

                    if (Spike.Upper != null)
                    {
                        Spike = Spike.Upper;
                        SeekUp = true;
                    }
                    else
                    {
                        Spike = Intermediary.Pop();
                        SeekUp = false;
                    }

                }

                return Sack;
                
            }

        }

        private IComparer<T> _Comparer;
        private BinaryNode<T> _Root;
        private BinaryTreeOption _Option;

        /// <summary>
        /// Created a binary tree
        /// </summary>
        /// <param name="Comparer"></param>
        public BinaryTree(IComparer<T> Comparer, BinaryTreeOption Option)
        {
            this._Comparer = Comparer;
            this._Option = Option;
            this._Root = null;
        }

        public BinaryTree(IComparer<T> Comparer)
            : this(Comparer, BinaryTreeOption.Wild)
        {
        }

        public BinaryTree(BinaryTreeOption Option)
            : this(Comparer<T>.Default, Option)
        {
        }

        public BinaryTree()
            : this(Comparer<T>.Default, BinaryTreeOption.Wild)
        {
        }

        public T Lowest
        {
            get { return BinaryNode<T>.Lowest(this._Root).Value; }
        }

        public T Highest
        {
            get { return BinaryNode<T>.Highest(this._Root).Value; }
        }

        public int Count
        {
            get { return (this._Root == null ? 0 : this._Root.Count); }
        }

        public T this[int Index]
        {
            get { return BinaryNode<T>.IndexOf(this._Root, Index).Value; }
        }

        public void Add(T Value)
        {

            if (this._Root == null)
            {
                this._Root = new BinaryNode<T>(null, Value);
                return;
            }

            bool Exists = (this._Option == BinaryTreeOption.Wild ? false : BinaryNode<T>.Exists(this._Root, Value, this._Comparer));

            if (Exists && this._Option == BinaryTreeOption.Distinct)
                return;
            else if (Exists && this._Option == BinaryTreeOption.Unique)
                throw new Exception("Element already exists");

            BinaryNode<T> Element = new BinaryNode<T>(null, Value);
            BinaryNode<T>.Insert(this._Root, Element, this._Comparer);

        }

        public void Remove(T Value)
        {

            if (this._Root == null)
                return;

            BinaryNode<T> Element = BinaryNode<T>.Search(this._Root, Value, this._Comparer, BinarySearchOption.Any);

            while (Element != null)
            {

                BinaryNode<T>.Remove(Element, this._Comparer);

                Element = BinaryNode<T>.Search(this._Root, Value, this._Comparer, BinarySearchOption.Any);


            }

        }

        public BinaryNode<T> Seek(T Value, BinarySearchOption Option)
        {
            return BinaryNode<T>.Search(this._Root, Value, this._Comparer, Option);
        }

        public List<T> Elements()
        {
            return BinaryNode<T>.Values(this._Root);
        }

        public override string ToString()
        {

            StringBuilder sb = new StringBuilder();
            foreach(T x in this.Elements())
            {
                sb.AppendLine(x.ToString());
            }
            return sb.ToString();

        }


    }

}
