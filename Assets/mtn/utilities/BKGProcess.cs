using System;
using System.ComponentModel;
/*
Example of a Background Process that uses background worker thread
To use: create your own Background Process Class or override this one
Then use the calling sequence in the 
MountainMVC.CSharp.Editor/Assets/Editor/Scripts/Test/EditorTestProxyAndThreading.cs Unit test class.
*/
namespace mtn
{
    public class BKGProcess
    {
        public BackgroundWorker bw;

        public BKGProcess() : base()
        {
            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);
        }
        public void run()
        {
            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
            }
        }
        public void cancel()
        {
            if (bw.WorkerSupportsCancellation == true)
            {
                bw.CancelAsync();
            }
        }
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            for (int i = 1; (i <= 10); i++)
            {
                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    // Perform a time consuming operation and report progress.
                    System.Threading.Thread.Sleep(100);
                    bw.ReportProgress((i * 10));
                }
            }
        }
        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
               Console.WriteLine("Canceled!");
            }

            else if (!(e.Error == null))
            {
               Console.WriteLine("Error: " + e.Error.Message);
            }

            else
            {
                Console.WriteLine("Done!");
            }
        }
        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine("Progress Percentage = "+e.ProgressPercentage + "%");
        }
    }
}