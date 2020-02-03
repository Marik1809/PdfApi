using MediatR;
using PdfApi.Model;

namespace PdfApi.Handling.Commands
{
    public class DeletePdfFileCommand : IRequest<HandlerResult<PdfFileModel>>
    {
        public string FileName { get; set; }
    }
}
