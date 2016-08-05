using System;

namespace G2O.Launcher.ServerRequests
{
    public class ServerStatusChangedEventArgs : EventArgs
    {
        public ServerStatusChangedEventArgs(IServerPoll changedStatus)
        {
            ChangedStatus = changedStatus;
        }

        public IServerPoll ChangedStatus { get; }
    }
}