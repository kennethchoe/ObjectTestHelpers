using System.Collections.Generic;

namespace ObjectTestHelpersTests
{
    class TestClass
    {
        public string Value1 { get; set; }
        public string Value2 { get; set; }

        public string Field1;

        public Dictionary<string, string> Dict { get; set; }

        public TestClass Composite { get; set; }

        public TestClass()
        {
            Dict = new Dictionary<string, string>();
        }
    }
}