using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Models.V2;
using Tweetinvi.Parameters.V2;

namespace TweetStream.Core.Configuration
{
    public class Settings
    {
        public bool Reset { get; set; }

        public int Delay { get; set; }
    }
    public class Rule
    {
        public string Tag { get; set; }

        public string Value { get; set; }
    }
    public class TweetConfiguration
    {
        private AppCredentials AppCredentials;

        private ConsumerOnlyCredentials ConsumerCredentials;

        private ILogger<TweetConfiguration> _logger;
        private Settings _settings;
        public TwitterClient Client { get; }

        private List<Rule> _rules;
        public TweetConfiguration(
            IOptions<AppCredentials> credentials,
            IOptions<List<Rule>> rules,
            IOptions<Settings> settings,
            ILogger<TweetConfiguration> logger)
        {
            _logger = logger;
            AppCredentials = credentials.Value;
            ConsumerCredentials = new ConsumerOnlyCredentials(
                AppCredentials.ConsumerKey,
                AppCredentials.ConsumerSecret,
                AppCredentials.BearerToken);

            Client = new TwitterClient(ConsumerCredentials);

            _settings = settings.Value;
            _rules = rules.Value;
            Configure();

        }

        private void Configure()
        {
            
            Client.Config.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;
        }

        private async Task<FilteredStreamRulesV2Response> ResetRules(FilteredStreamRuleV2[] current)
        {
            var toDelete = current.Select(x => x.Id);
            return await Client.StreamsV2.DeleteRulesFromFilteredStreamAsync(toDelete.ToArray());
        }
        public async Task CreateRules()
        {
            var rulesToAdd = new List<FilteredStreamRuleConfig>();
            // get rules
            var current = await Client.StreamsV2.GetRulesForFilteredStreamV2Async();
            if (_settings.Reset && current.Rules.Length != 0)
            {
                var reset = await ResetRules(current.Rules);
                if (reset.Errors?.Length > 0)
                {
                    _logger.LogError("Error deleting rules", reset.Errors);
                }
            }
            if (_settings.Reset)
            {
                await AddRules(rulesToAdd);
            }
            else
            {
                await CheckAndAddRules(rulesToAdd, current);
            }

            // check rules

        }
        private async Task AddRules(List<FilteredStreamRuleConfig> rulesToAdd)
        {
            foreach (var item in _rules)
            {

                var newRule = new FilteredStreamRuleConfig(tag: item.Tag, value: item.Value);

                rulesToAdd.Add(newRule);
            }
            FilteredStreamRulesV2Response test = await Client.StreamsV2.TestFilteredStreamRulesV2Async(rulesToAdd.ToArray());
            await Client.StreamsV2.AddRulesToFilteredStreamAsync(rulesToAdd.ToArray());
        }

        private async Task CheckAndAddRules(List<FilteredStreamRuleConfig> rulesToAdd, FilteredStreamRulesV2Response current)
        {
            var currentTags = current.Rules.Select(x => x.Tag);
            foreach (var item in _rules)
            {
                if (currentTags.Contains(item.Tag))
                {
                    continue;
                }
                var newRule = new FilteredStreamRuleConfig(tag: item.Tag, value: item.Value);

                rulesToAdd.Add(newRule);
            }
            if (rulesToAdd.Count != 0)
            {
                FilteredStreamRulesV2Response test = await Client.StreamsV2.TestFilteredStreamRulesV2Async(rulesToAdd.ToArray());
                await Client.StreamsV2.AddRulesToFilteredStreamAsync(rulesToAdd.ToArray());

            }
        }
    }
}
