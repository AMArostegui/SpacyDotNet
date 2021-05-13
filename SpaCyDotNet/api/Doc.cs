using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
                return ToPythonHelpers.GetListWrapperObj(_pyDoc, ref _tokens);
            }
        }

        public List<Span> Sents
        {
            get
            {
                return ToPythonHelpers.GetListWrapperObj(_pyDoc.sents, ref _sentences);
            }
        }

        public List<Span> NounChunks
        {
            get
            {
                return ToPythonHelpers.GetListWrapperObj(_pyDoc.noun_chunks, ref _nounChunks);
            }
        }

        public List<Span> Ents
        {
            get
            {
                return ToPythonHelpers.GetListWrapperObj(_pyDoc.ents, ref _ents);
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
            using (Py.GIL())
            {
                var pyPath = new PyString(path);
                _pyDoc.to_disk(pyPath);
            }
        }

        public void FromDisk(string path)
        {
            using (Py.GIL())
            {
                var pyPath = new PyString(path);
                _pyDoc.from_disk(pyPath);
            }
        }

        public byte[] ToBytes()
        {
            using (Py.GIL())
            {
                return ToPythonHelpers.GetBytes(_pyDoc.to_bytes());
            }
        }

        public void FromBytes(byte[] bytes)
        {
            using (Py.GIL())
            {
                var pyBytes = ToDotNetHelpers.GetBytes(bytes);
                _pyDoc.from_bytes(pyBytes);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using the property is important form the members to be loaded
            info.AddValue("Tokens", Tokens);
            info.AddValue("Sentences", Sents);
            info.AddValue("NounChunks", NounChunks);
            info.AddValue("Ents", Ents);
        }
    }
}
