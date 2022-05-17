﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManage
{
    internal class CaseData
    {
        public int Id { get; set; }
        public string Phase { get; set; }
        public int Phase_ratio { get; set; }
        public int Temperature { get; set; }
        public string Diff_plane { get; set; }
        public int Ehkl { get; set; }
        public double Vhkl { get; set; }
        public double Distance { get; set; }
    }
}