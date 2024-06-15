using Microsoft.Win32;
using ProjektSklep.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ProjektSklep
{
    public partial class AddProductWindow : Window
    {
        Product product = new Product();
        Warehouse warehouse = new Warehouse();
        List<Category> categories = new List<Category>();
        BitmapImage bitmapImage = null;

        public string InputName
        {
            get
            {
                return product.name;
            }
            set
            {
                product.name = value;
            }
        }

        public float InputPrice
        {
            get
            {
                return product.price;
            }
            set
            {
                product.price = value;
            }
        }

        public string InputDescription
        {
            get
            {
                return product.description;
            }
            set
            {
                product.description = value;
            }
        }

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
        public AddProductWindow()
        {
            InitializeComponent();
            LoadCategories();
        }

        private void LoadCategories()
        {
            using (var context = new MyDbContext())
            {
                categories = context.Categories.ToList();
                CategoryComboBox.ItemsSource = categories;
                CategoryComboBox.DisplayMemberPath = "name";
                CategoryComboBox.SelectedValuePath = "categoryId";
            }
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pliki obrazów|*.jpg;*.jpeg;*.png;*.gif;*.jfif|Wszystkie pliki|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(openFileDialog.FileName);
                bitmapImage.DecodePixelWidth = 200;
                bitmapImage.EndInit();
                ImagePreview.Source = bitmapImage;
            }
        }

        private void CloseAddProductWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (!isDataValid())
            {
                return;
            }

            string name = NameTextBox.Text;
            string priceText = PriceTextBox.Text;
            string desc = DescriptionTextBox.Text;
            int categoryId = 0;
            if (CategoryComboBox.SelectedValue != null)
            {
                categoryId = (int)CategoryComboBox.SelectedValue;
            }
            int stockLevel = int.Parse(StockTextBox.Text);
            int actualState = int.Parse(ActualStockTextBox.Text);

            float price = float.Parse(priceText);
            using (var context = new MyDbContext())
            {
                var product = new Product
                {
                    name = name,
                    price = price,
                    description = desc,
                    categoryId = categoryId,
                };

                if (bitmapImage != null)
                {
                    byte[] data;
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

                    using (MemoryStream ms = new MemoryStream())
                    {
                        encoder.Save(ms);
                        data = ms.ToArray();
                    }

                    Images image = new Images { image = data };
                    context.Images.Add(image);
                    context.SaveChanges();

                    product.imageId = image.imageId;
                }

                context.Products.Add(product);
                context.SaveChanges();

                var warehouse = new Warehouse
                {
                    productId = product.productId,
                    stockLevel = stockLevel,
                    actualState = actualState
                };

                context.Warehouse.Add(warehouse);
                context.SaveChanges();
            }

            MessageBox.Show("Produkt został dodany pomyślnie.", "Dodano produkt", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool isDataValid()
        {
            string errorMessage = "";

            if (NameTextBox.Text == "")
            {
                errorMessage += "Nie podano nazwy.\n";
            }

            if (PriceTextBox.Text == "")
            {
                errorMessage += "Nie podano ceny.\n";
            }
            else
            if (!float.TryParse(PriceTextBox.Text, out float price))
            {
                errorMessage += "Cena musi być liczbą.\n";
            }
            else
            {
                if (price < 0)
                {
                    errorMessage += "Podano ujemną cenę.\n";
                }
            }
            if (DescriptionTextBox.Text == "")
            {
                errorMessage += "Nie podano opisu.\n";
            }

            if (ActualStockTextBox.Text == "")
            {
                errorMessage += "Nie podano stanu aktualnego.\n";
            }
            else
            if (!int.TryParse(ActualStockTextBox.Text, out int actualState))
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

            if (StockTextBox.Text == "")
            {
                errorMessage += "Nie podano stanu magazynowego.\n";
            }
            else
                if (!int.TryParse(StockTextBox.Text, out int stockLevel))
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

            if(ImagePreview.Source == null)
            {
                errorMessage += "Nie podano zdjęcia produktu.\n";
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
