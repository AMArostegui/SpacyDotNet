using System;
using System.Collections.Generic;
using System.Numerics;
using Python.Runtime;

namespace SpacyDotNet
{
    public class StringStore
    {
        private dynamic _pyStringStore;

        private Dictionary<string, BigInteger> _dictStrToNumber;
        private Dictionary<BigInteger, string> _dictNumberToStr;

        internal StringStore(dynamic stringStore)
        {
            _pyStringStore = stringStore;
            _dictStrToNumber = new Dictionary<string, BigInteger>();
            _dictNumberToStr = new Dictionary<BigInteger, string>();            
        }

        public object this[object key]
        {
            get
            {
                var keyStr = key as string;
                if (keyStr != null)
                {                    
                    if (_dictStrToNumber.ContainsKey(keyStr))
                        return _dictStrToNumber[keyStr];

                    BigInteger valHash;
                    using (Py.GIL())
                    {
                        var dynPyNumber = _pyStringStore.__getitem__(key);
                        var pyNumber = new PyLong(dynPyNumber);
                        valHash = BigInteger.Parse(pyNumber.ToString());
                        _dictStrToNumber.Add(keyStr, valHash);
                    }

                    return valHash;
                }

                var keyHash = key.AsBigInteger();
                if (_dictNumberToStr.ContainsKey(keyHash))
                    return _dictNumberToStr[keyHash];

                var valStr = string.Empty;
                using (Py.GIL())
                {
                    var dynPyStr = _pyStringStore.__getitem__(key);
                    var pyString = new PyString(dynPyStr);
                    valStr = pyString.ToString();
                    _dictNumberToStr.Add(keyHash, valStr);
                }

                return valStr;
            }
        }
    }
}
