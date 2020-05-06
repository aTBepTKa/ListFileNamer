using System.Collections.Generic;

namespace ListFileNamer.Models.Interfaces
{
    public interface IMatchingResult
    {
        string DocName { get; set; }
        string DocNumber { get; set; }
        string FindFolder { get; set; }
        bool FindFolderIsExist { get; set; }
        int GroupId { get; set; }
        int Id { get; set; }
        bool IsAct { get; set; }
        bool IsExactMatch { get; set; }
        bool IsPrimary { get; set; }
        string NewFileName { get; set; }
        int PageNumber { get; set; }
        string ScanFileName { get; set; }
        IEnumerable<string> ScanFileNameVariants { get; set; }
    }
}