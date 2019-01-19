using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedPing
{
    public class PingEventArgs : EventArgs
    {
        public PingReply PingReply { get; }

        public PingResults PingResults { get; }

        public PingEventArgs(PingReply reply, PingResults results)
        {
            PingReply = reply;
            PingResults = results;
        }
    }
}
