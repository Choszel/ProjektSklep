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
        Warehouse warehouse = new Warehouse();

        public int InputActualState
        {
            get
            {
                return warehouse.actualState;
            }
            set
            {
                warehouse.actualState = value;
            }
        }

        public int InputStockLevel
        {
            get
            {
                return warehouse.stockLevel;
            }
            set
            {
                warehouse.stockLevel = value;
            }
        }

        public EditProductsInWarehouse(Warehouse warehouse)
        {
            InitializeComponent();

            MyDbContext db = new MyDbContext();


            this.warehouse = warehouse;

            ActualStateTextBox.Text = warehouse.actualState.ToString();
            StockLevelTextBox.Text  = warehouse.stockLevel.ToString();

            NameTextBox.Content = warehouse.product.name;
        }

        private void CloseEditInWaregouseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (!isDataValid())
            {
                return;
            }

            warehouse.actualState = int.Parse(ActualStateTextBox.Text);
            warehouse.stockLevel = int.Parse(StockLevelTextBox.Text);
            MyDbContext db = new MyDbContext();

            db.Warehouse.AddOrUpdate(warehouse);

            db.SaveChanges();

            this.DialogResult = true;
        }

        private bool isDataValid()
        {
            string errorMessage = "";

            if (ActualStateTextBox.Text == "")
            {
                errorMessage += "Nie podano stanu aktualnego.\n";
            }
            else
            if (!int.TryParse(ActualStateTextBox.Text, out int actualState))
            {
                errorMessage += "Stan aktualny musi być liczbą.\n";
            }
            else
            {
                if (actualState < 0)
                {
                    errorMessage += "Podano ujemny stan aktualny.\n";
                }
            }

            if (StockLevelTextBox.Text == "")
            {
                errorMessage += "Nie podano stanu magazynowego.\n";
            }
            else
                if (!int.TryParse(StockLevelTextBox.Text, out int stockLevel))
            {
                errorMessage += "Stan magazynowy musi być liczbą.\n";
            }
            else
            {
                if (stockLevel < 0)
                {
                    errorMessage += "Podano ujemny stan magazynowy.\n";
                }
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
