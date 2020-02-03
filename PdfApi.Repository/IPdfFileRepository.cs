using PdfApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PdfApi.Repository
{
    public interface IPdfFileRepository
    {
        Task<PdfFileModel> AddAsync(PdfFileModel model);
        Task<int> CountAsync();
        Task<PdfFileModel> DeleteAsync(string fileName);
        Task<IEnumerable<PdfFileModel>> GetAsync(int page, int itemsPerPage, Func<IQueryable<PdfFileModel>, IOrderedQueryable<PdfFileModel>> orderBy);
        Task<int> SaveAsync();
    }
}