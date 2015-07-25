using System;

namespace ObjectTestHelpers
{
    public class AssertDifferenceException : Exception
    {
        public AssertDifferenceException(string message): base(message)
        {
        }
    }
}