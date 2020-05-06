﻿using ListFileNamer.Services;
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
using System.Text.RegularExpressions;
using ListFileNamer.Services.Excel;
using ListFileNamer.Models.Interfaces;
using ListFileNamer.Models;

namespace ListFileNamer
{
    /// <summary>
    /// Логика взаимодействия для OpenDataFileWindow.xaml
    /// </summary>
    public partial class OpenDataFileWindow : Window
    {
        public IProjectProperties ProjectProperties { get; set; }

        public OpenDataFileWindow(IProjectProperties projectProperties)
        {
            InitializeComponent();
            DocListPathTextBox.Text = projectProperties.ExcelServicePath;
            ScanPathTextBox.Text = projectProperties.FindScanServicePath;

            FirstRowTextBox.Text = projectProperties.StartExcelRow.ToString();
            LastRowTextBox.Text = projectProperties.EndExcelRow.ToString();
        }

        private static readonly Regex integerRegex = new Regex(@"^[0-9]\d*$");

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

            ProjectProperties = new ProjectPropertiesViewModel()
            {
                ExcelServicePath = docListPath,
                FindScanServicePath = scanPath,
                StartExcelRow = startRowInt,
                EndExcelRow = lastRowInt
            };

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
