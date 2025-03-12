using Microsoft.Win32;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO; // FILE読み込み用

//Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
namespace csvFilterView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataTable dataTable;
        private ICollectionView dataView;
        public MainWindow()
        {
            InitializeComponent();
        }

        // CSV読み込み
        private void OpenCsvButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSVファイル (*.csv)|*.csv|すべてのファイル (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                Encoding encoding = GetSelectedEncoding(); // 選択した文字コードを取得
                LoadCsv(filePath, encoding);
            }
        }
        private Encoding GetSelectedEncoding()
        {
            string selectedEncoding = (EncodingComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            return selectedEncoding == "Shift-JIS" ? Encoding.GetEncoding("Shift_JIS") : Encoding.UTF8;
        }

        private void LoadCsv(string filePath, Encoding encoding)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath, encoding);
                if (lines.Length == 0)
                {
                    MessageBox.Show("CSVファイルが空です。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string[] headers = lines[0].Split(','); // 1行目をヘッダーとして取得
                DataTable dataTable = new DataTable();

                foreach (var header in headers)
                {
                    dataTable.Columns.Add(header);
                }

                foreach (var line in lines.Skip(1))
                {
                    dataTable.Rows.Add(line.Split(','));
                }

                CsvDataGrid.ItemsSource = dataTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"CSVの読み込み中にエラーが発生しました。\n{ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // フィルター適用
        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            if (dataTable == null || cmbColumns.SelectedItem == null) return;

            string columnName = cmbColumns.SelectedItem.ToString();
            string filterValue = txtFilter.Text.Trim();

            if (string.IsNullOrEmpty(filterValue))
            {
                dataTable.DefaultView.RowFilter = string.Empty;
            }
            else
            {
                dataTable.DefaultView.RowFilter = string.Format("[{0}] LIKE '%{1}%'", columnName, filterValue);
            }
        }
    }
}