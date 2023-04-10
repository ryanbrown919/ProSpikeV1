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
using System.ComponentModel;
using static ProSpikeV1.MainWindow;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;

namespace ProSpikeV1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
            
    public partial class SettingsView : System.Windows.Window
    {
        double defaultStartx = 150;
        double defaultStarty = 225;
        double defaultEndx = 640;
        double defaultEndy = 400;
        double defaultControlx1 = 300;
        double defaultControly1 = -100;
        double defaultControlx2 = 300;
        double defaultControly2 = -100;
        double newControlx1 = 300;
        double newControlx2 = -100;
        double newControly1 = 300;
        double newControly2 = -100;
        double xVal;
        private bool _ignoreFirstSelectionChange = true;
        private SharedDataModel _dataModel;
        //private SharedDataModel sharedData = new SharedDataModel();
        public SettingsView(SharedDataModel dataModel)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _dataModel = dataModel;
            timerSlider.Value = 3;
            //_dataModel = new SharedDataModel();
            //xSlider.Value = _dataModel.xSliderValue1;
            //ySlider.Value = _dataModel.ySliderValue1;
            //CustomSetBox.SelectedIndex = _dataModel.SelectedComboBoxIndex;
            //settingsLabel.Content = _dataModel.xSliderValue1.ToString();
            Debug.WriteLine($"xSlider.Value set to {_dataModel.xSliderValue1}");
            
            //_dataModel.xSliderValue1 = 600;
            Debug.WriteLine($"xSlider.Value set to {_dataModel.xSliderValue1}");
            //switch (CustomSetBox.SelectedIndex)
            //{
            //    case 0:
            //        xSlider.Value = _dataModel.xSliderValue1;
            //        ySlider.Value = _dataModel.ySliderValue1;
            //        _dataModel.custom1 = true;
            //        //_dataModel.AreButtonsEnabled = true ;
            //        break;
            //    case 1:
            //        xSlider.Value = _dataModel.xSliderValue2;
            //        ySlider.Value = _dataModel.ySliderValue2;
            //        _dataModel.custom2 = true;
            //        break;
            //    case 2:
            //        xSlider.Value = _dataModel.xSliderValue3;
            //        ySlider.Value = _dataModel.ySliderValue3;
            //        _dataModel.custom3 = true;
            //        break;
            //        // Add more cases as needed for each ComboBox item
            //}
            //xSlider.Value = _dataModel.xSliderValue1;
            //ySlider.Value = _dataModel.ySliderValue1;
            timerSlider.Value = _dataModel.userDelay;
            //DrawBezier(150, 225, 640, 400, 300, -100, 500, -100); Default Power High Ball confugurations
            xSlider.Value += 1;
            ySlider.Value += 1; 
            xSlider.Value -= 1;
            ySlider.Value -= 1;
            //DrawBezier(defaultStartx, defaultStarty, defaultEndx, defaultEndy, defaultControlx1, defaultControly1, defaultControlx2, defaultControly2); 
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            Debug.WriteLine("Window Activated");
            switch (_dataModel.SelectedComboBoxIndex)
            {
                case 0:
                    xSlider.Value = _dataModel.xSliderValue1;
                    ySlider.Value = _dataModel.ySliderValue1;
                    break;
                case 1:
                    xSlider.Value = _dataModel.xSliderValue2;
                    ySlider.Value = _dataModel.ySliderValue2;
                    break;
                case 2:
                    xSlider.Value = _dataModel.xSliderValue3;
                    ySlider.Value = _dataModel.ySliderValue3;
                    break;

            }
            CustomSetBox.SelectedIndex = _dataModel.SelectedComboBoxIndex;
        }
        private void backArrowSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void moveBall(double newX, double newY)
        {
            Canvas.SetLeft(volleyballIcon, newX - 25);
            Canvas.SetTop(volleyballIcon, newY - 25);
        }

        public void ySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            xVal = xSlider.Value + defaultStartx;
            newControly1 = defaultControly1 + (325 - ySlider.Value);
            newControly2 = defaultControly2 + (325 - ySlider.Value);
            newControlx1 = defaultEndx - (defaultEndx - xVal) * 1 / 3 ;
            newControlx2 = defaultEndx - (defaultEndx - xVal) * 2 / 3 ;
            DrawBezier(xVal, defaultStarty, defaultEndx, defaultEndy, newControlx1, newControly1, newControlx2, newControly2);
            //_dataModel.ySliderValue = (int)ySlider.Value;
            //CustomSet_SelectionChanged(null, null);
            //CustomSet_UpdateModel((int)xSlider.Value, (325 - (int)ySlider.Value), Convert.ToInt32(newControlx1), Convert.ToInt32(newControly1), Convert.ToInt32(newControlx2), Convert.ToInt32(newControly2));
        }


        public void xSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            

            xVal = xSlider.Value + defaultStartx;
            Canvas.SetLeft(volleyballIcon, xVal);
            
            newControlx1 = defaultEndx - (defaultEndx - xVal) * 1 / 3;
            newControlx2 = defaultEndx - (defaultEndx - xVal) * 2 / 3;
            
            newControly1 = defaultControly1 + (325 - ySlider.Value);
            newControly1 = defaultControly1 + (325 - ySlider.Value);
            DrawBezier(xVal, defaultStarty, defaultEndx, defaultEndy, newControlx1, newControly1, newControlx2, newControly2);
            //_dataModel.xSliderValue = (int)xSlider.Value;
            //CustomSet_SelectionChanged(null, null);
            //CustomSet_UpdateModel((int)xSlider.Value, (325 - (int)ySlider.Value), Convert.ToInt32(newControlx1), Convert.ToInt32(newControly1), Convert.ToInt32(newControlx2), Convert.ToInt32(newControly2));
        }
        private void DrawBezier(double startx, double starty, double endx, double endy, double controlx1, double controly1, double controlx2, double controly2)
        {
            double tempx, tempy, temp2x, temp2y;
            double moveX = startx;
            double moveY = starty;
            if (myCanvas.Children.Count > 0)
            {
                UIElement lastElement = myCanvas.Children[myCanvas.Children.Count - 1];
                if (lastElement is Path path && path.Tag as string == "bezier")
                    myCanvas.Children.RemoveAt(myCanvas.Children.Count - 1);
            }

            if (startx < endx)
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
            else {
                temp2x = controlx1;
                temp2y = controly1;
                controly1 = controly2;
                controlx1 = controlx2;
                controlx2 = temp2x;
                controly2 = temp2y;
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
        
        public void CustomSet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_ignoreFirstSelectionChange)
            {
                _ignoreFirstSelectionChange = false;
                return;
            }
            CustomSet_UpdateModel((int)xSlider.Value, ((int)ySlider.Value), Convert.ToInt32(newControlx1), Convert.ToInt32(newControly1), Convert.ToInt32(newControlx2), Convert.ToInt32(newControly2));
            switch (CustomSetBox.SelectedIndex)
            {
                case 0:
                    xSlider.Value = _dataModel.xSliderValue1;
                    ySlider.Value = _dataModel.ySliderValue1;
                    _dataModel.custom1 = true;
                    //_dataModel.AreButtonsEnabled = true ;
                    break;
                case 1:
                    xSlider.Value = _dataModel.xSliderValue2;
                    ySlider.Value = _dataModel.ySliderValue2;
                    _dataModel.custom2 = true;
                    break;
                case 2:
                    xSlider.Value = _dataModel.xSliderValue3;
                    ySlider.Value = _dataModel.ySliderValue3;
                    _dataModel.custom3 = true;
                    break;
                    // Add more cases as needed for each ComboBox item
            }
            _dataModel.SelectedComboBoxIndex = CustomSetBox.SelectedIndex;
        }
        
        private void CustomSet_UpdateModel(int valx, int valy, int newControlx1, int newControly1, int newControlx2, int newControly2)
        {
            switch (_dataModel.SelectedComboBoxIndex)
            {
                case 0:
                    _dataModel.xSliderValue1 = valx;
                    _dataModel.ySliderValue1 = valy;
                    _dataModel.c1Controlx1 = newControlx1;
                    _dataModel.c1Controly1 = newControly1;
                    _dataModel.c1Controlx2 = newControlx2;
                    _dataModel.c1Controly2 = newControly2;
                    break;
                case 1:
                    _dataModel.xSliderValue2 = valx;
                    _dataModel.ySliderValue2 = valy;
                    _dataModel.c2Controlx1 = newControlx1;
                    _dataModel.c2Controly1 = newControly1;
                    _dataModel.c2Controlx2 = newControlx2;
                    _dataModel.c2Controly2 = newControly2;
                    break;
                case 2:
                    _dataModel.xSliderValue3 = valx;
                    _dataModel.ySliderValue3 = valy;
                    _dataModel.c3Controlx1 = newControlx1;
                    _dataModel.c3Controly1 = newControly1;
                    _dataModel.c3Controlx2 = newControlx2;
                    _dataModel.c3Controly2 = newControly2;
                    break;
                    // Add more cases as needed for each ComboBox item
            }

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void manualControl_Click(object sender, RoutedEventArgs e)
        {
            ManualControl manual = new ManualControl(_dataModel);
            manual.Show();
        }

        public void timerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            timerBox.Content = $"Time between sets: {timerSlider.Value + 7} seconds";
            
            _dataModel.userDelay = timerSlider.Value;
        }
        private void SettingsView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _dataModel.SelectedComboBoxIndex = CustomSetBox.SelectedIndex;
            _ignoreFirstSelectionChange = true;
            CustomSet_UpdateModel((int)xSlider.Value, ((int)ySlider.Value), Convert.ToInt32(newControlx1), Convert.ToInt32(newControly1), Convert.ToInt32(newControlx2), Convert.ToInt32(newControly2));
        }

        

        private void demoMode_Checked(object sender, RoutedEventArgs e)
        {
            _dataModel.demoModeVal = (bool)demoMode.IsChecked;
        }

        private void demoMode_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void mensNet_Unchecked(object sender, RoutedEventArgs e)
        {
            womensNet.IsChecked = true;
            _dataModel.netHeight = 3;
        }

        private void mensNet_Checked(object sender, RoutedEventArgs e)
        {
            womensNet.IsChecked = false;
        }

        private void womensNet_Unchecked(object sender, RoutedEventArgs e)
        {
            mensNet.IsChecked = true;
        }

        private void womensNet_Checked(object sender, RoutedEventArgs e)
        {
            mensNet.IsChecked = false;
            _dataModel.netHeight = 0;
        }
    }
}
