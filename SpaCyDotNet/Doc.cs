using System.ComponentModel;
using System.Collections.Generic;
using Python.Runtime;
using System.Diagnostics;

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

        public byte[] ToBytes()
        {
            using (Py.GIL())
            {
                dynamic dpyBytes = _doc.to_bytes();
                var pyBytes = (PyObject)dpyBytes;
                var pyBuff = pyBytes.GetBuffer();

                var buff = new byte[pyBuff.Length];
                var read = pyBuff.Read(buff, 0, (int)pyBuff.Length);
                if (read != pyBuff.Length)
                {
                    Debug.Assert(false);
                    return null;
                }

                return buff;
            }
        }

        public void FromBytes(byte[] bytes)
        {
            using (Py.GIL())
            {
                // Seems like ToPython method doesn't convert properly in the case of a byte array
                // The lines below throw:
                //      Python.Runtime.PythonException: 'TypeError : a bytes-like object is required, not 'Byte[]''
                // var pyObj = bytes.ToPython();
                // _doc.from_bytes(pyObj);

                // We need to make use of builtin function bytes()
                // Taken from:
                //      https://github.com/pythonnet/pythonnet/issues/1150
                var builtins = Py.Import("builtins");
                var toBytesFunc = builtins.GetAttr("bytes");
                var pyBytes = toBytesFunc.Invoke(bytes.ToPython());
                _doc.from_bytes(pyBytes);
            }
        }
    }
}
