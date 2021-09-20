namespace Storage.Infra.Tests.Storage
{
    internal interface IIsOk<TComparer, TCompared>
    {
        IIsKo<TComparer, TCompared> Accepts(params TCompared[] compared);
    }

    internal interface IIsKo<TComparer, TCompared>
    {
        TestElement<TComparer, TCompared> AndRejects(params TCompared[] compared);
    }

    internal class TestElement<TComparer, TCompared>
    {
        internal TComparer Comparer { get; set; }
        public TCompared[] Accepted { get; set; }
        public TCompared[] Rejected { get; set; }

        internal TestElement() { }

        public object[] ToObjects() => new object[]{ Accepted, Rejected, Comparer };

        public static IIsOk<TComparer, TCompared> ForComparer(TComparer compareDateTime) => new TestDataBuilder<TComparer, TCompared>(compareDateTime);

    }

    internal class TestDataBuilder<TComparer, TCompared> : IIsKo<TComparer, TCompared>, IIsOk<TComparer, TCompared>
    {
        private readonly TestElement<TComparer, TCompared> _testElement;

        public TestDataBuilder(TComparer comparer)
        {
            _testElement = new TestElement<TComparer, TCompared> { Comparer = comparer };
        }

        public TestElement<TComparer, TCompared> AndRejects(params TCompared[] compared)
        {
            _testElement.Rejected = compared;
            return _testElement;
        }

        public IIsKo<TComparer, TCompared> Accepts(params TCompared[] compared)
        {
            _testElement.Accepted = compared;
            return this;
        }
    }
}
