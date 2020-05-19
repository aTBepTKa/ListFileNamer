using System;
using System.Collections.Generic;
using System.Text;

namespace ListFileNamer.Services.WorkProject
{
    /// <summary>
    /// Основной класс для сохранения данных в файл.
    /// </summary>
    class WorkProjectFileModel
    {
        /// <summary>
        /// Настройки сервиса.
        /// </summary>
        public ProjectPropertiesFile ProjectProperties { get; set; }

        /// <summary>
        /// Данные сопоставления.
        /// </summary>
        public IEnumerable<MatchingResultFile> MatchingResults { get; set; }
    }
}
