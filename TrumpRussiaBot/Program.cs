using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TweetSharp;

namespace TrumpRussiaBot
{
    class Program
    {
        public static long? statusId = 0;
        public static int tweets = 0;
        static void Main(string[] args)
        {
            var timer = new Timer()
            {
                AutoReset = true,
                Enabled = true,
                Interval = 3600
            };
            timer.Elapsed += Tweet;
            while (true)
            {

            }
        }

        private static async void Tweet(object sender, ElapsedEventArgs e)
        {
            Console.Clear();
            var service = GenerateTwitterService();
            var trumpTweet = await GetTrumpTweet(service);
            var russianTweet = Russianize(trumpTweet);
            Tweet(russianTweet, service);
        }

        static async void Tweet(string tweet, TwitterService service)
        {
            if (tweet != null)
            {
                Console.WriteLine("Sending Tweet...");
                var post = await service.SendTweetAsync(new SendTweetOptions() { Status = tweet, InReplyToStatusId = statusId, AutoPopulateReplyMetadata = true  });
                if (post != null)
                {
                    tweets++;
                    Console.WriteLine("Tweet Successful");
                }
            }
            Console.WriteLine($"Process Complete. Posted {tweets} tweets over the course of this runtime.");
        }

        static async Task<string> GetTrumpTweet(TwitterService service)
        {
            Console.WriteLine("Getting Tweet...");
            var listRaw = await service.ListTweetsOnUserTimelineAsync(new ListTweetsOnUserTimelineOptions() { ScreenName = "realDonaldTrump", Count = 100 });
            var list = listRaw.Value.ToList();
            var tweet = list.FirstOrDefault(x => x.RetweetedStatus == null);
            if (statusId == tweet?.Id)
            {
                Console.WriteLine("Tweet Already Fetched.");
                return null;
            }
            else
            {
                statusId = tweet?.Id;
                return tweet?.FullText ?? tweet?.Text;
            }

        }

        static TwitterService GenerateTwitterService()
        {
            Console.WriteLine("Initializing Service...");
            // insert credentials as parameters in method below
            return new TwitterService();
        }

        static string Russianize(string tweet)
        {
            if (tweet == null)
            {
                return null;
            }
            Console.WriteLine("Converting Tweet...");
            var russian = new StringBuilder();
            foreach (var c in tweet)
            {
                var chars = new Dictionary<char, char>
                {
                    { 'b', 'б' },{ 'B', 'Б' },{ 'v', 'ц' },
                    { 'V', 'Ц' },{ 'g', 'г' },{ 'G', 'Г' },
                    { 'a', 'д' },{ 'A', 'Д' },{ 'x', 'ж' },
                    { 'X', 'Ж' },{ 'k', 'к' },{ 'l', 'л' },
                    { 'L', 'Л' },{ 'n', 'и' },{ 'N', 'И' },
                    { 'm', 'м' },{ 'h', 'н' },{ 'p', 'п' },
                    { 'P', 'П' },{ 't', 'т' },{ 'o', 'ф' },
                    { 'O', 'Ф' },{ 'w', 'щ' },{ 'W', 'Щ' },
                    { 'e', 'э' },{ 'E', 'Э' },{ 'r', 'я' },
                    { 'R', 'Я' },{ 'y', 'ч' },{ 'Y', 'Ч' },
                    { 'd', 'ю' },{ 'D', 'Ю' }
                };
                chars.TryGetValue(c, out var value);
                if (value != 0)
                {
                    russian.Append(value);
                }
                else
                {
                    russian.Append(c);
                }
            };
            return russian.ToString();
        }
    }
}
