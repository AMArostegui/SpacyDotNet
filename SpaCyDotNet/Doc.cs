using System.ComponentModel;
using System.Collections.Generic;
using Python.Runtime;

namespace SpacyDotNet
{
    [DefaultProperty("Tokens")]
    public class Doc
    {
        private dynamic _doc;

        private Vocab _vocab;

        private List<Token> _tokens;

        private List<Span> _sentences;
        private List<Span> _nounChunks;
        private List<Span> _ents;

        public Doc(Vocab vocab)
        {
            _vocab = vocab;

            using (Py.GIL())
            {
                dynamic spacy = Py.Import("spacy");
                dynamic pyVocab = vocab.PyObj;
                _doc = spacy.tokens.doc.Doc.__call__(pyVocab);
            }
        }

        internal Doc(dynamic doc)
        {
            _doc = doc;
            _vocab = null;
        }

        internal dynamic PyObj
            { get { return _doc; } }

        public List<Token> Tokens
        {
            get
            {
                return Utils.GetList(_doc, ref _tokens);
            }
        }

        public List<Span> Sents
        {
            get
            {
                return Utils.GetList(_doc.sents, ref _sentences);
            }
        }

        public List<Span> NounChunks
        {
            get
            {
                return Utils.GetList(_doc.noun_chunks, ref _nounChunks);
            }
        }

        public List<Span> Ents
        {
            get
            {
                return Utils.GetList(_doc.ents, ref _ents);
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
                    var vocab = _doc.vocab;
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
                _doc.to_disk(pyPath);
            }
        }

        public void FromDisk(string path)
        {
            using (Py.GIL())
            {
                var pyPath = new PyString(path);
                _doc.from_disk(pyPath);
            }
        }
    }
}
