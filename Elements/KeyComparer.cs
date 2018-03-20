using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticWind.Elements
{

    public static class KeyComparer
    {

        public static bool IsStrongSubset(Key A, Key B)
        {

            if (B.Count > A.Count)
                return false;

            for (int i = 0; i < A.Count; i++)
            {
                if (!(A[i] == B[i] && A.Affinity(i) == B.Affinity(i)))
                {
                    return false;
                }
            }

            return true;

        }

        public static bool IsWeakSubset(Key A, Key B)
        {

            if (B.Count > A.Count)
                return false;

            for (int i = 0; i < A.Count; i++)
            {
                if (!(A[i] == B[i]))
                {
                    return false;
                }
            }

            return true;

        }

        public static bool IsStrongUnion(Key A, Key B)
        {

            for (int i = 0; i < Math.Min(A.Count, B.Count); i++)
            {
                if (!(A[i] == B[i] && A.Affinity(i) == B.Affinity(i)))
                {
                    return false;
                }
            }

            return true;

        }

        public static bool IsWeakUnion(Key A, Key B)
        {

            for (int i = 0; i < Math.Min(A.Count, B.Count); i++)
            {
                if (!(A[i] == B[i]))
                {
                    return false;
                }
            }

            return true;

        }

        public static bool StrongEquals(Key A, Key B)
        {

            if (B.Count != A.Count)
                return false;

            for (int i = 0; i < A.Count; i++)
            {
                if (!(A[i] == B[i] && A.Affinity(i) == B.Affinity(i)))
                {
                    return false;
                }
            }

            return true;

        }

        public static bool WeakEquals(Key A, Key B)
        {

            if (B.Count != A.Count)
                return false;

            for (int i = 0; i < A.Count; i++)
            {
                if (!(A[i] == B[i]))
                {
                    return false;
                }
            }

            return true;

        }

    }


}
