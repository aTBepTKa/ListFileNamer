using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace ListFileNamer.Services.DocNameDictionary
{
    /// <summary>
    /// Словарь сокращенных наименований документа.
    /// </summary>
    public static class DocNameDictionaryService
    {
        static DocNameDictionaryService()
        {
            int id = 1;
            DocDictionaryRecords = new List<DocDictionaryRecord>()
            {
                new DocDictionaryRecord()
                {
                    Id = id++,
                    OriginalRegex = "Акт(|а) освидетельствования скрытых работ",
                    ShortName = "АОСР"
                },
                new DocDictionaryRecord()
                {
                    Id = id++,
                    OriginalRegex = "Акт",
                    ShortName = "Акт"
                },
                new DocDictionaryRecord()
                {
                    Id = id++,
                    OriginalRegex = "Сертификат",
                    ShortName = "Серт."
                },
                new DocDictionaryRecord()
                {
                    Id = id++,
                    OriginalRegex = "Паспорт",
                    ShortName = "Паспорт"
                },
                new DocDictionaryRecord()
                {
                    Id = id++,
                    OriginalRegex = "Документ(|а) о качестве",
                    ShortName = "ДК"
                },
                new DocDictionaryRecord()
                {
                    Id = id++,
                    OriginalRegex = "Исполнительн(ая|ой) схем(а|ы)",
                    ShortName = "ИС"
                },
                new DocDictionaryRecord()
                {
                    Id = id++,
                    OriginalRegex = "Протокол(|а) испытаний",
                    ShortName = "ПИ"
                },
                new DocDictionaryRecord()
                {
                    Id = id++,
                    OriginalRegex = "Протокол",
                    ShortName = "Протокол"
                },
                new DocDictionaryRecord()
                {
                    Id = id++,
                    OriginalRegex = "Декларац",
                    ShortName = "Декл."
                },
                new DocDictionaryRecord()
                {
                    Id = id++,
                    OriginalRegex = "Письм.",
                    ShortName = "Письмо"
                },
                new DocDictionaryRecord()
                {
                    Id = id++,
                    OriginalRegex = "Удостоверен.",
                    ShortName = "Удостоверение"
                },
            };
        }

        private static IEnumerable<DocDictionaryRecord> DocDictionaryRecords { get; set; }

        /// <summary>
        /// Получить сокращенное наименование документа из словаря.
        /// </summary>
        /// <param name="originalName">Наименование оригинального документа.</param>
        /// <returns></returns>
        public static string GetShortName(string originalName)
        {
            // Если оригинал документа содержит ссылку на другой документ и, соотвественно,
            // в названии имеет "...оригинал в акте...", необходимо исключить из словаря
            // слово "Акт".
            var LinkText = "акте";
            IEnumerable<DocDictionaryRecord> dictionaryRecords;
            if (originalName.Contains(LinkText))
                dictionaryRecords = DocDictionaryRecords
                    .Where(x => x.Id != DocDictionaryRecords.FirstOrDefault(i => i.OriginalRegex == "Акт").Id);
            else
                dictionaryRecords = DocDictionaryRecords;

            foreach (var dictionaryName in dictionaryRecords)
            {
                Regex originalNameRegex = new Regex(dictionaryName.OriginalRegex, RegexOptions.IgnoreCase);
                if (originalNameRegex.IsMatch(originalName))
                    return dictionaryName.ShortName;
            }
            return null;
        }

    }
}
