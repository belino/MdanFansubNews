using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MDANBack
{
    
     public sealed class RSSItem
    {
        private string Description;

        public string Description1
        {
            get { return Description; }
            set { Description = value; }
        }

        private string Link;

        public string Link1
        {
            get { return Link; }
            set { Link = value; }
        }

        private string Title;

        public string Title1
        {
            get { return Title; }
            set { Title = value; }
        }

        private string pubDate;

        public string pubDate1
        {
            get { return pubDate; }
            set { pubDate = Regex.Replace(value, "<(img)\b[^>]*>", string.Empty); }
        }

        private string Image;
        public string Image1
        {
            get { return Image; }
            set { Image = value; }
        }
    }

}
