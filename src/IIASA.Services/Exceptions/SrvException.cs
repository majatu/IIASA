using System;
using IIASA.Common.Extensions;

namespace IIASA.Services.Exceptions
{
    public class SrvException : Exception
    {
        public SrvException(string message)
            : base(message)
        {
        }

        public SrvException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class SrvValidationException : SrvException
    {
        public SrvValidationException(string message)
            : base(message)
        {
        }

        public SrvValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    #region by services

    public class SrvImageException : SrvException
    {
        public enum Reason
        {
            Unknown = 0,

            InvalidByteArray = 1,

            InvalidContentType = 2,

            InvalidFileName = 3,

            InvalidFileType = 4,

            ImageNotFound = 5,

            ImageMetaDataEmpty = 6,

            ImageDirectoryNotFound = 7,

            ImageFileNotFound = 8,

            InvalidCustomThumbnail = 9
        }

        public Reason ExReason { get; set; }

        public SrvImageException(Reason reason)
            : base(reason.GetDisplayName())
        {
            ExReason = reason;
        }

        public SrvImageException(Reason reason, string message)
            : base(string.IsNullOrEmpty(message) ? reason.ToString() : message)
        {
            ExReason = reason;
        }

        public SrvImageException(Reason reason, string message, Exception ex)
            : base(string.IsNullOrEmpty(message) ? reason.ToString() : message, ex)
        {
            ExReason = reason;
        }
    }

    #endregion
}
