using System;
using System.Collections.Generic;
using System.Text;
using Python.Runtime;

namespace SpacyDotNet
{
    public class Span
    {
        private dynamic _span;

        private string _text;
        private string _label;

        public Span(dynamic sentence)
        {
            _span = sentence;
        }

        public string Text
        {
            get
            {
                if (!string.IsNullOrEmpty(_text))
                    return _text;

                using (Py.GIL())
                {
                    var textPy = new PyString(_span.text);
                    _text = textPy.ToString();
                    return _text;
                }
            }
        }

        public string Label
        {
            get
            {
                if (!string.IsNullOrEmpty(_label))
                    return _label;

                using (Py.GIL())
                {
                    var textPy = new PyString(_span.label_);
                    _label = textPy.ToString();
                    return _label;
                }
            }
        }
    }
}
