namespace Tools
{
    public class CompareString
    {
        public string Value { get; set; }
        public CompareStringType Type { get; set; }

        private CompareString()
        { }

        public static CompareString Bypass => new CompareString { Type = CompareStringType.Bypass };
        public static CompareString Equals(string value) => new CompareString { Type = CompareStringType.Equals, Value = value };
        public static CompareString DoesNotEqual(string value) => new CompareString { Type = CompareStringType.DoesNotEqual, Value = value };
        public static CompareString StartsWith(string value) => new CompareString { Type = CompareStringType.StartsWith, Value = value };
    }

    public enum CompareStringType
    {
        Bypass,
        Equals,
        DoesNotEqual,
        StartsWith
    }
}
