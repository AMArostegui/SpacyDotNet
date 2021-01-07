﻿using System;
using System.Collections.Generic;
using System.Text;
using Python.Runtime;

namespace SpacyDotNet
{
    public class Utils
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
                var startCharPy = new PyInt(dynIntPyObj);
                intMember = startCharPy.ToInt32();
                return (int)intMember;
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
                        lstMember.Add(Activator.CreateInstance(typeof(T), element));
                    }
                    catch (PythonException)
                    {
                        break;
                    }
                }
                return lstMember;
            }
        }
    }
}
