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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CnnWpfClient
{
    /// <summary>
    /// Interaction logic for GridCanvas.xaml
    /// </summary>
    public partial class GridCanvas : UserControl
    {
        public GridCanvas()
            : this(MinRowCount, MinColumnCount)
        {
        }

        public GridCanvas(int nRow, int nColumn)
        {
            Row = nRow < MinRowCount ? MinRowCount : nRow;
            Column = nColumn < MinColumnCount ? MinColumnCount : nColumn;
            InitializeComponent();
            //Points = new PointCollection(Row * Column);
            //for (int iRow = 0; iRow < Row; ++iRow)
            //    for (int iColumn = 0; iColumn < Column; ++iColumn)
            //        Points.Add(getPosition(iRow, iColumn));
        }

        private UIElementCollection Chesses;
        //private  PointCollection Points = null;


        private static double GridLineWeight = 5;
        private static int MinRowCount = 5;
        private static int MinColumnCount = 5;
        private static double ChessRadiu = 20;

        private void drawGrid()
        {
            double marginX = 20.0;
            double marginY = 20.0;
            Rect rectGrid = new Rect(20.0, 20.0, myGridCanvas.ActualWidth -2 * marginX, myGridCanvas.ActualHeight - 2 * marginY);
            double distanceX = (myGridCanvas.ActualWidth - 2 * marginX) / (Column - 1);
            double distanceY = (myGridCanvas.ActualHeight - 2 * marginY) / (Row - 1);
            for (int index = 0; index < Row; ++index)
            {
                Line mydrawline = new Line();
                mydrawline.Stroke = Brushes.Black;//外宽颜色，在直线里为线颜色
                                                  //mydrawline.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x5B, 0x9B, 0xD5));//自定义颜色则用这句
                mydrawline.StrokeThickness = GridLineWeight;//线宽度
                double lineY = rectGrid.Y + index * distanceY;
                mydrawline.X1 = rectGrid.X;
                mydrawline.Y1 = lineY;
                mydrawline.X2 = rectGrid.X + rectGrid.Width;
                mydrawline.Y2 = lineY;

                myGridCanvas.Children.Add(mydrawline);
            }
            for (int index = 0; index < Column; ++index)
            {
                Line mydrawline = new Line();
                mydrawline.Stroke = Brushes.Black;//外宽颜色，在直线里为线颜色
                                                  //mydrawline.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x5B, 0x9B, 0xD5));//自定义颜色则用这句
                mydrawline.StrokeThickness = GridLineWeight;//线宽度
                double lineX = rectGrid.X + index * distanceX;
                mydrawline.X1 = lineX;
                mydrawline.Y1 = rectGrid.Y;
                mydrawline.X2 = lineX;
                mydrawline.Y2 = rectGrid.Y + rectGrid.Height;
                myGridCanvas.Children.Add(mydrawline);
            }
        }

        private void drawChess(int nRowIndex, int nColIndex)
        {
            Point center = getPosition(nRowIndex, nColIndex);
            Button btn = new Button();
            btn.Width = btn.Height = ChessRadiu * 2;
            //btn.Background = Brushes.Transparent;
            //Ellipse elps = new Ellipse();
            //elps.Fill = Brushes.Red;
            //elps.Width = elps.Height = ChessRadiu * 2;
            //btn.Content = elps;
            Thickness margin = new Thickness(center.X - ChessRadiu, center.Y - ChessRadiu, 0, 0);
            btn.Margin = margin;
            myGridCanvas.Children.Add(btn);

            //EllipseGeometry geo = new EllipseGeometry(center, ChessRadiu, ChessRadiu);
            //Path path = new Path
            //{
            //    Stroke = Brushes.Gray,
            //    StrokeThickness = GridLineWeight,
            //    Fill = Brushes.White,
            //    Data = geo
            //};
            //myGridCanvas.Children.Add(path);
        }

        private Point getPosition(int nRowIndex, int nColIndex)
        {
            double marginX = 20.0;
            double marginY = 20.0;
            Rect rectGrid = new Rect(20.0, 20.0, myGridCanvas.ActualWidth - 2 * marginX, myGridCanvas.ActualHeight - 2 * marginY);
            double distanceX = (myGridCanvas.ActualWidth - 2 * marginX) / (Column - 1);
            double distanceY = (myGridCanvas.ActualHeight - 2 * marginY) / (Row - 1);

            Point pos = new Point(rectGrid.X + nColIndex * distanceX, rectGrid.Y + nRowIndex * distanceY);
            return pos;

        }

        #region property

        public int Row { get; set; }
        public int Column { get; set; }

        #endregion property

        private void createChesses()
        {
            Chesses = myGridCanvas.Children;

            double marginX = 20.0;
            double marginY = 20.0;
            double distanceX = (myGridCanvas.ActualWidth - 2 * marginX) / (Column - 1);
            double distanceY = (myGridCanvas.ActualHeight - 2 * marginY) / (Row - 1);

            for (int iRow = 0; iRow < Row; ++iRow)
                for (int iColumn = 0; iColumn < Column; ++iColumn)
                {
                    Image chess = new Image();
                    chess.Source = new BitmapImage(new Uri("Resource/Images/place_holder_16.ico", UriKind.Relative));
                    chess.Width = chess.Height = ChessRadiu * 2;

                    Point center = new Point(marginX + iColumn * distanceX, marginY + iRow * distanceY);

                    Thickness margin = new Thickness(center.X - ChessRadiu, center.Y - ChessRadiu, 0, 0);
                    chess.Margin = margin;
                    Chesses.Add(chess);
                }

        }

        private void onLoaded(object sender, RoutedEventArgs e)
        {
            drawGrid();
            createChesses();
        }
    }
}
