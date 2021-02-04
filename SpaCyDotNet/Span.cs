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

        internal Span(dynamic sentence)
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
                return Utils.GetInt(_span.start_char, ref _startChar);
            }
        }

        public int EndChar
        {
            get
            {
                return Utils.GetInt(_span.end_char, ref _endChar);
            }
        }
    }
}
