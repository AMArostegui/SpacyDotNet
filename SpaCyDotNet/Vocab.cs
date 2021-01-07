using System;
using System.Collections.Generic;
using System.Text;
using Python.Runtime;

namespace SpacyDotNet
{
    public class Vocab
    {
        private dynamic _vocab;

        private StringStore _stringStore;

        public Vocab(dynamic vocab)
        {
            _vocab = vocab;
            _stringStore = null;
        }

        public StringStore Strings
        {
            get
            {
                if (_stringStore != null)
                    return _stringStore;

                using (Py.GIL())
                {
                    var stringStore = _vocab.strings;
                    _stringStore = new StringStore(stringStore);
                    return _stringStore;
                }
            }
        }
    }
}
