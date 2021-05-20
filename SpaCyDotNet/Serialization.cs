using System;

namespace SpacyDotNet
{
    [Flags]
    public enum Serialization
    {
        Spacy = 1,
        DotNet = 2
    }

    public static class SerializationEx
    {
        public static bool IsSpacy(this Serialization value)
        {
            return (value & Serialization.Spacy) != 0;
        }

        public static bool IsDotNet(this Serialization value)
        {
            return (value & Serialization.DotNet) != 0;
        }
    }
}
