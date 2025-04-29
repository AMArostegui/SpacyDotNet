using Python.Runtime;

namespace PythonNetUtils
{
    public class ToPy
    {
        public static dynamic GetBytes(byte[] bytes)
        {
            using (Py.GIL())
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

        public static dynamic GetList<T>(T[] list)
        {
            using (Py.GIL())
            {
                var pyLst = new PyList();
                if (list != null)
                {
                    var type = typeof(T);

                    foreach (var element in list)
                    {
                        if (type == typeof(string))
                        {
                            var pyElement = new PyString((string)(object)element);
                            pyLst.Append(pyElement);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                }

                return pyLst;
            }
        }
    }
}
