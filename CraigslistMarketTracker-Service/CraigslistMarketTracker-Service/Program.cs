using System;
using System.ServiceProcess;

namespace CraigslistMarketTracker_Service
{
    static class Program
    {
        static void Main()
        {
            var myService = new Service1();
            if (Environment.UserInteractive)
            {
                Console.WriteLine("Starting service...");
                myService.Start();
                Console.WriteLine("Service is running.");
                Console.WriteLine("Press any key to stop...");
                Console.ReadKey(true);
                Console.WriteLine("Stopping service...");
                myService.Stop();
                Console.WriteLine("Service stopped.");
            }
            else
            {
                var servicesToRun = new ServiceBase[] { myService };
                ServiceBase.Run(servicesToRun);
            }
        }
    }
}
