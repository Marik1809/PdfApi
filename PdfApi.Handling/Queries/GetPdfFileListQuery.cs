using MediatR;
using PdfApi.Model;

namespace PdfApi.Handling.Queries
{
    public class GetPdfFileListQuery : IRequest<HandlerResult<PdfFileListModel>>
    {
        public int Page { get; set; }
        public int ItemsPerPage { get; set; }
        public string OrderBy { get; set; }
        public bool OrderByAscending { get; set; }
    }
}
