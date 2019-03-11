using System.Collections.Generic;
using System.Threading.Tasks;
using IIASA.Common.Enums;

namespace IIASA.Services.Core.Interfaces
{
    public interface IImageService
    {
        Task<int> UploadImage(byte[] byteArray, string fileName, string contentType);

        Task<ImageModel> GetImage(int imageId, ImageSize size = ImageSize.Unknown);

        Task<ImageExif> GetImageMetaData(int imageId);

        Task<ImageModel> GetImage(int imageId, int pixels);
    }

    #region models

    public class ImageModel
    {
        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string FileType { get; set; }
    }

    public class ImageExif
    {
        public ImageExif()
        {
            Dirs = new List<ImageExifDir>();
        }

        public List<ImageExifDir> Dirs { get; set; }
    }

    public class ImageExifDir
    {
        public ImageExifDir()
        {
            Tags = new List<ImageExifTag>();
        }

        public List<ImageExifTag> Tags { get; set; }

        public string Name { get; set; }
    }

    public class ImageExifTag
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }

    #endregion
}

