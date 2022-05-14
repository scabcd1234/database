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
    /// AddData.xaml 的交互逻辑
    /// </summary>
    public partial class AddData : Window
    {
        public AddData()
        {
            InitializeComponent();
        }
        public delegate void TransfDelegate(String value);

        public event TransfDelegate TransfEvent;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TransfEvent("haha");//触发事件

            this.Close();
        }
    

    }
}
