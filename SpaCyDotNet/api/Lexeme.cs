using System;
using System.Numerics;
using System.Runtime.Serialization;

namespace SpacyDotNet
{
    [Serializable]
    public class Lexeme : ISerializable
    {
        private dynamic _pyLexeme;

        private string _text;        
        private string _shape;
        private string _prefix;
        private string _suffix;
        private string _lang;

        private BigInteger? _orth;

        private bool? _isAlpha;
        private bool? _isDigit;
        private bool? _isTitle;

        public Lexeme()
        {
            // Needed to implement ISerializable
        }

        protected Lexeme(SerializationInfo info, StreamingContext context)
        {
            _text = info.GetString("Text");
            _shape = info.GetString("Shape");
            _prefix = info.GetString("Prefix");
            _suffix = info.GetString("Suffix");
            _lang = info.GetString("Lang");

            var tempBI = new BigInteger();
            _orth = (BigInteger)info.GetValue("Orth", tempBI.GetType());

            var tempBool = false;
            _isAlpha = (bool)info.GetValue("IsAlpha", tempBool.GetType());
            _isDigit = (bool)info.GetValue("IsDigit", tempBool.GetType());
            _isTitle = (bool)info.GetValue("IsTitle", tempBool.GetType());
        }

        internal Lexeme(dynamic lexeme)
        {
            _pyLexeme = lexeme;
            _text = null;            
            _shape = null;
            _prefix = null;
            _lang = null;

            _orth = null;

            _isAlpha = null;
            _isDigit = null;
            _isTitle = null;
        }

        public string Text
        {
            get
            {
                return Helpers.GetString(_pyLexeme.text, ref _text);
            }
        }

        public string Shape
        {
            get
            {
                return Helpers.GetString(_pyLexeme.shape_, ref _shape);
            }
        }

        public string Prefix
        {
            get
            {
                return Helpers.GetString(_pyLexeme.prefix_, ref _prefix);
            }
        }

        public string Suffix
        {
            get
            {
                return Helpers.GetString(_pyLexeme.suffix_, ref _suffix);
            }
        }


        public string Lang
        {
            get
            {
                return Helpers.GetString(_pyLexeme.lang_, ref _lang);
            }
        }

        public BigInteger Orth
        {
            get
            {
                return Helpers.GetBigInteger(_pyLexeme.orth, ref _orth);
            }
        }

        public bool IsAlpha
        {
            get
            {
                return Helpers.GetBool(_pyLexeme.is_alpha, ref _isAlpha);
            }
        }

        public bool IsDigit
        {
            get
            {
                return Helpers.GetBool(_pyLexeme.is_digit, ref _isDigit);
            }
        }

        public bool IsTitle
        {
            get
            {
                return Helpers.GetBool(_pyLexeme.is_title, ref _isTitle);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using the property is important form the members to be loaded
            info.AddValue("Text", Text);
            info.AddValue("Shape", Shape);
            info.AddValue("Prefix", Prefix);
            info.AddValue("Suffix", Suffix);
            info.AddValue("Lang", Lang);

            info.AddValue("Orth", Orth);

            info.AddValue("IsAlpha", IsAlpha);
            info.AddValue("IsDigit", IsDigit);
            info.AddValue("IsTitle", IsTitle);
        }
    }
}
