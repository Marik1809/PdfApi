using MediatR;
using Microsoft.AspNetCore.Http;
using PdfApi.Model;

namespace PdfApi.Handling.Commands
{
    public class UploadPdfFileCommand : IRequest<HandlerResult<PdfFileModel>>
    {
        public string UploadFolder { get; set; }
        public IFormFile File { get; set; }
    }
}
