using System.Threading.Channels;

namespace RWA.Web.Application.Services.BddMatch
{
    public record BddMatchJob(string Version);

    public interface IBddMatchJobQueue
    {
        void Enqueue(BddMatchJob job);
        ChannelReader<BddMatchJob> Reader { get; }
    }

    public class BddMatchJobQueue : IBddMatchJobQueue
    {
        private readonly Channel<BddMatchJob> _channel = Channel.CreateUnbounded<BddMatchJob>();
        public void Enqueue(BddMatchJob job) => _channel.Writer.TryWrite(job);
        public ChannelReader<BddMatchJob> Reader => _channel.Reader;
    }
}

