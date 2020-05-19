using ListFileNamer.Models;
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
            string filePath = (string)(sender as ListBox).SelectedItem;
            if (filePath != null)
            {
                var item = (MatchingResultViewModel)DocListDG.SelectedItem;
                item.ScanFileName = filePath;
                item.NewDocName = Path.GetFileNameWithoutExtension(filePath);
                var extension = Path.GetExtension(filePath);
                item.NewFileName = $"Стр. {item.PageNumber}. {item.NewDocName}{extension}";
            }
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

        // Созранить сканы с новыми именами в папку.
        private void SaveScanButton_Click(object sender, RoutedEventArgs e)
        {
            CollectFilesService collectService = new CollectFilesService();
            collectService.CollectFiles(MatchingResultModels, ProjectProperties.SaveResultPath);
        }

        // Диалог выбора папки для сохранения сканов.
        private void SelectSaveScanFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = GetDefaultFileName(ProjectProperties.SaveResultPath)
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ProjectProperties.SaveResultPath = dialog.FileName;
            }
        }

        // Сохранить проект.
        private void SaveProject_Click(object sender, RoutedEventArgs e)
        {

        }

        // Сохранить проект как.
        private async void SaveAsProject_Click(object sender, RoutedEventArgs e)
        {            
            var saveFile = new SaveFileDialog()
            {
                InitialDirectory = GetDefaultFileName(ProjectProperties.ProjectFilePath),
                Filter = "Файл проекта List file namer (*.lfn)|*.lfn",
                FileName = Path.GetFileName(ProjectProperties.ProjectFilePath)
            };
            if (saveFile.ShowDialog() == true)
            {
                ProjectProperties.ProjectFilePath = saveFile.FileName;
                await WorkProjectService.SaveAsAsync(MatchingResultModels, ProjectProperties);
            }
        }

        // Открыть проект.
        private void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            var folderPath = GetDefaultFileName(ProjectProperties.ProjectFilePath);
            var project = WorkProjectService.Open(folderPath);
            if (project != null)
            {
                var matchingResults = project.MatchingResults.Adapt<IEnumerable<MatchingResultViewModel>>();
                ProjectProperties = project.ProjectProperties.Adapt<ProjectPropertiesViewModel>();

                SetMatchingResult(matchingResults);
            }
        }

        /// <summary>
        /// Установить результат сравнения файлов.
        /// </summary>
        /// <param name="matchingResults"></param>
        private void SetMatchingResult(IEnumerable<MatchingResultViewModel> matchingResults)
        {
            MatchingResultModels = new ObservableCollection<MatchingResultViewModel>(matchingResults);
            DocListDG.ItemsSource = MatchingResultModels;
            RowProperties.DataContext = MatchingResultModels;
            SaveScanTextBox.DataContext = ProjectProperties;
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

        private void DocListDG_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
