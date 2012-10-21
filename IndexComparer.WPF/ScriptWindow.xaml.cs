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
    /// Interaction logic for ScriptWindow.xaml
    /// </summary>
    public partial class ScriptWindow : Window
    {
        public ScriptWindow()
        {
            InitializeComponent();
        }

        public ScriptWindow(string ScriptType, string ScriptText)
        {
            InitializeComponent();
            this.Title = ScriptType;
            txtScript.Text = ScriptText;

            txtScript.Focus();
            txtScript.SelectAll();
        }

        protected void CloseWindow(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
