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
    public partial class EditProductsInWarehouse : Window
    {
        Warehouse warehouse;

        public EditProductsInWarehouse(Warehouse warehouse)
        {
            InitializeComponent();

            MyDbContext db = new MyDbContext();


            this.warehouse = warehouse;

            ActualStateTextBox.Text = warehouse.actualState.ToString();
            StockLevelTextBox.Text  = warehouse.stockLevel.ToString();
        }

        private void CloseEditInWaregouseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            warehouse.actualState = int.Parse(ActualStateTextBox.Text);
            warehouse.stockLevel = int.Parse(StockLevelTextBox.Text);
            MyDbContext db = new MyDbContext();

            db.Warehouse.AddOrUpdate(warehouse);

            db.SaveChanges();

            this.DialogResult = true;
        }
    }
}
