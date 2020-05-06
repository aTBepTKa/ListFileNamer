﻿using ListFileNamer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Configuration;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using ListFileNamer.Services.FindScan;
using ListFileNamer.Services.Excel;
using ListFileNamer.Services.WorkProject;
using Mapster;
using ListFileNamer.Models.Interfaces;
using ListFileNamer.Services.CollectFiles;

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
        private IProjectProperties ProjectProperties { get; set; }

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
        }

        /// <summary>
        /// Основная коллекция для хранения записей перечня и сканов.
        /// </summary>
        public static ObservableCollection<MatchingResultViewModel> MatchingResultModels { get; set; }

        // Назначить пути для работы с файлами.
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileWindow = new OpenDataFileWindow(ProjectProperties);
            if (openFileWindow.ShowDialog() == true)
            {
                ProjectProperties = openFileWindow.ProjectProperties;

                var config = new AppConfigurationManager();
                config.SaveProperties(ProjectProperties);
                FindScanButton_Click(null, null);
            }
        }

        // Назначить сканы для записей в перечне.
        private void FindScanButton_Click(object sender, RoutedEventArgs e)
        {
            ExcelService = new ExcelService(ProjectProperties);
            var excelList = ExcelService.GetList();
            FindScanService = new FindScanService(ProjectProperties);
            var result = FindScanService.GetMatchingResultFromExcel(excelList).Adapt<IEnumerable<MatchingResultViewModel>>();
            SetMatchingResult(result);
        }

        // Событие изменения выделенной записи в списке файлов.
        private void ListBoxRowTemplate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var fileName = (sender as ListBox).SelectedItem;
            if (fileName != null)
                SetNewFileName((string)fileName);
        }

        // Задать имя нового файла для сохранения.
        private void SetNewFileName(string filePath)
        {
            var item = (MatchingResultViewModel)DocListDG.SelectedItem;
            item.ScanFileName = filePath;
            var fileName = Path.GetFileName(filePath);
            item.NewFileName = $"Стр. {item.PageNumber}. {fileName}";
        }

        // Установить папку сканов для одной строки.
        private void SetScanRowButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (MatchingResultViewModel)DocListDG.SelectedItem;
            var path = ScanFolderTextBox.Text;
            FindScanService.SetScanFolder(item, path);
        }

        // Установить папку сканов для акта и всех документов.
        private void SetScanGroupButton_Click(object sender, RoutedEventArgs e)
        {
            var models = MatchingResultModels;
            var groupId = ((MatchingResultViewModel)DocListDG.SelectedItem).GroupId;
            var path = ScanFolderTextBox.Text;
            FindScanService.SetScanFolder(models, groupId, path);
        }

        // Диалог выбора папки для записи.
        private void SetScanFolderTextBox_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ScanFolderTextBox.SetCurrentValue(TextBox.TextProperty, dialog.FileName);
            }
        }

        // Созранить сканы с новыми именами в папку.
        private void SaveScanButton_Click(object sender, RoutedEventArgs e)
        {
            var destinationPath = ProjectProperties.SaveResultPath;
            CollectFilesService filesService = new CollectFilesService();
            filesService.CollectFiles(MatchingResultModels, destinationPath);
        }

        // Диалог выбора папки для сохранения сканов.
        private void SelectScanButton_Click(object sender, RoutedEventArgs e)
        {

        }

        // Сохранить проект.
        private void SaveProject_Click(object sender, RoutedEventArgs e)
        {

        }

        // Сохранить проект как.
        private async void SaveAsProject_Click(object sender, RoutedEventArgs e)
        {
            await WorkProjectService.SaveAsAsync(MatchingResultModels, ProjectProperties, "project1.lfn");
        }

        // Открыть проект.
        private void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            var project = WorkProjectService.Open("project1.lfn");

            var matchingResults = project.MatchingResults.Adapt<IEnumerable<MatchingResultViewModel>>();
            SetMatchingResult(matchingResults);

            ProjectProperties = project.ServiceProperties.Adapt<ProjectPropertiesViewModel>();
        }

        private void SetMatchingResult(IEnumerable<MatchingResultViewModel> matchingResults)
        {
            MatchingResultModels = new ObservableCollection<MatchingResultViewModel>(matchingResults);
            DocListDG.ItemsSource = MatchingResultModels;
            RowProperties.DataContext = MatchingResultModels;
        }
    }
}
