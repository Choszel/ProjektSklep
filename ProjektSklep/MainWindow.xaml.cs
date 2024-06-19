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
using System.Printing;
using System.Windows.Xps.Packaging;
using System.Linq;
using System.Collections;
using System.Reflection;
using System.Windows.Threading;
using System.Xml.Linq;
using System.DirectoryServices;
using Notification.Wpf;
using System;

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
        Chart chart = new Chart();
        public static DispatcherTimer wheelTimer = new DispatcherTimer();
        NotificationManager notificationManager = new NotificationManager();

        private MyDbContext db = new MyDbContext();

        public MainWindow()
        {
            InitializeComponent();

            InitializeDBData();

            List<Order> orders = db.Orders.ToList();

            foreach (Order order in orders)
            {
                foreach (ProductOrder productOrder in order.ProductOrder)
                {
                    order.products += productOrder.product.name + " ";
                }
            }

            orderListBox.ItemsSource = db.Orders.ToList();
        }

        private void InitializeDBData()
        {
            InitializeProducts();
            InitializeCategories();
        }

        private void InitializeProducts()
        {
            db = new MyDbContext();
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
            productListBox.ItemsSource = db.Products.ToList();
        }

        private async Task ShowToast(string title, string message, NotificationType type)
        {
            await Task.Run(() =>
            {
                //Thread.Sleep(1_000);
                notificationManager.Show(title, message, type, onClick: () => MoveBasketPanel(this, new RoutedEventArgs()));
            });
        }

        private void InitializeCategories()
        {
            db = new MyDbContext();
            categoriesComboBox.SelectionChanged -= categoriesComboBox_SelectionChanged;
            categories = db.Categories.ToList();

            Category wszystkie = categories.Find(category => category.name == "Wszystko");


            if(wszystkie == null)
            {
                wszystkie = new Category();
                wszystkie.name = "Wszystko";
                db.Categories.Add(wszystkie);
                db.SaveChanges();

                categories = db.Categories.ToList();
                wszystkie = categories.Find(category => category.name == "Wszystko");
            }

            int wszystkieIndex= categories.IndexOf(wszystkie);

            categories[wszystkieIndex] = categories[0];
            categories[0] = wszystkie;

            categoriesListBox.ItemsSource = categories;
            categoriesComboBox.ItemsSource = categories;
            categoriesComboBox.SelectedItem = db.Categories.Find(4);
            categoriesComboBox.SelectionChanged += categoriesComboBox_SelectionChanged;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (UserType.Instance.loggedId != -1)
            {
                var user = db.Users.FirstOrDefault(u => u.userId == UserType.Instance.loggedId);
                if (user != null)
                {
                    TimeSpan timeLeft = TimeSpan.FromHours(24) + (user.lastSpin - DateTime.Now) ?? TimeSpan.FromSeconds(0);
                    if (timeLeft > TimeSpan.FromSeconds(0)) wheelTimerText.Text = "Koło fortuny\nDostępne za:\n" + timeLeft.ToString().Substring(0, 8);
                    else
                    {
                        wheelTimerText.Text = "Koło fortuny\nJuż dostępne!";
                        wheelButton.IsEnabled = true;
                        wheelTimer.Stop();
                        AskAboutWheel();
                    }
                }
            }

        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (loginButton.Content.ToString().CompareTo("Zaloguj Się") == 0)
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Owner = this;
                loginWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                if (loginWindow.ShowDialog() == true)
                {
                    loginButton.Content = "Wyloguj Się";
                    wheelButton.IsEnabled = false;
                    if (UserType.Instance.numericType == 0)
                    {
                        productsTab.Visibility = Visibility.Visible;
                        productsTab.IsEnabled = true;
                        categoriesTab.Visibility = Visibility.Visible;
                        categoriesTab.IsEnabled = true;
                        ordersTab.Visibility = Visibility.Visible;
                        ordersTab.IsEnabled = true;
                        warehouseTab.Visibility = Visibility.Visible;
                        warehouseTab.IsEnabled = true;
                        chartTab.Visibility = Visibility.Visible;
                        chartTab.IsEnabled = true;
                        printTab.Visibility = Visibility.Visible;
                        printTab.IsEnabled = true;
                        wheelButton.Visibility = Visibility.Hidden;
                        //wheelButton.IsEnabled = false;
                        if (!isSliderHidden) SpecialMoveBasketPanel(this, e);
                        ShowBasketButton.Content = "+";

                       
                        mainTabs.BorderBrush = new SolidColorBrush(Colors.Black);
                    }
                    else
                    {
                        wheelTimer.Interval = TimeSpan.FromSeconds(1);
                        wheelTimer.Tick += Timer_Tick;
                        wheelTimer.Start();
                    }
                }
            }
            else
            {
                if (!isSliderHidden) SpecialMoveBasketPanel(this, e);
                UserType.Instance.numericType = -1;
                UserType.Instance.loggedId = -1;

                productsTab.Visibility = Visibility.Hidden;
                productsTab.IsEnabled = false;
                categoriesTab.Visibility = Visibility.Hidden;
                categoriesTab.IsEnabled = false;
                ordersTab.Visibility = Visibility.Hidden;
                ordersTab.IsEnabled = false;
                warehouseTab.Visibility = Visibility.Hidden;
                warehouseTab.IsEnabled = false;
                chartTab.Visibility = Visibility.Hidden;
                chartTab.IsEnabled = false;
                printTab.Visibility = Visibility.Hidden;
                printTab.IsEnabled = false;
                wheelButton.Visibility = Visibility.Visible;
                wheelButton.IsEnabled = true;
                ShowBasketButton.Content = "<";

                mainTabs.SelectedIndex = 0;
                wheelTimer.Stop();
                wheelTimerText.Text = "Koło fortuny";
                loginButton.Content = "Zaloguj Się";
            }

            SelectionChangedEventArgs args = new SelectionChangedEventArgs(
                Selector.SelectionChangedEvent,
                removedItems: new List<Product>(),
                addedItems: new List<Product>()
            );
            productListSelectionChanged(productListBox, args);

        }

        private async Task AskAboutWheel()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(1000);
                MessageBox.Show("Pamiętaj o co 24 godzinnym kręceniu kołem fortuny!");
            });
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
                Category category = categoriesComboBox.SelectedItem as Category;
                chosenCategory = category.name;
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
                if (listBoxItem != null)
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

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedOption = comboBox.SelectedItem as ComboBoxItem;
            var sortOption = (SortOptions)Enum.Parse(typeof(SortOptions), selectedOption.Tag.ToString());
            SortProducts(sortOption);
        }

        private void SortProducts(SortOptions option)
        {
            switch (option)
            {
                case SortOptions.NewestToLatest:
                    products = products.OrderByDescending(p => p.productId).ToList();
                    break;
                case SortOptions.LatestToNewest:
                    products = products.OrderBy(p => p.productId).ToList();
                    break;
                case SortOptions.LeastExpensiveToMostExpensive:
                    products = products.OrderBy(p => p.price).ToList();
                    break;
                case SortOptions.MostExpensiveToLeastExpensive:
                    products = products.OrderByDescending(p => p.price).ToList();
                    break;
                case SortOptions.AlphabeticallyFirstToLast:
                    products = products.OrderBy(p => p.name).ToList();
                    break;
                case SortOptions.AlphabeticallyLastToFirst:
                    products = products.OrderByDescending(p => p.name).ToList();
                    break;
            }

            if (productListBox is not null)
            {
                productListBox.ItemsSource = null;
                productListBox.ItemsSource = products;
            }
        }
        public enum SortOptions
        {
            NewestToLatest,
            LatestToNewest,
            LeastExpensiveToMostExpensive,
            MostExpensiveToLeastExpensive,
            AlphabeticallyFirstToLast,
            AlphabeticallyLastToFirst
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
                    Grid grid = new Grid();
                    // Jeśli znaleziono productDesc, zmień jego widoczność
                    if (product == listBox.SelectedItem)
                    {
                        productDescLabel.Visibility = Visibility.Visible; // lub inna wartość Visibility
                    }
                    else if (productDescLabel != null)
                    {
                        productDescLabel.Visibility = Visibility.Hidden; // lub inna wartość Visibility
                    }

                    if (UserType.Instance.numericType == 0)
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
            if (UserType.Instance.numericType == -1)
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
                    Warehouse warehouse = db.Warehouse.First(warehouse => warehouse.productId == productId);
                    CartProduct cartProduct = null;

                    if (cart.FirstOrDefault(cp => cp.id == productId) != null)
                    {
                        cartProduct = cart.FirstOrDefault(cp => cp.id == productId);
                        if (warehouse.actualState < cartProduct.count + 1)
                        {
                            MessageBox.Show("Niestety nie posiadamy już tego produktu na stanie");
                            return;
                        }
                    }
                    else
                    {
                        if (warehouse.actualState < 1)
                        {
                            MessageBox.Show("Niestety nie posiadamy już tego produktu na stanie");
                            return;
                        }
                    }

                    // Dodaj produkt do koszyka lub zwiększ jego liczbę w koszyku
                    if (cart.Find(item => item.id == productId) != null)
                    {
                        cartProduct = cart.Find(item => item.id == productId);
                        cartProduct.count++;
                    }
                    else
                    {
                        Product product = products.FindLast(item => item.productId == productId);
                        float productPrice = product.price;
                        User currentUser = db.Users.FirstOrDefault(u => u.userId == UserType.Instance.loggedId);
                        if (currentUser != null)
                        {
                            if (((DateTime.Now - (currentUser.lastSpin ?? DateTime.Now))).Hours < 24)
                            {
                                if (currentUser.currDiscount != null)
                                {
                                    var discount = currentUser.currDiscount.Split("|");
                                    if (product.categoryId == int.Parse(discount[2])) productPrice -= (productPrice * int.Parse(discount[0])) / 100;
                                }
                            }
                        }
                        CartProduct addedCartProduct = new CartProduct(productId, product.name, 1, productPrice);
                        cart.Add(addedCartProduct);
                        basketListBox.Items.Add(addedCartProduct);
                    }

                    countSum();

                    foreach (var item in cart)
                    {
                        Debug.WriteLine($"Produkt ID: {item.id}, Ilość: {item.count}");
                    }
                    ShowToast("Koszyk", "Pomyślnie dodano produkt do koszyka!", NotificationType.Success);
                }
                else ShowToast("Koszyk", "Wystąpił nieoczekiwany błąd", NotificationType.Error);
            }
            else ShowToast("Koszyk", "Wystąpił nieoczekiwany błąd", NotificationType.Error);

        }

        private void countSum()
        {
            float wholePriceSum = 0;
            foreach (var item in cart)
            {
                wholePriceSum += item.singlePrice * item.count;
            }

            wholePrice.Content = wholePriceSum + " PLN";
        }

        private void editProduct(object sender, RoutedEventArgs e)
        {
            Button editButton = sender as Button;

            if (editButton.Tag != null && int.TryParse(editButton.Tag.ToString(), out int productId))
            {

                MyDbContext dbContext = new MyDbContext();

                Product product = products.First(item => item.productId == productId);

                EditProductWindow productEditWindow = new EditProductWindow(product, categories);

                if (productEditWindow.ShowDialog() == true)
                {
                    MessageBox.Show("Edytowano produkt: " + product.name);
                };
            }
        }

        private void editOrderButton_Click(object sender, RoutedEventArgs e)
        {
            Button editButton = sender as Button;

            if (editButton.Tag != null && int.TryParse(editButton.Tag.ToString(), out int orderID))
            {
                Order order = db.Orders.Find(orderID);
                EditOrderState editOrderState = new EditOrderState(order);
                editOrderState.ShowDialog();
            }
        }

        private void editInWarehouse_Click(object sender, RoutedEventArgs e)
        {
            Button editButton = sender as Button;

            if (editButton.Tag != null && int.TryParse(editButton.Tag.ToString(), out int warehouseProductId))
            {
                Warehouse wh = db.Warehouse.Find(warehouseProductId);
                EditProductsInWarehouse editProductsInWarehouse = new EditProductsInWarehouse(wh);
                editProductsInWarehouse.ShowDialog();
            }
        }
        private void deleteInWarehouse_Click(object sender, RoutedEventArgs e)
        {

        }

        private void deleteProduct(object sender, RoutedEventArgs e)
        {
            Button deleteButton = sender as Button;

            if (deleteButton.Tag != null && int.TryParse(deleteButton.Tag.ToString(), out int productId))
            {
                MyDbContext dbContext = new MyDbContext();

                Product product = dbContext.Products.Find(productId);

                var result = MessageBox.Show("Czy jesteś pewny że chcesz usunąć " + product.name + "?", "Potwierdzenie usunięcia", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        dbContext.Products.Remove(product);
                        dbContext.SaveChanges();
                        InitializeProducts();
                    }
                    catch (Exception error)
                    {
                        Debug.WriteLine(error);
                    }

                    Debug.WriteLine("Usunieto produktid: " + productId);
                }
            }
        }

        private void wheelButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserType.Instance.numericType == -1)
            {
                loginButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                return;
            }

            WheelWindow wheelWindow = new WheelWindow();

            wheelWindow.Owner = this;
            wheelWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            this.Opacity = 0.4;

            wheelWindow.Closed += (s, args) =>
            {
                this.Opacity = 1;
            };

            if (wheelWindow.ShowDialog() == true) ;
            wheelButton.IsEnabled = !wheelTimer.IsEnabled;
        }

        private bool isSliderHidden = true;

        private void MoveBasketPanel(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = mainTabs.SelectedItem as TabItem;
            if (tabItem.Name == "categoriesTab")
            {
                addCategory(sender, e);
            }
            else
                MoveBasketPanel(sender, e, false);
        }

        private void SpecialMoveBasketPanel(object sender, RoutedEventArgs e)
        {
            cart.Clear();
            basketListBox.Items.Clear();
            //countSum();
            MoveBasketPanel(sender, e, true);
        }

        private void MoveBasketPanel(object sender, RoutedEventArgs e, bool special)
        {
            if (ShowBasketButton.Content.Equals(">") || ShowBasketButton.Content.Equals("<"))
            {
                double targetX = isSliderHidden ? -167 : 0;
                DoubleAnimation animation;
                if (special)
                    animation = new DoubleAnimation(targetX, TimeSpan.FromSeconds(0));
                else
                    animation = new DoubleAnimation(targetX, TimeSpan.FromSeconds(0.5));
                buttonTransform.BeginAnimation(TranslateTransform.XProperty, animation);
                sliderTransform.BeginAnimation(TranslateTransform.XProperty, animation);

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
                    InitializeProducts();
                };
                productWindow.Show();
            }
        }

        private void PlaceOrderButton_Click(object sender, RoutedEventArgs e)
        {
            ShippingDetailsWindow shippingDetailsWindow = new ShippingDetailsWindow(cart);
            shippingDetailsWindow.Owner = this;
            shippingDetailsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            this.Opacity = 0.4;

            shippingDetailsWindow.Closed += (s, args) =>
            {
                this.Opacity = 1;
            };

            if (shippingDetailsWindow.ShowDialog() == true)
            {
                cart.Clear();
                basketListBox.Items.Clear();
                //countSum();
            }
        }

        private bool _isHandlingSelectionChanged = false;

        private void mainTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isHandlingSelectionChanged) return;
            _isHandlingSelectionChanged = true;

            try
            {
                TabItem tabItem = mainTabs.SelectedItem as TabItem;

                if (tabItem != null && tabItem.Name == "productsTab")
                {
                    orders = new List<Order>();
                    warehouse_list = new List<Warehouse>();
                    ShowBasketButton.IsEnabled = true;
                    Debug.WriteLine("productsTab");
                }
                else if (tabItem != null && tabItem.Name == "categoriesTab") 
                {
                    db = new MyDbContext();
                    orders = new List<Order>();
                    warehouse_list = new List<Warehouse>();
                    categoriesListBox.ItemsSource = db.Categories.ToList();
                    ShowBasketButton.IsEnabled = true;
                    Debug.WriteLine("categoriesTab");
                }
                else if (tabItem != null && tabItem.Name == "ordersTab")
                {
                    warehouse_list = new List<Warehouse>();
                    orderListBox.ItemsSource = db.Orders.ToList();
                    ShowBasketButton.IsEnabled = false;
                    Debug.WriteLine("ordersTab");
                }
                else if (tabItem != null && tabItem.Name == "warehouseTab")
                {
                    db = new MyDbContext();
                    orders = new List<Order>();
                    warehouseListBox.ItemsSource = db.Warehouse.ToList();
                    ShowBasketButton.IsEnabled = false;
                    Debug.WriteLine("warehouseTab");
                }
                else if (tabItem != null && tabItem.Name == "chartTab")
                {
                    printedItems.Children.Remove(chart);
                    orders = new List<Order>();
                    warehouse_list = new List<Warehouse>();
                    ShowBasketButton.IsEnabled = false;
                    chart.Margin = new System.Windows.Thickness(0, 10, 0, 0);

                    if ((chartFirstValue.Items.Count - 1 != products.Count && chartFirstValue.Items.Count - 2 != products.Count))
                    {
                        chartGrid.Children.Add(chart);
                        Grid.SetRow(chart, 1);
                        Grid.SetColumnSpan(chart, 2);
                        chartFirstValue.Items.Clear();
                        chartFirstValue.Items.Add("Wszystko");
                        foreach (Product product in products) chartFirstValue.Items.Add(product.name);

                        Debug.WriteLine("chartTab if");

                    }
                    Debug.WriteLine("chartTab");
                }
                else if (tabItem != null && tabItem.Name == "printTab")
                {
                    chartGrid.Children.Remove(chart);
                    chartFirstValue.Items.Clear();
                    orders = new List<Order>();
                    warehouse_list = new List<Warehouse>();

                    List<System.Reflection.PropertyInfo> tables = db.PrintAllTables();
                    if (selectTablePrint.Items.Count != tables.Count()) foreach (var table in tables) selectTablePrint.Items.Add(table.Name);
                }
            }
            finally
            {
                _isHandlingSelectionChanged = false;
            }
        }

        private void categoriesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            productFilters();
        }

        private void selectFirstChartValue(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                DateTime monthAgo = DateTime.Now.AddDays(-30);
                if (comboBox.SelectedValue != null)
                {
                    if (comboBox.SelectedValue.ToString() == "Wszystko")
                    {
                        List<ProductOrder> productOrder = db.ProductOrders.Where(p => p.order.orderDate >= monthAgo).Include(d => d.order).Include(e => e.product).ToList();
                        chart.generateFirstChart(productOrder, "Data zakupu", "Ilość");
                    }
                    else
                    {
                        List<ProductOrder> productOrder = db.ProductOrders.Where(p => p.order.orderDate >= monthAgo).Where(p => p.product.name == comboBox.SelectedValue.ToString()).Include(d => d.order).Include(e => e.product).ToList();
                        chart.generateFirstChart(productOrder, "Data zakupu", "Ilość");
                    }
                }
            }

        }

        private void createPrintFile(object sender, RoutedEventArgs e)
        {
            bool hidePrintDialog = false;
            PrintDialog printDialog = new PrintDialog();

            if (!hidePrintDialog)
            {
                // Display the dialog. This returns true if the user presses the Print button.
                bool? isPrinted = printDialog.ShowDialog();
                if (isPrinted != true)
                    return;
            }

            // Prepare the document for printing
            try
            {
                //FlowDocument printFlowDocument = new FlowDocument();
                //foreach (var item in printedItems.Children)
                //{
                //    if (item is DataGrid dataGrid)
                //    {
                //        // Convert DataGrid to Table and add to FlowDocument
                //        Table table = ConvertDataGridToTable(dataGrid);
                //        printFlowDocument.Blocks.Add(table);
                //    }
                //    else if (item is UIElement uiElement)
                //    {
                //        // Wrap UIElement in BlockUIContainer and add to FlowDocument

                //        BlockUIContainer blockUIContainer = new BlockUIContainer(uiElement);
                //        printFlowDocument.Blocks.Add(blockUIContainer);
                //    }
                //}
                printFlowDocument.PageWidth = 1000;
                printFlowDocument.PageHeight = 1000;
                //printFlowDocumentScrollViewer.Width = 2000;
                //printDataGrid.Width = 13000;

                IDocumentPaginatorSource idpSource = printFlowDocument;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Printing " + "WPF_Store_print");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private Table ConvertDataGridToTable(DataGrid dataGrid)
        {
            Table table = new Table();
            int numberOfColumns = dataGrid.Columns.Count;

            // Create columns
            for (int i = 0; i < numberOfColumns; i++)
            {
                table.Columns.Add(new TableColumn());
            }

            // Add header row
            TableRow headerRow = new TableRow();
            foreach (DataGridColumn column in dataGrid.Columns)
            {
                TableCell cell = new TableCell(new Paragraph(new Run(column.Header.ToString())));
                headerRow.Cells.Add(cell);
            }
            TableRowGroup headerRowGroup = new TableRowGroup();
            headerRowGroup.Rows.Add(headerRow);
            table.RowGroups.Add(headerRowGroup);

            // Add data rows
            foreach (var item in dataGrid.Items)
            {
                TableRow dataRow = new TableRow();
                foreach (DataGridColumn column in dataGrid.Columns)
                {
                    if (column is DataGridBoundColumn boundColumn)
                    {
                        Binding binding = (Binding)boundColumn.Binding;
                        string path = binding.Path.Path;
                        PropertyInfo property = item.GetType().GetProperty(path);
                        string cellValue = property?.GetValue(item)?.ToString() ?? string.Empty;
                        TableCell cell = new TableCell(new Paragraph(new Run(cellValue)));
                        dataRow.Cells.Add(cell);
                    }
                }
                TableRowGroup dataRowGroup = new TableRowGroup();
                dataRowGroup.Rows.Add(dataRow);
                table.RowGroups.Add(dataRowGroup);
            }

            return table;
        }


        private void selectTablePrint_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            printOptions.Children.Clear();
            printedItems.Children.Remove(chart);
            List<System.Reflection.PropertyInfo> tables = db.PrintAllTables();
            var entityType = tables.ElementAt(selectTablePrint.SelectedIndex).PropertyType.GetGenericArguments()[0];
            List<System.Reflection.PropertyInfo> attributes = db.printTableAttributes(entityType);

            foreach (System.Reflection.PropertyInfo obj in attributes)
            {
                CheckBox checkBox = new CheckBox
                {
                    Name = obj.Name,
                    Margin = new System.Windows.Thickness(5),
                    IsChecked = true,
                    Content = obj.Name,
                    FontSize = 12,
                };

                checkBox.Checked += printCheckBox_Checked;
                checkBox.Unchecked += printCheckBox_Unchecked;

                printOptions.Children.Add(checkBox);
            }

            string selectedTableName = selectTablePrint.SelectedItem.ToString();
            var tableData = GetTableData(selectedTableName);
            printDataGrid.ItemsSource = tableData;

            if (selectedTableName == "Orders")
            {
                CheckBox checkBox = new CheckBox
                {
                    Name = "chart",
                    Margin = new System.Windows.Thickness(5),
                    IsChecked = true,
                    Content = "Wykres",
                    FontSize = 12,
                };

                checkBox.Checked += printCheckBox_Checked;
                checkBox.Unchecked += printCheckBox_Unchecked;
                printDataGrid.Margin = new System.Windows.Thickness(0, 0, 0, 40);
                printOptions.Children.Add(checkBox);
                DateTime monthAgo = DateTime.Now.AddDays(-30);
                List<ProductOrder> productOrder = db.ProductOrders
                    .Where(p => p.order.orderDate >= monthAgo)
                    .Include(d => d.order)
                    .Include(e => e.product)
                    .ToList();
                chart.generateFirstChart(productOrder, "Data zakupu", "Ilość");
                printedItems.Children.Add(chart);
            }
            printedItems.Height = ActualHeight + 100;
            printFlowDocument.PageWidth = printedItems.ActualWidth;
            printFlowDocument.PageHeight = printedItems.ActualHeight + 100;
        }

        private IEnumerable GetTableData(string tableName)
        {
            var property = db.GetType().GetProperty(tableName);
            if (property != null)
            {
                var dbSet = property.GetValue(db) as IEnumerable;
                if (dbSet != null)
                {
                    var list = dbSet.Cast<object>().ToList();
                    return list;
                }
            }
            return null;
        }


        private List<DataGridColumn> removedColumns = new List<DataGridColumn>();

        private void printCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            string columnName = checkBox.Name;
            Debug.WriteLine($"{columnName} checked.");
            if (columnName == "chart")
            {
                printedItems.Children.Add(chart);
            }
            else
            {
                var column = removedColumns.FirstOrDefault(c => c.Header.ToString() == columnName);
                if (column != null)
                {
                    printDataGrid.Columns.Add(column);
                    removedColumns.Remove(column);
                }
            }
        }

        private void printCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            string columnName = checkBox.Name;
            Debug.WriteLine($"{columnName} unchecked.");
            if (columnName == "chart")
            {
                printedItems.Children.Remove(chart);
            }
            else
            {
                var column = printDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == columnName);
                if (column != null)
                {
                    printDataGrid.Columns.Remove(column);
                    removedColumns.Add(column);
                }
            }

        }

        private void addCategory(object sender, RoutedEventArgs e)
        {
            int categoryId = -1;

            EditCategory editCategory = new EditCategory(categoryId);
            if (editCategory.ShowDialog() == true)
            {
                db = new MyDbContext();
                InitializeCategories();
                InitializeProducts();
            }

        }

        private void editCategory(object sender, RoutedEventArgs e)
        {
            Button editButton = sender as Button;

            int categoryId = int.Parse(editButton.Tag.ToString());
            Category category = db.Categories.First(c => c.categoryId == categoryId);

            if(category.name =="Wszystko")
            {
                MessageBox.Show("Nie możesz edytować tej kategorii");
                return;
            }

            EditCategory editCategory = new EditCategory(categoryId);
            if(editCategory.ShowDialog() == true)
            {
                db = new MyDbContext();
                InitializeCategories();
                InitializeProducts();
            }

        }

        private void deleteCategory(object sender, RoutedEventArgs e)
        {
            Button editButton = sender as Button;

            int categoryId = int.Parse(editButton.Tag.ToString());
            Category category = db.Categories.First(c => c.categoryId == categoryId);

            if (category.name == "Wszystko")
            {
                MessageBox.Show("Nie możesz usunąć tej kategorii");
                return;
            }
            if (category != null)
            {
                if(MessageBox.Show("Właśnie usuwasz kategorię " + category.name +". Czy chcesz kontynuować?","Potwierdzenie usunięcia",MessageBoxButton.YesNo,MessageBoxImage.Warning)==MessageBoxResult.Yes)
                    db.Categories.Remove(category);
            }

                db.SaveChanges();
            InitializeCategories();
            InitializeProducts();
        }

        private void addProductCount(object sender, RoutedEventArgs e)
        {
            if(sender is Button senderButton)
            {
                cart.Find(p => p.id == int.Parse(senderButton.Tag.ToString())).count++;
                countSum();
            }
            foreach (var item in cart)
            {
                Debug.WriteLine($"Produkt ID: {item.id}, Ilość: {item.count}");
            }
        }

        private void subtractProductCount(object sender, RoutedEventArgs e)
        {
            if (sender is Button senderButton)
            {
                var cartProduct = cart.Find(p => p.id == int.Parse(senderButton.Tag.ToString()));

                if (cartProduct.count == 1)
                {
                    cart.Remove(cartProduct);
                    basketListBox.Items.Remove(cartProduct);
                }
                else cartProduct.count--;
                
                countSum();
            }
            foreach (var item in cart)
            {
                Debug.WriteLine($"Produkt ID: {item.id}, Ilość: {item.count}");
            }
        }
    }
}