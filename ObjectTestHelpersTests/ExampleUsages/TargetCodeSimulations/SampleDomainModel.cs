using System;

namespace ObjectTestHelpersTests.ExampleUsages.TargetCodeSimulations
{
    internal class SampleDomainModel
    {
        public int IntValue { get; set; }
        public string StringValue { get; set; }
        public DateTime DateTimeValue { get; set; }
        public Guid GuidValue { get; set; }
        public bool BoolValue { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}