using System.Windows;

namespace Notepad.Views
{
    public partial class ReplaceDialog : Window
    {
        public ReplaceDialog()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}