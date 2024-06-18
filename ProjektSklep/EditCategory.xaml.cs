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
    /// Interaction logic for EditCategory.xaml
    /// </summary>
    public partial class EditCategory : Window
    {
        Category category = new Category();

        public string InputCategory
        {
            get
            {
                return category.name;
            }
            set
            {
                category.name = value;
            }
        }

        public EditCategory(int categoryId)
        {
            InitializeComponent();

            MyDbContext db = new MyDbContext();
            category = db.Categories.Find(categoryId);

            categoryTextBox.Text = category.name;
        }

        private void CloseEditProductWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!validateData())
                return;

            MyDbContext db = new MyDbContext();
            db.Categories.AddOrUpdate(category);
            db.SaveChanges();

            this.DialogResult = true;
        }

        private bool validateData()
        {
            string errorMessage = "";
            if (categoryTextBox.Text == "")
            {
                errorMessage += "Nie podano nazwy kategorii.\n";
            }

            if (errorMessage != "")
            {
                MessageBox.Show(errorMessage, "Błąd Rejestracji", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
                return true;
        }
    }
}
