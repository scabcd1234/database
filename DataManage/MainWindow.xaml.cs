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
using System.Data.Odbc;
using System.Data.SqlClient;


namespace DataManage
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        //当前页
        public static int pageIndex = 1;
        //总页数
        public static int totalPage = 0;
        //页大小
        public static int pageSize = 4;

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

        // 分页查询数据
        private void ShowAllData(SQLiteConnection conn)
        {
            List<caseData> list = new List<caseData>();

            SQLiteConnection conn1 = getConn();
            int Count = selectData(conn1);
            totalPage = Count % pageSize == 0 ? Count / pageSize : Count / pageSize + 1;
            if (pageIndex > totalPage)
            {
                pageIndex = totalPage;
            }
            if (pageIndex == 0 && totalPage != 0)
            {
                pageIndex = 1;
            }
            int index = (pageIndex - 1) * pageSize;
            /*string sql = "SELECT * FROM casedata " + "limit " + index + "," + pageSize;*/
            
            string sql1 = "SELECT * FROM data " ;
            string sql2 = "where 1 = 1 ";
            string sql3 = "limit " + index + "," + pageSize;
            
            if (SearchField.Text.Trim() != "")
            {
                sql2 += " and diff_plane like '" + SearchField.Text.Trim() + "%' ";
            }
            string sql = sql1 + sql2 + sql3;
            
            try
            {
                conn.Open();
                
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    caseData casedata = new caseData();
                    casedata.Id = Convert.ToInt32(reader["id"]);
                    casedata.Phase = reader["phase"].ToString();
                    casedata.Phase_ratio = (int)reader["phase_ratio"];
                    casedata.Temperature = (int)reader["temperature"];
                    casedata.Diff_plane = reader["diff_plane"].ToString();
                    casedata.Ehkl = (int)reader["ehkl"];
                    casedata.Vhkl = Convert.ToDouble(reader["vhkl"]);                  
                    if (reader["distance"].ToString() == "")
                    {
                        
                        casedata.Distance = 0.00;
                    }
                    else
                    {
                        casedata.Distance = Convert.ToDouble(reader["distance"]);
                    }
                    
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
 
            if (totalPage <= 1)
            {
                BtnNext.IsEnabled = false;
                BtnUp.IsEnabled = false;
            }else if(pageIndex < totalPage && pageIndex >1)
            {
                BtnNext.IsEnabled = true;
                BtnUp.IsEnabled = true;
            }else if (pageIndex < totalPage && pageIndex == 1)
            {
                BtnUp.IsEnabled = false;
                BtnNext.IsEnabled = true;
            }else if (pageIndex == totalPage && pageIndex >1)
            {
                BtnNext.IsEnabled = false;
                BtnUp.IsEnabled = true;
            }

            
            AllPage.Content = totalPage.ToString();
            CurrentPage.Content = pageIndex.ToString();      
        }

        // 查询所有数据
        private int selectData(SQLiteConnection conn)
        {
            List<caseData> listAll = new List<caseData>();
            string sql1 = "select count(*) from data ";
            string sql2 = "where 1 = 1 ";
            
            if(SearchField.Text.Trim() != "")
            {
                sql2 += " and diff_plane like '" + SearchField.Text.Trim() + "%'";
            }
            
            string sqlAll = sql1 + sql2;
            int count = 0;
            try
            {
                
                conn.Open();
                SQLiteCommand command = new SQLiteCommand(sqlAll, conn);
                SQLiteDataReader reader = command.ExecuteReader();
                
                if (reader.Read())
                {
                    
                    count = Convert.ToInt32(reader[0].ToString());
                    
                }               
            
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询失败","");
                throw new Exception("查询数据失败：" + ex.Message);
            }
            conn.Close();
            return count;

        }
        
        // 增加数据
        void Add_TransfEvent(caseData caseData)
        {
            
            SQLiteConnection conn = getConn();
            /*int a = 80;*/
            try
            {
                conn.Open();
                /*insert into casedata(id, username, rank) values(70, '10', '生成2')*/
                /*字符串需要用单引号包起来*/
                /*string sql = "insert into data (id,username,rank) values (" + a + ",'" + value1 + "','" +  value2 + "')";*/
                string sql = "insert into data (diff_plane) values ("  + caseData.Diff_plane + ")";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                
            }
            catch (Exception ex)
            {
                throw new Exception("插入数据失败：" + ex.Message);
            }
            conn.Close();
            ShowAllData(conn);
            
        }

        private void BtnSelect(object sender, RoutedEventArgs e)
        {
            
            SQLiteConnection conn = getConn();
            ShowAllData(conn);
        }

        // 刷新
        private void BtnRefresh(object sender, RoutedEventArgs e)
        {
            ImportCsv("E:\\c#study", "数据库表格.csv");
        }

        // 增加数据
        private void BtnAdd(object sender, RoutedEventArgs e)
        {
            AddData adddata = new AddData();
            adddata.TransfEvent += Add_TransfEvent;
            adddata.ShowDialog();
        }

        // 删除数据
        private void BtnDelete(object sender, RoutedEventArgs e)
        {
            string sql = "delete from data where 1 = 2";
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

        // 修改数据
        private void BtnUpdate(object sender, RoutedEventArgs e)
        {
            ImportCsv("E:\\c#study", "数据库表格.csv");
        }

        // 全选数据
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

       
        
        // 点击事件触发
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

        // 上一页
        private void BtnUp_Click(object sender, RoutedEventArgs e)
        {
            if(pageIndex > 1 )
            {
                pageIndex -= 1;
                           
            }
    
            SQLiteConnection conn = getConn();
            ShowAllData(conn);
        }

        // 下一页
        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (pageIndex < totalPage)
            {              
                pageIndex += 1;                               
            }
            SQLiteConnection conn = getConn();
            ShowAllData(conn);

        }

        // 跳转
        private void Skip(object sender, RoutedEventArgs e)
        {
            int inputNumber = Convert.ToInt32(InputNumber.Text);
            
            if(inputNumber > totalPage)
            {
                pageIndex = totalPage;
                
            }else if (inputNumber < 1)
            {
                pageIndex = 1;
                
            }
            else
            {
                pageIndex = inputNumber;               
            }
            SQLiteConnection conn = getConn();
            ShowAllData(conn);
        }

        // 向数据库中插入csv文件
        public void ImportCsv(string filePath, string fileName)
        {
            SQLiteConnection conn = getConn();
            conn.Open();
            string strConn = @"Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=";
            strConn += filePath;
            strConn += ";Extensions=asc,csv,tab,txt;";
            OdbcConnection objConn = new OdbcConnection(strConn);
            DataSet ds = new DataSet();
            try
            {
                string strSQL = "select * from " + fileName;//文件名，不要带目录
                OdbcDataAdapter da = new OdbcDataAdapter(strSQL, objConn);
                da.Fill(ds);

                //开始导入数据库
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string sql = "insert into data (phase,phase_ratio,temperature,diff_plane,ehkl,vhkl,distance) values (";
                    foreach (DataColumn column in ds.Tables[0].Columns)
                    {
                        sql = sql + row[column].ToString() + ",";                           
                    }
                    sql = sql + ");";
                    MessageBox.Show(sql);
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            conn.Close();
        }
    }
}
