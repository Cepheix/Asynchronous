using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace FileAsync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string path = @"C:\temp\temp.txt";
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void WriteButton_Click(object sender, RoutedEventArgs e)
        {
            WriteButton.IsEnabled = false;
            string content = TextWrite.Text;
            await WriteToFileAsync(path, content);
            WriteButton.IsEnabled = true;
        }

        private async void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(path) == false)
            {
                TextRead.Text = "There was an error reading the file.";
            }
            else
            {
                try
                {
                    string content = await ReadFromFileAsync(path);
                    TextRead.Text = content;
                }
                catch (Exception ex)
                {
                    TextRead.Text = ex.Message;
                }
            }
        }

        private async Task WriteToFileAsync(string path, string content)
        {
            byte[] encodedContent = Encoding.Unicode.GetBytes(content);
            using (FileStream stream = new FileStream(path, FileMode.Append,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            useAsync: true))
            {
                await stream.WriteAsync(encodedContent, 0, encodedContent.Length);
            };
        }

        private async Task<string> ReadFromFileAsync(string path)
        {
            using (FileStream stream = new FileStream(path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            useAsync: true))
            {
                var sb = new StringBuilder();
                byte[] buffer = new byte[0x1000];
                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    string content = Encoding.Unicode.GetString(buffer,
                    0,
                    bytesRead);
                    sb.Append(content);
                }
                return sb.ToString();
            }
        }
    }
}
