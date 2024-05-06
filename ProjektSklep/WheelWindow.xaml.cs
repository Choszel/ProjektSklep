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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjektSklep
{
    /// <summary>
    /// Interaction logic for WheelWindow.xaml
    /// </summary>
    public partial class WheelWindow : Window
    {
        public WheelWindow()
        {
            InitializeComponent();
        }

        private void closeWheelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void spinWheel_Click(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            // Tworzymy animację obrotu
            DoubleAnimation rotateAnimation = new DoubleAnimation
            {                
                From = 0, // Początkowy kąt obrotu
                To = rnd.Next(375, 1_000), // Końcowy kąt obrotu (360 stopni)
                Duration = TimeSpan.FromSeconds(3), // Czas trwania animacji (np. 3 sekundy)
                AutoReverse = false, // Ustawienie czy animacja powinna się odwrócić po zakończeniu
                RepeatBehavior = new RepeatBehavior(1) // Liczba powtórzeń animacji
            };

            rotateAnimation.EasingFunction = new SineEase
            {
                EasingMode = EasingMode.EaseOut // Spowalnia animację w miarę czasu
            };

            // Rozpoczynamy animację na RotateTransform kontrolki Canvas
            canvasRotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
        }

    }
}
