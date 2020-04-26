﻿using System;
using System.Collections.Generic;
using System.Text;
using ExcelDataReader;
using System.IO;
using System.Data;
using System.Text.Encodings;
using ListFileNamer.Models;

namespace ListFileNamer.Services
{
    /// <summary>
    /// Средство для получения данных из таблицы Excel.
    /// </summary>
    class ExcelService
    {
        private ColumnNames columnNames;
        private int StartRow { get; set; }
        private int EndRow { get; set; }
        private string FilePath { get; set; }
        public ExcelService(string filePath, int startRow, int endRow)
        {
            FilePath = filePath;
            StartRow = startRow;
            EndRow = endRow;

            // Задать номера стобцов.
            columnNames = new ColumnNames
            {
                SequenceNumber = 0,
                Name = 1,
                Number = 2,
                NumberOfSheets = 4,
                PageNumber = 5
            };
        }

        /// <summary>
        /// Получить данные из файла.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public IEnumerable<ExcelItemModel> GetList()
        {
            var table = GetDataTable(FilePath);

            var excelItems = new List<ExcelItemModel>(table.Rows.Count);
            var idCounter = 1;
            for (int i = StartRow - 1; i < EndRow; i++)
            {
                var tableRow = table.Rows[i];

                var item = new ExcelItemModel
                {
                    Id = idCounter++,
                    SequenceNumber = tableRow[columnNames.SequenceNumber].ToString(),
                    Name = tableRow[columnNames.Name].ToString(),
                    Number = tableRow[columnNames.Number].ToString(),
                    IsPrimary = PrimaryCheck(tableRow[columnNames.SequenceNumber].ToString())
                };
                if (int.TryParse(tableRow[columnNames.NumberOfSheets].ToString(), out int numOfSheetsResult))
                    item.NumberOfSheets = numOfSheetsResult;
                if (int.TryParse(tableRow[columnNames.PageNumber].ToString(), out int pageNumberResult))
                    item.PageNumber = pageNumberResult;

                excelItems.Add(item);
            }
            return excelItems;
        }

        /// <summary>
        /// Получить таблицу из .xlsx (.xls) файла.
        /// </summary>
        /// <param name="filePath">Путь к файлу таблицы.</param>
        /// <param name="tableIndex">Индекс таблицы.</param>
        /// <returns></returns>
        private DataTable GetDataTable(string filePath, int tableIndex = 0)
        {
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var dataSet = reader.AsDataSet();
            var table = dataSet.Tables[tableIndex];
            return table;
        }

        /// <summary>
        /// Проверяет является ли номер первичным.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool PrimaryCheck(string str)
        {
            if (str.Contains('.'))
                return false;
            else
                return true;
        }
    }

    /// <summary>
    /// Номера столбцов.
    /// </summary>
    struct ColumnNames
    {
        /// <summary>
        /// Порядковый номер.
        /// </summary>
        public int SequenceNumber { get; set; }

        /// <summary>
        /// Наименование документа.
        /// </summary>
        public int Name { get; set; }

        /// <summary>
        /// Номер документа.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Количество листов.
        /// </summary>
        public int NumberOfSheets { get; set; }

        /// <summary>
        /// Номер страницы.
        /// </summary>
        public int PageNumber { get; set; }
    }
}
