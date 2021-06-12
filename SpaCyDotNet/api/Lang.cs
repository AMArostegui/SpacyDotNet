using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Python.Runtime;

namespace SpacyDotNet
{
    [Serializable]
    public class Lang : ISerializable
    {
        private dynamic _pyLang;

        private List<string> _pipeNames;
        private PipelineMeta _meta;

        internal Lang(dynamic lang)
        {
            _pyLang = lang;
            _pipeNames = null;
            _meta = new PipelineMeta(this);
        }

        protected Lang(SerializationInfo info, StreamingContext context)
        {
            var dummyBytes = new byte[1];

            var bytes = (byte[])info.GetValue("PyObj", dummyBytes.GetType());
            using (Py.GIL())
            {
                var pyBytes = ToPython.GetBytes(bytes);
                _pyLang.from_bytes(pyBytes);
            }

            var temp = new List<string>();
            _pipeNames = (List<string>)info.GetValue("Sentences", temp.GetType());
            _meta = new PipelineMeta(this);
        }

        public Doc GetDocument(string text)
        {
            using (Py.GIL())
            {
                var pyString = new PyString(text);
                dynamic doc = _pyLang.__call__(pyString);
                return new Doc(doc, text);
            }
        }

        internal dynamic PyLang { get => _pyLang; }

        public PipelineMeta Meta
        {
            get => _meta;
        }

        public List<string> PipeNames
        {
            get
            {
                return Interop.GetListFromList<string>(_pyLang?.pipe_names, ref _pipeNames);
            }
        }

        public Vocab Vocab
        {
            get
            {
                return new Vocab(_pyLang.vocab);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            using (Py.GIL())
            {
                var pyObj = Interop.GetBytes(_pyLang.to_bytes());
                info.AddValue("PyObj", pyObj);
            }

            // Using the property is important form the members to be loaded
            info.AddValue("PipeNames", PipeNames);
        }

        public class PipelineMeta : Dictionary<string, object>
        {
            private Lang _lang;

            public PipelineMeta(Lang lang)
            {
                _lang = lang;
            }

            public new object this[string key]
            {
                get
                {
                    if (ContainsKey(key))
                        return base[key];

                    if (_lang.PyLang == null)
                        return null;

                    object ret = null;
                    using (Py.GIL())
                    {
                        var pyKeyStr = new PyString(key);
                        var pyObj = (PyObject)_lang.PyLang.meta.__getitem__(pyKeyStr);

                        if (!PyString.IsStringType(pyObj))
                            throw new NotImplementedException();

                        var pyValStr = new PyString(pyObj);
                        ret = pyValStr.ToString();
                        Add(key, ret);
                    }

                    return ret;
                }
            }
        }
    }
}
