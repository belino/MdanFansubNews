using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


namespace MDAN_App_Base
{
    [DataContract]
    public class User
    {
        private static User _instance;
        public static User Instance => _instance ?? (_instance = new User());

        private User()
        {
            Cats = new List<int>();
        }

        public string Username { get; set; }

        public string Avatar { get; set; }

        private string _userId;

        public string Id
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public bool Status { get; set; }

        public string TrackerUri { get; set; }

        public List<int> Cats;
        public bool Notifications = true;

        public List<int> Catsreturn()
        {
            return Cats;
        }

        public void TrackerUriGenerator()
        {
            if (Cats.Count != 0)
            {
                var i = 0;
                //trackerRssUri = "http://bt.mdan.org/rss_app.php?cats=";
                var uribuilder = new StringBuilder();
                uribuilder.Append("http://bt.mdan.org/rss_app.php?cats=");
                while (i <= Cats.Count - 1)
                {
                    uribuilder.Append(Cats[i]);
                    if (i < Cats.Count - 1)
                    {
                        uribuilder.Append(",");
                    }
                    i++;
                }
                TrackerUri = uribuilder.ToString();
            }
            else
            {
                TrackerUri = "http://bt.mdan.org/rss_app.php";
            }
        }

        public async void Login(string username, string password)
        {
            await GetPostResponse(username, password);
        }

        private async Task<bool> GetPostResponse(string username, string password)
        {
            var req = WebRequest.Create("http://bt.mdan.org/login_json.php")
                as HttpWebRequest;
            if (req == null) return false;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            // Build a string with all the params, properly encoded.
            // We assume that the arrays paramName and paramVal are
            // of equal length:
            var paramz = new StringBuilder();
            paramz.Append("username=belino");
            paramz.Append(username);
            paramz.Append("&password=emscla");
            paramz.Append(password);

            using (var writer = new StreamWriter(await req.GetRequestStreamAsync()))
            {
                writer.Write(paramz);
            }

            using (var response = await req.GetResponseAsync())
            {
                var reader = new StreamReader(response.GetResponseStream());
                var x = reader.ReadToEnd();
                try
                {
                    var root = JArray.Parse(x);
                    
                    if (!int.TryParse(root[0].ToString(), out int k))
                    {
                        return false;
                    }
                    Id = root[0].ToString();
                    Username = root[1].ToString();
                    Avatar = root[2].ToString();
                    Status = true;
                    return true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    return false;
                }

            }
        }


    }
}
