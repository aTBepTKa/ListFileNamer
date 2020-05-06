using System;
using System.Collections.Generic;
using System.Text;

namespace ListFileNamer.Services.FindScan
{
    public interface IFindScanServiceProperties
    {
        /// <summary>
        /// Путь к папке со сканами
        /// </summary>
        string FindScanServicePath { get; set; }
    }
}
