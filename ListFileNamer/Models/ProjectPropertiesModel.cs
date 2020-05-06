using ListFileNamer.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ListFileNamer.Models
{
    /// <summary>
    /// Настройки проекта.
    /// </summary>
    class ProjectPropertiesModel : IProjectProperties
    {
        public string ExcelServicePath { get; set; }
        public int StartExcelRow { get; set; }
        public int EndExcelRow { get; set; }
        public string FindScanServicePath { get; set; }
        public string SaveResultPath { get; set; }
    }
}
