using FluentValidation;
using PdfApi.Handling.Commands;

namespace PdfApi.Web.Infrastructure.Validation
{
    public class DeletePdfFileCommandValidator : AbstractValidator<DeletePdfFileCommand>
    {
        public DeletePdfFileCommandValidator()
        {
            RuleFor(c => c.FileName).NotEmpty();
        }
    }
}
