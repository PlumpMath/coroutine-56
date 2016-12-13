using System;
using NUnit.Framework;

namespace tests
{

    public class CoroutineTestStub
    {
    }

    [TestFixture]
    public class CoroutineTest
    {
        TestCoroutineWithoutUnity tc;
        void Init()
        {
            tc = new TestCoroutineWithoutUnity();
        }

        [Test]
        public void testCoroutine()
        {
            Init();
            tc.Start();
            Console.Write("Log Report");
            Console.Write("----------");
            Console.Write(tc.report);
        }

       

    }
}
