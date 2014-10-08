using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MathProcessor
{
    /// <summary>
    /// Interaction logic for GamentryAd.xaml
    /// </summary>
    public partial class GamentryAd : Window
    {
        public GamentryAd()
        {
            InitializeComponent();
        }

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.Gamentry.com");
            this.Close();
        }

        private void DoNotShow_Checked(object sender, RoutedEventArgs e)
        {
            ConfigManager.ShowAd = false;
        }

        private void DoNotShow_UnChecked(object sender, RoutedEventArgs e)
        {
            ConfigManager.ShowAd = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("http://www.Gamentry.com");
            this.Close();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("http://www.Gamentry.com");
            this.Close();
        }
    }
}
