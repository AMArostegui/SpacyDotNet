﻿using System;
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

        internal Lang(dynamic lang)
        {
            _pyLang = lang;
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
                dynamic doc = _pyLang.__call__(pyString);
                return new Doc(doc);
            }
        }

        public List<string> PipeNames
        {
            get
            {
                return Helpers.GetListBuiltInType<string>(_pyLang.pipe_names, ref _pipeNames);
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
            // Using the property is important form the members to be loaded
            info.AddValue("PipeNames", PipeNames);
        }
    }
}