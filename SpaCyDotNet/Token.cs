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

        public Token(dynamic token)
        {
            _isAlpha = null;
            _isStop = null;
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

        public string Dep
        {
            get
            {
                if (!string.IsNullOrEmpty(_dep))
                    return _dep;

                using (Py.GIL())
                {
                    var depPy = new PyString(_token.dep_);
                    _dep = depPy.ToString();
                    return _dep;
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
                return Utils.GetBool(_token.is_alpha, ref _isAlpha);
            }
        }

        public bool IsStop
        {
            get
            {
                return Utils.GetBool(_token.is_stop, ref _isStop);
            }
        }

        public bool IsPunct
        {
            get
            {
                return Utils.GetBool(_token.is_punct, ref _isPunct);
            }
        }

        public bool IsDigit
        {
            get
            {
                return Utils.GetBool(_token.is_digit, ref _isDigit);
            }
        }

        public bool LikeNum
        {
            get
            {
                return Utils.GetBool(_token.like_num, ref _likeNum);
            }
        }

        public bool LikeEMail
        {
            get
            {
                return Utils.GetBool(_token.like_email, ref _likeEMail);
            }
        }

        public bool HasVector
        {
            get
            {
                return Utils.GetBool(_token.has_vector, ref _hasVector);
            }
        }

        public double VectorNorm
        {
            get
            {
                if (_vectorNorm != null)
                    return (double)_vectorNorm;

                using (Py.GIL())
                {
                    var vectorNormPy = _token.vector_norm;
                    var vectorNormFloatPy = PyFloat.AsFloat(vectorNormPy);
                    _vectorNorm = vectorNormFloatPy.As<double>();
                    return (double)_vectorNorm;
                }
            }
        }

        public bool IsOov
        {
            get
            {
                return Utils.GetBool(_token.is_oov, ref _isOov);
            }
        }
    }
}
