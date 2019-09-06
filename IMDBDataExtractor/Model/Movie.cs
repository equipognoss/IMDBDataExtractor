using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBDataExtractor.Model
{
   

    public class Movie : CreativeWork
    {
        public string Production { get; set; }
        public string DVD { get; set; }
    }
}
