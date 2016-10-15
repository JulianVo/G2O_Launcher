//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="Updater.cs" company="Gothic Online Project">
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
//  -------------------------------------------------------------------------------
namespace G2O_Launcher.Updater
{
    #region

    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Net;

    using Newtonsoft.Json;

    #endregion

    /// <summary>
    /// Encapsulates the updating process.
    /// </summary>
    internal class Updater
    {
        /// <summary>
        /// Path tho the gothic online update server.
        /// </summary>
        private const string UpdateUri = "http://gothic-online.com.pl/version/update.php";

        /// <summary>
        /// Stores the downloadlink for the next update.
        /// </summary>
        private string downloadLink;

        /// <summary>
        /// Calls all registered handlers if a available update was found by the <see cref="Check"/> method.
        /// </summary>
        public event EventHandler<EventArgs> AvailableUpdateDetected;

        /// <summary>
        /// Calls all registered handlers if progress on the current download is made.
        /// </summary>
        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgress;

        /// <summary>
        /// Calls all registered handlers if a download is started.
        /// </summary>
        public event EventHandler<EventArgs> DownloadStarted;

        /// <summary>
        /// Calls all registered handlers if a error occurs(used by the async methods)
        /// </summary>
        public event EventHandler<UpdateErrorEventArgs> ErrorOccured;

        /// <summary>
        /// Calls all registered handlers when the update is completed.
        /// </summary>
        public event EventHandler<EventArgs> UpdateCompleted;

        /// <summary>
        /// Starts a asynchronous server request to find out if the is a newer version that the currently used one.
        /// </summary>
        /// <param name="major">The current major version</param>
        /// <param name="minor">The current minor version</param>
        /// <param name="patch">The current patch number</param>
        /// <param name="build">THe current build number</param>
        public void Check(int major, int minor, int patch, int build)
        {
            using (WebClient webClient = new WebClient())
            {
                string uriString = UpdateUri;
                webClient.UploadStringCompleted += this.UpdateUploadStringCompleted;
                try
                {
                    var post = new UpdatePost() { major = minor, minor = minor, patch = patch, build = build };
                    string data = JsonConvert.SerializeObject(post);
                    webClient.UploadStringAsync(new Uri(uriString), "POST", data);
                }
                catch (WebException ex)
                {
                    this.ErrorOccured?.Invoke(this, new UpdateErrorEventArgs(ex.Message));
                }
            }
        }

        /// <summary>
        /// Starts the download and processing of a available download if one was detected by the <see cref="Check"/> method.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown if no update was found or the <see cref="Check"/> method was not called.</exception>
        public void Update()
        {
            if (string.IsNullOrEmpty(this.downloadLink))
            {
                throw new NotSupportedException("No update link available(call the check method first.)");
            }

            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadProgressChanged += this.UpdateDownloadProgressChanged;
                webClient.DownloadFileCompleted += this.UpdateDownloadFileCompleted;
                try
                {
                    webClient.DownloadFileAsync(new Uri(this.downloadLink), "update.zip");
                    this.DownloadStarted?.Invoke(this, new EventArgs());
                }
                catch (WebException ex)
                {
                    this.ErrorOccured?.Invoke(this, new UpdateErrorEventArgs(ex.Message));
                }
                catch (UriFormatException ex)
                {
                    this.ErrorOccured?.Invoke(this, new UpdateErrorEventArgs(ex.Message));
                }
            }
        }

        /// <summary>
        /// Is called when download of a file has completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // Extracts the update.zip file to the directory that the launcher assembly is in.
            new ZipExtractor().Extract("update.zip", Path.GetDirectoryName(this.GetType().Assembly.Location));

            // The launcher assembly has to be replaces(requires a restart).
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
                        var args = new UpdateErrorEventArgs($"Cannot start update process!");
                        this.ErrorOccured?.Invoke(this, args);
                    }
                }
                catch (Win32Exception)
                {
                    var args = new UpdateErrorEventArgs($"G2O_Update.exe didn't exist!");
                    this.ErrorOccured?.Invoke(this, args);
                }
            }

            // Update complete no restart required.
            else
            {
                this.UpdateCompleted?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Is called when the progress of a download changes.
        /// </summary>
        /// <param name="sender">The downloader that made the progress.</param>
        /// <param name="e"><see cref="DownloadProgressChangedEventArgs"/></param>
        private void UpdateDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.DownloadProgress?.Invoke(this, e);
        }

        /// <summary>
        /// Is calles when the request for information about a available update has been completed.
        /// </summary>
        /// <param name="sender">The WebClient that completed the operation.</param>
        /// <param name="e"><see cref="UploadStringCompletedEventArgs"/></param>
        private void UpdateUploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    var args = new UpdateErrorEventArgs($"Could not reach the update server..");
                    this.ErrorOccured?.Invoke(this, args);
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
                    new UpdateErrorEventArgs($"Could not parse the data that was returned from the update server."));
            }
        }
    }
}