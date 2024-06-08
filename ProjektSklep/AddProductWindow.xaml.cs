using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for AddProductWindow.xaml
    /// </summary>
    public partial class AddProductWindow : Window
    {
        public AddProductWindow()
        {
            InitializeComponent();
        }
        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pliki obrazów|*.jpg;*.jpeg;*.png;*.gif;*.jfif|Wszystkie pliki|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFileName = openFileDialog.FileName;
                // You can assign the selected file path to the Image object, for example:
                ImagePreview.Source = new BitmapImage(new Uri(selectedFileName));
                ImagePreview.RenderSize = new Size(200,200);
            }
        }

        private void CloseAddProductWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            // Add product logic here
        }
    }
}
