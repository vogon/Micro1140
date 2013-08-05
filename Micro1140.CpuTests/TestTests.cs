using System;
using Microsoft.SPOT;

namespace Micro1140.CpuTests
{
    public class TestTests : ITestSuite
    {
        public TestTests() { }

        public void Test_Octal_Null()
        {
            Assert.ExpectException(typeof(ArgumentNullException));
            ((string)null).AsOctal();
        }

        public void Test_Octal_Overflow()
        {
            Assert.ExpectException(typeof(ArgumentOutOfRangeException));
            "111111111111111".AsOctal();
        }

        public void Test_Octal_BadDigits()
        {
            Assert.ExpectException(typeof(ArgumentException));
            "88888".AsOctal();
        }

        public void Test_Octal_Good()
        {
            Assert.AreEqual("123456".AsOctal(), 42798);
        }
    }
}
