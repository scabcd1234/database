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
        public UpdateData(caseData caseData, List<String> phases)
        {
            InitializeComponent();
            SetPhase(phases);
            show(caseData);            
        }

        private void SetPhase(List<String> phases)
        {
            foreach (String phase in phases)
            {
                Phase.Items.Add(phase);
            }
        }

        public void show(caseData caseData)

        {
            id = caseData.Id;
            Phase.SelectedValue= caseData.Phase;
             
            Phase_ratio.Text = caseData.Phase_ratio.ToString();
            Temperature.Text = caseData.Temperature.ToString();
            Diff_plane.Text = caseData.Diff_plane;
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
            caseData.Phase = Phase.Text.Trim();
            caseData.Phase_ratio = Convert.ToInt32(Phase_ratio.Text.Trim());
            caseData.Temperature = Convert.ToInt32(Temperature.Text.Trim());
            caseData.Diff_plane = Diff_plane.Text.Trim();
            caseData.Ehkl = Convert.ToInt32(Ehkl.Text.Trim());
            caseData.Vhkl = Convert.ToDouble(Vhkl.Text.Trim());
            caseData.Distance = Convert.ToDouble(Distance.Text.Trim());
            TransfEvent(caseData);//触发事件
            this.Close();
        }
    }
}
