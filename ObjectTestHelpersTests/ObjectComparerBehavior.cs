using NUnit.Framework;
using ObjectTestHelpers;

namespace ObjectTestHelpersTests
{
    [TestFixture]
    public class ObjectComparerBehavior
    {
        [Test]
        public void AssertDifference_test()
        {
            var obj1 = new TestClass() { Value1 = "1", Value2 = "2", Field1 = "3", Composite = new TestClass() { Value1 = "111" } };
            obj1.Dict.Add("a", "value1");
            var obj2 = new TestClass() { Value1 = "2", Value2 = "3", Field1 = "4", Composite = new TestClass() { Value1 = "222"}};
            obj2.Dict.Add("a", "value2");

            var comparer = new ObjectComparer();
            comparer.AssertDifference(obj1, obj2, new[]
            {
                "/Composite/Value2 - null",
                "/Composite/Composite - null",
                "/Composite/Dict - no elements found",
                "/Composite/Field1 - null"
            });
        }

        [Test]
        [ExpectedException(typeof(AssertDifferenceException))]
        public void AssertDifference_should_error_if_any_propertiesToIgnore_is_redundant()
        {
            var obj1 = new TestClass() { Value1 = "1", Value2 = "2", Composite = new TestClass() { Value1 = "111" } };
            var obj2 = new TestClass() { Value1 = "2", Value2 = "3", Composite = new TestClass() { Value1 = "222" } };

            var comparer = new ObjectComparer();
            comparer.AssertDifference(obj1, obj2, new[]
            {
                "/Composite/Value2 - null",
                "/Composite/Composite - null",
                "redundant property to ignore"
            });
        }

        [Test]
        public void AssertEquality_test()
        {
            var obj1 = new TestClass() { Value1 = "1", Value2 = "2", Field1 = "3", Composite = new TestClass() { Value1 = "111" } };
            obj1.Dict.Add("a", "value1");
            var obj2 = new TestClass() { Value1 = "1", Value2 = "2", Field1 = "3", Composite = new TestClass() { Value1 = "111" } };
            obj2.Dict.Add("a", "value2");

            var comparer = new ObjectComparer();
            comparer.AssertEquality(obj1, obj2, new[]
            {
                "/Dict a - value1"
            });
        }
    }
}
