using System;
using System.Collections.Generic;
using System.Text;

namespace ListFileNamer.Services.CollectFiles
{
    /// <summary>
    /// Собрать файлы в папку.
    /// </summary>
    public interface ICollectFilesProperties
    {
        /// <summary>
        /// Путь к папке сохранения переименованных сканов (результат рарботы программы).
        /// </summary>
        string SaveResultPath { get; set; }
    }
}
