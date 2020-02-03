using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace PdfApi.Web.Infrastructure.Validation
{
    public class FormFileValidator : AbstractValidator<FormFile>
    {
        private const string _pdfExtension = ".pdf";
        private const string _pdfFlag = "%PDF-";
        private const long _maxBytes = 5242880;

        public FormFileValidator() 
        {
            RuleFor(f => f.Length)
                .GreaterThan(0)
                .LessThanOrEqualTo(_maxBytes)
                .WithMessage(f => $"File '{f.FileName}' has size more than 5 Mb.");

            RuleFor(f => f)        
                .Must(f => HasPdfExtension(f))
                .WithMessage(f => $"'{f.FileName}' has not suitable for pdf file extension.")
                .Must(f => IsValidPdf(f))
                .WithMessage(f => $"'{f.FileName}' is not valid PDF file.");                       
        }

        private bool HasPdfExtension(FormFile file)
            => Path.GetExtension(file.FileName) == _pdfExtension;

        private bool IsValidPdf(IFormFile file)
        {
            var pdfBytes = Encoding.ASCII.GetBytes(_pdfFlag);
            var len = pdfBytes.Length;
            var buffer = new byte[len];
            var remaining = len;
            var position = 0;

            using (var stream = file.OpenReadStream())
            {
                while (remaining > 0)
                {
                    var amtRead = stream.Read(buffer, position, remaining);

                    if (amtRead == 0)
                    {
                        return false;
                    }

                    remaining -= amtRead;
                    position += amtRead;
                }

                stream.Seek(0, SeekOrigin.Begin);
            }
            return pdfBytes.SequenceEqual(buffer);
        }

    }
}
