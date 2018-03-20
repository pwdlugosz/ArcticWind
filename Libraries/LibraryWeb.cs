using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using ArcticWind.Expressions.ActionExpressions;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Elements;
using ArcticWind.Tables;
using ArcticWind.Expressions;

namespace ArcticWind.Libraries
{


    //public class LibraryWeb : Library
    //{

    //    public const string S_GETBYID = "GETBYID";

    //    public LibraryWeb(Host Enviro)
    //        : base(Enviro, "WEB")
    //    {
    //    }


    //    public override ActionExpressionParameterized ActionLookup(string Name)
    //    {

    //        string n = Name.Trim().ToUpper();

    //        switch (n)
    //        {

    //        }

    //        return null;

    //    }

    //    public override bool ActionExists(string Name)
    //    {
    //        return this.ActionLookup(Name) != null;
    //    }

    //    public override ScalarExpressionFunction ScalarFunctionLookup(string Name)
    //    {

    //        string t = Name.ToUpper().Trim();

    //        switch (t)
    //        {
    //            case S_GETBYID:
    //                return new ScalarExpressionGetByID();
    //        }

    //        return null;

    //    }

    //    public override bool ScalarFunctionExists(string Name)
    //    {
    //        return this.ScalarFunctionLookup(Name) != null;
    //    }

    //    // Scalars //
    //    public class ScalarExpressionGetByID : ScalarExpressionFunction
    //    {

    //        private string _CurrentHTML = null;
    //        private HtmlDocument _Doc = null;

    //        public ScalarExpressionGetByID()
    //            : base(null, null, null, 3)
    //        {
    //        }

    //        public override CellAffinity ReturnAffinity()
    //        {
    //            return CellAffinity.BSTRING;
    //        }

    //        public override ScalarExpression CloneOfMe()
    //        {
    //            return new ScalarExpressionGetByID();
    //        }

    //        public override Cell Evaluate(FieldResolver Variants)
    //        {
                
    //            string raw_html = this._ChildNodes[0].Evaluate(Variants).valueCSTRING;
    //            string xpath = this._ChildNodes[1].Evaluate(Variants).valueCSTRING;
    //            int occurance = (this._ChildNodes.Count >= 3 ? this._ChildNodes[2].Evaluate(Variants).valueINT : 1);
    //            if (occurance < 1) occurance = 1;

    //            // Load the html //
    //            if ((this._CurrentHTML ?? "") != raw_html)
    //            {
    //                this._Doc = new HtmlDocument();
    //                this._Doc.LoadHtml(raw_html);
    //                this._CurrentHTML = raw_html;
    //            }
                
    //            // Find the one we want //
    //            HtmlNode node = null;
    //            if (occurance == 1)
    //            {
    //                node = this._Doc.DocumentNode.QuerySelector(xpath);
    //            }
    //            else
    //            {
    //                var v = this._Doc.DocumentNode.QuerySelectorAll(xpath).ToArray();
    //                if (occurance <= v.Length)
    //                {
    //                    node = v[occurance - 1];
    //                }

    //            }

    //            if (node == null)
    //                return CellValues.NullBSTRING;
    //            return new Cell(node.InnerText, true);

    //        }


    //    }





    //}


}
