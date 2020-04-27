using System.Configuration;

namespace ListFileNamer
{
    /// <summary>
    /// Работа с настройками приложения.
    /// </summary>
    class AppConfigurationManager
    {
        public string DocListFilePath => ConfigurationManager.AppSettings[DocListFilePathKey];
        public string ScanFolderPath => ConfigurationManager.AppSettings[ScanFolderPathKey];
        public int StartRow
        {
            get
            {
                if (int.TryParse(ConfigurationManager.AppSettings[StartRowKey], out int startRow))
                    return startRow;
                else
                    return 12;
            }
        }
        public int EndRow
        {
            get
            {
                if (int.TryParse(ConfigurationManager.AppSettings[EndRowKey], out int endRow))
                    return endRow;
                else
                    return 12;
            }
        }


        private readonly string DocListFilePathKey = "DocListFilePath";
        private readonly string ScanFolderPathKey = "ScanFolderPath";
        private readonly string StartRowKey = "ExcelStartRow";
        private readonly string EndRowKey = "ExcelEndRow";

        /// <summary>
        /// Сохранить пути к файлам.
        /// </summary>
        /// <param name="docListFilePath">Расположение перечня.</param>
        /// <param name="scanFolderPath">Расположение папки со сканами.</param>
        public void SavePathes(string docListFilePath, string scanFolderPath)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove(DocListFilePathKey);
            config.AppSettings.Settings.Add(DocListFilePathKey, docListFilePath);

            config.AppSettings.Settings.Remove(ScanFolderPathKey);
            config.AppSettings.Settings.Add(ScanFolderPathKey, scanFolderPath);

            config.Save();
        }

        /// <summary>
        /// Сохранить начальную и конечную строку считывания данных.
        /// </summary>
        /// <param name="startRow">Начальная строка.</param>
        /// <param name="endRow">Конечная строка.</param>
        public void SaveStartEndRows(string startRow, string endRow)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings.Remove(StartRowKey);
            config.AppSettings.Settings.Add(StartRowKey, startRow);

            config.AppSettings.Settings.Remove(EndRowKey);
            config.AppSettings.Settings.Add(EndRowKey, endRow);

            config.Save();
        }

        /// <summary>
        /// Сохранить начальную и конечную строку считывания данных.
        /// </summary>
        /// <param name="startRow">Начальная строка.</param>
        /// <param name="endRow">Конечная строка.</param>
        public void SaveStartEndRows(int startRow, int endRow) =>
            SaveStartEndRows(startRow.ToString(), endRow.ToString());
    }
}
