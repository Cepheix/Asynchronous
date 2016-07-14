using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProgressBar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        char[] delimiters = { ' ', ',', '.', ';', ':', '-', '_', '/', '\u000A' };
        const string headerText = "Mozilla/5.0 (compatible; MSIE 10.0;Windows NT 6.1; Trident/6.0)";
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            TextResult.Text += "Started downloading Origin of Species...\n";
            Task<int> countTask = GetWordCountAsync();
            int result = await countTask;
            TextResult.Text += String.Format("Finished downloading. Word count: {0}\n", result);
        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            TextResult.Text += " Download completed. \n";
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.
            ToString());
            double percentage = bytesIn / totalBytes * 100;
            DownloadProgress.Value = int.Parse(Math.Truncate(percentage).
            ToString());
        }

        public async Task<int> GetWordCountAsync()
        {
            TextResult.Text += " Getting the word count for Origin of Species...\n";

            var client = new WebClient();
            client.Headers.Add("user-agent", headerText);
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);

            Task<string> wordsTask = client.DownloadStringTaskAsync(new Uri("http://www.gutenberg.org/files/2009/2009.txt"));
            var words = await wordsTask;
            var wordArray = words.Split(delimiters, StringSplitOptions.
            RemoveEmptyEntries);
            return wordArray.Count();
        }
    }
}
