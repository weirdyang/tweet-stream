using System;
using System.Collections.Generic;
using System.Text;

namespace TweetStream.Configuration
{
    public class AppCredentials
    {   // api key a9IL6cV20YPrZXSVD4o8VCMH0
        // api secret z9TwmGmop0EJuk4KezHD5WyENLENrnCe93mYqO6RY0VbUeQ6QB
        // bearer token AAAAAAAAAAAAAAAAAAAAAPn3XwEAAAAAQNGxCf6sZS8ONnihanboKZR7Bg8%3DiNXWz8s3LoKYFF2UvetBpPYd1zxrUqB0GYugaiKAt828prEr2j
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
