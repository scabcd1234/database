﻿using System;
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
using System.Threading;
using System.Net.Sockets;
using System.Windows.Threading;

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

        public static int index = 0;

        public static int pageSize = 20;

        public static int flaseIdFlag = 1;

        public static bool selectedFlag = false; // 查询状态

        public static bool multiselectedFlag = false; // 多晶体查询状态

        public static bool singleselectedFlag = false; // 单晶体查询状态

        public static List<caseData> CurrentList = new List<caseData>(); //当前数据集合

        public static List<caseData> MultiCurrentList = new List<caseData>(); //多晶体当前数据集合

        public static List<SingleData> SingleCurrentList = new List<SingleData>(); //单晶体当前数据集合

        public static CheckBox cball;
        public MainWindow()
        {
            InitializeComponent();
        }
      
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            image.Source = new BitmapImage(new Uri(@"image\1.png", UriKind.Relative));
            /*image1.Source = new BitmapImage(new Uri(@"image\3.ico", UriKind.Relative));*/

            List<String> phases = selectPhaseALL();
            inputPhase.Items.Add("");
            foreach (String phase in phases){              
                inputPhase.Items.Add(phase);
            } 
            inputPhase.SelectedIndex = 0;

            ResetChecked();
            multiResetChecked();
            singleResetChecked();
            ShowAllData();   
        }
        
        // 显示原始数据
        private void ShowAllData()
        {
            index = 0;            
            List<caseData> list = new List<caseData>();
        
            // int Count = selectData();
            
            
            /*string sql = "SELECT * FROM casedata " + "limit " + index + "," + pageSize;*/
            
            string sql1 = "SELECT * FROM data " ;
            string sql2 = "";
            //string sql3 = " limit " + pageSize + " offset "+ index;
            string sql3 = "";

            /*if (inputPhase.Text.Trim() != "")
            {
                sql2 += " and phase like '" + inputPhase.Text.Trim() + "%' ";
            }*/
            string sql = sql1 + sql2 + sql3;

            //MessageBox.Show(sql);
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {                    
                using(SQLiteCommand command = new SQLiteCommand(sql, conn))
                {
                    try
                    {
                        conn.Open();
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            int i = 1;
                            while (reader.Read())
                            {
                                caseData casedata = new caseData();
                                casedata.Id = Convert.ToInt32(reader["id"]);
                                casedata.FlaseId = i;
                                casedata.Phase = reader["phase"].ToString();
                                casedata.Phase_ratio = Convert.ToDouble(reader["phase_ratio"]);
                                casedata.Temperature = Convert.ToDouble(reader["temperature"]);
                                casedata.Diff_plane = reader["diff_plane"].ToString();
                                casedata.Ehkl = Convert.ToDouble(reader["ehkl"]);
                                casedata.Vhkl = Convert.ToDouble(reader["vhkl"]);
                                casedata.IsChecked = (Boolean)reader["ischecked"];
                                if (reader["distance"].ToString() == "")
                                {
                                    casedata.Distance = 0.00;
                                }
                                else
                                {
                                    casedata.Distance = Convert.ToDouble(reader["distance"]);
                                }
                                list.Add(casedata);
                                i++;

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

            //ScrollViewer sv1 = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(this.dg1, 0), 0) as ScrollViewer;
            //sv1.ScrollChanged -= DataGrid_ScrollChanged;
            //sv1.ScrollChanged += DataGrid_ScrollChanged;
            CurrentList = list;
            //dg1.Items.Clear();
            //foreach (caseData item in list)
            //{
            //    dg1.Items.Add(item);
            //}
            dg1.ItemsSource = null;
            dg1.ItemsSource = list;
            //if(dg1.Items.Count > 0)
            //{
            //    dg1.ScrollIntoView(dg1.Items[0]);
            //}

            // 显示记录条数
            SetNumber();
        }

        // 设置条数
        private void SetNumber()
        {
            string sql = "select count(*) from data where 1 = 1";
            string sql1 = sql+" and phase='α'";
            string sql2 = sql + " and phase='β'";
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {               
                    try
                    {
                        conn.Open();
                        using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                        {
                            ALLNumber.Content = Convert.ToInt32(command.ExecuteScalar());                            
                        }
                        using (SQLiteCommand command = new SQLiteCommand(sql1, conn))
                        {
                            FirstNumber.Content = Convert.ToInt32(command.ExecuteScalar());                            
                        }
                        using (SQLiteCommand command = new SQLiteCommand(sql2, conn))
                        {
                            SecondNumber.Content= Convert.ToInt32(command.ExecuteScalar());
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
                        MessageBox.Show("插入成功", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    catch (Exception ex)
                    {
                        throw new Exception("插入数据失败：" + ex.Message);
                    }
                    conn.Close();
                }
            }
            flaseIdFlag = 1;
            ShowAllData();
            //cball.IsChecked = false;
        }

        // 多个搜索字段查询
        private void BtnSelect(object sender, RoutedEventArgs e)
        {
            bool HasData = false;
            string sql = "SELECT * FROM data where 1 = 1";
            if (inputPhase.SelectedValue.ToString() != "" && inputDiff_plane.SelectedValue == null)
            {
                MessageBox.Show("请选择衍射面！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if(inputPhase.SelectedValue.ToString() == "" && inputDiff_plane.SelectedValue != null)
            {
                MessageBox.Show("请选择相！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Warning);
            }else if (inputPhase.SelectedValue.ToString() == "" && inputDiff_plane.SelectedValue == null)
            {
                MessageBox.Show("请选择相和衍射面！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (isNotDouble(inputPhase_ratio.Text.ToString()) || isNotDouble(inputTemperature.Text.ToString()))
                {
                    MessageBox.Show("相比例或温度请输入double类型！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    if (inputPhase.SelectedValue.ToString() != "")
                    {
                        sql = sql + " and phase='" + inputPhase.SelectedValue.ToString() + "'";
                    }
                    if (inputTemperature.Text.ToString() != "")
                    {
                        sql = sql + " and temperature='" + inputTemperature.Text.ToString() + "'";
                    }
                    if (inputPhase_ratio.Text.ToString() != "")
                    {
                        sql = sql + " and phase_ratio='" + inputPhase_ratio.Text.ToString() + "'";
                    }
                    if (inputDiff_plane.SelectedValue != null)
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
                                    int i = 1;
                                    while (reader.Read())
                                    {
                                        HasData = true;
                                        caseData casedata = new caseData();
                                        casedata.Id = Convert.ToInt32(reader["id"]);
                                        casedata.FlaseId = i;
                                        casedata.Phase = reader["phase"].ToString();
                                        casedata.Phase_ratio = Convert.ToDouble(reader["phase_ratio"]);
                                        casedata.Temperature = Convert.ToDouble(reader["temperature"]);
                                        casedata.Diff_plane = reader["diff_plane"].ToString();
                                        casedata.Ehkl = Convert.ToDouble(reader["ehkl"]);
                                        casedata.Vhkl = Convert.ToDouble(reader["vhkl"]);
                                        casedata.IsChecked = (Boolean)reader["ischecked"];
                                        if (reader["distance"].ToString() == "")
                                        {
                                            casedata.Distance = 0.00;
                                        }
                                        else
                                        {
                                            casedata.Distance = Convert.ToDouble(reader["distance"]);
                                        }
                                        list.Add(casedata);
                                        i++;
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
                    CurrentList = list;
                    if (!HasData) // 未找到数据
                    {
                        if (MessageBox.Show("未找到查询数据，是否自动生成数据？", "提示信息", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            String phase = inputPhase.SelectedValue.ToString();
                            String diff_plane = inputDiff_plane.SelectedValue.ToString();
                            if (phase != "" && diff_plane != "" && inputPhase_ratio.Text != "" && inputTemperature.Text != "")
                            {
                                double before_phase_ratio = Convert.ToDouble(inputPhase_ratio.Text); // 近似前相位比
                                double before_temperature = Convert.ToDouble(inputTemperature.Text); //近似前温度

                                double after_phase_ratio1 = before_phase_ratio; // 近似后计算用相位比
                                double after_phase_ratio2 = Find_PhaseRatio(phase, diff_plane, before_phase_ratio); // 近似后查找用相位比                
                                double after_temperature = Find_Temperature(phase, diff_plane, after_phase_ratio2, before_temperature); // 近似后查找用温度
                                if (phase == "α")
                                {
                                    double[] ratio_list = { 62, 55.5, 70, 80 };
                                    //选择最接近的相位比
                                    after_phase_ratio1 = ratio_list[0];
                                    foreach (double tmp in ratio_list)
                                    {
                                        if (Math.Abs(before_phase_ratio - tmp) <= Math.Abs(before_phase_ratio - after_phase_ratio1))
                                        {
                                            after_phase_ratio1 = tmp;
                                        }
                                    }
                                }
                                else if (phase == "β")
                                {
                                    double[] ratio_list = { 38, 44.5, 30, 20 };
                                    //选择最接近的相位比
                                    after_phase_ratio1 = ratio_list[0];
                                    foreach (double tmp in ratio_list)
                                    {
                                        if (Math.Abs(before_phase_ratio - tmp) <= Math.Abs(before_phase_ratio - after_phase_ratio1))
                                        {
                                            after_phase_ratio1 = tmp;
                                        }
                                    }
                                }
                                double Ehkl = computeEhkl(phase, after_phase_ratio1, before_temperature, diff_plane);
                                caseData tmp_data = selectVhklAndDistance(phase, diff_plane, after_phase_ratio2, after_temperature);
                                caseData casedata = new caseData();
                                casedata.FlaseId = 1;
                                casedata.Phase = phase;
                                casedata.Phase_ratio = before_phase_ratio;
                                casedata.Temperature = before_temperature;
                                casedata.Diff_plane = diff_plane;
                                casedata.Ehkl = Ehkl;
                                casedata.Vhkl = tmp_data.Vhkl;
                                casedata.Distance = tmp_data.Distance;
                                list.Add(casedata);
                                updateButton.IsEnabled = false;
                                deleteButton.IsEnabled = false;
                                exportButton.IsEnabled = false;

                            }
                            else
                            {
                                MessageBox.Show("生成数据不能填入空值!请重新填写", "提示信息", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }

                        }
                    }
                    //ScrollViewer sv1 = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(this.dg1, 0), 0) as ScrollViewer;
                    //sv1.ScrollChanged -= DataGrid_ScrollChanged;
                    //dg1.Items.Clear();
                    //foreach (caseData item in list)
                    //{
                    //    dg1.Items.Add(item);
                    //}
                    selectedFlag = true;
                    dg1.ItemsSource = null;
                    dg1.ItemsSource = list;
                }
                
            }            
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
            
            bool flag = false;
            string sql = "delete from data where data.Id in (";                                   
            //for (int i = 0; i < dg1.Items.Count; i++)
            //{
            //    //获取行
            //    DataGridRow neddrow = (DataGridRow)dg1.ItemContainerGenerator.ContainerFromIndex(i);

            //    //获取该行的某列
            //    CheckBox cb = (CheckBox)dg1.Columns[0].GetCellContent(neddrow);
            //    if (cb.IsChecked == true)
            //    {
            //        sql = sql + " or data.Id =" + cb.Tag;
            //        flag = true;
            //    }
            //}
            foreach (caseData item in CurrentList)
            {
                String presql = "select ischecked from data where id='" + item.Id + "';";
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    using (SQLiteCommand command = new SQLiteCommand(presql, conn))
                    {                        
                        try
                        {
                            conn.Open();
                            bool ischecked = (bool)command.ExecuteScalar();
                            if (ischecked == true)
                            {
                                sql = sql + item.Id + ",";
                                flag = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("获取选中数据：" + "失败：" + ex.Message);
                        }
                        conn.Close();
                    }
                }
            }
            sql = sql.Substring(0, sql.Length - 1) + ");";
            //MessageBox.Show(sql);
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
                            MessageBox.Show("删除成功", "提示信息", MessageBoxButton.OK,MessageBoxImage.Information);
                            ResetChecked();
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
                MessageBox.Show("请选择需要操作的数据", "提示信息",MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
            flaseIdFlag = 1;

            cball.IsChecked = false;
            

            if (selectedFlag == true)
            {
                BtnSelect(null, null);
                
            }
            else
            {
                ShowAllData();
            }                                    
        }


        // 全选数据
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox headercb = (CheckBox)sender;          
            string sql = "update data set ischecked = " + headercb.IsChecked + " where data.Id in ( ";
            foreach (caseData item in CurrentList)
            {
                sql = sql +item.Id+",";
            }
            sql = sql.Substring(0, sql.Length - 1) + ");";
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                try
                {
                    conn.Open();                  
                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("全选数据：" + "失败：" + ex.Message);
                }
                conn.Close();
            }
            if (selectedFlag == true)
            {
                BtnSelect(null, null);
            }
            else
            {
                ShowAllData();
            }
        }

        //// 点击事件触发
        //private void Item_GotFocus(object sender, RoutedEventArgs e)
        //{

        //    //获取行
        //    DataGridRow neddrow = (DataGridRow)dg1.ItemContainerGenerator.ContainerFromIndex(dg1.SelectedIndex);

        //    if (neddrow != null)
        //    {
        //        //获取该行的某列
        //        CheckBox cb = (CheckBox)dg1.Columns[0].GetCellContent(neddrow);

        //        cb.IsChecked = !cb.IsChecked;
        //    }
        //}


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
                        string sql = "replace into data (phase,phase_ratio,temperature,diff_plane,ehkl,vhkl,distance) values ('";
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
                        MessageBox.Show("插入成功", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {                    
                    MessageBox.Show("插入失败", "提示信息", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        /*NotificationWindow data = new NotificationWindow("正在上传中!");
                        data.Show();*/
                        
                        conn.Open();
                        SQLiteTransaction tx = conn.BeginTransaction();
                        //开始导入数据库
                        for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                        {
                            IRow row = (IRow)sheet.GetRow(i);//获取第i行
                            string sql = "replace into data (phase,phase_ratio,temperature,diff_plane,ehkl,vhkl,distance) values ('";                            

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
                                command.Transaction = tx;
                                command.CommandText=sql;
                                command.ExecuteNonQuery();
                            }
                            /*MessageBox.Show("插入成功");*/
                        }
                        tx.Commit();
                        /*data.Close();*/
                        MessageBox.Show("上传成功", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("上传失败", "提示信息", MessageBoxButton.OK, MessageBoxImage.Error);
                        throw ex;
                    }
                    conn.Close();

                }
                workbook.Close();
            }
            catch (IOException)
            {
                MessageBox.Show("请先关闭该文件！","错误提示");
                return -1;
            }
            flaseIdFlag = 1;
            ShowAllData();
            cball.IsChecked = false;
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
                MessageBox.Show("导出成功", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (IOException)
            {
                MessageBox.Show("请先关闭该文件！", "错误提示", MessageBoxButton.OK, MessageBoxImage.Warning);                
            }
        }

        // 刷新数据
        private void BtnRefresh(object sender, RoutedEventArgs e)
        {
            inputTemperature.Text = "";
            inputPhase_ratio.Text = "";
            inputDiff_plane.Items.Clear();

            updateButton.IsEnabled = true;
            deleteButton.IsEnabled = true;
            exportButton.IsEnabled = true;

            List<String> phases = selectPhaseALL();
            inputPhase.Items.Clear();
            inputPhase.Items.Add("");
            foreach (String phase in phases)
            {
                inputPhase.Items.Add(phase);
            }
            inputPhase.SelectedIndex = 0;

           
            flaseIdFlag = 1;
            selectedFlag = false;
            ResetChecked();
            ShowAllData();
            cball.IsChecked = false;

        }

        // 修改数据
        private void BtnUpdate(object sender, RoutedEventArgs e)
        {

            bool flag = false;
            int i = 0;
            
            foreach (caseData item in CurrentList)
            {
                String presql = "select ischecked from data where id='" + item.Id + "';";
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    using (SQLiteCommand command = new SQLiteCommand(presql, conn))
                    {
                        try
                        {
                            conn.Open();
                            bool ischecked = (bool)command.ExecuteScalar();
                            if (ischecked == true)
                            {
                                caseData caseData = selectDataById(item.Id);
                                UpdateData updateData = new UpdateData(caseData);
                                updateData.TransfEvent += Update_TransfEvent;
                                updateData.ShowDialog();
                                flag = true;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("获取选中数据：" + "失败：" + ex.Message);
                        }
                        conn.Close();
                    }
                }
            }
            if (flag)
            {
                ResetChecked();
                if (selectedFlag == true)
                {
                    BtnSelect(null, null);

                }
                else
                {
                    ShowAllData();
                }
            }
            else
            {
                MessageBox.Show("请选择需要操作的数据", "提示信息", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        // 修改数据事件
        int Update_TransfEvent(caseData caseData)
        {
            int result = 0;
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
                        MessageBox.Show("修改成功", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);                        
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("表中已存在该实验条件，请重新输入！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Warning);
                    result = -1;
                    //throw new Exception("修改数据失败：" + ex.Message);
                }
                conn.Close();               
            }
            flaseIdFlag = 1;           

            return result;
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
                                casedata.Phase_ratio = Convert.ToDouble(reader["phase_ratio"]);
                                casedata.Temperature = Convert.ToDouble(reader["temperature"]);
                                casedata.Diff_plane = reader["diff_plane"].ToString();
                                casedata.Ehkl = Convert.ToDouble(reader["ehkl"]);
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
            fileDialog1.FileName = "导出数据";
            
            foreach (caseData item in CurrentList)
            {
                String presql = "select ischecked from data where id='" + item.Id + "';";
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    using (SQLiteCommand command = new SQLiteCommand(presql, conn))
                    {
                        try
                        {
                            conn.Open();
                            bool ischecked = (bool)command.ExecuteScalar();
                            if (ischecked == true)
                            {                               
                                caseData casedata = selectDataById(item.Id);
                                list.Add(casedata);
                                flag = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("获取选中数据：" + "失败：" + ex.Message);
                        }
                        conn.Close();
                    }
                }
            }            
            if (flag)
            {               
                if (fileDialog1.ShowDialog() == true)
                {
                    String filename = fileDialog1.FileName;
                    exportXls(list, filename);
                    ResetChecked();
                }
                else
                {

                }              
            }
            else
            {
                MessageBox.Show("请选择需要操作的数据", "提示信息", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
        }

        // 计算弹性常数
        private double computeEhkl(String phase,double phase_ratio,double temperature,string diff_plane)
        {
            double Ehkl = 0;
            if (phase == "α")
            {
                switch (phase_ratio)
                {
                    case 55.5:
                        switch (diff_plane)
                        {
                            case "101":
                                Ehkl = 112.84229 - 0.03648 * temperature;
                                break;
                            case "100":
                                Ehkl = 119.20884 - 0.05004 * temperature;
                                break;
                            case "103":
                                Ehkl = 110.87868 - 0.0396 * temperature;
                                break;
                            case "002":
                                Ehkl = 115.84749 - 0.05121 * temperature;
                                break;
                            case "011":
                                Ehkl = 112.84229 - 0.03648 * temperature;
                                break;
                            default:
                                break;
                        }
                        break;
                    case 62:
                        switch (diff_plane)
                        {
                            case "101":
                                Ehkl = 113.87868 - 0.0396 * temperature;
                                break;
                            case "100":
                                Ehkl = 120.44194 - 0.05217 * temperature;
                                break;
                            case "103":
                                Ehkl = 111.5052 - 0.03973 * temperature;
                                break;
                            case "002":
                                Ehkl = 117.10485 - 0.05542 * temperature;
                                break;
                            case "011":
                                Ehkl = 113.87868 - 0.0396 * temperature;
                                break;
                            default:
                                break;
                        }
                        break;
                    case 70:
                        switch (diff_plane)
                        {
                            case "101":
                                Ehkl = 114.87868 - 0.0396 * temperature;
                                break;
                            case "100":
                                Ehkl = 121.44194 - 0.05217 * temperature;
                                break;
                            case "103":
                                Ehkl = 112.5052 - 0.03973 * temperature;
                                break;
                            case "002":
                                Ehkl = 118.12912 - 0.0575 * temperature;
                                break;
                            case "011":
                                Ehkl = 114.87868 - 0.0396 * temperature;
                                break;
                            default:
                                break;
                        }
                        break ;
                    case 80:
                        switch (diff_plane)
                        {
                            case "101":
                                Ehkl = 115.87868 - 0.0396 * temperature;
                                break;
                            case "100":
                                Ehkl = 122.44194 - 0.05217 * temperature;
                                break;
                            case "103":
                                Ehkl = 113.93501 - 0.04086 * temperature;
                                break;
                            case "002":
                                Ehkl = 119.11698 - 0.05646 * temperature;
                                break;
                            case "011":
                                Ehkl = 115.87868 - 0.0396 * temperature;
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                
            }else if(phase == "β")
            {
                switch (phase_ratio)
                {
                    case 38:
                        switch (diff_plane)
                        {
                            case "100":
                                Ehkl = 88.22357 - 0.03345 * temperature;
                                break;
                            case "101":
                                Ehkl = 105.89515 - 0.04458 * temperature;
                                break;
                            case "102":
                                Ehkl = 98.448877 - 0.03847 * temperature;
                                break;
                            case "211":
                                Ehkl = 105.89515 - 0.04458 * temperature;
                                break;
                            case "110":
                                Ehkl = 105.89515 - 0.04458 * temperature;
                                break;
                            default:
                                break;
                        }
                        break;
                    case 44.5:
                        switch (diff_plane)
                        {
                            case "100":
                                Ehkl = 86.75737 - 0.0292 * temperature;
                                break;
                            case "101":
                                Ehkl = 103.75737 - 0.0292 * temperature;
                                break;
                            case "102":
                                Ehkl = 96.75737 - 0.0292 * temperature;
                                break;
                            case "211":
                                Ehkl = 103.75737 - 0.0292 * temperature;
                                break;
                            case "110":
                                Ehkl = 103.75737 - 0.0292 * temperature;
                                break;
                            default:
                                break;
                        }
                        break;
                    case 30:
                        switch (diff_plane)
                        {
                            case "100":
                                Ehkl = 88.37608 - 0.03324 * temperature;
                                break;
                            case "101":
                                Ehkl = 106.92374 - 0.05061 * temperature;
                                break;
                            case "102":
                                Ehkl = 100.54593 - 0.04679 * temperature;
                                break;
                            case "211":
                                Ehkl = 106.92374 - 0.05061 * temperature;
                                break;
                            case "110":
                                Ehkl = 106.92374 - 0.05061 * temperature;
                                break;
                            default:
                                break;
                        }
                        break;
                    case 20:
                        switch (diff_plane)
                        {
                            case "100":
                                Ehkl = 90.44887 - 0.03847 * temperature;
                                break;
                            case "101":
                                Ehkl = 109.00087 - 0.06079 * temperature;
                                break;
                            case "102":
                                Ehkl = 101.15251 - 0.04879 * temperature;
                                break;
                            case "211":
                                Ehkl = 109.00087 - 0.06079 * temperature;
                                break;
                            case "110":
                                Ehkl = 109.00087 - 0.06079 * temperature;
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }

            }
            return Ehkl;
        }

        // 根据相和衍射面查找所有的相比例
        private List<double> selectPhase_ratio(String phase,string diff_plane)
        { 
            string sql = "select phase_ratio from data where phase = '" + phase + "' and diff_plane = '" + diff_plane + "' group by phase_ratio";
            /*select phase_ratio from data where phase = 'α' and diff_plane = '100' group by phase_ratio*/
            
            List<double> strs = new List<double>();
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
                                strs.Add(Convert.ToDouble(reader["phase_ratio"]));
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

        //寻找最近似的相位比
        private double Find_PhaseRatio(string phase, string diff_plane,double before_phase_ratio)
        {            
            List<double> ratio_list = selectPhase_ratio(phase, diff_plane);
            //选择最接近的相位比
            double after_phase_ratio = ratio_list[0];
            foreach (double tmp in ratio_list)
            {
                if (Math.Abs(before_phase_ratio - tmp) <= Math.Abs(before_phase_ratio - after_phase_ratio))
                {
                    after_phase_ratio = tmp;
                }
            }
            return after_phase_ratio;
        }

        // 根据相、衍射面和相比例查找所有的温度
        private List<double> selectTemperature(String phase,string diff_plane,double phase_ratio)
        {
            
            string sql = "select temperature from data where phase = '" + phase + "' and diff_plane = '" + diff_plane + "' and phase_ratio = '" + phase_ratio + "' group by temperature" ;
            /*select phase_ratio from data where phase = 'α' and diff_plane = '100' group by phase_ratio*/
           
            List<double> strs = new List<double>();
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
                                strs.Add(Convert.ToDouble(reader["temperature"]));
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

        //寻找最近似的温度
        private double Find_Temperature(string phase,string diff_plane,double after_phase_ratio,double before_temperature)
        {
            List<double> temperature_list = selectTemperature(phase, diff_plane, after_phase_ratio);
            //选择最接近的温度
            double after_temperature = temperature_list[0];
            foreach (double tmp in temperature_list)
            {
                if (Math.Abs(before_temperature - tmp) <= Math.Abs(before_temperature - after_temperature))
                {
                    after_temperature = tmp;
                }
            }
            return after_temperature;
        }

        // 根据相、衍射面、相比例和温度查找确定的衍射弹性常数vhkl、晶面间距d
        private caseData selectVhklAndDistance(String phase, string diff_plane, double phase_ratio, double temperature)
        {
            caseData casedata = new caseData();

            string sql = "select * from data where phase = '" + phase + "' and diff_plane = '" + diff_plane + "' and phase_ratio = '" +
                phase_ratio + "' and temperature = '" + temperature + "'";
            /*select phase_ratio from data where phase = 'α' and diff_plane = '100' group by phase_ratio*/


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

        //判断输入是否为double,不是则返回true
        public static bool isNotDouble(string str)
        {
            bool flag = false;
            if (str.StartsWith(".") || str.EndsWith("."))
            {
                flag = true;
            }
            else
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if (!(char.IsDigit(str, i) || str[i].Equals('.')))
                    {
                        flag = true;
                        break;
                    }
                }

            }
            return flag;
        }

        private void CheckBox_Click_1(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sql = "";
                    if (cb.IsChecked == true)
                    {
                        sql += "update data  set ischecked = true where id='" + cb.Tag + "';";
                        
                    }
                    else
                    {
                        sql += "update data  set ischecked = false where id='" + cb.Tag + "';";
                    }                                          
                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    {
                        command.ExecuteNonQuery();
                        
                    }
                }
                catch (Exception ex)
                {
                   
                }
                conn.Close();
            }
        }

        //重置选择状态
        private void ResetChecked()
        {
            String sql = "update data set ischecked = false;";
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                try
                {
                    conn.Open();                    
                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    {
                        command.ExecuteNonQuery();

                    }
                }
                catch (Exception ex)
                {

                }
                conn.Close();
            }
            
        }

        //头部checkbox加载事件
        private void CheckBox_Loaded(object sender, RoutedEventArgs e)
        {
            cball = (CheckBox)sender;
        }

        // 多晶体页面点击事件
        private void multiClick(object sender, MouseButtonEventArgs e)
        {
            List<String> phases = selectMutiPhaseALL();
            multiInputPhase.Items.Clear();
            multiInputPhase.Items.Add("");
            foreach (String phase in phases)
            {
                multiInputPhase.Items.Add(phase);
            }
            multiInputPhase.SelectedIndex = 0;
            ShowMlutiAllData();
        }

        //搜索所有的多晶体相
        private List<String> selectMutiPhaseALL()
        {

            string sql = "select phase from multi_data group by phase ";
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

        // 展示所有的多晶体数据
        private void ShowMlutiAllData()
        {
            index = 0;
            List<caseData> list = new List<caseData>();
            string sql1 = "SELECT * FROM multi_data ";
            string sql2 = "";
            string sql3 = "";
            string sql = sql1 + sql2 + sql3;
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                {
                    try
                    {
                        conn.Open();
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            int i = 1;
                            while (reader.Read())
                            {
                                caseData casedata = new caseData();
                                casedata.Id = Convert.ToInt32(reader["id"]);
                                casedata.FlaseId = i;
                                casedata.Phase = reader["phase"].ToString();                             
                                casedata.Temperature = Convert.ToDouble(reader["temperature"]);
                                casedata.Diff_plane = reader["diff_plane"].ToString();
                                casedata.Ehkl = Convert.ToDouble(reader["ehkl"]);
                                casedata.Vhkl = Convert.ToDouble(reader["vhkl"]);
                                casedata.IsChecked = (Boolean)reader["ischecked"];
                                list.Add(casedata);
                                i++;

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
            MultiCurrentList = list;
            multiDg.ItemsSource = null;
            multiDg.ItemsSource = list;
            // 显示多晶体的记录数
            SetMultiNumber();
        }

        // 显示多晶体的记录数
        private void SetMultiNumber()
        {
            string sql = "select count(*) from multi_data where 1 = 1";
            string sql1 = sql + " and phase='α'";
            string sql2 = sql + " and phase='β'";
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                try
                {
                    conn.Open();
                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    {
                        multiALLNumber.Content = Convert.ToInt32(command.ExecuteScalar());
                    }
                    using (SQLiteCommand command = new SQLiteCommand(sql1, conn))
                    {
                        multiFirstNumber.Content = Convert.ToInt32(command.ExecuteScalar());
                    }
                    using (SQLiteCommand command = new SQLiteCommand(sql2, conn))
                    {
                        multiSecondNumber.Content = Convert.ToInt32(command.ExecuteScalar());
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
        
        // 多个字段搜索多晶体中的值
        private void multiBtnSelect(object sender, RoutedEventArgs e)
        {
            string sql = "SELECT * FROM multi_data where 1 = 1";
            if (multiInputPhase.SelectedValue.ToString() != "" && multiInputDiff_plane.SelectedValue == null)
            {
                MessageBox.Show("请选择衍射面！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (multiInputPhase.SelectedValue.ToString() == "" && multiInputDiff_plane.SelectedValue != null)
            {
                MessageBox.Show("请选择相！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (multiInputPhase.SelectedValue.ToString() == "" && multiInputDiff_plane.SelectedValue == null)
            {
                MessageBox.Show("请选择相和衍射面！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (multiInputPhase.SelectedValue.ToString() != "")
                {
                    sql = sql + " and phase='" + multiInputPhase.SelectedValue.ToString() + "'";
                }
                if (multiInputDiff_plane.SelectedValue != null)
                {
                    sql = sql + " and diff_plane='" + multiInputDiff_plane.SelectedValue.ToString() + "'";
                }

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
                                int i = 1;
                                while (reader.Read())
                                {
                                    
                                    caseData casedata = new caseData();
                                    casedata.Id = Convert.ToInt32(reader["id"]);
                                    casedata.FlaseId = i;
                                    casedata.Phase = reader["phase"].ToString();
                                    
                                    casedata.Temperature = Convert.ToDouble(reader["temperature"]);
                                    casedata.Diff_plane = reader["diff_plane"].ToString();
                                    casedata.Ehkl = Convert.ToDouble(reader["ehkl"]);
                                    casedata.Vhkl = Convert.ToDouble(reader["vhkl"]);
                                    casedata.IsChecked = (Boolean)reader["ischecked"];
                                    list.Add(casedata);
                                    i++;
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
                MultiCurrentList = list;
                multiselectedFlag = true;
                multiDg.ItemsSource = null;
                multiDg.ItemsSource = list;
            }
        }

        // 多晶体刷新
        private void multiBtnRefresh(object sender, RoutedEventArgs e)
        {
            multiInputDiff_plane.Items.Clear();
            List<String> phases = selectMutiPhaseALL();
            multiInputPhase.Items.Clear();
            multiInputPhase.Items.Add("");
            foreach (String phase in phases)
            {
                multiInputPhase.Items.Add(phase);
            }
            multiInputPhase.SelectedIndex = 0;

            flaseIdFlag = 1;
            multiselectedFlag = false;

            multiResetChecked();
            ShowMlutiAllData();
            
        }
        
        //多晶体修改
        private void multiBtnUpdate(object sender, RoutedEventArgs e)
        {
            bool flag = false;
            int i = 0;

            foreach (caseData item in MultiCurrentList)
            {
                String presql = "select ischecked from multi_data where id='" + item.Id + "';";
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    using (SQLiteCommand command = new SQLiteCommand(presql, conn))
                    {
                        try
                        {
                            conn.Open();
                            bool ischecked = (bool)command.ExecuteScalar();
                            if (ischecked == true)
                            {
                                caseData caseData = selectMutiDataById(item.Id);
                                UpdateMutiData updateMutiData = new UpdateMutiData(caseData);
                                updateMutiData.TransfEvent += UpdateMuti_TransfEvent;
                                updateMutiData.ShowDialog();
                                flag = true;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("获取选中数据：" + "失败：" + ex.Message);
                        }
                        conn.Close();
                    }
                }
            }
            if (flag)
            {
                multiResetChecked();
                if (multiselectedFlag == true)
                {
                    multiBtnSelect(null, null);

                }
                else
                {
                    ShowMlutiAllData();
                }
            }
            else
            {
                MessageBox.Show("请选择需要操作的数据", "提示信息", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 通过ID查询多晶体数据
        private caseData selectMutiDataById(int id)
        {
            caseData casedata = new caseData();
            string sql = "select * from multi_data  where id = " + id;

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
                                casedata.Temperature = Convert.ToDouble(reader["temperature"]);
                                casedata.Diff_plane = reader["diff_plane"].ToString();
                                casedata.Ehkl = Convert.ToDouble(reader["ehkl"]);
                                casedata.Vhkl = Convert.ToDouble(reader["vhkl"]);
                                
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

        // 修改数据事件
        int UpdateMuti_TransfEvent(caseData caseData)
        {
            int result = 0;
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sql = "update multi_data  set phase = '" + caseData.Phase  + "', temperature = " +
                    caseData.Temperature + ", diff_plane = '" + caseData.Diff_plane + "', ehkl = '" + caseData.Ehkl + "', vhkl = " +
                    caseData.Vhkl  + " where 1 = 1 and id = " + caseData.Id;

                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    {
                        command.ExecuteNonQuery();
                        MessageBox.Show("修改成功", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("表中已存在该实验条件，请重新输入！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Warning);
                    result = -1;
                    //throw new Exception("修改数据失败：" + ex.Message);
                }
                conn.Close();
            }
            flaseIdFlag = 1;
            if (multiselectedFlag == true)
            {
                multiBtnSelect(null, null);

            }
            else
            {
                ShowMlutiAllData();
            }

            return result;
        }
        //多晶体删除
        private void multiBtnDelete(object sender, RoutedEventArgs e)
        {
            bool flag = false;
            string sql = "delete from multi_data where multi_data.Id in (";            
            foreach (caseData item in MultiCurrentList)
            {
                String presql = "select ischecked from multi_data where id='" + item.Id + "';";
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    using (SQLiteCommand command = new SQLiteCommand(presql, conn))
                    {
                        try
                        {
                            conn.Open();
                            bool ischecked = (bool)command.ExecuteScalar();
                            if (ischecked == true)
                            {
                                sql = sql + item.Id + ",";
                                flag = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("获取选中数据：" + "失败：" + ex.Message);
                        }
                        conn.Close();
                    }
                }
            }
            sql = sql.Substring(0, sql.Length - 1) + ");";
            //MessageBox.Show(sql);
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
                            MessageBox.Show("删除成功", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
                            multiResetChecked();
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
                MessageBox.Show("请选择需要操作的数据", "提示信息", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            //cball.IsChecked = false;

            
        }

        //多晶体增加
        private void multiBtnAdd(object sender, RoutedEventArgs e)
        {
            AddMultiData addMultiData = new AddMultiData();
            addMultiData.TransfEvent += Add_MutiTransfEvent;
            addMultiData.ShowDialog();
        }

        // 增加多晶体数据
        void Add_MutiTransfEvent(String sql)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                {
                    try
                    {
                        conn.Open();
                        command.ExecuteNonQuery();
                        MessageBox.Show("插入成功", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    catch (Exception ex)
                    {
                        throw new Exception("插入数据失败：" + ex.Message);
                    }
                    conn.Close();
                }
            }
            flaseIdFlag = 1;
            ShowMlutiAllData();
            
        }

        //多晶体重置选择状态
        private void multiResetChecked()
        {
            String sql = "update multi_data set ischecked = false;";
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                try
                {
                    conn.Open();
                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    {
                        command.ExecuteNonQuery();

                    }
                }
                catch (Exception ex)
                {

                }
                conn.Close();
            }
        }

        // 多晶体全选数据
        private void multiCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox headercb = (CheckBox)sender;
            string sql = "update multi_data set ischecked = " + headercb.IsChecked + " where multi_data.Id in ( ";
            foreach (caseData item in MultiCurrentList)
            {
                sql = sql + item.Id + ",";
            }
            sql = sql.Substring(0, sql.Length - 1) + ");";
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                try
                {
                    conn.Open();
                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("全选数据：" + "失败：" + ex.Message);
                }
                conn.Close();
            }
            if (multiselectedFlag == true)
            {
                multiBtnSelect(null, null);
            }
            else
            {
                ShowMlutiAllData();
            }
        }
        // 多晶体点击事件
        private void multiCheckBox_Click1(object sender, RoutedEventArgs e)
        {           
                CheckBox cb = (CheckBox)sender;
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    try
                    {
                        conn.Open();
                        string sql = "";
                        if (cb.IsChecked == true)
                        {
                            sql += "update multi_data  set ischecked = true where id='" + cb.Tag + "';";

                        }
                        else
                        {
                            sql += "update multi_data  set ischecked = false where id='" + cb.Tag + "';";
                        }
                        using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                        {
                            command.ExecuteNonQuery();

                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    conn.Close();
                }            
        }

        // 多晶体中的相改变事件
        private void multiInputPhase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            multiInputDiff_plane.Items.Clear();
            if (multiInputPhase.SelectedValue != null)
            {
                List<String> phases = selectMultiDiff_planeALL(multiInputPhase.SelectedValue.ToString());
                foreach (String phase in phases)
                {
                    multiInputDiff_plane.Items.Add(phase);
                }
            }
        }

        // 搜索多晶体中的所有的衍射面
        private List<string> selectMultiDiff_planeALL(string phase)
        {

            string sql = "select diff_plane from multi_data where phase = '" + phase + "' group by diff_plane";
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



        // 单晶体页面点击事件
        private void singleClick(object sender, MouseButtonEventArgs e)
        {
            List<String> phases = selectPhaseALL();
            singleInputPhase.Items.Clear();
            singleInputPhase.Items.Add("");
            foreach (String phase in phases)
            {
                singleInputPhase.Items.Add(phase);
            }
            singleInputPhase.SelectedIndex = 0;
            ShowSingleAllData();
        }

        // 展示所有的单晶体数据
        private void ShowSingleAllData()
        {
            index = 0;
            List<SingleData> list = new List<SingleData>();
            string sql1 = "SELECT * FROM single_data ";
            string sql2 = "";
            string sql3 = "";
            string sql = sql1 + sql2 + sql3;
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                {
                    try
                    {
                        conn.Open();
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            int i = 1;
                            while (reader.Read())
                            {
                                SingleData singleData = new SingleData();
                                singleData.Id = Convert.ToInt32(reader["id"]);
                                singleData.FlaseId = i;
                                singleData.Phase = reader["phase"].ToString();
                                singleData.Temperature = Convert.ToDouble(reader["temperature"]);
                                if (reader["C11"].ToString() == "")
                                {
                                    singleData.C11 = 0.00;
                                }
                                else
                                {
                                    singleData.C11 = Convert.ToDouble(reader["C11"]);
                                }
                                if (reader["C12"].ToString() == "")
                                {
                                    singleData.C12 = 0.00;
                                }
                                else
                                {
                                    singleData.C12 = Convert.ToDouble(reader["C12"]);
                                }
                                if (reader["C13"].ToString() == "")
                                {
                                    singleData.C13 = 0.00;
                                }
                                else
                                {
                                    singleData.C13 = Convert.ToDouble(reader["C13"]);
                                }
                                if (reader["C33"].ToString() == "")
                                {
                                    singleData.C33 = 0.00;
                                }
                                else
                                {
                                    singleData.C33 = Convert.ToDouble(reader["C33"]);
                                }
                                if (reader["C44"].ToString() == "")
                                {
                                    singleData.C44 = 0.00;
                                }
                                else
                                {
                                    singleData.C44 = Convert.ToDouble(reader["C44"]);
                                }

                                singleData.IsChecked = (Boolean)reader["ischecked"];
                                list.Add(singleData);
                                i++;

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
            SingleCurrentList = list;
            singleDg.ItemsSource = null;
            singleDg.ItemsSource = list;
            // 显示单晶体的记录数
            SetSingleNumber();
        }
        
        // 显示单晶体的记录数
        private void SetSingleNumber()
        {
            string sql = "select count(*) from single_data where 1 = 1";
            string sql1 = sql + " and phase='α'";
            string sql2 = sql + " and phase='β'";
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                try
                {
                    conn.Open();
                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    {
                        singleALLNumber.Content = Convert.ToInt32(command.ExecuteScalar());
                    }
                    using (SQLiteCommand command = new SQLiteCommand(sql1, conn))
                    {
                        singleFirstNumber.Content = Convert.ToInt32(command.ExecuteScalar());
                    }
                    using (SQLiteCommand command = new SQLiteCommand(sql2, conn))
                    {
                        singleSecondNumber.Content = Convert.ToInt32(command.ExecuteScalar());
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

        //单晶体搜索
        private void singleBtnSelect(object sender, RoutedEventArgs e)
        {
            string sql = "SELECT * FROM single_data where 1 = 1";
            
            if (singleInputPhase.SelectedValue.ToString() == "")
            {
                MessageBox.Show("请选择相！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Warning);
            }            
            else
            {
                if (singleInputPhase.SelectedValue.ToString() != "")
                {
                    sql = sql + " and phase='" + singleInputPhase.SelectedValue.ToString() + "'";
                }               
                List<SingleData> list = new List<SingleData>();
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    {
                        try
                        {
                            conn.Open();
                            using (SQLiteDataReader reader = command.ExecuteReader())
                            {
                                int i = 1;
                                while (reader.Read())
                                {

                                    SingleData singleData = new SingleData();
                                    singleData.Id = Convert.ToInt32(reader["id"]);
                                    singleData.FlaseId = i;
                                    singleData.Phase = reader["phase"].ToString();
                                    singleData.Temperature = Convert.ToDouble(reader["temperature"]);
                                    if (reader["C11"].ToString() == "")
                                    {
                                        singleData.C11 = 0.00;
                                    }
                                    else
                                    {
                                        singleData.C11 = Convert.ToDouble(reader["C11"]);
                                    }
                                    if (reader["C12"].ToString() == "")
                                    {
                                        singleData.C12 = 0.00;
                                    }
                                    else
                                    {
                                        singleData.C12 = Convert.ToDouble(reader["C12"]);
                                    }
                                    if (reader["C13"].ToString() == "")
                                    {
                                        singleData.C13 = 0.00;
                                    }
                                    else
                                    {
                                        singleData.C13 = Convert.ToDouble(reader["C13"]);
                                    }
                                    if (reader["C33"].ToString() == "")
                                    {
                                        singleData.C33 = 0.00;
                                    }
                                    else
                                    {
                                        singleData.C33 = Convert.ToDouble(reader["C33"]);
                                    }
                                    if (reader["C44"].ToString() == "")
                                    {
                                        singleData.C44 = 0.00;
                                    }
                                    else
                                    {
                                        singleData.C44 = Convert.ToDouble(reader["C44"]);
                                    }

                                    singleData.IsChecked = (Boolean)reader["ischecked"];
                                    list.Add(singleData);
                                    i++;
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
                SingleCurrentList = list;
                singleselectedFlag = true;
                singleDg.ItemsSource = null;
                singleDg.ItemsSource = list;
            }
        }

        //单晶体刷新
        private void singleBtnRefresh(object sender, RoutedEventArgs e)
        {            
            List<String> phases = selectPhaseALL();
            singleInputPhase.Items.Clear();
            singleInputPhase.Items.Add("");
            foreach (String phase in phases)
            {
                singleInputPhase.Items.Add(phase);
            }
            singleInputPhase.SelectedIndex = 0;

            flaseIdFlag = 1;
            singleselectedFlag = false;

            singleResetChecked();
            ShowSingleAllData();
        }

        //单晶体修改
        private void singleBtnUpdate(object sender, RoutedEventArgs e)
        {
            bool flag = false;
            int i = 0;

            foreach (SingleData item in SingleCurrentList)
            {
                String presql = "select ischecked from single_data where id='" + item.Id + "';";
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    using (SQLiteCommand command = new SQLiteCommand(presql, conn))
                    {
                        try
                        {
                            conn.Open();
                            bool ischecked = (bool)command.ExecuteScalar();
                            if (ischecked == true)
                            {
                                SingleData singleData = selectSingleDataById(item.Id);
                                UpdateSingleData updateSingleData = new UpdateSingleData(singleData);
                                updateSingleData.TransfEvent += UpdateSingle_TransfEvent;
                                updateSingleData.ShowDialog();
                                flag = true;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("获取选中数据：" + "失败：" + ex.Message);
                        }
                        conn.Close();
                    }
                }
            }
            if (flag)
            { 
                singleResetChecked();
                if (singleselectedFlag == true)
                {
                    singleBtnSelect(null, null);
                }
                else
                { 
                    ShowSingleAllData();
                }
            }
            else
            {
                MessageBox.Show("请选择需要操作的数据", "提示信息", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 通过ID查询多晶体数据
        private SingleData selectSingleDataById(int id)
        {
            SingleData singleData = new SingleData();
            string sql = "select * from single_data  where id = " + id;

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
                                singleData.Id = Convert.ToInt32(reader["id"]);
                                singleData.Phase = reader["phase"].ToString();
                                singleData.Temperature = Convert.ToDouble(reader["temperature"]);
                                singleData.C11 = Convert.ToDouble(reader["C11"]);
                                singleData.C12 = Convert.ToDouble(reader["C12"]);
                                singleData.C13 = Convert.ToDouble(reader["C13"]);
                                singleData.C33 = Convert.ToDouble(reader["C33"]);
                                singleData.C44 = Convert.ToDouble(reader["C44"]);

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("查询失败123", "");
                        throw new Exception("查询数据失败：" + ex.Message);
                    }
                    conn.Close();
                    return singleData;
                }
            }

        }

        // 修改数据事件
        int UpdateSingle_TransfEvent(SingleData singleData)
        {
            int result = 0;
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sql = "update single_data  set phase = '" + singleData.Phase + "', temperature = " +
                    singleData.Temperature + ", C11= " + singleData.C11 + ", C12 = " + singleData.C12 + ", C13 = " +
                    singleData.C13 + ", C33 = " + singleData.C33 + ", C44 = " + singleData.C44 + 
                    " where 1 = 1 and id = " + singleData.Id;

                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    {
                        command.ExecuteNonQuery();
                        MessageBox.Show("修改成功", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("表中已存在该实验条件，请重新输入！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Warning);
                    result = -1;
                    //throw new Exception("修改数据失败：" + ex.Message);
                }
                conn.Close();
            }
            flaseIdFlag = 1;
            if (singleselectedFlag == true)
            {
                singleBtnSelect(null, null);

            }
            else
            {
                ShowSingleAllData();
            }

            return result;
        }
        // 单晶体删除
        private void singleBtnDelete(object sender, RoutedEventArgs e)
        {
            bool flag = false;
            string sql = "delete from single_data where single_data.Id in (";
            foreach (SingleData item in SingleCurrentList)
            {
                String presql = "select ischecked from single_data where id='" + item.Id + "';";
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    using (SQLiteCommand command = new SQLiteCommand(presql, conn))
                    {
                        try
                        {
                            conn.Open();
                            bool ischecked = (bool)command.ExecuteScalar();
                            if (ischecked == true)
                            {
                                sql = sql + item.Id + ",";
                                flag = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("获取选中数据：" + "失败：" + ex.Message);
                        }
                        conn.Close();
                    }
                }
            }
            sql = sql.Substring(0, sql.Length - 1) + ");";
            //MessageBox.Show(sql);
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
                            MessageBox.Show("删除成功", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
                            ResetChecked();
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
                MessageBox.Show("请选择需要操作的数据", "提示信息", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            cball.IsChecked = false;

            if (singleselectedFlag == true)
            {
                singleBtnSelect(null, null);

            }
            else
            {
                ShowSingleAllData();
            }
        }

        //单晶体增加
        private void singleBtnAdd(object sender, RoutedEventArgs e)
        {
            AddSingleData addSingleData = new AddSingleData();
            addSingleData.TransfEvent += Add_SingleTransfEvent;
            addSingleData.ShowDialog();
        }

        // 增加多晶体数据
        void Add_SingleTransfEvent(String sql)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                {
                    try
                    {
                        conn.Open();
                        command.ExecuteNonQuery();
                        MessageBox.Show("插入成功", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    catch (Exception ex)
                    {
                        throw new Exception("插入数据失败：" + ex.Message);
                    }
                    conn.Close();
                }
            }
            flaseIdFlag = 1;
            ShowSingleAllData();

        }

        //单晶体重置选择状态
        private void singleResetChecked()
        {
            String sql = "update single_data set ischecked = false;";
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                try
                {
                    conn.Open();
                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    {
                        command.ExecuteNonQuery();

                    }
                }
                catch (Exception ex)
                {

                }
                conn.Close();
            }
        }

        // 单晶体全选数据
        private void singleCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox headercb = (CheckBox)sender;
            string sql = "update single_data set ischecked = " + headercb.IsChecked + " where single_data.Id in ( ";
            foreach (SingleData item in SingleCurrentList)
            {
                sql = sql + item.Id + ",";
            }
            sql = sql.Substring(0, sql.Length - 1) + ");";
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                try
                {
                    conn.Open();
                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("全选数据：" + "失败：" + ex.Message);
                }
                conn.Close();
            }
            if (singleselectedFlag == true)
            {
                singleBtnSelect(null, null);
            }
            else
            {
                ShowSingleAllData();
            }
        }
        // 单晶体点击事件
        private void singleCheckBox_Click1(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sql = "";
                    if (cb.IsChecked == true)
                    {
                        sql += "update single_data  set ischecked = true where id='" + cb.Tag + "';";

                    }
                    else
                    {
                        sql += "update single_data  set ischecked = false where id='" + cb.Tag + "';";
                    }
                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    {
                        command.ExecuteNonQuery();

                    }
                }
                catch (Exception ex)
                {

                }
                conn.Close();
            }
        }
        

        
    }
}
