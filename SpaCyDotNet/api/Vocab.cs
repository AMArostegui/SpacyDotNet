using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Python.Runtime;

namespace SpacyDotNet
{
    public class Vocab : IXmlSerializable
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

        internal Vocab(dynamic vocab)
        {
            PyVocab = vocab;
        }

        internal dynamic PyVocab
            { get; set; }

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
            if (Serialization.Selected != Serialization.Mode.Spacy)
                throw new NotImplementedException();

            using (Py.GIL())
            {
                var pyPath = new PyString(path);
                PyVocab.to_disk(pyPath);
            }
        }

        public void FromDisk(string path)
        {
            if (Serialization.Selected != Serialization.Mode.Spacy)
                throw new NotImplementedException();

            using (Py.GIL())
            {
                var pyPath = new PyString(path);
                PyVocab.from_disk(pyPath);
            }
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            var serializationMode = Serialization.Selected;

            if (serializationMode == Serialization.Mode.SpacyAndDotNet)
            {
                reader.ReadStartElement();
                Debug.Assert(reader.Name == $"{Serialization.Prefix}:PyObj");
                var bytesB64 = reader.ReadElementContentAsString();
                var bytes = Convert.FromBase64String(bytesB64);
                using (Py.GIL())
                {
                    dynamic spacy = Py.Import("spacy");
                    PyVocab = spacy.vocab.Vocab.__call__();

                    var pyBytes = ToPython.GetBytes(bytes);
                    PyVocab.from_bytes(pyBytes);
                }

                reader.ReadEndElement();
            }
            else
                reader.Skip();

            Debug.Assert(serializationMode != Serialization.Mode.Spacy);
        }

        public void WriteXml(XmlWriter writer)
        {
            var serializationMode = Serialization.Selected;

            Debug.Assert(serializationMode != Serialization.Mode.Spacy);

            if (serializationMode == Serialization.Mode.SpacyAndDotNet)
            {
                using (Py.GIL())
                {
                    var pyObj = Interop.GetBytes(PyVocab.to_bytes());
                    var pyObjB64 = Convert.ToBase64String(pyObj);
                    writer.WriteElementString("PyObj", Serialization.Namespace, pyObjB64);
                }
            }
        }
    }
}
