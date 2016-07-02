using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Async
{
    class Program
    {
        static void Main(string[] args)
        {
            char[] delimiters = { ' ', ',', '.', ';', ':', '-', '_', '/', '\u000A' };
            const string headerText = "Mozilla/5.0 (compatible; MSIE 10.0;Windows NT 6.1; Trident / 6.0)";

            var task1 = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Starting first task.");
                var client = new WebClient();
                client.Headers.Add("user-agent", headerText);
                var words = client.DownloadString(@"http://www.gutenberg.org/
files/2009/2009.txt");
                var wordArray = words.Split(delimiters, StringSplitOptions.
                RemoveEmptyEntries);
                Console.WriteLine("Origin of Species word count: {0}",
                wordArray.Count());
            });

            task1.Wait();
            Console.WriteLine("Task 1 complete. Creating Task 2 and Task 3.");

            var task2 = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Starting second task.");
                var client = new WebClient();
                client.Headers.Add("user-agent", headerText);
                var words = client.DownloadString(@"http://www.gutenberg.org/files/16328/16328-8.txt");
                var wordArray = words.Split(delimiters, StringSplitOptions.
                RemoveEmptyEntries);

                Console.WriteLine("Beowulf word count: {0}", wordArray.
                Count());
            });

            var task3 = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Starting third task.");
                var client = new WebClient();
                client.Headers.Add("user-agent", headerText);
                var words = client.DownloadString(@"http://www.gutenberg.org/files/4300/4300.txt");
                var wordArray = words.Split(delimiters, StringSplitOptions.
                RemoveEmptyEntries);
                Console.WriteLine("Ulysses word count: {0}", wordArray.
                Count());
            });

            Task.WaitAll(task2, task3);
            Console.WriteLine("All tasks complete.");
            Console.WriteLine("Press <Enter> to exit.");
            Console.ReadLine();

        }
    }
}
