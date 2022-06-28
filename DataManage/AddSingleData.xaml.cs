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
    public partial class AddSingleData : Window
    {     

        public AddSingleData()
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
            }
            else if (string.IsNullOrEmpty(Temperature.Text.ToString()))
            {
                phaseLabel.Visibility = Visibility.Hidden;
                TemperatureLabel.Visibility = Visibility.Visible;
                TemperatureLabel.Content = "温度不能为空！";
            }
            else if (isNotDouble(Temperature.Text.ToString()))
            {
                TemperatureLabel.Visibility = Visibility.Visible;
                TemperatureLabel.Content = "请输入double类型！";
            }
            else if (string.IsNullOrEmpty(C11.Text.Trim()))
            {
                TemperatureLabel.Visibility = Visibility.Hidden;
                C1lLabel.Visibility = Visibility.Visible;
                C1lLabel.Content = "C11不能为空！";
            }else if (isNotDouble(C11.Text.ToString()))
            {
                C1lLabel.Visibility = Visibility.Visible;
                C1lLabel.Content = "请输入double类型！";
            }
            else if (string.IsNullOrEmpty(C12.Text.ToString()))
            {
                C1lLabel.Visibility = Visibility.Hidden;
                C12Label.Visibility = Visibility.Visible;
                C12Label.Content = "C12不能为空！";
            }
            else if (isNotDouble(C12.Text.ToString()))
            {
                C12Label.Visibility = Visibility.Visible;
                C12Label.Content = "请输入double类型！";
            }
            else if (string.IsNullOrEmpty(C13.Text.ToString()))
            {
                C12Label.Visibility = Visibility.Hidden;
                C13Label.Visibility = Visibility.Visible;
                C13Label.Content = "C13不能为空！";
            }
            else if (isNotDouble(C13.Text.ToString()))
            {
                C13Label.Visibility = Visibility.Visible;
                C13Label.Content = "请输入double类型！";
            }
            else if (string.IsNullOrEmpty(C33.Text.ToString()))
            {
                C13Label.Visibility = Visibility.Hidden;
                C33Label.Visibility = Visibility.Visible;
                C33Label.Content = "C33不能为空！";
            }
            else if (isNotDouble(C33.Text.ToString()))
            {
                C33Label.Visibility = Visibility.Visible;
                C33Label.Content = "请输入double类型！";
            }
            else if (string.IsNullOrEmpty(C44.Text.ToString()))
            {
                C33Label.Visibility = Visibility.Hidden;
                C44Label.Visibility = Visibility.Visible;
                C44Label.Content = "C44不能为空！";
            }
            else if (isNotDouble(C44.Text.ToString()))
            {
                C44Label.Visibility = Visibility.Visible;
                C44Label.Content = "请输入double类型！";
            }
            else
            {
                C44Label.Visibility = Visibility.Hidden;
                string sql = "replace into single_data (phase, temperature, C11, C12, C13, C33, C44) values ('"
                    + inputPhase.SelectedValue.ToString() + "','"  + Temperature.Text.Trim() + "','" + C11.Text.Trim() + "','"
                    + C12.Text.Trim() + "','" + C13.Text.Trim() + "','" + C33.Text.Trim() + "','"  + C44.Text.Trim() + "');";
                //MessageBox.Show(sql);
                TransfEvent(sql);//触发事件
                this.Close();
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

        // C11失去焦点
        private void C11_LostFocus(object sender, RoutedEventArgs e)
        {
            string temp = C11.Text.ToString();
            if (string.IsNullOrEmpty(temp))
            {
                C1lLabel.Visibility = Visibility.Visible;
                C1lLabel.Content = "C1l不能为空！";
            }
            else if (isNotDouble(temp))
            {
                C1lLabel.Visibility = Visibility.Visible;
                C1lLabel.Content = "请输入double类型！";
            }
            else
            {
                C1lLabel.Visibility = Visibility.Hidden;
            }

        }

        // C12失去焦点
        private void C12_LostFocus(object sender, RoutedEventArgs e)
        {
            string temp = C12.Text.ToString();
            if (string.IsNullOrEmpty(temp))
            {
                C12Label.Visibility = Visibility.Visible;
                C12Label.Content = "C12不能为空！";
            }
            else if (isNotDouble(temp))
            {
                C12Label.Visibility = Visibility.Visible;
                C12Label.Content = "请输入double类型！";
            }
            else
            {
                C12Label.Visibility = Visibility.Hidden;
            }

        }
        // C13失去焦点
        private void C13_LostFocus(object sender, RoutedEventArgs e)
        {
            string temp = C13.Text.ToString();
            if (string.IsNullOrEmpty(temp))
            {
                C13Label.Visibility = Visibility.Visible;
                C13Label.Content = "C13不能为空！";
            }
            else if (isNotDouble(temp))
            {
                C13Label.Visibility = Visibility.Visible;
                C13Label.Content = "请输入double类型！";
            }
            else
            {
                C13Label.Visibility = Visibility.Hidden;
            }

        }
        // C33失去焦点
        private void C33_LostFocus(object sender, RoutedEventArgs e)
        {
            string temp = C33.Text.ToString();
            if (string.IsNullOrEmpty(temp))
            {
                C33Label.Visibility = Visibility.Visible;
                C33Label.Content = "C33不能为空！";
            }
            else if (isNotDouble(temp))
            {
                C33Label.Visibility = Visibility.Visible;
                C33Label.Content = "请输入double类型！";
            }
            else
            {
                C33Label.Visibility = Visibility.Hidden;
            }

        }

        // C44失去焦点
        private void C44_LostFocus(object sender, RoutedEventArgs e)
        {
            string temp = C44.Text.ToString();
            if (string.IsNullOrEmpty(temp))
            {
                C44Label.Visibility = Visibility.Visible;
                C44Label.Content = "C44不能为空！";
            }
            else if (isNotDouble(temp))
            {
                C44Label.Visibility = Visibility.Visible;
                C44Label.Content = "请输入double类型！";
            }
            else
            {
                C44Label.Visibility = Visibility.Hidden;
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

       
    }
}
