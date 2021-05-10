﻿using System;
using System.Runtime.Serialization;

namespace SpacyDotNet
{
    [Serializable]
    public class Span : ISerializable
    {
        private dynamic _span;

        private string _text;
        private string _label;
        private int? _startChar;
        private int? _endChar;

        public Span()
        {
            // Needed to use generics and to implement ISerializable
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
                return ToPythonHelpers.GetString(_span.text, ref _text);
            }
        }

        public string Label
        {
            get
            {
                return ToPythonHelpers.GetString(_span.label_, ref _label);
            }
        }

        public int StartChar
        {
            get
            {
                return ToPythonHelpers.GetInt(_span.start_char, ref _startChar);
            }
        }

        public int EndChar
        {
            get
            {
                return ToPythonHelpers.GetInt(_span.end_char, ref _endChar);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using the property is important form the members to be loaded
            info.AddValue("Text", Text);
            info.AddValue("Label", Label);
            info.AddValue("StartChar", StartChar);
            info.AddValue("EndChar", EndChar);
        }
    }
}
