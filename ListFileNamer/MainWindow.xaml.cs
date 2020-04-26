using ListFileNamer.Models;
using ListFileNamer.Services;
using ListFileNamer.Services.FindScanService;
using ListFileNamer.Services.FindScanService.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Configuration;
using System.IO;

namespace ListFileNamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ExcelService ExcelService { get; set; }
        private string ExcelServicePath { get; set; }
        private int StartExcelRow { get; set; }
        private int EndExcelRow { get; set; }
        private FindScanService FindScanService { get; set; }
        private string FindScanServicePath { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            // Добавить кодировку для чтения .xlsx файла.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        }
        public static ObservableCollection<MatchingResultModel> MatchingResultModels { get; set; }
        public static ObservableCollection<string> OLOLO { get; set; }

        /// <summary>
        /// Назначить рабочие пути.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var config = new AppConfigurationManager();
            var openFileWindow = new OpenDataFileWindow(config.DocListFilePath, config.ScanFolderPath, config.StartRow, config.EndRow);
            if (openFileWindow.ShowDialog() == true)
            {
                ExcelServicePath = openFileWindow.DocListFilePath;
                FindScanServicePath = openFileWindow.ScanFolderPath;
                StartExcelRow = openFileWindow.ExcelFirstRow;
                EndExcelRow = openFileWindow.ExcelLastRow;
                config.SavePathes(ExcelServicePath, FindScanServicePath);
                config.SaveStartEndRows(StartExcelRow, EndExcelRow);
            }
        }

        /// <summary>
        /// Найти файлы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindScanButton_Click(object sender, RoutedEventArgs e)
        {
            ExcelService = new ExcelService(ExcelServicePath, StartExcelRow, EndExcelRow);
            var excelList = ExcelService.GetList();

            FindScanService = new FindScanService(FindScanServicePath);
            var result = FindScanService.GetMatchingResult(excelList);
            MatchingResultModels = new ObservableCollection<MatchingResultModel>(result);
            DocListDG.ItemsSource = MatchingResultModels;
        }

        private void ListBoxRowTemplate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var fileName = (sender as ListBox).SelectedItem;
            if (fileName != null)
                SetNewFileName((string)fileName);

        }

        private void SetNewFileName(string filePath)
        {
            var item = (MatchingResultModel)DocListDG.SelectedItem;
            item.ScanFileName = filePath;
            var fileName = Path.GetFileName(filePath);
            item.NewFileName = $"Стр. {item.PageNumber}. {fileName}";
        }
    }
}
