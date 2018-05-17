using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace MDAN.Base
{
    public class ImageDecoder
    {
        public List<string> GetImagesInHTMLString(string htmlString)
        {
            MatchCollection matches;
            var images = new List<string>();
            var patterns = new string[] { @"http://i.imgur\b[^>]*>",
                @"http://imgur\b[^>]*>",
                @"https://imgur\b[^>]*>",
                @"https://s^(([0-9]*)|(([0-9]*)\.([0-9]*)))$\b[^>]*>",
                @"https://i.imgur\b[^>]*>" };
            foreach (var p in patterns)
            {
                var rgx = new Regex(p, RegexOptions.IgnoreCase);
                matches = rgx.Matches(htmlString);
                if (matches.Count > 0)
                {
                    for (int i = 0, l = matches.Count; i < l; i++)
                    {
                        var fixedString2 = Regex.Replace(matches[i].Value, "<img ", string.Empty);
                        fixedString2 = Regex.Replace(fixedString2, "\\ class=\"", string.Empty);
                        var index = fixedString2.LastIndexOf("border=", StringComparison.Ordinal);
                        if (index > 0)
                            fixedString2 = fixedString2.Substring(0, index);
                        fixedString2 = Regex.Replace(fixedString2, "\\ border=\"", string.Empty);
                        fixedString2 = Regex.Replace(fixedString2, "src=\"", string.Empty);
                        fixedString2 = Regex.Replace(fixedString2, "\\ alt=\"", string.Empty);
                        fixedString2 = Regex.Replace(fixedString2, "\"", string.Empty);
                        fixedString2 = Regex.Replace(fixedString2, "/>", string.Empty);
                        fixedString2 = WebUtility.HtmlDecode(fixedString2).Trim();
                        images.Add(fixedString2);
                    }
                }
            }
            return images;
        }
    }
}
