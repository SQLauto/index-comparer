using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using IndexComparer.BusinessObjects;
using System.Windows.Data;

namespace IndexComparer.WPF
{
    public class MainWindowViewModel
    {
        public ICollectionView Indexes { get; private set; }

        public MainWindowViewModel
        (
            IEnumerable<IndexGroup> Groups,
            bool OnlyShowDifferences,
            bool ShowGroups
        )
        {
            if (OnlyShowDifferences)
                Indexes = CollectionViewSource.GetDefaultView(Groups.Where(x => x.ComparisonDiffersOrNull).OrderBy(x => x.SchemaAndTableName));
            else
                Indexes = CollectionViewSource.GetDefaultView(Groups.OrderBy(x => x.SchemaAndTableName));

            if (ShowGroups)
            {
                Indexes.GroupDescriptions.Add(new PropertyGroupDescription("SchemaAndTableName"));
            }
        }
    }

    public class DatabaseViewModel : INotifyPropertyChanged
    {
        public DatabaseViewModel()
        {
        }

        private string _databaseName = String.Empty;

        public string DatabaseName
        {
            get { return _databaseName; }
            set
            {
                if (_databaseName != value)
                {
                    _databaseName = value;
                    NotifyPropertyChanged("DatabaseName");
                }
            }
        }

        #region INotifyPropertyChanged Members

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
