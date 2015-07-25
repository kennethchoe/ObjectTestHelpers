using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ObjectTestHelpers;

namespace ObjectTestHelpersTests
{
    [TestFixture]
    public class SampleValueSetterBehavior
    {
        private readonly SampleValueSetter _sampleValueSetter;

        public SampleValueSetterBehavior()
        {
            _sampleValueSetter = new SampleValueSetter();
        }

        [Test]
        public void DifferentSeedShouldMakeEverythingDifferent()
        {
            var obj1 = new SampleClass();
            var obj2 = new SampleClass();
            _sampleValueSetter.AssignSampleValues(obj1, 1);
            _sampleValueSetter.AssignSampleValues(obj2, 2);

        }

        [Test]
        public void SampleValueShouldStartFromSeed()
        {
            var obj = new SampleClass();
            _sampleValueSetter.AssignSampleValues(obj, 1, processFields: true, processProperties: false);
            Assert.That(obj.Field1, Is.EqualTo("2"));
            Assert.Null(obj.Prop1);

            _sampleValueSetter.AssignSampleValues(obj, 2, processFields: false, processProperties: true);
            Assert.That(obj.Field1, Is.EqualTo("2"));
            Assert.That(obj.Prop1, Is.EqualTo("3"));
        }

        [Test]
        public void SampleValueWithChildObject()
        {
            var obj = new SampleClass();
            obj.Child = new SampleClass();
            _sampleValueSetter.AssignSampleValues(obj, 1);
            Assert.NotNull(obj.Child.Field1);
        }

        [Test]
        public void SampleValueWithChildrenArray()
        {
            var obj = new SampleClass();
            obj.Children.Add(new SampleClass());
            obj.Children.Add(new SampleClass());
            _sampleValueSetter.AssignSampleValues(obj, 1);
            Assert.NotNull(obj.Children.First().Field1);
            Assert.NotNull(obj.Children.Last().Field1);
        }

        [Test]
        public void StringArrayTest1()
        {
            var obj = new [] {"aaa", "bbb"};

            _sampleValueSetter.AssignSampleValues(obj, 1);
            Assert.That(obj[0], Is.EqualTo("2"));
            Assert.That(obj[1], Is.EqualTo("3"));
        }

        [Test]
        public void StringArrayTest2()
        {
            var obj = new StringArray {Array = new[] {"aaa", "bbb"}};

            _sampleValueSetter.AssignSampleValues(obj, 1);
            Assert.That(obj.Array[0], Is.EqualTo("2"));
            Assert.That(obj.Array[1], Is.EqualTo("3"));
        }

        private class SampleClass
        {
            public string Prop1 { get; set; }
            public DateTime Prop2 { get; set; }

            public string Field1 = null;
            public DateTime Field2 = new DateTime();

            public SampleClass Child { get; set; }

            public List<SampleClass> Children = new List<SampleClass>();
        }

        private class StringArray
        {
            public string[] Array { get; set; }
        }
    }
}
