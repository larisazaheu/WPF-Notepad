using Notepad.Models;
using Notepad.Services;
using Notepad.ViewModels.Commands;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Notepad.ViewModels
{
    public class FindViewModel : INotifyPropertyChanged
    {

        private readonly SearchService _searchService;
        private readonly Func<bool, List<MyTabModel>> _getTabsCallback;

        private string _searchText;
        private bool _searchInSelectedTab = true;
        private string _resultLabel;
        private List<(MyTabModel tab, int index)> _searchResults = new List<(MyTabModel, int)>();
        private int _currentResultIndex = -1;


        public Action<MyTabModel, int, int> HighlightCallback { get; set; }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged("SearchText");
                _searchResults.Clear();
                _currentResultIndex = -1;
                ResultLabel = "";
            }
        }

        public bool SearchInSelectedTab
        {
            get { return _searchInSelectedTab; }
            set { _searchInSelectedTab = value; OnPropertyChanged("SearchInSelectedTab"); }
        }

        public bool SearchInAllTabs
        {
            get { return !_searchInSelectedTab; }
            set { _searchInSelectedTab = !value; OnPropertyChanged("SearchInAllTabs"); }
        }

        public string ResultLabel
        {
            get { return _resultLabel; }
            set { _resultLabel = value; OnPropertyChanged("ResultLabel"); }
        }


        public ICommand FindCommand { get; set; }
        public ICommand NextCommand { get; set; }
        public ICommand PreviousCommand { get; set; }


        public FindViewModel(SearchService searchService,
                             Func<bool, List<MyTabModel>> getTabsCallback)
        {
            _searchService = searchService;
            _getTabsCallback = getTabsCallback;

            FindCommand = new RelayCommand(RunSearch, CanRunSearch);
            NextCommand = new RelayCommand(GoToNext, HasSearchResults);
            PreviousCommand = new RelayCommand(GoToPrevious, HasSearchResults);
        }


        private bool CanRunSearch()
        {
            return !string.IsNullOrEmpty(SearchText);
        }

        private bool HasSearchResults()
        {
            return _searchResults.Count > 0;
        }

        private void RunSearch()
        {
            List<MyTabModel> tabsToSearch = _getTabsCallback(SearchInAllTabs);
            _searchResults = _searchService.FindAll(SearchText, tabsToSearch);

            if (_searchResults.Count > 0)
                _currentResultIndex = 0;
            else
                _currentResultIndex = -1;

            UpdateResultLabel();
            NavigateToCurrentResult();
            CommandManager.InvalidateRequerySuggested();
        }

        private void GoToNext()
        {
            _currentResultIndex = (_currentResultIndex + 1) % _searchResults.Count;
            UpdateResultLabel();
            NavigateToCurrentResult();
        }

        private void GoToPrevious()
        {
            _currentResultIndex = (_currentResultIndex - 1 + _searchResults.Count) % _searchResults.Count;
            UpdateResultLabel();
            NavigateToCurrentResult();
        }

        private void NavigateToCurrentResult()
        {
            if (_currentResultIndex < 0 || _currentResultIndex >= _searchResults.Count) return;

            MyTabModel tab = _searchResults[_currentResultIndex].tab;
            int characterIndex = _searchResults[_currentResultIndex].index;

            if (HighlightCallback != null)
                HighlightCallback(tab, characterIndex, SearchText.Length);
        }

        private void UpdateResultLabel()
        {
            if (_searchResults.Count == 0)
                ResultLabel = "No results found";
            else
                ResultLabel = "Result " + (_currentResultIndex + 1) + " of " + _searchResults.Count;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}