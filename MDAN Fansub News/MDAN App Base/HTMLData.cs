using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDAN.Base
{
    public class HTMLData
    {
        public HTMLData() { }
        public HTMLData(string _Name, string _HTML)
        {
            Name = _Name;
            HTML = _HTML;
        }

        public string Name { get; set; }
        public string HTML { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
