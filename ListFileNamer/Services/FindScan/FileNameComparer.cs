using ListFileNamer.Models.Interfaces;
using ListFileNamer.Services.DocNameDictionary;
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
                SetFindFolder(model, previousFolder);
                FindAndSetScan(model);

                if (model.IsPrimary)
                    groupId++;
                model.GroupId = groupId;

                previousFolder = model.FindFolder;
            }
            return models;
        }

        /// <summary>
        /// Установить новый скан для группы записей: найти скан, установить новое имя документа и новое имя файла.
        /// </summary>
        /// <param name="matchingResults"></param>
        public static void FindAndSetScan(IEnumerable<IMatchingResult> matchingResults)
        {
            foreach(var result in matchingResults)
            {
                FindAndSetScan(result);
            }
        }

        /// <summary>
        /// Установить новый скан для записи: найти скан, установить новое имя документа и новое имя файла.
        /// </summary>
        /// <param name="matchingResult"></param>
        public static void FindAndSetScan(IMatchingResult matchingResult)
        {
            FindScanFile(matchingResult);
            SetNewDocName(matchingResult);
            SetNewFileName(matchingResult);
        }

        /// <summary>
        /// Установить новое имя файла.
        /// </summary>
        /// <param name="matchingResult">Модель данных.</param>
        /// <param name="scanFilePath">Имя файла.</param>
        public static void SetScanFileName(IMatchingResult matchingResult, string scanFilePath)
        {
            matchingResult.ScanFileName = scanFilePath;
            matchingResult.FileExtension = Path.GetExtension(scanFilePath);
            if (string.IsNullOrEmpty(matchingResult.NewDocName))
            {
                matchingResult.NewDocName = Path.GetFileNameWithoutExtension(scanFilePath);
                SetNewFileName(matchingResult);
            }
        }

        /// <summary>
        /// Установить новое имя документа.
        /// </summary>
        /// <param name="matchingResult"></param>
        /// <param name="scanFilePath"></param>
        public static void SetNewDocName(IMatchingResult matchingResult)
        {
            var newDocName = DocNameDictionaryService.GetShortName(matchingResult.DocName);
            if (!string.IsNullOrEmpty(newDocName))
            {
                matchingResult.NewDocName = newDocName + " № " + matchingResult.DocNumber;
            }
            else
            {
                var scanFilePath = matchingResult.ScanFileName;
                if (scanFilePath == null)
                    return;
                matchingResult.NewDocName = Path.GetFileNameWithoutExtension(scanFilePath);                
            }
        }

        /// <summary>
        /// Установить новое имя файла.
        /// </summary>
        /// <param name="matchingResult"></param>
        public static void SetNewFileName(IMatchingResult matchingResult)
        {
            var newFileName = $"Стр. {matchingResult.PageNumber}. " +
                $"{matchingResult.NewDocName}" +
                $"{matchingResult.FileExtension}";
            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                newFileName = newFileName.Replace(c, '_');
            }
            matchingResult.NewFileName = newFileName;
        }

        /// <summary>
        /// Назначить папку для выполнения поиска скана.
        /// </summary>
        /// <param name="findModel"></param>
        /// <param name="previousFolderPath"></param>
        private void SetFindFolder(IMatchingResult findModel, string previousFolderPath)
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
        private static void FindScanFile(IMatchingResult findModel)
        {
            var filePathes = GetFolderFiles(findModel.FindFolder);
            findModel.ScanFileNameVariants = filePathes;
            foreach (var filePath in filePathes)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                string findFileName = GetDigitsFromText(fileName);
                string findDocNumber = GetDigitsFromText(findModel.DocNumber);
                if (findFileName.Contains(findDocNumber) || findDocNumber.Contains(findFileName))
                    SetScanFileName(findModel, filePath);
            }
        }

        /// <summary>
        /// Получить список файлов из папки.
        /// </summary>
        private static IEnumerable<string> GetFolderFiles(string folderPath)
        {
            IEnumerable<string> fileEntries;
            if (Directory.Exists(folderPath))
                fileEntries = Directory.GetFiles(folderPath);
            else
                fileEntries = Array.Empty<string>();
            return fileEntries;
        }

        /// <summary>
        /// Получить цифры из текста.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string GetDigitsFromText(string text)
        {
            string newText = "";
            for(int i = 0; i < text.Length; i++)
            {
                if (char.IsDigit(text[i]))
                    newText += text[i];
            }
            return newText;
        }
    }
}
