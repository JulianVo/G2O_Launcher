using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using G2O.Launcher.ServerRequests;

namespace PollTest
{
    class Program
    {
        static void Main(string[] args)
        {
            PollManager manager = new PollManager(28970,1000);
            manager.OnServerStatusChanged += Manager_OnServerStatusChanged;
            manager.AddServer("192.168.2.199");
            manager.Start();
            Console.ReadKey();
        }

        private static void Manager_OnServerStatusChanged(object sender, ServerStatusChangedEventArgs e)
        {
           Console.WriteLine(e.ChangedStatus.Info);
            Console.WriteLine(e.ChangedStatus.LastPing);
        }
    }
}
