using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Expressions.TableExpressions;
using ArcticWind.Expressions.ActionExpressions;
using ArcticWind.Expressions;
using ArcticWind.Tables;
using ArcticWind.Elements;
using ArcticWind.Elements.Structures;

namespace ArcticWind.Libraries
{

    /// <summary>
    /// Represents a base class for all libraries
    /// </summary>
    public abstract class Library 
    {

        protected Host _Host;
        protected string _Name;

        public Library(Host Host, string Name)
        {
            this._Host = Host;
            this._Name = Name;
        }

        /// <summary>
        /// The name of the library
        /// </summary>
        public string Name
        {
            get { return this._Name; }
        }

        /// <summary>
        /// The library's host
        /// </summary>
        public Host Host
        {
            get { return this._Host; }
        }

        /// <summary>
        /// Shuts down the library
        /// </summary>
        public virtual void ShutDown()
        {
            // do something
        }

        /// <summary>
        /// Checks if a function exists
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract bool ScalarFunctionExists(string Name);

        /// <summary>
        /// Gets a function
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract ScalarExpressionFunction ScalarFunctionLookup(string Name);

        /// <summary>
        /// Checks if a function exists
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract bool MatrixFunctionExists(string Name);

        /// <summary>
        /// Gets a function
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract MatrixExpressionFunction MatrixFunctionLookup(string Name);

        /// <summary>
        /// Checks if a function exists
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract bool RecordFunctionExists(string Name);

        /// <summary>
        /// Gets a function
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract RecordExpressionFunction RecordFunctionLookup(string Name);

        /// <summary>
        /// Checks if a function exists
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract bool TableFunctionExists(string Name);

        /// <summary>
        /// Gets a function
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract TableExpressionFunction TableFunctionLookup(string Name);

        /// <summary>
        /// Checks if an action exists
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract bool ActionExists(string Name);

        /// <summary>
        /// Gets an action
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract ActionExpressionParameterized ActionLookup(string Name);

    }

    public sealed class ScriptedLibrary : Library
    {

        private Heap<TypedAbstractMethod> _Scalars;
        private Heap<TypedAbstractMethod> _Matrixes;
        private Heap<TypedAbstractMethod> _Records;
        //private Heap<TableExpressionFunction> _Tables;
        private Heap<AbstractMethod> _Void;

        public ScriptedLibrary(Host Host, string Name)
            : base(Host, Name)
        {
            this._Scalars = new Heap<TypedAbstractMethod>();
            this._Matrixes = new Heap<TypedAbstractMethod>();
            this._Records = new Heap<TypedAbstractMethod>();
            //this._Tables = new Heap<TableExpressionFunction>();
            this._Void = new Heap<AbstractMethod>();
        }

        // Actions
        public override bool ActionExists(string Name)
        {
            return this._Void.Exists(Name);
        }

        public override ActionExpressionParameterized ActionLookup(string Name)
        {
            if (!this._Void.Exists(Name))
                throw new Exception(string.Format("Action '{0}' does not exist", Name));
            AbstractMethod m = this._Void[Name];
            return new ActionExpressionInvokeVoid(this._Host, null, m);
        }

        public void AddVoid(AbstractMethod Method)
        {
            this._Void.Allocate(Method.Name, Method);
        }

        // Scalars
        public override bool ScalarFunctionExists(string Name)
        {
            return this._Scalars.Exists(Name);
        }

        public override ScalarExpressionFunction ScalarFunctionLookup(string Name)
        {
            if (!this._Scalars.Exists(Name))
                throw new Exception(string.Format("Function '{0}' does not exist", Name));
            return new ScalarExpressionFunctionScripted(this._Host, null, this._Scalars[Name]);
        }

        public void AddScalar(TypedAbstractMethod Method)
        {
            this._Scalars.Allocate(Method.Name, Method);
        }

        // Matrix
        public override bool MatrixFunctionExists(string Name)
        {
            throw new NotImplementedException();
        }

        public override MatrixExpressionFunction MatrixFunctionLookup(string Name)
        {
            throw new NotImplementedException();
        }

        // Records
        public override bool RecordFunctionExists(string Name)
        {
            throw new NotImplementedException();
        }

        public override RecordExpressionFunction RecordFunctionLookup(string Name)
        {
            throw new NotImplementedException();
        }

        // Tables
        public override bool TableFunctionExists(string Name)
        {
            throw new NotImplementedException();
        }

        public override TableExpressionFunction TableFunctionLookup(string Name)
        {
            throw new NotImplementedException();
        }

    }


}
