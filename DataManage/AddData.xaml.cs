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

        public class Add {
            /* 
             Phase，Phase_ratio，Temperature，Diff_plane，Ehkl，Vhkl，Distance
             */
            public static string Phase;
            public static int Phase_ratio;
            public static int Temperature;
            public static string Diff_plane;
            public static int Ehkl;
            public static double Vhkl;
            public static double Distance;
        }

        public AddData()
        {
            InitializeComponent();
        }
        public delegate void TransfDelegate(string value1,int value2, int value3, string value4, int value5, double value6, double value7);

        public event TransfDelegate TransfEvent;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Add.Phase = Phase.Text.Trim() ;
            Add.Phase_ratio = Convert.ToInt32(Phase_ratio.Text.Trim());
            Add.Temperature = Convert.ToInt32(Temperature.Text.Trim());
            Add.Diff_plane = Diff_plane.Text.Trim();
            Add.Ehkl = Convert.ToInt32(Ehkl.Text.Trim());
            Add.Vhkl = Convert.ToDouble(Vhkl.Text.Trim()) ;
            Add.Distance = Convert.ToDouble(Distance.Text.Trim());
            TransfEvent(Add.Phase, Add.Phase_ratio, Add.Temperature, Add.Diff_plane, Add.Ehkl, Add.Vhkl, Add.Distance);//触发事件
            this.Close();
        }

        

        
        
    }
}
