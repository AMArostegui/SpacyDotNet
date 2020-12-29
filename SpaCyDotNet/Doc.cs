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

        private List<Span> _sentences;
        private List<Span> _nounChunks;
        private List<Span> _ents;

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

        public List<Span> Sents
        {
            get
            {
                if (_sentences != null)
                    return _sentences;

                using (Py.GIL())
                {
                    _sentences = new List<Span>();

                    var iter = _doc.sents.__iter__();
                    while (true)
                    {
                        try
                        {
                            var element = iter.__next__();
                            _sentences.Add(new Span(element));
                        }
                        catch (PythonException)
                        {
                            break;
                        }
                    }
                    return _sentences;
                }
            }
        }

        public List<Span> NounChunks
        {
            get
            {
                if (_nounChunks != null)
                    return _nounChunks;

                using (Py.GIL())
                {
                    _nounChunks = new List<Span>();

                    var iter = _doc.noun_chunks.__iter__();
                    while (true)
                    {
                        try
                        {
                            var element = iter.__next__();
                            _nounChunks.Add(new Span(element));
                        }
                        catch (PythonException)
                        {
                            break;
                        }
                    }
                    return _nounChunks;
                }
            }
        }

        public List<Span> Ents
        {
            get
            {
                if (_ents != null)
                    return _ents;

                using (Py.GIL())
                {
                    _ents = new List<Span>();

                    var iter = _doc.ents.__iter__();
                    while (true)
                    {
                        try
                        {
                            var element = iter.__next__();
                            _ents.Add(new Span(element));
                        }
                        catch (PythonException)
                        {
                            break;
                        }
                    }
                    return _ents;
                }
            }
        }
    }
}
