using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using Python.Runtime;

namespace SpacyDotNet
{
    public class ToPythonHelpers
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
                var longPy = new PyLong(dynLongPyObj);
                longMember = longPy.ToInt64();
                return (long)longMember;
            }
        }


        public static List<T> GetList<T>(dynamic dynIterPy, ref List<T> lstMember) where T: new()
        {
            if (lstMember != null)
                return lstMember;

            using (Py.GIL())
            {
                lstMember = new List<T>();

                var iter = dynIterPy.__iter__();
                while (true)
                {
                    try
                    {
                        var element = iter.__next__();

                        Binder binder = null;
                        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                        CultureInfo culture = null;
                        var parameters = new object[] { element };

                        lstMember.Add((T)Activator.CreateInstance(typeof(T), flags, binder, parameters, culture));
                    }
                    catch (PythonException)
                    {
                        break;
                    }
                }
                return lstMember;
            }
        }

        public static byte[] GetBytes(dynamic dpyBytes)
        {
            var pyBytes = (PyObject)dpyBytes;
            var pyBuff = pyBytes.GetBuffer();

            var buff = new byte[pyBuff.Length];
            var read = pyBuff.Read(buff, 0, (int)pyBuff.Length);
            if (read != pyBuff.Length)
            {
                Debug.Assert(false);
                return null;
            }

            return buff;
        }
    }
}
