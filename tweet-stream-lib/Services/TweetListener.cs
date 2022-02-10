using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Events;
using Tweetinvi.Events.V2;
using Tweetinvi.Models.V2;
using Tweetinvi.Parameters.V2;
using Tweetinvi.Streaming.V2;
using Tweetinvi.Streams;
using TweetStream.Abstractions;
using TweetStream.Configuration;

namespace TweetStream.Services
{
    public class TweetListener : BackgroundService
    {
        private readonly ITweetQueue<TweetV2> _taskQueue;
        private readonly ILogger<TweetListener> _logger;
        private readonly TweetConfiguration _tweetConfig;
        private TwitterClient Client { get => _tweetConfig.Client; }

        internal IFilteredStreamV2 _stream { get; set; }
        public bool HasStarted { get; set; } = false;

        public TweetListener(
            ITweetQueue<TweetV2> taskQueue,
    ILogger<TweetListener> logger,
    TweetConfiguration tweetConfigurateion)
        {
            _taskQueue = taskQueue;
            _logger = logger;
            _tweetConfig = tweetConfigurateion;
        }


        private async Task SetupStream()
        {
            await _tweetConfig.CreateRules();
            _stream = Client.StreamsV2.CreateFilteredStream();

            _stream.TweetReceived += HandleTweet;
            _stream.EventReceived += HandleEvent;
        }

        private async void HandleTweet(object sender, FilteredStreamTweetV2EventArgs e)
        {

                await _taskQueue.QueueTweetAsync(e.Tweet);
         
        }

        private void HandleEvent(object sender, StreamEventReceivedArgs e)
        {
            _logger.LogInformation(e.Json);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
           
            await MonitorAsync(stoppingToken);
        }

        private async Task MonitorAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {

                try
                {
                    HasStarted = true;
                    await _stream.StartAsync();
                }
                catch (Exception ex)
                {
                    HasStarted = false;
                    _logger.LogCritical(ex, ex.InnerException?.Message);
                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
            }
        }
        public override async Task StartAsync(CancellationToken stoppingToken)
        {
            await SetupStream();
            await base.StartAsync(stoppingToken);
        }
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Listener Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
