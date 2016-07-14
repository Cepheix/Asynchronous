using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace WordCountAsync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly char[] delimiters = { ' ', ',', '.', ';', ':', '-', '_', '/', '\u000A' };

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            try
            {
                int result = await GetWordCount();
                TextResults.Text += String.Format("Origin of Species word count: {0}", result);
            }
            catch (AggregateException ex)
            {
                foreach (Exception exception in ex.InnerExceptions)
                {
                    Console.WriteLine("Caught excepton: {0}", exception.Message);
                }
            }
            finally
            {
                StartButton.IsEnabled = true;
            }
        }

        public async Task<int> GetWordCount()
        {
            TextResults.Text += "Getting the word count for Origin ofSpecies...\n";

            HttpClient client = new HttpClient();
            string bookContents = await client.GetStringAsync(@"http://www.gutenberg.org/files/2009/2009.txt");

            string[] wordArray = bookContents.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            return wordArray.Count();
        }
    }
}
