using System;
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
    }
}
