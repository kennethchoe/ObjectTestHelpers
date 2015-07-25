namespace ObjectTestHelpersTests.ExampleUsages.TargetCodeSimulations
{
    internal class SampleViewModelFactory
    {
        public SampleViewModel Build(SampleDomainModel domainModel)
        {
            var result = new SampleViewModel();
            result.IntValueString = domainModel.IntValue.ToString();
            result.StringValue = domainModel.StringValue;
            result.DateTimeValueString = domainModel.DateTimeValue.ToString();
            result.GuidValueString = domainModel.GuidValue.ToString();
            result.BoolValueString = domainModel.BoolValue.ToString();

            result.StaticValue = "static value";

            return result;
        }
    }
}