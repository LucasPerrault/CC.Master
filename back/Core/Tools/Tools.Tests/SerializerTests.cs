using FluentAssertions;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace Tools.Tests
{
    public enum TestSubjectTypes
    {
        Human,
        Robot
    }

    public enum TestToolTypes
    {
        Valuable,
        Disposable
    }

    public interface ITestSubject
    {
        public TestSubjectTypes Type { get; }
    }

    public class HumanTestSubject : ITestSubject
    {
        public TestSubjectTypes Type => TestSubjectTypes.Human;
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class RobotTestSubject : ITestSubject
    {
        public TestSubjectTypes Type => TestSubjectTypes.Robot;
        public string Name { get; set; }
        public string Version { get; set; }
    }


    public interface ITestTool
    {
        TestToolTypes Type { get; }
    }

    public class DisposableTestTool : ITestTool
    {
        public TestToolTypes Type => TestToolTypes.Disposable;
        public string Name { get; set; }
        public bool IsDisposed { get; set; }
    }

    public class ValuableTestTool : ITestTool
    {

        public TestToolTypes Type => TestToolTypes.Valuable;
        public string Name { get; set; }
        public ITestSubject Owner { get; set; }
    }

    public class GladosInventory
    {
        public List<ITestSubject> Subjects { get; set; }
        public List<ITestTool> TestTools { get; set; }
    }

    public class SerializerTests
    {
        [Fact]
        public void ShouldSerializeAndDeserialize()
        {

            var inventory = new GladosInventory
            {
                Subjects = new List<ITestSubject>
                {
                    new HumanTestSubject { Name = "Shell", Age = 999999999 },
                    new RobotTestSubject { Name = "ATLAS", Version = "v0.0.42" }
                },
                TestTools = new List<ITestTool>
                {
                    new DisposableTestTool { Name = "Weighted Storage Cube", IsDisposed = false },
                    new ValuableTestTool
                    {
                        Name = "Aperture Science Handheld Portal Device", Owner = new RobotTestSubject
                        {
                            Name = "P-Body",
                            Version = "v1.2.3"
                        }
                    }
                }
            };

            var serializer = new EmptyPolymorphicSerializerBuilder()
                .WithPolymorphism<ITestSubject, TestSubjectTypes>(nameof(ITestSubject.Type))
                    .AddMatch<HumanTestSubject>(TestSubjectTypes.Human)
                    .AddMatch<RobotTestSubject>(TestSubjectTypes.Robot)
                .WithPolymorphism<ITestTool, TestToolTypes>(nameof(ITestTool.Type))
                    .AddMatch<DisposableTestTool>(TestToolTypes.Disposable)
                    .AddMatch<ValuableTestTool>(TestToolTypes.Valuable)
                .Build();

            var serialized = Serializer.Serialize(inventory);
            var parsed = serializer.Deserialize<GladosInventory>(serialized);
            parsed.Should().BeEquivalentTo(inventory);
        }

        [Fact]
        public void ShouldDeserializeAnonymousTypes()
        {

            var inventory = new
            {
                Subjects = new List<object>
                {
                    new { Type = TestSubjectTypes.Human, Name = "Shell", Age = 999999999 },
                    new { Type = TestSubjectTypes.Robot, Name = "ATLAS", Version = "v0.0.42" }
                },
                TestTools = new List<object>
                {
                    new { Type = TestToolTypes.Disposable, Name = "Weighted Storage Cube", IsDisposed = false },
                    new
                    {
                        Type = TestToolTypes.Valuable,
                        Name = "Aperture Science Handheld Portal Device",
                        Owner = new { Type = TestSubjectTypes.Robot, Name = "P-Body", Version = "v1.2.3" }
                    }
                }
            };

            var serializer = new EmptyPolymorphicSerializerBuilder()
                .WithPolymorphism<ITestSubject, TestSubjectTypes>(nameof(ITestSubject.Type))
                    .AddMatch<HumanTestSubject>(TestSubjectTypes.Human)
                    .AddMatch<RobotTestSubject>(TestSubjectTypes.Robot)
                .WithPolymorphism<ITestTool, TestToolTypes>(nameof(ITestTool.Type))
                    .AddMatch<DisposableTestTool>(TestToolTypes.Disposable)
                    .AddMatch<ValuableTestTool>(TestToolTypes.Valuable)
                .Build();

            var serialized = Serializer.Serialize(inventory);
            var parsed = serializer.Deserialize<GladosInventory>(serialized);
            parsed.Should().BeEquivalentTo(inventory);
        }

        [Fact]
        public void ShouldPanicWhenMatchIsMissing()
        {

            var inventory = new
            {
                Subjects = new List<object>
                {
                    new { Type = TestSubjectTypes.Human, Name = "Shell", Age = 999999999 },
                    new { Type = TestSubjectTypes.Robot, Name = "ATLAS", Version = "v0.0.42" }
                }
            };

            var serializer = new EmptyPolymorphicSerializerBuilder()
                .WithPolymorphism<ITestSubject, TestSubjectTypes>(nameof(ITestSubject.Type))
                .AddMatch<HumanTestSubject>(TestSubjectTypes.Human)
                .Build();

            var serialized = Serializer.Serialize(inventory);
            Assert.Throws<JsonException>(() => serializer.Deserialize<GladosInventory>(serialized));
        }
    }
}
