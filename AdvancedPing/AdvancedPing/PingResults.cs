using System;
using System.Net.NetworkInformation;

namespace AdvancedPing
{
    public class PingResults
    {
        public long Count { get; private set; }
        public double Average { get; private set; }
        public long Max { get; private set; }
        public long Min { get; private set; }
        public long PacketsLost { get; private set; }
        public double PacketsLostPercent => (PacketsLost / Count) * 100;
        public long SuccessfulPackets => Count - PacketsLost;
        public long MaxJitter { get; private set; }
        public double AverageJitter { get; private set; }
        public PingResultStatus Status { get; private set; }
        public string StatusMessage { get; private set; }
        private PingReply _lastSuccessfulPing;

        public PingResults(PingReply reply)
        {
            Count = 1;
            Status = PingResultStatus.InProgress;
            StatusMessage = "In Progress";
            if (reply.Status == IPStatus.Success)
            {
                _lastSuccessfulPing = reply;
                Average = _lastSuccessfulPing.RoundtripTime;
                Max = _lastSuccessfulPing.RoundtripTime;
                Min = _lastSuccessfulPing.RoundtripTime;
            }
            else
            {
                Min = long.MaxValue;
                PacketsLost = 1;
            }
        }

        private PingResults(string errorMessage)
        {
            StatusMessage = errorMessage;
            Status = PingResultStatus.Failed;
            Count = 1;
            PacketsLost = 1;
            Min = long.MaxValue;
        }

        public static PingResults BuildFailedResults(string error)
        {
            return new PingResults(error);
        }

        public void AddResponse(PingReply reply)
        {
            if (Status == PingResultStatus.Failed)
            {
                throw new InvalidOperationException("Unable to add reply to already failed results.");
            }
            if (reply.Status == IPStatus.Success)
            {
                if (_lastSuccessfulPing == null)
                {
                    _lastSuccessfulPing = reply;
                }
                Max = reply.RoundtripTime > Max ? reply.RoundtripTime : Max;
                Min = reply.RoundtripTime < Min ? reply.RoundtripTime : Min;
                var jitter = Math.Abs(_lastSuccessfulPing.RoundtripTime - reply.RoundtripTime);
                MaxJitter = jitter > MaxJitter ? jitter : MaxJitter;

                if (SuccessfulPackets > 0)
                {
                    AverageJitter = (AverageJitter * (SuccessfulPackets - 1) + jitter) / SuccessfulPackets;
                }
                
                Average = (Average * SuccessfulPackets + reply.RoundtripTime) / (SuccessfulPackets + 1);
            }
            else
            {
                PacketsLost++;
            }

            Count++;
        }

        public void MarkComplete()
        {
            Status = PingResultStatus.Completed;
            StatusMessage = "Completed Successfully";
        }

        public void MarkFailed(string errorMessage)
        {
            StatusMessage = errorMessage;
            Status = PingResultStatus.Failed;
        }
    }

    public enum PingResultStatus
    {
        InProgress,
        Completed,
        Failed
    }
}
