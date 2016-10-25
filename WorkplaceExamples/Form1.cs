using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkplaceExamples
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void tmrCheckFeed_Tick(object sender, EventArgs e)
        {
            checkFeed();
        }

        private Int32 lastTimestampFetched = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        private void checkFeed()
        {
            string URI = "https://graph.facebook.com/"+ txtCommunityId.Text +"/feed";

            using (WebClient wc = new WebClient())
            {
                wc.QueryString.Add("access_token", txtAccessToken.Text);
                wc.QueryString.Add("fields", "message,message_tags,from,comments");
                wc.QueryString.Add("since", "" + lastTimestampFetched);
                string results = wc.DownloadString(URI);
                txtOutput.AppendText(results + Environment.NewLine + Environment.NewLine);
                dynamic dResults = JsonConvert.DeserializeObject(results);
                foreach (dynamic msg in dResults.data)
                {

                    if (msg.comments != null && msg.comments.data.Count > 0)
                    {
                        postMessage("" + msg.id, "" + msg.comments.data[msg.comments.data.Count - 1].message);
                    }
                    else
                    {
                        postMessage("" + msg.id, "" + msg.message);
                    }

                }

            }

            lastTimestampFetched = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }


        private void postMessage(String id, String message)
        {
            string URI = "https://graph.facebook.com/" + id + "/comments";

            using (WebClient wc = new WebClient())
            {
                wc.QueryString.Add("access_token", txtAccessToken.Text);
                wc.QueryString.Add("message", messageCommand(message));
                string results = wc.UploadString(URI, wc.QueryString.ToString());
                txtOutput.AppendText(results + Environment.NewLine);
            }
        }

        private String messageCommand(String command)
        {
            if (command.StartsWith("!time"))
            {
                return "The time is " + DateTime.Now.ToString("HH:mm:ss") + ", why would you ask me?";
            }
            else if (command.StartsWith("What is your name?"))
            {
                return "I am a cat, who cares?";
            }
            else if (command.ToLower().Contains("xkcd"))
            {
                // Get and post XKCD.
                string[] response = XKCD.getRandomXKCD();
                postMessageWithLink(response[0], response[1]);
                return "As you wish! XKCD posted.";
            }

            return "Here you go! http://lmgtfy.com/?q=" + Uri.EscapeDataString(command.Replace(" ", "%20"));
        }


        private void postMessageWithLink(string message, string url)
        {
            string URI = "https://graph.facebook.com/1039824986073080/feed";

            using (WebClient wc = new WebClient())
            {
                wc.QueryString.Add("access_token", txtAccessToken.Text);
                wc.QueryString.Add("message", message);
                wc.QueryString.Add("link", url);
                string results = wc.UploadString(URI, wc.QueryString.ToString());
                txtOutput.AppendText(results);
            }
        }

        private void btnStartTimer_Click(object sender, EventArgs e)
        {
            if(tmrCheckFeed.Enabled)
            {
                tmrCheckFeed.Enabled = false;
                btnStartTimer.Text = "Start";
            }
            else
            {
                tmrCheckFeed.Enabled = true;
                btnStartTimer.Text = "Stop";
            }
        }
    }

}
