using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Notepad.Models
{
    public class MyTabModel : INotifyPropertyChanged
    {
        private string _header;
        private string _content;
        private bool _isSaved;
        private string _filePath;
        private bool _isReadOnly;

        public string Header
        {
            get { return _header; }
            set { _header = value; OnPropertyChanged("Header"); }
        }

        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                OnPropertyChanged("Content");
                IsSaved = false;
            }
        }

        public bool IsSaved
        {
            get { return _isSaved; }
            set { _isSaved = value; OnPropertyChanged("IsSaved"); }
        }

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; OnPropertyChanged("FilePath"); }
        }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set { _isReadOnly = value; OnPropertyChanged("IsReadOnly"); }
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