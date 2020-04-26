using ListFileNamer.Models;
using ListFileNamer.Services.FindScanService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using Mapster;

namespace ListFileNamer.Services.FindScanService
{
    /// <summary>
    /// Сопоставляет записи из перечня со сканами документов.
    /// </summary>
    class FindScanService
    {
        private readonly string baseFolderPath;
        public FindScanService(string path)
        {
            baseFolderPath = path;
        }

        /// <summary>
        /// Получить сканы соответствующие документам в перечне.
        /// </summary>
        /// <param name="excelItems">Документы перечня.</param>
        /// <returns></returns>
        public IEnumerable<MatchingResultModel> GetMatchingResult(IEnumerable<ExcelItemModel> excelItems)
        {
            var findModels = excelItems.Select(x => new FindModel
            {                
                Id = x.Id,
                PageNumber = x.PageNumber,
                DocName = x.Name,
                DocNumber = x.Number,
                IsPrimary = x.IsPrimary
            }).ToArray();

            var comparer = new FileNameComparer(baseFolderPath);
            var compareResult = comparer.GetMatchingResults(findModels);
            var resultModel = compareResult.Adapt<IEnumerable<MatchingResultModel>>();
            return resultModel;
        }
    }
}
