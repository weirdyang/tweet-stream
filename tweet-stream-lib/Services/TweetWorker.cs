using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi.Events.V2;
using TweetStream.Core.Abstractions;
using System.Linq;
namespace TweetStream.Core.Services
{
    public class TweetWorker : BackgroundService, ITweetWorker
    {
        private readonly ILogger<TweetWorker> _logger;
        private ITweetQueue<FilteredStreamTweetV2EventArgs> _tweetQueue;
        private IEnumerable<ITweetHandler> _handlers;

        public TweetWorker(ILogger<TweetWorker> logger, ITweetQueue<FilteredStreamTweetV2EventArgs> tweetQueue, IEnumerable<ITweetHandler> handlers)
        {
            _handlers = handlers;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tweetQueue = tweetQueue ?? throw new ArgumentNullException(nameof(tweetQueue));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
    "Tweet Worker Hosted Service running.");

            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service is working.");

            while (!stoppingToken.IsCancellationRequested)
            {
       
                if(_tweetQueue.GetCount() != 0)
                {
                    var task = await _tweetQueue.DequeueAsync(stoppingToken);
                    foreach (var handler in _handlers)
                    {
                        if (handler.CanHandle(task))
                        {
                            await handler.HandleAsync(task);
                        }
                    }
                    
                }
                else
                {
                        continue;
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
