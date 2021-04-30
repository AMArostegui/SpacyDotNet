﻿using System.Collections.Generic;
using Python.Runtime;

namespace SpacyDotNet
{
    public class Token
    {
        private dynamic _token;

        private string _text;
        private string _lemma;

        private string _pos;
        private string _tag;
        private string _dep;
        private string _shape;

        private bool? _isAlpha;
        private bool? _isStop;
        private bool? _isPunct;
        private bool? _isDigit;
        private bool? _likeNum;
        private bool? _likeEMail;

        private bool? _hasVector;
        private double? _vectorNorm;
        private bool? _isOov;

        private Token _head;
        private List<Token> _children;

        public Token()
        {
            // Needed just to use generics
        }

        internal Token(dynamic token)
        {
            _isAlpha = null;
            _isStop = null;
            _isPunct = null;
            _isDigit = null;
            _likeNum = null;
            _likeEMail = null;

            _token = token;
        }

        internal dynamic PyObj
            { get { return _token; } }

        public string Text
        {
            get
            {
                return ToPythonHelpers.GetString(_token.text, ref _text);
            }
        }

        public string Lemma
        {
            get
            {
                return ToPythonHelpers.GetString(_token.lemma_, ref _lemma);
            }
        }

        public string PoS
        {
            get
            {
                return ToPythonHelpers.GetString(_token.pos_, ref _pos);
            }
        }

        public string Tag
        {
            get
            {
                return ToPythonHelpers.GetString(_token.tag_, ref _tag);
            }
        }

        public string Dep
        {
            get
            {
                return ToPythonHelpers.GetString(_token.dep_, ref _dep);
            }
        }

        public string Shape
        {
            get
            {
                return ToPythonHelpers.GetString(_token.shape_, ref _shape);
            }
        }

        public bool IsAlpha
        {
            get
            {
                return ToPythonHelpers.GetBool(_token.is_alpha, ref _isAlpha);
            }
        }

        public bool IsStop
        {
            get
            {
                return ToPythonHelpers.GetBool(_token.is_stop, ref _isStop);
            }
        }

        public bool IsPunct
        {
            get
            {
                return ToPythonHelpers.GetBool(_token.is_punct, ref _isPunct);
            }
        }

        public bool IsDigit
        {
            get
            {
                return ToPythonHelpers.GetBool(_token.is_digit, ref _isDigit);
            }
        }

        public bool LikeNum
        {
            get
            {
                return ToPythonHelpers.GetBool(_token.like_num, ref _likeNum);
            }
        }

        public bool LikeEMail
        {
            get
            {
                return ToPythonHelpers.GetBool(_token.like_email, ref _likeEMail);
            }
        }

        public bool HasVector
        {
            get
            {
                return ToPythonHelpers.GetBool(_token.has_vector, ref _hasVector);
            }
        }

        public double VectorNorm
        {
            get
            {
                return ToPythonHelpers.GetDouble(_token.vector_norm, ref _vectorNorm);
            }
        }

        public bool IsOov
        {
            get
            {
                return ToPythonHelpers.GetBool(_token.is_oov, ref _isOov);
            }
        }

        public Token Head
        {
            get
            {
                if (_head != null)
                    return _head;

                using (Py.GIL())
                {
                    _head = new Token(_token.head);
                    return _head;
                }
            }
        }

        public List<Token> Children
        {
            get
            {
                return ToPythonHelpers.GetListWrapperObj(_token.children, ref _children);
            }
        }

        public double Similarity(Token token)
        {
            using (Py.GIL())
            {                
                dynamic similarityPy = _token.similarity(token.PyObj);
                var similarityPyFloat = PyFloat.AsFloat(similarityPy);
                return similarityPyFloat.As<double>();
            }
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
