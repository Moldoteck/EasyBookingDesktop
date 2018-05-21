using Newtonsoft.Json;
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
using System.Windows.Shapes;

namespace TIP
{
    /// <summary>
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class Profile : Window
    {
        private string username = "";
        private static readonly HttpClient client = new HttpClient();


        public class UserDetailsGet
        {
            public string user_id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string email { get; set; }
            public string phone_number { get; set; }
            public string path_img { get; set; }
        }

        public Profile()
        {
            InitializeComponent();
            reset_Click(null, null);
        }
        public Profile(string _username)
        {
            username = _username;
            InitializeComponent();
            reset_Click(null, null);
        }

        private void my_home_Click(object sender, RoutedEventArgs e)
        {
            MyHome myHome = new MyHome(username);
            myHome.Top = this.Top;
            myHome.Left = this.Left;
            myHome.Show();
            this.Close();
        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainwindow = new MainWindow();
            mainwindow.Top = this.Top;
            mainwindow.Left = this.Left;
            mainwindow.Show();
            this.Close();
        }

        private async void save_Click(object sender, RoutedEventArgs e)
        {
            if (firstNameBox.Text != "" && lastNameBox.Text != "" && numberBox.Text != "" && emailBox.Text != "" && imageBox.Text != "")
            {
                double price = 0;
                bool succes = Double.TryParse(numberBox.Text, out price);
                if (succes == true)
                {
                    var firstresponse = await client.GetStringAsync("http://localhost:8080/EasyBookingWebServicesServer/api/users/" + username);
                    if (firstresponse == "-1")
                    {
                        MessageBox.Show("Fatal error. Please Log out!");
                        return;
                    }

                    HttpContent content = null;
                    string temp = "<userDetails>\n<user_id>" + firstresponse
                        + "</user_id>\n<first_name>" + firstNameBox.Text
                        + "</first_name>\n<last_name>" + lastNameBox.Text
                        + "</last_name>\n<email>" + emailBox.Text
                        + "</email>\n<phone_number>" + numberBox.Text
                        + "</phone_number>\n<path_img>" + imageBox.Text
                        + "</path_img>\n</userDetails>";
                    var stringContent = new StringContent(temp, Encoding.UTF8, "application/xml");

                    var response = await client.PostAsync("http://localhost:8080/EasyBookingWebServicesServer/api/users_details/", stringContent);
                    if (response.StatusCode.ToString() == "Created")
                    {
                        MessageBox.Show("Saved with succes!");
                    }
                    else
                    {
                        MessageBox.Show("Sorry, something went wrong");
                    }
                }
                else
                {
                    MessageBox.Show("The number contains invalid characters");
                }
            }
            else
            {
                MessageBox.Show("You have not entered all the fields");
            }
        }

        private async void reset_Click(object sender, RoutedEventArgs e)
        {
            firstNameBox.Text = "";
            lastNameBox.Text = "";
            imageBox.Text = "";
            numberBox.Text = "";
            emailBox.Text = "";
            var firstresponse = await client.GetStringAsync("http://localhost:8080/EasyBookingWebServicesServer/api/users/" + username);
            if (firstresponse == "-1")
            {
                MessageBox.Show("Fatal error. Please Log out!");
                return;
            }

            var response = await client.GetStringAsync("http://localhost:8080/EasyBookingWebServicesServer/api/users_details/" + firstresponse);
            if (response != "")
            {
                UserDetailsGet udg = new UserDetailsGet();
                udg = JsonConvert.DeserializeObject<UserDetailsGet>(response);
                firstNameBox.Text = udg.first_name;
                lastNameBox.Text = udg.last_name;
                imageBox.Text = udg.path_img;
                numberBox.Text = udg.phone_number;
                emailBox.Text = udg.email;
                lostFocus(null, null);
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Home homewindow = new Home(username, "");
            homewindow.Top = this.Top;
            homewindow.Left = this.Left;
            homewindow.Show();
            this.Close();
        }

        private void lostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                var fullFilePath = imageBox.Text;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
                bitmap.EndInit();

                image.Source = bitmap;
                image.Height = 150;
                image.Width = 150;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
        }
    }
}
