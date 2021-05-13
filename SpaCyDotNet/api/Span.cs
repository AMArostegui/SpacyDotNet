using System;
using System.Runtime.Serialization;

namespace SpacyDotNet
{
    [Serializable]
    public class Span : ISerializable
    {
        private dynamic _pySpan;

        private string _text;
        private string _label;
        private int? _startChar;
        private int? _endChar;

        public Span()
        {
            // Needed to use generics and to implement ISerializable
        }

        protected Span(SerializationInfo info, StreamingContext context)
        {
            _text = info.GetString("Text");
            _label = info.GetString("Label");

            var temp = 0;
            _startChar = (int)info.GetValue("StartChar", temp.GetType());
            _endChar = (int)info.GetValue("EndChar", temp.GetType());
        }

        internal Span(dynamic sentence)
        {
            _pySpan = sentence;
            _startChar = null;
            _endChar = null;
        }

        public string Text
        {
            get
            {
                return Helpers.GetString(_pySpan.text, ref _text);
            }
        }

        public string Label
        {
            get
            {
                return Helpers.GetString(_pySpan.label_, ref _label);
            }
        }

        public int StartChar
        {
            get
            {
                return Helpers.GetInt(_pySpan.start_char, ref _startChar);
            }
        }

        public int EndChar
        {
            get
            {
                return Helpers.GetInt(_pySpan.end_char, ref _endChar);
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
