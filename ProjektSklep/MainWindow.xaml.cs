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

        public MainWindow()
        {
            InitializeComponent();
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

            //Pętla inicjulizująca kategorie w comboboxie
            foreach (Category category in categories)
            {
                CategoriesBox.Items.Add(category.name);
            }
        }

   
    }
}