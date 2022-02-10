using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Events.V2;
using TweetStream.Abstractions;
using System.Linq;
namespace TweetStream.Handlers
{
    public class FollowerQuotaHandler : ITweetHandler
    {
        ILogger<FollowerQuotaHandler> _logger;

        public FollowerQuotaHandler(ILogger<FollowerQuotaHandler> logger)
        {
            _logger = logger;
        }
        public bool CanHandle(FilteredStreamTweetV2EventArgs item)
        {

            var owner = item.Includes.Users?.FirstOrDefault(x => x.Id == item.Tweet.AuthorId);

            var createdAt = item.Tweet.CreatedAt > DateTimeOffset.UtcNow.AddDays(-2);
     
            return owner?.PublicMetrics.FollowersCount > 100 && createdAt;
        }

        public Task HandleAsync(FilteredStreamTweetV2EventArgs item)
        {
            _logger.LogCritical($"{item.Tweet.CreatedAt.ToString()}");
            _logger.LogInformation($"{item.Includes.Users?[0].PublicMetrics.FollowersCount} : {item.Tweet.AuthorId} : {item.Tweet.Id}");

            return Task.CompletedTask;
        }
    }
}
