﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfTilausApp
{
    class cbPairTuote
    {
        internal object aHinta;

        public string tuoteNimi { get; set; }
        public int tuoteNro { get; set; }

        public cbPairTuote(string TuoteNimi, int TuoteNro)
        {
            tuoteNimi = TuoteNimi;
            tuoteNro = TuoteNro;
        }
    }
}
