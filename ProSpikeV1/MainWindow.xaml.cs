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
using System.ComponentModel;
//using System.Drawing;
//using System.Drawing.Printing;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
//using System.Drawing;
using System.Windows.Media.Animation;
using System.Threading;

namespace ProSpikeV1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        //public class SerialPort ; System.ComponentModel.Component;

        private Path myPath;
        private PathGeometry myPathGeometry;
        static int reSize = 4;
        static float growTime = 0.5f;
        static float shrinkTime = 0.3f;
        //private Path bezPath;
        //private Image image;
        static SerialPort port;
        List<int> sequence = new List<int>();
        List<int> motorSpeed = new List<int>();
        List<int> linAct = new List<int>();
        //List<Color> colorList = new List<Color> { Colors.Red, Colors.Orange, Colors.Yellow, Colors.Blue, Colors.Green, };
        string[] colours = { "Black", "Red", "Orange", "Yellow", "Green", "Blue", "Indigo", "Cyan", "Violet" , "Brown", "Lime"};
        Dictionary<string, Color> colourList = new Dictionary<string, Color> { { "White", Colors.White},{ "Black", Colors.Black }, { "Red", Colors.Red }, { "Lime", Colors.Lime }, { "Magenta", Colors.Magenta }, { "Cyan", Colors.Cyan }, { "Orange", Colors.Orange }, { "Brown", Colors.BurlyWood },  { "Yellow", Colors.Yellow }, { "Indigo", Colors.Indigo }, { "Violet", Colors.Violet }, { "Blue", Colors.Blue }, { "Green", Colors.Green } };

        public bool[] buttonActive = new bool[11];
        //public AppViewModel AppViewModel { get; set; }
        private SharedDataModel _dataModel;

        private bool isRunning = false;
        private int delay = 3000;
        public int defaultStartx = 125;
        public int defaultStarty = 225;
        public int defaultEndx = 640;
        public int defaultEndy = 400;
        public bool custom = false;
        public string homePoint = "14";
        public MainWindow()
        {
            
            InitializeComponent();
            _dataModel = new SharedDataModel();
            //AppViewModel viewModel = new AppViewModel();
            _dataModel.SelectedComboBoxIndex = 0;
            //_dataModel.xSliderValue1= 600;
            //_dataModel.ySliderValue1= 325;
            //_dataModel.xSliderValue2 = 0;
            //_dataModel.ySliderValue2 = 325;
            //_dataModel.xSliderValue3 = 250;
            //_dataModel.ySliderValue3 = 325;

            //this.DataContext = viewModel;

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
            if (port == null) { 
                //SerialPort port = new SerialPort("COM5", 115200);
                 //port.Open();


            }
            _dataModel.custom1 = false;
            _dataModel.custom2 = false;
            _dataModel.custom3 = false;


            Custom1.IsEnabled = _dataModel.custom1;
            Custom2.IsEnabled = _dataModel.custom2;
            Custom3.IsEnabled = _dataModel.custom3;

            //s1.Fill = new SolidColorBrush(red) ;
            _dataModel.xSliderValue1 = 600;
            _dataModel.ySliderValue1 = 300;
            _dataModel.xSliderValue2 = 300;
            _dataModel.ySliderValue2 = 300;
            _dataModel.xSliderValue3 = 400;
            _dataModel.ySliderValue3 = 300;
            _dataModel.c1Controlx1 = 300;
            _dataModel.c1Controly1 = -100;
            _dataModel.c1Controlx2 = 300;
            _dataModel.c1Controly2 = -100;
            _dataModel.c2Controlx1 = 300;
            _dataModel.c2Controly1 = -100;
            _dataModel.c2Controlx2 = 300;
            _dataModel.c2Controly2 = -100;
            _dataModel.c3Controlx1 = 300;
            _dataModel.c3Controly1 = -100;
            _dataModel.c3Controlx2 = 300;
            _dataModel.c3Controly2 = -100;
            //_dataModel.AreButtonsEnabled = false;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        
        public void activeButtonReset(int index)
        {
            for (int i = 1; i < 11; i++)
            {
                if (i != index)
                {
                    buttonActive[i] = false;
                }
            }
            
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
            int tempx, tempy, temp2x, temp2y;
            int moveX = startx;
            int moveY = starty;
            if (myCanvas.Children.Count > 0)
            {
                UIElement lastElement = myCanvas.Children[myCanvas.Children.Count - 1];
                if (lastElement is Path path && path.Tag as string == "bezier")
                    myCanvas.Children.RemoveAt(myCanvas.Children.Count - 1);
            }
            if (custom == false)
            {
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
            }
            else 
            {
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
                else
                {
                    temp2x = controlx1;
                    temp2y = controly1;
                    controly1 = controly2;
                    controlx1 = controlx2;
                    controlx2 = temp2x;
                    controly2 = temp2y;
                }
                custom = false;
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
            if (buttonActive[1] == false)
            {
                buttonActive[1] = true;
                activeButtonReset(1);
                MainText.Content = PHBall.Content;
                DrawBezier(150, 225, 640, 400, 300, -100, 500, -100);
            }
            else
            {
                
                sequence.Add(1);
                motorSpeed.Add(24);
                linAct.Add(20);

                seqUpdate();
            }
        }
        private void PHBall_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //MainText.Content = PHBall.Content;
            //DrawBezier(150, 225, 640, 400, 300, -100, 500, -100);
            
        }

        private void PShoot_Click(object sender, RoutedEventArgs e)
        {
            if (buttonActive[2] == false)
            {
                buttonActive[2] = true;
                activeButtonReset(2);
                MainText.Content = PShoot.Content;
                DrawBezier(150, 225, 640, 400, 300, 150, 500, 150);
            }
            else
            {
                
                //port.Write("2");
                sequence.Add(2);
                seqUpdate();
                motorSpeed.Add(54);
                linAct.Add(25);
            }
        }
        private void PShoot_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //MainText.Content = PShoot.Content;
            //DrawBezier(150, 225, 640, 400, 300, 150, 500, 150);
        }

        private void ThirtyThree_Click(object sender, RoutedEventArgs e)
        {
            if (buttonActive[3] == false)
            {
                buttonActive[3] = true;
                activeButtonReset(3);
                MainText.Content = ThirtyThree.Content;
                DrawBezier(310, 225, 640, 400, 420, 150, 500, 150);
            }
            else
            {
                
                sequence.Add(3);
                seqUpdate();
                motorSpeed.Add(50);
                linAct.Add(23);
            }
        }
        private void ThirtyThree_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //MainText.Content = ThirtyThree.Content;
            //DrawBezier(310, 225, 640, 400, 420, 150, 500, 150);
        }

        private void MHBall_Click(object sender, RoutedEventArgs e)
        {
            if (buttonActive[4] == false)
            {
                buttonActive[4] = true;
                activeButtonReset(4);
                MainText.Content = MHBall.Content;
                DrawBezier(480, 225, 640, 400, 530, -50, 550, -50);
            }
            else
            {
                
                //DrawBezier();
                sequence.Add(4);
                seqUpdate();
                motorSpeed.Add(50);
                linAct.Add(23);
            }
        }
        private void MHBall_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //MainText.Content = MHBall.Content;
            //DrawBezier(480, 225, 640, 400, 530, -50, 550, -50);
        }

        private void MQuick_Click(object sender, RoutedEventArgs e)
        {
            if (buttonActive[5] == false)
            {
                buttonActive[5] = true;
                activeButtonReset(5);
                MainText.Content = MQuick.Content;
                DrawBezier(480, 225, 640, 400, 530, 225, 550, 225);
            }
            else
            {
                
                sequence.Add(5);
                seqUpdate();
                motorSpeed.Add(50);
                linAct.Add(23);
            }

        }
        private void MQuick_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //MainText.Content = MQuick.Content;
            //DrawBezier(480, 225, 640, 400, 530, 225, 550, 225);
        }

        private void MSlide_Click(object sender, RoutedEventArgs e)
        {
            if (buttonActive[6] == false)
            {
                buttonActive[6] = true;
                activeButtonReset(6);
                DrawBezier(740, 225, 640, 400, 680, 150, 720, 150);
                MainText.Content = MSlide.Content;
            }
            else
            {
                
                sequence.Add(6);
                seqUpdate();
                motorSpeed.Add(50);
                linAct.Add(23);
            }
        }
        private void MSlide_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //DrawBezier(740, 225, 640, 400, 680, 150, 720, 150);
            //MainText.Content = MSlide.Content;
        }

        private void OHBall_Click(object sender, RoutedEventArgs e)
        {
            if (buttonActive[7] == false)
            {
                buttonActive[7] = true;
                activeButtonReset(7);
                DrawBezier(820, 225, 640, 400, 730, -100, 750, -100);
                MainText.Content = OHBall.Content;
            }
            else
            {
                
                sequence.Add(7);
                seqUpdate();
                motorSpeed.Add(50);
                linAct.Add(23);
            }
        }
        private void OHBall_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            
            //DrawBezier(820, 225, 640, 400, 730, -100, 750, -100);
            //MainText.Content = OHBall.Content;
        }

        private void Custom1_Click(object sender, RoutedEventArgs e)
        {
            //custom = false;
            if (buttonActive[8] == false)
            {
                buttonActive[8] = true;
                activeButtonReset(8);
                custom = true;
                MainText.Content = "Custom 1";
                DrawBezier((_dataModel.xSliderValue1 + defaultStartx), defaultStarty, defaultEndx, defaultEndy, _dataModel.c1Controlx1, _dataModel.c1Controly1, _dataModel.c1Controlx2, _dataModel.c1Controly2);

            }
            else
            {
                int y = defaultEndy - _dataModel.c1Controly1;
                int x = defaultEndx - _dataModel.c1Controlx1;

                sequence.Add(8);
                seqUpdate();
            }
        }
        private void Custom1_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //custom = true;
            //moveBall(-250, 100);
        }

        private void Custom2_Click(object sender, RoutedEventArgs e)
        {
            //custom = false;
            if (buttonActive[9] == false)
            {
                buttonActive[9] = true;
                activeButtonReset(9);
                custom = true;
                MainText.Content = "Custom 2";
                DrawBezier((_dataModel.xSliderValue2 + defaultStartx), defaultStarty, defaultEndx, defaultEndy, _dataModel.c2Controlx1, _dataModel.c2Controly1, _dataModel.c2Controlx2, _dataModel.c2Controly2);

            }
            else
            {
                //MainText.Content = "Custom 1";
                
                //DrawBezier((_dataModel.xSliderValue2 + defaultStartx), defaultStarty, defaultEndx, defaultEndy, _dataModel.c2Controlx1, _dataModel.c2Controly1, _dataModel.c2Controlx2, _dataModel.c2Controly2);

                sequence.Add(9);
                seqUpdate();
            }
        }
        private void Custom2_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {

            //MainText.Content = "Custom 2";
        }
        private void Custom3_Click(object sender, RoutedEventArgs e)
        {
            //custom = false;
            if (buttonActive[10] == false)
            {
                buttonActive[10] = true;
                activeButtonReset(10);
                MainText.Content = "Custom 3";
                custom = true;
                DrawBezier((_dataModel.xSliderValue3 + defaultStartx), defaultStarty, defaultEndx, defaultEndy, _dataModel.c3Controlx1, _dataModel.c3Controly1, _dataModel.c3Controlx2, _dataModel.c3Controly2);

            }
            else
            {
                sequence.Add(10);
                
                seqUpdate();
            }

        }
        private void Custom3_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            
            //MainText.Content = "Custom 3";

        }

        private void SettingsPage_Click(object sender, RoutedEventArgs e)
        {
            Custom1.IsEnabled = true;
            Custom2.IsEnabled = true;
            Custom3.IsEnabled = true;
            SettingsView setView = new SettingsView(_dataModel);
            setView.Show();
        }

        private async void StartStop_Checked(object sender, RoutedEventArgs e)
        {

            backArrow.IsEnabled = false;
            Garbage.IsEnabled = false;
            SerialPort serialPort = new SerialPort();
            if (SerialPort.GetPortNames().Contains("COM4"))
            {
                serialPort.PortName = "COM4";
            }
            else if (SerialPort.GetPortNames().Contains("COM5"))
            {
                serialPort.PortName = "COM5";
            }
            else if (SerialPort.GetPortNames().Contains("COM3"))
            {
                serialPort.PortName = "COM3";
            }
            else
            {
                MainText.Content="Arduino not found.";
                return;
            }

            string data;
            string response = "";
            int timeout = (int)_dataModel.userDelay *1000;
            serialPort.BaudRate = 9600;
            serialPort.Open();

            for (int i = 0; i < sequence.Count; i++)
            {
                if (StartStop.IsChecked == false)
                {
                    return;
                }
                //try
                //{
                data = motorSpeed[i].ToString() + "," + linAct[i].ToString()+".";
                serialPort.Write(data);
                DateTime start = DateTime.Now;
                //serialPort.Write(sequence[i].ToString());
                while ((DateTime.Now - start).TotalMilliseconds < timeout)
                {
                    // Check if a message has been received
                    if (response.Contains("Ready"))
                    {
                        // Do something with the response
                        break;
                    }
                    if (StartStop.IsChecked == false)
                    {
                        serialPort.Write("0," + homePoint + ".5");
                        serialPort.Close();
                        return;
                    }

                    // Wait for 100 milliseconds before checking again
                    Thread.Sleep(100);
                }
                //}
                //catch(Exception ex)
                //{
                //MainText.Content=ex.Message;
                //}
            }
            serialPort.Write("0,"+homePoint+".5");
            serialPort.Close();
            StartStop.IsChecked = false;
            
        }

        private void StartStop_Unchecked(object sender, RoutedEventArgs e)
        {
            backArrow.IsEnabled = true;
            Garbage.IsEnabled = true;
            //if serialPort()
        }
    }
}
