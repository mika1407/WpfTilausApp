﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfTilausApp
{
    class Tuote
    {
        public string TuoteID { get; set; }
        public string Nimi { get; set; }
        public string Ahinta { get; set; }
        public byte[] Kuva { get; set; }
    }
}
