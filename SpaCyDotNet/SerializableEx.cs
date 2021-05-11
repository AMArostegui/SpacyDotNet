using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SpacyDotNet
{
    /// <summary>
    /// This class aims to extend serialization, by including the .NET properties in the process
    /// Normally, spacyDotNet works by calling Python code on demand after a property has been read.
    /// After the first call, if a property is read again, its values are fetch from a .NET POCO object
    /// We're going to invoke all properties when serializing, and store their values, to be restored after deserialization
    /// We'll pay a penalty serializing, but after restore all values are available with no further Python call needed
    /// We still need native spaCy serialization to support full object restoration
    /// </summary>
    [Serializable]
    public class SerializableEx : ISerializable
    {
        private byte[] _native;
        private byte[] _wrapper;

        public byte[] Native { get => _native; }
        public byte[] Wrapper { get => _wrapper; }
        
        public SerializableEx()
        {
        }

        protected SerializableEx(SerializationInfo info, StreamingContext context)
        {
            var temp = new byte[1];
            _native = (byte[])info.GetValue("Native", temp.GetType());
            _wrapper = (byte[])info.GetValue("Wrapper", temp.GetType());
        }

        public byte[] ToBytes(Doc doc)
        {
            _native = doc.ToBytes();

            var formatter = new BinaryFormatter();

            var streamWrapper = new MemoryStream();
            formatter.Serialize(streamWrapper, doc);
            _wrapper = streamWrapper.ToArray();

            var streamAgg = new MemoryStream();
            formatter.Serialize(streamAgg, this);

            return streamAgg.ToArray();
        }

        public Doc FromBytes(byte[] bytes)
        {
            var formatter = new BinaryFormatter();

            var streamAgg = new MemoryStream(bytes);
            var fastSerializable = (SerializableEx)formatter.Deserialize(streamAgg);

            var streamWrapper = new MemoryStream(fastSerializable.Wrapper);
            var doc = (Doc)formatter.Deserialize(streamWrapper);

            doc.FromBytes(fastSerializable.Native);
            return doc;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Native", _native);
            info.AddValue("Wrapper", _wrapper);
        }
    }
}
