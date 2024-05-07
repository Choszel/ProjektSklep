using Microsoft.Win32;
using ProjektSklep.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
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
using static System.Net.Mime.MediaTypeNames;

namespace ProjektSklep
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        MyDbContext db;
        public LoginWindow()
        {
            InitializeComponent();
            db = new MyDbContext();
        }

        private void OnLoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = loginTextBox.Text;

            var sha = new System.Security.Cryptography.SHA256Managed();

            //hashowanie hasla na potem, do przerobienia kontrolka na passwordbox, i password na hash
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(passwordTextBox.Text);
            byte[] hashBytes = sha.ComputeHash(textBytes);
            string hash = BitConverter
                     .ToString(hashBytes)
                     .Replace("-", String.Empty);
            passwordTextBox.Text = null;

            var user = db.Users.FirstOrDefault(
                   e => e.login == login &&
                   e.password == hash);

            if (user == null) 
            { 
                MessageBox.Show("Nie udało się zalogować");
                DialogResult = false;
            }
            else
            {
                MessageBox.Show("Pomyślnie zalogowano");
                UserType.Instance.numericType = user.type;
                this.DialogResult = true;
            }
        }

        private void registerButtonClicked(object sender, RoutedEventArgs e)
        {
            Button changeButton = sender as Button;

            if (changeButton.Name == "changeToRegisterButton")
            {
                this.Height = 250;
                secondRowInLogin.Height = GridLength.Auto;

                loginButton.Click += new RoutedEventHandler(registringButtonClicked);
                loginButton.Click -= new RoutedEventHandler(OnLoginButton_Click);

                changeButton.Name = "changeToLoginButton";
                changeButton.Content = "Masz już konto? Zaloguj się";
                loginButton.Content = "Zarejestruj Się";
            }
            else
            {
                this.Height = 150;
                secondRowInLogin.Height = new GridLength(0.01, GridUnitType.Star);

                changeButton.Name = "changeToRegisterButton";
                changeButton.Content = "Nie masz konta? Zarejestruj się";
                loginButton.Content = "Zaloguj Się";

                loginButton.Click -= new RoutedEventHandler(registringButtonClicked);
                loginButton.Click += new RoutedEventHandler(OnLoginButton_Click);
            }



            //RegisterWindow registerWindow = new RegisterWindow();
            //this.DialogResult = true;
            //registerWindow.ShowDialog();
        }

        private void registringButtonClicked(object sender, RoutedEventArgs e)
        {

            string name = nameTextBox.Text;
            string login = loginTextBox.Text;
            string email = emailTextBox.Text;

            if (!isRegistrationCorrect())
            {
                return;
            }

            var sha = new System.Security.Cryptography.SHA256Managed();

            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(passwordTextBox.Text);
            byte[] hashBytes = sha.ComputeHash(textBytes);
            string hash = BitConverter
                     .ToString(hashBytes)
                     .Replace("-", String.Empty);
            passwordTextBox.Text = null;

            User newUser = new User(name, login, hash, email, 1);

            db.Users.Add(newUser);
            db.SaveChanges();
            DialogResult = false;
            MessageBox.Show("Pomyślnie zarejestrowano.");
        }

        private bool isRegistrationCorrect()
        {
            if (nameTextBox.Text == "")
            {
                MessageBox.Show("Nie podano nazwy konta.");
                return false;
            }
            else if (loginTextBox.Text == "")
            {
                MessageBox.Show("Nie podano loginu.");
                return false;
            }
            else if (db.Users.FirstOrDefault(e => e.login == loginTextBox.Text) != null)
            {
                MessageBox.Show("Podany login jest zajęty.");
                return false;
            }
            else if (emailTextBox.Text == "")
            {
                MessageBox.Show("Nie podano emaila.");
                return false;
            }
            else if (passwordTextBox.Text == "")
            {
                MessageBox.Show("Nie podano hasła.");
                return false;
            }
            else
                return true;
        }

        //do usuniecia placeholder
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                // Create source
                BitmapImage myBitmapImage = new BitmapImage();

                // BitmapImage.UriSource must be in a BeginInit/EndInit block
                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = new Uri(openFileDialog.FileName);

                // To save significant application memory, set the DecodePixelWidth or
                // DecodePixelHeight of the BitmapImage value of the image source to the desired
                // height or width of the rendered image. If you don't do this, the application will
                // cache the image as though it were rendered as its normal size rather than just
                // the size that is displayed.
                // Note: In order to preserve aspect ratio, set DecodePixelWidth
                // or DecodePixelHeight but not both.
                myBitmapImage.DecodePixelWidth = 200;
                myBitmapImage.EndInit();

                //konwersja do db
                byte[] data;
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create(myBitmapImage));

                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    data = ms.ToArray();
                }

                MyDbContext db = new MyDbContext();

                Images placeImages= new Images();

                placeImages.image = data;

                db.Images.Add(placeImages);

                db.SaveChanges();

                //koniec z db

                //set image source
                placeholderImage.Source = myBitmapImage;
            }
        }
    }
}
