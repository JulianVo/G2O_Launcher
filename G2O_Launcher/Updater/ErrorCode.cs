// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="ErrorCode.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace G2O_Launcher.Updater
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