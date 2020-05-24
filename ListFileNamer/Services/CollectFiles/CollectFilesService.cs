using ListFileNamer.Models.Interfaces;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ListFileNamer.Services.CollectFiles
{
    /// <summary>
    /// Собрать сканы файлов в папку.
    /// </summary>
    class CollectFilesService
    {
        /// <summary>
        /// Создать структуру файлов и папок сканов для теста работы программы.
        /// </summary>
        public void CreateEmptyFileStructure(string basePath, string destinationPath)
        {
            var files = Directory.GetFiles(basePath, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (string.IsNullOrEmpty(file))
                    continue;

                var relativePath = Path.GetRelativePath(basePath, file);
                var newFilePath = Path.Combine(destinationPath, relativePath);
                var newDirectoryPath = Path.GetDirectoryName(newFilePath);
                if (!Directory.Exists(newDirectoryPath))
                    Directory.CreateDirectory(newDirectoryPath);
                File.Create(newFilePath);
            }
        }

        /// <summary>
        /// Скомпановать файлы сканов согласно перечню.
        /// </summary>
        public async Task CollectFilesAsync(IEnumerable<IMatchingResult> matchingResults, string destinationPath, IProgress<double> progress)
        {
            await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(destinationPath) && !Uri.TryCreate(destinationPath, UriKind.Absolute, out _))
                {
                    MessageBox.Show("Не корректный путь папки сохранения.", "Ошибка при задании пути", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (matchingResults == null)
                {
                    MessageBox.Show("Отсутствует таблица исходных данных", "Ошибка при сохранении файлов", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                try
                {
                    var filesCount = matchingResults.Count();
                    var fileNumber = 0;
                    double completePrecent = 0;
                    foreach (var file in matchingResults)
                    {
                        var filePath = file.ScanFileName;
                        if (string.IsNullOrEmpty(filePath))
                            continue;
                        var newFilePath = Path.Combine(destinationPath, file.NewFileName);
                        File.Copy(filePath, newFilePath, true);
                        
                        fileNumber++;
                        completePrecent = (double)fileNumber / filesCount * 100;
                        progress.Report(completePrecent);
                    }
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка при сохранении файлов", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            });
        }
    }
}
