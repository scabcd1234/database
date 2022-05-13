using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Data.SQLite;

namespace DataManage
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public class CaseData
        {
            public int Id { get; set; }
            public string UserName { get; set; }
            public string Rank { get; set; }
            public string  Content { get; set; }
            public string PostDate { get; set; }
            public string Forward { get; set; }
            public string Comment { get; set; }
            public string Likes { get; set; }       
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<CaseData> list = new List<CaseData>();
            string sql = "SELECT * FROM casedata";
            string connStr = @"Data Source=" + @"E:\c#study\mydb.db;Initial Catalog=sqlite;";
            SQLiteConnection conn = new SQLiteConnection(connStr);
            
            try
            {   
                conn.Open();
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CaseData casedata = new CaseData();
                    casedata.Id = (int)reader["id"];
                    casedata.UserName = reader["username"].ToString();
                    casedata.Rank = reader["rank"].ToString();
                    casedata.Content = reader["content"].ToString();
                    casedata.PostDate = reader["postDate"].ToString();
                    casedata.Forward = reader["forwarding"].ToString();
                    casedata.Comment = reader["comment"].ToString();
                    casedata.Likes = reader["likes"].ToString();                 
                    list.Add(casedata);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("查询数据失败：" + ex.Message);
            }
            conn.Close();
            dg1.ItemsSource = list;

        }

        private void dg1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CaseData selected_case= (sender as DataGrid).SelectedItem as CaseData;
            MessageBox.Show(selected_case.UserName, "");
        }

        private void BtnSelect(object sender, RoutedEventArgs e)
        {

        }

        private void BtnRefresh(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAdd(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDelete(object sender, RoutedEventArgs e)
        {

        }

        private void BtnUpdate(object sender, RoutedEventArgs e)
        {

        }
    }
}
