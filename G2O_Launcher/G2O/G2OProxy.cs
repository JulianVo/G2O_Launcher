//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="G2OProxy.cs" company="Gothic Online Project">
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
namespace G2O_Launcher.G2O
{
    #region

    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    #endregion

    /// <summary>
    ///     Provides methods for starting the gothic 2 online multiplayer.
    /// </summary>
    public class G2OProxy : IDisposable
    {
        /// <summary>
        ///     Proxy dll module handle.
        /// </summary>
        private readonly IntPtr proxyDll;

        /// <summary>
        ///     Delegate to the native run function.
        /// </summary>
        private readonly RunFunctionDelegate runFunctionDelegateFunction;

        /// <summary>
        ///     Delegate to the native version function.
        /// </summary>
        private readonly VersionFunctionDelegate versionFunctionDelegateFunction;

        /// <summary>
        ///     Indicates whether this object is disposed or not.
        /// </summary>
        private bool disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="G2OProxy" /> class.
        /// </summary>
        public G2OProxy()
        {
            this.proxyDll = LoadLibrary("G2O_Proxy.dll");
            if (this.proxyDll == IntPtr.Zero)
            {
                throw new FileNotFoundException(
                    @"The gotic 2 online starter proxy could not be found", 
                    Path.Combine(Environment.CurrentDirectory, "G2O_Proxy.dll"));
            }

            IntPtr runPtr = GetProcAddress(this.proxyDll, "G2O_Run");
            this.runFunctionDelegateFunction =
                (RunFunctionDelegate)Marshal.GetDelegateForFunctionPointer(runPtr, typeof(RunFunctionDelegate));

            IntPtr versionPtr = GetProcAddress(this.proxyDll, "G2O_Version");
            this.versionFunctionDelegateFunction =
                (VersionFunctionDelegate)
                Marshal.GetDelegateForFunctionPointer(versionPtr, typeof(VersionFunctionDelegate));
        }

        /// <summary>
        ///     Delegate for the native run function.
        /// </summary>
        /// <param name="major">The major version number.</param>
        /// <param name="minor">The minor version number</param>
        /// <param name="patch">The patch number.</param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int RunFunctionDelegate(int major, int minor, int patch);

        /// <summary>
        ///     Delegate for the native version function
        /// </summary>
        /// <param name="major">The major version number.</param>
        /// <param name="minor">The minor version number</param>
        /// <param name="patch">The patch number.</param>
        /// <param name="build">The build number.</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void VersionFunctionDelegate(ref int major, ref int minor, ref int patch, ref int build);

        /// <summary>
        ///     Defines the possible results of the run method.
        /// </summary>
        public enum RunResult
        {
            /// <summary>
            ///     G2O was startet successfully.
            /// </summary>
            Success = 0, 

            /// <summary>
            ///     Wrong version was detected.
            /// </summary>
            WrongVersion = 1, 

            /// <summary>
            ///     Gothic2.exe was not found.
            /// </summary>
            GothicNotFound = 2, 

            /// <summary>
            ///     Unknow error.
            /// </summary>
            Unknown = 3, 
        }

        /// <summary>
        ///     Releases all unmanaged resources associated with this object.
        /// </summary>
        public void Dispose()
        {
            if (!this.disposed && this.proxyDll != IntPtr.Zero)
            {
                FreeLibrary(this.proxyDll);
                this.disposed = true;
            }
        }

        /// <summary>
        ///     Runs a specific version gothic 2 multiplayer.
        /// </summary>
        /// <param name="major">The major version number.</param>
        /// <param name="minor">The minor version number</param>
        /// <param name="patch">The patch number.</param>
        public RunResult Run(int major, int minor, int patch)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(G2OProxy));
            }

            return (RunResult)this.runFunctionDelegateFunction(major, minor, patch);
        }

        /// <summary>
        ///     Gets the local version of the gothic 2 online multiplayer.
        /// </summary>
        /// <returns></returns>
        public G2OVersion Version()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(G2OProxy));
            }

            int major = 0;
            int minor = 0;
            int patch = 0;
            int update = 0;

            this.versionFunctionDelegateFunction?.Invoke(ref major, ref minor, ref patch, ref update);
            return new G2OVersion(major, minor, patch, update);
        }

        /// <summary>
        ///     Releases a loaded library.
        /// </summary>
        /// <param name="hModule">The module handle of the library.</param>
        /// <returns>True if successfull.</returns>
        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hModule);

        /// <summary>
        ///     Gets the pointer to a function entry point inside a loaded module.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="procedureName">Function name.</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        /// <summary>
        ///     Loads a native library.
        /// </summary>
        /// <param name="dllToLoad">Name of the dll thats should be loaded.</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllToLoad);     
    }
}