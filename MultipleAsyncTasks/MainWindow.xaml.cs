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

namespace MultipleAsyncTasks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        char[] delimiters = { ' ', ',', '.', ';', ':', '-', '_', '/', '\u000A' };
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            TextResult.Text += "Started downloading books...\n";
            await GetMultipleWordCount();
            TextResult.Text += "Finished downloading books...\n";
        }

        private Dictionary<string, string> GetBookUrls()
        {
            Dictionary<string, string> urlList = new Dictionary<string, string>
            {
                { "Origin of Species", "http://www.gutenberg.org/files/2009/2009.txt" },
                { "Beowulf","http://www.gutenberg.org/files/16328/16328-8.txt" },
                { "Ulysses","http://www.gutenberg.org/files/4300/4300.txt" }
            };

            return urlList;
        }

        async Task<KeyValuePair<string, int>> ProcessBook(KeyValuePair<string, string> book, HttpClient client)
        {
            var bookContents = await client.GetStringAsync(book.Value);
            var wordArray = bookContents.Split(delimiters,
            StringSplitOptions.RemoveEmptyEntries);
            return new KeyValuePair<string, int>(book.Key, wordArray.Count());
        }

        public async Task GetMultipleWordCount()
        {
            HttpClient client = new HttpClient();

            Dictionary<string, int> results = new Dictionary<string, int>();
            Dictionary<string, string> urlList = GetBookUrls();
            List<Task<KeyValuePair<string, int>>> bookTasks = new List<Task<KeyValuePair<string, int>>>();

            foreach (KeyValuePair<string, string> book in urlList)
            {
                Task<KeyValuePair<string, int>> task = ProcessBook(book, client);
                bookTasks.Add(task);
            }

            //IEnumerable<Task<KeyValuePair<string, int>>> bookQuery =
            //from book in urlList select ProcessBook(book, client);
            //List<Task<KeyValuePair<string, int>>> bookTasks = bookQuery.
            //ToList();

            while (bookTasks.Count > 0)
            {
                Task<KeyValuePair<string, int>> firstFinished = await Task.
                WhenAny(bookTasks);
                bookTasks.Remove(firstFinished);
                var thisBook = await firstFinished;
                TextResult.Text += String.Format("Finished downloading {0}.Word count: {1}\n", thisBook.Key, thisBook.Value);
            }
        }
    }
}
