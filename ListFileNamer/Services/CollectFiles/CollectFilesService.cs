using ListFileNamer.Models.Interfaces;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        public void CollectFiles(IEnumerable<IMatchingResult> matchingResults, string destinationPath)
        {
            if (!string.IsNullOrEmpty(destinationPath) && !Uri.TryCreate(destinationPath, UriKind.Absolute, out _))
            {
                MessageBox.Show("Не корректный путь папки сохранения.", "Ошибка при задании пути", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (var file in matchingResults)
            {
                var filePath = file.ScanFileName;
                if (string.IsNullOrEmpty(filePath))
                    continue;
                var newFilePath = Path.Combine(destinationPath, file.NewFileName);
                File.Copy(filePath, newFilePath);
            }
        }
    }
}
