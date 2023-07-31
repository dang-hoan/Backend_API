using System.ComponentModel;

namespace Application.Enums
{
    public enum UploadType : byte
    {
        [Description(@"Images/ProfilePictures")]
        ProfilePicture,
        [Description(@"Videos/UploadVideos")]
        UploadVideo
    }
}