using System;
using System.ComponentModel;
using System.Threading;

namespace mtn
{
    using NUnit.Framework;

    public class ProxyAndThreadingTestStub
    {
        public static void Main()
        {
        }
    }

    [TestFixture]
    public class TestProxyAndThreading
    {
        [SetUp]
        public void Init()
        {
        }
        [Test]
        public void TestProxy()
        {
            ITestObject intf = ProgramProxy<ITestObject>.Create(new TestObject());
            string retstr = intf.TestMethod("Ronald");
            Console.WriteLine(retstr);
        }
        [Test]
        public void TestRunAsnyc()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                // call the function
                ITestObject test = new TestObject();
                e.Result = test.TestMethod("Hi Smitty");
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                // use the result of the XYZ function:
                string  result = (string) e.Result;
                Console.WriteLine("Worker Completed with result="+result);
                // Here you can safely manipulate the GUI controls
            };

            Console.WriteLine("----> Invoke worker.RunWorkerAsync()");
            worker.RunWorkerAsync();
            Console.WriteLine("----> Sleeeping for 1000 millsecs");
            Thread.Sleep(1000);
            Console.WriteLine("----> End of Sleeeping for 1000 millsecs");
        }

        [Test]
        public void TestRunAsnycWithProgress()
        {
            BKGProcess bg = new BKGProcess();
            Console.WriteLine("RUN #1");
            bg.run();
            Console.WriteLine("----> Starting to sleep#1 for 500 milliseconds now.");
            Thread.Sleep(500);
            Console.WriteLine("----> Awake from Sleep#1 of 500 milliseconds and cancelling background task.");
            bg.cancel();

            Console.WriteLine("----> Starting to sleep#2 for 500 milliseconds now.");
            Thread.Sleep(500);

            Console.WriteLine("----> Awake from Sleep#2 of 500 milliseconds and starting run #2.");
            Console.WriteLine("RUN #2");
            bg.run();

            Console.WriteLine("----> Starting to sleep#2 for 2000 milliseconds now.");
            Thread.Sleep(2000);
        }

        [Test]
        public void TestThreadPoolProcess()
        {
            const int totalCountToProcess = 5;

            ManualResetEvent[] doneEvents = new ManualResetEvent[totalCountToProcess];
            BKGTProcess[] bkg = new BKGTProcess[totalCountToProcess];

            // Configure and launch threads using ThreadPool:
            Console.WriteLine("launching tasks...");
            for (int i = 0; i < totalCountToProcess; i++)
            {
                doneEvents[i] = new ManualResetEvent(false);
                BKGTProcess p = new BKGTProcess(doneEvents[i]);
                bkg[i] = p;
                ThreadPool.QueueUserWorkItem(p.BKGTProcessCallback, i);
            }
            ThreadPool.SetMaxThreads(5, 5);
            // If we want to wait for all threads in the pool to finish
            // uncomment the following line

            foreach (var e in doneEvents) e.WaitOne();
            Console.WriteLine("All Process are complete.");
            // Thread.Sleep(1000);
             Console.WriteLine("Only some processes may be completed - because without the e.WaitOne() loop, this thread will exit as soon as all items are queued !");
        }
    }
}
