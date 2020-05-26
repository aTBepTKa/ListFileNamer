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

        /// <summary>
        /// Проект является новым, путь для сохранения не назнчаен.
        /// </summary>
        bool IsNewProject { get; set; }

        /// <summary>
        /// Показывать примечания к строке в DataGrid.
        /// </summary>
        bool IsShowRowDetailsTemplate { get; set; }
    }
}
