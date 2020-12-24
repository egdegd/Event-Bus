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
using Newtonsoft.Json;
using Model;
using WebServiceClient;

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string curMessage = "";
        string name = "";
        static readonly EventBusClient client = new EventBusClient();

        public MainWindow()
        {
            InitializeComponent();
            RequestMsgAsync();
        }

        private void RequestMsg()
        {
            while (true)
            {
                Thread.Sleep(1000);
                var result = client.SendEvent(name);

                if (result != "\"no new events\"")
                {
                    List<Event> actualResultFromGet = JsonConvert.DeserializeObject<List<Event>>(result);
                    foreach (Event e in actualResultFromGet)
                    {
                        Dispatcher.Invoke(() => chatBox.AppendText(e.Organizer + ": " + e.Description + "\n"));
                    }

                }
            }
        }

        private async void RequestMsgAsync()
        {
            await Task.Run(() => RequestMsg());
        }

        private void sendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(curMessage))
            {
                client.Publish(name, name, curMessage);
                messageText.Clear();
            }
        }

        private void messageText_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            curMessage = textBox.Text;
        }

        private void flag1_Checked(object sender, RoutedEventArgs e)
        {
            client.Subscribe(name, serviceName1.Text);
        }

        private void flag1_Unchecked(object sender, RoutedEventArgs e)
        {
            client.Unsubscribe(name, serviceName1.Text);
        }

        private void flag2_Checked(object sender, RoutedEventArgs e)
        {
            client.Subscribe(name, serviceName2.Text);
        }

        private void flag2_Unchecked(object sender, RoutedEventArgs e)
        {
            client.Unsubscribe(name, serviceName2.Text);
        }

        private void flag3_Checked(object sender, RoutedEventArgs e)
        {
            client.Subscribe(name, serviceName3.Text);
        }

        private void flag3_Unchecked(object sender, RoutedEventArgs e)
        {
            client.Unsubscribe(name, serviceName3.Text);
        }

        private void flag4_Checked(object sender, RoutedEventArgs e)
        {
            client.Subscribe(name, serviceName4.Text);
        }

        private void flag4_Unchecked(object sender, RoutedEventArgs e)
        {
            client.Unsubscribe(name, serviceName4.Text);
        }

        private void localName_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            name = textBox.Text;
        }
    }
}
