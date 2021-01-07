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
        private string _shape;

        private bool? _isAlpha;
        private bool? _isPunct;
        private bool? _isDigit;
        private bool? _likeNum;
        private bool? _likeEMail;

        public Token(dynamic token)
        {
            _isAlpha = null;
            _isPunct = null;
            _isDigit = null;
            _likeNum = null;
            _likeEMail = null;

            _token = token;
        }

        public string Text
        {
            get
            {
                if (!string.IsNullOrEmpty(_text))
                    return _text;

                using (Py.GIL())
                {
                    var textPy = new PyString(_token.text);
                    _text = textPy.ToString();
                    return _text;
                }                 
            }
        }

        public string Lemma
        {
            get
            {
                if (!string.IsNullOrEmpty(_lemma))
                    return _lemma;

                using (Py.GIL())
                {
                    var textPy = new PyString(_token.lemma_);
                    _lemma = textPy.ToString();
                    return _lemma;
                }
            }
        }

        public string PoS
        {
            get
            {
                if (!string.IsNullOrEmpty(_pos))
                    return _pos;

                using (Py.GIL())
                {
                    var posPy = new PyString(_token.pos_);
                    _pos = posPy.ToString();
                    return _pos;
                }
            }
        }

        public string Tag
        {
            get
            {
                if (!string.IsNullOrEmpty(_tag))
                    return _tag;

                using (Py.GIL())
                {
                    var tagPy = new PyString(_token.tag_);
                    _tag = tagPy.ToString();
                    return _tag;
                }
            }
        }

        public string Shape
        {
            get
            {
                if (!string.IsNullOrEmpty(_shape))
                    return _shape;

                using (Py.GIL())
                {
                    var shapePy = new PyString(_token.shape_);
                    _shape = shapePy.ToString();
                    return _shape;
                }
            }
        }

        public bool IsAlpha
        {
            get
            {
                if (_isAlpha != null)
                    return (bool)_isAlpha;

                using (Py.GIL())
                {
                    var isAlphaPy = new PyInt(_token.is_alpha);
                    _isAlpha = isAlphaPy.ToInt32() != 0;
                    return (bool)_isAlpha;
                }
            }
        }

        public bool IsPunct
        {
            get
            {
                if (_isPunct != null)
                    return (bool)_isPunct;

                using (Py.GIL())
                {
                    var isPunctPy = new PyInt(_token.is_punct);
                    _isPunct = isPunctPy.ToInt32() != 0;
                    return (bool)_isPunct;
                }
            }
        }

        public bool IsDigit
        {
            get
            {
                if (_isDigit != null)
                    return (bool)_isDigit;

                using (Py.GIL())
                {
                    var isDigitPy = new PyInt(_token.is_digit);
                    _isDigit = isDigitPy.ToInt32() != 0;
                    return (bool)_isDigit;
                }
            }
        }

        public bool LikeNum
        {
            get
            {
                if (_likeNum != null)
                    return (bool)_likeNum;

                using (Py.GIL())
                {
                    var isLikeNumPy = new PyInt(_token.like_num);
                    _likeNum = isLikeNumPy.ToInt32() != 0;
                    return (bool)_likeNum;
                }
            }
        }

        public bool LikeEMail
        {
            get
            {
                if (_likeEMail != null)
                    return (bool)_likeEMail;

                using (Py.GIL())
                {
                    var isLikeEMailPy = new PyInt(_token.like_email);
                    _likeEMail = isLikeEMailPy.ToInt32() != 0;
                    return (bool)_likeEMail;
                }
            }
        }
    }
}
