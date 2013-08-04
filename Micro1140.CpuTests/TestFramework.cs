using System;

namespace Micro1140.CpuTests
{
    public class AssertionFailedException : Exception
    {
        public AssertionFailedException(string message) : base(message: message) { }
    }

    public interface ITestSuite {}

    public static partial class Assert
    {
        public static void IsTrue(bool expr, string message = "")
        {
            if (!expr)
            {
                throw new AssertionFailedException(message: message);
            }
        }

        public static void AreEqual(int a, int b, string message = "")
        {
            Assert.IsTrue(a == b, message: message);
        }
    }
}