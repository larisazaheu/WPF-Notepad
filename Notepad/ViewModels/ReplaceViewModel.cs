using Notepad.Models;
using Notepad.Services;
using Notepad.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Notepad.ViewModels
{
    public class ReplaceViewModel : INotifyPropertyChanged
    {
        private readonly SearchService _searchService;
        private readonly Func<bool, List<MyTabModel>> _getTabsCallback;

        private string _searchText;
        private string _replacementText;
        private bool _searchInSelectedTab = true;

        public Action<MyTabModel, int, int> HighlightCallback { get; set; }

        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; OnPropertyChanged("SearchText"); }
        }

        public string ReplacementText
        {
            get { return _replacementText; }
            set { _replacementText = value; OnPropertyChanged("ReplacementText"); }
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

        public ICommand ReplaceCommand { get; set; }
        public ICommand ReplaceAllCommand { get; set; }

        public ReplaceViewModel(SearchService searchService, Func<bool, List<MyTabModel>> getTabsCallback)
        {
            _searchService = searchService;
            _getTabsCallback = getTabsCallback;

            ReplaceCommand = new RelayCommand(ReplaceFirst, CanReplace);
            ReplaceAllCommand = new RelayCommand(ReplaceAll, CanReplace);
        }

        private bool CanReplace()
        {
            return !string.IsNullOrEmpty(SearchText);
        }

        private void ReplaceFirst()
        {
            List<MyTabModel> tabsToSearch = _getTabsCallback(SearchInAllTabs);
            string safeReplacementText = ReplacementText ?? "";

            List<(MyTabModel tab, int index)> allResults = _searchService.FindAll(SearchText, tabsToSearch);

            if (allResults.Count == 0)
            {
                MessageBox.Show("\"" + SearchText + "\" not found.");
                return;
            }

            MyTabModel targetTab = allResults[0].tab;
            int foundIndex = allResults[0].index;

            targetTab.Content = targetTab.Content
                .Remove(foundIndex, SearchText.Length)
                .Insert(foundIndex, safeReplacementText);

            List<(MyTabModel tab, int index)> remainingResults = _searchService.FindAll(SearchText, tabsToSearch);

            if (remainingResults.Count > 0 && HighlightCallback != null)
                HighlightCallback(remainingResults[0].tab, remainingResults[0].index, SearchText.Length);
        }

        private void ReplaceAll()
        {
            List<MyTabModel> tabsToSearch = _getTabsCallback(SearchInAllTabs);
            string safeReplacementText = ReplacementText ?? "";

            int totalReplacements = _searchService.ReplaceAll(SearchText, safeReplacementText, tabsToSearch);
            MessageBox.Show("Replaced " + totalReplacements + " occurrence(s).");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}