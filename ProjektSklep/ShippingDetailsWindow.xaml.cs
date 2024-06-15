using ProjektSklep.Model;
using System;
using System.Collections.Generic;
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

namespace ProjektSklep
{
    /// <summary>
    /// Interaction logic for ShippingDetailsWindow.xaml
    /// </summary>
    public partial class ShippingDetailsWindow : Window
    {
        Order order = new Order();

        public string InputCountry
        {
            get 
            {
                return order.country;
            }
            set
            {
                order.country = value;
            }
        }

        public string InputCity
        {
            get
            {
                return order.city;
            }
            set
            {
                order.city = value;
            }
        }

        public string InputStreet
        {
            get
            {
                return order.street;
            }
            set
            {
                order.street = value;
            }
        }

        public string InputZipCode
        {
            get
            {
                return order.zipCode;
            }
            set
            {
                order.zipCode = value;
            }
        }
        public ShippingDetailsWindow()
        {
            InitializeComponent();
        }

        private void PlaceOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isDataValid())
            {
                return;
            }
            // dodawanie zamówienia do bazy danych
            this.Close();
        }

        private void CloseShippingWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool isDataValid()
        {
            string errorMessage = "";

            if (countryTextBox.Text == "")
            {
                errorMessage += "Nie podano kraju.\n";
            }

            if (cityTextBox.Text == "")
            {
                errorMessage += "Nie podano miasta.\n";
            }

            if(streetTextBox.Text == "")
            {
                errorMessage += "Nie podano ulicy.\n";
            }

            if (postalCodeTextBox.Text == "")
            {
                errorMessage += "Nie podano kodu pocztowego.\n";
            }

            if (errorMessage != "")
            {
                MessageBox.Show(errorMessage, "Błąd podczas edycji", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
                return true;
        }
    }
}
