using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Xml;

namespace Twitter {

    public partial class frmTwitter:Form {

        private GoogleDrive google_drive;
        private Twitter twitter;

        private List<Tweet> tweets = new List<Tweet>();


        public frmTwitter() {
            InitializeComponent();
            System.Net.ServicePointManager.Expect100Continue = false;

            google_drive = new GoogleDrive();
            twitter = new Twitter();
            tweets = twitter.SearchHashtags(pnlTweets, txtUsername.Text);

            /*
            GoogleDrive drive = new GoogleDrive();
            List<Google.Apis.Drive.v2.Data.File> files = drive.GetFileList();
            foreach (Google.Apis.Drive.v2.Data.File file in files) {
                txtBox.AppendText(file.Title + "\n");
            }
            */
        }

        


        private void Search(object sender,EventArgs e) {
            string username = txtUsername.Text;

            pnlTweets.Controls.Clear();
            Label lblLoading = new Label();
            lblLoading.Text = "Loading...";
            lblLoading.Font = new Font(this.Font.FontFamily, 20.0f, FontStyle.Regular);
            lblLoading.TextAlign = ContentAlignment.MiddleCenter;
            lblLoading.Size = new Size(pnlTweets.Width - 40, 40);
            lblLoading.Location = new Point(25, 0);
            pnlTweets.Controls.Add(lblLoading);

            if (!username.Substring(0,1).Equals("#")) {
                tweets = twitter.GetFeed(pnlTweets, username);
            } else {
                tweets = twitter.SearchHashtags(pnlTweets, username);
            }

            if (tweets.Count > 0) {
                msSave.Enabled = true;
            } else {
                MessageBox.Show("No tweets were found", "No Tweets", MessageBoxButtons.OK, MessageBoxIcon.Error);
                msSave.Enabled = false;
            }
        }

        private void BackupTweets(object sender,EventArgs e) {
            string file = @"C:\TempTwitterBackup.txt";
            if (tweets.Count > 0) {
                msSave.Enabled = false;
                foreach (Tweet tweet in tweets) {
                    System.IO.File.AppendAllText(file, tweet.ToString());
                }

                google_drive.Upload(file);
            }
        }

        private void Exit(object sender,EventArgs e) {
            this.Close();
        }

    }

}
