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

namespace DataManage
{
    /// <summary>
    /// UpdateData.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateSingleData : Window
    {

        private static int id;
        public UpdateSingleData(SingleData singleData)
        {
            InitializeComponent();
            SetPhase();
            show(singleData);            
        }

        private void SetPhase()
        {
            Phase.Items.Add("α");
            Phase.Items.Add("β");
        }

        public void show(SingleData singleData)

        {
            
            id = singleData.Id;
            Phase.SelectedValue = singleData.Phase;
            Temperature.Text = singleData.Temperature.ToString();
            C11.Text = singleData.C11.ToString();
            C12.Text = singleData.C12.ToString();
            C13.Text = singleData.C13.ToString();
            C33.Text = singleData.C33.ToString();
            C44.Text = singleData.C44.ToString();           
        }

        public delegate int TransfDelegate(SingleData singleData);

        public event TransfDelegate TransfEvent;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Temperature.Text.ToString()))
            {
                
                TemperatureLabel.Visibility = Visibility.Visible;
                TemperatureLabel.Content = "温度不能为空！";
            }else if (isNotDouble(Temperature.Text.ToString()))
            {
                TemperatureLabel.Visibility = Visibility.Visible;
                TemperatureLabel.Content = "请输入double类型！";
            }else if (string.IsNullOrEmpty(C11.Text.Trim()))
            {
                TemperatureLabel.Visibility = Visibility.Hidden;
                C11Label.Visibility = Visibility.Visible;
                C11Label.Content = "C11不能为空！";
            }else if (isNotDouble(C11.Text.ToString()))
            {
                C11Label.Visibility = Visibility.Visible;
                C11Label.Content = "请输入double类型！";
            }
            else if (string.IsNullOrEmpty(C12.Text.ToString()))
            {
                C11Label.Visibility = Visibility.Hidden;
                C12Label.Visibility = Visibility.Visible;
                C12Label.Content = "C12不能为空！";
            }else if (isNotDouble(C12.Text.ToString()))
            {
                C12Label.Visibility = Visibility.Visible;
                C12Label.Content = "请输入double类型！";
            }else if (string.IsNullOrEmpty(C13.Text.ToString()))
            {
                C12Label.Visibility = Visibility.Hidden;
                C13Label.Visibility = Visibility.Visible;
                C13Label.Content = "C13不能为空！";
            }else if (isNotDouble(C13.Text.ToString()))
            {
                C13Label.Visibility = Visibility.Visible;
                C13Label.Content = "请输入double类型！";
            }else if (string.IsNullOrEmpty(C33.Text.ToString()))
            {
                C13Label.Visibility = Visibility.Hidden;
                C33Label.Visibility = Visibility.Visible;
                C33Label.Content = "C33不能为空！";
            }else if (isNotDouble(C33.Text.ToString()))
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
                SingleData singleData = new SingleData();
                singleData.Id = id;
                singleData.Phase = Phase.SelectedValue.ToString();
                singleData.Temperature = Convert.ToDouble(Temperature.Text.Trim());
                singleData.C11 = Convert.ToDouble(C11.Text.Trim());
                singleData.C12 = Convert.ToDouble(C12.Text.Trim());
                singleData.C13 = Convert.ToDouble(C13.Text.Trim());
                singleData.C33 = Convert.ToDouble(C33.Text.Trim());
                singleData.C44 = Convert.ToDouble(C44.Text.Trim());
                int result = TransfEvent(singleData);//触发事件
                if (result != -1){
                    this.Close();
                }
                
            }
            
        }

        
        
        

        //温度失去焦点
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
                C11Label.Visibility = Visibility.Visible;
                C11Label.Content = "C11不能为空！";

            }
            else if (isNotDouble(temp))
            {
                C11Label.Visibility = Visibility.Visible;
                C11Label.Content = "请输入double类型！";

            }
            else
            {
                C11Label.Visibility = Visibility.Hidden;

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

        // C33失去焦点
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
