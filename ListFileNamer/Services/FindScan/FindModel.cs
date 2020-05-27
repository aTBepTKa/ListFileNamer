using ListFileNamer.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ListFileNamer.Services.FindScan
{
    /// <summary>
    /// Модель для поиска файлов и папок.
    /// </summary>
    public class FindModel : IMatchingResult
    {
        private string findFolder;

        /// <summary>
        /// Id документа из перечня.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id группы документов.
        /// </summary>
        /// <remarks>
        /// Акт с приложениями является одной группой.
        /// </remarks>
        public int GroupId { get; set; }

        /// <summary>
        /// Номер страницы документа.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Основной документ.
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Документ является актом (АОСР).
        /// </summary>
        public bool IsAct { get; set; }

        /// <summary>
        /// Структура папок соответствует структуре выгруженных документов из ИСУП.
        /// </summary>
        public bool IsIsup { get; set; }

        /// <summary>
        /// Наименование документа из перечня
        /// </summary>
        public string DocName { get; set; }

        /// <summary>
        /// Номер документа из перечня.
        /// </summary>
        public string DocNumber { get; set; }

        /// <summary>
        /// Папка для выполнения поиска документа.
        /// </summary>
        public string FindFolder
        {
            get => findFolder;
            set
            {                
                findFolder = value;
                if (string.IsNullOrEmpty(value))
                    FindFolderIsExist = false;
                else
                    FindFolderIsExist = true;
            }
        }

        /// <summary>
        /// Папка для документа существует.
        /// </summary>
        public bool FindFolderIsExist { get; set; }

        /// <summary>
        /// Имя найденного файла.
        /// </summary>
        public string ScanFileName { get; set; }

        /// <summary>
        /// Новое наименование документа.
        /// </summary>
        public string NewDocName { get; set; }

        /// <summary>
        /// Новое имя файла.
        /// </summary>
        public string NewFileName { get; set; }

        /// <summary>
        /// Возможные варианты имени файла.
        /// </summary>
        public IEnumerable<string> ScanFileNameVariants { get; set; }

        /// <summary>
        /// Точное совпадение при поиске.
        /// </summary>
        public bool IsExactMatch { get; set; }
        public string FileExtension { get; set; }
    }
}
