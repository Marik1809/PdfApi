using PdfApi.Repository;

namespace PdfApi.Handling.Handlers
{
    public abstract class BasePdfFileHandler
    {
        protected IPdfFileRepository _repository;

        public BasePdfFileHandler(IPdfFileRepository repository)
        {
            _repository = repository;
        }
    }
}
