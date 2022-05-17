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
        public delegate void TransfDelegate(String sql);

        public event TransfDelegate TransfEvent;

        private void Button_Click(object sender, RoutedEventArgs e)
        {                     
            string sql = "insert into data (phase,phase_ratio,temperature,diff_plane,ehkl,vhkl,distance) values ('"
                    + Phase.Text.Trim() + "','" + Phase_ratio.Text.Trim() + "','" + Temperature.Text.Trim() + "','" + Diff_plane.Text.Trim() + "','"
                    + Ehkl.Text.Trim() + "','" + Vhkl.Text.Trim() + "','" + Distance.Text.Trim() + "');";
            MessageBox.Show(sql);
            TransfEvent(sql);//触发事件
            this.Close();
        }

        

        
        
    }
}
