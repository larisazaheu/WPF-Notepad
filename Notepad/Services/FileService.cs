using Microsoft.Win32;
using Notepad.Models;
using System.IO;
using System.Windows;

namespace Notepad.Services
{
    public class FileService
    {
        public MyTabModel OpenFile()
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Text Files|*.txt|All Files|*.*";

            if (openDialog.ShowDialog() != true) return null;

            MyTabModel newTab = new MyTabModel();
            newTab.Header = Path.GetFileName(openDialog.FileName);
            newTab.FilePath = openDialog.FileName;
            newTab.Content = File.ReadAllText(openDialog.FileName);
            newTab.IsSaved = true;

            return newTab;
        }

        public bool SaveFile(MyTabModel tabToSave)
        {
            if (string.IsNullOrEmpty(tabToSave.FilePath))
                return SaveFileAs(tabToSave);

            File.WriteAllText(tabToSave.FilePath, tabToSave.Content);
            tabToSave.IsSaved = true;
            return true;
        }

        public bool SaveFileAs(MyTabModel tabToSave)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Text Files|*.txt|All Files|*.*";

            if (saveDialog.ShowDialog() != true) return false;

            File.WriteAllText(saveDialog.FileName, tabToSave.Content);
            tabToSave.FilePath = saveDialog.FileName;
            tabToSave.Header = Path.GetFileName(saveDialog.FileName);
            tabToSave.IsSaved = true;
            return true;
        }

        public bool ConfirmClose(MyTabModel tabToClose, Func<MyTabModel, bool> saveCallback)
        {
            if (tabToClose.IsSaved) return true;

            MessageBoxResult userChoice = MessageBox.Show(
                "File \"" + tabToClose.Header + "\" is not saved.\nDo you want to save it?",
                "Save",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Warning);

            if (userChoice == MessageBoxResult.Yes)
                return saveCallback(tabToClose);

            if (userChoice == MessageBoxResult.No)
                return true;

            return false;
        }
    }
}