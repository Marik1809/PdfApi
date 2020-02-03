using FluentValidation;
using PdfApi.Handling.Queries;
using PdfApi.Model;

namespace PdfApi.Web.Infrastructure.Validation
{
    public class GetPdfFileListQueryValidator : AbstractValidator<GetPdfFileListQuery>
    {
        public GetPdfFileListQueryValidator()
        {
            RuleFor(m => m.OrderBy)
               .NotEmpty()
               .Must(v => v.ToLower().Trim() == nameof(PdfFileModel.Name).ToLower() || v.ToLower().Trim() == nameof(PdfFileModel.Size).ToLower())
               .WithMessage(m => $"'{m.OrderBy}' is not valid value for 'OrderBy' parameter. Supported values: '{nameof(PdfFileModel.Name)}', '{nameof(PdfFileModel.Size)}'");

            RuleFor(m => m.ItemsPerPage)
                .GreaterThan(0)
                .WithMessage(m => $"'{m.ItemsPerPage}' is not valid value for 'ItemsPerPage' parameter");

            RuleFor(m => m.Page)
                .GreaterThan(0)
                .WithMessage(m => $"'{m.Page}' is not valid value for 'Page' parameter");
        }
    }
}
