using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticWind;
using ArcticWind.Elements;
using ArcticWind.Expressions;
using ArcticWind.Expressions.ScalarExpressions;
using ArcticWind.Expressions.MatrixExpressions;
using ArcticWind.Expressions.RecordExpressions;
using ArcticWind.Expressions.TableExpressions;
using System.IO;
using System.Security.Cryptography;

namespace ArcticWind.Libraries
{
   
    public sealed class CryptoLibrary : Library
    {

        public const string MD5 = "MD5";
        public const string SHA1 = "SHA1";
        public const string SHA256 = "SHA256";
        public const string SHA384 = "SHA384";
        public const string SHA512 = "SHA512";
        public const string AES128_ENCRYPT = "AES128_ENCRYPT";
        public const string AES256_ENCRYPT = "AES256_ENCRYPT";
        public const string AES512_ENCRYPT = "AES512_ENCRYPT";
        public const string AES128_DECRYPT = "AES128_DECRYPT";
        public const string AES256_DECRYPT = "AES256_DECRYPT";
        public const string AES512_DECRYPT = "AES512_DECRYPT";
        public static readonly string[] ScalarFunctions = { };

        public CryptoLibrary(Host Host)
            : base(Host, "CRYPTO")
        {
        }

        public override Expressions.ActionExpressions.ActionExpressionParameterized ActionLookup(string Name)
        {
            throw new NotImplementedException();
        }

        public override bool ActionExists(string Name)
        {
            throw new NotImplementedException();
        }

        public override RecordExpressionFunction RecordFunctionLookup(string Name)
        {
            throw new NotImplementedException();
        }

        public override bool RecordFunctionExists(string Name)
        {
            throw new NotImplementedException();
        }

        public override MatrixExpressionFunction MatrixFunctionLookup(string Name)
        {
            throw new NotImplementedException();
        }

        public override bool MatrixFunctionExists(string Name)
        {
            throw new NotImplementedException();
        }

        public override TableExpressionFunction TableFunctionLookup(string Name)
        {
            throw new NotImplementedException();
        }

        public override bool TableFunctionExists(string Name)
        {
            throw new NotImplementedException();
        }

        public override ScalarExpressionFunction ScalarFunctionLookup(string Name)
        {
            throw new Exception(string.Format("Invalid function '{0}'", Name));
        }

        public override bool ScalarFunctionExists(string Name)
        {
            return ScalarFunctions.Contains(Name.ToUpper());
        }

    }

}
