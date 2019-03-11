using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using IIASA.Api.Global.Response;
using IIASA.Common.Enums;
using IIASA.Services.Core.Interfaces;
using IIASA.Services.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IIASA.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/image")]
    [ApiController]
    public class ImageController : BaseController<ImageController>
    {
        private readonly IImageService _imageService;
   
        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        #region UploadImage

        /// <summary>
        /// Upload image.
        /// </summary>
        /// <param name="file">File data.</param>
        /// <returns>Id of created image.</returns>
        /// <response code="200" cref="ValueResponseModel{T}">Id of created image.</response>
        /// <response code="400" cref="ValidationFailedResultModel">Bad input params.</response>
        /// <response code="422" cref="ErrorResponseModel">Reason.</response>
        [ProducesResponseType(typeof(ValueResponseModel<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationFailedResultModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseModel), StatusCodes.Status422UnprocessableEntity)]
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ValueResponseModel<int>>> UploadImage([Required]IFormFile file)
        {
            if (file == null)
                return StatusCode(StatusCodes.Status422UnprocessableEntity, "File is empty.");

            byte[] fileBytes;

            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                fileBytes = ms.ToArray();
            }

            int imageId;

            try
            {
                imageId = await _imageService.UploadImage(fileBytes, file.FileName, file.ContentType);
            }
            catch (SrvImageException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new ErrorResponseEnumModel<SrvImageException.Reason>(ex.ExReason));
            }
            catch (SrvException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new ErrorResponsePredefinedModel(ex.Message));
            }

            return Ok(new ValueResponseModel<int>(imageId));
        }

        #endregion

        #region GetImage

        /// <summary>
        /// Get image by custom size.
        /// </summary>
        /// <param name="imageId">Id of image.</param>
        /// <param name="size">Size of image.</param>
        /// <returns>Thumbnail image.</returns>
        /// <response code="200" cref="File">Image.</response>
        /// <response code="400" cref="ValidationFailedResultModel">Bad input params.</response>
        /// <response code="422" cref="ErrorResponseModel">Reason.</response>
        [ProducesResponseType(typeof(File), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationFailedResultModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseModel), StatusCodes.Status422UnprocessableEntity)]
        [HttpGet("show")]
        public async Task<IActionResult> GetImage([Required]int imageId, ImageSize size = ImageSize.Unknown)
        {
            ImageModel model;

            try
            {
                model = await _imageService.GetImage(imageId);
            }
            catch (SrvImageException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new ErrorResponseEnumModel<SrvImageException.Reason>(ex.ExReason));
            }
            catch (SrvException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new ErrorResponsePredefinedModel(ex.Message));
            }

            var cd = new ContentDisposition
            {
                FileName = model.FileName,
                Inline = true
            };

            Response.Headers.Add("Content-Disposition", cd.ToString());

            var contents = System.IO.File.ReadAllBytes(model.FilePath);

            return File(contents, model.FileType);
        }

        #endregion

        #region GetImageMetaData

        /// <summary>
        /// Get image metadata.
        /// </summary>
        /// <param name="imageId">Id of image.</param>
        /// <returns>Image metadata.</returns>
        /// <response code="200" cref="ImageExif">Metadata model.</response>
        /// <response code="400" cref="ValidationFailedResultModel">Bad input params.</response>
        /// <response code="422" cref="ErrorResponseModel">Reason.</response>
        [ProducesResponseType(typeof(ImageExif), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationFailedResultModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseModel), StatusCodes.Status422UnprocessableEntity)]
        [HttpGet("metadata")]
        public async Task<ActionResult<ImageExif>> GetImageMetaData([Required]int imageId)
        {
            ImageExif metadata;

            try
            {
                metadata = await _imageService.GetImageMetaData(imageId);
            }
            catch (SrvImageException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new ErrorResponseEnumModel<SrvImageException.Reason>(ex.ExReason));
            }
            catch (SrvException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new ErrorResponsePredefinedModel(ex.Message));
            }

            return Ok(metadata);
        }

        #endregion

        #region GetImageCustomSize

        /// <summary>
        /// Get image by custom size.
        /// </summary>
        /// <param name="imageId">Id of image.</param>
        /// <param name="pixels">Pixels.</param>
        /// <returns>Thumbnail image.</returns>
        /// <response code="200" cref="File">Image.</response>
        /// <response code="400" cref="ValidationFailedResultModel">Bad input params.</response>
        /// <response code="422" cref="ErrorResponseModel">Reason.</response>
        [ProducesResponseType(typeof(File), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationFailedResultModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseModel), StatusCodes.Status422UnprocessableEntity)]
        [HttpGet("show_custom_size")]
        public async Task<IActionResult> GetImageCustomSize([Required]int imageId, [Required]int pixels)
        {
            ImageModel model;

            try
            {
                model = await _imageService.GetImage(imageId, pixels);
            }
            catch (SrvImageException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new ErrorResponseEnumModel<SrvImageException.Reason>(ex.ExReason));
            }
            catch (SrvException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new ErrorResponsePredefinedModel(ex.Message));
            }

            var cd = new ContentDisposition
            {
                FileName = model.FileName,
                Inline = true
            };

            Response.Headers.Add("Content-Disposition", cd.ToString());

            var contents = System.IO.File.ReadAllBytes(model.FilePath);

            return File(contents, model.FileType);
        }

        #endregion
    }
}
