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

        public Lexeme(dynamic lexeme)
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
                return Utils.GetString(_lexeme.text, ref _text);
            }
        }

        public string Shape
        {
            get
            {
                return Utils.GetString(_lexeme.shape_, ref _shape);
            }
        }

        public string Prefix
        {
            get
            {
                return Utils.GetString(_lexeme.prefix_, ref _prefix);
            }
        }

        public string Suffix
        {
            get
            {
                return Utils.GetString(_lexeme.suffix_, ref _suffix);
            }
        }


        public string Lang
        {
            get
            {
                return Utils.GetString(_lexeme.lang_, ref _lang);
            }
        }

        public long Orth
        {
            get
            {
                return Utils.GetLong(_lexeme.orth, ref _orth);
            }
        }


        public bool IsAlpha
        {
            get
            {
                return Utils.GetBool(_lexeme.is_alpha, ref _isAlpha);
            }
        }

        public bool IsDigit
        {
            get
            {
                return Utils.GetBool(_lexeme.is_digit, ref _isDigit);
            }
        }

        public bool IsTitle
        {
            get
            {
                return Utils.GetBool(_lexeme.is_title, ref _isTitle);
            }
        }
    }
}
