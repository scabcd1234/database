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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataManage
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 测试
    public partial class MainWindow : Window
    {
        public class Student
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<Student> list = new List<Student>();
            Student student1 = new Student { Id = 1, Name = "yy", Age = 22 };
            Student student2 = new Student { Id = 2, Name = "zz", Age = 22 };
            Student student3 = new Student { Id = 3, Name = "xxx", Age = 22 };
            Student student4 = new Student { Id = 4, Name = "ttt", Age = 22 };
            list.Add(student1);
            list.Add(student2);
            list.Add(student3);
            list.Add(student4);

            dg1.ItemsSource = list;
        }

        private void dg1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Student student= (sender as DataGrid).SelectedItem as Student;
            MessageBox.Show(student.Name, "");
        }
    }
}
