using System.ComponentModel;
using System.Collections.Generic;
using Python.Runtime;

namespace SpacyDotNet
{
    [DefaultProperty("Tokens")]
    public class Doc
    {
        private dynamic _doc;

        private List<Token> _tokens;

        public Doc(dynamic doc)
        {
            _doc = doc;
        }

        public List<Token> Tokens
        {
            get
            {
                if (_tokens != null)
                    return _tokens;

                using (Py.GIL())
                {
                    _tokens = new List<Token>();

                    var lenPy = new PyInt(_doc.__len__());
                    var len = lenPy.ToInt32();
                    
                    for (var i = 0; i < len; i++)
                    {
                        var iPy = new PyInt(i);                        
                        var token = new Token(_doc.__getitem__(iPy));
                        _tokens.Add(token);
                    }

                    return _tokens;
                }
            }
        }
    }
}
