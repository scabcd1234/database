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
using System.Windows.Shapes;
using System.Data.SQLite;


namespace DataManage
{
    /// <summary>
    /// AddData.xaml 的交互逻辑
    /// </summary>
    public partial class AddData : Window
    {     

        public AddData()
        {
            InitializeComponent();
        }
        public static string dbpath = AppDomain.CurrentDomain.BaseDirectory + @"mydb.db";

        public static string connStr = @"Data Source=" + dbpath + @";Initial Catalog=sqlite;Version=3;";

        public delegate void TransfDelegate(String sql);

        public event TransfDelegate TransfEvent;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            List<String> phases = selectPhaseALL();
            inputPhase.Items.Add("");
            foreach (String phase in phases)
            {
                inputPhase.Items.Add(phase);
            }
            inputPhase.SelectedIndex = 0;

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {                     
            string sql = "insert into data (phase,phase_ratio,temperature,diff_plane,ehkl,vhkl,distance) values ('"
                    + inputPhase.SelectedValue.ToString() + "','" + Phase_ratio.Text.Trim() + "','" + Temperature.Text.Trim() + "','" + Diff_plane.Text.Trim() + "','"
                    + Ehkl.Text.Trim() + "','" + Vhkl.Text.Trim() + "','" + Distance.Text.Trim() + "');";
            //MessageBox.Show(sql);
            TransfEvent(sql);//触发事件
            this.Close();
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




    }
}
