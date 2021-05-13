using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Python.Runtime;

namespace SpacyDotNet
{
    [Serializable]
    public class Doc : ISerializable
    {
        private dynamic _pyDoc;

        private Vocab _vocab;

        private List<Token> _tokens;

        private List<Span> _sentences;
        private List<Span> _nounChunks;
        private List<Span> _ents;

        public Doc()
        {
            // Needed for ISerializable interface
        }

        protected Doc(SerializationInfo info, StreamingContext context)
        {
            var dummyBytes = new byte[1];

            var bytes = (byte[])info.GetValue("PyObj", dummyBytes.GetType());
            using (Py.GIL())
            {
                var pyBytes = ToPython.GetBytes(bytes);
                _pyDoc.from_bytes(pyBytes);
            }

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
                dynamic pyVocab = vocab.PyObj;
                _pyDoc = spacy.tokens.doc.Doc.__call__(pyVocab);
            }
        }

        internal Doc(dynamic doc)
        {
            _pyDoc = doc;
            _vocab = null;
        }

        internal dynamic PyObj
            { get { return _pyDoc; } }

        public List<Token> Tokens
        {
            get
            {
                return Helpers.GetListWrapperObj(_pyDoc, ref _tokens);
            }
        }

        public List<Span> Sents
        {
            get
            {
                return Helpers.GetListWrapperObj(_pyDoc.sents, ref _sentences);
            }
        }

        public List<Span> NounChunks
        {
            get
            {
                return Helpers.GetListWrapperObj(_pyDoc.noun_chunks, ref _nounChunks);
            }
        }

        public List<Span> Ents
        {
            get
            {
                return Helpers.GetListWrapperObj(_pyDoc.ents, ref _ents);
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
                    var vocab = _pyDoc.vocab;
                    _vocab = new Vocab(vocab);
                    return _vocab;
                }
            }
        }

        public void ToDisk(string path)
        {
            var formatter = new BinaryFormatter();
            using var stream = new FileStream(path, FileMode.Create);
            formatter.Serialize(stream, this);
        }

        public void FromDisk(string path)
        {
            var formatter = new BinaryFormatter();
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var doc = (Doc)formatter.Deserialize(stream);

            Copy(doc);
        }

        public byte[] ToBytes()
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();
            formatter.Serialize(stream, this);

            return stream.ToArray();
        }

        public void FromBytes(byte[] bytes)
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream(bytes);
            var doc = (Doc)formatter.Deserialize(stream);

            Copy(doc);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            using (Py.GIL())
            {
                var pyObj = Helpers.GetBytes(_pyDoc.to_bytes());
                info.AddValue("PyObj", pyObj);
            }

            // Using the property is important form the members to be loaded
            info.AddValue("Tokens", Tokens);
            info.AddValue("Sentences", Sents);
            info.AddValue("NounChunks", NounChunks);
            info.AddValue("Ents", Ents);
        }

        private void Copy(Doc doc)
        {
            _pyDoc = doc._pyDoc;
            _vocab = doc._vocab;
            _tokens = doc._tokens;
            _sentences = doc._sentences;
            _nounChunks = doc._nounChunks;
            _ents = doc._ents;
        }
    }
}
