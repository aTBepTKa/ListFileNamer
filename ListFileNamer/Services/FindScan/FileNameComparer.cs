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
        private readonly IEnumerable<string> baseSubFoldersPath;
        public FileNameComparer(string basePath)
        {
            if (!Directory.Exists(basePath))
                return;
            baseFolderPath = basePath;
            baseSubFoldersPath = Directory.GetDirectories(basePath);
        }

        /// <summary>
        /// Получить сканы для документов перечня.
        /// </summary>
        /// <param name="findModels"></param>
        /// <returns></returns>
        public IEnumerable<FindModel> GetMatchingResults(IEnumerable<FindModel> findModels)
        {
            var models = findModels.ToArray();
            FindModel primaryModel = null;
            var groupId = 0;
            foreach (var model in models)
            {
                SetScan(model, primaryModel);
                if (model.IsPrimary)
                {
                    groupId++;
                    primaryModel = model;
                }
                model.GroupId = groupId;
            }
            return models;
        }

        /// <summary>
        /// Назначить скан документа для записи в перечне.
        /// </summary>
        /// <param name="findModel">Запись перечня.</param>
        /// <param name="folders">Список путей к папкам, содержащим сканы.</param>
        /// <param name="primaryModel">Основная запись перечня, к которой относится запись.</param>
        private void SetScan(FindModel findModel, FindModel primaryModel)
        {
            if (findModel.IsPrimary)
                SetPrimaryScan(findModel);
            else
                SetSecondaryScan(findModel, primaryModel);
        }

        /// <summary>
        /// Установить скан основного докумена.
        /// </summary>
        /// <param name="findModel">Запись перечня.</param>
        private void SetPrimaryScan(FindModel findModel)
        {
            Regex actRegex = new Regex(@"[0-2]AJ\.(\d{5,6})\.[C|С]-2\.\d{3}.\d", RegexOptions.IgnoreCase);
            if (actRegex.IsMatch(findModel.DocNumber))            
                findModel.IsAct = true;

            var findFolderNameModel = GetDigitsFromText(findModel.DocNumber);
            foreach (var folder in baseSubFoldersPath)
            {
                var folderNameDisk = Path.GetFileName(folder);
                var findFolderNameDisk = GetDigitsFromText(folderNameDisk);
                if (findFolderNameDisk.Contains(findFolderNameModel) || findFolderNameModel.Contains(findFolderNameDisk))
                {
                    findModel.FindFolder = folder;
                    findModel.FindFolderIsExist = true;

                    // Назначить папку как из ИСУП исходя из длины ее названия.
                    // Из исупа папка выгружается с именем следующего формата: "Акт № 1AJ.78717.С-2.0563",
                    // при наименовании папок вручную, применяется формат "78717".
                    if (folderNameDisk.Length > 5)
                    {
                        findModel.IsIsup = true;
                        findModel.FindFolder = folder;
                        findModel.ScanFileNameVariants = Directory.GetFiles(folder);

                        var scanFileName = Directory.GetFiles(folder).FirstOrDefault();
                        SetScanFile(findModel, scanFileName);
                        SetNewDocName(findModel);
                        SetNewFileName(findModel);
                    }
                    else
                    {
                        findModel.IsIsup = false;
                        FindAndSetScan(findModel, folder, true);
                    }
                    break;
                }                
            }
            if (string.IsNullOrEmpty(findModel.FindFolder))
            {
                if (!findModel.IsAct)
                {
                    findModel.FindFolder = baseFolderPath;
                    FindAndSetScan(findModel, baseFolderPath);                    
                }
            }
        }

        /// <summary>
        /// Установить скан второстепенного документа.
        /// </summary>
        /// <param name="findModel"></param>
        /// <param name="primaryModel"></param>
        private void SetSecondaryScan(FindModel findModel, FindModel primaryModel)
        {
            if (primaryModel == null)
                return;
            if (primaryModel.IsIsup)
            {
                var findFolder = Directory.GetDirectories(primaryModel.FindFolder).FirstOrDefault();
                findModel.FindFolder = findFolder;
                FindAndSetScan(findModel, findFolder);                
            }
            else
            {
                var findFolder = primaryModel.FindFolder;
                findModel.FindFolder = findFolder;
                FindAndSetScan(findModel, findFolder);
            }
        }

        /// <summary>
        /// Найти скан документа.
        /// </summary>
        /// <param name="findModel">Запись перечня.</param>
        /// <param name="folder">Папка для поиска.</param>
        /// <param name="isAct">Является ли искомы документ актом</param>
        public static void FindAndSetScan(IMatchingResult findModel, string folder, bool isAct = false)
        {
            if (!Directory.Exists(folder))
                return;
            var filePathes = Directory.GetFiles(folder);
            findModel.ScanFileNameVariants = filePathes;

            foreach (var filePath in filePathes)
            {                
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                if (isAct && fileName.Contains("ИС"))
                    continue;

                var findFileName = GetDigitsFromText(fileName);
                if (string.IsNullOrEmpty(findFileName))
                    continue;
                string findDocNumber = GetDigitsFromText(findModel.DocNumber);
                if (findFileName.Contains(findDocNumber) || findDocNumber.Contains(findFileName))
                {
                    SetScanFile(findModel, filePath);
                    SetNewDocName(findModel);
                    SetNewFileName(findModel);
                }
            }
        }

        /// <summary>
        /// Установить новый скан для группы записей: найти скан, установить новое имя документа и новое имя файла.
        /// </summary>
        /// <param name="matchingResults"></param>
        public static void FindAndSetScan(IEnumerable<IMatchingResult> matchingResults,  int groupId, string folder)
        {
            foreach (var result in matchingResults)
            {
                if(result.GroupId == groupId)
                    FindAndSetScan(result, folder);
            }
        }

        /// <summary>
        /// Назначить новый файл со сканом.
        /// </summary>
        /// <param name="matchingResult">Модель данных.</param>
        /// <param name="scanFilePath">Имя файла.</param>
        public static void SetScanFile(IMatchingResult matchingResult, string scanFilePath)
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
        /// Получить цифры из текста.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string GetDigitsFromText(string text)
        {
            string newText = "";
            for (int i = 0; i < text.Length; i++)
            {
                if (char.IsDigit(text[i]))
                    newText += text[i];
            }
            return newText;
        }
    }
}
