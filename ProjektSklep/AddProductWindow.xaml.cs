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
        List<Category> categories = new List<Category>();
        BitmapImage bitmapImage = null;

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
            string name = NameTextBox.Text;
            string priceText = PriceTextBox.Text;
            string desc = DescriptionTextBox.Text;
            int categoryId = (int)CategoryComboBox.SelectedValue;
            int stockLevel = int.Parse(StockTextBox.Text);
            int actualState = int.Parse(ActualStockTextBox.Text);

            try
            {
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

                MessageBox.Show("Produkt został dodany pomyślnie.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (FormatException)
            {
                MessageBox.Show("Cena musi być wartością liczbową.", "Nieprawidłowa wartość ceny.", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
