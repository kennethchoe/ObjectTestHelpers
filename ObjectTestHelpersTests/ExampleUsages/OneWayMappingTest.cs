using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ObjectTestHelpers;
using ObjectTestHelpersTests.ExampleUsages.TargetCodeSimulations;

namespace ObjectTestHelpersTests.ExampleUsages
{
    [TestFixture]
    class OneWayMappingTest
    {
        /// <summary>
        /// view model factory is one-way mapping. 
        /// this test will detect missing mappings, especially well when one domain property maps to one view model property.
        /// try commenting out one line in SampleViewModelFactory.Build() and running this test.
        /// 
        /// StaticValue needs to be ignored in the comparison, since it is supposed to be the same.
        /// </summary>
        [Test]
        public void Source_property_change_should_result_in_destination_property_change()
        {
            var setter = new SampleValueSetter();
            var comparer = new ObjectComparer();
            var viewModelFactory = new SampleViewModelFactory();

            var domainModel1 = new SampleDomainModel();
            setter.AssignSampleValues(domainModel1, 0);
            var viewModel1 = viewModelFactory.Build(domainModel1);

            var domainModel2 = new SampleDomainModel();
            setter.AssignSampleValues(domainModel2, 1);
            var viewModel2 = viewModelFactory.Build(domainModel2);

            comparer.AssertDifference(viewModel1, viewModel2, new []
            {
                "/StaticValue - static value"
            });
        }
    }
}
