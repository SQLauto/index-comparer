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
using System.Windows.Navigation;
using System.Windows.Shapes;
using IndexComparer.BusinessObjects;
using System.ComponentModel;

namespace IndexComparer.WPF
{
    #region Helper classes for passing variables around between threads.

    public class MainWindowValues
    {
        public string CurrentPrimaryInstanceName { get; set; }
        public string CurrentSecondaryInstanceName { get; set; }
        public string CurrentPrimaryDatabaseName { get; set; }
        public string CurrentSecondaryDatabaseName { get; set; }
        public bool ShowOnlyDifferences { get; set; }
        public bool DisplayGroups { get; set; }
        public bool ForceRefresh { get; set; }
        public bool DatabaseListingChanged { get; set; }
    }

    #endregion

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Parts

        private IEnumerable<IndexSet> PrimaryGroup = null;
        private IEnumerable<IndexSet> SecondaryGroup = null;
        private IEnumerable<IndexGroup> Groups = null;
        private string CurrentPrimaryInstanceName = null;
        private string CurrentSecondaryInstanceName = null;
        private string CurrentPrimaryDatabaseName = null;
        private string CurrentSecondaryDatabaseName = null;
        private bool ReadyToRock = false;

        private bool DatabaseListingChanged
        {
            get
            {
                return CurrentPrimaryInstanceName == txtPrimaryInstance.Text &&
                    CurrentSecondaryInstanceName == txtSecondaryInstance.Text &&
                    CurrentPrimaryDatabaseName == cmbPrimaryDatabase.SelectedValue.ToString() &&
                    CurrentSecondaryDatabaseName == cmbSecondaryDatabase.SelectedValue.ToString();
            }
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            HelpCommand.InputGestures.Add(new KeyGesture(Key.F1));
        }

        #region Asynchronous Behavior:  Grid Population

        private void PopulateGrid(bool ForceRefresh)
        {
            //Show the waiting page.
            SetVisibility(PageToShow.Waiting);
            
            //Set the async values
            MainWindowValues mwvSetup = LoadMainWindowValues(ForceRefresh);

            System.Windows.Threading.Dispatcher d = this.Dispatcher;

            //Set up a worker
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                try
                {
                    MainWindowValues mwv = e.Argument as MainWindowValues;

                    if (mwv.ForceRefresh || mwv.DatabaseListingChanged || PrimaryGroup == null || SecondaryGroup == null)
                    {
                        PrimaryGroup = IndexSet.RetrieveIndexData(mwv.CurrentPrimaryInstanceName, mwv.CurrentPrimaryDatabaseName);
                        SecondaryGroup = IndexSet.RetrieveIndexData(mwv.CurrentSecondaryInstanceName, mwv.CurrentSecondaryDatabaseName);
                        Groups = IndexGroup.PopulateIndexGroups(PrimaryGroup, SecondaryGroup);
                    }

                    //Now update the display
                    RefreshGridDelegate refresh = new RefreshGridDelegate(RefreshDataGrid);
                    d.BeginInvoke(refresh, mwv, Groups);
                }
                catch (Exception ex)
                {
                    ShowErrorDelegate error = new ShowErrorDelegate(ShowErrorMessage);
                    d.BeginInvoke(error, ex.Message);
                }
            };
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.RunWorkerAsync(mwvSetup);
        }

        private delegate void RefreshGridDelegate(MainWindowValues mwv, IEnumerable<IndexGroup> groups);
        private void RefreshDataGrid(MainWindowValues mwv, IEnumerable<IndexGroup> groups)
        {
            try
            {
                DataContext = new MainWindowViewModel
                                            (
                                                Groups,
                                                mwv.ShowOnlyDifferences,
                                                mwv.DisplayGroups
                                            );
                SetColumnVisibility(!mwv.DisplayGroups);

                //Hide the wait screen & show the display screen.
                SetVisibility(PageToShow.Results);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private delegate void ShowErrorDelegate(string ErrorMessage);
        private void ShowErrorMessage(string ErrorMessage)
        {
            SetVisibility(PageToShow.Errors);
            tbErrors.Text = "An error occurred while attempting to pull in your data:" + Environment.NewLine;
            tbErrors.Text += ErrorMessage;
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //We don't need to do anything here because there is a delegate calling the UI update method.
        }

        #endregion

        #region UI Helpers And Validation

        private void LoadComboBox(ComboBox BoxToLoad, string InstanceName)
        {
            if (String.IsNullOrWhiteSpace(InstanceName))
            {
                BoxToLoad.ItemsSource = null;
                SetVisibility(PageToShow.Instructions);
            }
            else if (InstanceName == CurrentPrimaryInstanceName)
            {
                //Don't need to reload the list.
                return;
            }
            else
            {
                //We need to reload the list, so hide any grids or errors that might be there already.
                SetVisibility(PageToShow.Waiting);

                UpdateDatabaseComboBoxDelegate dg = new UpdateDatabaseComboBoxDelegate(GetDatabaseListingByInstanceName);
                System.Windows.Threading.Dispatcher d = this.Dispatcher;

                //Set up a worker
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += delegate(object sender, DoWorkEventArgs e)
                {
                    string Instance = e.Argument.ToString();

                    try
                    {
                        //Populate the dropdown list.
                        IEnumerable<Database> Databases = Database.GetByServerName(InstanceName);
                        Dispatcher.Invoke(dg, BoxToLoad, Databases);
                        
                        //Now update the display
                        UpdateDisplayDelegate refresh = new UpdateDisplayDelegate(SetVisibility);
                        d.BeginInvoke(refresh, PageToShow.Instructions);
                    }
                    catch (Exception ex)
                    {
                        ShowErrorDelegate error = new ShowErrorDelegate(ShowErrorMessage);
                        d.BeginInvoke(error, ex.Message);
                    }
                };
                worker.RunWorkerAsync(InstanceName);
            }
        }

        private delegate void UpdateDisplayDelegate(PageToShow p);
        private delegate void UpdateDatabaseComboBoxDelegate(ComboBox BoxToLoad, IEnumerable<Database> Databases);
        private void GetDatabaseListingByInstanceName(ComboBox BoxToLoad, IEnumerable<Database> Databases)
        {
            BoxToLoad.ItemsSource = Databases;
        }

        private MainWindowValues LoadMainWindowValues(bool ForceRefresh)
        {
            MainWindowValues mwv = new MainWindowValues();
            mwv.CurrentPrimaryInstanceName = txtPrimaryInstance.Text;
            mwv.CurrentPrimaryDatabaseName = cmbPrimaryDatabase.SelectedValue.ToString();
            mwv.CurrentSecondaryInstanceName = txtSecondaryInstance.Text;
            mwv.CurrentSecondaryDatabaseName = cmbSecondaryDatabase.SelectedValue.ToString();
            mwv.DisplayGroups = chkDisplayGroups.IsChecked.Value;
            mwv.ForceRefresh = ForceRefresh;
            mwv.ShowOnlyDifferences = chkShowOnlyDifferences.IsChecked.Value;
            mwv.DatabaseListingChanged = DatabaseListingChanged;

            return mwv;
        }

        private void SetColumnVisibility(bool ShowColumns)
        {
            //If we are grouping, we don't need to include the column which lists Schema+Table names.
            if (ShowColumns)
            {
                dgResults.Columns[4].Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                dgResults.Columns[4].Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private enum PageToShow
        {
            Errors = 1,
            Results = 2,
            Instructions = 3,
            Waiting = 4
        }

        private void SetVisibility(PageToShow p)
        {
            cErrors.Visibility = p == PageToShow.Errors ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            cResults.Visibility = p == PageToShow.Results ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            cInstructions.Visibility = p == PageToShow.Instructions ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            cWaiting.Visibility = p == PageToShow.Waiting ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            gMain.Height = p == PageToShow.Results ? 680 : 300;
        }

        private List<string> ValidationErrors()
        {
            List<string> validationErrors = new List<string>();

            if (String.IsNullOrWhiteSpace(txtPrimaryInstance.Text))
                validationErrors.Add("You are missing a primary SQL Server instance name.");
            if (cmbPrimaryDatabase.SelectedValue == null || String.IsNullOrWhiteSpace(cmbPrimaryDatabase.SelectedValue.ToString()))
                validationErrors.Add("You are missing a primary SQL Server database name.");
            if (String.IsNullOrWhiteSpace(txtSecondaryInstance.Text))
                validationErrors.Add("You are missing a secondary SQL Server instance name.");
            if (cmbSecondaryDatabase.SelectedValue == null || String.IsNullOrWhiteSpace(cmbSecondaryDatabase.SelectedValue.ToString()))
                validationErrors.Add("You are missing a secondary SQL Server database name.");

            return validationErrors;
        }

        #endregion

        #region Events

        #region Menu Events

        protected void Menu_Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public static RoutedCommand HelpCommand = new RoutedCommand();

        protected void Menu_Help(object sender, RoutedEventArgs e)
        {
            ShowHelp();
        }

        protected void Menu_Help_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        protected void Menu_Help_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ShowHelp();
        }

        private void ShowHelp()
        {
            HelpWindow hw = new HelpWindow();
            hw.Show();
        }

        protected void Menu_About(object sender, RoutedEventArgs e)
        {
            AboutWindow aw = new AboutWindow();
            aw.Show();
        }

        #endregion

        private void PrimaryInstanceLostFocus(object sender, EventArgs e)
        {
            LoadComboBox(cmbPrimaryDatabase, txtPrimaryInstance.Text);
        }

        private void SecondaryInstanceLostFocus(object sender, EventArgs e)
        {
            LoadComboBox(cmbSecondaryDatabase, txtSecondaryInstance.Text);
        }

        private void btnRunComparison_Click(object sender, RoutedEventArgs e)
        {
            List<string> validationErrors = ValidationErrors();
            if (validationErrors.Count == 0)
            {
                try
                {
                    PopulateGrid(true);
                    ReadyToRock = true;
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(ex.Message);
                }
            }
            else
            {
                SetVisibility(PageToShow.Errors);

                tbErrors.Text = String.Empty;
                foreach (string error in validationErrors)
                {
                    tbErrors.Text += "*" + error + Environment.NewLine;
                }
            }
        }

        void OptionChanged(object sender, RoutedEventArgs e)
        {
            if (ReadyToRock)
                PopulateGrid(false);
        }

        void ExportToTextFile(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog d = new Microsoft.Win32.SaveFileDialog();
            d.DefaultExt = ".txt";
            d.Filter = "Text documents (.txt)|*.txt";
            if (d.ShowDialog() == true)
            {
                string OutputFileName = d.FileName;

                try
                {
                    using (System.IO.StreamWriter writer = System.IO.File.CreateText(OutputFileName))
                    {
                        DataStreamer.StreamFile(
                            chkWriteOutSectionHeaders.IsChecked.Value, chkScriptDrops.IsChecked.Value, chkScriptCreates.IsChecked.Value, writer, 
                            txtPrimaryInstance.Text, cmbPrimaryDatabase.SelectedValue.ToString(),
                            txtSecondaryInstance.Text, cmbSecondaryDatabase.SelectedValue.ToString(), 
                            PrimaryGroup, SecondaryGroup);
                    }

                    System.Windows.MessageBox.Show(String.Format("Your file is now available at {0}.", OutputFileName), "File Export Successful.", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(ex.Message);
                }
            }
        }

        #endregion

        #region Grid Buttons

        void ShowDropScript(object sender, RoutedEventArgs e)
        {
            IndexComparer.BusinessObjects.IndexGroup ix = ((FrameworkElement)sender).DataContext as IndexComparer.BusinessObjects.IndexGroup;

            ScriptWindow sw = new ScriptWindow(String.Format("DROP Script For {0}", ix.SchemaAndTableName), ix.DropScript);
            sw.Show();
        }

        void ShowPrimaryCreateScript(object sender, RoutedEventArgs e)
        {
            IndexComparer.BusinessObjects.IndexGroup ix = ((FrameworkElement)sender).DataContext as IndexComparer.BusinessObjects.IndexGroup;

            ScriptWindow sw = new ScriptWindow
            (
                String.Format
                (
                    "CREATE Script For {0}.{1}.{2}", 
                    ix.PrimaryIndexSet.ServerName, 
                    ix.PrimaryIndexSet.DatabaseName, 
                    ix.SchemaAndTableName
                ), 
                ix.PrimaryIndexCreateScript
            );
            sw.Show();
        }

        void ShowSecondaryCreateScript(object sender, RoutedEventArgs e)
        {
            IndexComparer.BusinessObjects.IndexGroup ix = ((FrameworkElement)sender).DataContext as IndexComparer.BusinessObjects.IndexGroup;

            ScriptWindow sw = new ScriptWindow
            (
                String.Format
                (
                    "CREATE Script For {0}.{1}.{2}",
                    ix.SecondaryIndexSet.ServerName,
                    ix.SecondaryIndexSet.DatabaseName,
                    ix.SchemaAndTableName
                ),
                ix.SecondaryIndexCreateScript
            );
            sw.Show();
        }

        #endregion
    }
}
