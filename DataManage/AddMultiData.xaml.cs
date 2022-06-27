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
    public partial class AddMultiData : Window
    {     

        public AddMultiData()
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
            if (string.IsNullOrEmpty(inputPhase.Text.Trim()))
            {
                phaseLabel.Visibility = Visibility.Visible;
                phaseLabel.Content = "相不能为空！";
            }else if (string.IsNullOrEmpty(Phase_ratio.Text.ToString()))
            {
                phaseLabel.Visibility = Visibility.Hidden;
                Phase_ratioLabel.Visibility = Visibility.Visible;
                Phase_ratioLabel.Content = "相比例不能为空！";
            }
            else if (isNotDouble(Phase_ratio.Text.ToString()))
            {
                Phase_ratioLabel.Visibility = Visibility.Visible;
                Phase_ratioLabel.Content = "请输入double类型！";
            }
            else if (string.IsNullOrEmpty(Temperature.Text.ToString()))
            {
                Phase_ratioLabel.Visibility = Visibility.Hidden;
                TemperatureLabel.Visibility = Visibility.Visible;
                TemperatureLabel.Content = "温度不能为空！";
            }
            else if (isNotDouble(Temperature.Text.ToString()))
            {
                TemperatureLabel.Visibility = Visibility.Visible;
                TemperatureLabel.Content = "请输入double类型！";
            }
            else if (string.IsNullOrEmpty(inputDiff_plane.Text.Trim()))
            {
                TemperatureLabel.Visibility = Visibility.Hidden;
                Diff_planeLabel.Visibility = Visibility.Visible;
                Diff_planeLabel.Content = "衍射面不能为空！";
            }
            else if (string.IsNullOrEmpty(Ehkl.Text.ToString()))
            {
                Diff_planeLabel.Visibility = Visibility.Hidden;
                EhklLabel.Visibility = Visibility.Visible;
                EhklLabel.Content = "Ehkl不能为空！";
            }
            else if (isNotDouble(Ehkl.Text.ToString()))
            {
                EhklLabel.Visibility = Visibility.Visible;
                EhklLabel.Content = "请输入double类型！";
            }
            else if (string.IsNullOrEmpty(Vhkl.Text.ToString()))
            {
                EhklLabel.Visibility = Visibility.Hidden;
                VhklLabel.Visibility = Visibility.Visible;
                VhklLabel.Content = "vhkl不能为空！";
            }
            else if (isNotDouble(Vhkl.Text.ToString()))
            {
                VhklLabel.Visibility = Visibility.Visible;
                VhklLabel.Content = "请输入double类型！";
            }
            else if (string.IsNullOrEmpty(Distance.Text.ToString()))
            {
                VhklLabel.Visibility = Visibility.Hidden;
                DistanceLabel.Visibility = Visibility.Visible;
                DistanceLabel.Content = "晶面间距d不能为空！";
            }
            else if (isNotDouble(Distance.Text.ToString()))
            {
                DistanceLabel.Visibility = Visibility.Visible;
                DistanceLabel.Content = "请输入double类型！";
            }
            else
            {
                DistanceLabel.Visibility = Visibility.Hidden;
                string sql = "replace into data (phase,phase_ratio,temperature,diff_plane,ehkl,vhkl,distance) values ('"
                    + inputPhase.SelectedValue.ToString() + "','" + Phase_ratio.Text.Trim() + "','" + Temperature.Text.Trim() + "','" + inputDiff_plane.Text.Trim() + "','"
                    + Ehkl.Text.Trim() + "','" + Vhkl.Text.Trim() + "','" + Distance.Text.Trim() + "');";
                //MessageBox.Show(sql);
                TransfEvent(sql);//触发事件
                this.Close();
            }
                
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


        // 相失去焦点 LostFocus="inputPhase_LostFocus"
        private void inputPhase_LostFocus(object sender, RoutedEventArgs e)
        {

            string temp = inputPhase.Text.Trim();
            if (string.IsNullOrEmpty(temp))
            {
                phaseLabel.Visibility = Visibility.Visible;
                phaseLabel.Content = "相不能为空！";
            }
            else
            {
                phaseLabel.Visibility = Visibility.Hidden;
            }
        }

        // 相比例失去焦点
        private void Phase_ratio_LostFocus(object sender, RoutedEventArgs e)
        {
            string temp = Phase_ratio.Text.ToString();
            if (string.IsNullOrEmpty(temp))
            {
                Phase_ratioLabel.Visibility = Visibility.Visible;
                Phase_ratioLabel.Content = "相比例不能为空！";
            }
            else if(isNotDouble(temp))
            {
                Phase_ratioLabel.Visibility = Visibility.Visible;
                Phase_ratioLabel.Content = "请输入double类型！";

            }
            else
            {
                Phase_ratioLabel.Visibility = Visibility.Hidden;
            }
            
        }

        // 温度失去焦点
        private void Temperature_LostFocus(object sender, RoutedEventArgs e)
        {
            string temp = Temperature.Text.ToString();
            if (string.IsNullOrEmpty(temp))
            {
                TemperatureLabel.Visibility = Visibility.Visible;
                TemperatureLabel.Content = "温度不能为空！";
            }
            else if (isNotDouble(temp))
            {
                TemperatureLabel.Visibility = Visibility.Visible;
                TemperatureLabel.Content = "请输入double类型！";

            }
            else
            {
                TemperatureLabel.Visibility = Visibility.Hidden;
            }
        }

        // 衍射面失去焦点 LostFocus="inputDiff_plane_LostFocus"
        private void inputDiff_plane_LostFocus(object sender, RoutedEventArgs e)
        {
            string temp = inputDiff_plane.Text.Trim();
            if (string.IsNullOrEmpty(temp))
            {
                Diff_planeLabel.Visibility = Visibility.Visible;
                Diff_planeLabel.Content = "衍射面不能为空！";
            }
            else
            {
                Diff_planeLabel.Visibility = Visibility.Hidden;
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

        // 衍射弹性常数Ehkl（GPa）失去焦点
        private void Ehkl_LostFocus(object sender, RoutedEventArgs e)
        {
            string temp = Ehkl.Text.ToString();
            if (string.IsNullOrEmpty(temp))
            {
                EhklLabel.Visibility = Visibility.Visible;
                EhklLabel.Content = "Ehkl不能为空！";
            }
            else if (isNotDouble(temp))
            {
                EhklLabel.Visibility = Visibility.Visible;
                EhklLabel.Content = "请输入double类型！";

            }
            else
            {
                EhklLabel.Visibility = Visibility.Hidden;
            }
        }

        // 衍射弹性常数vhkl失去焦点
        private void Vhkl_LostFocus(object sender, RoutedEventArgs e)
        {
            string temp = Vhkl.Text.ToString();
            if (string.IsNullOrEmpty(temp))
            {
                VhklLabel.Visibility = Visibility.Visible;
                VhklLabel.Content = "vhkl不能为空！";
            }
            else if (isNotDouble(temp))
            {
                VhklLabel.Visibility = Visibility.Visible;
                VhklLabel.Content = "请输入double类型！";

            }
            else
            {
                VhklLabel.Visibility = Visibility.Hidden;
            }
        }
        
        // 晶面间距d失去焦点
        private void Distance_LostFocus(object sender, RoutedEventArgs e)
        {
            string temp = Distance.Text.ToString();
            if (string.IsNullOrEmpty(temp))
            {
                DistanceLabel.Visibility = Visibility.Visible;
                DistanceLabel.Content = "晶面间距d不能为空！";
            }
            else if (isNotDouble(temp))
            {
                DistanceLabel.Visibility = Visibility.Visible;
                DistanceLabel.Content = "请输入double类型！";

            }
            else
            {
                DistanceLabel.Visibility = Visibility.Hidden;
            }
        }
    }
}
