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
using NPOI.XSSF.UserModel;
using System.IO;
/*using System.Windows.Forms;*/

namespace DataManage
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

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

        // 显示所有数据
        private void ShowAllData()
        {
            List<caseData> list = new List<caseData>();
        
            int Count = selectData();
            
            
            /*string sql = "SELECT * FROM casedata " + "limit " + index + "," + pageSize;*/
            
            string sql1 = "SELECT * FROM data " ;
            string sql2 = "where 1 = 1 ";
            
            
            /*if (inputPhase.Text.Trim() != "")
            {
                sql2 += " and phase like '" + inputPhase.Text.Trim() + "%' ";
            }*/
            string sql = sql1 + sql2 ;

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
            
                
        }

        // 查询所有数据条数
        private int selectData()
        {
            List<caseData> listAll = new List<caseData>();
            string sql1 = "select count(*) from data ";
            string sql2 = "where 1 = 1 ";
            
            //if(inputPhase.Text.Trim() != "")
            //{
            //    sql2 += " and phase like '" + inputPhase.Text.Trim() + "%'";
            //}
            
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
            string sql = "SELECT * FROM data where 1 = 1";
            
            if (inputPhase.SelectedValue.ToString()!="")
            {
                sql = sql + " and phase='"+ inputPhase.SelectedValue.ToString()+"'";
            }
            if (inputTemperature.Text.ToString() != "")
            {
                sql=sql+" and temperature='" + inputTemperature.Text.ToString()+ "'";
            }
            if (inputPhase_ratio.Text.ToString() != "")
            {
                sql = sql + " and phase_ratio='" + inputPhase_ratio.Text.ToString() + "'";
            }
            if (inputDiff_plane.SelectedValue!= null)
            {
                sql = sql + " and diff_plane='" + inputDiff_plane.SelectedValue.ToString() + "'";
            }
            //MessageBox.Show(sql);

            List<caseData> list = new List<caseData>();
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
        }

       
         // 增加数据
         private void BtnAdd(object sender, RoutedEventArgs e)
        {
            List<String> phases = selectPhaseALL();
            AddData adddata = new AddData(phases);
            adddata.TransfEvent += Add_TransfEvent;
            adddata.ShowDialog();
         }

        // 删除数据
        private void BtnDelete(object sender, RoutedEventArgs e)
        {
            bool flag = false;
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
                    flag = true;
                }         
            }

            if (flag)
            {
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
            }
            else
            {
                MessageBox.Show("请选择需要操作的数据", "提示信息");
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

                if (neddrow != null)
                {
                    //获取该行的某列
                    CheckBox cb = (CheckBox)dg1.Columns[0].GetCellContent(neddrow);

                    cb.IsChecked = headercb.IsChecked;
                }                               
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
        public int ImportXls(string filePath)
        {
            try
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
                            for (int j = 0; j <= 6; j++)
                            {
                                if (sheet.GetRow(i).GetCell(j) != null)
                                {
                                    sql = sql + sheet.GetRow(i).GetCell(j).ToString() + "','";
                                }
                                else
                                {
                                    sql = sql + "" + "','";
                                }
                            }
                            sql = sql.Substring(0, sql.Length - 2);
                            sql = sql + ");";

                            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                            {
                                command.ExecuteNonQuery();
                            }
                            /*MessageBox.Show("插入成功");*/
                        }
                        MessageBox.Show("上传成功");

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("上传失败");
                        throw ex;
                    }
                    conn.Close();

                }
                workbook.Close();
            }
            catch (IOException)
            {
                MessageBox.Show("请先该关闭文件！","错误提示");
                return -1;
            }                                 
            ShowAllData();
            return 0;
        }

        //导出excel文件
        public void exportXls(List<caseData> list,string filePath)
        {
            try
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("sheet1");
                FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
                
                //写入excel文件         
                IRow row = (IRow)sheet.CreateRow(0);//获取第一行                        
                row.CreateCell(0).SetCellValue("相");
                row.CreateCell(1).SetCellValue("相比例（%）");
                row.CreateCell(2).SetCellValue("温度（℃）");
                row.CreateCell(3).SetCellValue("衍射面");
                row.CreateCell(4).SetCellValue("衍射弹性常数Ehkl（GPa）");
                row.CreateCell(5).SetCellValue("衍射弹性常数Vhkl");
                row.CreateCell(6).SetCellValue("晶面间距d");

                int i = 1;
                foreach (caseData casedata in list)
                {
                    row = (IRow)sheet.CreateRow(i);//获取第i行
                    row.CreateCell(0).SetCellValue(casedata.Phase);
                    row.CreateCell(1).SetCellValue(casedata.Phase_ratio);
                    row.CreateCell(2).SetCellValue(casedata.Temperature);
                    row.CreateCell(3).SetCellValue(casedata.Diff_plane);
                    row.CreateCell(4).SetCellValue(casedata.Ehkl);
                    row.CreateCell(5).SetCellValue(casedata.Vhkl);
                    row.CreateCell(6).SetCellValue(casedata.Distance);
                    i++;
                }
                //导出excel               
                workbook.Write(fs);                                
                workbook.Close();
                fs.Close();
                MessageBox.Show("导出成功");
            }
            catch (IOException)
            {
                MessageBox.Show("请先该关闭文件！", "错误提示");                
            }
        }

        private void BtnRefresh(object sender, RoutedEventArgs e)
        {
            inputTemperature.Text = "";
            inputPhase_ratio.Text = "";
            inputDiff_plane.Items.Clear();
            ShowAllData();
            List<String> phases = selectPhaseALL();
            inputPhase.Items.Clear();
            inputPhase.Items.Add("");
            foreach (String phase in phases)
            {
                inputPhase.Items.Add(phase);
            }
            inputPhase.SelectedIndex = 0;
        }

        // 修改
        private void BtnUpdate(object sender, RoutedEventArgs e)
        {
            bool flag = false;
            for (int i = 0; i < dg1.Items.Count; i++)
            {
                //获取行
                DataGridRow neddrow = (DataGridRow)dg1.ItemContainerGenerator.ContainerFromIndex(i);
                
                //获取该行的某列
                CheckBox cb = (CheckBox)dg1.Columns[0].GetCellContent(neddrow);
                if (cb.IsChecked == true)
                {
                    int updateId = Convert.ToInt32(cb.Tag);
                    List<String> phases = selectPhaseALL();
                    caseData caseData = selectDataById(updateId);
                    UpdateData updateData = new UpdateData(caseData,phases);
                    updateData.TransfEvent += Update_TransfEvent;
                    updateData.ShowDialog();
                    flag= true;
                    return;
                }
            }
            if (flag)
            {

            }
            else
            {
                MessageBox.Show("请选择需要操作的数据", "提示信息");
            }

        }

        // 修改数据事件
        void Update_TransfEvent(caseData caseData)
        {

            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sql = "update data  set phase = '" + caseData.Phase + "' , phase_ratio = " + caseData.Phase_ratio + ", temperature = " +
                    caseData.Temperature + ", diff_plane = '" + caseData.Diff_plane + "', ehkl = '" + caseData.Ehkl + "', vhkl = " +
                    caseData.Vhkl + ", distance = " + caseData.Distance + " where 1 = 1 and id = " + caseData.Id;

                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    {
                        command.ExecuteNonQuery();
                        MessageBox.Show("修改成功");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("修改数据失败：" + ex.Message);
                }
                conn.Close();               
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
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
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
            //MessageBox.Show(sql);
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
            inputDiff_plane.Items.Clear();
            if (inputPhase.SelectedValue != null)
            {
                List<String> phases = selectDiff_planeALL(inputPhase.SelectedValue.ToString());
                foreach (String phase in phases)
                {
                    inputDiff_plane.Items.Add(phase);
                }
            }
            
        }

        private void UploadFile(object sender, RoutedEventArgs e)
        {
            
            
            /*ImportXls("C:\\Users\\艺涛\\Desktop\\数据库表格(1).xlsx");*/
            Microsoft.Win32.OpenFileDialog fileDialog1 = new Microsoft.Win32.OpenFileDialog();
            fileDialog1.InitialDirectory = "c:\\";//初始目录
            fileDialog1.Filter = "Execl files (*.xlsx)|*.xlsx";//文件的类型
            fileDialog1.FilterIndex = 1;
            fileDialog1.RestoreDirectory = true;
            
            if (fileDialog1.ShowDialog() == true)
            {
                FilePath.Text = fileDialog1.FileName;
                string str1 = fileDialog1.FileName;
                /*MessageBox.Show(str1);*/
                MessageBox.Show("正在上传中");
                ImportXls(str1);                                     
            }
            else
            {
                FilePath.Text = "";
            }
        }

        private void BtnExport(object sender, RoutedEventArgs e)
        {
            List<caseData> list = new List<caseData>();
            bool flag=false;           
            Microsoft.Win32.SaveFileDialog fileDialog1 = new Microsoft.Win32.SaveFileDialog();
            fileDialog1.InitialDirectory = "c:\\";//初始目录
            fileDialog1.Filter = "Execl files (*.xlsx)|*.xlsx";//文件的类型
            fileDialog1.FilterIndex = 1;
            for (int i = 0; i < dg1.Items.Count; i++)
            {
                //获取行
                DataGridRow neddrow = (DataGridRow)dg1.ItemContainerGenerator.ContainerFromIndex(i);

                //获取该行的某列
                CheckBox cb = (CheckBox)dg1.Columns[0].GetCellContent(neddrow);
                if (cb.IsChecked == true)
                {                    
                    int Id = Convert.ToInt32(cb.Tag);                    
                    caseData casedata = selectDataById(Id);
                    list.Add(casedata);
                    flag = true;
                }
            }
            if (flag)
            {
                if (fileDialog1.ShowDialog() == true)
                {
                    String filename = fileDialog1.FileName;
                    exportXls(list, filename);
                }
                else
                {

                }              
            }
            else
            {
                MessageBox.Show("请选择需要操作的数据","提示信息");
            }
            
        }
    }
}
