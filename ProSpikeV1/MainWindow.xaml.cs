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
using System.IO.Ports;
//using System.Drawing;
//using System.Drawing.Printing;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
//using System.Drawing;
using System.Windows.Media.Animation;

namespace ProSpikeV1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        //public class SerialPort ; System.ComponentModel.Component

        private Path myPath;
        private PathGeometry myPathGeometry;
        static int reSize = 4;
        static float growTime = 0.5f;
        static float shrinkTime = 0.3f;
        //private Path bezPath;
        //private Image image;
        //static SerialPort port;
        List<int> sequence = new List<int>();
        //List<Color> colorList = new List<Color> { Colors.Red, Colors.Orange, Colors.Yellow, Colors.Blue, Colors.Green, };
        string[] colours = { "Black", "Red", "Orange", "Yellow", "Green", "Blue", "Indigo", "Cyan", "Violet" , "Magenta", "Lime"};
        Dictionary<string, Color> colourList = new Dictionary<string, Color> { { "White", Colors.White},{ "Black", Colors.Black }, { "Red", Colors.Red }, { "Lime", Colors.Lime }, { "Magenta", Colors.Magenta }, { "Cyan", Colors.Cyan }, { "Orange", Colors.Orange }, { "Yellow", Colors.Yellow }, { "Indigo", Colors.Indigo }, { "Violet", Colors.Violet }, { "Blue", Colors.Blue }, { "Green", Colors.Green } };

       

        public MainWindow()
        {
            InitializeComponent();

            c1.Fill = new SolidColorBrush(colourList[colours[1]]);
            c2.Fill = new SolidColorBrush(colourList[colours[2]]);
            c3.Fill = new SolidColorBrush(colourList[colours[3]]);
            c4.Fill = new SolidColorBrush(colourList[colours[4]]);
            c5.Fill = new SolidColorBrush(colourList[colours[5]]);
            c6.Fill = new SolidColorBrush(colourList[colours[6]]);
            c7.Fill = new SolidColorBrush(colourList[colours[7]]);
            c8.Fill = new SolidColorBrush(colourList[colours[8]]);
            c9.Fill = new SolidColorBrush(colourList[colours[9]]);
            c10.Fill = new SolidColorBrush(colourList[colours[10]]);
            /* if (port == null) { 
                port = new SerialPort("COM5", 115200);
                port.Open();


            }*/
            /*void MainWindow_WindowClosed(object sender, WindowClosedEventArgs e)
            {
                if (port != null && port.IsOpen)
                {
                    port.Close();
                }
            }*/

            //s1.Fill = new SolidColorBrush(red) ;


            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        public void animateSeq(Rectangle targetBox, int size, float Time)
        {
           
            DoubleAnimation heightAnimation = new DoubleAnimation();
            heightAnimation.From = targetBox.Height;
            heightAnimation.To = size; // Set the final height of the rectangle
            heightAnimation.Duration = new Duration(TimeSpan.FromSeconds(Time));
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(heightAnimation);
            Storyboard.SetTarget(heightAnimation, targetBox);
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath(System.Windows.Shapes.Rectangle.HeightProperty));
            storyboard.Begin();

        }
        public void seqUpdate()
        {
            int tempLen = sequence.Count;
            int sizeGrow = 100;
            //if (tempLen < 8)
            //{
            if (sequence.Count > 8)
            {
                return;
            }
            if (sequence.Count == 1) animateSeq(s1, sizeGrow, growTime);
            else if (sequence.Count == 2) animateSeq(s2, sizeGrow, growTime);
            else if (sequence.Count == 3) animateSeq(s3, sizeGrow, growTime);
            else if (sequence.Count == 4) animateSeq(s4, sizeGrow, growTime);
            else if (sequence.Count == 5) animateSeq(s5, sizeGrow, growTime);
            else if (sequence.Count == 6) animateSeq(s6, sizeGrow, growTime);
            else if (sequence.Count == 7) animateSeq(s7, sizeGrow, growTime);
            else if (sequence.Count == 8) animateSeq(s8, sizeGrow, growTime);

            //for (int c = tempLen; c < 8; c++;
            //sequence.Add(0);
            //}

            List<int> Zeros = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0 };

            //List<int> tempList = new List<int>
            for (int j = 0; j < tempLen; j++){

                Zeros[j] +=  + sequence[j];
            }

            //for (int i = 0; i <= sequence.Count; i++)
            //{

            

                // Get the i-th element that represents the button order
                s1.Fill = new SolidColorBrush(colourList[colours[Zeros[0]]]);
                s2.Fill = new SolidColorBrush(colourList[colours[Zeros[1]]]);
                s3.Fill = new SolidColorBrush(colourList[colours[Zeros[2]]]);
                s4.Fill = new SolidColorBrush(colourList[colours[Zeros[3]]]);
                s5.Fill = new SolidColorBrush(colourList[colours[Zeros[4]]]);
                s6.Fill = new SolidColorBrush(colourList[colours[Zeros[5]]]);
                s7.Fill = new SolidColorBrush(colourList[colours[Zeros[6]]]);
                s8.Fill = new SolidColorBrush(colourList[colours[Zeros[7]]]);
                // Set the fill color based on whether the button at that index has been pressed or not

            //}
        }
        private void Garbage_Click(object sender, RoutedEventArgs e)
        {
            
            sequence.Clear();
            
            animateSeq(s1, reSize, shrinkTime);
            animateSeq(s2, reSize, shrinkTime);
            animateSeq(s3, reSize, shrinkTime);
            animateSeq(s4, reSize, shrinkTime);
            animateSeq(s5, reSize, shrinkTime);
            animateSeq(s6, reSize, shrinkTime);
            animateSeq(s7, reSize, shrinkTime);
            animateSeq(s8, reSize, shrinkTime);
            seqUpdate();
        }
        private void backArrow_Click(object sender, RoutedEventArgs e)
        {
            int tempSeqCount = sequence.Count;
            if (tempSeqCount == 0)
            {
                return;
            }
            else {
                sequence.RemoveAt(sequence.Count - 1);
                seqUpdate();
                if (tempSeqCount == 1) animateSeq(s1, reSize, shrinkTime);
                else if (tempSeqCount == 2) animateSeq(s2, reSize, shrinkTime);
                else if (tempSeqCount == 3) animateSeq(s3, reSize, shrinkTime);
                else if (tempSeqCount == 4) animateSeq(s4, reSize, shrinkTime);
                else if (tempSeqCount == 5) animateSeq(s5, reSize, shrinkTime);
                else if (tempSeqCount == 6) animateSeq(s6, reSize, shrinkTime);
                else if (tempSeqCount == 7) animateSeq(s7, reSize, shrinkTime);
                else if (tempSeqCount == 8) animateSeq(s8, reSize, shrinkTime);
            }
        }

        bool arcDisplayed = false;
        // drawArc(xradius, yradius, xterminalpoint, yterminalpoint, beginningx, beginningy)
        // int xradius, int yradius, int pointx, int pointy, int startx, int starty
        //int startPointx, int startPointy, int endPointx, int endPointy, int controlPointx, int controlPointy
        private void drawArc(int xradius, int yradius, int pointx, int pointy, int startx, int starty)
        {
            // Deletes existing arc segments before redrawing
            if (arcDisplayed)
            {
                myCanvas.Children.Remove(myPath); // myPath = p
                arcDisplayed = false;
            }

            /* Here the Point is used to define the terminal point of the Arc. 
             * The Size specifies the X and Y radius of the arc. 
             * The Rotation Angle specifies the X axis of the arc. 
             * IsLargeArc specifies that the arc will be drawn more than 180 degrees.
               And SweepDirection specifies whether the arc sweeps clockwise or counterclockwise. */
            myPath = new Path();
            myPath.Stroke = System.Windows.Media.Brushes.Black;
            myPath.StrokeThickness = 2;

            myPathGeometry = new PathGeometry();
            PathFigure myPathFigure = new PathFigure();
            myPathFigure.StartPoint = new Point(startx, starty);

            ArcSegment myArcSegment = new ArcSegment();
            myArcSegment.Point = new Point(pointx, pointy);
            myArcSegment.Size = new Size(xradius, yradius);
            myArcSegment.SweepDirection = SweepDirection.Counterclockwise;
            myArcSegment.IsLargeArc = true;

            myPathFigure.Segments.Add(myArcSegment);
            myPathGeometry.Figures.Add(myPathFigure);
            myPath.Data = myPathGeometry;

            myCanvas.Children.Add(myPath);

            
             arcDisplayed = true;


            
        }
        //bool bezierDisplayed = false;
        private void moveBall(int newX, int newY)
        {
            Canvas.SetLeft(volleyballIcon, newX - 25);
            Canvas.SetTop(volleyballIcon, newY - 25);
        }
        private void DrawBezier(int startx, int starty, int endx, int endy, int controlx1, int controly1, int controlx2, int controly2)
        {
            int tempx, tempy;
            int moveX = startx;
            int moveY = starty;
            if (myCanvas.Children.Count > 0)
            {
                UIElement lastElement = myCanvas.Children[myCanvas.Children.Count - 1];
                if (lastElement is Path path && path.Tag as string == "bezier")
                    myCanvas.Children.RemoveAt(myCanvas.Children.Count - 1);
            }

            if (startx > endx)
            {
                tempx = endx;
                tempy = endy;
                endx = startx;
                endy = starty;
                startx = tempx;
                starty = tempy;
                moveX = endx;
                moveY = endy;


            }

            moveBall(moveX, moveY); //Not moving the ball

            Path bezPath = new Path();
            bezPath.Stroke = Brushes.Black;
            bezPath.StrokeThickness = 2;
            bezPath.Tag = "bezier";

            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(startx, starty); // startx, starty

            BezierSegment bezierSegment = new BezierSegment();
            bezierSegment.Point1 = new Point(controlx1, controly1); // controlpoint1
            bezierSegment.Point2 = new Point(controlx2, controly2); // controlpoint2
            bezierSegment.Point3 = new Point(endx, endy); // endx, endy

            pathFigure.Segments.Add(bezierSegment);

            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);
            bezPath.Data = pathGeometry;

          
            myCanvas.Children.Add(bezPath);

            

        }

        

        // drawArc(xradius, yradius, xterminalpoint, yterminalpoint, beginningx, beginningy)
        private void PHBall_Click(object sender, RoutedEventArgs e)
        {
            MainText.Content = PHBall.Content;
            DrawBezier(150, 225, 640, 400, 300, -100, 500, -100);
            sequence.Add(1);

            seqUpdate();
        }
        private void PHBall_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MainText.Content = PHBall.Content;
            DrawBezier(150, 225, 640, 400, 300, -100, 500, -100);

        }

        private void PShoot_Click(object sender, RoutedEventArgs e)
        {
            
            MainText.Content = PShoot.Content;
            DrawBezier(150, 225, 640, 400, 300, 150, 500, 150);
            //port.Write("2");
            sequence.Add(2);
            seqUpdate();

        }
        private void PShoot_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MainText.Content = PShoot.Content;
            DrawBezier(150, 225, 640, 400, 300, 150, 500, 150);
        }

        private void ThirtyThree_Click(object sender, RoutedEventArgs e)
        {
            MainText.Content = ThirtyThree.Content;
            DrawBezier(310, 225, 640, 400, 420, 150, 500, 150);
            sequence.Add(3);
            seqUpdate();
        }
        private void ThirtyThree_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MainText.Content = ThirtyThree.Content;
            DrawBezier(310, 225, 640, 400, 420, 150, 500, 150);
        }

        private void MHBall_Click(object sender, RoutedEventArgs e)
        {
            MainText.Content = MHBall.Content;
            DrawBezier(480, 225, 640, 400, 530, -50, 550, -50);
            //DrawBezier();
            sequence.Add(4);
            seqUpdate();
        }
        private void MHBall_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MainText.Content = MHBall.Content;
            DrawBezier(480, 225, 640, 400, 530, -50, 550, -50);
        }

        private void MQuick_Click(object sender, RoutedEventArgs e)
        {
            MainText.Content = MQuick.Content;
            DrawBezier(480, 225, 640, 400, 530, 225, 550, 225);
            sequence.Add(5);
            seqUpdate();
        }
        private void MQuick_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MainText.Content = MQuick.Content;
            DrawBezier(480, 225, 640, 400, 530, 225, 550, 225);
        }

        private void MSlide_Click(object sender, RoutedEventArgs e)
        {
            DrawBezier(740, 225, 640, 400, 680, 150, 720, 150);
            MainText.Content = MSlide.Content;
            sequence.Add(6);
            seqUpdate();
        }
        private void MSlide_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DrawBezier(740, 225, 640, 400, 680, 150, 720, 150);
            MainText.Content = MSlide.Content;
        }

        private void OHBall_Click(object sender, RoutedEventArgs e)
        {
            DrawBezier(820, 225, 640, 400, 730, -100, 750, -100);
            MainText.Content = OHBall.Content;
            sequence.Add(7);
            seqUpdate();
        }
        private void OHBall_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DrawBezier(820, 225, 640, 400, 730, -100, 750, -100);
            MainText.Content = OHBall.Content;
        }

        private void BackrowPipe_Click(object sender, RoutedEventArgs e)
        {
            MainText.Content = BackrowPipe.Content;
            sequence.Add(8);
            seqUpdate();
        }
        private void BackrowPipe_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DrawBezier(0, 0, 0, 0, 0, 0, 0, 0);
            moveBall(250, 100);
        }

        private void BackrowC_Click(object sender, RoutedEventArgs e)
        {
            MainText.Content = BackrowC.Content;
            sequence.Add(9);
            seqUpdate();
        }
        private void BackrowC_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DrawBezier(0, 0, 0, 0, 0, 0, 0, 0);
            moveBall(250, 100);
        }
        private void Custom_Click(object sender, RoutedEventArgs e)
        {
            MainText.Content = Custom.Content;
            sequence.Add(10);
            seqUpdate();

        }
        private void Custom_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DrawBezier(0, 0, 0, 0, 0, 0, 0, 0);
            moveBall(250, 100);
        }

        private void SettingsPage_Click(object sender, RoutedEventArgs e)
        {
           
            SettingsView setView = new SettingsView();
            setView.Show();
        }

        
    }
}
