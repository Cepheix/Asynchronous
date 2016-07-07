using System;
using System.Collections.Generic;
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

namespace UIUpdate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                char[] delimiters = {' ', ',', '.', ';', ':', '-','_', '/', '\u000A'};
                var client = new WebClient();
                const string headerText = "Mozilla/5.0 (compatible;MSIE 10.0; Windows NT 6.1; Trident / 6.0)";
                client.Headers.Add("user-agent", headerText);
                try
                {
                    var words = client.DownloadString(@"http://www.gutenberg.org/files/2009/2009.txt");
                    var wordArray = words.Split(delimiters,
                    StringSplitOptions.RemoveEmptyEntries);
                    return wordArray;
                }
                finally
                {
                    client.Dispose();
                }
            }).
            ContinueWith(antecedent =>
            {
                lblWordCount.Content = String.Concat("Origin ofSpecies word count: ", antecedent.Result.Count().ToString());
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
