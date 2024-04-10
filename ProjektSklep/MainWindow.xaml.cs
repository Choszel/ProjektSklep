using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProjektSklep.Model;

namespace ProjektSklep
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Product> products = new List<Product>();
        List<Category> categories = new List<Category>();

        private MyDbContext db = new MyDbContext();

        public MainWindow()
        {
            InitializeComponent();
            /*
            products.Add(new Product("stringi",1));
            products.Add(new Product("kapelusze",1));
            products.Add(new Product("wiadra", 2));
            products.Add(new Product("marichuanen",2)); //XD
            products.Add(new Product("parówy",3));
            products.Add(new Product("pierogi",3));

            categories.Add(new Category(0, "All"));
            categories.Add(new Category(1,"ubrania"));
            categories.Add(new Category(2,"fun time"));
            categories.Add(new Category(3,"żarcie"));

            trzeba dać dane do DB*/
            InitializeDBData();


            categoriesComboBox.Items.Add("Wszystko");
            //Pętla inicjulizująca kategorie w comboboxie
            foreach (Category category in categories)
            {
                categoriesComboBox.Items.Add(category.name);
            }
        }

        private void InitializeDBData()
        {
            InitializeProducts();
            InitializeCategories();
        }

        private void InitializeProducts()
        {
            var query = db.Products.ToList();
            products = query;
        }

        private void InitializeCategories()
        {
            var query = db.Categories.ToList();
            categories = query;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.ShowDialog();
        }

        //Wyszukiwanie produktów po nazwie i kategorii
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchTest != null)
            {
                searchTest.Content = "Szukanie\n";
            }

            string? chosenCategory = "brak";
            int chosenCategoryId = 0;

            if (categoriesComboBox != null)
            {
                chosenCategory = categoriesComboBox.SelectedItem.ToString();
            }

            foreach (Category category in categories)
            {
                if (chosenCategory == "Wszystko")
                {
                    break;
                }
                if (category.name == chosenCategory)
                {
                    chosenCategoryId = category.categoryId;
                    break;
                }
            }

            foreach (Product product in products)
            {
                if (product.name.ToLower().Contains(searchTextBox.Text.ToLower()) && (chosenCategoryId == 0 || product.categoryId == chosenCategoryId))
                {
                    //debugowe wypisywanie, do zamiany na to, co ma wyświetlać produkty
                    if (searchTest is not null)
                    {
                        searchTest.Content += "Twoja mama lubi: " + product.name + chosenCategoryId + "\n";
                    }
                }
            }
        }


    }
}