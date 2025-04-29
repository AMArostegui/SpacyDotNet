using Python.Runtime;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Reflection;

namespace PythonNetUtils
{
    public static class ToClr
    {
        public static T GetMember<T>(dynamic dynBoolPyObj, ref T member)
        {
            if (member != null)
            {
                return member;
            }

            member = Get<T>(dynBoolPyObj);
            return member;
        }

        public static T Get<T>(dynamic dynPyBasicType)
        {
            using (Py.GIL())
            {
                var type = typeof(T);

                if (type == typeof(bool) || type == typeof(bool?))
                {
                    var boolPyInt = new PyInt(dynPyBasicType);
                    T boolVar = (T)(object)(boolPyInt.ToInt32() != 0);
                    return boolVar;
                }
                else if (type == typeof(string))
                {
                    var depPy = new PyString(dynPyBasicType);
                    T stringVar = (T)(object)depPy.ToString();
                    return stringVar;                
                }
                else if (type == typeof(double) || type == typeof(double?))
                {
                    var dynDoublePyFloat = PyFloat.AsFloat(dynPyBasicType);
                    T doubleVar = (T)(object)dynDoublePyFloat.As<double>();
                    return doubleVar;
                }
                else if (type == typeof(int) || type == typeof(int?))
                {
                    var intPy = new PyInt(dynPyBasicType);
                    T intVar = (T)(object)intPy.ToInt32();
                    return intVar;
                }
                else if (type == typeof(long) || type == typeof(long?))
                {
                    var longPy = new PyInt(dynPyBasicType);
                    T longVar = (T)(object)longPy.ToInt64();
                    return longVar;
                }
                else if (type == typeof(BigInteger) || type == typeof(BigInteger?))
                {
                    var pyInt = new PyInt(dynPyBasicType);

                    // This is inefficient, and should be reworked in the future
                    var str = pyInt.ToString();
                    T bigInt = (T)(object)BigInteger.Parse(str);
                    return bigInt;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        public static List<T> GetListFromGeneratorMember<T>(dynamic pyGenerator, ref List<T> lstMember) where T : new()
        {
            if (lstMember != null)
            {
                return lstMember;
            }

            lstMember = GetListFromGenerator<T>(pyGenerator);
            return lstMember;
        }

        public static List<T> GetListFromGenerator<T>(dynamic pyGenerator) where T : new()
        {
            dynamic list;

            using (Py.GIL())
            {
                dynamic builtins = Py.Import("builtins");
                list = builtins.list(pyGenerator);
            }

            return GetListFromCollection<T>(list);
        }

        public static List<T> GetListFromCollectionMember<T>(dynamic pyCollection, ref List<T> lstMember) where T : new()
        {
            if (lstMember != null)
            {
                return lstMember;
            }
            
            lstMember = GetListFromCollection<T>(pyCollection);
            return lstMember;
        }

        public static List<T> GetListFromCollection<T>(dynamic pyCollection) where T: new()
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

        public static List<T> GetListFromListMember<T>(dynamic pyList, ref List<T> lstMember)
        {
            if (lstMember != null)
            {
                return lstMember;
            }              

            lstMember = GetListFromList<T>(pyList);
            return lstMember;
        }

        public static List<T> GetListFromList<T>(dynamic pyList)
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
