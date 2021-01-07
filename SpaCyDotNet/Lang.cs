using Python.Runtime;

namespace SpacyDotNet
{
    public class Lang
    {
        private dynamic _lang;

        public Lang(dynamic lang)
        {
            _lang = lang;
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

        public string PipeNames
        {
            get
            {
                using (Py.GIL())
                {
                    var pyPipeNames = _lang.pipe_names;
                    return pyPipeNames.ToString();
                }
            }
        }
    }
}
