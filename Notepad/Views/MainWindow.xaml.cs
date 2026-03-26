using Notepad.Models;
using Notepad.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Notepad.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }

        private void TreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeView treeView = sender as TreeView;
            if (treeView == null) return;

            FileDirectoryModel selectedItem = treeView.SelectedItem as FileDirectoryModel;
            if (selectedItem == null) return;

            if (selectedItem.IsDirectory) return;

            MainViewModel viewModel = DataContext as MainViewModel;
            if (viewModel == null) return;

            viewModel.OpenFileFromExplorer(selectedItem.FullPath);
        }

        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox activeTextBox = sender as TextBox;
            if (activeTextBox == null) return;

            MainViewModel viewModel = DataContext as MainViewModel;
            if (viewModel == null) return;

            viewModel.HasTextSelected = !string.IsNullOrEmpty(activeTextBox.SelectedText);
        }
    }
}