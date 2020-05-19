using System;
using System.Collections.Generic;
using System.Text;

namespace ListFileNamer.Services.WorkProject
{
    /// <summary>
    /// Настройки сервиса работы с проектом.
    /// </summary>
    public interface IWorkProjectProperties
    {
        /// <summary>
        /// Путь к файлу проекта.
        /// </summary>
        string ProjectFilePath { get; set; }
    }
}
