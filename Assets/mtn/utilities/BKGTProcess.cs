using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*
Example of a Background Process that uses threadpool
To use: create your own Background Process Class or override this one
Then use the calling sequence in the 
MountainMVC.CSharp.Editor/Assets/Editor/Scripts/Test/EditorTestProxyAndThreading.cs Unit test class.
*/
namespace mtn
{
    using System;
    using System.Threading;

    public class BKGTProcess
    {

        private ManualResetEvent _doneEvent;
        private int threadIndex;
        private string threadIndexId;
        public BKGTProcess() : base()
        {
            // empty constructor
        }
        public BKGTProcess(ManualResetEvent doneEvent)
        {
            _doneEvent = doneEvent;
        }

        public void BKGTProcessCallback(Object threadContext)
        {
            threadIndex = (int)threadContext;
            threadIndexId = "[" + threadIndex + "]->";
            Console.WriteLine("--> thread {0} Started ... ", threadIndex);
            StartProcess();
            Console.WriteLine("--> thread {0} Ended    ... ", threadIndex);

            // Indicates that the process had been completed
            _doneEvent.Set();
        }

        public void StartProcess()
        {
            // this executes on a background thread via the callback
            var prefix = " .. ";
            for (int j=0;j<10;j++)
            {
                Console.Write(threadIndexId+prefix+j);
               
            }

        }

    }

}
