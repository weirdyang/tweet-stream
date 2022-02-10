using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Events.V2;

namespace TweetStream.Abstractions
{
    public interface ITweetHandler
    {
        bool CanHandle(FilteredStreamTweetV2EventArgs item);

        Task HandleAsync(FilteredStreamTweetV2EventArgs item);


    }
}
