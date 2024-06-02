using ProjektSklep.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
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
        List<Category> categories = new List<Category>();
        private MyDbContext db = new MyDbContext();
        int[] promotions = [5, 5, 5,  5, 10, 10, 10, 15, 15, 20, 20, 30, 50]; 

        public WheelWindow()
        {
            InitializeComponent();
            categories = db.Categories.ToList();
            Random rndCategory = new Random();
            Random rndPromotion = new Random();
            double x=150, y=1;
            int segCount = 8;
            double arcAngle = 360 / segCount;
            double firstAngle = -90;

            for (int i = 0; i < segCount; i++)
            {
                Path path = new Path
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    Fill = i % 2 == 0 ? Brushes.Yellow : Brushes.Red,
                };

                PathGeometry pathGeometry = new PathGeometry();
                PathFigure pathFigure = new PathFigure
                {
                    StartPoint = new Point(150, 150),
                    IsClosed = true
                };

                pathFigure.Segments.Add(new LineSegment(new Point(x, y), true));

                firstAngle = firstAngle + arcAngle;
                double radians = firstAngle * (Math.PI / 180); 

                x = 150 + (150 * Math.Cos(radians));
                y = 150 + (150 * Math.Sin(radians));

                //System.Diagnostics.Debug.WriteLine("\nx: " + x + "\ny: " + y + "\nfirstAngle: " + firstAngle + "\ncos: " + Math.Cos(radians) + "\nsin: " + Math.Sin(radians));

                pathFigure.Segments.Add(new ArcSegment
                {
                    Point = new Point(x, y),
                    Size = new Size(150, 150),
                    RotationAngle = 0,
                    IsLargeArc = false,
                    SweepDirection = SweepDirection.Clockwise
                });

                pathGeometry.Figures.Add(pathFigure);
                path.Data = pathGeometry;
                wheelCanvas.Children.Add(path);

                double middleAngle = firstAngle - (arcAngle / 1.4);
                double middleRadians = middleAngle * (Math.PI / 180);
                double textX = 150 + (120 * Math.Cos(middleRadians)); 
                double textY = 150 + (120 * Math.Sin(middleRadians));

                int choosenCategory = rndCategory.Next(categories.Count());

                TextBlock spinText = new TextBlock
                {
                    Foreground = Brushes.Black,
                    FontWeight = FontWeights.Bold,
                    FontSize = 12,
                    TextAlignment = TextAlignment.Center, 
                    Text = promotions[rndPromotion.Next(promotions.Length)].ToString() + "% na\n" + categories[choosenCategory].name.Substring(0, categories[choosenCategory].name.Length < 7 ? categories[choosenCategory].name.Length : 7)
                };

                RotateTransform rotateText = new RotateTransform();
                rotateText.Angle = arcAngle * i + arcAngle / 2; 
                spinText.RenderTransform = rotateText;

                Canvas.SetLeft(spinText, textX ); 
                Canvas.SetTop(spinText, textY - spinText.ActualHeight / 2); 

                wheelCanvas.Children.Add(spinText);
            }
        }

        private void closeWheelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void spinWheel_Click(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            DoubleAnimation rotateAnimation = new DoubleAnimation
            {                
                From = 0, 
                To = rnd.Next(375, 1_000), 
                Duration = TimeSpan.FromSeconds(3), 
                AutoReverse = false, 
                RepeatBehavior = new RepeatBehavior(1) 
            };

            rotateAnimation.EasingFunction = new SineEase
            {
                EasingMode = EasingMode.EaseOut // Spowalnia animację w miarę czasu
            };

            canvasRotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
        }

    }
}
