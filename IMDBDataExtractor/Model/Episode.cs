﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBDataExtractor.Model
{
    public class TVSeriesEpisode : CreativeWork
    {
        public int Season { get; set; }
        public int Episode { get; set; }
        public string seriesID { get; set; }
    }
}
