namespace ListFileNamer.Models
{
    /// <summary>
    /// Документ перечня.
    /// </summary>
    public class ExcelItemModel
    {
        public int Id { get; set; }

        /// <summary>
        /// Основной документ.
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Порядковый номер документа.
        /// </summary>
        public string SequenceNumber { get; set; }

        /// <summary>
        /// Наименование документа.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Номер документа.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Количество листов в документе.
        /// </summary>
        public int NumberOfSheets { get; set; }

        /// <summary>
        /// Номер страницы документа в перечне.
        /// </summary>
        public int PageNumber { get; set; }
    }
}
