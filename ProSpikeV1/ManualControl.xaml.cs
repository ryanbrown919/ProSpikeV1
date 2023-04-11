using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Diagnostics;

namespace ProSpikeV1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ManualControl : Window
    {
        private SharedDataModel _dataModel;
        //public int linActVal;
        //public int motorSpeed;
        public String data;
        //public SerialPort serialPort = new SerialPort();
        public string[] ports = SerialPort.GetPortNames();
        public SerialPort serialPort = new SerialPort("COM5", 9600, Parity.None, 8, StopBits.One);
        public ManualControl(SharedDataModel dataModel)
        {
            InitializeComponent();
            _dataModel = dataModel;
            linAct.Value = 9;

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            
            /*if (SerialPort.GetPortNames().Contains("COM4"))
            {
                serialPort.PortName = "COM4";
                serialPort.BaudRate = 9600;
                serialPort.Open();
            }
            else if (SerialPort.GetPortNames().Contains("COM3"))
            {
                serialPort.PortName = "COM3";
                serialPort.BaudRate = 9600;
                serialPort.Open();
            }
            else if (SerialPort.GetPortNames().Contains("COM5"))
            {
                serialPort.PortName = "COM5";
                serialPort.BaudRate = 9600;
                serialPort.Open();
            }
            else
            {
                MessageBox.Show("Error");
            }*/
            serialPort.Open();
           

           // serialPort.BaudRate = 9600;
            //serialPort.Open();

            //linAct.Value = 14;

            //serialPort.Write(sequence[i].ToString());
            //await Task.Delay(delay);

            //}
            //catch(Exception ex)
            //{
            //MainText.Content=ex.Message;
            //}

            //serialPort.Write("0");


        }

        
        private void SetSliders_Click(object sender, RoutedEventArgs e)
        {
            data = motorSpeed.Value.ToString() + "," + (linAct.Value + 5).ToString() + "." + "5";

            Debug.WriteLine("Set SLiders: " + data); 
            serialPort.Write(data);
        }
        private void LoadBall_Click(object sender, RoutedEventArgs e)
        {
            data = motorSpeed.Value.ToString() + "," + (linAct.Value + 5).ToString() + "." + "1";
            Debug.WriteLine(data);
            serialPort.Write(data);
        }

        private void LaunchBall_Click(object sender, RoutedEventArgs e)
        {
            data = motorSpeed.Value.ToString() + "," + (linAct.Value + 5).ToString() + "." + "3";
            serialPort.Write(data);
        }
        private void ManualControl_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            serialPort.Close();
        }

        private void linAct_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
           // data = motorSpeed.Value.ToString() + "," + (linAct.Value + 5).ToString() + "." + "5";
            //serialPort.Write(data);
            linActTextBox.Text = $"Linear Actuator Stroke: {linAct.Value + 5}cm";
            //Thread.Sleep(100);

        }

        private void motorSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //data = motorSpeed.Value.ToString() + "," +  (linAct.Value + 5).ToString() + "." + "5";
            //serialPort.Write(data);
            //Thread.Sleep(100);
            //Task async wait(100);
        }
    }
}
