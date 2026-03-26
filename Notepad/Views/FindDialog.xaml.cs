using System.Windows;

namespace Notepad.Views
{
    public partial class FindDialog : Window
    {
        public FindDialog()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}