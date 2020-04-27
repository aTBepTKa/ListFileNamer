using System;
using System.Collections.Generic;
using System.Text;

namespace ListFileNamer.Models
{
    class ScanFile
    {
        public int Id { get; set; }

        /// <summary>
        /// Имя файла.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Путь к файлу.
        /// </summary>
        public string Path { get; set; }
    }
}
