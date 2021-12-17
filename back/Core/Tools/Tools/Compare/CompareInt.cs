namespace Tools
{
    public class CompareInt
    {
        public int Value { get; set; }
        public CompareIntType Type { get; set; }

        private CompareInt()
        { }

        public static CompareInt Bypass => new CompareInt { Type = CompareIntType.Bypass };
        public static CompareInt Equals(int value) => new CompareInt { Type = CompareIntType.Equals, Value = value };
        public static CompareInt DoesNotEqual(int value) => new CompareInt { Type = CompareIntType.DoesNotEqual, Value = value };
    }

    public enum CompareIntType
    {
        Bypass,
        Equals,
        DoesNotEqual,
    }
}
