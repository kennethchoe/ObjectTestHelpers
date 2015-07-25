using System.Linq;
using NUnit.Framework;
using ObjectTestHelpers;

namespace ObjectTestHelpersTests
{
    [TestFixture]
    class ObjectSerializerBehavior
    {
        [Test]
        public void SerializeForComparison_test()
        {
            var obj = new TestClass() { Value1 = "1", Value2 = "2", Composite = new TestClass { Value1 = "111" } };
            obj.Dict.Add("key", "value");

            var comparer = new ObjectSerializer();
            var objStrings = comparer.SerializeForComparison(obj);

            Assert.That(objStrings.Contains("/Value1 - 1"), Is.True);
            Assert.That(objStrings.Contains("/Value2 - 2"), Is.True);
            Assert.That(objStrings.Contains("/Composite/Value1 - 111"), Is.True);
        }
    }
}
