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
    /// Interaction logic for HouseDetails.xaml
    /// </summary>
    public partial class HouseDetails : Window
    {
        private string owner_id = "";
        private string search_words = "";
        string username = "";

        private static readonly HttpClient client = new HttpClient();
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

        public class UsersDetailsGet
        {
            public string user_id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string email { get; set; }
            public string phone_number { get; set; }
            public string path_img { get; set; }
        }
        public HouseDetails()
        {
            InitializeComponent();
        }
        public HouseDetails(string _username, string id_user, string search_string)
        {
            username = _username;
            owner_id = id_user;
            search_words = search_string;
            InitializeComponent();
            getAllDetails();
        }
        private async void getAllDetails()
        {
            try
            {
                var response = await client.GetStringAsync("http://localhost:8080/EasyBookingWebServicesServer/api/home/fing_home_by_id/" + owner_id);

                response = "{\"data\" : [" + response + "]}";
                Console.WriteLine(response);
                Homes homesFromServer = new Homes();// JavaScriptSerializer().Deserialize<Homes>(response);
                homesFromServer = JsonConvert.DeserializeObject<Homes>(response);
                Console.WriteLine(response);
                foreach (var item in homesFromServer.data)
                {
                    var image = new Image();
                    var fullFilePath = item.path_img;

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
                    bitmap.EndInit();

                    image.Source = bitmap;
                    image.Height = 300;
                    image.Width = 300;

                    supergrid.Height = 550;
                    supergrid.Width = 700;




                    Grid content = new Grid();
                    Grid rating = new Grid();
                    Grid rating_price = new Grid();
                    Grid hostedBy = new Grid();

                    RowDefinition titleMainRow = new RowDefinition();
                    titleMainRow.Height = new GridLength(30);
                    RowDefinition contentMainRow = new RowDefinition();
                    contentMainRow.Height = new GridLength(200);
                    RowDefinition ratingMainRow = new RowDefinition();
                    ratingMainRow.Height = new GridLength(50);
                    RowDefinition hostedByMainRow = new RowDefinition();
                    hostedByMainRow.Height = new GridLength(300);
                    supergrid.RowDefinitions.Add(titleMainRow);
                    supergrid.RowDefinitions.Add(contentMainRow);
                    supergrid.RowDefinitions.Add(ratingMainRow);
                    supergrid.RowDefinitions.Add(hostedByMainRow);


                    TextBlock mainTitle = new TextBlock();
                    mainTitle.Text = item.name;
                    mainTitle.TextWrapping = TextWrapping.Wrap;
                    mainTitle.FontFamily = new FontFamily("Script MT Bold");
                    mainTitle.FontSize = 25;
                    mainTitle.FontStyle = FontStyles.Italic;
                    mainTitle.VerticalAlignment = VerticalAlignment.Center;
                    mainTitle.Height = 25;

                    Grid.SetRow(mainTitle, 0);
                    Grid.SetColumn(mainTitle, 0);
                    supergrid.Children.Add(mainTitle);



                    ColumnDefinition contentLeftColumn = new ColumnDefinition();
                    contentLeftColumn.Width = new GridLength(300);
                    ColumnDefinition contentRightColumn = new ColumnDefinition();
                    contentRightColumn.Width = new GridLength(450);
                    content.ColumnDefinitions.Add(contentLeftColumn);
                    content.ColumnDefinitions.Add(contentRightColumn);

                    Grid.SetRow(image, 0);
                    Grid.SetColumn(image, 0);
                    content.Children.Add(image);

                    TextBlock mainDescription = new TextBlock();
                    mainDescription.Text = item.description;
                    mainDescription.TextWrapping = TextWrapping.Wrap;
                    mainDescription.FontFamily = new FontFamily("Libre Baskerville");
                    mainDescription.FontSize = 15;
                    mainDescription.VerticalAlignment = VerticalAlignment.Center;
                    mainDescription.Height = 100;
                    mainDescription.Width = 350;
                    mainDescription.HorizontalAlignment = HorizontalAlignment.Left;
                    mainDescription.Margin = new Thickness(10, 0, 0, 0);


                    Grid.SetRow(mainDescription, 0);
                    Grid.SetColumn(mainDescription, 1);
                    content.Children.Add(mainDescription);

                    Grid.SetRow(content, 1);
                    Grid.SetColumn(content, 0);
                    supergrid.Children.Add(content);


                    /////////////////////////////////////////////////////


                    ColumnDefinition rating_priceLeftColumn = new ColumnDefinition();
                    rating_priceLeftColumn.Width = new GridLength(300);
                    ColumnDefinition rating_priceRightColumn = new ColumnDefinition();
                    rating_priceRightColumn.Width = new GridLength(450);
                    rating_price.ColumnDefinitions.Add(rating_priceLeftColumn);
                    rating_price.ColumnDefinitions.Add(rating_priceRightColumn);

                    RowDefinition ratingLabelRow = new RowDefinition();
                    ratingLabelRow.Height = new GridLength(25);
                    RowDefinition ratingValueRow = new RowDefinition();
                    ratingValueRow.Height = new GridLength(25);
                    rating.RowDefinitions.Add(ratingLabelRow);
                    rating.RowDefinitions.Add(ratingValueRow);




                    Border fullScoreBar = new Border();

                    fullScoreBar.Background = Brushes.Red;//  Colors.Red);
                    fullScoreBar.HorizontalAlignment = HorizontalAlignment.Left;
                    fullScoreBar.Height = 20;
                    fullScoreBar.Width = 300;

                    Border realScoreBar = new Border();
                    realScoreBar.HorizontalAlignment = HorizontalAlignment.Left;
                    int stars = 0, nr_review = 1;
                    Int32.TryParse(item.stars, out stars);
                    Int32.TryParse(item.nr_review, out nr_review);
                    byte finalColor = (byte)(255 * (stars * 100 / (nr_review * 5)) / 100);
                    byte newcolor = (byte)(255 - finalColor);

                    realScoreBar.Background = new LinearGradientBrush(Colors.Gray, Color.FromArgb(255, newcolor, finalColor, newcolor), 0.6);
                    realScoreBar.Height = 20;
                    realScoreBar.Width = 3 * (stars * 100 / (nr_review * 5));



                    TextBlock ratingLabel = new TextBlock();
                    ratingLabel.Text = "Rating";
                    ratingLabel.TextWrapping = TextWrapping.Wrap;
                    ratingLabel.TextAlignment = TextAlignment.Center;
                    ratingLabel.FontFamily = new FontFamily("Script MT Bold");
                    ratingLabel.FontSize = 25;
                    ratingLabel.VerticalAlignment = VerticalAlignment.Center;
                    ratingLabel.Height = 25;
                    ratingLabel.Width = 300;
                    ratingLabel.HorizontalAlignment = HorizontalAlignment.Center;

                    TextBlock housePrice = new TextBlock();
                    housePrice.Text = "Price: "+item.price+"$";
                    housePrice.TextWrapping = TextWrapping.Wrap;
                    housePrice.FontFamily = new FontFamily("Script MT Bold");
                    housePrice.FontSize = 25;
                    housePrice.VerticalAlignment = VerticalAlignment.Center;
                    housePrice.Height = 25;
                    housePrice.Width = 300;
                    housePrice.HorizontalAlignment = HorizontalAlignment.Left;
                    housePrice.Margin = new Thickness(10, 0, 0, 0);

                    rating.HorizontalAlignment = HorizontalAlignment.Center;




                    Grid.SetRow(ratingLabel, 0);
                    Grid.SetColumn(ratingLabel, 0);
                    rating.Children.Add(ratingLabel);

                    Grid.SetRow(fullScoreBar, 1);
                    Grid.SetColumn(fullScoreBar, 0);
                    rating.Children.Add(fullScoreBar);

                    Grid.SetRow(realScoreBar, 1);
                    Grid.SetColumn(realScoreBar, 0);
                    rating.Children.Add(realScoreBar);

                    Grid.SetRow(rating, 0);
                    Grid.SetColumn(rating, 0);
                    rating_price.Children.Add(rating);

                    Grid.SetRow(housePrice, 0);
                    Grid.SetColumn(housePrice, 1);
                    rating_price.Children.Add(housePrice);

                    //*/*/*/*/

                    Grid.SetRow(rating_price, 2);
                    Grid.SetColumn(rating_price, 0);
                    supergrid.Children.Add(rating_price);



                    /////////////////////////////////////////////////////////////////////////////////
                    response = null;
                    response = await client.GetStringAsync("http://localhost:8080/EasyBookingWebServicesServer/api/users_details/" + owner_id);

                    if (response != null)
                    {
                        RowDefinition hostedByImage = new RowDefinition();
                        hostedByImage.Height = new GridLength(120);
                        RowDefinition hostedByName = new RowDefinition();
                        hostedByName.Height = new GridLength(25);
                        hostedBy.RowDefinitions.Add(hostedByImage);
                        hostedBy.RowDefinitions.Add(hostedByName);


                        UsersDetailsGet userDetailsFromServer = new UsersDetailsGet();// JavaScriptSerializer().Deserialize<Homes>(response);
                        userDetailsFromServer = JsonConvert.DeserializeObject<UsersDetailsGet>(response);

                        TextBlock userName = new TextBlock();
                        userName.Text = userDetailsFromServer.first_name+" "+userDetailsFromServer.last_name;
                        userName.TextWrapping = TextWrapping.Wrap;
                        userName.FontFamily = new FontFamily("Libre Baskerville");
                        userName.FontSize = 25;
                        userName.FontStyle = FontStyles.Italic;
                        userName.VerticalAlignment = VerticalAlignment.Center;
                        userName.Height = 100;

                        var imageProfile = new Image();
                        var fullFilePathProfile = userDetailsFromServer.path_img;

                        BitmapImage bitmapProfile = new BitmapImage();
                        bitmapProfile.BeginInit();
                        bitmapProfile.UriSource = new Uri(fullFilePathProfile, UriKind.Absolute);
                        bitmapProfile.EndInit();

                        imageProfile.Source = bitmapProfile;
                        imageProfile.Height = 200;
                        imageProfile.Width = 200;

                        imageProfile.VerticalAlignment = VerticalAlignment.Top;
                        imageProfile.HorizontalAlignment = HorizontalAlignment.Left;
                        userName.VerticalAlignment = VerticalAlignment.Top;
                        userName.HorizontalAlignment= HorizontalAlignment.Center;
                        hostedBy.VerticalAlignment = VerticalAlignment.Top;
                        hostedBy.HorizontalAlignment = HorizontalAlignment.Left;

                        Grid.SetRow(imageProfile, 0);
                        Grid.SetColumn(imageProfile, 0);
                        hostedBy.Children.Add(imageProfile);

                        Grid.SetRow(userName, 1);
                        Grid.SetColumn(userName, 0);
                        hostedBy.Children.Add(userName);

                        Grid.SetRow(hostedBy, 3);
                        Grid.SetColumn(hostedBy, 0);
                        supergrid.Children.Add(hostedBy);

                    }
                    //supergrid.ShowGridLines = true;
                    //hostedBy.ShowGridLines = true;
                    //content.ShowGridLines = true;
                    //rating.ShowGridLines = true;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainwindow = new MainWindow();
            mainwindow.Top = this.Top;
            mainwindow.Left = this.Left;
            mainwindow.Show();
            this.Close();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Home homewindow = new Home(username, search_words);
            homewindow.Top = this.Top;
            homewindow.Left = this.Left;
            homewindow.Show();
            this.Close();
        }
    }
}
