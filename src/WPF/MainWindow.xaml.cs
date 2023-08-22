using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BomComparer.Comparer;
using BomComparer.ExcelReaders;
using BomWriter.ExcelWriter;
using Microsoft.Win32;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var currentDirectory = Directory.GetCurrentDirectory();
            OutputFilePathTextBox.Text = currentDirectory;
        }

        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx"
            };

            if (sender is not Button button) return;

            var associatedTextBoxName = button.Name
                .Replace("Select", "")
                .Replace("Button", "PathTextBox");

            var associatedTextBox = FindName(associatedTextBoxName) as TextBox;

            if (associatedTextBox == null)
                throw new ArgumentNullException($"{button.Name} button does not have a coresponding text box");

            if (fileDialog.ShowDialog() == true)
            {
                associatedTextBox.Text = fileDialog.FileName;
            }
        }

        private void SelectPath_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

            if (sender is Button button)
            {
                var associatedTextBoxName = button.Name
                    .Replace("Select", "")
                    .Replace("Button", "PathTextBox");

                var associatedTextBox = FindName(associatedTextBoxName) as TextBox;

                if (associatedTextBox == null)
                    throw new ArgumentNullException($"{button.Name} button does not have a coresponding text box");

                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    associatedTextBox.Text = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private async void CompareFiles_Click(object sender, RoutedEventArgs e)
        {
            CompareButton.IsEnabled = false;
            Spinner.Visibility = Visibility.Visible;
            ErrorLabel.Visibility = Visibility.Collapsed;
            ErrorLabel.Text = "";

            try
            {
                var sourcePath = SourceFilePathTextBox.Text;
                var targetPath = TargetFilePathTextBox.Text;
                var baseOutputPath = OutputFilePathTextBox.Text;

                var reader = new NpoiReader();
                var comparer = new BomCompare();
                var results = await Task.Run(() =>
                {
                    var sourceData = reader.ReadData(sourcePath);
                    var targetData = reader.ReadData(targetPath);

                    return comparer.Compare(sourceData, targetData);
                });

                var writer = new NpoiWriter();
                var outputPath = Path.Combine(baseOutputPath,
                    $"{Path.GetFileName(sourcePath)}_vs_{Path.GetFileName(targetPath)}.xlsx");

                await Task.Run(() => { writer.Write(outputPath, results); });

                Spinner.Visibility = Visibility.Collapsed;

                MessageBox.Show("Done!");
            }
            catch (IOException ex)
            {
                Spinner.Visibility = Visibility.Collapsed;
                ErrorLabel.Text = "Comparison failed! Check if the source, target or result files are closed.";
                ErrorLabel.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                Spinner.Visibility = Visibility.Collapsed;
                ErrorLabel.Text = "Unexpected error occurred!";
                ErrorLabel.Visibility = Visibility.Visible;
            }

            CompareButton.IsEnabled = true;
        }
    }
}
