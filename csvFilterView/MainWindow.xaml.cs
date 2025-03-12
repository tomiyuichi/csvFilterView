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

namespace csvFilterView;

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
    private void LoadCsv_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
            Title = "CSVファイルを選択"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            string filePath = openFileDialog.FileName;
            dataTable = new DataTable();

            // CSVを読み込む
            string[] lines = File.ReadAllLines(filePath);
            if (lines.Length == 0) return;

            // ヘッダー行（カラム名）を取得
            string[] headers = lines[0].Split(',');
            foreach (string header in headers)
            {
                dataTable.Columns.Add(header.Trim());
            }

            // データ行を追加
            foreach (string line in lines.Skip(1))
            {
                dataTable.Rows.Add(line.Split(','));
            }

            // DataGridにデータをバインド
            dataGrid.ItemsSource = dataTable.DefaultView;

            // カラム選択用のComboBoxを更新
            cmbColumns.ItemsSource = headers;
            if (cmbColumns.Items.Count > 0) cmbColumns.SelectedIndex = 0;
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