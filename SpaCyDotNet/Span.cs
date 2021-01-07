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
        private int? _startChar;
        private int? _endChar;

        public Span()
        {
            // Needed just to use generics
        }

        public Span(dynamic sentence)
        {
            _span = sentence;
            _startChar = null;
            _endChar = null;
        }

        public string Text
        {
            get
            {
                return Utils.GetString(_span.text, ref _text);
            }
        }

        public string Label
        {
            get
            {
                return Utils.GetString(_span.label_, ref _label);
            }
        }

        public int StartChar
        {
            get
            {
                if (_startChar != null)
                    return (int)_startChar;

                using (Py.GIL())
                {
                    var startCharPy = new PyInt(_span.start_char);
                    _startChar = startCharPy.ToInt32();
                    return (int)_startChar;
                }
            }
        }

        public int EndChar
        {
            get
            {
                if (_endChar != null)
                    return (int)_endChar;

                using (Py.GIL())
                {
                    var endCharPy = new PyInt(_span.end_char);
                    _endChar = endCharPy.ToInt32();
                    return (int)_endChar;
                }
            }
        }
    }
}
