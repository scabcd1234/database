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
using System.Windows.Shapes;

namespace DataManage
{
    /// <summary>
    /// UpdateData.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateMutiData : Window
    {

        private static int id;
        public UpdateMutiData(caseData caseData)
        {
            InitializeComponent();
            SetPhase();
            show(caseData);            
        }

        private void SetPhase()
        {
            Phase.Items.Add("α");
            Phase.Items.Add("β");
        }

        public void show(caseData caseData)
        {
            id = caseData.Id;
            Phase.SelectedValue = caseData.Phase;
            Temperature.Text = caseData.Temperature.ToString();
            Diff_plane.SelectedValue = caseData.Diff_plane;
            Ehkl.Text = caseData.Ehkl.ToString();
            Vhkl.Text = caseData.Vhkl.ToString();           
        }

        public delegate int TransfDelegate(caseData caseData);

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
            }else if (string.IsNullOrEmpty(Diff_plane.Text.Trim()))
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
            }else if (isNotDouble(Ehkl.Text.ToString()))
            {
                EhklLabel.Visibility = Visibility.Visible;
                EhklLabel.Content = "请输入double类型！";
            }else if (string.IsNullOrEmpty(Vhkl.Text.ToString()))
            {
                EhklLabel.Visibility = Visibility.Hidden;
                VhklLabel.Visibility = Visibility.Visible;
                VhklLabel.Content = "vhkl不能为空！";
            }else if (isNotDouble(Vhkl.Text.ToString()))
            {
                VhklLabel.Visibility = Visibility.Visible;
                VhklLabel.Content = "请输入double类型！";
            }
            else
            {
                VhklLabel.Visibility = Visibility.Hidden;
                caseData caseData = new caseData();
                caseData.Id = id;
                caseData.Phase = Phase.SelectedValue.ToString();
                
                caseData.Temperature = Convert.ToDouble(Temperature.Text.Trim());
                caseData.Diff_plane = Diff_plane.SelectedValue.ToString();
                caseData.Ehkl = Convert.ToDouble(Ehkl.Text.Trim());
                caseData.Vhkl = Convert.ToDouble(Vhkl.Text.Trim());
                
                int result=TransfEvent(caseData);//触发事件
                if (result != -1){
                    this.Close();
                }
                
            }
            
        }

        //相的点击事件
        private void inputPhase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Diff_plane.Items.Clear();
            if (Phase.SelectedValue.ToString() == "α")
            {
                Diff_plane.Items.Add("101");
                Diff_plane.Items.Add("100");
                Diff_plane.Items.Add("103");
                Diff_plane.Items.Add("002");
                Diff_plane.Items.Add("011");
            }
            if (Phase.SelectedValue.ToString() == "β")
            {
                Diff_plane.Items.Add("100");
                Diff_plane.Items.Add("101");
                Diff_plane.Items.Add("102");
                Diff_plane.Items.Add("211");
                Diff_plane.Items.Add("110");
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

        // 衍射面失去焦点
        private void Diff_plane_LostFocus(object sender, RoutedEventArgs e)
        {
            string temp = Diff_plane.Text.Trim();
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
    }
}
