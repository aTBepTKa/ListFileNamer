using ListFileNamer.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace ListFileNamer.Models
{
    /// <summary>
    /// Результат сопоставления файлов.
    /// </summary>
    public class MatchingResultViewModel : INotifyPropertyChanged, IMatchingResult
    {
        private string scanFileName;
        private string newFileName;
        private string findFolder;
        private bool findFolderIsExist;
        private IEnumerable<string> scanFileNameVariants;
        private string newDocName;

        /// <summary>
        /// Id документа из перечня.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id группы документов.
        /// </summary>
        /// <remarks>Акт с приложениями является одной группой.</remarks>
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
            set => SetField(ref findFolder, value, "FindFolder");
        }

        /// <summary>
        /// Папка для документа существует.
        /// </summary>
        public bool FindFolderIsExist
        {
            get => findFolderIsExist;
            set => SetField(ref findFolderIsExist, value, "findFolderIsExist");
        }

        /// <summary>
        /// Имя найденного файла.
        /// </summary>
        public string ScanFileName
        {
            get => scanFileName;
            set => SetField(ref scanFileName, value, "ScanFileName");
        }

        /// <summary>
        /// Новое наименование документа.
        /// </summary>
        public string NewDocName
        {
            get => newDocName;
            set => SetField(ref newDocName, value, "NewDocName");
        }

        /// <summary>
        /// Новое имя файла.
        /// </summary>
        public string NewFileName
        {
            get => newFileName;
            set => SetField(ref newFileName, value, "NewFileName");
        }

        /// <summary>
        /// Возможные варианты имени файла.
        /// </summary>
        public IEnumerable<string> ScanFileNameVariants
        {
            get => scanFileNameVariants;
            set => SetField(ref scanFileNameVariants, value, "ScanFileNameVariants");
        }

        /// <summary>
        /// Точное совпадение при поиске.
        /// </summary>
        public bool IsExactMatch { get; set; }


        private bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
