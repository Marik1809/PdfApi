using MediatR;
using PdfApi.Handling.Queries;
using PdfApi.Model;
using PdfApi.Repository;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using static PdfApi.Handling.HandlerResult<PdfApi.Model.PdfFileListModel>;

namespace PdfApi.Handling.Handlers
{
    public class GetPdfFileListQueryHandler
       : BasePdfFileHandler, IRequestHandler<GetPdfFileListQuery, HandlerResult<PdfFileListModel>>
    {
        public GetPdfFileListQueryHandler(IPdfFileRepository repository) 
            : base(repository)
        { }

        public async Task<HandlerResult<PdfFileListModel>> Handle(GetPdfFileListQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = new PdfFileListModel
                {
                    Page = request.Page,
                    ItemsPerPage = request.ItemsPerPage,
                    FilesTotal = await _repository.CountAsync(),
                    Files = await _repository.GetAsync(
                          request.Page,
                          request.ItemsPerPage,
                          GenerateOrdering(request.OrderBy, request.OrderByAscending))
                };

                return CreateSuccessResult(result);
            }
            catch(Exception ex)
            {
                return CreateFailureResult("Failed to load Pdf list", ex);
            }
        }

        private Func<IQueryable<PdfFileModel>, IOrderedQueryable<PdfFileModel>> GenerateOrdering(
            string orderBy, 
            bool orderByAscending)
        {
            var formattedOrderBy = orderBy.Trim().ToLower();

            if (formattedOrderBy == nameof(PdfFileModel.Name).ToLower() && !orderByAscending)
            {
                return i => i.OrderByDescending(m => m.Name);
            }

            if(formattedOrderBy == nameof(PdfFileModel.Size).ToLower())
            {
                return i => orderByAscending
                       ? i.OrderBy(m => m.Size)
                       : i.OrderByDescending(m => m.Size);
            }

            return i => i.OrderBy(m => m.Name);
        }
    }
}
