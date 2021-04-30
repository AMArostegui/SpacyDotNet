using System.Collections.Generic;
using System.Text;
using Python.Runtime;

namespace SpacyDotNet
{
    public class Lang
    {
        private dynamic _lang;

        private List<string> _pipeNames;

        internal Lang(dynamic lang)
        {
            _lang = lang;
            _pipeNames = null;
        }

        public Doc GetDocument(string text)
        {
            using (Py.GIL())
            {
                var pyString = new PyString(text);
                dynamic doc = _lang.__call__(pyString);
                return new Doc(doc);
            }
        }

        public List<string> PipeNames
        {
            get
            {
                return ToPythonHelpers.GetListBuiltInType<string>(_lang.pipe_names, ref _pipeNames);
            }
        }

        public Vocab Vocab
        {
            get
            {
                return new Vocab(_lang.vocab);
            }
        }
    }
}
