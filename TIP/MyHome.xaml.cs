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
    /// Interaction logic for MyHome.xaml
    /// </summary>
    public partial class MyHome : Window
    {
        private string username = "";
        private static readonly HttpClient client = new HttpClient();

        public class HomesGet
        {
            public string name { get; set; }
            public string description { get; set; }
            public string price { get; set; }
            public string stars { get; set; }
            public string nr_review { get; set; }
            public string path_img { get; set; }
            public string id_user { get; set; }
        }

        public MyHome()
        {
            InitializeComponent();
            reset_Click(null, null);
        }
        public MyHome(string _username)
        {
            username = _username;
            InitializeComponent();
            reset_Click(null, null);
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Home homewindow = new Home(username, "");
            homewindow.Top = this.Top;
            homewindow.Left = this.Left;
            homewindow.Show();
            this.Close();
        }

        private void my_profile_Click(object sender, RoutedEventArgs e)
        {
            Profile profilewindow = new Profile(username);
            profilewindow.Top = this.Top;
            profilewindow.Left = this.Left;
            profilewindow.Show();
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
            if(titleBox.Text!="" && descriptionBox.Text!=""&&priceBox.Text!=""&&imageBox.Text!="")
            {
                double price = 0;
                bool succes= Double.TryParse(priceBox.Text, out price);
                if(succes==true)
                {
                    var firstresponse = await client.GetStringAsync("http://localhost:8080/EasyBookingWebServicesServer/api/users/" + username);
                    if(firstresponse=="-1")
                    {
                        MessageBox.Show("Fatal error. Please Log out!");
                        return;
                    }

                    HttpContent content = null;
                    string temp = "<home>\n<name>"+ titleBox.Text
                        + "</name>\n<description>"+ descriptionBox.Text
                        + "</description>\n<price>"+ priceBox.Text
                        + "</price>\n<stars>0</stars>\n<nr_review>0</nr_review>\n<path_img>"+ imageBox.Text
                        + "</path_img>\n<id_user>"+ firstresponse
                        + "</id_user>\n</home>";
                    var stringContent = new StringContent(temp, Encoding.UTF8, "application/xml");

                    var response = await client.PostAsync("http://localhost:8080/EasyBookingWebServicesServer/api/home/", stringContent);
                    if (response.StatusCode.ToString() == "Created")
                    {
                        MessageBox.Show("Saved with succes!");
                    }
                    else
                    {
                        MessageBox.Show("Sorry, already exists with this name");
                    }
                }
                else
                {
                    MessageBox.Show("The price contains invalid characters");
                }
            }
            else
            {
                MessageBox.Show("You have not entered all the fields");
            }
        }

        private async void reset_Click(object sender, RoutedEventArgs e)
        {
            descriptionBox.Text = "";
            titleBox.Text = "";
            imageBox.Text = "";
            priceBox.Text = "";
            var firstresponse = await client.GetStringAsync("http://localhost:8080/EasyBookingWebServicesServer/api/users/" + username);
            if (firstresponse == "-1")
            {
                MessageBox.Show("Fatal error. Please Log out!");
                return;
            }


            var response = await client.GetStringAsync("http://localhost:8080/EasyBookingWebServicesServer/api/home/fing_home_by_id/" + firstresponse);
            if(response != "")
            {
                HomesGet hg = new HomesGet();
                hg = JsonConvert.DeserializeObject<HomesGet>(response); ;
                descriptionBox.Text = hg.description;
                titleBox.Text = hg.name;
                imageBox.Text = hg.path_img;
                priceBox.Text = hg.price;
                lostFocus(null, null);
            }

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
            catch(Exception exc)
            {
                Console.WriteLine(exc);
            }
        }
    }
}
