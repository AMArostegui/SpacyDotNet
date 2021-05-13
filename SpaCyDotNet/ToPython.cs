using System;
using System.Collections.Generic;
using System.Text;
using Python.Runtime;

namespace SpacyDotNet
{
    public class ToPython
    {
        public static dynamic GetBytes(byte[] bytes)
        {
            // Seems like ToPython method doesn't convert properly in the case of a byte array
            // The lines below throw:
            //      Python.Runtime.PythonException: 'TypeError : a bytes-like object is required, not 'Byte[]''
            // var pyObj = bytes.ToPython();
            // _doc.from_bytes(pyObj);

            // We need to make use of builtin function bytes()
            // Taken from:
            //      https://github.com/pythonnet/pythonnet/issues/1150
            var builtins = Py.Import("builtins");
            var toBytesFunc = builtins.GetAttr("bytes");
            return toBytesFunc.Invoke(bytes.ToPython());
        }
    }
}
