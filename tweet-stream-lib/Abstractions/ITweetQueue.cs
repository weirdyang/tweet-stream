using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi.Models.V2;

namespace TweetStream.Core.Abstractions
{
    public interface ITweetQueue<T>
    {

        int GetCount();
        ValueTask<T> DequeueAsync(
            CancellationToken cancellationToken);
        ValueTask QueueTweetAsync(T tweet);
    }
}
