using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CancellingTask
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create a cancellation token source
            CancellationTokenSource tokenSource = new
            CancellationTokenSource();
            //get the cancellation token
            CancellationToken token = tokenSource.Token;

            tokenSource.Token.Register(() =>
            {
                Console.WriteLine("The operation has been cancelled.");
            });

            Task<int> task1 = Task.Factory.StartNew(() =>
            {
                //wait a bit for the cancellation
                Thread.Sleep(2000);
                const string headerText = "Mozilla/5.0 (compatible; MSIE 10.0;Windows NT 6.1; Trident / 6.0)";
                var client = new WebClient();
                client.Headers.Add("user-agent", headerText);

                if (token.IsCancellationRequested)
                {
                    client.Dispose();
                    throw new OperationCanceledException(token);
                }
                else
                {
                    var book = client.DownloadString(@"http://www.gutenberg.org/files/2009/2009.txt");
                    char[] delimiters = { ' ', ',', '.', ';', ':', '-', '_', '/', '\u000A' };
                    Console.WriteLine("Starting the task.");
                    var wordArray = book.Split(delimiters, StringSplitOptions.
                    RemoveEmptyEntries);
                    return wordArray.Count();
                }
            }, token);

            Console.WriteLine("Has the task been cancelled?: {0}", task1.IsCanceled);
            //Cancel the token source
            tokenSource.Cancel();

            if (!task1.IsCanceled || !task1.IsFaulted)
            {
                try
                {
                    if (!task1.IsFaulted)
                    {
                        Console.WriteLine("Origin of Specied word count: {0}",
                        task1.Result);
                    }
                }
                catch (AggregateException aggEx)
                {
                    foreach (Exception ex in aggEx.InnerExceptions)
                    {
                        Console.WriteLine("Caught exception: {0}", ex.Message);
                    }
                }
            }
            else
            {
                Console.WriteLine("The task has been cancelled");
            }

            Console.WriteLine("Press <Enter> to exit.");
            Console.ReadLine();
        }
    }
}
