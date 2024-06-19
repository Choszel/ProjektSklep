using Microsoft.Win32;
using ProjektSklep.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.IO;
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
    /// Interaction logic for ProductEditWindow.xaml
    /// </summary>
    public partial class EditProductWindow : Window
    {
        Product product;
        List<Category> categories;
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

        public EditProductWindow(int productId, List<Category> categories)
        {
            MyDbContext myDbContext = new MyDbContext();
            product = myDbContext.Products.Find(productId);
            this.categories = categories;
            InitializeComponent();
            PriceTextBox.Text = product.price.ToString();
            DescriptionTextBox.Text = product.description.ToString();

            foreach (Category category in categories)
            {
                CategoryComboBox.Items.Add(category.name);
            }

            CategoryComboBox.SelectedItem = product.category.name;

            ImagePreview.Source = product.bitmapImage;
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (!isDataValid())
            {
                return;
            }

            product.name = NameTextBox.Text;
            product.price = float.Parse(PriceTextBox.Text);
            product.description = DescriptionTextBox.Text;
            product.category = categories.Find(category => category.name == CategoryComboBox.SelectedItem.ToString());
            product.categoryId = product.category.categoryId;

            MyDbContext db = new MyDbContext();

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

                Images placeImages = new Images();

                placeImages.image = data;

                product.image = placeImages;


                db.Images.Add(placeImages);

                product.imageId = placeImages.imageId;
            }

            db.Products.AddOrUpdate(product);
            db.SaveChanges();

            this.DialogResult = true;
        }

        private void CloseEditProductWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
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

                    product.bitmapImage = bitmapImage;
                    ImagePreview.Source = bitmapImage;
            }
        }

        private bool isDataValid()
        {
            string errorMessage = "";

            if(NameTextBox.Text == "")
            {
                errorMessage += "Nie podano nazwy.\n";
            }

            if(PriceTextBox.Text == "")
            {
                errorMessage += "Nie podano ceny.\n";
            }
            else
            if(!float.TryParse(PriceTextBox.Text, out float price))
            {
                errorMessage += "Cena musi być liczbą.\n";
            }
            else
            {
                if(price < 0)
                {
                    errorMessage += "Podano ujemną cenę.\n";
                }
            }
            if(DescriptionTextBox.Text == "")
            {
                errorMessage += "Nie podano opisu.\n";
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
