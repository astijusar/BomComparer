using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using BomComparer.Comparer;
using BomComparer.ExcelReaders;
using BomWriter.ExcelWriter;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NPOI.SS.Formula.Functions;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly NpoiReader _reader = new();
        private readonly BomCompare _comparer = new();
        private readonly NpoiWriter _writer = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEnabled))]
        private string _sourceFilePath = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEnabled))]
        private string _targetFilePath = string.Empty;

        [ObservableProperty]
        private string _outputFilePath = Directory.GetCurrentDirectory();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SpinnerVisibility))]
        [NotifyPropertyChangedFor(nameof(IsEnabled))]
        private bool _isBusy;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ErrorVisibility))]
        private string _error = string.Empty;

        public string ErrorVisibility => Error.Length == 0 ? "Collapsed" : "Visible";
        public string SpinnerVisibility => IsBusy ? "Visible" : "Collapsed";
        public bool IsEnabled => !IsBusy && SourceFilePath.Length != 0 && TargetFilePath.Length != 0;

        [RelayCommand]
        private void SelectSourceFile()
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx"
            };

            if (fileDialog.ShowDialog() == true)
            {
                SourceFilePath = fileDialog.FileName;
            }
        }

        [RelayCommand]
        private void SelectTargetFile()
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx"
            };

            if (fileDialog.ShowDialog() == true)
            {
                TargetFilePath = fileDialog.FileName;
            }
        }

        [RelayCommand]
        private void SelectOutputDirectory()
        {
            var folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                OutputFilePath = folderBrowserDialog.SelectedPath;
            }
        }

        [RelayCommand]
        private async Task Compare()
        {
            IsBusy = true;
            Error = string.Empty;

            try
            {
                var results = await Task.Run(() =>
                {
                    var sourceData = _reader.ReadData(SourceFilePath);
                    var targetData = _reader.ReadData(TargetFilePath);

                    return _comparer.Compare(sourceData, targetData);
                });

                var outputPath = Path.Combine(OutputFilePath,
                    $"{Path.GetFileName(SourceFilePath)}_VS_{Path.GetFileName(TargetFilePath)}");

                await Task.Run(() => { _writer.Write(outputPath, results); });

                IsBusy = false;

                MessageBox.Show("Done!");

                return;
            }
            catch (IOException)
            {
                Error = "Comparison failed! Check if the source, target or result files are closed.";
            }
            catch (Exception)
            {
                Error = "Unexpected error occurred!";
            }

            IsBusy = false;
        }
    }
}
