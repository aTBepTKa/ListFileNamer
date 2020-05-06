using System;
using System.Collections.Generic;
using System.Text;

namespace ListFileNamer.Services.Excel
{
    /// <summary>
    /// Настройки сервиса.
    /// </summary>
    public interface IExcelServiceProperties
    {
        /// <summary>
        /// Путь к файлу перечня в формате таблицы excel.
        /// </summary>
        string ExcelServicePath { get; set; }

        /// <summary>
        /// Первая строка считывания перечня.
        /// </summary>
        int StartExcelRow { get; set; }

        /// <summary>
        /// Последняя строка считывания перечня.
        /// </summary>
        int EndExcelRow { get; set; }
    }
}
