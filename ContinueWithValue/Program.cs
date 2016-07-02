using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ContinueWithValue
{
    class Program
    {
        static void Main(string[] args)
        {
            char[] delimiters = { ' ', ',', '.', ';', ':', '-','_', '/', '\u000A' };

            try
            {
                Task<string[]> task1 = Task.Factory.StartNew(() =>
                {
                    var client = new WebClient();
                    const string headerText = "Mozilla/5.0 (compatible;MSIE 10.0; Windows NT 6.1; Trident / 6.0)";
                    client.Headers.Add("user-agent", headerText);
                    var words =
                    client.DownloadString(@"http://www.gutenberg.org/files/2009/2009.txt");
                    string[] wordArray = words.Split(delimiters,
                    StringSplitOptions.RemoveEmptyEntries);
                    Console.WriteLine("Word count for Origin of Species:{ 0}", wordArray.Count());
                    Console.WriteLine();
                    return wordArray;
                });

                task1.ContinueWith((antecedent) => 
                {
                    var wordsByUsage = antecedent.Result.Where(word => word.Length > 5)
                        .GroupBy(word => word)
                        .OrderByDescending(grouping => grouping.Count())
                        .Select(grouping => grouping.Key);
                    var commonWords = (wordsByUsage.Take(5)).ToArray();
                    Console.WriteLine("The 5 most commonly used words inOrigin of Species: ");
                    Console.WriteLine("----------------------------------------------------");
                    foreach (var word in commonWords)
                    {
                        Console.WriteLine(word);
                    }
                }).Wait();

                Console.WriteLine();
                Console.WriteLine("Complete. Please hit <Enter> to exit.");
                Console.ReadLine();
            }
            catch (AggregateException ex)
            {
                foreach (var innerException in ex.InnerExceptions)
                {
                    Console.WriteLine("An exception has occured: {0}" + innerException.Message);
                }
            }
        }
    }
}
