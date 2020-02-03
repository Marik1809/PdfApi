using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PdfApi.Handling.Commands;
using PdfApi.Handling.Queries;
using System.IO;
using System.Threading.Tasks;

namespace PdfApi.Web.Controllers
{
    [ApiController]
    [Route("api/Pdf")]
    public class PdfFileController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _environment;

        public PdfFileController(IMediator mediator, IWebHostEnvironment environment)
        {
            _mediator = mediator;
            _environment = environment;
        }

        /// <summary>
        /// Get ordered and paginted pdf file list.
        /// </summary>
        /// <remarks>
        /// Query parameters:
        /// Page - current page number. REQUIRED.
        /// ItemsPerPage - items returned per page. REQUIRED.
        /// OrderBy - property to order the list by. REQUIRED. Supported values: Name or Size (case-insensitive).
        /// OrederByAscending - specifies ordering direction. (default - false).
        /// </remarks>
        [HttpGet]
        [Produces("application/json")]
        [Route("getList")]
        public async Task<IActionResult> GetList([FromQuery] GetPdfFileListQuery getListCommand)
        {
            var result = await _mediator.Send(getListCommand);

            return result.ToActionResult();
        }

        /// <summary>
        /// Upload Pdf file. (Duplicate file names not supported.)
        /// </summary>
        /// <param name="file">File to upload. Should be valid pdf with size less or equal 5 mb.</param>
        [HttpPost]
        [Produces("application/json")]
        [Route("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if(file is null)
            {
                return new ObjectResult("File not specified")
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            var result = await _mediator.Send(
                new UploadPdfFileCommand
                {
                    File = file,
                    UploadFolder = Path.Combine(_environment.ContentRootPath, "PdfFiles")
                });

            return result.ToActionResult();
        }

        /// <summary>
        /// Delete Pdf file (by name).
        /// </summary>
        [HttpDelete]
        [Produces("application/json")]
        [Route("delete")]
        public async Task<IActionResult> Delete([FromQuery] DeletePdfFileCommand deleteCommand)
        {
            var result = await _mediator.Send(deleteCommand);

            return result.ToActionResult();
        }
    }
}
