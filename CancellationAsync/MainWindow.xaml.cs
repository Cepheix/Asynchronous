using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
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

namespace CancellationAsync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly char[] delimiters = { ' ', ',', '.', ';', ':', '-', '_', '/', '\u000A' };
        CancellationTokenSource cts;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            cts = new CancellationTokenSource();
            TimeSpan timeoutPeriod = TimeSpan.FromSeconds(5);
            cts.CancelAfter(timeoutPeriod);
            CancellationToken token = cts.Token;

            StartButton.IsEnabled = false;

            try
            {
                var result = await GetWordCountAsync(token);
                TextResult.Text += String.Format("Origin of Species wordcount: {0}", result);
            }
            catch (OperationCanceledException)
            {
                TextResult.Text += "The operation was cancelled. \n";
            }
            catch (Exception ex)
            {
                TextResult.Text += String.Format("An error has occurred: {0} \n", ex.Message);
            }
            finally
            {
                StartButton.IsEnabled = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel();
            }
        }

        public async Task<int> GetWordCountAsync(CancellationToken ct)
        {
            TextResult.Text += "Getting the word count for Origin ofSpecies...\n";
            var client = new HttpClient();
            await Task.Delay(500);
            try
            {
                HttpResponseMessage response = await client.GetAsync(@"http://www.gutenberg.org/files/2009/2009.txt", ct);
                var words = await response.Content.ReadAsStringAsync();
                var wordArray = words.Split(delimiters, StringSplitOptions.
                RemoveEmptyEntries);
                return wordArray.Count();
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}
