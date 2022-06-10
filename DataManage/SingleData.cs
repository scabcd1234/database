using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManage
{
    public class SingleData
    {
        public int FlaseId { get; set; }
        public int Id { get; set; }
        public string Phase { get; set; }
        public double Temperature { get; set; }
        public double C11 { get; set; }
        public double C12 { get; set; }
        public double C13 { get; set; }
        public double C33 { get; set; }
        public double C44 { get; set; }
        public Boolean IsChecked { get; set; }
    }
}
