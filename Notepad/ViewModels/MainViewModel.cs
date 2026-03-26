using Notepad.Models;
using Notepad.Services;
using Notepad.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Notepad.ViewModels
{
    public class MainViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        private readonly FileService _fileService = new FileService();
        private readonly SearchService _searchService = new SearchService();

        private MyTabModel _selectedTab;
        private bool _showFolderExplorer;
        private string _copiedFolderPath;
        private bool _hasTextSelected;

        public DataContextTree FolderTree { get; set; }
        public ObservableCollection<MyTabModel> Tabs { get; set; }

        public MyTabModel SelectedTab
        {
            get { return _selectedTab; }
            set { _selectedTab = value; OnPropertyChanged("SelectedTab"); }
        }

        public bool ShowFolderExplorer
        {
            get { return _showFolderExplorer; }
            set { _showFolderExplorer = value; OnPropertyChanged("ShowFolderExplorer"); }
        }

        public bool HasTextSelected
        {
            get { return _hasTextSelected; }
            set
            {
                _hasTextSelected = value;
                OnPropertyChanged("HasTextSelected");
                CommandManager.InvalidateRequerySuggested();
            }
        }

        #region Commands
        public ICommand NewFileCommand { get; set; }
        public ICommand OpenFileCommand { get; set; }
        public ICommand SaveFileCommand { get; set; }
        public ICommand SaveFileAsCommand { get; set; }
        public ICommand ExitCommand { get; set; }
        public ICommand CloseAllFilesCommand { get; set; }
        public ICommand CloseTabCommand { get; set; }

        public ICommand FindCommand { get; set; }
        public ICommand ReplaceCommand { get; set; }

        public ICommand ShowStandardViewCommand { get; set; }
        public ICommand ShowFolderExplorerCommand { get; set; }

        public ICommand NewFileInFolderCommand { get; set; }
        public ICommand CopyPathCommand { get; set; }
        public ICommand CopyFolderCommand { get; set; }
        public ICommand PasteFolderCommand { get; set; }

        public ICommand CopyCommand { get; set; }
        public ICommand CutCommand { get; set; }
        public ICommand PasteCommand { get; set; }
        public ICommand ToUpperCaseCommand { get; set; }
        public ICommand ToLowerCaseCommand { get; set; }
        public ICommand GoToLineCommand { get; set; }
        public ICommand RemoveEmptyLinesCommand { get; set; }
        public ICommand ToggleReadOnlyCommand { get; set; }

        #endregion

        public MainViewModel()
        {
            Tabs = new ObservableCollection<MyTabModel>();
            FolderTree = new DataContextTree();

            NewFileCommand = new RelayCommand(NewFile);
            OpenFileCommand = new RelayCommand(OpenFile);
            SaveFileCommand = new RelayCommand(SaveFile, CanSaveFile);
            SaveFileAsCommand = new RelayCommand(SaveFileAs, CanSaveFile);
            ExitCommand = new RelayCommand(Exit);
            CloseAllFilesCommand = new RelayCommand(CloseAllFiles);
            ShowStandardViewCommand = new RelayCommand(ShowStandardView);
            ShowFolderExplorerCommand = new RelayCommand(ShowFolderExplorerView);

            FindCommand = new RelayCommand(Find);
            ReplaceCommand = new RelayCommand(Replace);

            NewFileInFolderCommand = new RelayCommand<FileDirectoryModel>(NewFileInFolder);
            CopyPathCommand = new RelayCommand<FileDirectoryModel>(CopyPath);
            CopyFolderCommand = new RelayCommand<FileDirectoryModel>(CopyFolder);
            PasteFolderCommand = new RelayCommand<FileDirectoryModel>(PasteFolder, CanPasteFolder);

            CopyCommand = new RelayCommand(Copy, IsTextSelected);
            CutCommand = new RelayCommand(Cut, IsTextSelected);
            PasteCommand = new RelayCommand(Paste);
            ToUpperCaseCommand = new RelayCommand(ToUpperCase, IsTextSelected);
            ToLowerCaseCommand = new RelayCommand(ToLowerCase, IsTextSelected);
            GoToLineCommand = new RelayCommand(GoToLine);
            RemoveEmptyLinesCommand = new RelayCommand(RemoveEmptyLines);
            ToggleReadOnlyCommand = new RelayCommand(ToggleReadOnly);

            CloseTabCommand = new RelayCommand<MyTabModel>(CloseTab);

            NewFile();
        }

        #region File

        private void NewFile()
        {
            int fileNumber = Tabs.Count + 1;
            MyTabModel newTab = new MyTabModel();
            newTab.Header = "File " + fileNumber;
            newTab.Content = "";
            newTab.IsSaved = false;

            Tabs.Add(newTab);
            SelectedTab = newTab;
        }

        private void OpenFile()
        {
            MyTabModel openedTab = _fileService.OpenFile();
            if (openedTab == null) return;

            Tabs.Add(openedTab);
            SelectedTab = openedTab;
        }

        private void SaveFile()
        {
            if (SelectedTab == null) return;
            _fileService.SaveFile(SelectedTab);
        }

        private void SaveFileAs()
        {
            if (SelectedTab == null) return;
            _fileService.SaveFileAs(SelectedTab);
        }

        private bool CanSaveFile()
        {
            return SelectedTab != null;
        }

        private bool ConfirmClose(MyTabModel tabToClose)
        {
            return _fileService.ConfirmClose(tabToClose, delegate (MyTabModel tab)
            {
                SelectedTab = tab;
                return _fileService.SaveFile(tab);
            });
        }

        public void CloseAllFiles()
        {
            List<MyTabModel> allTabs = new List<MyTabModel>(Tabs);
            foreach (MyTabModel tab in allTabs)
            {
                if (!ConfirmClose(tab)) return;
            }

            Tabs.Clear();
            NewFile();
        }

        private void CloseTab(MyTabModel tabToClose)
        {
            if (tabToClose == null) return;
            if (!ConfirmClose(tabToClose)) return;

            int tabIndex = Tabs.IndexOf(tabToClose);
            Tabs.Remove(tabToClose);

            if (Tabs.Count == 0)
            {
                NewFile();
                return;
            }

            SelectedTab = Tabs[Math.Max(0, tabIndex - 1)];
        }

        public void Exit()
        {
            List<MyTabModel> allTabs = new List<MyTabModel>(Tabs);
            foreach (MyTabModel tab in allTabs)
            {
                if (!ConfirmClose(tab)) return;
            }

            Application.Current.Shutdown();
        }

        #endregion

        #region Edit
        private bool IsTextSelected()
        {
            return HasTextSelected;
        }

        private void Copy()
        {
            TextBox activeTextBox = GetActiveTextBox();
            if (activeTextBox == null || string.IsNullOrEmpty(activeTextBox.SelectedText)) return;
            Clipboard.SetText(activeTextBox.SelectedText);
        }

        private void Cut()
        {
            TextBox activeTextBox = GetActiveTextBox();
            if (activeTextBox == null || SelectedTab == null || string.IsNullOrEmpty(activeTextBox.SelectedText)) return;

            Clipboard.SetText(activeTextBox.SelectedText);
            SelectedTab.Content = SelectedTab.Content.Remove(activeTextBox.SelectionStart, activeTextBox.SelectedText.Length);
        }

        private void Paste()
        {
            TextBox activeTextBox = GetActiveTextBox();
            if (activeTextBox == null || SelectedTab == null) return;
            if (!Clipboard.ContainsText()) return;

            string textToPaste = Clipboard.GetText();
            int caretPosition = activeTextBox.CaretIndex;
            SelectedTab.Content = SelectedTab.Content.Insert(caretPosition, textToPaste);

            Application.Current.Dispatcher.BeginInvoke(
                new Action(delegate
                {
                    activeTextBox.CaretIndex = caretPosition + textToPaste.Length;
                }),
                System.Windows.Threading.DispatcherPriority.ContextIdle);
        }

        private void ToUpperCase()
        {
            TextBox activeTextBox = GetActiveTextBox();
            if (activeTextBox == null || string.IsNullOrEmpty(activeTextBox.SelectedText)) return;

            int selectionStart = activeTextBox.SelectionStart;
            int selectionLength = activeTextBox.SelectionLength;
            string upperText = activeTextBox.SelectedText.ToUpper();

            SelectedTab.Content = SelectedTab.Content
                .Remove(selectionStart, selectionLength)
                .Insert(selectionStart, upperText);

            Application.Current.Dispatcher.BeginInvoke(
                new Action(delegate
                {
                    activeTextBox.Select(selectionStart, selectionLength);
                }),
                System.Windows.Threading.DispatcherPriority.ContextIdle);
        }

        private void ToLowerCase()
        {
            TextBox activeTextBox = GetActiveTextBox();
            if (activeTextBox == null || string.IsNullOrEmpty(activeTextBox.SelectedText)) return;

            int selectionStart = activeTextBox.SelectionStart;
            int selectionLength = activeTextBox.SelectionLength;
            string lowerText = activeTextBox.SelectedText.ToLower();

            SelectedTab.Content = SelectedTab.Content
                .Remove(selectionStart, selectionLength)
                .Insert(selectionStart, lowerText);

            Application.Current.Dispatcher.BeginInvoke(
                new Action(delegate
                {
                    activeTextBox.Select(selectionStart, selectionLength);
                }),
                System.Windows.Threading.DispatcherPriority.ContextIdle);
        }

        private void RemoveEmptyLines()
        {
            if (SelectedTab == null || string.IsNullOrEmpty(SelectedTab.Content)) return;

            string[] allLines = SelectedTab.Content.Split(
                new string[] { "\r\n", "\n" },
                StringSplitOptions.None);

            List<string> nonEmptyLines = new List<string>();
            foreach (string line in allLines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                    nonEmptyLines.Add(line);
            }

            SelectedTab.Content = string.Join(Environment.NewLine, nonEmptyLines);
        }

        private void ToggleReadOnly()
        {
            if (SelectedTab == null) return;
            SelectedTab.IsReadOnly = !SelectedTab.IsReadOnly;
        }

        private void GoToLine()
        {
            TextBox activeTextBox = GetActiveTextBox();
            if (activeTextBox == null || SelectedTab == null) return;

            int totalLines = activeTextBox.LineCount > 0 ? activeTextBox.LineCount : 1;

            Views.GoToLineDialog goToLineDialog = new Views.GoToLineDialog(totalLines);
            if (goToLineDialog.ShowDialog() != true) return;

            int lineNumber = goToLineDialog.LineNumber;

            if (lineNumber < 1 || lineNumber > totalLines)
            {
                MessageBox.Show("Line number must be between 1 and " + totalLines + ".");
                return;
            }

            int charIndex = activeTextBox.GetCharacterIndexFromLineIndex(lineNumber - 1);
            if (charIndex < 0) return;

            activeTextBox.Focus();
            activeTextBox.CaretIndex = charIndex;
            activeTextBox.ScrollToLine(lineNumber - 1);
        }
        #endregion

        #region Search
        private void Find()
        {
            FindViewModel findViewModel = new FindViewModel(
                _searchService,
                delegate (bool searchInAllTabs) { return GetTargetTabs(searchInAllTabs); });

            findViewModel.HighlightCallback = HighlightInEditor;

            Views.FindDialog findDialog = new Views.FindDialog();
            findDialog.DataContext = findViewModel;
            findDialog.Show();
        }

        private void Replace()
        {
            ReplaceViewModel replaceViewModel = new ReplaceViewModel(
                _searchService,
                delegate (bool searchInAllTabs) { return GetTargetTabs(searchInAllTabs); });

            replaceViewModel.HighlightCallback = HighlightInEditor;

            Views.ReplaceDialog replaceDialog = new Views.ReplaceDialog();
            replaceDialog.DataContext = replaceViewModel;
            replaceDialog.Show();
        }

        private List<MyTabModel> GetTargetTabs(bool searchInAllTabs)
        {
            if (searchInAllTabs)
                return new List<MyTabModel>(Tabs);

            if (SelectedTab != null)
                return new List<MyTabModel> { SelectedTab };

            return new List<MyTabModel>();
        }

        private void HighlightInEditor(MyTabModel tabToHighlight, int characterIndex, int selectionLength)
        {
            SelectedTab = tabToHighlight;

            Application.Current.Dispatcher.BeginInvoke(
                new Action(delegate
                {
                    Application.Current.Dispatcher.BeginInvoke(
                        new Action(delegate
                        {
                            TextBox activeTextBox = GetActiveTextBox();
                            if (activeTextBox == null) return;

                            if (characterIndex < 0 || characterIndex + selectionLength > activeTextBox.Text.Length) return;

                            activeTextBox.Focus();
                            activeTextBox.Select(characterIndex, selectionLength);

                            int lineIndex = activeTextBox.GetLineIndexFromCharacterIndex(characterIndex);
                            if (lineIndex >= 0)
                                activeTextBox.ScrollToLine(lineIndex);
                        }),
                        System.Windows.Threading.DispatcherPriority.ContextIdle);
                }),
                System.Windows.Threading.DispatcherPriority.ContextIdle);
        }

        #endregion 

        #region View 
        private void ShowStandardView()
        {
            ShowFolderExplorer = false;
        }

        private void ShowFolderExplorerView()
        {
            ShowFolderExplorer = true;
        }

        #endregion 

        #region File Explorer Operations

        public void OpenFileFromExplorer(string filePath)
        {
            if (!File.Exists(filePath)) return;

            foreach (MyTabModel tab in Tabs)
            {
                if (tab.FilePath == filePath)
                {
                    SelectedTab = tab;
                    return;
                }
            }

            MyTabModel newTab = new MyTabModel();
            newTab.Header = Path.GetFileName(filePath);
            newTab.FilePath = filePath;
            newTab.Content = File.ReadAllText(filePath);
            newTab.IsSaved = true;

            Tabs.Add(newTab);
            SelectedTab = Tabs[Tabs.Count - 1];
        }

        private void NewFileInFolder(FileDirectoryModel selectedFolder)
        {
            if (selectedFolder == null || !selectedFolder.IsDirectory) return;

            string newFilePath = Path.Combine(selectedFolder.FullPath, "NewFile.txt");
            int counter = 1;

            while (File.Exists(newFilePath))
            {
                newFilePath = Path.Combine(selectedFolder.FullPath, "NewFile" + counter + ".txt");
                counter++;
            }

            File.WriteAllText(newFilePath, "");
            selectedFolder.RefreshChildren();
        }

        private void CopyPath(FileDirectoryModel selectedItem)
        {
            if (selectedItem == null) return;
            Clipboard.SetText(selectedItem.FullPath);
        }

        private void CopyFolder(FileDirectoryModel selectedFolder)
        {
            if (selectedFolder == null || !selectedFolder.IsDirectory) return;
            _copiedFolderPath = selectedFolder.FullPath;
            CommandManager.InvalidateRequerySuggested();
        }

        private void PasteFolder(FileDirectoryModel destinationFolder)
        {
            if (destinationFolder == null || !destinationFolder.IsDirectory || _copiedFolderPath == null) return;

            string destinationPath = Path.Combine(
                destinationFolder.FullPath,
                Path.GetFileName(_copiedFolderPath));

            CopyDirectoryRecursively(_copiedFolderPath, destinationPath);
            destinationFolder.RefreshChildren();
        }

        private bool CanPasteFolder(FileDirectoryModel destinationFolder)
        {
            return destinationFolder != null && destinationFolder.IsDirectory && _copiedFolderPath != null;
        }

        private void CopyDirectoryRecursively(string sourcePath, string destinationPath)
        {
            Directory.CreateDirectory(destinationPath);

            foreach (string filePath in Directory.GetFiles(sourcePath))
            {
                string destinationFilePath = Path.Combine(destinationPath, Path.GetFileName(filePath));
                File.Copy(filePath, destinationFilePath, true);
            }

            foreach (string subDirectoryPath in Directory.GetDirectories(sourcePath))
            {
                string destinationSubPath = Path.Combine(destinationPath, Path.GetFileName(subDirectoryPath));
                CopyDirectoryRecursively(subDirectoryPath, destinationSubPath);
            }
        }

        #endregion

        #region Utilities
        private TextBox GetActiveTextBox()
        {
            TabControl tabControl = FindVisualChild<TabControl>(Application.Current.MainWindow);
            if (tabControl == null) return null;

            ContentPresenter contentPresenter = tabControl.Template.FindName("PART_SelectedContentHost", tabControl) as ContentPresenter;

            if (contentPresenter == null) return null;

            return FindVisualChild<TextBox>(contentPresenter);
        }

        private static T FindVisualChild<T>(DependencyObject parentElement)
            where T : DependencyObject
        {
            if (parentElement == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parentElement); i++)
            {
                DependencyObject childElement = VisualTreeHelper.GetChild(parentElement, i);

                if (childElement is T)
                    return (T)childElement;

                T foundChild = FindVisualChild<T>(childElement);
                if (foundChild != null) return foundChild;
            }

            return null;
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}