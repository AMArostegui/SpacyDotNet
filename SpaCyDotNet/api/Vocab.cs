using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.Serialization;
using Python.Runtime;

namespace SpacyDotNet
{
    [Serializable]
    public class Vocab : ISerializable
    {
        private Dictionary<string, Lexeme> _dictStr2Lex = new Dictionary<string, Lexeme>();
        private Dictionary<BigInteger, Lexeme> _dictLong2Lex = new Dictionary<BigInteger, Lexeme>();
        private StringStore _stringStore = null;

        public Vocab()
        {
            using (Py.GIL())
            {
                dynamic spacy = Py.Import("spacy");
                PyVocab = spacy.vocab.Vocab.__call__();
            }
        }

        protected Vocab(SerializationInfo info, StreamingContext context)
        {
            SerializationMode = (SerializationMode)context.Context;

            if (SerializationMode == SerializationMode.SpacyAndDotNet)
            {
                var dummyBytes = new byte[1];

                var bytes = (byte[])info.GetValue("PyObj", dummyBytes.GetType());
                using (Py.GIL())
                {
                    dynamic spacy = Py.Import("spacy");
                    PyVocab = spacy.vocab.Vocab.__call__();

                    var pyBytes = ToPython.GetBytes(bytes);
                    PyVocab.from_bytes(pyBytes);
                }
            }

            Debug.Assert(SerializationMode != SerializationMode.Spacy);
        }

        internal Vocab(dynamic vocab)
        {
            PyVocab = vocab;
        }

        internal dynamic PyVocab
            { get; set; }

        public SerializationMode SerializationMode { get; set; } = SerializationMode.Spacy;

        public Lexeme this[object key]
        {
            get
            {
                var keyStr = key as string;
                if (keyStr != null)
                {
                    if (_dictStr2Lex.ContainsKey(keyStr))
                        return _dictStr2Lex[keyStr];

                    Lexeme lexeme = null;

                    if (PyVocab != null)
                    {
                        using (Py.GIL())
                        {
                            var pyStr = new PyString(keyStr);
                            var dynPyObj = PyVocab.__getitem__(pyStr);
                            lexeme = new Lexeme(dynPyObj);
                            _dictStr2Lex.Add(keyStr, lexeme);
                        }
                    }

                    return lexeme;
                }

                var keyHashN = key as BigInteger?;
                if (keyHashN != null)
                {
                    var keyHash = (BigInteger)keyHashN;
                    if (_dictLong2Lex.ContainsKey(keyHash))
                        return _dictLong2Lex[keyHash];

                    Lexeme lexeme = null;

                    if (PyVocab != null)
                    {
                        using (Py.GIL())
                        {
                            var dynPyObj = PyVocab.__getitem__(key);
                            lexeme = new Lexeme(dynPyObj);
                            _dictLong2Lex.Add(keyHash, lexeme);
                        }
                    }

                    return lexeme;
                }

                throw new Exception("Wrong datatype in parameter passed to Vocab");
            }
        }

        public StringStore Strings
        {
            get
            {
                if (_stringStore != null)
                    return _stringStore;

                using (Py.GIL())
                {
                    var stringStore = PyVocab.strings;
                    _stringStore = new StringStore(stringStore);
                    return _stringStore;
                }
            }
        }

        public void ToDisk(string path)
        {
            if (SerializationMode != SerializationMode.Spacy)
                throw new NotImplementedException();

            using (Py.GIL())
            {
                var pyPath = new PyString(path);
                PyVocab.to_disk(pyPath);
            }
        }

        public void FromDisk(string path)
        {
            if (SerializationMode != SerializationMode.Spacy)
                throw new NotImplementedException();

            using (Py.GIL())
            {
                var pyPath = new PyString(path);
                PyVocab.from_disk(pyPath);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var serializationMode = (SerializationMode)context.Context;

            Debug.Assert(serializationMode != SerializationMode.Spacy);

            if (serializationMode == SerializationMode.SpacyAndDotNet)
            {
                using (Py.GIL())
                {
                    var pyObj = Interop.GetBytes(PyVocab.to_bytes());
                    info.AddValue("PyObj", pyObj);
                }
            }
        }
    }
}
