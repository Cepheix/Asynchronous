using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TaskException
{
    class Program
    {
        static void Main(string[] args)
        {
            Task<int> task1 = Task.Factory.StartNew(() =>
            {
            const string headerText = "Mozilla/5.0 (compatible; MSIE 10.0;Windows NT 6.1; Trident / 6.0)";
Console.WriteLine("Starting the task.");
            var client = new WebClient();
            client.Headers.Add("user-agent", headerText);
            var words = client.DownloadString(@"http://www.gutenberg.org/
files/2009/2009.txt");
            var ex = new WebException("Unable to download book contents");
            throw ex;
            return 0;
            });

            try
            {
                task1.Wait();
                if (!task1.IsFaulted)
                {
                    Console.WriteLine("Task complete. Origin of Species wordcount: { 0}",task1.Result);
                }
            }
            catch (AggregateException ex)
            {
                foreach (var exception in ex.InnerExceptions)
                {
                    Console.WriteLine("Caught excepton: {0}", exception.Message);
                }
            }

            Console.WriteLine("Press <Enter> to exit.");
            Console.ReadLine();
        }
    }
}
