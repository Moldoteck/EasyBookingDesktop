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

namespace TIP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();
        public MainWindow()
        {
            InitializeComponent();
            label1.HorizontalContentAlignment = HorizontalAlignment.Center;
        }

        private async void login_Click(object sender, RoutedEventArgs e)
        {
            var values = new Dictionary<string, string> { { username.Text, password.Text } };
            try
            {
                var response = await client.GetStringAsync("http://localhost:8080/EasyBookingWebServicesServer/api/users/" + username.Text + "/" + password.Text);
                if (response == "true")
                {
                    Home home = new Home(username.Text);
                    home.Top = this.Top;
                    home.Left = this.Left;
                    home.Show();
                    this.Close();
                }
                else
                {
                    label1.Content = "Sorry, try again";
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
        }
        private async void register_Click(object sender, RoutedEventArgs e)
        {
            HttpContent content = null;
            String temp = "<user>\n<username>" + username.Text + "</username>\n<password>" + password.Text + "</password>\n</user> ";

            var stringContent = new StringContent(temp, Encoding.UTF8, "application/xml");

            var response = await client.PostAsync("http://localhost:8080/EasyBookingWebServicesServer/api/users/", stringContent);
            if(response.StatusCode.ToString()=="Created")
            {
                Home home = new Home(username.Text);
                home.Top = this.Top;
                home.Left = this.Left;
                home.Show();
                this.Close();
            }
            else
            {
                label1.Content = "Sorry, already exists";
            }
        }
    }
}
