﻿using ListFileNamer.Models;
using Mapster;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ListFileNamer.Services
{
    /// <summary>
    /// Сервис для поиска и работы со сканами документов и сопоставления их с записями перечня.
    /// </summary>
    class FindScanService
    {
        private readonly string baseFolderPath;
        private IEnumerable<FindModel> FindModels { get; set; }
        public FindScanService(string path)
        {
            baseFolderPath = path;
        }

        /// <summary>
        /// Получить сканы соответствующие документам в перечне.
        /// </summary>
        /// <param name="excelItems">Документы перечня.</param>
        /// <returns></returns>
        public IEnumerable<MatchingResultViewModel> GetMatchingResultFromExcel(IEnumerable<ExcelItemModel> excelItems)
        {
            FindModels = excelItems.Select(x => new FindModel
            {
                Id = x.Id,
                PageNumber = x.PageNumber,
                DocName = x.Name,
                DocNumber = x.Number,
                IsPrimary = x.IsPrimary                
            }).ToArray();            

            var comparer = new FileNameComparer(baseFolderPath);
            var compareResult = comparer.GetMatchingResults(FindModels);
            var resultModel = compareResult.Adapt<IEnumerable<MatchingResultViewModel>>();
            return resultModel;
        }

        /// <summary>
        /// Установить папку поиска скана для одной записи.
        /// </summary>
        /// <param name="model">Объект записи.</param>
        /// <param name="path">Адрес папки поиска.</param>
        /// <returns>Возвращает успешность задания пути.</returns>
        public bool SetScanFolderRecord(MatchingResultViewModel model, string path)
        {
            if (!Directory.Exists(path))
                return false;
            SetScanFolder(model, path);
            return true;
        }

        /// <summary>
        /// Установить папку посика скана для группы записей.
        /// </summary>
        /// <param name="model">Объект записи.</param>
        /// <param name="groupId">Id группы для задания папки.</param>
        /// <param name="path">Адрес папки поиска.</param>
        /// <returns>Возвращает успешность задания пути.</returns>
        public bool SetScanFolderGroup(IEnumerable<MatchingResultViewModel> models, int groupId, string path)
        {
            if (!Directory.Exists(path))
                return false;
            foreach(var model in models)
            {
                if (model.GroupId == groupId)
                    SetScanFolder(model, path);
            }
            return true;

        }

        /// <summary>
        /// Установить папку поиска скана.
        /// </summary>
        /// <param name="model">Объект записи.</param>
        /// <param name="path">Адрес папки поиска.</param>
        private void SetScanFolder(MatchingResultViewModel model, string path)
        {
            model.FindFolder = path;
            model.FindFolderIsExist = true;
            model.ScanFilePathVariants = Directory.GetFiles(path);
        }
    }
}
