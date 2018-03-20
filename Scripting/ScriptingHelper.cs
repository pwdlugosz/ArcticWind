using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind.Elements;
using ArcticWind.Expressions;
using ArcticWind.Tables;

namespace ArcticWind.Scripting
{
    
    public static class ScriptingHelper
    {

        public static Schema GetSchema(ArcticWindParser.SchemaContext context)
        {
            Schema s = new Schema();
            for (int i = 0; i < context.IDENTIFIER().Length; i++)
            {
                string Name = context.IDENTIFIER()[i].GetText();
                CellAffinity Type = ScriptingHelper.GetTypeAffinity(context.type()[i]);
                int Size = ScriptingHelper.GetTypeSize(context.type()[i]);
                s.Add(Name, Type, Size);
            }
            return s;
        }

        public static int GetTypeSize(ArcticWindParser.TypeContext context)
        {
            if (context.LITERAL_INT() == null)
            {
                CellAffinity a = GetTypeAffinity(context);
                return CellSerializer.DefaultLength(a);
            }
            return int.Parse(context.LITERAL_INT().GetText());
        }

        public static CellAffinity GetTypeAffinity(ArcticWindParser.TypeContext context)
        {

            if (context.T_BOOL() != null)
                return CellAffinity.BOOL;

            else if (context.T_DATE() != null)
                return CellAffinity.DATE_TIME;

            else if (context.T_BYTE() != null)
                return CellAffinity.BYTE;

            else if (context.T_SHORT() != null)
                return CellAffinity.SHORT;

            else if (context.T_INT() != null)
                return CellAffinity.INT;

            else if (context.T_LONG() != null)
                return CellAffinity.LONG;

            else if (context.T_SINGLE() != null)
                return CellAffinity.SINGLE;

            else if (context.T_DOUBLE() != null)
                return CellAffinity.DOUBLE;

            else if (context.T_BINARY() != null)
                return CellAffinity.BINARY;

            else if (context.T_BSTRING() != null)
                return CellAffinity.BSTRING;

            else if (context.T_CSTRING() != null)
                return CellAffinity.CSTRING;

            throw new Exception(string.Format("Invalid type '{0}'", context.GetText()));

        }

        public static ParameterAffinity GetParameterAffinity(ArcticWindParser.Param_defContext context)
        {
            if (context.scalar_name() != null)
                return ParameterAffinity.Scalar;
            else if (context.matrix_name() != null)
                return ParameterAffinity.Matrix;
            else if (context.record_name() != null)
                return ParameterAffinity.Record;
            else if (context.table_name() != null)
                return ParameterAffinity.Table;
            return ParameterAffinity.Missing;
        }

        public static string GetParameterName(ArcticWindParser.Param_defContext context)
        {
            if (context.scalar_name() != null)
                return context.scalar_name().IDENTIFIER().GetText();
            else if (context.matrix_name() != null)
                return context.matrix_name().IDENTIFIER().GetText();
            else if (context.record_name() != null)
                return context.record_name().IDENTIFIER().GetText();
            else if (context.table_name() != null)
                return context.table_name().IDENTIFIER().GetText();
            return null;
        }

        public static string GetLibName(ArcticWindParser.Scalar_nameContext context)
        {
            if (context.lib_name() == null) return Host.GLOBAL;
            return context.lib_name().GetText();
        }

        public static string GetLibName(ArcticWindParser.Matrix_nameContext context)
        {
            if (context.lib_name() == null) return Host.GLOBAL;
            return context.lib_name().GetText();
        }

        public static string GetLibName(ArcticWindParser.Record_nameContext context)
        {
            if (context.lib_name() == null) return Host.GLOBAL;
            return context.lib_name().GetText();
        }

        public static string GetLibName(ArcticWindParser.Table_nameContext context)
        {
            if (context.lib_name() == null) return Host.GLOBAL;
            return context.lib_name().GetText();
        }

        public static string GetVarName(ArcticWindParser.Scalar_nameContext context)
        {
            return context.IDENTIFIER().GetText();
        }

        public static string GetVarName(ArcticWindParser.Matrix_nameContext context)
        {
            return context.IDENTIFIER().GetText();
        }

        public static string GetVarName(ArcticWindParser.Record_nameContext context)
        {
            return context.IDENTIFIER().GetText();
        }

        public static string GetVarName(ArcticWindParser.Table_nameContext context)
        {
            return context.IDENTIFIER().GetText();
        }

        public static Expressions.ActionExpressions.Assignment GetAssignment(ArcticWindParser.AssignmentContext context)
        {

            if (context.PLUS() != null)
                return Expressions.ActionExpressions.Assignment.PlusEquals;
            else if (context.MINUS() != null)
                return Expressions.ActionExpressions.Assignment.MinusEquals;
            else if (context.MUL() != null)
                return Expressions.ActionExpressions.Assignment.ProductEquals;
            else if (context.DIV() != null)
                return Expressions.ActionExpressions.Assignment.DivideEquals;
            else if (context.DIV2() != null)
                return Expressions.ActionExpressions.Assignment.CheckDivideEquals;
            else if (context.MOD() != null)
                return Expressions.ActionExpressions.Assignment.ModEquals;
            else
                return Expressions.ActionExpressions.Assignment.Equals;

        }

    }

}
