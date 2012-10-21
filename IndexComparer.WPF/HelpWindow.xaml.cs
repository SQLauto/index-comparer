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
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
            tbHelp.Text = @"The Index Comparer compares indexes between two databases:  a 'Primary' and a 'Secondary' database.  It doesn't matter which you select as Primary and Secondary, however; all comparisons will work the same.

Once you enter an Instance name, the Database drop-down list will populate with a list of databases on that instance.  Do that for both sides and then press the 'GO!' button to display a grid with comparisons.

Options:
    * Show Only Differences?  If selected, the grid will include only indexes which fulfill one of the following three conditions:
        - Only exist on the Primary database
        - Only exist on the Secondary database
        - Exist in both databases but differ in some regard
    * Group By Table?  If selected, the grid will include groups for individual tables.  This is recommended when there are a large number of tables and indexes.

The grid allows you to export the results to a text file for later viewing.  There are three options:
    * Script Creates?  This also includes a drop statement in case the index already exists in your destination database.  Indexes will be rebuilt online whenever possible--this depends upon whether you have an appropriate version of SQL Server and whether the index may be rebuilt online.
    * Script Separate Drops?  This lets you script individual drop statements.
    * Write Out Section Headers?  This lets you include or get rid of section headers in the export file.

Each row in the grid has three buttons:  a DROP and two CREATEs.  If the index exists on the Primary database, the Primary button will be enabled; otherwise, it will be disabled.  This applies to the Secondary database button as well.  Clicking on each of these buttons will pop up a new window with the relevant script.  The Primary database's CREATE button will create an index which matches the current index on the Primary databases.  If the 'index' is actually a heap, the button will give you a message indicating so.

In addition, there are a few columns for index details, including the index name, type, and whether it is unique.  If there is a difference between the Primary and Secondary implementations of an index, the 'Diff?' box will be checked.  If the index type or uniqueness differs, you will see a coded version so it can fit into the allotted space.

Clicking on a row displays row details, which let you compare the columns, included columns, and filters for each index.";

            btnClose.Focus();
        }

        protected void Help_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
