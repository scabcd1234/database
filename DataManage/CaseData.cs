using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManage
{
    public class caseData
    {
        public int FlaseId { get; set; }
        public int Id { get; set; }
        public string Phase { get; set; }
        public double Phase_ratio { get; set; }
        public double Temperature { get; set; }
        public string Diff_plane { get; set; }
        public double Ehkl { get; set; }
        public double Vhkl { get; set; }
        public double Distance { get; set; }
    }
}
