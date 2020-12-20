using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Twitter {

    public class GoogleDrive {

        private UserCredential CREDENTIAL;
        private DriveService SERVICE;
        private string UPLOAD_ID;


        public GoogleDrive() {
            //Creates the users credentials
            CREDENTIAL = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets {
                    ClientId = "814618111083-bt950ug870ar2unhm29esuf2la2m4889.apps.googleusercontent.com",
                    ClientSecret = "0peAvHEnPUKsraQn0LnsJp30",
                }, 
                new[] { DriveService.Scope.Drive },
                "user",
                CancellationToken.None
            ).Result;

            // Create the service.
            SERVICE = new DriveService(new BaseClientService.Initializer() {
                HttpClientInitializer = CREDENTIAL,
                ApplicationName = "GoogleDriveApp",
            });

            UPLOAD_ID = "";
        }


        /// <summary>
        /// /Get a list of all Google Drive files
        /// </summary>
        /// <returns></returns>
        public List<File> GetFileList() {
            return (GetFileList(""));
        }

        /// <summary>
        /// /Get a list of all Google Drive files with a specific term in the title
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public List<File> GetFileList(string term) {
            List<File> files = new List<File>();

            FilesResource.ListRequest list_request = SERVICE.Files.List();
            if (term != null && !term.Equals("")) {
                list_request.Q = "title contains '" + term + "'";
            }

            do {
                try {
                    FileList file_list = list_request.Execute();

                    files.AddRange(file_list.Items);
                    list_request.PageToken = file_list.NextPageToken;
                } catch (Exception ex) {
                    Debug.WriteLine(ex.Message);
                }
            } while (!String.IsNullOrEmpty(list_request.PageToken));

            return (files);
        }

        /// <summary>
        /// Uploads a file to Google Drive
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Upload(string filepath) {
            if (System.IO.File.Exists(filepath)) {
                Thread thread = new Thread(new ThreadStart(() => {
                    Mime mime = new Mime();
                    DateTime now = DateTime.Now;

                    //Upload a file to Google Drive
                    File body = new File();
                    body.Title = "Twitter Backup - " + now.ToString();
                    body.MimeType = mime.GetMimeType(System.IO.Path.GetExtension(filepath));

                    //Data.txt is stored in the solution explorer and set to 'Copy to Output' in settings. ------------------------------------------------------->
                    System.IO.Stream stream = new System.IO.FileStream(filepath, System.IO.FileMode.Open);

                    FilesResource.InsertMediaUpload upload_request = SERVICE.Files.Insert(body,stream,body.MimeType);
                    upload_request.Upload();
                    stream.Close();

                    File upload_responce = upload_request.ResponseBody;
                    UPLOAD_ID = upload_responce.Id;

                    Debug.WriteLine("File uploaded successfully");
                    MessageBox.Show("Tweets were backed up successfully", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    System.IO.File.Delete(filepath);
                }));
                thread.Start();            
            }
        }

        /// <summary>
        /// Deletes the file that was created
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Delete() {
            if (!UPLOAD_ID.Equals("")) {
                Thread thread = new Thread(new ThreadStart(() => {
                    //Delete a file by its ID (the one we just created above)
                    FilesResource.DeleteRequest delete_request = SERVICE.Files.Delete(UPLOAD_ID);
                    delete_request.Execute();

                    Debug.WriteLine("File deleted successfully");
                }));
                thread.Start();
            }
        }
    }

}