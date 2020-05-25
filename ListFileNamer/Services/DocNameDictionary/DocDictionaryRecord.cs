using System;
using System.Collections.Generic;
using System.Text;

namespace ListFileNamer.Services.DocNameDictionary
{
    /// <summary>
    /// Пара полное наименование документа - сокращенное наименование документа.
    /// </summary>
    class DocDictionaryRecord
    {
        public int Id { get; set; }

        /// <summary>
        /// Исходное наименование докумена
        /// </summary>
        public string OriginalRegex { get; set; }

        /// <summary>
        /// Сокращенное наименование документа.
        /// </summary>
        public string ShortName { get; set; }
    }
}
