using ListFileNamer.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using ListFileNamer.Models;
using System.Text.RegularExpressions;

namespace ListFileNamer
{
    /// <summary>
    /// Логика взаимодействия для OpenDataFileWindow.xaml
    /// </summary>
    public partial class OpenDataFileWindow : Window
    {
        public string DocListFilePath { get; set; }
        public string ScanFolderPath { get; set; }
        public int ExcelFirstRow { get; set; }
        public int ExcelLastRow { get; set; }

        public IEnumerable<ExcelItemModel> DocListItems { get; set; }
        public OpenDataFileWindow(string filePath, string folderPath, int startRow, int endRow)
        {
            InitializeComponent();
            DocListPathTextBox.Text = filePath;
            ScanPathTextBox.Text = folderPath;

            FirstRowTextBox.Text = startRow.ToString();
            LastRowTextBox.Text = endRow.ToString();
        }

        private static readonly Regex integerRegex = new Regex(@"^[1-9]\d*$");

        private void DocListPathButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Таблица excel |*.xls; *.xlsx; *.xlsm"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                DocListPathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void ScanPathPathButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                ScanPathTextBox.Text = dialog.FileName;
            Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            var docListPath = DocListPathTextBox.Text;
            if (!string.IsNullOrEmpty(docListPath) && !Uri.TryCreate(docListPath, UriKind.Absolute, out _))
            {
                MessageBox.Show("Не корректный путь к файлу таблицы.", "Ошибка при задании пути", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var scanPath = ScanPathTextBox.Text;
            if (!string.IsNullOrEmpty(scanPath) && !Uri.TryCreate(scanPath, UriKind.Absolute, out _))
            {
                MessageBox.Show("Не корректный путь к папке сканов.", "Ошибка при задании пути", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var startRowText = FirstRowTextBox.Text;
            var lastRowText = LastRowTextBox.Text;

            if(!int.TryParse(startRowText, out int startRowInt))
            {
                MessageBox.Show("Не корректное значение первой строки.", "Ошибка при получении строки", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(!int.TryParse(lastRowText, out int lastRowInt))
            {
                MessageBox.Show("Не корректное значение последней строки.", "Ошибка при получении строки", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DocListFilePath = docListPath;
            ScanFolderPath = scanPath;
            ExcelFirstRow = startRowInt;
            ExcelLastRow = lastRowInt;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        
        private static bool IsTextAllowed(string text) =>
            !integerRegex.IsMatch(text);
        private void FirstRowTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextAllowed(e.Text);
        }

        private void LastRowTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextAllowed(e.Text);
        }
    }
}
