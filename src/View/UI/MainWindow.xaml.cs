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

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string curMessage = "";
        string name = "";
        public MainWindow()
        {
            InitializeComponent();
            RequestMsgAsync();
        }

        private void RequestMsg()
        {
            var client = new HttpClient();
            while (true)
            {
                Thread.Sleep(1000);
                var response = client.GetAsync("http://localhost:9000/api/eventbus/sendevent?name=" + name).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                if (result != "\"no new events\"")
                {
                    Event actualResultFromGet = JsonConvert.DeserializeObject<Event>(result);
                    Dispatcher.Invoke(() => chatBox.AppendText(actualResultFromGet.Organizer + ": " + actualResultFromGet.Description + "\n"));

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
                var client = new HttpClient();
                Event evnt = new Event
                {
                    Type = name,
                    Description = curMessage,
                    Organizer = name
                };

                var serializedObject = JsonConvert.SerializeObject(evnt);
                var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
                var response = client.PostAsync("http://localhost:9000/api/eventbus/publish", content).Result;
                //chatBox.AppendText(curMessage + "\n");
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
            var client = new HttpClient();
            Pair p = new Pair
            {
                First = name,
                Second = serviceName1.Text
            };
            var serializedObject = JsonConvert.SerializeObject(p);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            var response = client.PostAsync("http://localhost:9000/api/eventbus/subscribe", content).Result;
        }
        private void flag1_Unchecked(object sender, RoutedEventArgs e)
        {
            var client = new HttpClient();
            Pair p = new Pair
            {
                First = name,
                Second = serviceName1.Text
            };
            var serializedObject = JsonConvert.SerializeObject(p);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            var response = client.PostAsync("http://localhost:9000/api/eventbus/unsubscribe", content).Result;
        }
        private void flag2_Checked(object sender, RoutedEventArgs e)
        {
            var client = new HttpClient();
            Pair p = new Pair
            {
                First = name,
                Second = serviceName2.Text
            };
            var serializedObject = JsonConvert.SerializeObject(p);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            var response = client.PostAsync("http://localhost:9000/api/eventbus/subscribe", content).Result;
        }
        private void flag2_Unchecked(object sender, RoutedEventArgs e)
        {
            var client = new HttpClient();
            Pair p = new Pair
            {
                First = name,
                Second = serviceName2.Text
            };
            var serializedObject = JsonConvert.SerializeObject(p);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            var response = client.PostAsync("http://localhost:9000/api/eventbus/unsubscribe", content).Result;
        }
        private void flag3_Checked(object sender, RoutedEventArgs e)
        {
            var client = new HttpClient();
            Pair p = new Pair
            {
                First = name,
                Second = serviceName3.Text
            };
            var serializedObject = JsonConvert.SerializeObject(p);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            var response = client.PostAsync("http://localhost:9000/api/eventbus/subscribe", content).Result;
        }
        private void flag3_Unchecked(object sender, RoutedEventArgs e)
        {
            var client = new HttpClient();
            Pair p = new Pair
            {
                First = name,
                Second = serviceName3.Text
            };
            var serializedObject = JsonConvert.SerializeObject(p);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            var response = client.PostAsync("http://localhost:9000/api/eventbus/unsubscribe", content).Result;
        }
        private void flag4_Checked(object sender, RoutedEventArgs e)
        {
            var client = new HttpClient();
            Pair p = new Pair
            {
                First = name,
                Second = serviceName4.Text
            };
            var serializedObject = JsonConvert.SerializeObject(p);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            var response = client.PostAsync("http://localhost:9000/api/eventbus/subscribe", content).Result;
        }
        private void flag4_Unchecked(object sender, RoutedEventArgs e)
        {
            var client = new HttpClient();
            Pair p = new Pair
            {
                First = name,
                Second = serviceName4.Text
            };
            var serializedObject = JsonConvert.SerializeObject(p);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            var response = client.PostAsync("http://localhost:9000/api/eventbus/unsubscribe", content).Result;
        }

        private void localName_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            name = textBox.Text;
        }
    }
}
