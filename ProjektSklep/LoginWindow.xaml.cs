using ProjektSklep.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            /* hashowanie hasla na potem, do przerobienia kontrolka na passwordbox, i password na hash
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(passwordTextBox.Text);
            byte[] hashBytes = sha.ComputeHash(textBytes);
            string hash = BitConverter
                     .ToString(hashBytes)
                     .Replace("-", String.Empty);
            passwordTextBox.Text = null;
            */

            string password = passwordTextBox.Text;

            if (db.Users.FirstOrDefault(
                    e => e.login == login &&
                    e.password == password) == null)
            {
                MessageBox.Show("Nie udało się zalogować");
                DialogResult = false;
            }
            else
            {
                MessageBox.Show("Pomyślnie zalogowano");
                this.DialogResult = true;
            }
        }

        private void registerButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Height = 250;
            secondRowInLogin.Height = GridLength.Auto;

            loginButton.Click += new RoutedEventHandler(registringButtonClicked); 
            //RegisterWindow registerWindow = new RegisterWindow();
            //this.DialogResult = true;
            //registerWindow.ShowDialog();
        }

        private void registringButtonClicked(object sender, RoutedEventArgs e)
        {

            string name = nameTextBox.Text;
            string login = loginTextBox.Text;
            string email = emailTextBox.Text;

            if (nameTextBox.Text == null)
            {
                MessageBox.Show("Nie podano nazwy konta.");
            }
            else if(loginTextBox.Text == null)
            {
                MessageBox.Show("Nie podano loginu.");
            }
            else if(emailTextBox.Text == null)
            {
                MessageBox.Show("Nie podano emaila.");
            }
            else if(passwordTextBox.Text == null)
            {
                MessageBox.Show("Nie podano hasła.");
            }

            var sha = new System.Security.Cryptography.SHA256Managed();

            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(passwordTextBox.Text);
            byte[] hashBytes = sha.ComputeHash(textBytes);
            string hash = BitConverter
                     .ToString(hashBytes)
                     .Replace("-", String.Empty);
            passwordTextBox.Text = null;
            
            User newUser = new User(name,login,hash,email,1);

            db.Users.Add(newUser);
            db.SaveChanges();
        }
    }
}
