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
    using System.Reflection;
    using System.Text;

    using G2O_Launcher.G2O;

    using Newtonsoft.Json;

    #endregion

    /// <summary>
    ///     Encapsulates the updating process.
    /// </summary>
    internal class Updater
    {
        /// <summary>
        ///     Path tho the gothic online update server.
        /// </summary>
        private const string UpdateUri = "http://gothic-online.com.pl/version/update.php";

        /// <summary>
        ///     Stores the downloadlink for the next update.
        /// </summary>
        private string downloadLink;

        /// <summary>
        ///     Calls all registered handlers if a available update was found by the <see cref="Check" /> method.
        /// </summary>
        public event EventHandler<EventArgs> AvailableUpdateDetected;

        /// <summary>
        ///     Calls all registered handlers if progress on the current download is made.
        /// </summary>
        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgress;

        /// <summary>
        ///     Calls all registered handlers if a download is started.
        /// </summary>
        public event EventHandler<EventArgs> DownloadStarted;

        /// <summary>
        ///     Calls all registered handlers if a error occurs(used by the async methods)
        /// </summary>
        public event EventHandler<UpdateErrorEventArgs> ErrorOccured;

        /// <summary>
        ///     Calls all registered handlers when the update is completed.
        /// </summary>
        public event EventHandler<EventArgs> UpdateCompleted;

        /// <summary>
        ///     Starts a asynchronous server request to find out if the is a newer version that the currently used one.
        /// </summary>
        public void Check()
        {
            G2OProxy.G2OVersion version;
            using (G2OProxy proxy = new G2OProxy())
            {
                version = proxy.Version();
            }

            using (WebClient webClient = new WebClient())
            {
                string uriString = UpdateUri;
                webClient.UploadStringCompleted += this.UpdateUploadStringCompleted;
                try
                {
                    var post = new UpdatePost()
                                   {
                                       major = version.Major, 
                                       minor = version.Minor, 
                                       patch = version.Patch, 
                                       build = version.Build
                                   };
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
        ///     Does executes a update check but sets the current version to 0,0,0,0 which causes a redownload of the all updates.
        /// </summary>
        public void CheckReset()
        {
            G2OProxy.G2OVersion version = new G2OProxy.G2OVersion(0, 0, 0, 0);

            using (WebClient webClient = new WebClient())
            {
                string uriString = UpdateUri;
                webClient.UploadStringCompleted += this.UpdateUploadStringCompleted;
                try
                {
                    var post = new UpdatePost()
                                   {
                                       major = version.Major, 
                                       minor = version.Minor, 
                                       patch = version.Patch, 
                                       build = version.Build
                                   };
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
        ///     Starts the download and processing of a available download if one was detected by the <see cref="Check" /> method.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///     Thrown if no update was found or the <see cref="Check" /> method was not
        ///     called.
        /// </exception>
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
        ///     Is called when download of a file has completed.
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
                    StringBuilder batch = new StringBuilder();
                    batch.AppendLine($"taskkill /F /IM {Assembly.GetExecutingAssembly().GetName().Name}.exe");
                    string appPath = Assembly.GetExecutingAssembly().Location;

                    batch.AppendLine("timeout /t 1 /nobreak");
                    batch.AppendLine($"move /y \"{appPath}.update\" \"{appPath}\"");
                    batch.AppendLine($"\"{appPath}\"");
                    batch.AppendLine("del /F \"%~f0\"");
                    batch.AppendLine("exit");

                    string batchPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".bat");
                    File.WriteAllText(batchPath, batch.ToString());

                    ProcessStartInfo processStart = new ProcessStartInfo();
                    processStart.WindowStyle = ProcessWindowStyle.Hidden;
                    processStart.FileName = batchPath;
                    processStart.UseShellExecute = true;
                    processStart.RedirectStandardOutput = false;
                    processStart.RedirectStandardError = false;
                    Process.Start(processStart);

                    //// Kill the G2O_Update process if it already exists.
                    // foreach (var process in Process.GetProcessesByName("G2O_Update.exe"))
                    // {
                    // process.Kill();
                    // }

                    // if (Process.Start("G2O_Update.exe") != null)
                    // {
                    // Environment.Exit(0);
                    // }
                    // else
                    // {
                    // var args = new UpdateErrorEventArgs($"Cannot start update process!");
                    // this.ErrorOccured?.Invoke(this, args);
                    // }
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
        ///     Is called when the progress of a download changes.
        /// </summary>
        /// <param name="sender">The downloader that made the progress.</param>
        /// <param name="e">
        ///     <see cref="DownloadProgressChangedEventArgs" />
        /// </param>
        private void UpdateDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.DownloadProgress?.Invoke(this, e);
        }

        /// <summary>
        ///     Is calles when the request for information about a available update has been completed.
        /// </summary>
        /// <param name="sender">The WebClient that completed the operation.</param>
        /// <param name="e">
        ///     <see cref="UploadStringCompletedEventArgs" />
        /// </param>
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
                    UpdateResponse response = JsonConvert.DeserializeObject<UpdateResponse>(e.Result);
                    this.downloadLink = response.Link;
                    if (!string.IsNullOrEmpty(this.downloadLink))
                    {
                        this.AvailableUpdateDetected?.Invoke(this, new EventArgs());
                    }
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