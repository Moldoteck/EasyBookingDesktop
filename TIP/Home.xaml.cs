using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TIP
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        private string username="";
        private static readonly HttpClient client = new HttpClient();
        private List<string> users_id = new List<string>();
        public class Homes
        {
            public List<HomesGet> data { get; set; }
        }
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
        public Home()
        {
            InitializeComponent();
        }
        public Home(string _username)
        {
            InitializeComponent();
            username = _username;
        }
        public Home(string _username, string searchwords)
        {
            InitializeComponent();
            searchbox.Text = searchwords;
            username = _username;
            searchbutton_Click(null, null);
        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainwindow = new MainWindow();
            mainwindow.Top = this.Top;
            mainwindow.Left = this.Left;
            mainwindow.Show();
            this.Close();
        }

        private async void searchbutton_Click(object sender, RoutedEventArgs e)
        {
            users_id.Clear();
            listBox.Items.Clear();
            var values = new Dictionary<string, string> { { searchbox.Text, searchbox.Text } };
            try
            {
                var response = await client.GetStringAsync("http://localhost:8080/EasyBookingWebServicesServer/api/home/get_home_name/" + searchbox.Text);

                response = "{\"data\" : " + response + "}";
                Homes homesFromServer = new Homes();// JavaScriptSerializer().Deserialize<Homes>(response);
                homesFromServer = JsonConvert.DeserializeObject<Homes>(response);
                foreach (var item in homesFromServer.data)
                {
                    users_id.Add(item.id_user);
                    var image = new Image();
                    var fullFilePath = item.path_img;

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
                    bitmap.EndInit();

                    image.Source = bitmap;
                    image.Height = 150;
                    image.Width = 150;
                    Grid t = new Grid();
                    Grid textGrid = new Grid();

                    RowDefinition titleRow = new RowDefinition();
                    titleRow.Height = new GridLength(50);
                    RowDefinition descriptionRow = new RowDefinition();
                    textGrid.RowDefinitions.Add(titleRow);
                    textGrid.RowDefinitions.Add(descriptionRow);


                    ColumnDefinition firstColumn = new ColumnDefinition();
                    firstColumn.Width = new GridLength(150);
                    ColumnDefinition secondColumn = new ColumnDefinition();
                    t.ColumnDefinitions.Add(firstColumn);
                    t.ColumnDefinitions.Add(secondColumn);
                    image.VerticalAlignment = VerticalAlignment.Center;
                    Grid.SetRow(image, 0);
                    Grid.SetColumn(image, 0);
                    t.Children.Add(image);

                    TextBlock title = new TextBlock();
                    title.TextWrapping = TextWrapping.Wrap;
                    title.Text = item.name;
                    title.FontFamily = new FontFamily("Libre Baskerville");
                    title.FontSize = 30;
                    title.FontStyle = FontStyles.Italic;
                    title.VerticalAlignment = VerticalAlignment.Center;
                    title.Height = 50;
                    title.MaxWidth = 500;
                    title.Width = Double.NaN;
                    t.Height = 150;
                    t.Width = 700;
                    title.VerticalAlignment = VerticalAlignment.Center;
                    Grid.SetRow(title, 0);
                    Grid.SetColumn(title, 0);
                    textGrid.Children.Add(title);

                    TextBlock description = new TextBlock();
                    description.Text = item.description;
                    description.TextWrapping = TextWrapping.Wrap;
                    description.FontFamily = new FontFamily("Libre Baskerville");
                    description.FontSize = 15;
                    description.VerticalAlignment = VerticalAlignment.Center;
                    description.Height = 100;
                    description.MaxWidth = 500;
                    description.Width = Double.NaN;
                    description.VerticalAlignment = VerticalAlignment.Center;
                    Grid.SetRow(description, 1);
                    Grid.SetColumn(description, 0);

                    textGrid.Children.Add(description);

                    Grid.SetRow(textGrid, 0);
                    Grid.SetColumn(textGrid, 1);
                    t.Children.Add(textGrid);
                    listBox.Items.Add(t);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }

        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.Items.Count != 0 && listBox.SelectedIndex != -1)
            {
                HouseDetails houseDetails = new HouseDetails(username, users_id[listBox.SelectedIndex], searchbox.Text);
                houseDetails.Top = this.Top;
                houseDetails.Left = this.Left;
                houseDetails.Show();
                this.Close();
            }
        }

        private void my_profile_Click(object sender, RoutedEventArgs e)
        {
            Profile profilewindow = new Profile(username);
            profilewindow.Top = this.Top;
            profilewindow.Left = this.Left;
            profilewindow.Show();
            this.Close();
        }

        private void my_home_Click(object sender, RoutedEventArgs e)
        {
            MyHome myHome = new MyHome(username);
            myHome.Top = this.Top;
            myHome.Left = this.Left;
            myHome.Show();
            this.Close();
        }
    }
}
