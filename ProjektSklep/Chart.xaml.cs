using ProjektSklep.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjektSklep
{
    /// <summary>
    /// Interaction logic for Chart.xaml
    /// </summary>
    
    public partial class Chart
    {
        int startX = 70, endX= 620, startY = 20, endY = 290;
        public Chart()
        {
            InitializeComponent();
        }

        public void voidSpawn(string labelOX, string labelOY)
        {
            TextBlock textOY = new TextBlock
            {
                Name = "textOY",
                FontSize = 12,
                Text = labelOX
            };
            Canvas.SetLeft(textOY, 10);
            Canvas.SetBottom(textOY, 190);
            RotateTransform rotateTransform = new RotateTransform(-90);
            textOY.RenderTransform = rotateTransform;

            TextBlock textOX = new TextBlock
            {
                Name = "textOX",
                FontSize = 12,
                Text = labelOY
            };
            Canvas.SetLeft(textOX, 350);
            Canvas.SetBottom(textOX, 10);
            textOX.Text = labelOX;
            textOY.Text = labelOY;

            Line lineX = new Line
            {
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1,
                X1 = startX,
                Y1 = startY,
                X2 = startX,
                Y2 = endY
            };

            Line lineY = new Line
            {
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1,
                X1 = startX,
                Y1 = endY,
                X2 = endX,
                Y2 = endY
            };

            Polyline polyline1 = new Polyline
            {
                Stroke = new SolidColorBrush(Colors.Black),
                Points = new PointCollection
                {
                    new Point(startX-10, startY+15),
                    new Point(startX, startY),
                    new Point(startX+10, startY+15)
                }
            };

            Polyline polyline2 = new Polyline
            {
                Stroke = new SolidColorBrush(Colors.Black),
                Points = new PointCollection
                {
                    new Point(endX-15, endY-10),
                    new Point(endX, endY),
                    new Point(endX-15, endY+10)
                }
            };

            chartCanvas.Children.Add(polyline1);
            chartCanvas.Children.Add(polyline2);
            chartCanvas.Children.Add(textOX);
            chartCanvas.Children.Add(textOY);
            chartCanvas.Children.Add(lineX);
            chartCanvas.Children.Add(lineY);

            Canvas.SetLeft(textOX, startX + (endX - startX) / 2 - 40);
            Canvas.SetTop(textOY, startY + (endY - startY) / 2 + 20);
        }

        public void generateFirstChart(List<ProductOrder> productOrders, string labelOX, string labelOY) //dostaje listę zamówień produktów, np. psów
        {
            chartCanvas.Children.Clear();
            voidSpawn(labelOX, labelOY);
            if (productOrders.Count == 0) return;
            Polyline polyOfProduct = new Polyline();
            polyOfProduct.Stroke = new SolidColorBrush(Colors.Red);
            polyOfProduct.StrokeThickness = 1;
            Dictionary<DateTime, int> productsBoughtTime = new Dictionary<DateTime, int>();
            List<int> boughtInOneDay = new List<int>();

            foreach (ProductOrder productOrder in productOrders)
            {
                if (!productsBoughtTime.ContainsKey(productOrder.order.orderDate)) { productsBoughtTime[productOrder.order.orderDate] = productOrder.count; }
                else productsBoughtTime[productOrder.order.orderDate] += productOrder.count;
            }
            int iterator = 1;

            productsBoughtTime = productsBoughtTime
            .OrderBy(entry => entry.Key)
            .ToDictionary(entry => entry.Key, entry => entry.Value);

            int dayDiff = (productsBoughtTime.Keys.Max() - productsBoughtTime.Keys.Min()).Days;
            int divider = dayDiff > 20 ? 3 : dayDiff > 10 ? 2 : 1;
            int scaleCount = (dayDiff / divider) + 1;
            double lengthOfOneScaleX = (endX - startX - 30) / scaleCount; // maksymalna ilość dni to zawsze 30

            for (int i = 1; i <= scaleCount; i++)
            {
                Polyline polyline = new Polyline();
                polyline.Points.Add(new Point(startX + lengthOfOneScaleX * i, endY - 10));
                polyline.Points.Add(new Point(startX + lengthOfOneScaleX * i, endY + 10));
                polyline.Stroke = new SolidColorBrush(Colors.Black);
                polyline.StrokeThickness = 1;
                TextBlock textBlock = new TextBlock();
                textBlock.FontSize = 12;
                textBlock.Text = (productsBoughtTime.Keys.Min().AddDays(divider * (i - 1))).ToString().Substring(0, 10);
                Canvas.SetTop(textBlock, endY + 35);
                Canvas.SetLeft(textBlock, -20 + startX + lengthOfOneScaleX * i);

                RotateTransform rotateText = new RotateTransform();
                rotateText.Angle = -30;
                textBlock.RenderTransform = rotateText;
                chartCanvas.Children.Add(polyline);
                chartCanvas.Children.Add(textBlock);
            }

            double actualOneItemScaleWidth = lengthOfOneScaleX / divider; 

            foreach (KeyValuePair<DateTime, int> entry in productsBoughtTime)
            {
                if (!boughtInOneDay.Contains(entry.Value)) boughtInOneDay.Add(entry.Value);
                Debug.WriteLine(entry.Key + " " + entry.Value);
            }

            boughtInOneDay.Sort();
            if (boughtInOneDay.Count() < 10) scalePerPointY(boughtInOneDay);
            else scalePerScaleY(boughtInOneDay, 10);

            DateTime maxDate = productsBoughtTime.Keys.Max();
            Debug.WriteLine("maxDate: " + maxDate);
            int datePeriod = (maxDate - productsBoughtTime.Keys.Min()).Days;
            double actualOnePointScaleHeight = (endY - startY - 30) / (double)boughtInOneDay.Max(); //wielkość jednego punktu na wykresie
            iterator = 1;
            double lastX = 0;

            if (productsBoughtTime.Count < 2)
            {               
                Line line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Black); line.StrokeThickness = 2;
                line.X1 = startX + iterator * actualOneItemScaleWidth;
                line.Y1 = startY + boughtInOneDay[0] * actualOnePointScaleHeight;
                line.X2 = 1 + startX + iterator * actualOneItemScaleWidth;
                line.Y2 = 1 + startY + boughtInOneDay[0] * actualOnePointScaleHeight;
                chartCanvas.Children.Add(line);
            }
            else
            {
                foreach (KeyValuePair<DateTime, int> entry in productsBoughtTime)
                {
                    int diffDays = (maxDate - entry.Key).Days;
                    double realXPlacement = startX + lengthOfOneScaleX + (datePeriod - diffDays) * actualOneItemScaleWidth;
                    if (realXPlacement == lastX) realXPlacement += actualOneItemScaleWidth;
                    polyOfProduct.Points.Add(new Point(realXPlacement, endY - actualOnePointScaleHeight * entry.Value));
                    Rectangle rectangle = new Rectangle();
                    rectangle.RadiusX = 10;
                    rectangle.RadiusY = 10;
                    rectangle.Stroke = new SolidColorBrush(Colors.Black);
                    rectangle.StrokeThickness = 4;
                    rectangle.Stretch = Stretch.Fill;
                    chartCanvas.Children.Add(rectangle);
                    Canvas.SetLeft(rectangle, -2 + realXPlacement);
                    Canvas.SetTop(rectangle, -2 + endY - actualOnePointScaleHeight * entry.Value);
                    iterator++;
                    Debug.WriteLine(entry.Key + " " + entry.Value + "x: " + (realXPlacement + " y: " + (endY - actualOnePointScaleHeight * entry.Value)));
                    Debug.WriteLine("(maxDate - entry.Key).Days: " + diffDays);
                    lastX = realXPlacement;

                }
                chartCanvas.Children.Add(polyOfProduct);
            }
           
            Debug.WriteLine("ilość elementów na boughtInOneDay: " + boughtInOneDay.Count());
        }

        public void scalePerPointY(List<int> boughtInOneDay)
        {
            int iterator = 1;
            int maxValue = boughtInOneDay.Max();
            double lengthOfOnePointY = (endY - startY - 30) / (double)maxValue; //wielkość jednego punktu na wykresie
            
            foreach (int entry in boughtInOneDay)
            {
                Polyline polyline = new Polyline();
                Polyline grid = new Polyline();
                grid.Points.Add(new Point(startX, endY - lengthOfOnePointY * entry));
                grid.Points.Add(new Point(endX, endY - lengthOfOnePointY * entry));
                grid.Stroke = new SolidColorBrush(Colors.LightGray);
                grid.StrokeThickness = 1;
                chartCanvas.Children.Add(grid);
                polyline.Points.Add(new Point(startX - 10, endY - lengthOfOnePointY * entry));
                polyline.Points.Add(new Point(startX + 10, endY - lengthOfOnePointY * entry));
                polyline.Stroke = new SolidColorBrush(Colors.Black);
                polyline.StrokeThickness = 1;
                chartCanvas.Children.Add(polyline);
                TextBlock textBlock = new TextBlock();
                textBlock.FontSize = 12;
                textBlock.Text = entry.ToString();
                Canvas.SetTop(textBlock, -10 + endY - lengthOfOnePointY * entry);
                Canvas.SetLeft(textBlock, startX - 35);
                chartCanvas.Children.Add(textBlock);
                iterator++;
            }
        }

        public void scalePerScaleY(List<int> boughtInOneDay, int numberOfScales)
        {
            double lengthOfOneScaleY = (endY - startY) / (double)(numberOfScales +1);
            double valueOfScale = boughtInOneDay.Max() / (double)numberOfScales;
                        
            for (int i=1; i< numberOfScales+1; i++)
            {
                Polyline polyline = new Polyline();
                Polyline grid = new Polyline();
                grid.Points.Add(new Point(startX, endY - lengthOfOneScaleY * i));
                grid.Points.Add(new Point(endX, endY - lengthOfOneScaleY * i));
                grid.Stroke = new SolidColorBrush(Colors.LightGray);
                grid.StrokeThickness = 1;
                chartCanvas.Children.Add(grid);
                polyline.Points.Add(new Point(startX - 10, endY - lengthOfOneScaleY * i));
                polyline.Points.Add(new Point(startX + 10, endY - lengthOfOneScaleY * i));
                polyline.Stroke = new SolidColorBrush(Colors.Black);
                polyline.StrokeThickness = 1;
                chartCanvas.Children.Add(polyline);
                TextBlock textBlock = new TextBlock();
                textBlock.FontSize = 12;
                double value = valueOfScale * i;
                value *= 100;
                value -= value % 1;
                value /= 100;
                textBlock.Text = (value).ToString();
                Canvas.SetTop(textBlock, -10 + endY - lengthOfOneScaleY * i);
                Canvas.SetLeft(textBlock, startX - 35);
                chartCanvas.Children.Add(textBlock);
            }         
        }
    }
}
