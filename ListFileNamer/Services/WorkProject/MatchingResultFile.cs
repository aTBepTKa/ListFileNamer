using ListFileNamer.Models.Interfaces;
using System.Collections.Generic;

namespace ListFileNamer.Services.WorkProject
{
    /// <summary>
    /// Таблица сопоставления сканов для сохранения в файл.
    /// </summary>
    class MatchingResultFile : IMatchingResult
    {
        public string DocName { get; set; }
        public string DocNumber { get; set; }
        public string FindFolder { get; set; }
        public bool FindFolderIsExist { get; set; }
        public int GroupId { get; set; }
        public int Id { get; set; }
        public bool IsAct { get; set; }
        public bool IsExactMatch { get; set; }
        public bool IsPrimary { get; set; }
        public string NewDocName { get; set; }
        public string NewFileName { get; set; }
        public int PageNumber { get; set; }
        public string ScanFileName { get; set; }
        public IEnumerable<string> ScanFileNameVariants { get; set; }
    }
}
