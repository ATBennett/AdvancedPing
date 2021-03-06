﻿using System;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace AdvancedPing
{
    public class PingManager : IDisposable
    {
        public event EventHandler<PingEventArgs> PingReceived;

        private readonly int _count;
        private readonly int _delay;
        private readonly string _address;
        private readonly Ping _pinger;

        public PingManager(int count, int delay, string address)
        {
            _count = count;
            _delay = delay;
            _address = address;
            _pinger = new Ping();
        }

        protected virtual void OnPingReceived(PingReply reply, PingResults results)
        {
            var handler = PingReceived;

            handler?.Invoke(this, new PingEventArgs(reply, results));
        }

        public async Task<PingResults> StartPing(CancellationToken cancellationToken)
        {
            //Send first ping
            PingReply reply;
            try
            {
                reply = await _pinger.SendPingAsync(_address);
            }
            catch (PingException e)
            {
                return PingResults.BuildFailedResults(e.Message);
            }

            var currentResults = new PingResults(reply);

            OnPingReceived(reply, currentResults);
            
            for (var i = 1; i < _count; i++)
            {
                await Task.Delay(_delay, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    return currentResults;
                }
                try
                {
                    reply = await _pinger.SendPingAsync(_address);
                }
                catch (PingException e)
                {
                    currentResults.MarkFailed(e.Message);
                    return currentResults;
                }

                currentResults.AddResponse(reply);
                OnPingReceived(reply, currentResults);
            }

            return currentResults;
        }

        public void CancelPings()
        {
            _pinger.SendAsyncCancel();
        }

        public void Dispose()
        {
            _pinger.Dispose();
        }
    }
}
