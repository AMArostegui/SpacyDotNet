using Python.Runtime;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Reflection;

namespace PythonNetUtils
{
    public class ToClr
    {
        public static bool GetBoolMember(dynamic dynBoolPyObj, ref bool? boolMember)
        {
            if (boolMember != null)
            {
                return (bool)boolMember;
            }

            boolMember = GetBool(dynBoolPyObj);
            return (bool)boolMember;
        }

        public static bool GetBool(dynamic dynBoolPyObj)
        {
            using (Py.GIL())
            {
                var boolPyInt = new PyInt(dynBoolPyObj);
                bool boolVar = boolPyInt.ToInt32() != 0;
                return boolVar;
            }
        }

        public static string GetStringMember(dynamic dynStringPyObj, ref string stringMember)
        {
            if (stringMember != null)
                return stringMember;

            stringMember = GetString(dynStringPyObj);
            return stringMember;
        }

        public static string GetString(dynamic dynStringPyObj)
        {
            using (Py.GIL())
            {
                var depPy = new PyString(dynStringPyObj);
                string stringVar = depPy.ToString();
                return stringVar;
            }
        }

        public static double GetDoubleMember(dynamic dynDoublePyObj, ref double? doubleMember)
        {
            if (doubleMember != null)
            {
                return (double)doubleMember;
            }                

            doubleMember = GetDouble(dynDoublePyObj);
            return (double)doubleMember;
        }

        public static double GetDouble(dynamic dynDoublePyObj)
        {
            using (Py.GIL())
            {
                var dynDoublePyFloat = PyFloat.AsFloat(dynDoublePyObj);
                double doubleVar = dynDoublePyFloat.As<double>();
                return doubleVar;
            }
        }

        public static int GetIntMember(dynamic dynIntPyObj, ref int? intMember)
        {
            if (intMember != null)
            {
                return (int)intMember;
            }

            intMember = GetInt(dynIntPyObj);
            return (int)intMember;
        }

        public static int GetInt(dynamic dynIntPyObj)
        {
            using (Py.GIL())
            {
                var intPy = new PyInt(dynIntPyObj);
                int intVar = intPy.ToInt32();
                return intVar;
            }
        }

        public static long GetLongMember(dynamic dynLongPyObj, ref long? longMember)
        {
            if (longMember != null)
            {
                return (long)longMember;
            }

            longMember = GetLong(dynLongPyObj);
            return (long)longMember;               
        }

        public static long GetLong(dynamic dynLongPyObj)
        {
            using (Py.GIL())
            {
                var longPy = new PyInt(dynLongPyObj);
                long longMember = longPy.ToInt64();
                return longMember;
            }
        }

        public static BigInteger GetBigIntegerMember(dynamic dynLongPyObj, ref BigInteger? bigInt)
        {
            if (bigInt != null)
            {
                return (BigInteger)bigInt;
            }

            bigInt = GetBigInteger(dynLongPyObj);
            return (BigInteger)bigInt;
        }

        public static BigInteger GetBigInteger(dynamic dynLongPyObj)
        {
            using (Py.GIL())
            {                
                var pyLong = new PyInt(dynLongPyObj);

                // This is inefficient, and should be reworked in the future
                var str = pyLong.ToString();
                BigInteger bigInt = BigInteger.Parse(str);
                return bigInt;
            }
        }

        public static List<T> GetListFromPyGeneratorMember<T>(dynamic pyGenerator, ref List<T> lstMember) where T : new()
        {
            if (lstMember != null)
            {
                return lstMember;
            }

            lstMember = GetListFromPyGenerator<T>(pyGenerator);
            return lstMember;
        }

        public static List<T> GetListFromPyGenerator<T>(dynamic pyGenerator) where T : new()
        {
            dynamic builtins = Py.Import("builtins");
            dynamic list = builtins.list(pyGenerator);

            return GetListFromPyCollection<T>(list);
        }

        public static List<T> GetListFromPyCollectionMember<T>(dynamic pyCollection, ref List<T> lstMember) where T : new()
        {
            if (lstMember != null)
            {
                return lstMember;
            }
            
            lstMember = GetListFromPyCollection<T>(pyCollection);
            return lstMember;
        }

        public static List<T> GetListFromPyCollection<T>(dynamic pyCollection) where T: new()
        {
            var lstVar = new List<T>();

            using (Py.GIL())
            {               
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

                    lstVar.Add((T)Activator.CreateInstance(typeof(T), flags, binder, parameters, culture));
                }

                return lstVar;
            }
        }

        public static List<T> GetListFromPyListMember<T>(dynamic pyList, ref List<T> lstMember)
        {
            if (lstMember != null)
            {
                return lstMember;
            }              

            lstMember = GetListFromPyList<T>(pyList);
            return lstMember;
        }

        public static List<T> GetListFromPyList<T>(dynamic pyList)
        {
            var lstVar = new List<T>();

            using (Py.GIL())
            {
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

                    lstVar.Add((T)created);
                }

                return lstVar;
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
