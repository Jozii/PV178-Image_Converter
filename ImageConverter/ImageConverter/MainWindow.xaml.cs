using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using ImageConverter.BusinessLogic;
using ImageConverter.BusinessLogic.Enumerations;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SizeConverter = ImageConverter.BusinessLogic.SizeConverter;

namespace ImageConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> _files = new List<string>();
        private string _outputDirectory;
        private readonly IXMLLog _xmlLog = new XMLLog();
        private readonly IFormatConverter _formatConverter;
        private readonly ISizeConverter _sizeConverter;
        public MainWindow()
        {
            InitializeComponent();
            InitScreen();
            _formatConverter = new FormatConverter(new BitmapSourceLoader(), _xmlLog, new FormatEncoder());
            _sizeConverter = new SizeConverter(new BitmapSourceLoader(), _xmlLog, new FormatEncoder());
        }
        private void ButtonSelectFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JPEG Files (*.jpeg)|*.jpeg|JPG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|Tiff Files (*.tif)|*.tiff|GIF Files (*.gif)|*.gif|BMP Files (*.bmp)|*.bmp";
            dialog.Multiselect = true;
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                _files.Clear();
                _files.AddRange(dialog.FileNames);
                SetNumberOfFiles();
            }
        }

        private void SetNumberOfFiles()
        {
            TextBoxNumberOfSelectedFiles.Text = _files.Count.ToString();
        }

        private void OutputDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog fb = new FolderBrowserDialog())
            {
                fb.ShowNewFolderButton = true;
                if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _outputDirectory = fb.SelectedPath;
                    TextBoxOutputDirectory.Text = fb.SelectedPath;
                }
            }
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ControlProperties())
                return;
            if (FormatConversionRadioBox.IsChecked != null && (bool) FormatConversionRadioBox.IsChecked)
            {
                ConvertFormat();
            }
            else if (SizeConversionRadioBox.IsChecked != null && (bool) SizeConversionRadioBox.IsChecked)
            {
                ConvertSize();
            }
        }

        private bool ControlProperties()
        {
            if (FormatConversionRadioBox.IsChecked != true && SizeConversionRadioBox.IsChecked != true)
            {
                MessageBox.Show("Select either format or size conversion", "Select what to do", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            if (FormatConversionRadioBox.IsChecked == true)
            {
                Format format = (Format) Enum.Parse(typeof (Format), OutputFormatComboBox.SelectedItem.ToString());
                if (format == Format.JPEG && Int32.Parse(TextBoxJPEGCompression.Text) > 100)
                {
                    MessageBox.Show("Compression has to be mostly 100%", "Wrong compression", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return false;
                }
            }
            return true;
        }


        private void ConvertFormat()
        {
            string outputFileName = _outputDirectory + "\\" + TextBoxOutputFileName.Text;
            Format format = (Format)Enum.Parse(typeof(Format), OutputFormatComboBox.SelectedItem.ToString());
            switch (format)
            {
                case Format.JPEG:
                    outputFileName += ".jpeg";
                    break;
                case Format.PNG:
                    outputFileName += ".png";
                    break;
                case Format.Tiff:
                    outputFileName += ".tiff";
                    break;
                case Format.GIF:
                    outputFileName += ".gif";
                    break;
                case Format.BMP:
                    outputFileName += ".bmp";
                    break;
            }
            using (BackgroundWorker worker = new BackgroundWorker())
            {
                int compression = Int32.Parse(TextBoxJPEGCompression.Text);
                bool overwriteFiles = (bool)CheckBoxOverwriteExistingFiles.IsChecked;
                worker.WorkerReportsProgress = true;
                worker.DoWork += BgConvertType;
                worker.ProgressChanged += BgWorkerProgressChanged;
                worker.RunWorkerCompleted += BgWorkerCompleted;
                worker.RunWorkerAsync(Tuple.Create(_files, format, outputFileName,
                    compression, overwriteFiles));
            }
        }
        private void ConvertSize()
        {
            int width = 0;
            int height = 0;
            KeepAspectRatio ratio = (KeepAspectRatio)Enum.Parse(typeof(KeepAspectRatio), ComboBoxKeepAspectRatio.SelectedItem.ToString());
            switch (ratio)
            {
                case KeepAspectRatio.NONE:
                    width = Int32.Parse(TextBoxWidth.Text);
                    height = Int32.Parse(TextBoxHeight.Text);
                    break;
                case KeepAspectRatio.WIDTH:
                    width = Int32.Parse(TextBoxWidth.Text);
                    break;
                case KeepAspectRatio.HEIGHT:
                    height = Int32.Parse(TextBoxHeight.Text);
                    break;
            }
            string outputFileName = _outputDirectory + "\\" + TextBoxOutputFileName.Text;
            bool enlargeSmallerImages = CheckBoxEnlargeSmallerImages.IsChecked != null && (bool)  CheckBoxEnlargeSmallerImages.IsChecked;
            bool overWriteOutput = CheckBoxOverwriteExistingFiles.IsChecked != null && (bool) CheckBoxOverwriteExistingFiles.IsChecked;
            using (BackgroundWorker worker = new BackgroundWorker())
            {
                worker.WorkerReportsProgress = true;
                worker.DoWork += BgConvertSize;
                worker.ProgressChanged += BgWorkerProgressChanged;
                worker.RunWorkerCompleted += BgWorkerCompleted;
                worker.RunWorkerAsync(Tuple.Create(_files, width, height, outputFileName, ratio, enlargeSmallerImages,
                    overWriteOutput));
            }
        }

        

        private void BgWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = (List<string>) e.Result;
            if (result.Count == 0)
            {
                MessageBox.Show("All selected files were converted","Conversion successful",MessageBoxButton.OK,MessageBoxImage.Information);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (string s in result)
                {
                    sb.Append(s + '\n');
                }
                MessageBox.Show("These files were not converted\n" + sb.ToString(),"Error in converting files",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            TextBoxProcessedFile.Text = "";
            ProgressBarProgress.Value = 0;
        }

        private void BgWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            TextBoxProcessedFile.Text = e.UserState as string;
            ProgressBarProgress.Value = Math.Min(100, e.ProgressPercentage);
        }

        private void BgConvertType(object sender, DoWorkEventArgs e)
        {
            var bw = sender as BackgroundWorker;
            var args = (Tuple<List<string>, Format, string, int, bool>) e.Argument;
            var files = args.Item1;
            var format = args.Item2;
            var outputFileName = args.Item3;
            var compression = args.Item4;
            var overwriteOutput = args.Item5;
            var result = _formatConverter.Convert(files, format, outputFileName, compression, overwriteOutput,bw);
            e.Result = result;
        }

        private void BgConvertSize(object sender, DoWorkEventArgs e)
        {
            var bw = sender as BackgroundWorker;
            var args = (Tuple<List<string>, int, int, string, KeepAspectRatio, bool, bool>) e.Argument;
            var files = args.Item1;
            int width = args.Item2;
            int height = args.Item3;
            string outputFileName = args.Item4;
            KeepAspectRatio ratio = args.Item5;
            bool enlargeSmallerImages = args.Item6;
            bool overwriteOutput = args.Item7;
            var result = _sizeConverter.Resize(files, width, height, outputFileName, ratio, enlargeSmallerImages,
                overwriteOutput, bw);
            e.Result = result;
        }

        private void InitScreen()
        {
            InitComboBoxes();
            TextBoxNumberOfSelectedFiles.Text = "0";
            TextBoxNumberOfSelectedFiles.IsReadOnly = true;
            TextBoxProcessedFile.IsReadOnly = true;
            TextBoxOutputDirectory.IsReadOnly = true;
        }

        private void InitComboBoxes()
        {
            OutputFormatComboBox.ItemsSource = Enum.GetValues(typeof (Format));
            OutputFormatComboBox.SelectedIndex = 0;
            TextBoxJPEGCompression.Visibility = Visibility.Visible;
            LabelJPEGCompression.Visibility = Visibility.Visible;
            ComboBoxKeepAspectRatio.ItemsSource = Enum.GetValues(typeof (KeepAspectRatio));
            ComboBoxKeepAspectRatio.SelectedIndex = 0;
        }

        private void ButtonSelectDirectory_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog fb = new FolderBrowserDialog())
            {
                fb.ShowNewFolderButton = false;
                if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _files = Directory.GetFiles(fb.SelectedPath).ToList();
                    SetNumberOfFiles();
                }
            }
        }

        private void OutputFormatComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OutputFormatComboBox.SelectedIndex == 0)
            {
                TextBoxJPEGCompression.Visibility = Visibility.Visible;
                LabelJPEGCompression.Visibility = Visibility.Visible;
            }
            else
            {
                TextBoxJPEGCompression.Visibility = Visibility.Hidden;
                LabelJPEGCompression.Visibility = Visibility.Hidden;
            }
        }

        private void ComboBoxKeepAspectRatio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ComboBoxKeepAspectRatio.SelectedIndex)
            {
                case 0:
                    WidthAndHeightSetVisibility(true,true);
                    break;
                case 1:
                    WidthAndHeightSetVisibility(true,false);
                    break;
                case 2:
                    WidthAndHeightSetVisibility(false,true);
                    break;
            }
        }

        private void WidthAndHeightSetVisibility(bool width, bool height)
        {
            if (width)
            {
                LabelWidth.Visibility = Visibility.Visible;
                TextBoxWidth.Visibility = Visibility.Visible;
            }
            else
            {
                LabelWidth.Visibility = Visibility.Hidden;
                TextBoxWidth.Visibility = Visibility.Hidden;
            }
            if (height)
            {
                LabelHeight.Visibility = Visibility.Visible;
                TextBoxHeight.Visibility = Visibility.Visible;
            }
            else
            {
                LabelHeight.Visibility = Visibility.Hidden;
                TextBoxHeight.Visibility = Visibility.Hidden;
            }
        }

        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]+");
            return !regex.IsMatch(text);
        }

        private void FileNamePreview(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowedInFileName(e.Text);
            MessageBox.Show("This character is there not allowed (" + e.Text + ")","Cannot enter this character",MessageBoxButton.OK,MessageBoxImage.Information);
        }

        private bool IsTextAllowedInFileName(string text)
        {
            IEnumerable<char> list = Path.GetInvalidFileNameChars();
            bool result = !(list.Contains(text[0]));
            return result;
        }
    }
}
