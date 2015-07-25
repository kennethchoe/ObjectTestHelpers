using System;
using System.Collections.Generic;

namespace ObjectTestHelpersTests.ExampleUsages.TargetCodeSimulations
{
    internal class SampleRepository
    {
        private Dictionary<string, object> MockDatabase { get; set; }

        public SampleRepository()
        {
            MockDatabase = new Dictionary<string, object>();
        }

        public int Save(SampleDomainModel domainModel)
        {
            MockDatabase["StringValue"] = domainModel.StringValue;
            MockDatabase["BoolValue"] = domainModel.BoolValue;
            MockDatabase["DateTimeValue"] = domainModel.DateTimeValue;
            MockDatabase["GuidValue"] = domainModel.GuidValue;
            MockDatabase["IntValue"] = domainModel.IntValue;
            MockDatabase["CreatedAt"] = DateTime.UtcNow;

            return 1;
        }

        public SampleDomainModel GetById(int id)
        {
            var domainModel = new SampleDomainModel();
            domainModel.StringValue = (string) MockDatabase["StringValue"];
            domainModel.BoolValue = (bool) MockDatabase["BoolValue"];
            domainModel.DateTimeValue = (DateTime) MockDatabase["DateTimeValue"];
            domainModel.GuidValue = (Guid) MockDatabase["GuidValue"];
            domainModel.IntValue = (int) MockDatabase["IntValue"];
            domainModel.CreatedAt = (DateTime)MockDatabase["CreatedAt"];

            return domainModel;
        }
    }
}