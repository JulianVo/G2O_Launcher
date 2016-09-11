// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="UpdateErrorEventArgs.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace Updater
{
    #region

    using System;

    #endregion

    public class UpdateErrorEventArgs : EventArgs
    {
        public UpdateErrorEventArgs(string errorMessage, ErrorCode code)
        {
            if (errorMessage == null)
            {
                throw new ArgumentNullException(nameof(errorMessage));
            }

            this.ErrorMessage = errorMessage;
            this.Code = code;
        }

        public string ErrorMessage { get; }

        public ErrorCode Code { get; }
    }
}