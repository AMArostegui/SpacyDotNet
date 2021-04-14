using System;
using System.Collections.Generic;
using System.Text;

namespace SpacyDotNet
{
    public class Lexeme
    {
        private dynamic _lexeme;

        private string _text;        
        private string _shape;
        private string _prefix;
        private string _suffix;
        private string _lang;

        private long? _orth;

        private bool? _isAlpha;
        private bool? _isDigit;
        private bool? _isTitle;

        internal Lexeme(dynamic lexeme)
        {
            _lexeme = lexeme;
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
                return ToPythonHelpers.GetString(_lexeme.text, ref _text);
            }
        }

        public string Shape
        {
            get
            {
                return ToPythonHelpers.GetString(_lexeme.shape_, ref _shape);
            }
        }

        public string Prefix
        {
            get
            {
                return ToPythonHelpers.GetString(_lexeme.prefix_, ref _prefix);
            }
        }

        public string Suffix
        {
            get
            {
                return ToPythonHelpers.GetString(_lexeme.suffix_, ref _suffix);
            }
        }


        public string Lang
        {
            get
            {
                return ToPythonHelpers.GetString(_lexeme.lang_, ref _lang);
            }
        }

        public long Orth
        {
            get
            {
                return ToPythonHelpers.GetLong(_lexeme.orth, ref _orth);
            }
        }


        public bool IsAlpha
        {
            get
            {
                return ToPythonHelpers.GetBool(_lexeme.is_alpha, ref _isAlpha);
            }
        }

        public bool IsDigit
        {
            get
            {
                return ToPythonHelpers.GetBool(_lexeme.is_digit, ref _isDigit);
            }
        }

        public bool IsTitle
        {
            get
            {
                return ToPythonHelpers.GetBool(_lexeme.is_title, ref _isTitle);
            }
        }
    }
}
