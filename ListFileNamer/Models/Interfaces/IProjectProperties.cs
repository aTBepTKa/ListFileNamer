using ListFileNamer.Services.CollectFiles;
using ListFileNamer.Services.Excel;
using ListFileNamer.Services.FindScan;
using System;
using System.Collections.Generic;
using System.Text;

namespace ListFileNamer.Models.Interfaces
{
    /// <summary>
    ///  Настройки сервисов и проекта.
    /// </summary>
    public interface IProjectProperties : IExcelServiceProperties, IFindScanServiceProperties, ICollectFilesProperties
    {

    }
}
