using ListFileNamer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using ListFileNamer.Services.FindScan;
using ListFileNamer.Services.Excel;
using ListFileNamer.Services.WorkProject;
using Mapster;
using ListFileNamer.Services.CollectFiles;
using Microsoft.Win32;

namespace ListFileNamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Сервисы.
        private ExcelService ExcelService { get; set; }
        private FindScanService FindScanService { get; set; }

        /// <summary>
        /// Текущие настройки проекта.
        /// </summary>
        public ProjectPropertiesViewModel ProjectProperties { get; set; }

        /// <summary>
        /// Сервис сохранения текущих настроек прокта в файл.
        /// </summary>
        private AppConfigurationManager AppConfiguration { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            AppConfiguration = new AppConfigurationManager();
            ProjectProperties = AppConfiguration.GetProjectProperties();
            // Добавить кодировку для чтения .xlsx файла.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Назначить DataContext.
            SaveScanTextBox.DataContext = ProjectProperties;
            DataContext = ProjectProperties;
        }

        /// <summary>
        /// Основная коллекция для хранения записей перечня и сканов.
        /// </summary>
        public static ObservableCollection<MatchingResultViewModel> MatchingResultModels { get; set; }

        /// <summary>
        /// Bзменениt выделенной записи в списке файлов.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxRowTemplate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string filePath = (string)(sender as ListBox).SelectedItem;
            if (filePath != null)
            {
                var item = (MatchingResultViewModel)DocListDG.SelectedItem;
                FindScanService.SetScanFileName(item, filePath);
            }
        }

        // Установить папку сканов для одной строки.
        private void SetScanRowButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (MatchingResultViewModel)DocListDG.SelectedItem;
            var path = ScanFolderTextBox.Text;            
            FindScanService?.SetScanFolder(item, path);
        }

        // Установить папку сканов для акта и всех документов.
        private void SetScanGroupButton_Click(object sender, RoutedEventArgs e)
        {
            var models = MatchingResultModels;
            var groupId = ((MatchingResultViewModel)DocListDG.SelectedItem).GroupId;
            var path = ScanFolderTextBox.Text;
            FindScanService?.SetScanFolder(models, groupId, path);
        }

        // Диалог выбора папки с исходными сканами.
        private void SelectScanFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = GetDefaultFileName(ScanFolderTextBox.Text)
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ScanFolderTextBox.SetCurrentValue(TextBox.TextProperty, dialog.FileName);
            }
        }

        /// <summary>
        /// Получить путь к файлу/папке по умолчанию.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetDefaultFileName(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName) && Uri.TryCreate(fileName, UriKind.Absolute, out _))
                return Path.GetDirectoryName(fileName);
            else
                return ProjectProperties.FindScanServicePath;
        }        
    }
}
