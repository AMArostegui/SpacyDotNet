using Python.Runtime;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Reflection;

namespace PythonNetUtils
{
    public class ToClr
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
