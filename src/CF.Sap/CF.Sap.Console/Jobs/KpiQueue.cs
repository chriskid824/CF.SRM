using Hangfire;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF.Sap.Console.Jobs
{
    internal class Job
    {
        public static void Send(string message)
        {
            //Thread.Sleep(10000);
            Trace.WriteLine($"Message:{message}, Now:{DateTime.Now}");
        }
    }
    public class KpiQueue
    {
        [Queue("kpi")]
        public void SomeMethod() {
            //立即執行一次
            BackgroundJob.Enqueue(() => Job.Send("測試"));
        }
    }
}
