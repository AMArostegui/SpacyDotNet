﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Python.Runtime;

namespace SpacyDotNet
{
    public class Token : IXmlSerializable
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

        private int? _i;

        private Token _head;
        private int _headPos;

        private List<Token> _children;

        public Token()
        {
            // Needed to use generics
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
            { get => _pyToken; } 

        public string Text
        {
            get
            {
                return Interop.GetString(_pyToken?.text, ref _text);
            }
        }

        public string Lemma
        {
            get
            {
                return Interop.GetString(_pyToken?.lemma_, ref _lemma);
            }
        }

        public string PoS
        {
            get
            {
                return Interop.GetString(_pyToken?.pos_, ref _pos);
            }
        }

        public string Tag
        {
            get
            {
                return Interop.GetString(_pyToken?.tag_, ref _tag);
            }
        }

        public string Dep
        {
            get
            {
                return Interop.GetString(_pyToken?.dep_, ref _dep);
            }
        }

        public string Shape
        {
            get
            {
                return Interop.GetString(_pyToken?.shape_, ref _shape);
            }
        }

        public bool IsAlpha
        {
            get
            {
                return Interop.GetBool(_pyToken?.is_alpha, ref _isAlpha);
            }
        }

        public bool IsStop
        {
            get
            {
                return Interop.GetBool(_pyToken?.is_stop, ref _isStop);
            }
        }

        public bool IsPunct
        {
            get
            {
                return Interop.GetBool(_pyToken?.is_punct, ref _isPunct);
            }
        }

        public bool IsDigit
        {
            get
            {
                return Interop.GetBool(_pyToken?.is_digit, ref _isDigit);
            }
        }

        public bool LikeNum
        {
            get
            {
                return Interop.GetBool(_pyToken?.like_num, ref _likeNum);
            }
        }

        public bool LikeEMail
        {
            get
            {
                return Interop.GetBool(_pyToken?.like_email, ref _likeEMail);
            }
        }

        public bool HasVector
        {
            get
            {
                return Interop.GetBool(_pyToken?.has_vector, ref _hasVector);
            }
        }

        public double VectorNorm
        {
            get
            {
                return Interop.GetDouble(_pyToken?.vector_norm, ref _vectorNorm);
            }
        }

        public bool IsOov
        {
            get
            {
                return Interop.GetBool(_pyToken?.is_oov, ref _isOov);
            }
        }

        public int I
        {
            get
            {
                return Interop.GetInt(_pyToken?.i, ref _i);
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
                    var pyHeadIsSelf = new PyInt(_pyToken.head.__eq__(_pyToken));
                    var headIsSelf = pyHeadIsSelf.ToInt32() != 0;
                    if (headIsSelf)
                        _head = this;
                    else
                        _head = new Token(_pyToken.head);

                    return _head;
                }
            }

            set
            {
                _head = value;
            }
        }

        public List<Token> Children
        {
            get
            {
                return Interop.GetListFromCollection(_pyToken?.children, ref _children);
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

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Debug.Assert(reader.Name == "Text");
            _text = reader.ReadElementContentAsString();
            Debug.Assert(reader.Name == "Lemma");
            _lemma = reader.ReadElementContentAsString();

            Debug.Assert(reader.Name == "Pos");
            _pos = reader.ReadElementContentAsString();
            Debug.Assert(reader.Name == "Tag");
            _tag = reader.ReadElementContentAsString();
            Debug.Assert(reader.Name == "Dep");
            _dep = reader.ReadElementContentAsString();
            Debug.Assert(reader.Name == "Shape");
            _shape = reader.ReadElementContentAsString();

            Debug.Assert(reader.Name == "IsAlpha");
            _isAlpha = reader.ReadElementContentAsBoolean();
            Debug.Assert(reader.Name == "IsStop");
            _isStop = reader.ReadElementContentAsBoolean();
            Debug.Assert(reader.Name == "IsPunct");
            _isPunct = reader.ReadElementContentAsBoolean();
            Debug.Assert(reader.Name == "IsDigit");
            _isDigit = reader.ReadElementContentAsBoolean();
            Debug.Assert(reader.Name == "LikeNum");
            _likeNum = reader.ReadElementContentAsBoolean();
            Debug.Assert(reader.Name == "LikeEMail");
            _likeEMail = reader.ReadElementContentAsBoolean();

            Debug.Assert(reader.Name == "HasVector");
            _hasVector = reader.ReadElementContentAsBoolean();
            Debug.Assert(reader.Name == "VectorNorm");
            _vectorNorm = reader.ReadElementContentAsDouble();
            Debug.Assert(reader.Name == "IsOov");
            _isOov = reader.ReadElementContentAsBoolean();

            Debug.Assert(reader.Name == "I");
            _i = reader.ReadElementContentAsInt();

            Debug.Assert(reader.Name == "Head");
            var headPosStr = reader.GetAttribute("Pos");
            if (string.IsNullOrEmpty(headPosStr))
                _headPos = -1;
            else
                _headPos = int.Parse(headPosStr);
                
            reader.Skip();
        }

        public void WriteXml(XmlWriter writer)
        {
            // Using the property is important form the members to be loaded
            writer.WriteElementString("Text", Text);
            writer.WriteElementString("Lemma", Lemma);

            writer.WriteElementString("Pos", PoS);
            writer.WriteElementString("Tag", Tag);
            writer.WriteElementString("Dep", Dep);
            writer.WriteElementString("Shape", Shape);

            writer.WriteStartElement("IsAlpha");
            writer.WriteValue(IsAlpha);
            writer.WriteEndElement();
            writer.WriteStartElement("IsStop");
            writer.WriteValue(IsStop);
            writer.WriteEndElement();
            writer.WriteStartElement("IsPunct");
            writer.WriteValue(IsPunct);
            writer.WriteEndElement();
            writer.WriteStartElement("IsDigit");
            writer.WriteValue(IsDigit);
            writer.WriteEndElement();
            writer.WriteStartElement("LikeNum");
            writer.WriteValue(LikeNum);
            writer.WriteEndElement();
            writer.WriteStartElement("LikeEMail");
            writer.WriteValue(LikeEMail);
            writer.WriteEndElement();

            writer.WriteStartElement("HasVector");
            writer.WriteValue(HasVector);
            writer.WriteEndElement();
            writer.WriteStartElement("VectorNorm");
            writer.WriteValue(VectorNorm);
            writer.WriteEndElement();
            writer.WriteStartElement("IsOov");
            writer.WriteValue(IsOov);
            writer.WriteEndElement();

            writer.WriteStartElement("I");
            writer.WriteValue(I);
            writer.WriteEndElement();

            writer.WriteStartElement("Head");
            var head = Head;
            if (head == this)
                writer.WriteAttributeString("Pos", string.Empty);
            else
                writer.WriteAttributeString("Pos", head.I.ToString());
            writer.WriteEndElement();

            // This one was already commented
            //info.AddValue("Children", Children);
        }

        internal void RestoreHead(List<Token> tokens)
        {
            if (_headPos == -1)
                _head = this;
            else
                _head = tokens[_headPos];
        }
    }
}
