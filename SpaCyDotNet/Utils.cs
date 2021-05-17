using System;
using System.Numerics;

namespace SpacyDotNet
{
    public static class Utils
    {
        public static BigInteger AsBigInteger(this object obj)
        {
            if (obj is int)            
                return new BigInteger((int)obj);
            if (obj is uint)
                return new BigInteger((uint)obj);
            if (obj is long)
                return new BigInteger((long)obj);
            if (obj is ulong)
                return new BigInteger((ulong)obj);
            if (obj is short)
                return new BigInteger((short)obj);
            if (obj is ushort)
                return new BigInteger((ushort)obj);

            throw new InvalidCastException("Wrong datatype to convert to BigInteger");
        }
    }
}
