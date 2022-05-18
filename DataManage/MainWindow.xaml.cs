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
using NPOI.SS.UserModel;

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
        public static int pageSize = 10;

        public static string dbpath = AppDomain.CurrentDomain.BaseDirectory + @"mydb.db";

        public static string connStr = @"Data Source=" + dbpath + @";Initial Catalog=sqlite;Version=3;";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
            ShowAllData();
            List<String> phases = selectPhaseALL();
            inputPhase.Items.Add("");
            foreach (String phase in phases){              
                inputPhase.Items.Add(phase);
            }
            
            inputPhase.SelectedIndex = 0;

        }

        // 分页查询数据
        private void ShowAllData()
        {
            List<caseData> list = new List<caseData>();
        
            int Count = selectData();
            
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
            
            /*if (inputPhase.Text.Trim() != "")
            {
                sql2 += " and phase like '" + inputPhase.Text.Trim() + "%' ";
            }*/
            string sql = sql1 + sql2 + sql3;

            /*MessageBox.Show(sql);*/
            using(SQLiteConnection conn = new SQLiteConnection(connStr))
            {                    
                using(SQLiteCommand command = new SQLiteCommand(sql, conn))
                {
                    try
                    {
                        conn.Open();
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
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
                            /*MessageBox.Show("进来了");*/
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("查询数据失败：" + ex.Message);
                    }
                    conn.Close();               
                }                                                                            
            }
            
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

        // 查询所有数据条数
        private int selectData()
        {
            List<caseData> listAll = new List<caseData>();
            string sql1 = "select count(*) from data ";
            string sql2 = "where 1 = 1 ";
            
            if(inputPhase.Text.Trim() != "")
            {
                sql2 += " and phase like '" + inputPhase.Text.Trim() + "%'";
            }
            
            string sqlAll = sql1 + sql2;
            int count = 0;
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                using (SQLiteCommand command = new SQLiteCommand(sqlAll, conn))
                {
                    try
                    {
                        conn.Open();
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                count = Convert.ToInt32(reader[0].ToString());
                            }
                        }
                            
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("查询失败", "");
                        throw new Exception("查询数据失败：" + ex.Message);
                    }
                    conn.Close();
                }
            }
            return count;
        }
        
        // 增加数据
        void Add_TransfEvent(String sql)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                {
                    try
                    {
                        conn.Open();
                        command.ExecuteNonQuery();
                        MessageBox.Show("插入成功");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("插入数据失败：" + ex.Message);
                    }
                    conn.Close();
                }
            }
            ShowAllData();
            
        }

        // 多个搜索字段查询
        private void BtnSelect(object sender, RoutedEventArgs e)
        {

            /*if (inputDiff_plane.SelectedValue.ToString() != "" )
            {
                
            }

            inputTemperature.Text.ToString();
            inputPhase.SelectedValue.ToString();
            inputDiff_plane.SelectedValue.ToString();*/
            ShowAllData();
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
                    sql = sql + " or data.Id =" + cb.Tag;                  
                }         
            }
            
            object lockThis = new object();
            
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                {
                    try
                    {
                         conn.Open();
                         command.ExecuteNonQuery();
                         MessageBox.Show("删除成功", "");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("删除数据：" + "失败：" + ex.Message);
                    }
                    conn.Close();
                }
            }            
            ShowAllData();            
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
            ShowAllData();
        }

        // 下一页
        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            
            if (pageIndex < totalPage)
            {              
                pageIndex += 1;                               
            }
            ShowAllData();
        }

        // 跳转
        private void Skip(object sender, RoutedEventArgs e)
        {
            if(InputNumber.Text != "")
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
            
                ShowAllData();
            }
            
        }

        // 向数据库中插入csv文件
        public void ImportCsv(string filePath, string fileName)
        {        
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                try
                {
                    conn.Open();
                    //开始导入数据库
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        string sql = "insert into data (phase,phase_ratio,temperature,diff_plane,ehkl,vhkl,distance) values ('";
                        foreach (DataColumn column in ds.Tables[0].Columns)
                        {
                             sql = sql + row[column].ToString() + "','";
                        }
                        sql = sql.Substring(0, sql.Length - 2);
                        sql = sql + ");";
                        using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                        {
                            command.ExecuteNonQuery();
                        }
                        MessageBox.Show("插入成功");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("插入失败");
                    throw ex;
                }
                conn.Close();
            }           
        }

        // 向数据库中插入excel文件
        public void ImportXls(string filePath)
        {           
            IWorkbook workbook = WorkbookFactory.Create(filePath);
            ISheet sheet = workbook.GetSheetAt(0);//获取第一个工作簿            
            
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                try
                {
                    conn.Open();
                    //开始导入数据库
                    for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                    {
                        IRow row = (IRow)sheet.GetRow(i);//获取第i行
                        string sql = "insert into data (phase,phase_ratio,temperature,diff_plane,ehkl,vhkl,distance) values ('";
                        for (int j = 0; j <= row.LastCellNum; j++)
                        {
                            if (sheet.GetRow(i).GetCell(j) != null)
                            {
                                sql = sql + sheet.GetRow(i).GetCell(j).ToString() + "','";
                            }
                            else
                            {
                                sql = sql + ""+ "','";
                            }                           
                        }
                        sql = sql.Substring(0, sql.Length - 2);
                        sql = sql + ");";
                        MessageBox.Show(sql);
                        using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                        {
                            command.ExecuteNonQuery();
                        }
                        MessageBox.Show("插入成功");
                    }                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show("插入失败");
                    throw ex;
                }
                conn.Close();
            }
        }

        private void BtnRefresh(object sender, RoutedEventArgs e)
        {
            inputPhase.Text = "";
            ShowAllData();

        }

        // 修改
        private void BtnUpdate(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < dg1.Items.Count; i++)
            {
                //获取行
                DataGridRow neddrow = (DataGridRow)dg1.ItemContainerGenerator.ContainerFromIndex(i);

                //获取该行的某列
                CheckBox cb = (CheckBox)dg1.Columns[0].GetCellContent(neddrow);
                if (cb.IsChecked == true)
                {
                    int updateId = Convert.ToInt32(cb.Tag);                    
                    caseData caseData = selectDataById(updateId);
                    UpdateData updateData = new UpdateData(caseData);
                    updateData.TransfEvent += Update_TransfEvent;
                    updateData.ShowDialog();
                    return ;
                }
            }
             

        }

        // 修改数据事件
        void Update_TransfEvent(caseData caseData)
        {

            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                string sql = "update data  set phase = '" + caseData.Phase + "' , phase_ratio = " + caseData.Phase_ratio + ", temperature = " +
                    caseData.Temperature + ", diff_plane = '" + caseData.Diff_plane + "', ehkl = '" + caseData.Ehkl + "', vhkl = " +
                    caseData.Vhkl + ", distance = " + caseData.Distance + " where 1 = 1 and id = " + caseData.Id;
         
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                {
                    try
                    {
                        conn.Open();
                        command.ExecuteNonQuery();
                        MessageBox.Show("修改成功");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("修改数据失败：" + ex.Message);
                    }
                    conn.Close();
                }
            }
            ShowAllData();

        }

        // 通过ID查询数据
        private caseData selectDataById(int id)
        {
            caseData casedata = new caseData();
            string sql = "select * from data  where id = " + id;
            
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {               
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                {
                    try
                    {
                        conn.Open();                       
                        SQLiteDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
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
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("查询失败123", "");
                        throw new Exception("查询数据失败：" + ex.Message);
                    }
                    conn.Close();
                    return casedata;
                }
            }
            
        }

        // 搜索所有的相字段
        private List<String> selectPhaseALL()
        {
            
            string sql = "select phase from data group by phase ";
            List<String> strs = new List<string>();
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                {
                    try
                    {                        
                        conn.Open();
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {                           
                            while (reader.Read())
                            {
                                strs.Add(reader["phase"].ToString());                              
                            }
                        }                                                                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("查询失败123", "");
                        throw new Exception("查询数据失败：" + ex.Message);
                    }
                    conn.Close();
                    return strs;
                }
            }
        }


        // 搜索所有的衍射面
        private List<string>  selectDiff_planeALL(string phase)
        {

            string sql = "select diff_plane from data where phase = '" + phase + "' group by diff_plane";
            MessageBox.Show(sql);
            List<String> strs = new List<string>();
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                {
                    try
                    {
                        conn.Open();
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {              
                            while (reader.Read())
                            {
                                
                                strs.Add(reader["diff_plane"].ToString());
                            }
                        }                           
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("查询失败123", "");
                        throw new Exception("查询数据失败：" + ex.Message);
                    }
                    conn.Close();
                    return strs;

                }
            }
        }

        private void inputPhase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            MessageBox.Show(inputPhase.SelectedValue.ToString());
            /*if(inputPhase.SelectedValue.ToString() == "")
            {
                inputPhase.Items.Clear();
            }
            else
            {
                List<String> phases = selectDiff_planeALL(inputPhase.SelectedValue.ToString());           
                foreach (String phase in phases)
                {
                    inputDiff_plane.Items.Add(phase);
                }
            }*/
            List<String> phases = selectDiff_planeALL(inputPhase.SelectedValue.ToString());
            foreach (String phase in phases)
            {
                inputDiff_plane.Items.Add(phase);
            }
        }
    }
}
