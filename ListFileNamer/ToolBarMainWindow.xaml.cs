using ListFileNamer.Models;
using ListFileNamer.Services.CollectFiles;
using ListFileNamer.Services.Excel;
using ListFileNamer.Services.FindScan;
using ListFileNamer.Services.WorkProject;
using Mapster;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ListFileNamer
{
    public partial class MainWindow : Window
    {
        // Назначить пути для работы с файлами.
        private async void LoadProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileWindow = new OpenDataFileWindow(ProjectProperties);
            if (openFileWindow.ShowDialog() == true)
            {
                LoadProgressBar.IsIndeterminate = true;
                StatusLabel.Content = "Идет импорт проекта...";

                ProjectProperties = openFileWindow.ProjectProperties;

                var config = new AppConfigurationManager();
                config.SaveProperties(ProjectProperties);

                try
                {
                    ExcelService = new ExcelService(ProjectProperties);
                    var excelListTask = ExcelService.GetListAsync();

                    FindScanService = new FindScanService(ProjectProperties);                    
                    var resultTask = FindScanService.GetMatchingResultFromExcelAsync(await excelListTask);
                    var resultModel = (await resultTask).Adapt<IEnumerable<MatchingResultViewModel>>();
                    LoadProject(resultModel);

                    resultTask.Wait();
                    LoadProgressBar.IsIndeterminate = false;
                    StatusLabel.Content = "Перечень загружен";
                    SetNewWindowName("Новый проект");
                }
                catch(IOException ex)
                {
                    LoadProgressBar.IsIndeterminate = false;
                    MessageBox.Show($"Ошибка при открытии файла, файл занят другим процессом. Попробуйте закрыть Excel.\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    LoadProgressBar.IsIndeterminate = false;
                    MessageBox.Show($"Ошибка при загрузке данных {ex.Message}");
                }
            }
        }

        // Сохранить проект.
        private async void SaveProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ProjectProperties.ProjectFilePath))
                SaveAsProject_Click(sender, e);
            else
                await WorkProjectService.SaveAsync(MatchingResultModels, ProjectProperties);
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
                var matchingResults = project.MatchingResults?.Adapt<IEnumerable<MatchingResultViewModel>>();
                ProjectProperties = project.ProjectProperties.Adapt<ProjectPropertiesViewModel>();

                LoadProject(matchingResults);
            }
        }

        // Созранить сканы с новыми именами в папку.
        private async void SaveScanButton_Click(object sender, RoutedEventArgs e)
        {
            CollectFilesService collectService = new CollectFilesService();
            StatusLabel.Content = "Кпоирование файлов...";
            IProgress<double> progress = new Progress<double>(percent => LoadProgressBar.Value = percent);
            var collectFilesTask = collectService.CollectFilesAsync(MatchingResultModels, ProjectProperties.SaveResultPath, progress);
            await collectFilesTask;
            collectFilesTask.Wait();
            progress.Report(0);
            StatusLabel.Content = "Копирование файлов завершено";
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

        /// <summary>
        /// Установить результат сравнения файлов.
        /// </summary>
        /// <param name="matchingResults"></param>
        private void LoadProject(IEnumerable<MatchingResultViewModel> matchingResults)
        {
            if (matchingResults == null)
                return;
            MatchingResultModels = new ObservableCollection<MatchingResultViewModel>(matchingResults);
            DocListDG.ItemsSource = MatchingResultModels;
            RowProperties.DataContext = MatchingResultModels;
            SaveScanTextBox.DataContext = ProjectProperties;
            DataContext = ProjectProperties;

            // Настройки интерфейса
            SetNewWindowName(ProjectProperties.ProjectFilePath);
            RowProperties.IsEnabled = true;
        }

        /// <summary>
        /// Установить имя окна.
        /// </summary>
        /// <param name="windowName"></param>
        private void SetNewWindowName(string windowName)
        {
            var programName = "ListFileNamer";
            if (!string.IsNullOrEmpty(windowName))
                Title = windowName + " - " + programName;
        }
    }
}
