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

namespace ProSpikeV1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        
        public MainWindow()
        {
            InitializeComponent();

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void PHBall_Click(object sender, RoutedEventArgs e)
        {
            MainText.Content = PHBall.Content;
        }

        private void PShoot_Click(object sender, RoutedEventArgs e)
        {
            MainText.Content = PShoot.Content;
        }

        private void MHBall_Click(object sender, RoutedEventArgs e)
        {
            MainText.Content = MHBall.Content;
        }

        private void MQuick_Click(object sender, RoutedEventArgs e)
        {
            MainText.Content = MQuick.Content;
        }

        private void MSlide_Click(object sender, RoutedEventArgs e)
        {
            MainText.Content = MSlide.Content;
        }

        private void OHBall_Click(object sender, RoutedEventArgs e)
        {
            MainText.Content = OHBall.Content;
        }

        private void Custom_Click(object sender, RoutedEventArgs e)
        {
            MainText.Content = Custom.Content;
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
           
            SettingsView setView = new SettingsView();
            setView.Show();
        }

        private void BackrowPipe_Click(object sender, RoutedEventArgs e)
        {
            MainText.Content = BackrowPipe.Content;
        }

        private void BackrowC_Click(object sender, RoutedEventArgs e)
        {
            MainText.Content = BackrowC.Content;
        }

        private void ThirtyThree_Click(object sender, RoutedEventArgs e)
        {
            MainText.Content = ThirtyThree.Content;
        }
    }
}
