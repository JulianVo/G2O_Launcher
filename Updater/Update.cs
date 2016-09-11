// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="Update.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace Updater
{
    #region

    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Windows;

    using ICSharpCode.SharpZipLib.Core;
    using ICSharpCode.SharpZipLib.Zip;

    using Newtonsoft.Json;

    #endregion

    internal class Update
    {
        private const string UpdateUri = "http://gothic-online.com.pl/version/update.php";

        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgress;

        public event EventHandler<EventArgs> DownloadStarted;

        public event EventHandler<UpdateErrorEventArgs> ErrorOccured;

        public void Check(int major, int minor, int patch, int build)
        {
            using (WebClient webClient = new WebClient())
            {
                string uriString = UpdateUri;
                webClient.UploadStringCompleted += this.Update_UploadStringCompleted;
                try
                {
                    string data =
                        JsonConvert.SerializeObject(
                            (object)new UpdatePost() { major = minor, minor = minor, patch = patch, build = build });
                    webClient.UploadStringAsync(new Uri(uriString), "POST", data);
                }
                catch (WebException ex)
                {
                }
            }
        }

        public void UpdateDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.Extract("update.zip", string.Empty);
            if (File.Exists("G2O_Launcher.exe.update"))
            {
                try
                {
                    if (Process.Start("G2O_Update.exe") != null)
                    {
                        Environment.Exit(0);
                    }
                    else
                    {
                        this.ErrorOccured?.Invoke(
                      this,
                      new UpdateErrorEventArgs(
                          $"Cannot start update process!",
                          ErrorCode.UpdateProcessCouldNotBeStarted));
                    }
                }
                catch (Win32Exception ex)
                {

                    this.ErrorOccured?.Invoke(
                    this,
                    new UpdateErrorEventArgs(
                      $"G2O_Update.exe didn't exist!",
                      ErrorCode.UpdateProcessNotFound));
                }
            }
            else
            {
                // MainWindow.Instance.UpdateVersion();
                // Update.check();
            }
        }

        public void Update_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.DownloadProgress?.Invoke(this, e);
        }

        public void Update_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    this.ErrorOccured?.Invoke(
                        this,
                        new UpdateErrorEventArgs(
                            $"Could not reach the update server..",
                            ErrorCode.CouldNotReachUpdateServer));
                }
                else
                {
                    UpdateResponse response = JsonConvert.DeserializeObject<UpdateResponse>(e.Result);
                    this.DownloadStarted?.Invoke(this, new EventArgs());
                    this.download(response.Link);
                }
            }
            catch (JsonReaderException ex)
            {
                this.ErrorOccured?.Invoke(
                    this,
                    new UpdateErrorEventArgs(
                        $"Could not parse the data that was returned from the update server.",
                        ErrorCode.ParsingUpdateResponseFailed));
            }
        }

        private void download(string link)
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadProgressChanged +=
                    new DownloadProgressChangedEventHandler(this.Update_DownloadProgressChanged);
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.UpdateDownloadFileCompleted);
                try
                {
                    webClient.DownloadFileAsync(new Uri(link), "update.zip");
                }
                catch (WebException ex)
                {
                }
                catch (UriFormatException ex)
                {
                    throw;
                }
            }
        }

        private void Extract(string archiveFilename, string path)
        {
            ZipFile zipFile = (ZipFile)null;
            try
            {
                zipFile = new ZipFile((FileStream)File.OpenRead(archiveFilename));
                IEnumerator enumerator = (IEnumerator)zipFile.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        ZipEntry entry = (ZipEntry)enumerator.Current;
                        if (entry.IsFile)
                        {
                            string name = entry.Name;
                            byte[] buffer = new byte[4096];
                            Stream stream = (Stream)zipFile.GetInputStream(entry);
                            string path1 = Path.Combine(path, name);
                            string directoryName = Path.GetDirectoryName(path1);
                            if (directoryName.Length > 0) Directory.CreateDirectory(directoryName);
                            using (FileStream fileStream = File.Create(path1)) StreamUtils.Copy((Stream)stream, (Stream)fileStream, buffer);
                        }
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    disposable?.Dispose();
                }

                // MainWindow.Instance.Message("Ready.");
            }
            catch (FileNotFoundException ex)
            {
                // MainWindow.Instance.Update_Result(new UpdateResponse() { Code = -2 });
                this.ErrorOccured?.Invoke(
                    this,
                    new UpdateErrorEventArgs($"The update file could not be found.", ErrorCode.UpdateFileNotFound));
            }
            finally
            {
                if (zipFile != null)
                {
                    zipFile.IsStreamOwner = true;
                    zipFile.Close();
                }
            }
        }
    }
}