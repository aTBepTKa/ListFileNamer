using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ListFileNamer.Services.DocNameDictionary
{
    /// <summary>
    /// Словарь сокращенных наименований документа.
    /// </summary>
    class DocNameDictionaryService
    {
        public DocNameDictionaryService()
        {
            int id = 1;
            DocDictionaryRecords = new List<DocDictionaryRecord>()
            {
                new DocDictionaryRecord()
                {
                    Id = id++,
                    OriginalName = "Акт освидетельствования скрытых работ",
                    ShortName = "АОСР"
                },
                new DocDictionaryRecord()
                {
                    Id = id++,
                    OriginalName = "Акт",
                    ShortName = "Акт"
                },
                new DocDictionaryRecord()
                {
                    Id = id++,
                    OriginalName = "Сертификат",
                    ShortName = "Серт."
                },
                new DocDictionaryRecord()
                {
                    Id = id++,
                    OriginalName = "Паспорт",
                    ShortName = "Паспорт"
                },
                new DocDictionaryRecord()
                {
                    Id = id++,
                    OriginalName = "Документ о качестве",
                    ShortName = "ДК"
                },
                new DocDictionaryRecord()
                {
                    Id = id++,
                    OriginalName = "Документа о качестве",
                    ShortName = "ДК"
                },
                new DocDictionaryRecord()
                {
                    Id = id++,
                    OriginalName = "Исполнительная схема",
                    ShortName = "ИС"
                },
            };
        }

        private IEnumerable<DocDictionaryRecord> DocDictionaryRecords { get; set; }

        /// <summary>
        /// Сохранить словарь.
        /// </summary>
        public void SaveDictionary()
        {

        }

        /// <summary>
        /// Загрузить словарь.
        /// </summary>
        public void LoadDictionary()
        {

        }

        /// <summary>
        /// Получить сокращенное наименование документа из словаря.
        /// </summary>
        /// <param name="originalName">Наименование оригинального документа.</param>
        /// <returns></returns>
        public string GetShortName(string originalName) =>
            DocDictionaryRecords.FirstOrDefault(x => originalName.Contains(x.OriginalName)).ShortName;

    }
}
