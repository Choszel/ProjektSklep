using ProjektSklep.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
        List<CartProduct> cart;
        MyDbContext db = new MyDbContext();
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
        public ShippingDetailsWindow(List<CartProduct> cart)
        {
            InitializeComponent();
            this.cart = cart;
        }

        private void PlaceOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isDataValid())
            {
                return;
            }

            float price = 0;

            //order.discount = discount ?;--dododania

            foreach (CartProduct product in cart)
            {
                price += product.singlePrice * product.count;
            }

            int loggedUserID = UserType.Instance.loggedId;
            var currentUser = db.Users.FirstOrDefault(u => u.userId == loggedUserID);

            order.totalPrice = price;
            order.orderDate = DateTime.Now;

            order.userId = UserType.Instance.loggedId;
            User user = db.Users.Find(UserType.Instance.loggedId);
            order.user = user;
            order.discount = currentUser.currDiscount;
            db.Orders.Add(order);

            db.SaveChanges();

            foreach (CartProduct product in cart)
            {
                ProductOrder productOrder = new ProductOrder();
                productOrder.productId = product.id;
                productOrder.orderId = order.orderId;
                productOrder.count = product.count;

                Warehouse warehouse = db.Warehouse.First(warehouse => warehouse.productId == product.id);
                if(warehouse.actualState < productOrder.count)
                {
                    MessageBox.Show("Nie posiadamy już produktu: " + product.name + " na stanie.");
                    return;
                }

                db.ProductOrders.Add(productOrder);
            }

            foreach (CartProduct product in cart)
            {
                ProductOrder productOrder = new ProductOrder();
                productOrder.productId = product.id;
                productOrder.orderId = order.orderId;
                productOrder.count = product.count;

                Warehouse warehouse = db.Warehouse.First(warehouse => warehouse.productId == product.id);
                warehouse.actualState -= productOrder.count;

                db.Warehouse.AddOrUpdate(warehouse);
                db.ProductOrders.Add(productOrder);

                db.SaveChanges();
            }

            if (currentUser != null)
            {
                currentUser.currDiscount = null;
            }

            db.SaveChanges();

            this.DialogResult = true;
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

            if (streetTextBox.Text == "")
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
