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

        private User userInput = new User();

        public string LoginInput
        {
            get
            {
                return userInput.login;
            }
            set
            {
                userInput.login = value;
            }
        }

        public string NameInput
        {
            get
            {
                return userInput.name;
            }
            set
            {
                userInput.name = value;
            }
        }

        public string EmailInput
        {
            get
            {
                return userInput.email;
            }
            set
            {
                userInput.email = value;
            }
        }

        public LoginWindow()
        {
            InitializeComponent();
            db = new MyDbContext();
        }

        private void OnLoginButton_Click(object sender, RoutedEventArgs e)
        {
            var sha = new System.Security.Cryptography.SHA256Managed();

            //hashowanie hasla na potem, do przerobienia kontrolka na passwordbox, i password na hash
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(passwordTextBox.Password);
            byte[] hashBytes = sha.ComputeHash(textBytes);
            string hash = BitConverter
                     .ToString(hashBytes)
                     .Replace("-", String.Empty);

            var user = db.Users.FirstOrDefault(
                   e => e.login == userInput.login &&
                   e.password == hash);

            if (user == null)
            {
                MessageBox.Show("Wprowadzono nieprawidłowy login lub hasło","Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                passwordTextBox.Password = null;
                return;
            }
            else
            {
                MessageBox.Show("Pomyślnie zalogowano");
                passwordTextBox.Password = null;
                UserType.Instance.numericType = user.type;
                UserType.Instance.loggedId = user.userId;
                this.DialogResult = true;
            }
        }

        private void registerButtonClicked(object sender, RoutedEventArgs e)
        {
            Button changeButton = sender as Button;

            if (changeButton.Name == "changeToRegisterButton")
            {
                this.Height = 400;
                secondRowInLogin.Height = GridLength.Auto;

                loginButton.Click += new RoutedEventHandler(registringButtonClicked);
                loginButton.Click -= new RoutedEventHandler(OnLoginButton_Click);

                changeButton.Name = "changeToLoginButton";
                changeButton.Content = "Masz już konto? Zaloguj się";
                loginButton.Content = "Zarejestruj Się";
            }
            else
            {
                this.Height = 250;
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

             if(!isRegistrationCorrect())
            {
                return;
            }

            var sha = new System.Security.Cryptography.SHA256Managed();

            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(passwordTextBox.Password);
            byte[] hashBytes = sha.ComputeHash(textBytes);
            string hash = BitConverter
                     .ToString(hashBytes)
                     .Replace("-", String.Empty);
            passwordTextBox.Password = null;

            userInput.password = hash;
            userInput.type = 1;

            db.Users.Add(userInput);
            db.SaveChanges();
            DialogResult = false;
            MessageBox.Show("Pomyślnie zarejestrowano.");
        }

        private bool isRegistrationCorrect()
        {
            string errorMessage = "";
            if (nameTextBox.Text == "")
            {
                errorMessage += "Nie podano nazwy konta.\n";
            }
            if (loginTextBox.Text == "")
            {
                errorMessage += "Nie podano loginu.\n";
            }
            if (db.Users.FirstOrDefault(e => e.login == loginTextBox.Text) != null)
            {
                errorMessage += "Podany login jest zajęty.\n";
            }
            if (emailTextBox.Text == "")
            {
                errorMessage += "Nie podano emaila.\n";
            }
            if (passwordTextBox.Password == "")
            {
                errorMessage += "Nie podano hasła.\n";
            }
            if (passwordTextBox.Password.Length < 6)
            {
                errorMessage += "Hasło musi mieć conajmniej 6 znaków.\n";
            }

            if(errorMessage != "")
            {
                MessageBox.Show(errorMessage, "Błąd Rejestracji", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
                return true;
        }

        private void passwordValidation(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = (PasswordBox)sender;

            if(passwordBox.Password  == "") 
            {
                passwordErrorLabel.Visibility = Visibility.Visible;
                passwordErrorTextBlock.Text = "Hasło jest wymagane";
            }
            else
            if(passwordBox.Password.Length < 6 && loginButton.Content == "Zarejestruj Się")
            {
                passwordErrorTextBlock.Text = "Hasło musi mieć conajmniej 6 znaków";
            }
            else
            {
                passwordErrorLabel.Visibility= Visibility.Hidden;
            }
        }
    }
}
