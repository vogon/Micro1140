using System;
using System.Collections;
using System.Reflection;
using Microsoft.SPOT;

namespace Micro1140.CpuTests
{
    public class TestRunner
    {
        private readonly Assembly assemblyUnderTest = Assembly.GetExecutingAssembly();

        private bool IsTestSuite(Type t)
        {
            bool isSuite = false;

            foreach (Type i in t.GetInterfaces())
            {
                if (i == typeof(ITestSuite))
                {
                    isSuite = true;
                    break;
                }
            }

            return isSuite;
        }

        private Type[] GetAllTestSuiteTypes(Assembly asm)
        {
            ArrayList suites = new ArrayList();

            Type[] allTypes = asm.GetTypes();

            foreach (Type t in allTypes)
            {
                if (IsTestSuite(t)) suites.Add(t);
            }

            return (Type[])suites.ToArray(typeof(Type));
        }

        private MethodInfo[] GetAllTestMethods(Type t)
        {
            ArrayList methods = new ArrayList();

            MethodInfo[] allMethods = t.GetMethods();

            foreach (MethodInfo mi in allMethods)
            {
                // test methods have names that start with "Test_"
                //Debug.Print(mi.Name);
                if (mi.Name.IndexOf("Test_") == 0)
                {
                    methods.Add(mi);
                }
            }

            return (MethodInfo[])methods.ToArray(typeof(MethodInfo));
        }

        private object MakeSuite(Type t)
        {
            if (!IsTestSuite(t)) throw new ArgumentException();

            ConstructorInfo ci = t.GetConstructor(new Type[] { });
            object suite = ci.Invoke(new object[] { });

            return suite;
        }

        private void Run()
        {
            Type[] suites = GetAllTestSuiteTypes(assemblyUnderTest);
            int run = 0, passed = 0, failed = 0;

            Debug.Print(suites.Length.ToString() + " test suites found.");

            foreach (Type suite in suites)
            {
                Debug.Print("===== Test suite " + suite.Name + " =====");
                object suiteObj = MakeSuite(suite);

                MethodInfo[] tests = GetAllTestMethods(suite);

                foreach (MethodInfo test in tests)
                {
                    run += 1;

                    try
                    {
                        test.Invoke(suiteObj, new object[] { });
                        passed += 1;
                    }
                    catch (Exception e)
                    {
                        Debug.Print(test.Name + ": failed (" + e.Message + ")");
                        failed += 1;
                    }
                }
            }

            Debug.Print("===== all tests completed =====");
            Debug.Print(run.ToString() + " tests run, " + passed.ToString() + " tests passed, " + failed + " tests failed");
        }

        public static void Main()
        {
            (new TestRunner()).Run();
        }
    }
}
