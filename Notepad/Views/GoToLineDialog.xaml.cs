using System.Windows;

namespace Notepad.Views
{
    public partial class GoToLineDialog : Window
    {
        public int LineNumber { get; private set; }

        public GoToLineDialog(int totalLines)
        {
            InitializeComponent();
            PromptLabel.Text = "Line (1 - " + totalLines + "):";
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            int parsedNumber;
            if (!int.TryParse(LineNumberTextBox.Text, out parsedNumber))
            {
                MessageBox.Show("Invalid line number.");
                return;
            }
            LineNumber = parsedNumber;
            DialogResult = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}