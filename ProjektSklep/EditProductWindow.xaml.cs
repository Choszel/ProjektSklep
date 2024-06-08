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
        public EditProductWindow(Product product, List<Category> categories)
        {
            this.categories = categories;
            this.product = product;
            InitializeComponent();
            NameTextBox.Text = product.name;
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
                    // You can assign the selected file path to the Image object, for example:
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(openFileDialog.FileName);

                    // To save significant application memory, set the DecodePixelWidth or
                    // DecodePixelHeight of the BitmapImage value of the image source to the desired
                    // height or width of the rendered image. If you don't do this, the application will
                    // cache the image as though it were rendered as its normal size rather than just
                    // the size that is displayed.
                    // Note: In order to preserve aspect ratio, set DecodePixelWidth
                    // or DecodePixelHeight but not both.
                    bitmapImage.DecodePixelWidth = 200;
                    bitmapImage.EndInit();

                    product.bitmapImage = bitmapImage;
                    ImagePreview.Source = bitmapImage;
            }
        }
    }
}
