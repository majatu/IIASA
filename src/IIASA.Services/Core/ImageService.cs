using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IIASA.Common;
using IIASA.Common.Enums;
using IIASA.Common.Extensions;
using IIASA.Services.Context;
using IIASA.Services.Core.Interfaces;
using IIASA.Services.Exceptions;
using IIASA.Services.Providers.Interfaces;
using MetadataExtractor;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IIASA.Services.Core
{
    public class ImageService : BaseService<ImageService>, IImageService
    {
        private ExtendedDbContext _context;
        private IPathsProvider _pathsProvider;

        public ImageService(ExtendedDbContext context, IPathsProvider pathsProvider)
        {
            _context = context;
            _pathsProvider = pathsProvider;
        }

        public async Task<int> UploadImage(byte[] byteArray, string fileName, string contentType)
        {
            fileName = fileName.TrimOrEmpty();
            contentType = contentType.TrimOrEmpty();

            if (byteArray == null || byteArray.Length == 0)
                throw new SrvImageException(SrvImageException.Reason.InvalidByteArray);

            if (string.IsNullOrEmpty(contentType))
                throw new SrvImageException(SrvImageException.Reason.InvalidContentType);

            if (string.IsNullOrEmpty(fileName))
                throw new SrvImageException(SrvImageException.Reason.InvalidFileName);

            if (!MimeAssistant.MimeTypesDictionary.Any(c => c.ContentType == contentType && c.Type == MimeType.Image))
                throw new SrvImageException(SrvImageException.Reason.InvalidFileType);

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var guid = Guid.NewGuid().ToString();

            var fileNameToSave = guid + extension;

            int imageId;

            {
                var imageToAdd = new Repository.Entities.Image
                {
                    FileName = fileNameToSave,
                    FileType = contentType,
                    MetaData = null,
                    DateCreated = DateTimeOffset.UtcNow
                };

                imageId = await _context.AddImage(imageToAdd);
            }

            #region original

            {
                var path = Path.Combine(_pathsProvider.ImagePath, "original", fileNameToSave);

                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(path));

                using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                }

                #region metadata

                {
                    try
                    {
                        var metadata = await GetMetaData(path);
                        var image = await _context.GetImageById(imageId);
                        image.MetaData = JsonConvert.SerializeObject(metadata);

                        await _context.UpdateImage(image);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Failed to extract or save metadata.");
                    }
                }

                #endregion
            }

            #endregion

            #region thumbnails

            var @enum = Enum.GetValues(typeof(ImageSize)).Cast<ImageSize>().ToList();

            foreach (var dr in @enum)
            {
                var attr = dr.GetAttribute<ImageSizeExtAttribute>();

                if (attr == null)
                    continue;

                var path = Path.Combine(_pathsProvider.ImagePath, attr.FolderName.ToLowerInvariant(), fileNameToSave);

                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(path));

                try
                {
                    var pixels = attr.Pixels;
                    Thumbnail.Create(byteArray, new Size(pixels, pixels), path);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to make thumbnail.");
                }
            }

            #endregion

            return imageId;
        }

        public async Task<ImageModel> GetImage(int imageId, ImageSize size = ImageSize.Unknown)
        {
            var image = await _context.GetImageById(imageId);

            if (image == null)
                throw new SrvImageException(SrvImageException.Reason.ImageNotFound);

            var di = new DirectoryInfo(_pathsProvider.ImagePath);

            if (!di.Exists)
                throw new SrvImageException(SrvImageException.Reason.ImageDirectoryNotFound);

            var folderName = size == ImageSize.Unknown ? "original" : size.GetAttribute<ImageSizeExtAttribute>().FolderName.ToLowerInvariant();
            var fullPath = Path.Combine(_pathsProvider.ImagePath, folderName, image.FileName);
            var fileInfo = new FileInfo(fullPath);

            if (!fileInfo.Exists)
                throw new SrvImageException(SrvImageException.Reason.ImageFileNotFound);

            return new ImageModel
            {
                FileName = image.FileName,
                FileType = image.FileType,
                FilePath = fullPath
            };
        }

        public async Task<ImageExif> GetImageMetaData(int imageId)
        {
            var image = await _context.GetImageById(imageId);

            if (image == null)
                throw new SrvImageException(SrvImageException.Reason.ImageNotFound);

            if(string.IsNullOrEmpty(image.MetaData))
                throw new SrvImageException(SrvImageException.Reason.ImageMetaDataEmpty);

            return JsonConvert.DeserializeObject<ImageExif>(image.MetaData);
        }

        public async Task<ImageModel> GetImage(int imageId, int pixels)
        {
            var image = await _context.GetImageById(imageId);

            if (image == null)
                throw new SrvImageException(SrvImageException.Reason.ImageNotFound);

            var di = new DirectoryInfo(_pathsProvider.ImagePath);

            if (!di.Exists)
                throw new SrvImageException(SrvImageException.Reason.ImageDirectoryNotFound);

            var @enum = Enum.GetValues(typeof(ImageSize)).Cast<ImageSize>().ToList();

            //if already exists
            foreach (var dr in @enum)
            {
                var attr = dr.GetAttribute<ImageSizeExtAttribute>();

                if (attr == null)
                    continue;

                if (attr.Pixels != pixels)
                    continue;

                var fullPath = Path.Combine(_pathsProvider.ImagePath, attr.FolderName.ToLowerInvariant(), image.FileName);
                var fileInfo = new FileInfo(fullPath);

                if (!fileInfo.Exists)
                    continue;
                
                return new ImageModel
                {
                    FileName = image.FileName,
                    FileType = image.FileType,
                    FilePath = fullPath
                };
                
            }

            var split = image.FileName.Split(".", StringSplitOptions.RemoveEmptyEntries);

            if (split.Length != 2)
                throw new Exception("Wrong file name at database.");

            var guid = split[0];
            var ext = split[1];

            var customFileName = $"{guid}_{pixels}.{ext}";

            //if already exists at custom folder
            {
                var fullPath = Path.Combine(_pathsProvider.ImagePath, "custom", customFileName);
                var fileInfo = new FileInfo(fullPath);

                if (fileInfo.Exists)
                {
                    return new ImageModel
                    {
                        FileName = customFileName,
                        FileType = image.FileType,
                        FilePath = fullPath
                    };
                }
            }

            //doesn't exist
            { 
                try
                {
                    var originalPath = Path.Combine(_pathsProvider.ImagePath, "original", image.FileName);
                    var customPath = Path.Combine(_pathsProvider.ImagePath, "custom", customFileName);

                    var byteArray = System.IO.File.ReadAllBytes(originalPath);

                    System.IO.Directory.CreateDirectory(Path.GetDirectoryName(customPath));

                    Thumbnail.Create(byteArray, new Size(pixels, pixels), customPath);

                    return new ImageModel
                    {
                        FileName = customFileName,
                        FileType = image.FileType,
                        FilePath = customPath
                    };
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to make custom thumbnail.");

                    throw new SrvImageException(SrvImageException.Reason.InvalidCustomThumbnail);
                }
            }
        }


        #region private

        private async Task<ImageExif> GetMetaData(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    throw new Exception("MetaData. Wrong file path.");

                if (!File.Exists(filePath))
                    throw new Exception("MetaData. File not found.");

                var result = new ImageExif();

                using (var stream = File.OpenRead(filePath))
                {
                    if (stream.CanSeek)
                        stream.Seek(0, SeekOrigin.Begin);

                    var dirs = ImageMetadataReader.ReadMetadata(stream);

                    foreach (var dir in dirs)
                    {
                        var resultDir = new ImageExifDir
                        {
                            Name = dir.Name
                        };

                        foreach (var tag in dir.Tags)
                        {
                            var resultTag = new ImageExifTag
                            {
                                Type = string.Format("0x{0:X4}", tag.Type),
                                Name = tag.Name,
                                Description = tag.Description ?? dir.GetString(tag.Type) + " (unable to formulate description).",
                            };

                            resultDir.Tags.Add(resultTag);
                        }

                        result.Dirs.Add(resultDir);
                    }
                }

                return result;
            }
            catch (ImageProcessingException ex)
            {
                Logger.LogError(ex, string.Format("MetaData. Image processing failed. {0}.", filePath));

                throw;
            }
            catch (IOException ex)
            {
                Logger.LogError(ex, string.Format("MetaData. IO Image processing failed. {0}.", filePath));

                throw;
            }
        }

        #endregion
    }
}