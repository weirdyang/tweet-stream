using System;
using System.Collections.Generic;
using System.Text;

namespace TweetStream.Core.Configuration
{
    public class AppCredentials
    {  
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string BearerToken { get; set; }


        public AppCredentials()
        {

        }

        public AppCredentials(string consumerKey, string consumerSecret, string bearerToken)
        {
            ConsumerKey = consumerKey ?? throw new ArgumentNullException(nameof(consumerKey));
            ConsumerSecret = consumerSecret ?? throw new ArgumentNullException(nameof(consumerSecret));
            BearerToken = bearerToken ?? throw new ArgumentNullException(nameof(bearerToken));
        }
    }
}
