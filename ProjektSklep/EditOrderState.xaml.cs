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
    /// Interaction logic for EditProductsInWarehouse.xaml
    /// </summary>
    public partial class EditOrderState : Window
    {
        Order order = new Order();
        List<string> states = ["W trakcie realizacji", "Zrealizowane", "Anulowane"];

        public string InputState
        {
            get
            {
                return order.state;
            }
            set
            {
                order.state = value;
            }
        }

        public EditOrderState(Order order)
        {
            InitializeComponent();

            MyDbContext db = new MyDbContext();


            if(states.Find(state => state == order.state) != null)
                states.Remove(order.state);

            foreach (string state in states)
            {
                orderStateComboBox.Items.Add(state);
            }

            this.order = order;

            orderProductsListBox.ItemsSource = order.ProductOrder;

            orderStateLabel.Content = order.state;
        }

        private void CloseEditInWaregouseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            order.state = orderStateComboBox.SelectedValue.ToString();
            MyDbContext db = new MyDbContext();

            db.Orders.AddOrUpdate(order);

            db.SaveChanges();

            this.DialogResult = true;
        }
    }
}
