using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
        List<Order> orders = new List<Order>();
        List<Warehouse> warehouse_list = new List<Warehouse>();
        List<CartProduct> cart = new List<CartProduct>();

        private MyDbContext db = new MyDbContext();

        public MainWindow()
        {
            InitializeComponent();

            InitializeDBData();
            productListBox.ItemsSource = db.Products.ToList();

            List<Order> orders = db.Orders.ToList();

            foreach (Order order in orders)
            {
                foreach(ProductOrder productOrder in order.ProductOrder)
                {
                    order.products += productOrder.product.name + " ";
                }
            }

            orderListBox.ItemsSource = db.Orders.ToList();
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

            foreach (Product product in products)
            {
                BitmapImage bitmapImage = new BitmapImage();
                Images imageDB = new Images();

                if ((imageDB = db.Images
                        .FirstOrDefault(
                    i => i.imageId == product.imageId)) == null)
                {
                    MessageBox.Show("Brak obrazków");
                }
                else
                {
                    using (var mem = new MemoryStream(imageDB.image))
                    {
                        mem.Position = 0;
                        bitmapImage.BeginInit();
                        bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.UriSource = null;
                        bitmapImage.StreamSource = mem;
                        bitmapImage.EndInit();
                    }
                    bitmapImage.Freeze();
                }
            
                product.bitmapImage = bitmapImage;
            }
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

                if (loginWindow.ShowDialog() == true)
                {
                    loginButton.Content = "Wyloguj Się";
                    if (UserType.Instance.numericType == 0) {
                        productsTab.Visibility = Visibility.Visible; 
                        productsTab.IsEnabled = true; 
                        ordersTab.Visibility = Visibility.Visible;
                        ordersTab.IsEnabled = true;
                        warehouseTab.Visibility = Visibility.Visible;
                        warehouseTab.IsEnabled = true;
                        wheelButton.Visibility = Visibility.Hidden;
                        wheelButton.IsEnabled = false;
                        if (!isSliderHidden) MoveBasketPanel(this, e);
                        ShowBasketButton.Content = "+";

                       
                        mainTabs.BorderBrush = new SolidColorBrush(Colors.Black); 
                    }
                }
            }
            else
            {
                UserType.Instance.numericType = -1;

                productsTab.Visibility = Visibility.Hidden;
                productsTab.IsEnabled = false;
                ordersTab.Visibility = Visibility.Hidden;
                ordersTab.IsEnabled = false;
                warehouseTab.Visibility = Visibility.Hidden;
                warehouseTab.IsEnabled = false;
                wheelButton.Visibility = Visibility.Visible;
                wheelButton.IsEnabled = true;
                ShowBasketButton.Content = "<";

                mainTabs.SelectedIndex = 0;

                loginButton.Content = "Zaloguj Się";
                UserType.Instance.numericType = -1;

            }

            SelectionChangedEventArgs args = new SelectionChangedEventArgs(
                Selector.SelectionChangedEvent,
                removedItems: new List<Product>(),
                addedItems: new List<Product>()
            );
            productListSelectionChanged(productListBox, args);

        }

        //Wyszukiwanie produktów po nazwie i kategorii
        private void searchBoxChanged(object sender, TextChangedEventArgs e)
        {
            productFilters();
        }

        private void productFilters()
        {
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
                ListBoxItem listBoxItem = (ListBoxItem)productListBox.ItemContainerGenerator.ContainerFromItem(product);
                if(listBoxItem != null)
                {
                    if (product.name.ToLower().Contains(searchTextBox.Text.ToLower()) && (chosenCategoryId == 0 || product.categoryId == chosenCategoryId))
                    {
                        listBoxItem.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        listBoxItem.Visibility = Visibility.Collapsed;
                    }
                }
            }
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

                    Button addButton = FindVisualChild<Button>(listBoxItem, "addProductButton");
                    Button editButton = FindVisualChild<Button>(listBoxItem, "editProductButton");
                    Button deleteButton = FindVisualChild<Button>(listBoxItem, "deleteProductButton");

                    // Jeśli znaleziono productDesc, zmień jego widoczność
                    if (product == listBox.SelectedItem)
                    {
                        productDescLabel.Visibility = Visibility.Visible; // lub inna wartość Visibility
                    }
                    else if (productDescLabel != null)
                    {
                        productDescLabel.Visibility = Visibility.Hidden; // lub inna wartość Visibility
                    }

                    if(UserType.Instance.numericType == 0)
                    {
                        addButton.Visibility = Visibility.Hidden;
                        addButton.IsEnabled = false;
                        editButton.Visibility = Visibility.Visible;
                        editButton.IsEnabled = true;
                        deleteButton.Visibility = Visibility.Visible;
                        deleteButton.IsEnabled = true;
                    }
                    else
                    {
                        addButton.Visibility = Visibility.Visible;
                        addButton.IsEnabled = true;
                        editButton.Visibility = Visibility.Hidden;
                        editButton.IsEnabled = false;
                        deleteButton.Visibility = Visibility.Hidden;
                        deleteButton.IsEnabled = false;

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
            if(UserType.Instance.numericType == -1)
            {
                loginButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                return;
            }

            if (cart == null)
                cart = new List<CartProduct>();

            if (sender is Button button)
            {
                // Pobierz identyfikator produktu z taga przycisku
                if (button.Tag != null && int.TryParse(button.Tag.ToString(), out int productId))
                {
                    // Dodaj produkt do koszyka lub zwiększ jego liczbę w koszyku
                    if (cart.Find(item => item.id == productId) != null)
                    {
                        CartProduct cartProduct = cart.Find(item => item.id == productId);
                        cartProduct.count++;
                    }
                    else
                    {
                        Product product = products.FindLast(item => item.productId == productId);

                        CartProduct cartProduct = new CartProduct(productId,product.name, 1, product.price);
                        cart.Add(cartProduct);
                        basketListBox.Items.Add(cartProduct);
                    }

                    float wholePriceSum = 0;
                    foreach (var item in cart)
                    {
                        wholePriceSum += item.singlePrice*item.count;
                    }

                    wholePrice.Content = wholePriceSum + " PLN";

                    foreach (var item in cart)
                    {
                        Debug.WriteLine($"Produkt ID: {item.id}, Ilość: {item.count}");
                    }
                }
            }
        }

        private void editProduct(object sender, RoutedEventArgs e)
        {
            Button editButton = sender as Button;

            if (editButton.Tag != null && int.TryParse(editButton.Tag.ToString(), out int productId))
            {

                MyDbContext dbContext = new MyDbContext();

                Product product = products.First(item => item.productId == productId);

                EditProductWindow productEditWindow = new EditProductWindow(product,categories);

                if(productEditWindow.ShowDialog() == true)
                {
                    MessageBox.Show("Edytowano produkt: " + product.name);
                };
            }
        }

        private void editOrderButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void editInWarehouse_Click(object sender, RoutedEventArgs e)
        {

        }
        
        private void deleteInWarehouse_Click(object sender, RoutedEventArgs e)
        {

        }

        private void deleteProduct(object sender, RoutedEventArgs e)
        {
            Button deleteButton = sender as Button;

            if(deleteButton.Tag != null && int.TryParse(deleteButton.Tag.ToString(), out int productId))
            {
                MyDbContext dbContext = new MyDbContext();

                Product product = dbContext.Products.Find(productId);

                var result = MessageBox.Show("Czy jesteś pewny że chcesz usunąć " + product.name + "?", "Potwierdzenie usunięcia", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        dbContext.Products.Remove(product);
                    }
                    catch(Exception error)
                    {
                        Debug.WriteLine(error);
                    }

                    Debug.WriteLine("Usunieto produktid: " + productId);
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

        private bool isSliderHidden = true;
        private void MoveBasketPanel(object sender, RoutedEventArgs e)
        {
            if(ShowBasketButton.Content.Equals(">") || ShowBasketButton.Content.Equals("<"))
            {
                double targetX = isSliderHidden ? -167 : 0;
                DoubleAnimation animation = new DoubleAnimation(targetX, TimeSpan.FromSeconds(0.5));
                sliderTransform.BeginAnimation(TranslateTransform.XProperty, animation);
                buttonTransform.BeginAnimation(TranslateTransform.XProperty, animation);

                // Zmiana kierunku strzałki w zależności od sliderHidden
                ShowBasketButton.Content = isSliderHidden ? ">" : "<";

                isSliderHidden = !isSliderHidden;
            }
            else
            {
                AddProductWindow productWindow = new AddProductWindow();
                productWindow.Owner = this;
                productWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                this.Opacity = 0.4;

                productWindow.Closed += (s, args) =>
                {
                    this.Opacity = 1;
                };
                productWindow.Show();
            }          
        }

        private void PlaceOrderButton_Click(object sender, RoutedEventArgs e)
        {
            ShippingDetailsWindow shippingDetailsWindow = new ShippingDetailsWindow();
            shippingDetailsWindow.Owner = this;
            shippingDetailsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            this.Opacity = 0.4;

            shippingDetailsWindow.Closed += (s, args) =>
            {
                this.Opacity = 1;
            };

            shippingDetailsWindow.ShowDialog();
        }

        private void mainTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem tabItem = mainTabs.SelectedItem as TabItem;          

            if(tabItem != null && tabItem.Name == "productsTab") 
            {
                orders = new List<Order>();
                warehouse_list = new List<Warehouse>();
                InitializeProducts();
                ShowBasketButton.IsEnabled = true;
            }
            else if(tabItem != null && tabItem.Name == "ordersTab")
            {
                products = new List<Product>();
                warehouse_list = new List<Warehouse>();
                orderListBox.ItemsSource = db.Orders.ToList();
                ShowBasketButton.IsEnabled = false;
            }
            else if(tabItem != null && tabItem.Name == "warehouseTab")
            {
                products = new List<Product>();
                orders = new List<Order>();
                warehouseListBox.ItemsSource = db.Warehouse.ToList();
                ShowBasketButton.IsEnabled = false;
            }

        }

        private void orderListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void warehouseListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void categoriesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            productFilters();
        }
    }
}