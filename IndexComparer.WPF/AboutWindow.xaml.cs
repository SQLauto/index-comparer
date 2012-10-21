using System;
using System.Collections.Generic;
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

namespace IndexComparer.WPF
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            btnClose.Focus();
        }

        protected void URL_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(URL.NavigateUri.ToString());
        }

        protected void Email_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Email.NavigateUri.ToString());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
