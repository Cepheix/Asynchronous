using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PassingData
{
    class Program
    {
        static void Main(string[] args)
        {
            char[] delimiters = { ' ', ',', '.', ';', ':', '-', '_', '/', '\u000A' };
            const string headerText = "Mozilla/5.0 (compatible; MSIE 10.0;Windows NT 6.1; Trident / 6.0)";

            var dictionary = new Dictionary<string, string>
            {
                {"Origin of Species", "http://www.gutenberg.org/files/2009/2009.txt"},
                {"Beowulf", "http://www.gutenberg.org/files/16328/16328-8.txt"},
                {"Ulysses", "http://www.gutenberg.org/files/4300/4300.txt"}
            };

            var tasks = new List<Task>();

            foreach (var pair in dictionary)
            {
                tasks.Add(Task.Factory.StartNew((stateObj) =>
                {
                    var taskData = (KeyValuePair<string, string>)stateObj;
                    var client = new WebClient();
                    client.Headers.Add("user-agent", headerText);
                    var words = client.DownloadString(taskData.Value);
                    var wordArray = words.Split(delimiters, StringSplitOptions.
                    RemoveEmptyEntries);
                    Console.WriteLine("Word count for {0}: {1}", taskData.Key,
                    wordArray.Count());
                }, pair));
            }

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("Press <Enter> to exit.");
            Console.ReadLine();
        }
    }
}
