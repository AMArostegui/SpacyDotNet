using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Python.Runtime;

namespace SpacyDotNet
{
    [Serializable]
    public class Doc : ISerializable
    {
        private string _text;

        private Vocab _vocab;

        private List<Token> _tokens;

        private List<Span> _sentences;
        private List<Span> _nounChunks;
        private List<Span> _ents;

        private SerializationMode _serializationMode;

        public Doc()
        {
            // Needed for ISerializable interface
        }

        protected Doc(SerializationInfo info, StreamingContext context)
        {
            _serializationMode = (SerializationMode)context.Context;

            if (_serializationMode == SerializationMode.SpacyAndDotNet)
            {
                var dummyBytes = new byte[1];

                var bytes = (byte[])info.GetValue("PyObj", dummyBytes.GetType());
                using (Py.GIL())
                {
                    dynamic spacy = Py.Import("spacy");
                    dynamic pyVocab = spacy.vocab.Vocab.__call__();
                    PyDoc = spacy.tokens.doc.Doc.__call__(pyVocab);

                    var pyBytes = ToPython.GetBytes(bytes);
                    PyDoc.from_bytes(pyBytes);
                    _vocab = new Vocab(PyDoc.vocab);
                }
            }

            Debug.Assert(_serializationMode != SerializationMode.Spacy);

            _text = info.GetString("Text");

            var tempVocab = new Vocab();
            _vocab = (Vocab)info.GetValue("Vocab", tempVocab.GetType());

            var tempTokens = new List<Token>();
            _tokens = (List<Token>)info.GetValue("Tokens", tempTokens.GetType());

            var tempSpan = new List<Span>();
            _sentences = (List<Span>)info.GetValue("Sentences", tempSpan.GetType());
            _nounChunks = (List<Span>)info.GetValue("NounChunks", tempSpan.GetType());
            _ents = (List<Span>)info.GetValue("Ents", tempSpan.GetType());
        }

        public Doc(Vocab vocab)
        {
            _vocab = vocab;

            using (Py.GIL())
            {
                dynamic spacy = Py.Import("spacy");
                dynamic pyVocab = vocab.PyVocab;
                PyDoc = spacy.tokens.doc.Doc.__call__(pyVocab);
            }
        }

        internal Doc(dynamic doc)
        {
            PyDoc = doc;
            _vocab = null;
        }

        internal dynamic PyDoc { get; set; }

        public SerializationMode SerializationMode
        {
            get => _serializationMode;

            set
            {
                _serializationMode = value;
                if (_vocab != null)
                    _vocab.SerializationMode = value;
            }
        }

        public string Text
        {
            get
            {
                return Interop.GetString(PyDoc?.text, ref _text);
            }
        }

        public List<Token> Tokens
        {
            get
            {
                return Interop.GetListWrapperObj(PyDoc, ref _tokens);
            }
        }

        public List<Span> Sents
        {
            get
            {
                return Interop.GetListWrapperObj(PyDoc?.sents, ref _sentences);
            }
        }

        public List<Span> NounChunks
        {
            get
            {
                return Interop.GetListWrapperObj(PyDoc?.noun_chunks, ref _nounChunks);
            }
        }

        public List<Span> Ents
        {
            get
            {
                return Interop.GetListWrapperObj(PyDoc?.ents, ref _ents);
            }
        }

        public Vocab Vocab
        {
            get
            {
                if (_vocab != null)
                    return _vocab;

                using (Py.GIL())
                {
                    var vocab = PyDoc.vocab;
                    _vocab = new Vocab(vocab);
                    return _vocab;
                }
            }
        }

        public void ToDisk(string path)
        {
            if (SerializationMode == SerializationMode.Spacy)
            {
                using (Py.GIL())
                {
                    var pyPath = new PyString(path);
                    PyDoc.to_disk(pyPath);
                }
            }
            else
            {
                var formatter = new BinaryFormatter();
                using var stream = new FileStream(path, FileMode.Create);
                formatter.Serialize(stream, this);
            }
        }

        public void FromDisk(string path)
        {
            if (SerializationMode == SerializationMode.Spacy)
            {
                using (Py.GIL())
                {
                    var pyPath = new PyString(path);
                    PyDoc.from_disk(pyPath);
                }
            }
            else
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                var formatter = new BinaryFormatter();
                formatter.Context = new StreamingContext(StreamingContextStates.All, _serializationMode);
                var doc = (Doc)formatter.Deserialize(stream);
                Copy(doc);
            }
        }

        public byte[] ToBytes()
        {
            if (SerializationMode == SerializationMode.Spacy)
            {
                using (Py.GIL())
                {
                    return Interop.GetBytes(PyDoc.to_bytes());
                }
            }
            else
            {
                var formatter = new BinaryFormatter();
                var stream = new MemoryStream();
                formatter.Serialize(stream, this);
                return stream.ToArray();
            }
        }

        public void FromBytes(byte[] bytes)
        {
            if (SerializationMode == SerializationMode.Spacy)
            {
                using (Py.GIL())
                {
                    var pyBytes = ToPython.GetBytes(bytes);
                    PyDoc.from_bytes(pyBytes);
                }
            }
            else
            {
                var stream = new MemoryStream(bytes);
                var formatter = new BinaryFormatter();
                formatter.Context = new StreamingContext(StreamingContextStates.All, _serializationMode);
                var doc = (Doc)formatter.Deserialize(stream);
                Copy(doc);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (_serializationMode == SerializationMode.SpacyAndDotNet)
            {
                using (Py.GIL())
                {
                    var pyObj = Interop.GetBytes(PyDoc.to_bytes());
                    info.AddValue("PyObj", pyObj);
                }
            }

            Debug.Assert(_serializationMode != SerializationMode.Spacy);

            // Using the property is important form the members to be loaded
            info.AddValue("Text", Text);
            info.AddValue("Vocab", Vocab);
            info.AddValue("Tokens", Tokens);
            info.AddValue("Sentences", Sents);
            info.AddValue("NounChunks", NounChunks);
            info.AddValue("Ents", Ents);
        }

        private void Copy(Doc doc)
        {
            // I'd rather copy Python object no matter the serialization mode
            // If set to DotNet, the variable will be initialized to null
            // disregarding its current value which might be a default object
            PyDoc = doc.PyDoc;

            _text = doc._text;
            _vocab = doc._vocab;
            _tokens = doc._tokens;
            _sentences = doc._sentences;
            _nounChunks = doc._nounChunks;
            _ents = doc._ents;                
        }
    }
}
