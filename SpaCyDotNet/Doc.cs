using System.ComponentModel;
using System.Collections.Generic;
using Python.Runtime;

namespace SpacyDotNet
{
    [DefaultProperty("Tokens")]
    public class Doc
    {
        private dynamic _doc;

        private List<Token> _tokens;

        private List<Span> _sentences;
        private List<Span> _nounChunks;
        private List<Span> _ents;

        public Doc(dynamic doc)
        {
            _doc = doc;
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
    }
}
