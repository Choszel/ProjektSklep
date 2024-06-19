using Notification.Wpf;
using ProjektSklep.Model;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ProjektSklep
{
    /// <summary>
    /// Interaction logic for WheelWindow.xaml
    /// </summary>
    public partial class WheelWindow : Window
    {
        List<Category> categories = new List<Category>();
        private MyDbContext db = new MyDbContext();
        int[] promotions = new int[] { 5, 5, 5, 5, 10, 10, 10, 15, 15, 20, 20, 30, 50 };

        public WheelWindow()
        {
            InitializeComponent();
            categories = db.Categories.ToList();
            Random rndCategory = new Random();
            Random rndPromotion = new Random();
            double x = 150, y = 1;
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

                int chosenCategory = rndCategory.Next(categories.Count());
                int chosenDiscount = rndPromotion.Next(promotions.Length);
                TextBlock spinText = new TextBlock
                {
                    Foreground = Brushes.Black,
                    FontWeight = FontWeights.Bold,
                    FontSize = 12,
                    TextAlignment = TextAlignment.Center,
                    Text = promotions[chosenDiscount].ToString() + "% na\n" + categories[chosenCategory].name.Substring(0, categories[chosenCategory].name.Length < 7 ? categories[chosenCategory].name.Length : 7)
                };

                spinText.Tag = $"{i}|{promotions[chosenDiscount]}|{chosenCategory+1}";

                RotateTransform rotateText = new RotateTransform();
                rotateText.Angle = arcAngle * i + arcAngle / 2;
                spinText.RenderTransform = rotateText;

                Canvas.SetLeft(spinText, textX);
                Canvas.SetTop(spinText, textY - spinText.ActualHeight / 2);

                wheelCanvas.Children.Add(spinText);
            }

            CheckCooldownAndInitializeButton();
        }

        private void CheckCooldownAndInitializeButton()
        {
            User currentUser = GetCurrentLoggedInUser();

            if (currentUser != null)
            {
                DateTime? lastSpinTime = currentUser.lastSpin;
                TimeSpan cooldown = TimeSpan.FromMinutes(1); // Adjust cooldown duration as needed
                DateTime nextSpinTime = lastSpinTime.HasValue ? lastSpinTime.Value + cooldown : DateTime.MinValue;

                if (lastSpinTime.HasValue && DateTime.Now < nextSpinTime)
                {
                    spinWheel.IsEnabled = false;
                    cooldownLabel.Content = $"Koło niedostępne. Następne zakręcenie możliwe: {nextSpinTime.ToString("d MMMM, yyyy HH:mm")}";
                    cooldownLabel.Visibility = Visibility.Visible;
                }
                else
                {
                    spinWheel.IsEnabled = true;
                    cooldownLabel.Content = "Zakręć kołem i wygraj zniżkę!";
                    cooldownLabel.Visibility = Visibility.Visible;
                }
            }
            else
            {
                MessageBox.Show("Nie udało się pobrać danych użytkownika.", "Błąd");
            }
        }

        private void closeWheelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private async Task noMoreSpins()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(3_000);
                spinWheel.IsEnabled = false;
            });
            spinWheel.IsEnabled = true;
        }

        private void spinWheel_Click(object sender, RoutedEventArgs e)
        {
            spinWheel.Visibility = Visibility.Hidden;
            cooldownLabel.Visibility = Visibility.Hidden;

            Random rnd = new Random();
            double target = rnd.Next(375, 1_000);
            DoubleAnimation rotateAnimation = new DoubleAnimation
            {
                From = 0,
                To = target,
                Duration = TimeSpan.FromSeconds(3),
                AutoReverse = false,
                RepeatBehavior = new RepeatBehavior(1)
            };

            rotateAnimation.EasingFunction = new SineEase
            {
                EasingMode = EasingMode.EaseOut // Spowalnia animację w miarę czasu
            };

            rotateAnimation.Completed += (s, _) =>
            {
                double currentAngle = -target % 360;
                int segment = (int)Math.Floor(currentAngle / (360 / 8)) + 8;
                int promo = -1;
                int catID = -1;
                foreach (var tb in wheelCanvas.Children.OfType<TextBlock>())
                {
                    string[] values = tb.Tag.ToString().Split('|');
                    if (values[0] == segment.ToString())
                    {
                        promo = Convert.ToInt32(values[1]);
                        catID = Convert.ToInt32(values[2]);
                    }
                }
                if (promo != -1 & catID != -1)
                {
                    // Update the currently logged user data
                    UpdateUserDiscount(promo, categories[catID-1].name, catID);

                    MessageBox.Show($"Uzyskano {promo}% zniżki na\nprodukty z kategorii {categories[catID-1].name}.", "Gratulacje!");

                    MainWindow.wheelTimer.Start();                   
                }
                else
                {
                    MessageBox.Show("Wystąpił nieoczekiwany błąd, spróbuj ponownie później."); //TO NIE MA PRAWA SIĘ NIGDY POJAWIĆ
                }
                spinWheel.IsEnabled = false;

                // Set the timer for 10 minutes
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMinutes(1); // Adjust cooldown duration as needed

                DateTime nextSpinTime = DateTime.Now.AddMinutes(1);

                timer.Tick += (s, _) =>
                {
                    // Enable the button after cooldown
                    spinWheel.IsEnabled = true;
                    cooldownLabel.Content = "Zakręć kołem i wygraj zniżkę!";
                    timer.Stop();
                };
                timer.Start();

                // Update the label to show when the next spin will be available
                cooldownLabel.Content = $"Koło niedostępne. Następne zakręcenie możliwe: {nextSpinTime.ToString("d MMMM, yyyy HH:mm")}";
                cooldownLabel.Visibility = Visibility.Visible;
                spinWheel.Visibility = Visibility.Visible;
            };

            canvasRotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
        }

        private void UpdateUserDiscount(int promo, string categoryName, int catID)
        {
            User currentUser = GetCurrentLoggedInUser();

            if (currentUser != null)
            {
                currentUser.lastSpin = DateTime.Now;
                currentUser.currDiscount = $"{promo}|{categoryName}|{catID}";

                db.Users.AddOrUpdate(currentUser);
                db.SaveChanges();
            }
            else
            {
                MessageBox.Show("Nie udało się zaktualizować danych użytkownika.", "Błąd");
            }
        }

        private User GetCurrentLoggedInUser()
        {
            // This method should return the currently logged-in user.
            int loggedUserID = UserType.Instance.loggedId;
            return db.Users.FirstOrDefault(u => u.userId == loggedUserID);
        }
    }
}
