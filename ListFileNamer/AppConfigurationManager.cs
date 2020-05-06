using ListFileNamer.Models;
using ListFileNamer.Models.Interfaces;
using System.Configuration;
using System.Security.Policy;

namespace ListFileNamer
{
    /// <summary>
    /// Работа с настройками приложения, сохраняемыми в файл.
    /// </summary>
    class AppConfigurationManager
    {
        private readonly string DocListFilePathKey = "DocListFilePath";
        private readonly string ScanFolderPathKey = "ScanFolderPath";
        private readonly string StartRowKey = "ExcelStartRow";
        private readonly string EndRowKey = "ExcelEndRow";

        /// <summary>
        /// Сохранить настройки проекта в файл.
        /// </summary>
        /// <param name="projectProperties"></param>
        public void SaveProperties(IProjectProperties projectProperties)
        {
            SaveProperty(DocListFilePathKey, projectProperties.ExcelServicePath);
            SaveProperty(StartRowKey, projectProperties.StartExcelRow);
            SaveProperty(EndRowKey, projectProperties.EndExcelRow);

            SaveProperty(ScanFolderPathKey, projectProperties.FindScanServicePath);
        }

        /// <summary>
        /// Загрузить настройки проекта из файла.
        /// </summary>
        /// <returns></returns>
        public IProjectProperties GetProjectProperties()
        {
            var projectProperties = new ProjectPropertiesViewModel()
            {
                ExcelServicePath = ConfigurationManager.AppSettings[DocListFilePathKey],
                FindScanServicePath = ConfigurationManager.AppSettings[ScanFolderPathKey]
            };

            if (int.TryParse(ConfigurationManager.AppSettings[StartRowKey], out int startRowParse))
                projectProperties.StartExcelRow = startRowParse;
            else
                projectProperties.StartExcelRow = 1;

            if (int.TryParse(ConfigurationManager.AppSettings[EndRowKey], out int endRowParse))
                projectProperties.EndExcelRow = endRowParse;
            else
                projectProperties.EndExcelRow = 1;

            return projectProperties;
        }

        /// <summary>
        /// Сохранить настройку в файл.
        /// </summary>
        private void SaveProperty(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(key, value);
            config.Save();
        }

        /// <summary>
        /// Сохранить настройку в файл.
        /// </summary>
        private void SaveProperty(string key, int value) =>
            SaveProperty(key, value.ToString());
    }
}
