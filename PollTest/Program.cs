// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="Program.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace PollTest
{
    #region

    using System;

    using G2O.Launcher.ServerRequests;

    #endregion

    internal class Program
    {
        static void Main(string[] args)
        {
            // Test for the ServerRequests library.
            ServerWatcher manager = new ServerWatcher(28970, 100, 1000, 2000);
            manager.ServerStatusChanged += ManagerServerStatusChanged;
            manager.AddServer("192.168.2.199");
            manager.Start();
            Console.ReadKey();
        }

        private static void ManagerServerStatusChanged(object sender, ServerStatusChangedEventArgs e)
        {
            Console.Write(e.ChangedStatus.Info?.ToString() ?? "null");
            Console.Write("   Ping:");
            Console.WriteLine(e.ChangedStatus.LastPing + " " + e.ChangedStatus.PingSuccessfull);
        }
    }
}