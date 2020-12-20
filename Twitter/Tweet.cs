using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitter {

    public class Tweet {
        
        private long id;
        private long user_id;
        private string username;
        private string fullname;
        private string text;
        private Bitmap avatar;

        public Tweet(long id,long user_id,string username,string fullname,string text,string avatar) {
            this.id = id;
            this.user_id = user_id;
            this.username = username;
            this.fullname = fullname;
            this.text = text;

            System.Net.WebRequest request = System.Net.WebRequest.Create(avatar);
            System.Net.WebResponse response = request.GetResponse();
            System.IO.Stream responseStream = response.GetResponseStream();
            this.avatar = new Bitmap(responseStream);
        }


        #region Getters
        public long GetID() {
            return (id);
        }

        public long GetUserID() {
            return (user_id);
        }

        public string GetUsername() {
            return (username);
        }

        public string GetFullname() {
            return (fullname);
        }

        public string GetText() {
            return (text);
        }

        public Bitmap GetAvatar() {
            return (avatar);
        }
        #endregion


        override
        public String ToString() {
            return ("Name: " + username + " (" + fullname + ")\n" + text + "\n\n");
        }
    }

}