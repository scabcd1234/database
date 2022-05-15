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
using System.Data;

namespace DataManage
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public class CaseData
        {
            public  int Id { get; set; }                       
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
            SQLiteConnection conn = getConn();
            ShowAllData(conn);

        }

        // 获得数据库连接
        private SQLiteConnection getConn()
        {
            string dbpath = AppDomain.CurrentDomain.BaseDirectory + @"mydb.db";
            string connStr = @"Data Source=" + dbpath + @";Initial Catalog=sqlite;";
            SQLiteConnection conn = new SQLiteConnection(connStr);
            return conn;
        }

        // 显示所有数据
        private void ShowAllData(SQLiteConnection conn)
        {
            List<CaseData> list = new List<CaseData>();
            string sql = "SELECT * FROM casedata";
            
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
            dg1.ItemsSource = null;
            dg1.ItemsSource = list;
        }

        // 增加数据
        void Add_TransfEvent(string value)
        {
            MessageBox.Show(value);
            SQLiteConnection conn = getConn();
            try
            {
                string sql = "insert into casedata (name, score) values (,,)";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("插入数据失败：" + ex.Message);
            }
            ShowAllData(conn);
        }

        private void BtnSelect(object sender, RoutedEventArgs e)
        {
            SQLiteConnection conn = getConn();
            ShowAllData(conn);
        }

        private void BtnRefresh(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAdd(object sender, RoutedEventArgs e)
        {
           AddData adddata=new AddData();
           adddata.TransfEvent += Add_TransfEvent;
           adddata.ShowDialog();          
        }

        private void BtnDelete(object sender, RoutedEventArgs e)
        {
            string sql = "delete from casedata where 1 = 2";
            for (int i = 0; i < dg1.Items.Count; i++)
            {
                //获取行
                DataGridRow neddrow = (DataGridRow)dg1.ItemContainerGenerator.ContainerFromIndex(i);

                //获取该行的某列
                CheckBox cb = (CheckBox)dg1.Columns[0].GetCellContent(neddrow);
                if (cb.IsChecked == true)
                {
                    sql = sql + " or CaseData.Id =" + cb.Tag;                  
                }
            }         
            // MessageBox.Show(sql);
                    
            SQLiteConnection conn = getConn();
            try
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                MessageBox.Show("删除成功","");
            }
            catch (Exception ex)
            {
                throw new Exception("删除数据：" + "失败：" + ex.Message);
            }
            conn.Close();
            ShowAllData(conn);            
        }

        private void BtnUpdate(object sender, RoutedEventArgs e)
        {

        }

        // 全选
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox headercb = (CheckBox)sender;

            for (int i = 0; i < dg1.Items.Count; i++)
            {
                //获取行
                DataGridRow neddrow = (DataGridRow)dg1.ItemContainerGenerator.ContainerFromIndex(i);

                //获取该行的某列
                CheckBox cb = (CheckBox)dg1.Columns[0].GetCellContent(neddrow);

                cb.IsChecked = headercb.IsChecked;
            }
        }

      

        private void dg1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            //if (dg1.SelectedIndex != -1)
            //{
            //    //获取行
            //    DataGridRow neddrow = (DataGridRow)dg1.ItemContainerGenerator.ContainerFromIndex(dg1.SelectedIndex);

            //    //获取该行的某列
            //    CheckBox cb = (CheckBox)dg1.Columns[0].GetCellContent(neddrow);

            //    cb.IsChecked = !cb.IsChecked;
            //}
            
        }

        private void Item_GotFocus(object sender, RoutedEventArgs e)
        {
            
                //获取行
                DataGridRow neddrow = (DataGridRow)dg1.ItemContainerGenerator.ContainerFromIndex(dg1.SelectedIndex);

                if (neddrow != null)
                {
                    //获取该行的某列
                    CheckBox cb = (CheckBox)dg1.Columns[0].GetCellContent(neddrow);

                    cb.IsChecked = !cb.IsChecked;
                }                                 
        }


    }
}
