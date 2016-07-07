using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WhenAny
{
    class Program
    {
        static void Main(string[] args)
        {
            char[] delimiters = { ' ', ',', '.', ';', ':', '-', '_','/', '\u000A' };
            const string headerText = "Mozilla/5.0 (compatible; MSIE10.0; Windows NT 6.1; Trident / 6.0)";
            Dictionary<string, string> dictionary = new Dictionary<string, string>
            {
                {"Origin of Species","http://www.gutenberg.org/files/2009/2009.txt"},
                {"Beowulf","http://www.gutenberg.org/files/16328/16328-8.txt"},
                {"Ulysses","http://www.gutenberg.org/files/4300/4300.txt"}
            };

            List<Task<KeyValuePair<string, int>>> tasks = new List<Task<KeyValuePair<string, int>>>();

            try
            {
                foreach (KeyValuePair<string, string> book in dictionary)
                {
                    Task<KeyValuePair<string, int>> task = Task.Factory.StartNew(obj => 
                    {
                        KeyValuePair<string, string> taskData = (KeyValuePair<string, string>) obj;
                        Console.WriteLine("Starting task for {0}", taskData.Key);
                        WebClient client = new WebClient();
                        client.Headers.Add("user-agent", headerText);
                        string words = client.DownloadString(taskData.Value);
                        string[] wordArray = words.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                        return new KeyValuePair<string, int>(taskData.Key, wordArray.Count());
                    }, book);

                    tasks.Add(task);
                }

                Task.Factory.ContinueWhenAny(tasks.ToArray(), (task) => 
                {
                    Console.WriteLine("And the winner is: {0}", task.Result.Key);
                    Console.WriteLine("Word count: {0}", task.Result.Value);
                }).Wait();

                Task.Factory.ContinueWhenAll(tasks.ToArray(), (allTasks) =>
                {
                    Console.WriteLine("All tasks have been downloaded");
                    foreach (var task in allTasks)
                    {
                        Console.WriteLine("Book Title: {0}", task.Result.
                        Key);
                        Console.WriteLine("Word count: {0}", task.Result.
                        Value);
                    }
                }).Wait();
            }
            catch (AggregateException ex)
            {
                foreach (Exception exception in ex.InnerExceptions)
                {
                    Console.WriteLine("An exception has occured: {0}" + exception.Message);
                }
            }

            Console.WriteLine("Complete. Press <Enter> to exit.");
            Console.ReadLine();
        }
    }
}
