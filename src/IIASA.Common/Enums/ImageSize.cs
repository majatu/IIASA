using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IIASA.Common.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ImageSize
    {
        Unknown = 0,

        [ImageSizeExt("small", 128)]
        Small = 1,

        [ImageSizeExt("medium", 512)]
        Medium = 2,

        [ImageSizeExt("large", 2048)]
        Large = 3
    }

    public class ImageSizeExtAttribute : Attribute
    {
        internal ImageSizeExtAttribute(string folderName, int pixels)
        {
            FolderName = folderName;
            Pixels = pixels;
        }

        public string FolderName { get; private set; }

        public int Pixels { get; private set; }
    }

}
