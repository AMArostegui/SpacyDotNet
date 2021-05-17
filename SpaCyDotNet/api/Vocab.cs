using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Python.Runtime;

namespace SpacyDotNet
{
    [Serializable]
    public class Vocab : ISerializable
    {
        private dynamic _pyVocab;

        private Dictionary<string, Lexeme> _dictStr2Lex = new Dictionary<string, Lexeme>();
        private Dictionary<BigInteger, Lexeme> _dictLong2Lex = new Dictionary<BigInteger, Lexeme>();
        private StringStore _stringStore = null;

        public Vocab()
        {
            using (Py.GIL())
            {
                dynamic spacy = Py.Import("spacy");
                _pyVocab = spacy.vocab.Vocab.__call__();
            }
        }

        protected Vocab(SerializationInfo info, StreamingContext context)
        {
            if (Serialization.IsSpacy())
            {
                var dummyBytes = new byte[1];

                var bytes = (byte[])info.GetValue("PyObj", dummyBytes.GetType());
                using (Py.GIL())
                {
                    dynamic spacy = Py.Import("spacy");
                    _pyVocab = spacy.vocab.Vocab.__call__();

                    var pyBytes = ToPython.GetBytes(bytes);
                    _pyVocab.from_bytes(pyBytes);
                }
            }
        }

        internal Vocab(dynamic vocab)
        {
            _pyVocab = vocab;
        }

        internal dynamic PyObj
            { get => _pyVocab; }

        public static Serialization Serialization { get; set; } = Serialization.Spacy;

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
                    using (Py.GIL())
                    {
                        var pyStr = new PyString(keyStr);
                        var dynPyObj = _pyVocab.__getitem__(pyStr);
                        lexeme = new Lexeme(dynPyObj);                        
                        _dictStr2Lex.Add(keyStr, lexeme);
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
                    using (Py.GIL())
                    {
                        var dynPyObj = _pyVocab.__getitem__(key);
                        lexeme = new Lexeme(dynPyObj);                        
                        _dictLong2Lex.Add(keyHash, lexeme);
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
                    var stringStore = _pyVocab.strings;
                    _stringStore = new StringStore(stringStore);
                    return _stringStore;
                }
            }
        }

        public void ToDisk(string path)
        {
            if (Serialization != Serialization.Spacy)
                throw new NotImplementedException();

            using (Py.GIL())
            {
                var pyPath = new PyString(path);
                _pyVocab.to_disk(pyPath);
            }
        }

        public void FromDisk(string path)
        {
            if (Serialization != Serialization.Spacy)
                throw new NotImplementedException();

            using (Py.GIL())
            {
                var pyPath = new PyString(path);
                _pyVocab.from_disk(pyPath);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (Serialization.IsSpacy())
            {
                using (Py.GIL())
                {
                    var pyObj = Interop.GetBytes(_pyVocab.to_bytes());
                    info.AddValue("PyObj", pyObj);
                }
            }
        }
    }
}
