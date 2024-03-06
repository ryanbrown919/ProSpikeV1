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
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Windows.Media.Animation;
using System.Threading;
using System.Reflection;
using System.Diagnostics;

/*
Author: Ryan Brown
Date: April 2023

This program was designed to interact with a series of arduinos and other electrical components as the user interface for ProSpike, 
    a volleyball setting machien created for IGEN 330 design course

This GUI allows users to choose a sequence of pre-configured sets and well as custom sets with a graphical representation in real-time
*/



namespace ProSpikeV1
{
    /// Interaction logic for MainWindow.xaml
    public partial class MainWindow : Window
    {



        private Path myPath;
        private PathGeometry myPathGeometry;
        static int reSize = 4;
        static float growTime = 0.5f;
        static float shrinkTime = 0.3f;
        static SerialPort port;
        List<int> sequence = new List<int>();
        List<int> motorSpeed = new List<int>();
        List<int> linAct = new List<int>();

        // These are the colours used for visually differentiating the different buttons and set types, along with their window.shapes equivalent colour
        string[] colours = { "Black", "Red", "Orange", "Yellow", "Green", "Blue", "Indigo", "Cyan", "Violet", "Brown", "Lime" };
        Dictionary<string, Color> colourList = new Dictionary<string, Color> { { "White", Colors.White }, { "Black", Colors.Black }, { "Red", Colors.Red }, { "Lime", Colors.Lime }, { "Magenta", Colors.Magenta }, { "Cyan", Colors.Cyan }, { "Orange", Colors.Orange }, { "Brown", Colors.BurlyWood }, { "Yellow", Colors.Yellow }, { "Indigo", Colors.Indigo }, { "Violet", Colors.Violet }, { "Blue", Colors.Blue }, { "Green", Colors.Green } };

        
        public bool[] buttonActive = new bool[11];

        // This data model allowed variables to be shared between the main page and the settings page where custom set parameters are set
        private SharedDataModel _dataModel;

        private bool isRunning = false;
        private int delay = 3000;

        // Sets the defualt start position for the volleyball, seen as the 2/3 mark along the net where the setter traditionally starts from
        public int defaultStartx = 125;
        public int defaultStarty = 225;
        public int defaultEndx = 640;
        public int defaultEndy = 400;
        public bool custom = false;

        // This represents the extension of the linear actuator in cm, and was determined empirically to be the horizontal point for the frame
        public string homePoint = "14";
        public int sizeGrow = 70;
        double distanceToAxis = 26.67;
        public event EventHandler<bool> DemoButtonCheckedChanged;

        public MainWindow()
        {


            // Initialize the GUI with attributes and interactive elements

            InitializeComponent();
            _dataModel = new SharedDataModel();
            _dataModel.SelectedComboBoxIndex = 0;
            
            _dataModel.PropertyChanged += DataModel_PropertyChanged;

            // Parameter used to differ men's and women's net height
            _dataModel.netHeight = 3;

            // Adding colours to the buttons as shapes so they can be configured and trandformed in real-time
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
            
           
           // Initialize the custom sets as disabled until user enables them
            _dataModel.custom1 = false;
            _dataModel.custom2 = false;
            _dataModel.custom3 = false;
            Custom1.IsEnabled = _dataModel.custom1;
            Custom2.IsEnabled = _dataModel.custom2;
            Custom3.IsEnabled = _dataModel.custom3;


            // initialize the slider values with default values
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
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        // Function used temporarily with DEMO mode for Deisgn and Innovation day presentation
        private void DataModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_dataModel.demoModeVal))
            {
                if (_dataModel.demoModeVal)
                {
                    MHBall.Content = "DEMO";
                }
                else
                {
                    MHBall.Content = "Middle High Ball";
                }
            }
        }

        // Button to reset sequence to blanks 
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

        // Function will grow or shrink targetpoxes to indicate which set was selected in sequence bar at bottom of screen
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

        // Function grows the colours element of the set selection buttons so users on touch screen can easily see which set is currently picked

        public void animateSel(Rectangle targetBox, int size, float Time)
        {

            DoubleAnimation widthAnimation = new DoubleAnimation();
            widthAnimation.From = targetBox.Width;

            widthAnimation.To = size; // Set the final height of the rectangle
            widthAnimation.Duration = new Duration(TimeSpan.FromSeconds(Time / 1.5));
            Storyboard storyboardSel = new Storyboard();
            storyboardSel.Children.Add(widthAnimation);
            Storyboard.SetTarget(widthAnimation, targetBox);
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(System.Windows.Shapes.Rectangle.WidthProperty));
            storyboardSel.Begin();

        }

        // Update the sequence visually and logically if an update occurs
        public void seqUpdate()
        {
            int tempLen = sequence.Count;
            
            
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

           

            List<int> Zeros = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0 };

            for (int j = 0; j < tempLen; j++) {

                Zeros[j] += +sequence[j];
            }


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

            
        }

        // Reset the animations is reset button is selected
        private void resetSel(){
            animateSel(c1, 10, shrinkTime);
            animateSel(c2, 10, shrinkTime);
            animateSel(c3, 10, shrinkTime);
            animateSel(c4, 10, shrinkTime);
            animateSel(c5, 10, shrinkTime);
            animateSel(c6, 10, shrinkTime);
            animateSel(c7, 10, shrinkTime);
            animateSel(c8, 10, shrinkTime);
            animateSel(c9, 10, shrinkTime);
            animateSel(c10, 10, shrinkTime);
        }

        // If garbage icon is clicked, clear the current sequence visually and in stored array
        private void Garbage_Click(object sender, RoutedEventArgs e)
        {
            
            sequence.Clear();
            motorSpeed.Clear();
            linAct.Clear();
            
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

        // If back arrow icon is clicked, get rid of the last element in sequwnce list and visually undo animation
        private void backArrow_Click(object sender, RoutedEventArgs e)
        {
            int tempSeqCount = sequence.Count;
            if (tempSeqCount == 0)
            {
                return;
            }
            else {
                sequence.RemoveAt(sequence.Count - 1);
                motorSpeed.RemoveAt(motorSpeed.Count - 1);
                linAct.RemoveAt(linAct.Count - 1);
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


        // Outdated function meant to draw the volleyball's trajectory
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

        // Move the ball sprite to specified location on canvas
        private void moveBall(int newX, int newY)
        {
            Canvas.SetLeft(volleyballIcon, newX - 25);
            Canvas.SetTop(volleyballIcon, newY - 25);
        }

        // Function creates a bezier curve using start, end and two control points to simulate ball trajectory.
        //      Arc is placed over volleyball net image and behind volleyball sprite
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

            // Nature of bezier curve meant of end of arc was to the right or left of start point the behaviour would not work correctly,
            //      so if the end is futher to the right than the start, the start and end values are swapped behind the scenes, visually looks the same

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

        // Demo mode added for design and innovation day limiting power amd aim to designated target location
        private void HandleDemoButtonCheckedChanged(object sender, bool isChecked)
        {
            if (isChecked)
            {
                // Do something when the toggle button is checked
                MHBall.Content = "Middle High Ball (Demo)";
            }
            else
            {
                // Do something when the toggle button is unchecked
                MHBall.Content = "Middle High Ball";
            }
        }

        // Coordinates the visual sequence elements depending on situation
        public void resetSeq()
        {
            for (int i = 0; i <= sequence.Count; i++)
            {
                switch (i)
                {
                    case 1:
                        animateSeq(s1, sizeGrow, shrinkTime);
                        break;
                    case 2:
                        animateSeq(s2, sizeGrow, shrinkTime);
                        break;
                    case 3:
                        animateSeq(s3, sizeGrow, shrinkTime);
                        break;
                    case 4:
                        animateSeq(s4, sizeGrow, shrinkTime);
                        break;
                    case 5:
                        animateSeq(s5, sizeGrow, shrinkTime);
                        break;
                    case 6:
                        animateSeq(s6, sizeGrow, shrinkTime);
                        break;
                    case 7:
                        animateSeq(s7, sizeGrow, shrinkTime);
                        break;
                    case 8:
                        animateSeq(s8, sizeGrow, shrinkTime);
                        break;
                }
            }
        }

        // Coordiantes arc creation based on whoch button was pressed, using empirical magic numbers for concrete sets and saved values for custom sets
        public void drawSeq(int index, int set)
        {
            switch (index) { 
                case 1:
                    DrawBezier(150, 225, 640, 400, 300, -100, 500, -100);

                    break;
                case 2:
                    DrawBezier(150, 225, 640, 400, 300, 150, 500, 150);
                    break;
                case 3:
                    DrawBezier(310, 225, 640, 400, 420, 150, 500, 150);
                    break;
                case 4:
                    DrawBezier(480, 225, 640, 400, 530, -50, 550, -50);
                    break;
                case 5:
                    DrawBezier(480, 225, 640, 400, 530, 225, 550, 225);
                    break;
                case 6:
                    DrawBezier(740, 225, 640, 400, 680, 150, 720, 150);
                    break;
                case 7:
                    DrawBezier(820, 225, 640, 400, 730, -100, 750, -100);
                    break;
                case 8:
                    custom = true;
                    DrawBezier((_dataModel.xSliderValue1 + defaultStartx), defaultStarty, defaultEndx, defaultEndy, _dataModel.c1Controlx1, _dataModel.c1Controly1, _dataModel.c1Controlx2, _dataModel.c1Controly2);

                    break;
                case 9:
                    custom = true;
                    DrawBezier((_dataModel.xSliderValue2 + defaultStartx), defaultStarty, defaultEndx, defaultEndy, _dataModel.c2Controlx1, _dataModel.c2Controly1, _dataModel.c2Controlx2, _dataModel.c2Controly2);

                    break;
                case 10:
                    custom = true;
                    DrawBezier((_dataModel.xSliderValue3 + defaultStartx), defaultStarty, defaultEndx, defaultEndy, _dataModel.c3Controlx1, _dataModel.c3Controly1, _dataModel.c3Controlx2, _dataModel.c3Controly2);

                    break;
            }

            

            switch (set)
            {
                case 1:
                    animateSeq(s1, sizeGrow+30, growTime/2);
                    break;
                case 2:
                    animateSeq(s1, sizeGrow, shrinkTime);
                    
                    animateSeq(s2, sizeGrow + 30, growTime / 2);
                    break;
                case 3:
                    animateSeq(s2, sizeGrow, shrinkTime);
                    animateSeq(s3, sizeGrow + 30, growTime / 2);
                    break;
                case 4:
                    animateSeq(s3, sizeGrow, shrinkTime);
                    animateSeq(s4, sizeGrow + 30, growTime / 2);
                    break;
                case 5:
                    animateSeq(s4, sizeGrow, shrinkTime);
                    animateSeq(s5, sizeGrow + 30, growTime / 2);
                    break;
                case 6:
                    animateSeq(s5, sizeGrow, shrinkTime);
                    animateSeq(s6, sizeGrow + 30, growTime / 2);
                    break;
                case 7:
                    animateSeq(s6, sizeGrow, shrinkTime);
                    animateSeq(s7, sizeGrow + 30, growTime / 2);
                    break;
                case 8:
                    animateSeq(s7, sizeGrow, shrinkTime);
                    animateSeq(s8, sizeGrow + 30, growTime / 2);
                    break;

            }


        }


        // Events if Power High Ball button is pressed
        private void PHBall_Click(object sender, RoutedEventArgs e)
        {
            if (buttonActive[1] == false)
            {
                resetSel();
                buttonActive[1] = true;
                activeButtonReset(1);
                MainText.Content = PHBall.Content;
                DrawBezier(150, 225, 640, 400, 300, -100, 500, -100);
                animateSel(c1, 50, growTime);
            }
            else
            {
                double y = defaultEndy - -100;
                double x = defaultEndx - 500;
                double customLinAct = (x / y * 30 + 14);
                Debug.WriteLine((int)customLinAct);
                if (customLinAct > 25)
                {
                    customLinAct = 25;
                }
                else if (customLinAct < 5)
                {
                    customLinAct = 5;
                }
                sequence.Add(1);
                motorSpeed.Add(50);
                linAct.Add((int)customLinAct);

                seqUpdate();
            }
        }
        
        // Events if Power Shoot button is pressed

        private void PShoot_Click(object sender, RoutedEventArgs e)
        {
            if (buttonActive[2] == false)
            {
                resetSel();
                buttonActive[2] = true;
                activeButtonReset(2);
                MainText.Content = PShoot.Content;
                DrawBezier(150, 225, 640, 400, 300, 150, 500, 150);
                animateSel(c2, 50, growTime);
            }
            else
            {
                double y = defaultEndy - 150;
                double x = defaultEndx - 500;
                double customLinAct = (x / y * 30 + 14);
                Debug.WriteLine((int)customLinAct);
                if (customLinAct > 25)
                {
                    customLinAct = 25;
                }
                else if (customLinAct < 5)
                {
                    customLinAct = 5;
                }
                sequence.Add(2);
                seqUpdate();
                motorSpeed.Add(54);
                linAct.Add((int)customLinAct);
            }
        }

        // Events if Thirty Three button is pressed

        private void ThirtyThree_Click(object sender, RoutedEventArgs e)
        {
            if (buttonActive[3] == false)
            {
                resetSel();
                buttonActive[3] = true;
                activeButtonReset(3);
                MainText.Content = ThirtyThree.Content;
                DrawBezier(310, 225, 640, 400, 420, 150, 500, 150);
                animateSel(c3, 50, growTime);
            }
            else
            {
                double y = defaultEndy - 150;
                double x = defaultEndx - 500;
                double customLinAct = (x / y * 30 + 14);
                Debug.WriteLine((int)customLinAct);
                if (customLinAct > 25)
                {
                    customLinAct = 25;
                }
                else if (customLinAct < 5)
                {
                    customLinAct = 5;
                }
                sequence.Add(3);
                seqUpdate();
                motorSpeed.Add(50);
                linAct.Add((int)customLinAct);
            }
        }
        
        // Events if Middle High Ball button is pressed
        //      Special instructions for demo mode
        private void MHBall_Click(object sender, RoutedEventArgs e)
        {
            if (buttonActive[4] == false)
            {
                resetSel();
                buttonActive[4] = true;
                activeButtonReset(4);
                MainText.Content = MHBall.Content;
                if (_dataModel.demoModeVal)
                {
                    DrawBezier(310, 225, 640, 400, 420, -50, 500, -50);
                }
                else
                {
                    DrawBezier(480, 225, 640, 400, 530, -50, 550, -50);
                }
                animateSel(c4, 50, growTime);
            }
            else
            {
                double y = defaultEndy - -50;
                double x = defaultEndx - 500;
                double customLinAct = (x / y * 30 + 14);
                Debug.WriteLine((int)customLinAct);
                if (customLinAct > 25)
                {
                    customLinAct = 25;
                }
                else if (customLinAct < 5)
                {
                    customLinAct = 5;
                }
                sequence.Add(4);
                seqUpdate();
                motorSpeed.Add(50);
                linAct.Add((int)customLinAct);
            }
        }


        // Events if Middle Quick button is pressed
        private void MQuick_Click(object sender, RoutedEventArgs e)
        {
            if (buttonActive[5] == false)
            {
                resetSel();
                buttonActive[5] = true;
                activeButtonReset(5);
                MainText.Content = MQuick.Content;
                DrawBezier(480, 225, 640, 400, 530, 225, 550, 225);
                animateSel(c5, 50, growTime);
            }
            else
            {
                double y = defaultEndy - 225;
                double x = defaultEndx - 550;
                double customLinAct = (x / y * 30 + 14);
                Debug.WriteLine((int)customLinAct);
                if (customLinAct > 25)
                {
                    customLinAct = 25;
                }
                else if (customLinAct < 5)
                {
                    customLinAct = 5;
                }
                sequence.Add(5);
                seqUpdate();
                motorSpeed.Add(50);
                linAct.Add((int)customLinAct);
            }

        }

        // Events if Middle Slide button is pressed
        private void MSlide_Click(object sender, RoutedEventArgs e)
        {
            if (buttonActive[6] == false)
            {
                resetSel();
                buttonActive[6] = true;
                activeButtonReset(6);
                DrawBezier(740, 225, 640, 400, 680, 150, 720, 150);
                MainText.Content = MSlide.Content;
                animateSel(c6, 50, growTime);
            }
            else
            {
                double y = defaultEndy - 150;
                double x = defaultEndx - 680;
                double customLinAct = (x / y * 30 + 14);
                Debug.WriteLine((int)customLinAct);
                if (customLinAct > 25)
                {
                    customLinAct = 25;
                }
                else if (customLinAct < 5)
                {
                    customLinAct = 5;
                }
                sequence.Add(6);
                seqUpdate();
                motorSpeed.Add(50);
                linAct.Add((int)customLinAct);
            }
        }

        // Events if Offside High Ball button is pressed
        private void OHBall_Click(object sender, RoutedEventArgs e)
        {
            if (buttonActive[7] == false)
            {
                resetSel();
                buttonActive[7] = true;
                activeButtonReset(7);
                DrawBezier(820, 225, 640, 400, 730, -100, 750, -100);
                MainText.Content = OHBall.Content;
                animateSel(c7, 50, growTime);
            }
            else
            {
                double y = defaultEndy - -100;
                double x = defaultEndx - 730;
                double customLinAct = (x / y * 30 + 14);
                Debug.WriteLine((int)customLinAct);
                if (customLinAct > 25)
                {
                    customLinAct = 25;
                }
                else if (customLinAct < 5)
                {
                    customLinAct = 5;
                }
                sequence.Add(7);
                seqUpdate();
                motorSpeed.Add(50);
                linAct.Add((int)customLinAct);
            }
        }

        // Events if Custom 1 button is pressed
        private void Custom1_Click(object sender, RoutedEventArgs e)
        {
            //custom = false;
            if (buttonActive[8] == false)
            {
                resetSel();
                buttonActive[8] = true;
                activeButtonReset(8);
                custom = true;
                MainText.Content = "Custom 1";
                DrawBezier((_dataModel.xSliderValue1 + defaultStartx), defaultStarty, defaultEndx, defaultEndy, _dataModel.c1Controlx1, _dataModel.c1Controly1, _dataModel.c1Controlx2, _dataModel.c1Controly2);
                animateSel(c8, 50, growTime);
            }
            else
            {
                double y;
                double x;
                if ((_dataModel.xSliderValue1 + defaultStartx) > defaultEndx){
                    y = defaultEndy - _dataModel.c1Controly2;
                    x = defaultEndx - _dataModel.c1Controlx2;
                }
                else
                {
                    y = defaultEndy - _dataModel.c1Controly1;
                    x = defaultEndx - _dataModel.c1Controlx1;
                }
                
                double customLinAct = (x / y * 30 + 14);
                Debug.WriteLine((int)customLinAct);
                if (customLinAct > 25)
                {
                    customLinAct = 25;
                }
                else if (customLinAct < 5)
                {
                    customLinAct = 5;
                }
                motorSpeed.Add(50);
                linAct.Add((int)customLinAct);
                sequence.Add(8);
                seqUpdate();
            }
        }
    
        // Events if Custom 2 button is pressed
        private void Custom2_Click(object sender, RoutedEventArgs e)
        {
            if (buttonActive[9] == false)
            {
                resetSel();
                buttonActive[9] = true;
                activeButtonReset(9);
                custom = true;
                MainText.Content = "Custom 2";
                DrawBezier((_dataModel.xSliderValue2 + defaultStartx), defaultStarty, defaultEndx, defaultEndy, _dataModel.c2Controlx1, _dataModel.c2Controly1, _dataModel.c2Controlx2, _dataModel.c2Controly2);
                animateSel(c9, 50, growTime);
            }
            else
            {
                double y;
                double x;
                if ((_dataModel.xSliderValue1 + defaultStartx) > defaultEndx)
                {
                    y = defaultEndy - _dataModel.c2Controly2;
                    x = defaultEndx - _dataModel.c2Controlx2;
                }
                else
                {
                    y = defaultEndy - _dataModel.c2Controly1;
                    x = defaultEndx - _dataModel.c2Controlx1;
                }
                double customLinAct = (x / y * 30 + 14);
                Debug.WriteLine((int)customLinAct);
                if (customLinAct > 25)
                {
                    customLinAct = 25;
                }
                else if (customLinAct < 5)
                {
                    customLinAct = 5;
                }
                motorSpeed.Add(50);
                linAct.Add((int)customLinAct);
                sequence.Add(9);
                seqUpdate();
            }
        }

        // Events if Custom 3button is pressed
        private void Custom3_Click(object sender, RoutedEventArgs e)
        {
            if (buttonActive[10] == false)
            {
                resetSel();
                buttonActive[10] = true;
                activeButtonReset(10);
                MainText.Content = "Custom 3";
                custom = true;
                DrawBezier((_dataModel.xSliderValue3 + defaultStartx), defaultStarty, defaultEndx, defaultEndy, _dataModel.c3Controlx1, _dataModel.c3Controly1, _dataModel.c3Controlx2, _dataModel.c3Controly2);
                animateSel(c10, 50, growTime);
            }
            else
            {
                double y;
                double x;
                if ((_dataModel.xSliderValue1 + defaultStartx) > defaultEndx)
                {
                    y = defaultEndy - _dataModel.c3Controly2;
                    x = defaultEndx - _dataModel.c3Controlx2;
                }
                else
                {
                    y = defaultEndy - _dataModel.c3Controly1;
                    x = defaultEndx - _dataModel.c3Controlx1;
                }
                double customLinAct = (x / y * 30 + 14);
                Debug.WriteLine((int)customLinAct);
                if (customLinAct > 25)
                {
                    customLinAct = 25;
                }
                else if (customLinAct < 5)
                {
                    customLinAct = 5;
                }
                motorSpeed.Add(50);
                linAct.Add((int)customLinAct);
                sequence.Add(10);
                
                seqUpdate();
            }

        }

        // If settigns icon is clikced, a new window opens with various srttings and sliders dynamically showing custom sets
        private void SettingsPage_Click(object sender, RoutedEventArgs e)
        {
            Custom1.IsEnabled = true;
            Custom2.IsEnabled = true;
            Custom3.IsEnabled = true;
            SettingsView setView = new SettingsView(_dataModel);
            setView.Show();
        }

        // Once the start button is selected, go through process of initializing machien and setting commands as well as cycling through visualization of sets on screen
        private async void StartStop_Checked(object sender, RoutedEventArgs e)
        {   await Task.Delay(200);
            if (sequence.Count == 0){
                StartStop.IsChecked = false;
                return;
            }
            
            resetSel();
            resetSeq();
            backArrow.IsEnabled = false;
            Garbage.IsEnabled = false;
            SerialPort serialPort = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
            serialPort.Open();
        
            string data;
            string response = "";
            string demo = "0";
            if (_dataModel.demoModeVal == true)
            {
                demo = "6";
            }
            int timeout = (int)_dataModel.userDelay *1000;
            
            

            for (int i = 0; i < sequence.Count; i++)
            {
                if (StartStop.IsChecked == false)
                {
                    serialPort.Write("0," + homePoint + ".5");
                    serialPort.Close();
                    return;

                }
                drawSeq(sequence[i], i + 1);
                await Task.Delay(timeout);
                if (sequence[i] == 4 && demo == "6")
                {
                    data = motorSpeed[i].ToString() + "," + linAct[i].ToString() + ".1";
                    serialPort.Write(data);
                    await Task.Delay(2000);
                }
                
                data = motorSpeed[i].ToString() + "," + linAct[i].ToString()+"." + demo;
                serialPort.Write(data);

                
;
                DateTime start = DateTime.Now;
                while (true)
                {
                    response = serialPort.ReadLine();

                    if (response.Contains("Ready"))
                    {

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
                response = "";
            }
            serialPort.Write("0,"+homePoint+".5");
            serialPort.Close();
            StartStop.IsChecked = false;
            
        }

        private void StartStop_Unchecked(object sender, RoutedEventArgs e)
        {
            resetSeq();
            backArrow.IsEnabled = true;
            Garbage.IsEnabled = true;
        }
    }
}
