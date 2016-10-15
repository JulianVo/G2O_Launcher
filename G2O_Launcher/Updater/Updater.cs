//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="Update.cs" company="Gothic Online Project">
//  Copyright (C) <2016>  <Julian Vogel>
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//  -
//  This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
// -
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http:www.gnu.org/licenses/>.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------
namespace G2O_Launcher.Updater
{
    #region

    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Net;

    using ICSharpCode.SharpZipLib.Core;
    using ICSharpCode.SharpZipLib.Zip;

    using Newtonsoft.Json;

    #endregion

    internal class Updater
    {
        private string downloadLink;

        private const string UpdateUri = "http://gothic-online.com.pl/version/update.php";

        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgress;

        public event EventHandler<EventArgs> DownloadStarted;

        public event EventHandler<UpdateErrorEventArgs> ErrorOccured;

        public event EventHandler<EventArgs> AvailableUpdateDetected;

        public event EventHandler<EventArgs> UpdateCompleted;

        public void Check(int major, int minor, int patch, int build)
        {
            using (WebClient webClient = new WebClient())
            {
                string uriString = UpdateUri;
                webClient.UploadStringCompleted += this.UpdateUploadStringCompleted;
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

        private void UpdateDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.DownloadProgress?.Invoke(this, e);
        }

        private void UpdateUploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
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
                    this.AvailableUpdateDetected?.Invoke(this, new EventArgs());
                    UpdateResponse response = JsonConvert.DeserializeObject<UpdateResponse>(e.Result);
                    this.downloadLink = response.Link;
                }
            }
            catch (JsonReaderException)
            {
                this.ErrorOccured?.Invoke(
                    this,
                    new UpdateErrorEventArgs(
                        $"Could not parse the data that was returned from the update server.",
                        ErrorCode.ParsingUpdateResponseFailed));
            }
        }

        public void Update()
        {
            if (string.IsNullOrEmpty(this.downloadLink))
            {
                throw new NotSupportedException("No update available");
            }
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadProgressChanged += this.UpdateDownloadProgressChanged;
                webClient.DownloadFileCompleted += this.UpdateDownloadFileCompleted;
                try
                {
                    webClient.DownloadFileAsync(new Uri(this.downloadLink), "update.zip");
                    this.DownloadStarted?.Invoke(this,new EventArgs());
                }
                catch (WebException ex)
                {
                    this.ErrorOccured?.Invoke(this, new UpdateErrorEventArgs(ex.Message, ErrorCode.UpdateProcessCouldNotBeStarted));
                }
                catch (UriFormatException ex)
                {
                    this.ErrorOccured?.Invoke(this, new UpdateErrorEventArgs(ex.Message, ErrorCode.UpdateProcessCouldNotBeStarted));
                }
            }
        }

        private void UpdateDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
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
                        new UpdateErrorEventArgs($"G2O_Update.exe didn't exist!", ErrorCode.UpdateProcessNotFound));
                }
            }
            else
            {
                this.UpdateCompleted?.Invoke(this,new EventArgs());
            }
        }



        private void Extract(string archiveFilename, string path)
        {
            if (string.IsNullOrEmpty(archiveFilename))
            {
                throw new ArgumentException(@"Value cannot be null or empty.", nameof(archiveFilename));
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(@"Value cannot be null or empty.", nameof(path));
            }
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
                            if (directoryName.Length > 0)
                            {
                                Directory.CreateDirectory(directoryName);
                            }
                            using (FileStream fileStream = File.Create(path1))
                            {
                                StreamUtils.Copy((Stream)stream, (Stream)fileStream, buffer);
                            }
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