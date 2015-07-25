using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ObjectTestHelpers;
using ObjectTestHelpersTests.ExampleUsages.TargetCodeSimulations;

namespace ObjectTestHelpersTests.ExampleUsages
{
    [TestFixture]
    class BidirectionalMappingTest
    {
        /// <summary>
        /// repository pattern implements bi-directional mapping between the domain object and database layer.
        /// this test will detect missing mappings when you add new property to your domain object but not update the repository.
        /// try commenting out a line in SampleRepository (either Save() or GetById()) and running this test.
        /// 
        /// CreatedAt needs to be ignored in the comparison, since it is supposed to be different.
        /// </summary>
        [Test]
        public void Restored_object_must_match_with_original_object()
        {
            var repository = new SampleRepository();

            var domainModel1 = BuildSampleDomainModel();
            var id = repository.Save(domainModel1);
            var domainModel2 = repository.GetById(id);

            new ObjectComparer().AssertEquality(domainModel1, domainModel2, new[]
            {
                "/CreatedAt - "     // CreatedAt is supposed to be different.
            });
        }

        private SampleDomainModel BuildSampleDomainModel()
        {
            var model = new SampleDomainModel();
            var setter = new SampleValueSetter();
            setter.AssignSampleValues(model, 0);
            return model;
        }
    }
}
