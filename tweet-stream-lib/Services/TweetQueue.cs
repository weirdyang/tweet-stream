using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Tweetinvi.Models.V2;
using TweetStream.Core.Abstractions;

namespace TweetStream.Core.Services
{
    public class TweetQueue: ITweetQueue<TweetV2>
    {
        private readonly Channel<TweetV2> _queue;

        public int GetCount()
        {
            return _queue.Reader.CanCount ? _queue.Reader.Count : 0;
        }
        public TweetQueue(int capacity = 100)
        {
            // Capacity should be set based on the expected application load and
            // number of concurrent threads accessing the queue.            
            // BoundedChannelFullMode.Wait will cause calls to WriteAsync() to return a task,
            // which completes only when space became available. This leads to backpressure,
            // in case too many publishers/calls start accumulating.
            var options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<TweetV2>(options);
        }

        public async ValueTask QueueTweetAsync(TweetV2 tweet)
        {
            if (tweet == null)
            {
                throw new ArgumentNullException(nameof(tweet));
            }

            await _queue.Writer.WriteAsync(tweet);
        }

        public async ValueTask<TweetV2> DequeueAsync(
            CancellationToken cancellationToken)
        {
            var workItem = await _queue.Reader.ReadAsync(cancellationToken);

            return workItem;
        }
    }
}
