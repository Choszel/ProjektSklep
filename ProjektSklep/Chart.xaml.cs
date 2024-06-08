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
            double lengthOfOneScaleX = (endX - startX) / ((productsBoughtTime.Count() > 14 ?  14 : productsBoughtTime.Count()) + 1);
            int iterator = 1;

            productsBoughtTime = productsBoughtTime
            .OrderBy(entry => entry.Key)
            .ToDictionary(entry => entry.Key, entry => entry.Value);

            foreach (KeyValuePair<DateTime, int> entry in productsBoughtTime)
            {
                if(!boughtInOneDay.Contains(entry.Value))boughtInOneDay.Add(entry.Value);      
                Polyline polyline = new Polyline();
                polyline.Points.Add(new Point(startX + lengthOfOneScaleX * iterator, endY-10));
                polyline.Points.Add(new Point(startX + lengthOfOneScaleX * iterator, endY+10));
                polyline.Stroke = new SolidColorBrush(Colors.Black);
                polyline.StrokeThickness = 1;
                TextBlock textBlock = new TextBlock();
                textBlock.FontSize = 12;
                textBlock.Text = entry.Key.ToString().Substring(0, 10);
                Canvas.SetTop(textBlock, endY + 30);
                Canvas.SetLeft(textBlock, -20 + startX + lengthOfOneScaleX * iterator);

                RotateTransform rotateText = new RotateTransform();
                rotateText.Angle = -30;
                textBlock.RenderTransform = rotateText;
                chartCanvas.Children.Add(polyline);
                chartCanvas.Children.Add(textBlock);
                iterator++;
                Debug.WriteLine(entry.Key + " " + entry.Value);
            }

            iterator = 1;
            double lengthOfOneScaleY = (endY - startY) / ((boughtInOneDay.Count() > 8 ? 8 : productsBoughtTime.Count()) + 1);
         
            boughtInOneDay.Sort();
            foreach (int entry in boughtInOneDay)
            {
                Polyline polyline = new Polyline();
                polyline.Points.Add(new Point(startX-10, endY - lengthOfOneScaleY * iterator));
                polyline.Points.Add(new Point(startX+10, endY - lengthOfOneScaleY * iterator));
                polyline.Stroke = new SolidColorBrush(Colors.Black);
                polyline.StrokeThickness = 1;
                chartCanvas.Children.Add(polyline);
                TextBlock textBlock = new TextBlock();
                textBlock.FontSize = 12;
                textBlock.Text = entry.ToString();
                Canvas.SetTop(textBlock, -10 + endY - lengthOfOneScaleY * iterator);
                Canvas.SetLeft(textBlock, startX - 35);             
                chartCanvas.Children.Add(textBlock);
                iterator++;
                Debug.WriteLine(entry);
            }

            double actualOneItemScaleWidth = (endX - startX) / (productsBoughtTime.Count()+1);
            double actualOneItemScaleHeight = (endY - startY) / (boughtInOneDay.Count() + 1);
            iterator = 1;

            if (productsBoughtTime.Count < 2)
            {               
                Line line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Black); line.StrokeThickness = 2;
                line.X1 = startX + iterator * actualOneItemScaleWidth;
                line.Y1 = startY + boughtInOneDay[0] * actualOneItemScaleHeight;
                line.X2 = 1 + startX + iterator * actualOneItemScaleWidth;
                line.Y2 = 1 + startY + boughtInOneDay[0] * actualOneItemScaleHeight;
                chartCanvas.Children.Add(line);
            }
            else
            {
                foreach (KeyValuePair<DateTime, int> entry in productsBoughtTime)
                {
                    polyOfProduct.Points.Add(new Point(startX + iterator* actualOneItemScaleWidth, endY - actualOneItemScaleHeight * (boughtInOneDay.IndexOf(entry.Value)+1)));
                    iterator++;
                    Debug.WriteLine(entry.Key + " " + entry.Value);
                }
                chartCanvas.Children.Add(polyOfProduct);
            }
           
            Debug.WriteLine("ilość elementów na boughtInOneDay: " + boughtInOneDay.Count());

            Debug.WriteLine("\nlengthOfOneScaleX: " + lengthOfOneScaleX + "\nlengthOfOneScaleY: " + lengthOfOneScaleY);
        }
    }
}
