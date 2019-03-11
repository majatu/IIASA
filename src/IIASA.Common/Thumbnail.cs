using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace IIASA.Common
{
    public class Thumbnail
    {
        public static void Create(byte[] imageByteArray, Size thumbnailSize, string savePath)
        {
            using (var ms = new MemoryStream(imageByteArray))
            using (var image = Image.FromStream(ms))
            {
                if (thumbnailSize.Width >= thumbnailSize.Height && image.Size.Height < thumbnailSize.Height)
                    throw new Exception();

                if (thumbnailSize.Width <= thumbnailSize.Height && image.Size.Width < thumbnailSize.Width)
                    throw new Exception();

                var scalingRatio = CalculateScalingRatio(image.Size, thumbnailSize);

                var scaledWidth = (int)Math.Round(image.Size.Width * scalingRatio);
                var scaledHeight = (int)Math.Round(image.Size.Height * scalingRatio);

                var scaledLeft = (thumbnailSize.Width - scaledWidth) / 2;
                var scaledTop = (thumbnailSize.Height - scaledHeight) / 2;

                var destRect = new Rectangle(scaledLeft, scaledTop, scaledWidth, scaledHeight);

                using (Image thumbnail = new Bitmap(thumbnailSize.Width, thumbnailSize.Height))
                {
                    using (var thumbnailGraphics = Graphics.FromImage(thumbnail))
                    {
                        var wrapMode = new ImageAttributes();
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);

                        thumbnailGraphics.CompositingQuality = CompositingQuality.HighQuality;
                        thumbnailGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        thumbnailGraphics.SmoothingMode = SmoothingMode.HighQuality;

                        thumbnailGraphics.Clear(Color.White);

                        var srcX = 0;
                        var srcY = 0;
                        var srcWidth = image.Width;
                        var srcHeight = image.Height;

                        thumbnailGraphics.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, GraphicsUnit.Pixel, wrapMode);
                    }

                    var extension = Path.GetExtension(savePath);
                    extension = extension.ToLowerInvariant();

                    if (extension == ".jpg" || extension == ".jpeg")
                    {
                        var info = ImageCodecInfo.GetImageEncoders();
                        var encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);

                        thumbnail.Save(savePath, info[1], encoderParameters);

                        return;
                    }

                    thumbnail.Save(savePath);
                }
            }
        }

        #region private

        private static float CalculateScalingRatio(Size originalSize, Size thumbnailSize, bool reverse = false)
        {
            var originalAspectRatio = (float)originalSize.Width / originalSize.Height;
            var thumbnailAspectRatio = (float)thumbnailSize.Width / thumbnailSize.Height;

            var scalingRatioByWidth = (float)thumbnailSize.Width / originalSize.Width;
            var scalingRatioByHeight = (float)thumbnailSize.Height / originalSize.Height;

            if (originalAspectRatio >= thumbnailAspectRatio)
                return !reverse ? scalingRatioByWidth : scalingRatioByHeight;

            return !reverse ? scalingRatioByHeight : scalingRatioByWidth;
        }

        #endregion
    }
}
