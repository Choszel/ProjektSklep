using System.Diagnostics;
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
        Dictionary<int, int> cart;

        private MyDbContext db = new MyDbContext();

        public MainWindow()
        {
            InitializeComponent();

            productListBox.ItemsSource = db.Products.ToList();
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
            products = db.Products.ToList();
        }

        private void InitializeCategories()
        {
            categories = db.Categories.ToList();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if(loginButton.Content.ToString().CompareTo("Zaloguj Się")==0)
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Owner = this;
                loginWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                if (loginWindow.ShowDialog() == true)loginButton.Content = "Wyloguj Się";       
            }
            else
            {
                loginButton.Content = "Zaloguj Się";
            }           
        }

        //Wyszukiwanie produktów po nazwie i kategorii
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        //    if (productName != null)
        //    {
        //        productName.Content = "Szukanie\n";
        //    }

        //    string? chosenCategory = "brak";
        //    int chosenCategoryId = 0;

        //    if (categoriesComboBox != null)
        //    {
        //        chosenCategory = categoriesComboBox.SelectedItem.ToString();
        //    }

        //    foreach (Category category in categories)
        //    {
        //        if (chosenCategory == "Wszystko")
        //        {
        //            break;
        //        }
        //        if (category.name == chosenCategory)
        //        {
        //            chosenCategoryId = category.categoryId;
        //            break;
        //        }
        //    }

        //    foreach (Product product in products)
        //    {
        //        if (product.name.ToLower().Contains(searchTextBox.Text.ToLower()) && (chosenCategoryId == 0 || product.categoryId == chosenCategoryId))
        //        {
        //            //debugowe wypisywanie, do zamiany na to, co ma wyświetlać produkty
        //            if (productName is not null)
        //            {
        //                productName.Content += "Twoja mama lubi: " + product.name + chosenCategoryId + "\n";
        //            }
        //        }
        //    }
        }

        private void productScroll(object sender, ScrollChangedEventArgs e)
        {
            // Przykładowa obsługa zdarzenia przewijania
            // Możesz zrobić coś przydatnego z wartościami e.VerticalOffset, e.HorizontalOffset itp.

            // Przykładowe użycie:
            if (e.VerticalChange > 0)
            {
                Console.WriteLine("Przewijanie w dół");
            }
            else if (e.VerticalChange < 0)
            {
                Console.WriteLine("Przewijanie w górę");
            }
        }

        private void productListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox)
            {
                foreach (Product product in listBox.Items)
                {
                    // Pobierz ListBoxItem, który odpowiada wybranemu elementowi
                    ListBoxItem listBoxItem = (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromItem(product);

                    // Znajdź element productDesc wewnątrz ListBoxItem
                    Label productDescLabel = FindVisualChild<Label>(listBoxItem, "productDesc");

                    // Jeśli znaleziono productDesc, zmień jego widoczność
                    if(product == listBox.SelectedItem)
                    {
                        productDescLabel.Visibility = Visibility.Visible; // lub inna wartość Visibility
                    }
                    else if (productDescLabel != null)
                    {
                        productDescLabel.Visibility = Visibility.Hidden; // lub inna wartość Visibility
                    }
                    
                }
            }
        }


        private T FindVisualChild<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is FrameworkElement frameworkElement && frameworkElement.Name == name)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child, name);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        private void addToCart(object sender, RoutedEventArgs e)
        {
            if (cart == null)
                cart = new Dictionary<int, int>();

            if (sender is Button button)
            {
                // Pobierz identyfikator produktu z taga przycisku
                if (button.Tag != null && int.TryParse(button.Tag.ToString(), out int productId))
                {
                    // Dodaj produkt do koszyka lub zwiększ jego liczbę w koszyku
                    if (cart.ContainsKey(productId))
                    {
                        cart[productId]++;
                    }
                    else
                    {
                        cart.Add(productId, 1);
                    }

                    foreach (var item in cart)
                    {
                        Debug.WriteLine($"Produkt ID: {item.Key}, Ilość: {item.Value}");
                    }
                }
            }
        }

        private void wheelButton_Click(object sender, RoutedEventArgs e)
        {
            WheelWindow wheelWindow = new WheelWindow();

            wheelWindow.Owner = this;
            wheelWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            this.Opacity = 0.4;

            wheelWindow.Closed += (s, args) =>
            {
                this.Opacity = 1;
            };

            if (wheelWindow.ShowDialog() == true) ;
        }
    }
}