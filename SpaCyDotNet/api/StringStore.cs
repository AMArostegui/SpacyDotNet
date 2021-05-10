using System;
using System.Collections.Generic;
using Python.Runtime;

namespace SpacyDotNet
{
    public class StringStore
    {
        private dynamic _stringStore;

        private Dictionary<string, long> _dictStrToLong;
        private Dictionary<long, string> _dictLongToStr;

        internal StringStore(dynamic stringStore)
        {
            _stringStore = stringStore;
            _dictStrToLong = new Dictionary<string, long>();
            _dictLongToStr = new Dictionary<long, string>();            
        }

        public object this[object key]
        {
            get
            {
                var keyStr = key as string;
                if (keyStr != null)
                {                    
                    if (_dictStrToLong.ContainsKey(keyStr))
                        return _dictStrToLong[keyStr];

                    long valHash = 0;
                    using (Py.GIL())
                    {
                        var dynPyNumber = _stringStore.__getitem__(key);
                        var pyNumber = new PyInt(dynPyNumber);
                        valHash = pyNumber.As<long>();
                        _dictStrToLong.Add(keyStr, valHash);
                    }

                    return valHash;
                }

                var keyHashN = key as long?;
                if (keyHashN != null)
                {
                    var keyHash = (long)keyHashN;
                    if (_dictLongToStr.ContainsKey(keyHash))
                        return _dictLongToStr[keyHash];

                    var valStr = string.Empty;
                    using (Py.GIL())
                    {
                        var dynPyStr = _stringStore.__getitem__(key);
                        var pyString = new PyString(dynPyStr);
                        valStr = pyString.ToString();
                        _dictLongToStr.Add(keyHash, valStr);
                    }

                    return valStr;
                }

                throw new Exception("Wrong datatype in parameter passed to StringStore");
            }
        }
    }
}
