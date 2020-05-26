using ListFileNamer.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ListFileNamer.Models
{
    /// <summary>
    /// Настройки проекта.
    /// </summary>
    public class ProjectPropertiesViewModel : IProjectProperties, INotifyPropertyChanged
    {
        private string saveResultPath;
        private string projectFilePath;
        private bool isNewProject = true;
        private bool isShowRowDetailsTemplate = true;

        public string ExcelServicePath { get; set; }
        public int StartExcelRow { get; set; }
        public int EndExcelRow { get; set; }
        public string FindScanServicePath { get; set; }
        public string SaveResultPath { get => saveResultPath; set => SetField(ref saveResultPath, value, "SaveResultPath"); }
        public string ProjectFilePath
        {
            get => projectFilePath;
            set
            {
                projectFilePath = value;
                IsNewProject = false;
            }
        }
        public bool IsNewProject { get => isNewProject; set => SetField(ref isNewProject, value, "IsNewProject"); }
        public bool IsShowRowDetailsTemplate { get => isShowRowDetailsTemplate; set => SetField(ref isShowRowDetailsTemplate, value, "IsShowRowDetailsTemplate"); }

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
