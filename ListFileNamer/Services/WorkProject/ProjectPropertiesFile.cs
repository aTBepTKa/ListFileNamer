using ListFileNamer.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ListFileNamer.Services.WorkProject
{
    /// <summary>
    /// Настройки проекта для сохранения в файл.
    /// </summary>
    class ProjectPropertiesFile : IProjectProperties
    {
        public string ExcelServicePath { get; set; }
        public int StartExcelRow { get; set; }
        public int EndExcelRow { get; set; }
        public string FindScanServicePath { get; set; }
        public string SaveResultPath { get; set; }
        public string ProjectFilePath { get; set; }
    }
}
