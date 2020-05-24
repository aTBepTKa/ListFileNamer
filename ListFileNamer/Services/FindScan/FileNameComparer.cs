﻿using ListFileNamer.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ListFileNamer.Services.FindScan
{
    class FileNameComparer
    {
        private readonly string baseFolderPath;
        public FileNameComparer(string basePath)
        {
            baseFolderPath = basePath;
        }

        /// <summary>
        /// Получить сканы для документов перечня.
        /// </summary>
        /// <param name="findModels"></param>
        /// <returns></returns>
        public IEnumerable<FindModel> GetMatchingResults(IEnumerable<FindModel> findModels)
        {
            var models = findModels.ToArray();
            var modelsLength = models.Length;
            var previousFolder = baseFolderPath;
            var groupId = 1;
            for (int i = 0; i < modelsLength; i++)
            {
                var model = models[i];
                FindFindFolder(model, previousFolder);
                FindScanFile(model);

                if (model.IsPrimary)
                    groupId++;
                model.GroupId = groupId;

                previousFolder = model.FindFolder;
            }
            return models;
        }

        /// <summary>
        /// Установить новое имя файла.
        /// </summary>
        /// <param name="matchingResult">Модель данных.</param>
        /// <param name="scanFilePath">Имя файла.</param>
        public static void SetScanFileName(IMatchingResult matchingResult, string scanFilePath)
        {
            matchingResult.ScanFileName = scanFilePath;
            matchingResult.NewDocName = Path.GetFileNameWithoutExtension(scanFilePath);
            matchingResult.FileExtension = Path.GetExtension(scanFilePath);
            matchingResult.NewFileName = $"Стр. {matchingResult.PageNumber}. " +
                $"{matchingResult.NewDocName}" +
                $"{matchingResult.FileExtension}";
        }

        /// <summary>
        /// Назначить папку для выполнения поиска скана.
        /// </summary>
        /// <param name="findModel"></param>
        /// <param name="previousFolderPath"></param>
        private void FindFindFolder(FindModel findModel, string previousFolderPath)
        {
            Regex actRegex = new Regex(@"[0-2]AJ\.(\d{5,6})\.[C|С]-2\.\d{3}.\d", RegexOptions.IgnoreCase);

            findModel.IsAct = actRegex.IsMatch(findModel.DocNumber);

            if (findModel.IsPrimary)
            {
                if (findModel.IsAct)
                {
                    var matches = actRegex.Matches(findModel.DocNumber);
                    var folderName = matches.FirstOrDefault().Groups[1].Value;
                    var findFolder = Path.Combine(baseFolderPath, folderName);
                    findModel.FindFolderIsExist = Directory.Exists(findFolder);
                    findModel.FindFolder = findFolder;
                }
                else
                {
                    findModel.FindFolderIsExist = Directory.Exists(baseFolderPath);
                    findModel.FindFolder = baseFolderPath;
                }
            }
            else
            {
                findModel.FindFolderIsExist = Directory.Exists(previousFolderPath);
                findModel.FindFolder = previousFolderPath;
            }
        }

        /// <summary>
        /// Найти скан документа.
        /// </summary>
        /// <param name="findModel"></param>
        private void FindScanFile(FindModel findModel)
        {    
            var filePathes = GetFolderFiles(findModel.FindFolder);
            findModel.ScanFileNameVariants = filePathes;
            foreach(var filePath in filePathes)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                if (fileName.Contains(findModel.DocNumber))
                    SetScanFileName(findModel, filePath);
            }
        }

        /// <summary>
        /// Получить список файлов из папки.
        /// </summary>
        private IEnumerable<string> GetFolderFiles(string folderPath)
        {
            IEnumerable<string> fileEntries;
            if (Directory.Exists(folderPath))
                fileEntries = Directory.GetFiles(folderPath);
            else
                fileEntries = Array.Empty<string>();
            return fileEntries;
        }
    }
}
