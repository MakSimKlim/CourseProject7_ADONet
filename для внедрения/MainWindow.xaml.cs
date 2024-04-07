using System.Diagnostics;
using System.Windows;

namespace DaughterProcesses
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartNotepad_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("notepad.exe");
        }

        private void StartPaint_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("mspaint.exe");
        }

        private void StartCalculator_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("calc.exe");
        }
    }
}
