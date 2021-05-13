using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Python.Runtime;

namespace SpacyDotNet
{
    [Serializable]
    public class Token : ISerializable
    {
        private dynamic _pyToken;

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
            // Needed to use generics and to implement ISerializable
        }

        protected Token(SerializationInfo info, StreamingContext context)
        {
            var dummyBytes = new byte[1];

            var bytes = (byte[])info.GetValue("PyObj", dummyBytes.GetType());
            using (Py.GIL())
            {
                var pyBytes = ToPython.GetBytes(bytes);
                _pyToken.from_bytes(pyBytes);
            }

            _text = info.GetString("Text");
            _lemma = info.GetString("Lemma");

            _pos = info.GetString("Pos");
            _tag = info.GetString("Tag");
            _dep = info.GetString("Dep");
            _shape = info.GetString("Shape");

            var tempBool = false;
            _isAlpha = (bool)info.GetValue("IsAlpha", tempBool.GetType());
            _isStop = (bool)info.GetValue("IsStop", tempBool.GetType());
            _isPunct = (bool)info.GetValue("IsPunct", tempBool.GetType());
            _isDigit = (bool)info.GetValue("IsDigit", tempBool.GetType());
            _likeNum = (bool)info.GetValue("LikeNum", tempBool.GetType());
            _likeEMail = (bool)info.GetValue("LikeEMail", tempBool.GetType());

            var tempDouble = 0.0;
            _hasVector = (bool)info.GetValue("HasVector", tempBool.GetType());
            _vectorNorm = (double)info.GetValue("VectorNorm", tempDouble.GetType());
            _isOov = (bool)info.GetValue("IsOov", tempBool.GetType());

            // TODO: This needs to be reviewed
            //var tempToken = new Token();
            //_head = (Token)info.GetValue("Head", tempToken.GetType());
        }

        internal Token(dynamic token)
        {
            _isAlpha = null;
            _isStop = null;
            _isPunct = null;
            _isDigit = null;
            _likeNum = null;
            _likeEMail = null;

            _pyToken = token;
        }

        internal dynamic PyObj
            { get { return _pyToken; } }

        public string Text
        {
            get
            {
                return Helpers.GetString(_pyToken.text, ref _text);
            }
        }

        public string Lemma
        {
            get
            {
                return Helpers.GetString(_pyToken.lemma_, ref _lemma);
            }
        }

        public string PoS
        {
            get
            {
                return Helpers.GetString(_pyToken.pos_, ref _pos);
            }
        }

        public string Tag
        {
            get
            {
                return Helpers.GetString(_pyToken.tag_, ref _tag);
            }
        }

        public string Dep
        {
            get
            {
                return Helpers.GetString(_pyToken.dep_, ref _dep);
            }
        }

        public string Shape
        {
            get
            {
                return Helpers.GetString(_pyToken.shape_, ref _shape);
            }
        }

        public bool IsAlpha
        {
            get
            {
                return Helpers.GetBool(_pyToken.is_alpha, ref _isAlpha);
            }
        }

        public bool IsStop
        {
            get
            {
                return Helpers.GetBool(_pyToken.is_stop, ref _isStop);
            }
        }

        public bool IsPunct
        {
            get
            {
                return Helpers.GetBool(_pyToken.is_punct, ref _isPunct);
            }
        }

        public bool IsDigit
        {
            get
            {
                return Helpers.GetBool(_pyToken.is_digit, ref _isDigit);
            }
        }

        public bool LikeNum
        {
            get
            {
                return Helpers.GetBool(_pyToken.like_num, ref _likeNum);
            }
        }

        public bool LikeEMail
        {
            get
            {
                return Helpers.GetBool(_pyToken.like_email, ref _likeEMail);
            }
        }

        public bool HasVector
        {
            get
            {
                return Helpers.GetBool(_pyToken.has_vector, ref _hasVector);
            }
        }

        public double VectorNorm
        {
            get
            {
                return Helpers.GetDouble(_pyToken.vector_norm, ref _vectorNorm);
            }
        }

        public bool IsOov
        {
            get
            {
                return Helpers.GetBool(_pyToken.is_oov, ref _isOov);
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
                    _head = new Token(_pyToken.head);
                    return _head;
                }
            }
        }

        public List<Token> Children
        {
            get
            {
                return Helpers.GetListWrapperObj(_pyToken.children, ref _children);
            }
        }

        public double Similarity(Token token)
        {
            using (Py.GIL())
            {                
                dynamic similarityPy = _pyToken.similarity(token.PyObj);
                var similarityPyFloat = PyFloat.AsFloat(similarityPy);
                return similarityPyFloat.As<double>();
            }
        }

        public override string ToString()
        {
            return Text;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            using (Py.GIL())
            {
                var pyObj = Helpers.GetBytes(_pyToken.to_bytes());
                info.AddValue("PyObj", pyObj);
            }

            // Using the property is important form the members to be loaded
            info.AddValue("Text", Text);
            info.AddValue("Lemma", Lemma);

            info.AddValue("Pos", PoS);
            info.AddValue("Tag", Tag);
            info.AddValue("Dep", Dep);
            info.AddValue("Shape", Shape);

            info.AddValue("IsAlpha", IsAlpha);
            info.AddValue("IsStop", IsStop);
            info.AddValue("IsPunct", IsPunct);
            info.AddValue("IsDigit", IsDigit);
            info.AddValue("LikeNum", LikeNum);
            info.AddValue("LikeEMail", LikeEMail);

            info.AddValue("HasVector", HasVector);
            info.AddValue("VectorNorm", VectorNorm);
            info.AddValue("IsOov", IsOov);
            
            var headIsSelf = false;
            var pyHead = Head.PyObj;
            using (Py.GIL())
            {
                var pyHeadIsSelf = new PyInt(_pyToken.__eq__(pyHead));
                headIsSelf = pyHeadIsSelf.ToInt32() != 0;
            }

            // TODO: This needs to be reviewed
            if (!headIsSelf)
                info.AddValue("Head", Head);
            //info.AddValue("Children", Children);
        }
    }
}
