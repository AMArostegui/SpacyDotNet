using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Text;
using Python.Runtime;

namespace SpacyDotNet
{
    public class Interop
    {
        public static bool GetBool(dynamic dynBoolPyObj, ref bool? boolMember)
        {
            if (boolMember != null)
                return (bool)boolMember;

            using (Py.GIL())
            {
                var boolPyInt = new PyInt(dynBoolPyObj);
                boolMember = boolPyInt.ToInt32() != 0;
                return (bool)boolMember;
            }
        }

        public static string GetString(dynamic dynStringPyObj, ref string stringMember)
        {
            if (stringMember != null)
                return stringMember;

            using (Py.GIL())
            {
                var depPy = new PyString(dynStringPyObj);
                stringMember = depPy.ToString();
                return stringMember;
            }
        }

        public static double GetDouble(dynamic dynDoublePyObj, ref double? doubleMember)
        {
            if (doubleMember != null)
                return (double)doubleMember;

            using (Py.GIL())
            {
                var dynDoublePyFloat = PyFloat.AsFloat(dynDoublePyObj);
                doubleMember = dynDoublePyFloat.As<double>();
                return (double)doubleMember;
            }
        }

        public static int GetInt(dynamic dynIntPyObj, ref int? intMember)
        {
            if (intMember != null)
                return (int)intMember;

            using (Py.GIL())
            {
                var intPy = new PyInt(dynIntPyObj);
                intMember = intPy.ToInt32();
                return (int)intMember;
            }
        }

        public static long GetLong(dynamic dynLongPyObj, ref long? longMember)
        {
            if (longMember != null)
                return (long)longMember;

            using (Py.GIL())
            {
                var longPy = new PyInt(dynLongPyObj);
                longMember = longPy.ToInt64();
                return (long)longMember;
            }
        }

        public static BigInteger GetBigInteger(dynamic dynLongPyObj, ref BigInteger? bigInt)
        {
            if (bigInt != null)
                return (BigInteger)bigInt;

            using (Py.GIL())
            {                
                var pyLong = new PyInt(dynLongPyObj);

                // This is inefficient, and should be reworked in the future
                var str = pyLong.ToString();
                bigInt = BigInteger.Parse(str);
                return (BigInteger)bigInt;
            }
        }

        public static List<T> GetListFromGenerator<T>(dynamic pyGenerator, ref List<T> lstMember) where T : new()
        {
            if (lstMember != null)
                return lstMember;

            dynamic builtins = Py.Import("builtins");
            dynamic list = builtins.list(pyGenerator);

            return GetListFromCollection(list, ref lstMember);
        }

        public static List<T> GetListFromCollection<T>(dynamic pyCollection, ref List<T> lstMember) where T: new()
        {
            if (lstMember != null)
                return lstMember;

            using (Py.GIL())
            {
                lstMember = new List<T>();

                dynamic builtins = Py.Import("builtins");
                var pyCount = new PyInt(builtins.len(pyCollection));
                var count = pyCount.ToInt32();

                for (var i = 0; i < count; i++)
                {
                    var element = pyCollection[i];

                    Binder binder = null;
                    BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                    CultureInfo culture = null;
                    var parameters = new object[] { element };

                    lstMember.Add((T)Activator.CreateInstance(typeof(T), flags, binder, parameters, culture));
                }

                return lstMember;
            }
        }

        public static List<T> GetListFromList<T>(dynamic pyList, ref List<T> lstMember)
        {
            if (lstMember != null)
                return lstMember;

            using (Py.GIL())
            {
                lstMember = new List<T>();

                dynamic builtins = Py.Import("builtins");
                var pyCount = new PyInt(builtins.len(pyList));
                var count = pyCount.ToInt32();

                for (var i = 0; i < count; i++)
                {
                    var element = pyList[i];

                    object created = null;
                    if (typeof(T) == typeof(string))
                    {
                        var pyObj = new PyString(element);
                        created = pyObj.ToString();
                    }
                    else
                    {
                        Debug.Assert(false);
                        return null;
                    }                            

                    lstMember.Add((T)created);
                }

                return lstMember;
            }
        }

        public static byte[] GetBytes(dynamic dpyBytes)
        {
            var pyBytes = (PyObject)dpyBytes;
            var pyBuff = pyBytes.GetBuffer();

            var buff = new byte[pyBuff.Length];
            pyBuff.Read(buff, 0, (int)pyBuff.Length, 0);
            return buff;
        }
    }
}
