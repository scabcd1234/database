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
            SetPhaseAndDiffPlane();

        }
        public static string dbpath = AppDomain.CurrentDomain.BaseDirectory + @"mydb.db";

        public static string connStr = @"Data Source=" + dbpath + @";Initial Catalog=sqlite;Version=3;";

        public delegate void TransfDelegate(String sql);

        public event TransfDelegate TransfEvent;

        private void SetPhaseAndDiffPlane()
        {            
            inputPhase.Items.Add("α");
            inputPhase.Items.Add("β");
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {                     
            string sql = "insert into data (phase,phase_ratio,temperature,diff_plane,ehkl,vhkl,distance) values ('"
                    + inputPhase.SelectedValue.ToString() + "','" + Phase_ratio.Text.Trim() + "','" + Temperature.Text.Trim() + "','" + inputDiff_plane.Text.Trim() + "','"
                    + Ehkl.Text.Trim() + "','" + Vhkl.Text.Trim() + "','" + Distance.Text.Trim() + "');";
            //MessageBox.Show(sql);
            TransfEvent(sql);//触发事件
            this.Close();
        }

        //相的点击事件
        private void inputPhase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            inputDiff_plane.Items.Clear();
            if (inputPhase.SelectedValue.ToString() == "α")
            {
                inputDiff_plane.Items.Add("101");
                inputDiff_plane.Items.Add("100");
                inputDiff_plane.Items.Add("103");
                inputDiff_plane.Items.Add("002");
                inputDiff_plane.Items.Add("011");
            }
            if (inputPhase.SelectedValue.ToString() == "β")
            {
                inputDiff_plane.Items.Add("100");
                inputDiff_plane.Items.Add("101");
                inputDiff_plane.Items.Add("102");
                inputDiff_plane.Items.Add("211");
                inputDiff_plane.Items.Add("110");
            }

        }

    }
}
