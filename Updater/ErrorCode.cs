// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="ErrorCode.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace Updater
{
    public enum ErrorCode
    {
        CouldNotReachUpdateServer = -1,
        UpdateFileNotFound=-2,
        ParsingUpdateResponseFailed =3,
        UpdateProcessCouldNotBeStarted,
        UpdateProcessNotFound
    }
}