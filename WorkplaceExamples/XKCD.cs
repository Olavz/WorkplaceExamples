using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WorkplaceExamples
{
    class XKCD
    {

        public static string[] getRandomXKCD()
        {
            string[] response = null;
            using (WebClient client = new WebClient())
            {
                string s = client.DownloadString("http://c.xkcd.com/random/comic/");
                Match mc = Regex.Match(s, "http://imgs.xkcd.com/comics/(.*?).png");
                Match mc2 = Regex.Match(s, "<div id=\"ctitle\">(.+?)</div>");
                if (mc2.Success)
                {
                    response = new string[2] { mc2.Groups[1].Value, mc.Value };
                }
                else
                {
                    Console.WriteLine("Well shit");
                }

            }

            return response;
        }

    }
}