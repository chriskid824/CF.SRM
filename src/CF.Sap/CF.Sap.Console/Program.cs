using Microsoft.Owin.Hosting;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF.Sap.ConsoleTask
{
    class Program
    {
        private static IDisposable s_webApp;
        private const string HOST_ADDRESS = "http://localhost:8001";
        static void Main(string[] args)
        {
            s_webApp = WebApp.Start<Startup>(HOST_ADDRESS);
            System.Console.ReadLine();
        }
    }
}
