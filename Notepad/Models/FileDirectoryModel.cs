using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace Notepad.Models
{
    public class FileDirectoryModel : INotifyPropertyChanged
    {
        public string Name { get; }
        public string FullPath { get; }
        public bool IsDirectory { get; }
        public ObservableCollection<FileDirectoryModel> Items { get; }

        private bool _childrenLoaded;
        private bool _isExpanded;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                OnPropertyChanged("IsExpanded");

                if (value == true)
                {
                    LoadChildren();
                }
            }
        }

        public FileDirectoryModel(string path)
        {
            FullPath = path;
            IsDirectory = Directory.Exists(path);

            string fileName = Path.GetFileName(path);
            if (string.IsNullOrEmpty(fileName))
                Name = path;
            else
                Name = fileName;

            Items = new ObservableCollection<FileDirectoryModel>();

            if (IsDirectory)
            {
                Items.Add(null);
            }
        }

        private void LoadChildren()
        {
            if (!IsDirectory) return;
            if (_childrenLoaded) return;

            FillItems();
            _childrenLoaded = true;
        }

        public void RefreshChildren()
        {
            if (!IsDirectory) return;

            Items.Clear();
            _childrenLoaded = false;

            FillItems();
            _childrenLoaded = true;
        }


        private void FillItems()
        {
            if (Items.Count == 1 && Items[0] == null)
            {
                Items.Clear();
            }

            try
            {
                foreach (string directoryPath in Directory.GetDirectories(FullPath))
                {
                    Items.Add(new FileDirectoryModel(directoryPath));
                }

                foreach (string filePath in Directory.GetFiles(FullPath))
                {
                    Items.Add(new FileDirectoryModel(filePath));
                }
            }
            catch
            {
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}