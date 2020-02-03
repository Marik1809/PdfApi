using MediatR;
using PdfApi.Handling.Commands;
using PdfApi.Model;
using PdfApi.Repository;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using static PdfApi.Handling.HandlerResult<PdfApi.Model.PdfFileModel>;

namespace PdfApi.Handling.Handlers
{
    public class PdfFileCommandHandler :
        BasePdfFileHandler,
        IRequestHandler<UploadPdfFileCommand, HandlerResult<PdfFileModel>>,
        IRequestHandler<DeletePdfFileCommand, HandlerResult<PdfFileModel>>
    {
        public PdfFileCommandHandler(IPdfFileRepository repository) 
            : base(repository)
        { }

        public async Task<HandlerResult<PdfFileModel>> Handle(UploadPdfFileCommand request, CancellationToken cancellationToken)
        {
            if(!Directory.Exists(request.UploadFolder))
            {
                Directory.CreateDirectory(request.UploadFolder);
            }

            var fileName = request.File.FileName;
            var fullPath = Path.Combine(request.UploadFolder, fileName);
            var uploadFailureMessage = $"Failed to upload file '{fileName}'.";

            try
            {              
                if (File.Exists(fullPath))
                {
                    return CreateFailureResult($"File with name '{fileName}' already exists on the server. Duplicate file names are not supported.");
                }

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await request.File.CopyToAsync(fileStream);
                }

                var result = await _repository.AddAsync(
                    new PdfFileModel
                    {
                        Name = fileName,
                        Location = fullPath,
                        Size = request.File.Length
                    });

                await _repository.SaveAsync();
                return CreateSuccessResult(result);
            }
            catch(Exception ex)
            {
                if (ex is DbException)
                {
                    File.Delete(fullPath);
                }

                return CreateFailureResult(uploadFailureMessage, ex);
            }
        }

        public async Task<HandlerResult<PdfFileModel>> Handle(DeletePdfFileCommand request, CancellationToken cancellationToken)
        {
            IEnumerable<byte> fileCopy = null;
            string fileLocation = string.Empty;

            try
            {
                var fileName = request.FileName.Trim();
                var fileModel = await _repository.DeleteAsync(fileName);

                if(fileModel is null)
                {
                    return CreateFailureResult($"File record with name '{fileName}' does not exist in the database");
                }

                fileLocation = fileModel.Location;

                if(!File.Exists(fileLocation))
                {
                    await _repository.SaveAsync();
                    return CreateFailureResult($"File '{fileName}' does not exist on the server.");
                }

                fileCopy = File.ReadAllBytes(fileLocation);
                File.Delete(fileModel.Location);
                await _repository.SaveAsync();

                return CreateSuccessResult(fileModel);
            }
            catch(Exception ex)
            {
                if(
                    ex is DbException &&
                    !string.IsNullOrEmpty(fileLocation) &&
                    !File.Exists(fileLocation) &&
                    fileCopy != null)
                {
                    File.WriteAllBytes(fileLocation, fileCopy.ToArray());
                }

                return CreateFailureResult($"Failed to delete file '{request.FileName}'", ex);
            }
        }
    }
}
