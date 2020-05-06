﻿using ListFileNamer.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ListFileNamer.Models
{
    /// <summary>
    /// Настройки проекта.
    /// </summary>
    class ProjectPropertiesViewModel : IProjectProperties
    {
        private string saveResultPath;

        public string ExcelServicePath { get; set; }
        public int StartExcelRow { get; set; }
        public int EndExcelRow { get; set; }
        public string FindScanServicePath { get; set; }
        public string SaveResultPath { get => saveResultPath; set => saveResultPath = value; }

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
