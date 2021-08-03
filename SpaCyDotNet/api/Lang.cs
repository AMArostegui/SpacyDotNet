using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Python.Runtime;

namespace SpacyDotNet
{
    public class Lang : IXmlSerializable
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

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            var dummyBytes = new byte[1];

            Debug.Assert(reader.Name == $"{Serialization.Prefix}:PyObj");
            var bytesB64 = reader.ReadElementContentAsString();
            var bytes = Convert.FromBase64String(bytesB64);
            using (Py.GIL())
            {
                var pyBytes = ToPython.GetBytes(bytes);
                _pyLang.from_bytes(pyBytes);
            }

            Debug.Assert(reader.Name == $"{Serialization.Prefix}:PipeNames");
            var pipeNames = reader.ReadElementContentAsString();
            _pipeNames = pipeNames.Split(',').ToList();

            // TODO: Yet to debug. It's not being used so far
            _meta = new PipelineMeta(this);
        }

        public void WriteXml(XmlWriter writer)
        {
            using (Py.GIL())
            {
                var pyObj = Interop.GetBytes(_pyLang.to_bytes());
                var pyObjB64 = Convert.ToBase64String(pyObj);
                writer.WriteElementString("PyObj", pyObjB64, Serialization.Namespace);
            }

            // Using the property is important form the members to be loaded
            writer.WriteElementString("PipeNames", string.Join(',', PipeNames), Serialization.Namespace);
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
