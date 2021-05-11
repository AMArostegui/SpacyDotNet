using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Python.Runtime;

namespace SpacyDotNet
{
    [Serializable]
    public class Lang : ISerializable
    {
        private dynamic _lang;

        private List<string> _pipeNames;

        internal Lang(dynamic lang)
        {
            _lang = lang;
            _pipeNames = null;
        }

        protected Lang(SerializationInfo info, StreamingContext context)
        {
            var temp = new List<string>();
            _pipeNames = (List<string>)info.GetValue("Sentences", temp.GetType());
        }

        public Doc GetDocument(string text)
        {
            using (Py.GIL())
            {
                var pyString = new PyString(text);
                dynamic doc = _lang.__call__(pyString);
                return new Doc(doc);
            }
        }

        public List<string> PipeNames
        {
            get
            {
                return ToPythonHelpers.GetListBuiltInType<string>(_lang.pipe_names, ref _pipeNames);
            }
        }

        public Vocab Vocab
        {
            get
            {
                return new Vocab(_lang.vocab);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using the property is important form the members to be loaded
            info.AddValue("PipeNames", PipeNames);
        }
    }
}
