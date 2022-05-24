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
    public partial class UpdateData : Window
    {

        private static int id;
        public UpdateData(caseData caseData)
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
            Phase_ratio.Text = caseData.Phase_ratio.ToString();
            Temperature.Text = caseData.Temperature.ToString();
            Diff_plane.SelectedValue = caseData.Diff_plane;
            Ehkl.Text = caseData.Ehkl.ToString();
            Vhkl.Text = caseData.Vhkl.ToString();
            Distance.Text = caseData.Distance.ToString();           
        }

        public delegate void TransfDelegate(caseData caseData);

        public event TransfDelegate TransfEvent;
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            caseData caseData = new caseData();
            caseData.Id = id;
            caseData.Phase = Phase.SelectedValue.ToString();
            caseData.Phase_ratio = Convert.ToDouble(Phase_ratio.Text.Trim());
            caseData.Temperature = Convert.ToDouble(Temperature.Text.Trim());
            caseData.Diff_plane = Diff_plane.SelectedValue.ToString();
            caseData.Ehkl = Convert.ToDouble(Ehkl.Text.Trim());
            caseData.Vhkl = Convert.ToDouble(Vhkl.Text.Trim());
            caseData.Distance = Convert.ToDouble(Distance.Text.Trim());
            TransfEvent(caseData);//触发事件
            this.Close();
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
    }
}
