﻿using System.Text.RegularExpressions;


namespace MDAN.Base
{

    public class RSSItem
    {
        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private string _link;
        public string Link
        {
            get { return _link; }
            set { _link = value; }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private string _pubDate;
        public string PubDate
        {
            get { return _pubDate; }
            set { _pubDate = Regex.Replace(value, "<(img)\b[^>]*>", string.Empty); }
        }

        private string _image;
        public string Image
        {
            get { return _image; }
            set { _image = value; }
        }

        private string _content;
        public string NewsContent
        {
            get { return _content; }
            set { _content = value; }
        }
    }

}

