using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Twitter {

    public class Twitter {

        private string consumer_key = "lreL6VivPsgv652WCkrCnv7sD";
        private string consumer_secret = "YiR7L3hLAHfPIr15oBGbsKVzR8celhP1PMzDfl1dq9CncNoBa8";

        private string access_token = "966537006-ZcclGS8SkENkLepUZw5ia2ChchPK3fArq68bZUhK";
        private string access_token_secret = "MeeD9HBM2klaWQsWyIrPxUacy82u8kwxcYq2rAf4P7oAn";

        private string oauth_version = "1.0";
        private string oauth_signature_method = "HMAC-SHA1";


        public Twitter() {}


        /// <summary>
        /// Pulls in a specified users feed and displays them to the UI
        /// </summary>
        /// <param name="screen_name"></param>
        public List<Tweet> GetFeed(Control pnlTweets, string screen_name) {
            List<Tweet> tweets = new List<Tweet>();

            string oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            string oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

            string resource_url = "https://api.twitter.com/1.1/statuses/user_timeline.json";
            string baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" + "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&screen_name={6}";

            string baseString = string.Format(
                baseFormat,
                consumer_key,
                oauth_nonce,
                oauth_signature_method,
                oauth_timestamp,
                access_token,
                oauth_version,
                Uri.EscapeDataString(screen_name)
            );
            baseString = string.Concat("GET&", Uri.EscapeDataString(resource_url), "&", Uri.EscapeDataString(baseString));

            string compositeKey = string.Concat(Uri.EscapeDataString(consumer_secret), "&", Uri.EscapeDataString(access_token_secret));
            string oauth_signature;
            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(compositeKey))) {
                oauth_signature = Convert.ToBase64String(hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)));
            }

            string headerFormat = "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
                "oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
                "oauth_token=\"{4}\", oauth_signature=\"{5}\", " +
                "oauth_version=\"{6}\"";

            string authHeader = string.Format(headerFormat,
                Uri.EscapeDataString(oauth_nonce),
                Uri.EscapeDataString(oauth_signature_method),
                Uri.EscapeDataString(oauth_timestamp),
                Uri.EscapeDataString(consumer_key),
                Uri.EscapeDataString(access_token),
                Uri.EscapeDataString(oauth_signature),
                Uri.EscapeDataString(oauth_version)
            );

            ServicePointManager.Expect100Continue = false;

            string postBody = "screen_name=" + Uri.EscapeDataString(screen_name);
            resource_url += "?" + postBody;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resource_url);
            request.Headers.Add("Authorization", authHeader);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";

            WebResponse response = request.GetResponse();
            string responseData = new StreamReader(response.GetResponseStream()).ReadToEnd();
                
            JArray arr = JArray.Parse(responseData);
            for (int i = 0; i < arr.Count; i++) {
                JObject obj = JObject.Parse(arr[i].ToString());
                    
                long id = long.Parse(obj["id"].ToString());
                string text = obj["text"].ToString();

                JObject user = JObject.Parse(obj["user"].ToString());
                long user_id = long.Parse(user["id"].ToString());
                string username = user["screen_name"].ToString();
                string fullname = user["name"].ToString();
                string avatar = user["profile_image_url"].ToString();

                tweets.Add(new Tweet(id,user_id,username,fullname,text,avatar));
            }

            DisplayTweets(pnlTweets, screen_name, tweets);

            return (tweets);
        }

        /// <summary>
        /// Pulls in a list of statuses containing a specified hashtag and displays them to the UI
        /// </summary>
        /// <param name="hashtag"></param>
        public List<Tweet> SearchHashtags(Control pnlTweets, string hashtag) {
            List<Tweet> tweets = new List<Tweet>();

            if (hashtag.Substring(0, 1).Equals("#")) {
                hashtag = hashtag.Substring(1);
            }

            string oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            string oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

            string resource_url = "https://api.twitter.com/1.1/search/tweets.json";
            string baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" + "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&q={6}";

            string baseString = string.Format(
                baseFormat,
                consumer_key,
                oauth_nonce,
                oauth_signature_method,
                oauth_timestamp,
                access_token,
                oauth_version,
                Uri.EscapeDataString("#" + hashtag)
            );
            baseString = string.Concat("GET&", Uri.EscapeDataString(resource_url), "&", Uri.EscapeDataString(baseString));

            string compositeKey = string.Concat(Uri.EscapeDataString(consumer_secret), "&", Uri.EscapeDataString(access_token_secret));
            string oauth_signature;
            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(compositeKey))) {
                oauth_signature = Convert.ToBase64String(hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)));
            }

            string headerFormat = "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
                "oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
                "oauth_token=\"{4}\", oauth_signature=\"{5}\", " +
                "oauth_version=\"{6}\"";

            string authHeader = string.Format(headerFormat,
                Uri.EscapeDataString(oauth_nonce),
                Uri.EscapeDataString(oauth_signature_method),
                Uri.EscapeDataString(oauth_timestamp),
                Uri.EscapeDataString(consumer_key),
                Uri.EscapeDataString(access_token),
                Uri.EscapeDataString(oauth_signature),
                Uri.EscapeDataString(oauth_version)
            );

            ServicePointManager.Expect100Continue = false;

            string postBody = "q=" + Uri.EscapeDataString("#" + hashtag);
            resource_url += "?" + postBody;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resource_url);
            request.Headers.Add("Authorization", authHeader);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";

            WebResponse response = request.GetResponse();
            string responseData = new StreamReader(response.GetResponseStream()).ReadToEnd();

            JObject root = JObject.Parse(responseData);
            JArray arr = JArray.Parse(root["statuses"].ToString());
            for (int i = 0; i < arr.Count; i++) {
                JObject obj = JObject.Parse(arr[i].ToString());
                    
                long id = long.Parse(obj["id"].ToString());
                string text = obj["text"].ToString();

                JObject user = JObject.Parse(obj["user"].ToString());
                long user_id = long.Parse(user["id"].ToString());
                string username = user["screen_name"].ToString();
                string fullname = user["name"].ToString();
                string avatar = user["profile_image_url"].ToString();

                tweets.Add(new Tweet(id,user_id,username,fullname,text,avatar));
            }

            DisplayTweets(pnlTweets, hashtag, tweets);

            return (tweets);
        }

        /// <summary>
        /// Displays a list of tweets to the UI
        /// </summary>
        /// <param name="tweets"></param>
        private void DisplayTweets(Control panel, string term, List<Tweet> tweets) {
            if (panel != null && panel.GetType() == typeof(Panel)) {
                Panel pnlTweets = (Panel) panel;

                pnlTweets.Controls.Clear();
                GetWeather(panel, term);

                int offset = 0;
                if (pnlTweets.Controls.Count > 0) {
                    foreach (Control c in pnlTweets.Controls) {
                        offset += c.Height;
                    }
                }

                Thread thread = new Thread(new ThreadStart(() => {
                    foreach (Tweet tweet in tweets) {
                        Panel pnlWrapper = new Panel();
                        pnlWrapper.Location = new Point(0,offset);
                        pnlWrapper.Width = pnlTweets.Width;
                        pnlWrapper.Anchor = (AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right);
                        pnlWrapper.BackColor = Color.Gray;

                        Panel pnlTweet = new Panel();
                        pnlTweet.Location = new Point(0,0);
                        pnlTweet.Width = pnlWrapper.Width;
                        pnlTweet.Anchor = (AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right);
                        pnlTweet.BackColor = Color.White;

                        PictureBox pbAvatar = new PictureBox();
                        pbAvatar.Cursor = Cursors.Hand;
                        pbAvatar.Location = new Point(5,5);
                        pbAvatar.Size = new Size(45,45);
                        pbAvatar.BackgroundImage = tweet.GetAvatar();
                        pbAvatar.BackgroundImageLayout = ImageLayout.Stretch;
                        pbAvatar.Anchor = (AnchorStyles.Top|AnchorStyles.Left);
                        pbAvatar.Click += (object sender,EventArgs e) => {
                            Process.Start("http://www.twitter.com/" + tweet.GetUsername());  
                        };

                        Label lblTitle = new Label();
                        lblTitle.Cursor = Cursors.Hand;
                        lblTitle.Text = tweet.GetFullname() + " - (" + tweet.GetUsername() + ")";
                        lblTitle.Font = new Font(new FontFamily("Microsoft Sans Serif"), 8.25f, FontStyle.Bold);
                        lblTitle.Location = new Point(pbAvatar.Width + 10,5);
                        lblTitle.AutoSize = true;
                        lblTitle.Anchor = (AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right);
                        lblTitle.Click += (object sender,EventArgs e) => {
                            Process.Start("http://www.twitter.com/" + tweet.GetUsername());  
                        };

                        Label lblMess = new Label();
                        lblMess.Text = tweet.GetText();
                        lblMess.Location = new Point(pbAvatar.Width + 10,lblTitle.Height - 5);
                        lblMess.Size = new Size(pnlTweet.Width - pbAvatar.Width - 40,45);
                        lblMess.MaximumSize = new Size(pnlTweet.Width - pbAvatar.Width - 40,5000);
                        lblMess.Anchor = (AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right);
                        lblMess.AutoSize = true;

                        int height = lblTitle.Height + lblMess.Height + 15;
                        if (height < pbAvatar.Height + 10) {
                            height = pbAvatar.Height + 10;
                        }
                        pnlTweet.Height = height;
                        pnlWrapper.Height = pnlTweet.Height + 1;

                        pnlTweet.Controls.Add(pbAvatar);
                        pnlTweet.Controls.Add(lblTitle);
                        pnlTweet.Controls.Add(lblMess);

                        pnlWrapper.Controls.Add(pnlTweet);

                        pnlTweets.Invoke((MethodInvoker) delegate() {
                            pnlTweets.Controls.Add(pnlWrapper);
                            pnlTweets.Focus();
                        });
                        offset += pnlWrapper.Height;
                    }
                }));
                thread.Start();
            }
        }

        /// <summary>
        /// Detects a city and finds its weather
        /// </summary>
        /// <param name="pnlTweets"></param>
        /// <param name="city"></param>
        private void GetWeather(Control pnlTweets, string city) {
            try {
                Weather.GlobalWeather weather_service = new Weather.GlobalWeather();
                string data = weather_service.GetCitiesByCountry("Ireland");

                XmlDocument document = new XmlDocument();
                document.LoadXml(data);

                XmlNodeList node_list = document.DocumentElement.SelectNodes("Table");
                foreach (XmlNode node in node_list) {
                    string node_country = node.SelectSingleNode("Country").InnerText;
                    string node_city = node.SelectSingleNode("City").InnerText;

                    if (node_city.ToUpper().Contains(city.ToUpper())) {
                        string temp_data = weather_service.GetWeather(node_city, node_country);
                        document.LoadXml(temp_data);

                        XmlNodeList temp_node_list = document.DocumentElement.SelectNodes("Temperature");
                        string temp = temp_node_list[0].InnerText;

                        RichTextBox txtInfo = new RichTextBox();
                        txtInfo.Size = new Size(pnlTweets.Width - 17, 25);
                        txtInfo.ReadOnly = true;
                        txtInfo.Font = new Font(new FontFamily("Microsoft Sans Serif"), 12.0f, FontStyle.Regular);
                        txtInfo.Text = "The weather in " + city + " is " + temp;
                        pnlTweets.Controls.Add(txtInfo);
                        break;
                    }
                }
            } catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }
    }

}