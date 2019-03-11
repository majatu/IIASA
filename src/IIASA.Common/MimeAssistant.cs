using System.Collections.Generic;

namespace IIASA.Common
{
    public class MimeAssistant
    {
        public static readonly List<MimeItem> MimeTypesDictionary = new List<MimeItem>
        {
            new MimeItem(".gif", "image/gif", MimeType.Image),
            new MimeItem(".jpg", "image/jpeg", MimeType.Image),
            new MimeItem(".jpg",  "pjpeg", MimeType.Image),
            new MimeItem(".jpeg", "image/jpeg", MimeType.Image),
            new MimeItem(".jpeg", "pjpeg", MimeType.Image),
            new MimeItem(".png", "image/png", MimeType.Image),
            new MimeItem(".bmp", "image/bmp", MimeType.Image),
            new MimeItem(".tiff", "image/tiff", MimeType.Image),
            new MimeItem(".tif", "image/tiff", MimeType.Image),
        };
    }

    public class MimeItem
    {
        public string Extension { get; set; }
        public string ContentType { get; set; }
        public MimeType Type { get; set; }

        public MimeItem(string extension, string contentType, MimeType type)
        {
            Extension = extension;
            ContentType = contentType;
            Type = type;
        }
    }

    public enum MimeType
    {
        Unknown,

        Image
    }

}
