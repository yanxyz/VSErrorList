using System;
using System.Collections.Generic;
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
using System.Diagnostics;

namespace VSErrorList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string dialogDashes = "----------------------";

        public MainWindow()
        {
            InitializeComponent();
            txtInput.Text = Clipboard.GetText();
        }

        private void txtInput_TextChanged(object sender, RoutedEventArgs e)
        {
            var items = new List<LogItem>();
            var text = txtInput.Text;

            try
            {
                var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines[0].StartsWith(dialogDashes)) {
                    var arr = GetDialog(lines);
                    items.Add(new LogItem { Name = arr[0], Value = arr[1] });
                }
                else
                {
                    var a = lines[0].Split('\t');
                    var b = lines[1].Split('\t');
                    var i = 0;
                    foreach (var x in a)
                    {
                        items.Add(new LogItem { Name = x + ":", Value = b[i] });
                        ++i;
                    }
                }
            }
            catch (Exception)
            {
                //throw;
            }

            lbLog.ItemsSource = items;
        }

        private string[] GetDialog(string[] lines)
        {
            var ret = new string[2];
            var title = lines[1];
            if (title.StartsWith(dialogDashes)) ret[0] = "NoTitle";
            var beforeContent = false;

            var sb = new StringBuilder();
            for (var i = 2; i < lines.Length; i++)
            {
                var item = lines[i];
                if (item.StartsWith(dialogDashes))
                {
                    if (beforeContent)
                        continue;
                    else
                        break;
                }
                sb.Append(item);
                if (beforeContent) beforeContent = false;
            }
            ret[1] = sb.ToString();
            return ret;
        }

        private void lbLog_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CopySelectedItemValue();
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            CopySelectedItemValue();
        }

        private void CopySelectedItemValue()
        {
            var text = GetSelectedItemValue();
            try
            {
                Clipboard.SetText(text);
            }
            catch (Exception)
            {
                txtMessage.Text = text;
                txtMessage.SelectAll();
                txtMessage.Copy();
            }
        }

        private string GetSelectedItemValue()
        {
            if (lbLog.SelectedItem != null)
            {
                var text = (lbLog.SelectedItem as LogItem).Value;
                return text;
            }
            return null;
        }

        private void Google_Click(object sender, RoutedEventArgs e)
        {
            var text = GetSelectedItemValue();
            if (!String.IsNullOrEmpty(text))
            {
                var qs = System.Web.HttpUtility.UrlEncode(text);
                Process.Start($"https://www.google.com/search?q={qs}");
            }
        }
    }

    public class LogItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
