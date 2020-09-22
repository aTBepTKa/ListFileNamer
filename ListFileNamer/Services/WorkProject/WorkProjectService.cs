using ListFileNamer.Models;
using ListFileNamer.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Windows;
using System.Threading.Tasks;
using System.IO.Compression;
using Mapster;
using Microsoft.Win32;
using System.Linq;

namespace ListFileNamer.Services.WorkProject
{
    /// <summary>
    /// Сервис для работы с текущим проектом. Представляет средства для сохранения проекта на диск, чтения проекта с диска.
    /// </summary>
    static class WorkProjectService
    {
        /// <summary>
        /// Сохранить проект.
        /// </summary>
        public static async Task SaveAsync(IEnumerable<IMatchingResult> matchingResult, IProjectProperties projectProperties)
        {
            await SaveAsAsync(matchingResult, projectProperties);
        }

        /// <summary>
        /// Сохранить проект как.
        /// </summary>
        public static async Task SaveAsAsync(IEnumerable<IMatchingResult> matchingResult, IProjectProperties projectProperties)
        {
            try
            {
                var fileModel = new WorkProjectFileModel()
                {
                    MatchingResults = matchingResult.Adapt<IEnumerable<MatchingResultFile>>(),
                    ProjectProperties = projectProperties.Adapt<ProjectPropertiesFile>()
                };
                
                // Поток записи в файл
                using var fileStream = new FileStream(projectProperties.ProjectFilePath, FileMode.OpenOrCreate);
                using var zipStream = new GZipStream(fileStream, CompressionLevel.Optimal);

                await JsonSerializer.SerializeAsync(zipStream, fileModel);                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при записи в файл: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Открыть проект.
        /// </summary>
        public static WorkProjectFileModel Open(string path)
        {
            string fileName;
            var openFile = new OpenFileDialog()
            {
                InitialDirectory = path,
                Filter = "Файл проекта List file namer (*.lfn)|*.lfn"
            };
            if (openFile.ShowDialog() == true)
                fileName = openFile.FileName;
            else
                return null;

            WorkProjectFileModel workProject;
            try
            {
                using var fileStream = new FileStream(fileName, FileMode.OpenOrCreate);
                var zipStream = new GZipStream(fileStream, CompressionMode.Decompress);
                var streamReader = new StreamReader(zipStream);
                var jsonText = streamReader.ReadToEnd();
                workProject = JsonSerializer.Deserialize<WorkProjectFileModel>(jsonText);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                workProject = null;
            }
            return workProject;
        }
    }
}
